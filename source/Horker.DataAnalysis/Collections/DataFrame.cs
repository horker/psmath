using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Management.Automation;
using Accord.Math;
using Accord.Math.Decompositions;
using System.Text;

namespace Horker.DataAnalysis
{
    public class DataFrame : IEnumerable<PSObject>
    {
        private List<string> _names;
        private Dictionary<string, Vector> _columns;
        private PSObject _link;

        private WeakReference _arrayCache;
        private WeakReference _jaggedArrayCache;

        public DataFrame()
        {
            _names = new List<string>();
            _columns = new Dictionary<string, Vector>(new StringKeyComparer());
            _link = new PSObject(this);
        }

        public DataFrame(IEnumerable<PSObject> objects)
            : this()
        {
            foreach (var obj in objects) {
                AddRow(obj);
            }
        }

        public DataFrame(object[][] jagged)
            : this()
        {
            int rowCount = jagged.Length;

            int columnCount = 0;
            foreach (var j in jagged) {
                if (columnCount < j.Length) {
                    columnCount = j.Length;
                }
            }

            var data = new Vector[columnCount];
            for (var column = 0; column < columnCount; ++column) {
                data[column] = new Vector(rowCount);
            }

            for (var row = 0; row < rowCount; ++row) {
                for (var column = 0; column < jagged[row].Length; ++column) {
                    data[column].Add(jagged[row][column]);
                }
                for (var column = jagged[row].Length; column < columnCount; ++column) {
                    data[column].Add(null);
                }
            }

            for (var column = 0; column < columnCount; ++column) {
                DefineNewColumn("c" + column, data[column]);
            }
        }

        public DataFrame(object[] array, int rowCount = int.MaxValue, int columnCount = int.MaxValue, bool transpose = false)
            : this()
        {
            if (columnCount == int.MaxValue) {
                if (rowCount == int.MaxValue) {
                    columnCount = 1;
                    rowCount = array.Length;
                }
                else {
                    columnCount = array.Length / rowCount;
                    if (columnCount == 0 || array.Length % rowCount != 0) {
                        ++columnCount;
                    }
                }
            }
            else {
                if (rowCount == int.MaxValue) {
                    rowCount = array.Length / columnCount;
                    if (rowCount == 0 || array.Length % columnCount != 0) {
                        ++rowCount;
                    }
                }
            }

            var data = new Vector[columnCount];
            for (int column = 0; column < columnCount; ++column) {
                data[column] = new Vector(rowCount);
            }

            for (int column = 0; column < columnCount; ++column) {
                var v = data[column];
                for (int row = 0; row < rowCount; ++row) {
                    int index;
                    if (transpose) {
                        index = (column * rowCount + row) % array.Length;
                    }
                    else {
                        index = (row * columnCount + column) % array.Length;
                    }
                    v.Add(array[index]);
                }
            }

            for (var column = 0; column < columnCount; ++column) {
                DefineNewColumn("c" + column, data[column]);
            }
        }

        public DataFrame(double[,] matrix)
            : this()
        {
            int rowCount = matrix.GetLength(0);
            int columnCount = matrix.GetLength(1);

            for (var column = 0; column < columnCount; ++column) {
                var v = new Vector(rowCount);
                for (var row = 0; row < rowCount; ++row) {
                    v.Add(matrix[row, column]);
                }
                DefineNewColumn("c" + column, v);
            }
        }

        public static DataFrame Diagonal(object value, int rowCount, int columnCount = int.MaxValue)
        {
            if (columnCount == int.MaxValue) {
                columnCount = rowCount;
            }

            var data = new Vector[columnCount];
            for (int column = 0; column < columnCount; ++column) {
                data[column] = new Vector(rowCount);
            }

            for (var column = 0; column < columnCount; ++column) {
                for (var row = 0; row < rowCount; ++row) {
                    if (row == column) {
                        data[column].Add(value);
                    }
                    else {
                        data[column].Add(0.0);
                    }
                }
            }

            var df = new DataFrame();

            for (var column = 0; column < columnCount; ++column) {
                df.DefineNewColumn("c" + column, data[column]);
            }

            return df;
        }

        public static DataFrame Identity(int rowCount, int columnCount = int.MaxValue)
        {
            return Diagonal(1.0, rowCount, columnCount);
        }

        public static DataFrame WithValue(object value, int rowCount, int columnCount = int.MaxValue)
        {
            if (columnCount == int.MaxValue) {
                columnCount = rowCount;
            }

            var data = new Vector[columnCount];
            for (int column = 0; column < columnCount; ++column) {
                data[column] = new Vector(rowCount);
            }

            for (var column = 0; column < columnCount; ++column) {
                for (var row = 0; row < rowCount; ++row) {
                    data[column].Add(value);
                }
            }

            var df = new DataFrame();

            for (var column = 0; column < columnCount; ++column) {
                df.DefineNewColumn("c" + column, data[column]);
            }

            return df;
        }

