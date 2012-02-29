using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Reflection;
using System.Globalization;
using System.Collections;
using System.Runtime;

namespace Mercury.Data.Util
{
    public class DataBinderLite
    {
            // Fields
            private static bool enableCaching = true;
            private static readonly char[] expressionPartSeparator = new char[] { '.' };
            private static readonly char[] indexExprEndChars = new char[] { ']', ')' };
            private static readonly char[] indexExprStartChars = new char[] { '[', '(' };
            private static readonly ConcurrentDictionary<Type, PropertyDescriptorCollection> propertyCache = new ConcurrentDictionary<Type, PropertyDescriptorCollection>();

            // Methods
            public static object Eval(object container, string expression)
            {
                if (expression == null)
                {
                    throw new ArgumentNullException("expression");
                }
                expression = expression.Trim();
                if (expression.Length == 0)
                {
                    throw new ArgumentNullException("expression");
                }
                if (container == null)
                {
                    return null;
                }
                string[] expressionParts = expression.Split(expressionPartSeparator);
                return Eval(container, expressionParts);
            }

            private static object Eval(object container, string[] expressionParts)
            {
                object propertyValue = container;
                for (int i = 0; (i < expressionParts.Length) && (propertyValue != null); i++)
                {
                    string propName = expressionParts[i];
                    if (propName.IndexOfAny(indexExprStartChars) < 0)
                    {
                        propertyValue = GetPropertyValue(propertyValue, propName);
                    }
                    else
                    {
                        propertyValue = GetIndexedPropertyValue(propertyValue, propName);
                    }
                }
                return propertyValue;
            }

            public static string Eval(object container, string expression, string format)
            {
                object obj2 = Eval(container, expression);
                if ((obj2 == null) || (obj2 == DBNull.Value))
                {
                    return string.Empty;
                }
                if (string.IsNullOrEmpty(format))
                {
                    return obj2.ToString();
                }
                return string.Format(format, obj2);
            }

            public static object GetDataItem(object container)
            {
                bool flag;
                return GetDataItem(container, out flag);
            }

            public static object GetDataItem(object container, out bool foundDataItem)
            {
                if (container == null)
                {
                    foundDataItem = false;
                    return null;
                }
                IDataItemContainerLite container2 = container as IDataItemContainerLite;
                if (container2 != null)
                {
                    foundDataItem = true;
                    return container2.DataItem;
                }
                string name = "DataItem";
                PropertyInfo property = container.GetType().GetProperty(name, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                if (property == null)
                {
                    foundDataItem = false;
                    return null;
                }
                foundDataItem = true;
                return property.GetValue(container, null);
            }

            public static object GetIndexedPropertyValue(object container, string expr)
            {
                if (container == null)
                {
                    throw new ArgumentNullException("container");
                }
                if (string.IsNullOrEmpty(expr))
                {
                    throw new ArgumentNullException("expr");
                }
                object obj2 = null;
                bool flag = false;
                int length = expr.IndexOfAny(indexExprStartChars);
                int num2 = expr.IndexOfAny(indexExprEndChars, length + 1);
                if (((length < 0) || (num2 < 0)) || (num2 == (length + 1)))
                {
                    throw new ArgumentException(String.Format("DataBinder Error: Invalid indexed epxression \"{0}\"", expr));
                }
                string propName = null;
                object obj3 = null;
                string s = expr.Substring(length + 1, (num2 - length) - 1).Trim();
                if (length != 0)
                {
                    propName = expr.Substring(0, length);
                }
                if (s.Length != 0)
                {
                    if (((s[0] == '"') && (s[s.Length - 1] == '"')) || ((s[0] == '\'') && (s[s.Length - 1] == '\'')))
                    {
                        obj3 = s.Substring(1, s.Length - 2);
                    }
                    else if (char.IsDigit(s[0]))
                    {
                        int num3;
                        flag = int.TryParse(s, NumberStyles.Integer, CultureInfo.InvariantCulture, out num3);
                        if (flag)
                        {
                            obj3 = num3;
                        }
                        else
                        {
                            obj3 = s;
                        }
                    }
                    else
                    {
                        obj3 = s;
                    }
                }
                if (obj3 == null)
                {
                    throw new ArgumentException(String.Format("DataBinder Error: Invalid indexed epxression \"{0}\"",expr));
                }
                object propertyValue = null;
                if ((propName != null) && (propName.Length != 0))
                {
                    propertyValue = GetPropertyValue(container, propName);
                }
                else
                {
                    propertyValue = container;
                }
                if (propertyValue == null)
                {
                    return obj2;
                }
                Array array = propertyValue as Array;
                if ((array != null) && flag)
                {
                    return array.GetValue((int)obj3);
                }
                if ((propertyValue is IList) && flag)
                {
                    return ((IList)propertyValue)[(int)obj3];
                }
                PropertyInfo info = propertyValue.GetType().GetProperty("Item", BindingFlags.Public | BindingFlags.Instance, null, null, new Type[] { obj3.GetType() }, null);
                if (info == null)
                {
                    throw new ArgumentException(String.Format("DataBinder Error: No indexed accessor for property \"{0}\"",propertyValue.GetType().FullName));
                }
                return info.GetValue(propertyValue, new object[] { obj3 });
            }

            public static string GetIndexedPropertyValue(object container, string propName, string format)
            {
                object indexedPropertyValue = GetIndexedPropertyValue(container, propName);
                if ((indexedPropertyValue == null) || (indexedPropertyValue == DBNull.Value))
                {
                    return string.Empty;
                }
                if (string.IsNullOrEmpty(format))
                {
                    return indexedPropertyValue.ToString();
                }
                return string.Format(format, indexedPropertyValue);
            }

            internal static PropertyDescriptorCollection GetPropertiesFromCache(object container)
            {
                if (!EnableCaching || (container is ICustomTypeDescriptor))
                {
                    return TypeDescriptor.GetProperties(container);
                }
                PropertyDescriptorCollection properties = null;
                Type type = container.GetType();
                if (!propertyCache.TryGetValue(type, out properties))
                {
                    properties = TypeDescriptor.GetProperties(type);
                    propertyCache.TryAdd(type, properties);
                }
                return properties;
            }

            public static object GetPropertyValue(object container, string propName)
            {
                if (container == null)
                {
                    throw new ArgumentNullException("container");
                }
                if (string.IsNullOrEmpty(propName))
                {
                    throw new ArgumentNullException("propName");
                }
                PropertyDescriptor descriptor = GetPropertiesFromCache(container).Find(propName, true);
                if (descriptor == null)
                {
                    throw new ApplicationException(String.Format("DataBinder did not find property \"{0}\" in type \"{1}\"", propName, container.GetType().FullName));
                }
                return descriptor.GetValue(container);
            }

            public static string GetPropertyValue(object container, string propName, string format)
            {
                object propertyValue = GetPropertyValue(container, propName);
                if ((propertyValue == null) || (propertyValue == DBNull.Value))
                {
                    return string.Empty;
                }
                if (string.IsNullOrEmpty(format))
                {
                    return propertyValue.ToString();
                }
                return string.Format(format, propertyValue);
            }

            internal static bool IsNull(object value)
            {
                if ((value != null) && !Convert.IsDBNull(value))
                {
                    return false;
                }
                return true;
            }

            // Properties
            public static bool EnableCaching
            {
                [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
                get
                {
                    return enableCaching;
                }
                set
                {
                    enableCaching = value;
                    if (!value)
                    {
                        propertyCache.Clear();
                    }
                }
            }        
    }
}
