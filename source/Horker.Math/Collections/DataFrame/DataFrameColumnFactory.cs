using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Horker.Math
{
    public class DataFrameColumnFactory
    {
        public static DataFrameColumnBase Create(DataFrameType type, DataFrame owner)
        {
            switch (type)
            {
                case DataFrameType.Object: return new DataFrameColumn<Object>(owner);
                case DataFrameType.Decimal: return new DataFrameColumn<Decimal>(owner);
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

        public static DataFrameColumnBase Create(DataFrameType type, DataFrame owner, int capacity, int fillSize)
        {
            switch (type)
            {
                case DataFrameType.Object: return new DataFrameColumn<Object>(owner, capacity, fillSize);
                case DataFrameType.Decimal: return new DataFrameColumn<Decimal>(owner, capacity, fillSize);
                case DataFrameType.Double: return new DataFrameColumn<Double>(owner, capacity, fillSize);
                case DataFrameType.Single: return new DataFrameColumn<Single>(owner, capacity, fillSize);
                case DataFrameType.Int64: return new DataFrameColumn<Int64>(owner, capacity, fillSize);
                case DataFrameType.Int32: return new DataFrameColumn<Int32>(owner, capacity, fillSize);
                case DataFrameType.Int16: return new DataFrameColumn<Int16>(owner, capacity, fillSize);
                case DataFrameType.Byte: return new DataFrameColumn<Byte>(owner, capacity, fillSize);
                case DataFrameType.SByte: return new DataFrameColumn<SByte>(owner, capacity, fillSize);
                case DataFrameType.String: return new DataFrameColumn<String>(owner, capacity, fillSize);
                case DataFrameType.Boolean: return new DataFrameColumn<Boolean>(owner, capacity, fillSize);
            }

            throw new ArgumentException("Unknown DataFrameType");
        }

        public static DataFrameColumnBase Create(Type type, DataFrame owner)
        {
            return Create(DataFrameTypeHelper.GetDataFrameType(type), owner);
        }

        public static DataFrameColumnBase Create(Type type, DataFrame owner, int capacity, int fillSize)
        {
            return Create(DataFrameTypeHelper.GetDataFrameType(type), owner, capacity, fillSize);
        }

        public static DataFrameColumnBase CreateFromTypeName(string typeName, DataFrame owner)
        {
            if (typeName == "System.Object")
                return new DataFrameColumn<Object>(owner);
            if (typeName == "System.Decimal")
                return new DataFrameColumn<Decimal>(owner);
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

        public static DataFrameColumnBase CreateFromTypeName(string typeName, DataFrame owner, int capacity, int fillSize)
        {
            if (typeName == "System.Object")
                return new DataFrameColumn<Object>(owner, capacity, fillSize);
            if (typeName == "System.Decimal")
                return new DataFrameColumn<Decimal>(owner, capacity, fillSize);
            if (typeName == "System.Double")
                return new DataFrameColumn<Double>(owner, capacity, fillSize);
            if (typeName == "System.Single")
                return new DataFrameColumn<Single>(owner, capacity, fillSize);
            if (typeName == "System.Int64")
                return new DataFrameColumn<Int64>(owner, capacity, fillSize);
            if (typeName == "System.Int32")
                return new DataFrameColumn<Int32>(owner, capacity, fillSize);
            if (typeName == "System.Int16")
                return new DataFrameColumn<Int16>(owner, capacity, fillSize);
            if (typeName == "System.Byte")
                return new DataFrameColumn<Byte>(owner, capacity, fillSize);
            if (typeName == "System.SByte")
                return new DataFrameColumn<SByte>(owner, capacity, fillSize);
            if (typeName == "System.String")
                return new DataFrameColumn<String>(owner, capacity, fillSize);
            if (typeName == "System.Boolean")
                return new DataFrameColumn<Boolean>(owner, capacity, fillSize);

            return new DataFrameColumn<object>(owner, capacity, fillSize);
        }

        public static DataFrameColumnBase Clone(DataFrameColumnBase source)
        {
            switch (DataFrameTypeHelper.GetDataFrameType(source.DataType))
            {
                case DataFrameType.Object: return new DataFrameColumn<Object>((DataFrameColumn<Object>)(source));
                case DataFrameType.Decimal: return new DataFrameColumn<Decimal>((DataFrameColumn<Decimal>)(source));
                case DataFrameType.Double: return new DataFrameColumn<Double>((DataFrameColumn<Double>)(source));
                case DataFrameType.Single: return new DataFrameColumn<Single>((DataFrameColumn<Single>)(source));
                case DataFrameType.Int64: return new DataFrameColumn<Int64>((DataFrameColumn<Int64>)(source));
                case DataFrameType.Int32: return new DataFrameColumn<Int32>((DataFrameColumn<Int32>)(source));
                case DataFrameType.Int16: return new DataFrameColumn<Int16>((DataFrameColumn<Int16>)(source));
                case DataFrameType.Byte: return new DataFrameColumn<Byte>((DataFrameColumn<Byte>)(source));
                case DataFrameType.SByte: return new DataFrameColumn<SByte>((DataFrameColumn<SByte>)(source));
                case DataFrameType.String: return new DataFrameColumn<String>((DataFrameColumn<String>)(source));
                case DataFrameType.Boolean: return new DataFrameColumn<Boolean>((DataFrameColumn<Boolean>)(source));
            }

            throw new ArgumentException("Unknown DataFrameType");
        }
    }
}