        public static DataFrame Zero(int rowCount, int columnCount = int.MaxValue)
        {
            return WithValue(0.0, rowCount, columnCount);
        }

        public PSObject LinkedPSObject
        {
            get => _link;
        }

        private void InvalidateCache()
        {
            if (_jaggedArrayCache != null) {
                _jaggedArrayCache.Target = null;
            }
        }

        private void DefineNewColumn(string name, Vector data)
        {
            if (_columns.ContainsKey(name)) {
                throw new RuntimeException("Column already exists");
            }

            InvalidateCache();

            _names.Add(name);
            _columns[name] = data;
            _link.Properties.Add(new PSNoteProperty(name, data));
        }

        public int Count
        {
            get
            {
                if (_columns.Count == 0) {
                    return 0;
                }
                return _columns[_names[0]].Count;
            }
        }

        public int ColumnCount
        {
            get => _columns.Count;
        }

        public int RowCount
        {
            get => Count;

        }

        public List<String> ColumnNames => _names;

        public List<object> Values
        {
            get
            {
                var values = new List<object>();
                var count = this.Count;
                for (var i = 0; i < count; ++i) {
                    values.Add(GetRow(i));
                }
                return values;
            }
        }

        public Vector this[string name] {
            get => GetColumn(name);
            set => SetColumn(name, value);
        }

        public PSObject this[int index]
        {
            get => GetRow(index);
            set => SetRow(value);
        }

        public object this[int row, int column]
        {
            get => GetColumn(column)[row];
        }

        public void Add(string name, object value)
        {
            InvalidateCache();
            ((Vector)_columns[name]).Add(value);
        }

        public PSObject GetRow(int index)
        {
            var obj = new PSObject();
            foreach (var name in _names) {
                var column = _columns[name];
                var prop = new PSNoteProperty(name, column[index]);
                obj.Properties.Add(prop);
            }

            return obj;
        }

        public Vector GetColumn(string name)
        {
            if (name.ToLower() == "__line__") {
                var count = this.Count;
                var column = new Vector(Count);
                for (var i = 0; i < count; ++i) {
                    column.Add(i);
                }
                return column;
            }
            return _columns[name];
        }

        public Vector GetColumn(int index)
        {
            return _columns[_names[index]];
        }

        public void SetRow(PSObject obj)
        {
            throw new NotImplementedException();
        }

        public void AddRow(PSObject obj)
        {
            var names = new HashSet<string>();

            var count = this.Count;
            foreach (var p in obj.Properties) {
                names.Add(p.Name);
                if (_columns.ContainsKey(p.Name)) {
                    _columns[p.Name].Add(p.Value);
                }
                else {
                    var l = new Vector();

                    for (var i = 0; i < count; ++i) {
                        l.Add(null);
                    }

                    l.Add(p.Value);

                    DefineNewColumn(p.Name, l);
                }
            }

            foreach (var entry in _columns) {
                if (!names.Contains(entry.Key)) {
                    _columns[entry.Key].Add(null);
                }
            }

            InvalidateCache();
        }

        public void SetColumn(string name, IEnumerable<object> values, bool fill = true)
        {
            var count = this.Count;

            InvalidateCache();

            if (!_names.Contains(name)) {
                AddColumn(name, values, fill);
                return;
            }

            var l = _columns[name];
            l.Clear();
            l.AddRange(values);

            if (fill) {
                if (l.Count < count) {
                    for (var i = 0; i < count - l.Count; ++i) {
                        l.Add(null);
                    }
                }
                else if (l.Count > count) {
                    foreach (var entry in _columns) {
                        l = entry.Value;
                        while (l.Count < count) {
                            l.Add(null);
                        }
                    }
                }
            }
        }

        public void AddColumn(string name, IEnumerable<object> values, bool fill = true)
        {
            if (_columns.ContainsKey(name)) {
                throw new RuntimeException("Column already exists");
            }

            InvalidateCache();

            var count = this.Count;

            Vector l;
            if (_columns.ContainsKey(name)) {
                l = (Vector)_columns[name];
                l.AddRange(values);
            }
            else {
                l = new Vector(count);
                l.AddRange(values);
            }

            count = System.Math.Max(l.Count, count);

            if (fill) {
                foreach (var entry in _columns) {
                    var l2 = (Vector)entry.Value;
                    while (l2.Count < count) {
                        l2.Add(null);
                    }
                }
            }

            DefineNewColumn(name, l);
        }

