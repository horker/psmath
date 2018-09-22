using System;
using System.Collections.Generic;

namespace Horker.Math
{
    public abstract class DataFrameColumnInternal
    {
        public abstract int Count { get; }

        public abstract DataFrame Owner { get; internal set; }

        public abstract Type DataType { get; } 

        public abstract double[] ToDoubleArray();

        internal abstract IList<DataFrameColumnInternal> ToOneHot(int total, bool dropFirst = false);
        public abstract DataFrame ToDummyValues(string baseName, CodificationType codificationType = CodificationType.OneHot);

        public abstract object GetObject(int index);

        internal abstract void AddObject(object value);
        internal abstract void SetObject(int index, object value);
    }
}
