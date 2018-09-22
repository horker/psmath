using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Accord.Math;

namespace Horker.Math
{
    public enum CodificationType
    {
        OneHotDropFirst,
        OneHot,
        Multilevel,
        Boolean
    }

    public class DataFrameColumn<T> : DataFrameColumnInternal, IEnumerable<T>
    {
        private DataFrame _owner;
        private List<T> _data;

        #region Properties

        public override int Count => _data.Count;

        public override DataFrame Owner
        {
            get { return _owner; }
            internal set { _owner = value; }
        }

        public T this[int index]
        {
            get => _data[index];
            set => _data[index] = value;
        }

        public override Type DataType => typeof(T);

        #endregion

        #region Override methods

        public override object GetObject(int index) => _data[index];

        internal override void AddObject(object value) => _data.Add((T)value);

        internal override void SetObject(int index, object value) { _data[index] = (T)value; }

        #endregion

        #region IEnumerable<T>

        public IEnumerator<T> GetEnumerator()
        {
            return _data.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _data.GetEnumerator();
            throw new NotImplementedException();
        }

        #endregion

        #region Constructors

        public DataFrameColumn(DataFrame owner)
        {
            _owner = owner;
            _data = new List<T>();
        }

        public DataFrameColumn(DataFrame owner, int capacity, bool fill)
        {
            _owner = owner;
            _data = new List<T>(capacity);

            if (fill)
            {
                for (var i = 0; i < capacity; ++i)
                    _data.Add(default(T));
            }
        }

        public DataFrameColumn(DataFrame owner, IEnumerable<T> data)
        {
            _owner = owner;
            _data = new List<T>();

            foreach (var d in data)
                _data.Add(d);
        }

        #endregion

        #region Manipulators

        internal void Add(T element)
        {
            _data.Add(element);
        }

        #endregion

        #region Conversions

        public override double[] ToDoubleArray()
        {
            var result = new double[Count];

            for (var i = 0; i < Count; ++i)
                result[i] = Converter.ToDouble(this[i]);

            return result;
        }

        public void ToDoubleArray(double[] dest, int offset = 0)
        {
            if (dest.Length + offset < Count)
                throw new ArgumentException("Destination array doesn't have enough size");

            for (var i = 0; i < Count; ++i)
                dest[i + offset] = Converter.ToDouble(this[i]);
        }

        public int[] ToIntArray()
        {
            var result = new int[Count];

            for (var i = 0; i < Count; ++i)
                result[i] = Convert.ToInt32(this[i]);

            return result;
        }

        internal override IList<DataFrameColumnInternal> ToOneHot(int total, bool dropFirst = false)
        {
            var result = new List<DataFrameColumnInternal>();

            for (var i = dropFirst ? 1 : 0; i < total; ++i)
            {
                var column = new DataFrameColumn<Int32>(null, Count, false);
                for (var j = 0; j < Count; ++j)
                    column.Add(Converter.ToInt(this[j]) == i ? 1 : 0);

                result.Add(column);
            }

            return result;
        }

        public override DataFrame ToDummyValues(string baseName, CodificationType codificationType = CodificationType.OneHot)
        {
            var df = new DataFrame();

            HashSet<object> set = new HashSet<object>(new ObjectKeyComparer());
            foreach (var e in _data)
            {
                set.Add(e);
            }

            var factors = new List<object>();
            foreach (var e in set)
            {
                factors.Add(e);
            }
            factors.Sort();

            var comparer = new ObjectKeyComparer();
            if (codificationType == CodificationType.Multilevel)
            {
                var values = new List<object>(Count);
                foreach (var e in _data)
                {
                    var index = factors.FindIndex((x) => comparer.Equals(x, e));
                    values.Add(index);
                }
                df.AddColumn(baseName, values);
            }
            else
            {
                bool excludeFirst = codificationType == CodificationType.OneHotDropFirst;
                bool asBoolean = codificationType == CodificationType.Boolean;

                var values = new List<object>[factors.Count - (excludeFirst ? 1 : 0)];
                for (var i = 0; i < values.Length; ++i)
                {
                    values[i] = new List<object>(Count);
                }

                foreach (var e in _data)
                {
                    var index = factors.FindIndex((x) => comparer.Equals(x, e));
                    if (excludeFirst)
                    {
                        --index;
                    }
                    for (var i = 0; i < values.Length; ++i)
                    {
                        if (asBoolean)
                        {
                            values[i].Add(i == index);
                        }
                        else
                        {
                            values[i].Add(i == index ? 1 : 0);
                        }
                    }
                }

                for (var i = 0; i < values.Length; ++i)
                {
                    var name = baseName + factors[i + (excludeFirst ? 1 : 0)].ToString();
                    df.AddColumn(name, values[i]);
                }
            }

            return df;
        }

        #endregion
    }
}