        public void RemoveColumn(string name)
        {
            if (!_names.Contains(name)) {
                throw new RuntimeException("No such a column");
            }

            InvalidateCache();
            _columns.Remove(name);
            _names.Remove(name);
            _link.Properties.Remove(name);
        }

        public bool ContainsKey(string key)
        {
            return _columns.ContainsKey(key);
        }

        public void Clear()
        {
            InvalidateCache();
            _columns.Clear();
        }

        // Enumerator<PSObject>

        private class Enumerator : IEnumerator<PSObject>
        {
            private DataFrame _dataFrame;
            private int _index;

            public Enumerator(DataFrame dataFrame)
            {
                _dataFrame = dataFrame;
                _index = -1;
            }

            public PSObject Current
            {
                get { return _dataFrame.GetRow(_index); }
            }

            object IEnumerator.Current
            {
                get { return _dataFrame.GetRow(_index);  }
            }

            public void Dispose()
            {
                // Do nothing
            }

            public bool MoveNext()
            {
                ++_index;
                return _index < _dataFrame.Count;
            }

            public void Reset()
            {
                _index = 0;
            }
        }

        public IEnumerator<PSObject> GetEnumerator()
        {
            return new Enumerator(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new Enumerator(this);
        }

        // Conversion

        public double[][] ToDoubleJaggedArray()
        {
            if (_jaggedArrayCache != null) {
                if (_jaggedArrayCache.IsAlive && _jaggedArrayCache.Target != null) {
                    return (double[][])_jaggedArrayCache.Target;
                }
            }
            else {
                _jaggedArrayCache = new WeakReference(null);
            }

            var data = new double[this.Count][];
            for (var i = 0; i < data.Length; ++i) {
                data[i] = new double[_names.Count];
            }

            var columnCount = 0;
            foreach (var entry in _columns) {
                var column = entry.Value;
                for (var rowCount = 0; rowCount < column.Count; ++rowCount) {
                    data[rowCount][columnCount] = Convert.ToDouble(column[rowCount]);
                }
                ++columnCount;
            }

            _jaggedArrayCache.Target = data;

            return data;
        }

        public double[,] ToDoubleArray()
        {
            if (_arrayCache != null) {
                if (_arrayCache.IsAlive && _arrayCache.Target != null) {
                    return (double[,])_arrayCache.Target;
                }
            }
            else {
                _arrayCache = new WeakReference(null);
            }

            var data = new double[this.Count, _names.Count];
            for (var column = 0; column < _names.Count; ++column) {
                var v = GetColumn(0);
                for (var row = 0; row < this.Count; ++row) {
                    data[row, column] = Converter.ToDouble(v[row]);
                }
            }

            _arrayCache.Target = data;

            return data;
        }

        public int[,] ToIntArray()
        {
            var data = new int[this.Count, _names.Count];
            for (var column = 0; column < _names.Count; ++column) {
                var v = GetColumn(column);
                for (var row = 0; row < this.Count; ++row) {
                    data[row, column] = Convert.ToInt32(v[row]);
                }
            }

            return data;
        }

        // Inplace conversions

        public void ColumnToDummyValues(string columnName, CodificationType codificationType = CodificationType.OneHotDropFirst)
        {
            var baseName = columnName;
            if (codificationType != CodificationType.Multilevel) {
                baseName += "_";
            }

            var column = this[columnName];
            RemoveColumn(columnName);

            column.ToDummyValues(this, baseName, codificationType);
        }

        // Data manupilation

        public void Aggregate(DataFrame df)
        {
            for (var i = 0; i < df.ColumnCount; ++i) {
                var column = df.GetColumn(i);
                DefineNewColumn(df.ColumnNames[i], new Vector(column));
            }
        }

        public DataFrameGroup GroupBy(string columnName)
        {
            var result = new DataFrameGroup();
            foreach (var row in this) {
                var group = row.Properties[columnName].Value;
                if (!result.Contains(group)) {
                    result[group] = new DataFrame();
                }
                ((DataFrame)result[group]).AddRow(row);
            }

            return result;
        }

        public DataFrameGroup GroupBy(params object[] columnNames)
        {
            var result = new DataFrameGroup();
            foreach (var row in this) {
                var builder = new StringBuilder();
                for (var i = 0; i < columnNames.Length - 1; ++i) {
                    builder.Append(row.Properties[columnNames[i].ToString()].Value.ToString());
                    builder.Append(",");
                }
                builder.Append(row.Properties[columnNames[columnNames.Length - 1].ToString()].Value.ToString());

                var group = builder.ToString();

                if (!result.Contains(group)) {
                    result[group] = new DataFrame();
                }
                ((DataFrame)result[group]).AddRow(row);
            }

            return result;
        }
    }
}
