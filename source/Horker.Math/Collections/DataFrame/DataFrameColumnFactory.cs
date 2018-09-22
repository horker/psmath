using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Horker.Math
{
    public class DataFrameColumnFactory
    {
        public static DataFrameColumnInternal Create(DataFrameType type, DataFrame owner)
        {
            switch (type)
            {
                case DataFrameType.Object: return new DataFrameColumn<Object>(owner);
                case DataFrameType.Double: return new DataFrameColumn<Double>(owner);
                case DataFrameType.Single: return new DataFrameColumn<Single>(owner);
                case DataFrameType.Int64: return new DataFrameColumn<Int64>(owner);
                case DataFrameType.Int32: return new DataFrameColumn<Int32>(owner);
                case DataFrameType.Int16: return new DataFrameColumn<Int16>(owner);
                case DataFrameType.Byte: return new DataFrameColumn<Byte>(owner);
                case DataFrameType.SByte: return new DataFrameColumn<SByte>(owner);
                case DataFrameType.String: return new DataFrameColumn<String>(owner);
                case DataFrameType.Boolean: return new DataFrameColumn<Boolean>(owner);
            }

            throw new ArgumentException("Unknown DataFrameType");
        }

        public static DataFrameColumnInternal Create(DataFrameType type, DataFrame owner, int capacity, bool fill)
        {
            switch (type)
            {
                case DataFrameType.Object: return new DataFrameColumn<Object>(owner, capacity, fill);
                case DataFrameType.Double: return new DataFrameColumn<Double>(owner, capacity, fill);
                case DataFrameType.Single: return new DataFrameColumn<Single>(owner, capacity, fill);
                case DataFrameType.Int64: return new DataFrameColumn<Int64>(owner, capacity, fill);
                case DataFrameType.Int32: return new DataFrameColumn<Int32>(owner, capacity, fill);
                case DataFrameType.Int16: return new DataFrameColumn<Int16>(owner, capacity, fill);
                case DataFrameType.Byte: return new DataFrameColumn<Byte>(owner, capacity, fill);
                case DataFrameType.SByte: return new DataFrameColumn<SByte>(owner, capacity, fill);
                case DataFrameType.String: return new DataFrameColumn<String>(owner, capacity, fill);
                case DataFrameType.Boolean: return new DataFrameColumn<Boolean>(owner, capacity, fill);
            }

            throw new ArgumentException("Unknown DataFrameType");
        }

        public static DataFrameColumnInternal Create(Type type, DataFrame owner)
        {
            return Create(GetDataFrameType(type), owner);
        }

        public static DataFrameColumnInternal Create(Type type, DataFrame owner, int capacity, bool fill)
        {
            return Create(GetDataFrameType(type), owner, capacity, fill);
        }

        public static DataFrameColumnInternal CreateFromTypeName(string typeName, DataFrame owner)
        {
            if (typeName == "System.Object")
                return new DataFrameColumn<Object>(owner);
            if (typeName == "System.Double")
                return new DataFrameColumn<Double>(owner);
            if (typeName == "System.Single")
                return new DataFrameColumn<Single>(owner);
            if (typeName == "System.Int64")
                return new DataFrameColumn<Int64>(owner);
            if (typeName == "System.Int32")
                return new DataFrameColumn<Int32>(owner);
            if (typeName == "System.Int16")
                return new DataFrameColumn<Int16>(owner);
            if (typeName == "System.Byte")
                return new DataFrameColumn<Byte>(owner);
            if (typeName == "System.SByte")
                return new DataFrameColumn<SByte>(owner);
            if (typeName == "System.String")
                return new DataFrameColumn<String>(owner);
            if (typeName == "System.Boolean")
                return new DataFrameColumn<Boolean>(owner);

            return new DataFrameColumn<object>(owner);
        }

        public static DataFrameColumnInternal CreateFromTypeName(string typeName, DataFrame owner, int capacity, bool fill)
        {
            if (typeName == "System.Object")
                return new DataFrameColumn<Object>(owner, capacity, fill);
            if (typeName == "System.Double")
                return new DataFrameColumn<Double>(owner, capacity, fill);
            if (typeName == "System.Single")
                return new DataFrameColumn<Single>(owner, capacity, fill);
            if (typeName == "System.Int64")
                return new DataFrameColumn<Int64>(owner, capacity, fill);
            if (typeName == "System.Int32")
                return new DataFrameColumn<Int32>(owner, capacity, fill);
            if (typeName == "System.Int16")
                return new DataFrameColumn<Int16>(owner, capacity, fill);
            if (typeName == "System.Byte")
                return new DataFrameColumn<Byte>(owner, capacity, fill);
            if (typeName == "System.SByte")
                return new DataFrameColumn<SByte>(owner, capacity, fill);
            if (typeName == "System.String")
                return new DataFrameColumn<String>(owner, capacity, fill);
            if (typeName == "System.Boolean")
                return new DataFrameColumn<Boolean>(owner, capacity, fill);

            return new DataFrameColumn<object>(owner, capacity, fill);
        }

        public static DataFrameType GetDataFrameType(Type type)
        {
            if (type == typeof(Object)) return DataFrameType.Object;
            if (type == typeof(Double)) return DataFrameType.Double;
            if (type == typeof(Single)) return DataFrameType.Single;
            if (type == typeof(Int64)) return DataFrameType.Int64;
            if (type == typeof(Int32)) return DataFrameType.Int32;
            if (type == typeof(Int16)) return DataFrameType.Int16;
            if (type == typeof(Byte)) return DataFrameType.Byte;
            if (type == typeof(SByte)) return DataFrameType.SByte;
            if (type == typeof(Boolean)) return DataFrameType.Boolean;
            if (type == typeof(String)) return DataFrameType.String;

            throw new ArgumentException("Invalid type");
        }
    }
}
