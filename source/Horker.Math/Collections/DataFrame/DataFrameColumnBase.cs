using System;
using System.Collections;
using System.Collections.Generic;

namespace Horker.Math
{
    public abstract class DataFrameColumnBase : IEnumerable
    {
        // Common properties

        public abstract Type DataType { get; } 

        public abstract int Count { get; }

        public abstract DataFrame Owner { get; internal set; }

        // Internal object methods

        internal abstract object[] ToObjectArray();
        internal abstract double[] ToDoubleArray();
        internal abstract string[] ToStringArray();

        internal abstract IList<DataFrameColumnBase> ToOneHot(int total, bool dropFirst = false);

        internal abstract object GetObject(int index);

        internal abstract void AddObject(object value);
        internal abstract void SetObject(int index, object value);

        internal abstract void AddColumn(DataFrameColumnBase column);
        internal abstract void AddDefaultValues(int count);

        // Public methods

        public abstract IEnumerator GetEnumerator();

        public abstract DataFrame ToDummyValues(string baseName, CodificationType codificationType = CodificationType.OneHot);
    }
}
