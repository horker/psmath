using System;

namespace Horker.Math
{
    public enum DataFrameType
    {
        Object,
        Decimal,
        Double,
        Single,
        Float = Single,
        Int64,
        Int32,
        Int = Int32,
        Int16,
        Byte,
        SByte,
        String,
        Boolean
    }

    public class DataFrameTypeHelper
    {
        public static DataFrameType GetDataFrameType(Type type)
        {
            if (type == typeof(Object)) return DataFrameType.Object;
            if (type == typeof(Decimal)) return DataFrameType.Decimal;
            if (type == typeof(Double)) return DataFrameType.Double;
            if (type == typeof(Single)) return DataFrameType.Single;
            if (type == typeof(Int64)) return DataFrameType.Int64;
            if (type == typeof(Int32)) return DataFrameType.Int32;
            if (type == typeof(Int16)) return DataFrameType.Int16;
            if (type == typeof(Byte)) return DataFrameType.Byte;
            if (type == typeof(SByte)) return DataFrameType.SByte;
            if (type == typeof(String)) return DataFrameType.String;
            if (type == typeof(Boolean)) return DataFrameType.Boolean;

            throw new ArgumentException("Unsupported type for DataFrame: " + type.FullName);
        }
    }
}
