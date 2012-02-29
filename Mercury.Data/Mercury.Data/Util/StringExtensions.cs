using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Reflection;
using System.Collections;

namespace Mercury.Data.Util
{
    public static class StringExtensions
    {
        /// <summary>
        /// Returns a string and replaces named place holders with object values
        /// </summary>
        /// <remarks>Originally from Scott Hanselman's Blog: http://www.hanselman.com/blog/ASmarterOrPureEvilToStringWithExtensionMethods.aspx </remarks>
        /// <param name="anObject"></param>
        /// <param name="aFormat"></param>
        /// <returns></returns>
        public static string ToStringWithFormat(this object anObject, string aFormat)
        {
            return StringExtensions.ToStringWithFormat(anObject, aFormat, null);
        }

        public static string ToStringWithFormat(this object anObject, string aFormat, IFormatProvider formatProvider)
        {
            if (anObject == null)//Can't merge null object. Be nice and return original format string.
                return aFormat;

            StringBuilder sb = new StringBuilder();
            Type type = anObject.GetType();
            //Old pattern: @"({)([^}]+)(})" - Doesn't handle nested brackets
            //New pattern:"({)((?:[^{}]|{[^{}]*})*)(})" - Handles ONE LEVEL of nested brackets
            Regex reg = new Regex(@"({)((?:[^{}]|{[^{}]*})*)(})", RegexOptions.IgnoreCase);
            MatchCollection mc = reg.Matches(aFormat);
            int startIndex = 0;
            foreach (Match m in mc)
            {
                Group g = m.Groups[2]; //it's second in the match between { and }
                int length = g.Index - startIndex - 1;
                sb.Append(aFormat.Substring(startIndex, length));

                string toGet = String.Empty;
                string toFormat = String.Empty;
                int formatIndex = g.Value.IndexOf(":"); //formatting would be to the right of a :
                if (formatIndex == -1) //no formatting, no worries
                {
                    toGet = g.Value;
                }
                else //pickup the formatting
                {
                    toGet = g.Value.Substring(0, formatIndex);
                    toFormat = g.Value.Substring(formatIndex + 1);
                }

                //first try properties
                PropertyInfo retrievedProperty = type.GetProperty(toGet);
                Type retrievedType = null;
                object retrievedObject = null;
                if (retrievedProperty != null)
                {
                    retrievedType = retrievedProperty.PropertyType;
                    retrievedObject = retrievedProperty.GetValue(anObject, null);
                }
                else //try fields
                {
                    FieldInfo retrievedField = type.GetField(toGet);
                    if (retrievedField != null)
                    {
                        retrievedType = retrievedField.FieldType;
                        retrievedObject = retrievedField.GetValue(anObject);
                    }
                }

                if (retrievedType != null) //Cool, we found something
                {
                    string result = String.Empty;
                    if (toFormat == String.Empty) //no format info
                    {
                        if (retrievedObject is ICollection)
                        {
                            foreach (var item in (retrievedObject as ICollection))
                            {
                                //In this branch toFormat is blank, so just call toString on
                                //each object in collection (ex:{Items})
                                result += item.ToString();
                            }
                        }
                        else
                            result = retrievedObject.ToString();
                    }
                    else //format info
                    {
                        if (retrievedObject is ICollection) //Process first level collection
                        {
                            foreach (var item in (retrievedObject as ICollection))
                            {
                                //In this branch toFormat contains nested property name, so
                                //make recursive call to ToStringWithFormat to process property value
                                //(ex: {Items: {PropertyName}})
                                result += item.ToStringWithFormat(toFormat);
                            }
                        }
                        else
                            result = String.Format(formatProvider, toFormat, retrievedObject);
                    }
                    sb.Append(result);
                }
                else //didn't find a property with that name, so be gracious and put it back
                {
                    sb.Append("{");
                    sb.Append(g.Value);
                    sb.Append("}");
                }
                startIndex = g.Index + g.Length + 1;
            }
            if (startIndex < aFormat.Length) //include the rest (end) of the string
            {
                sb.Append(aFormat.Substring(startIndex));
            }
            return sb.ToString();
        }

        /// <summary>
        /// Removes HTML tags from string
        /// </summary>
        /// <remarks>Uses character arrary to optimize tag stripping performance</remarks>
        /// <see cref="http://dotnetperls.com/remove-html-tags"/>
        /// <param name="htmlString"></param>
        /// <returns>Plain text string with no HTML tags</returns>
        public static string ToPlainText(this string htmlString)
        {
            char[] array = new char[htmlString.Length];
            int arrayIndex = 0;
            bool inside = false;

            for (int i = 0; i < htmlString.Length; i++)
            {
                char let = htmlString[i];
                if (let == '<')
                {
                    inside = true;
                    continue;
                }
                if (let == '>')
                {
                    inside = false;
                    continue;
                }
                if (!inside)
                {
                    array[arrayIndex] = let;
                    arrayIndex++;
                }
            }
            return new string(array, 0, arrayIndex);
        }
        
        
        /// <summary>
        /// Formats string by replacing named placeholders with object values
        /// </summary>
        /// <remarks>Originally from: http://james.newtonking.com/archive/2008/03/29/formatwith-2-0-string-formatting-with-named-variables.aspx</remarks>
        /// <param name="format"></param>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string FormatWith(this string format, object source)
        {
            return FormatWith(format, null, source);
        }

        public static string FormatWith(this string format, IFormatProvider provider, object source)
        {
            if (format == null)
                throw new ArgumentNullException("format");

            Regex r = new Regex(@"(?<start>\{)+(?<property>[\w\.\[\]]+)(?<format>:[^}]+)?(?<end>\})+",
              RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);

            List<object> values = new List<object>();
            string rewrittenFormat = r.Replace(format, delegate(Match m)
            {
                Group startGroup = m.Groups["start"];
                Group propertyGroup = m.Groups["property"];
                Group formatGroup = m.Groups["format"];
                Group endGroup = m.Groups["end"];

                values.Add((propertyGroup.Value == "0")
                  ? source
                  : DataBinderLite.Eval(source, propertyGroup.Value));

                return new string('{', startGroup.Captures.Count) + (values.Count - 1) + formatGroup.Value
                  + new string('}', endGroup.Captures.Count);
            });

            return string.Format(provider, rewrittenFormat, values.ToArray());
        }
    }
}
