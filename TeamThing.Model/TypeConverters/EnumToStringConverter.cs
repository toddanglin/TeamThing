using System;
using Telerik.OpenAccess.Data;

namespace TeamThing.Model.TypeConverters
{
    public class EnumToStringConverter<TEnum> : Telerik.OpenAccess.Data.AdoTypeConverter where TEnum : struct
    {
        public override Type DefaultType
        {
            get
            {
                return typeof(TEnum);
            }
        }
        public override bool CreateLiteralSql(ref DataHolder holder)
        {
            if (holder.NoValue)
            {
                holder.StringValue = this.NullValueSql;
                return false;
            }
            else
            {
                return true; // inidicating that ' are needed around, because it is a character column (VARCHAR)
            }
        }
        public override AdoTypeConverter Initialize(IDataColumn user, Type clr, IAdoTypeConverterRegistry registry, bool secondaryTable)
        {
            //first we need to ensure that the generic type specified is an Enum.
            if (!DefaultType.IsEnum)
                throw new System.ArgumentException( typeof(TEnum).Name + " is not an Enum");

            //second we need to make sure OpenAccess is looking for the Enum this converter handles.
            if (clr == typeof(TEnum))
            {
                return base.Initialize(user, clr, registry, secondaryTable);
            }
            return null;
        }
        public override object Read(ref DataHolder holder)
        {
            bool isNull = holder.Reader.IsDBNull(holder.Position);

            holder.NoValue = isNull;

            if (isNull)
            {
                holder.ObjectValue = 0;
            }
            else
            {
                string value = holder.Reader.GetValue(holder.Position).ToString();
                holder.ObjectValue = Enum.Parse(typeof(TEnum), value);
            }
            return (holder.Box) ? holder.ObjectValue : null;
        }
        public override void Write(ref DataHolder holder)
        {
            holder.Parameter.DbType = System.Data.DbType.StringFixedLength;
            if (!holder.NoValue)
            {
                string s = Enum.GetName(typeof(TEnum), holder.ObjectValue);
                //if (holder.Parameter.Size < s.Length)
                //{
                    holder.Parameter.Size = 25; //YIKES
                //}
                holder.Box = false;
                holder.Parameter.Value = s;
                holder.ObjectValue = s;
                holder.StringValue = s;
            }
        }
    }
}