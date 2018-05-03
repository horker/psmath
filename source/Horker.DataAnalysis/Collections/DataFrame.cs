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

        #region Constructors

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

        #endregion

        #region Factory methods

        public static DataFrame Create<T>(T[][] jagged)
        {
            var df = new DataFrame();

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
                df.DefineNewColumn("c" + column, data[column]);
            }

            return df;
        }

        public static DataFrame Create<T>(T[] array, int rowCount = int.MaxValue, int columnCount = int.MaxValue, bool transpose = false)
        {
            var df = new DataFrame();

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
                        index = (row * columnCount + column) % array.Length;
                    }
                    else {
                        index = (column * rowCount + row) % array.Length;
                    }
                    v.Add(array[index]);
                }
            }

            for (var column = 0; column < columnCount; ++column) {
                df.DefineNewColumn("c" + column, data[column]);
            }

            return df;
        }

        public static DataFrame Create<T>(T[,] matrix)
        {
            var df = new DataFrame();

            int rowCount = matrix.GetLength(0);
            int columnCount = matrix.GetLength(1);

            for (var column = 0; column < columnCount; ++column) {
                var v = new Vector(rowCount);
                for (var row = 0; row < rowCount; ++row) {
                    v.Add(matrix[row, column]);
                }
                df.DefineNewColumn("c" + column, v);
            }

            return df;
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

        public DataFrame Clone()
        {
            var df = new DataFrame();

            for (var i = 0; i < ColumnCount; ++i) {
                df.DefineNewColumn(ColumnNames[i], new Vector(GetColumn(i)));
            }

            return df;
        }

        #endregion

        #region Properties

        public PSObject LinkedPSObject
        {
            get => _link;
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

        public Vector this[string name] {
            get => GetColumn(name);
            set => SetColumn(name, value);
        }

        public PSObject this[int index]
        {
            get => GetRow(index);
            set => SetRow(value);
        }

        public object this[int? row, int? column]
        {
            get
            {
                if (row == null) {
                    return GetColumn(column.Value);
                }
                if (column == null) {
                    return GetRow(row.Value);
                }
                return GetColumn(column.Value)[row.Value];
            }

            set
            {
                if (row == null || column == null) {
                    throw new ArgumentException("row and column should not be null");
                }
                GetColumn(column.Value)[row.Value] = value;
            }
        }

        public object this[int? row, string column]
        {
            get
            {
                if (row == null) {
                    return GetColumn(column);
                }
                return GetColumn(column)[row.Value];
            }

            set
            {
                if (row == null) {
                    throw new ArgumentException("row should not be null");
                }
                GetColumn(column)[row.Value] = value;
            }
        }

        #endregion

        #region  Enumerator<PSObject>

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

        #endregion

        #region Conversions

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

            var data = new double[RowCount, ColumnCount];
            for (var column = 0; column < ColumnCount; ++column) {
                var v = GetColumn(column);
                for (var row = 0; row < RowCount; ++row) {
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

        #endregion

        #region Inplace conversions

        public void ColumnToDummyValues(string columnName, CodificationType codificationType = CodificationType.OneHotDropFirst)
        {
            InvalidateCache();

            var baseName = columnName;
            if (codificationType != CodificationType.Multilevel) {
                baseName += "_";
            }

            var column = this[columnName];
            RemoveColumn(columnName);

            column.ToDummyValues(this, baseName, codificationType);
        }

        #endregion

        #region Data manipulations

        public void InvalidateCache()
        {
            if (_jaggedArrayCache != null) {
                _jaggedArrayCache.Target = null;
            }
            if (_arrayCache != null) {
                _arrayCache.Target = null;
            }
        }

        public bool HasColumn(string name)
        {
            // Use this method to find if a column name exists in the object
            // because column names are case-incensitive.
            return _columns.ContainsKey(name);
        }

        public void Add(string name, object value)
        {
            InvalidateCache();
            _columns[name].Add(value);
        }

        public PSObject GetRow(int index)
        {
            var obj = new PSObject();
            foreach (var name in _names) {
                var column = _columns[name];
                object value = null;
                if (index < column.Count) {
                    value = column[index];
                }
                var prop = new PSNoteProperty(name, value);
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
            InvalidateCache();

            var names = new HashSet<string>(new StringKeyComparer());

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
        }

        public void SetColumn<T>(string name, IEnumerable<T> values)
        {
            if (!_columns.ContainsKey(name)) {
                AddColumn(name, values);
            }
            else {
                InvalidateCache();
                _columns[name].AddRange(values);
            }
        }

        public void SetColumn<T>(string name, T[] values)
        {
            if (!_columns.ContainsKey(name)) {
                AddColumn(name, values);
            }
            else {
                InvalidateCache();
                _columns[name].AddRange(values);
            }
        }

        public void DefineNewColumn(string name, Vector data)
        {
            if (_columns.ContainsKey(name)) {
                throw new RuntimeException("Column already exists");
            }

            InvalidateCache();

            _names.Add(name);
            _columns[name] = data;
            _link.Properties.Add(new PSNoteProperty(name, data));
        }

        public void AddColumn<T>(string name, IEnumerable<T> values)
        {
            DefineNewColumn(name, Vector.Create(values));
        }

        public void AddColumn<T>(string name, T[] values)
        {
            DefineNewColumn(name, Vector.Create(values));
        }

        public void InsertColumn<T>(int index, string name, IEnumerable<T> values)
        {
            DefineNewColumn(name, Vector.Create(values));
            MoveColumn(name, index);
        }

        public void InsertColumn<T>(string before, string name, T[] values)
        {
            DefineNewColumn(name, Vector.Create(values));
            MoveColumn(name, before);
        }

        public void Aggregate(DataFrame df)
        {
            for (var i = 0; i < df.ColumnCount; ++i) {
                DefineNewColumn(df.ColumnNames[i], new Vector(df.GetColumn(i)));
            }
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

        public void RenameColumn(string name, string newName)
        {
            var index = _names.FindIndex((x) => StringKeyComparer.Compare(x, name));
            if (index == -1) {
                throw new RuntimeException("No such a column");
            }

            _names[index] = newName;
            _columns[newName] = _columns[name];
            _columns.Remove(name);

            UpdateLinkedObject();
        }

        public void MoveColumn(string name, int index)
        {
            _names.Remove(name);
            _names.Insert(index, name);

            UpdateLinkedObject();
        }

        public void MoveColumn(string name, string location)
        {
            var index = _names.FindIndex((x) => StringKeyComparer.Compare(x, location));
            if (index == -1) {
                throw new RuntimeException("No such a column");
            }

            MoveColumn(name, index);
        }

        private void UpdateLinkedObject()
        {
            var names = new List<string>();
            foreach (var p in _link.Properties) {
                names.Add(p.Name);
            }

            foreach (var n in names) {
                _link.Properties.Remove(n);
            }

            foreach (var n in _names) {
                _link.Properties.Add(new PSNoteProperty(n, _columns[n]));
            }
        }

        public void FillShortColumns()
        {
            var maxCount = _columns.Values.Max(x => x.Count);

            foreach (var column in _columns.Values) {
                while (column.Count < maxCount) {
                    column.Add(null);
                }
            }
        }

        public void Clear()
        {
            InvalidateCache();
            _names.Clear();
            _columns.Clear();
        }

        #endregion

        #region Elementwise operations

        public DataFrame Apply(Func<object, object> f)
        {
            var df = new DataFrame();

            for (var column = 0; column < ColumnCount; ++column) {
                var v = GetColumn(column);
                var newVec = new Vector(v.Count);
                for (var row = 0; row < RowCount; ++row) {
                    newVec.Add(f.Invoke(v[row]));
                }
                df.DefineNewColumn(ColumnNames[column], newVec);
            }

            return df;
        }

        public DataFrame Apply(Func<object, object, object> f, object arg1)
        {
            var df = new DataFrame();

            for (var column = 0; column < ColumnCount; ++column) {
                var v = GetColumn(column);
                var newVec = new Vector(v.Count);
                for (var row = 0; row < RowCount; ++row) {
                    newVec.Add(f.Invoke(v[row], arg1));
                }
                df.DefineNewColumn(ColumnNames[column], newVec);
            }

            return df;
        }

        public DataFrame Apply(ScriptBlock f)
        {
            var df = new DataFrame();

            for (var column = 0; column < ColumnCount; ++column) {
                var v = GetColumn(column);
                var newVec = new Vector(v.Count);
                for (var row = 0; row < RowCount; ++row) {
                    var va = new List<PSVariable>() { new PSVariable("_") };
                    va[0].Value = v[row];
                    newVec.Add(f.InvokeWithContext(null, va, null).Last());
                }
                df.DefineNewColumn(ColumnNames[column], newVec);
            }

            return df;
        }

        public void ApplyInPlace(Func<object, object> f)
        {
            for (var column = 0; column < ColumnCount; ++column) {
                var v = GetColumn(column);
                for (var row = 0; row < RowCount; ++row) {
                    v[row] = f.Invoke(v[row]);
                }
            }
        }

        public void ApplyInPlace(Func<object, object, object> f, object arg1)
        {
            for (var column = 0; column < ColumnCount; ++column) {
                var v = GetColumn(column);
                for (var row = 0; row < RowCount; ++row) {
                    v[row] = f.Invoke(v[row], arg1);
                }
            }
        }

        public void ApplyInPlace(ScriptBlock f)
        {
            for (var column = 0; column < ColumnCount; ++column) {
                var v = GetColumn(column);
                for (var row = 0; row < RowCount; ++row) {
                    var va = new List<PSVariable>() { new PSVariable("_") };
                    va[0].Value = v[row];
                    v[row] = f.InvokeWithContext(null, va, null).Last();
                }
            }
        }

        public DataFrame Apply(DataFrame b, Func<object, object, object> f)
        {
            if (ColumnCount > b.ColumnCount || RowCount > b.RowCount) {
                throw new RuntimeException("Data sizes are not the same");
            }

            var df = new DataFrame();

            for (var column = 0; column < ColumnCount; ++column) {
                var v = GetColumn(column);
                var v2 = b.GetColumn(column);
                var newVec = new Vector(v.Count);
                for (var row = 0; row < RowCount; ++row) {
                    newVec.Add(f.Invoke(v[row], v2[row]));
                }
                df.DefineNewColumn(ColumnNames[column], newVec);
            }

            return df;
        }

        public DataFrame Apply(DataFrame b, ScriptBlock f)
        {
            if (ColumnCount > b.ColumnCount || RowCount > b.RowCount) {
                throw new RuntimeException("Data sizes are not the same");
            }

            var df = new DataFrame();

            for (var column = 0; column < ColumnCount; ++column) {
                var v = GetColumn(column);
                var v2 = b.GetColumn(column);
                var newVec = new Vector(v.Count);
                for (var row = 0; row < RowCount; ++row) {
                    var va = new List<PSVariable>() { new PSVariable("a"), new PSVariable("b") };
                    va[0].Value = v[row];
                    va[1].Value = v2[row];
                    newVec.Add(f.InvokeWithContext(null, va, null).Last());
                }
                df.DefineNewColumn(ColumnNames[column], newVec);
            }

            return df;
        }

        // Scalar arithmetics

        public DataFrame Plus(double arg1) { return Apply(x => Converter.ToDouble(x) + arg1); }
        public DataFrame Minus(double arg1) { return Apply(x => Converter.ToDouble(x) - arg1); }
        public DataFrame Multiply(double arg1) { return Apply(x => Converter.ToDouble(x) * arg1); }
        public DataFrame Divide(double arg1) { return Apply(x => Converter.ToDouble(x) / arg1); }
        public DataFrame Minus() { return Apply(x => -Converter.ToDouble(x)); }

        // System.Math functions

        public DataFrame Abs() { return Apply(x => Math.Abs(Converter.ToDouble(x))); }
        public DataFrame Acos() { return Apply(x => Math.Acos(Converter.ToDouble(x))); }
        public DataFrame Asin() { return Apply(x => Math.Asin(Converter.ToDouble(x))); }
        public DataFrame Atan() { return Apply(x => Math.Atan(Converter.ToDouble(x))); }
        public DataFrame Atan2(double arg) { return Apply(x => Math.Atan2(Converter.ToDouble(x), arg)); }
        public DataFrame Ceiling() { return Apply(x => Math.Ceiling(Converter.ToDouble(x))); }
        public DataFrame Cos() { return Apply(x => Math.Cos(Converter.ToDouble(x))); }
        public DataFrame Cosh() { return Apply(x => Math.Cosh(Converter.ToDouble(x))); }
        public DataFrame Exp() { return Apply(x => Math.Exp(Converter.ToDouble(x))); }
        public DataFrame Floor() { return Apply(x => Math.Floor(Converter.ToDouble(x))); }
        public DataFrame IEEERemainder(double arg) { return Apply(x => Math.IEEERemainder(Converter.ToDouble(x), arg)); }
        public DataFrame Log() { return Apply(x => Math.Log(Converter.ToDouble(x))); }
        public DataFrame Log(double arg) { return Apply(x => Math.Log(Converter.ToDouble(x), arg)); }
        public DataFrame Log10() { return Apply(x => Math.Log10(Converter.ToDouble(x))); }
        public DataFrame Max(double arg) { return Apply(x => Math.Max(Converter.ToDouble(x), arg)); }
        public DataFrame Min(double arg) { return Apply(x => Math.Min(Converter.ToDouble(x), arg)); }
        public DataFrame Pow(double arg) { return Apply(x => Math.Pow(Converter.ToDouble(x), arg)); }
        public DataFrame Round() { return Apply(x => Math.Round(Converter.ToDouble(x))); }
        public DataFrame Round(int arg) { return Apply(x => Math.Round(Converter.ToDouble(x), arg)); }
        public DataFrame Round(int arg, MidpointRounding arg2) { return Apply(x => Math.Round(Converter.ToDouble(x), arg, arg2)); }
        public DataFrame Sign() { return Apply(x => Math.Sign(Converter.ToDouble(x))); }
        public DataFrame Sin() { return Apply(x => Math.Sin(Converter.ToDouble(x))); }
        public DataFrame Sinh() { return Apply(x => Math.Sinh(Converter.ToDouble(x))); }
        public DataFrame Sqrt() { return Apply(x => Math.Sqrt(Converter.ToDouble(x))); }
        public DataFrame Tan() { return Apply(x => Math.Tan(Converter.ToDouble(x))); }
        public DataFrame Tanh() { return Apply(x => Math.Tanh(Converter.ToDouble(x))); }
        public DataFrame Truncate() { return Apply(x => Math.Truncate(Converter.ToDouble(x))); }

        // Matrix arithmetics

        public DataFrame Add(DataFrame b) { return Apply(b, (x, y) => Converter.ToDouble(x) + Converter.ToDouble(y)); }
        public DataFrame Plus(DataFrame b) { return Apply(b, (x, y) => Converter.ToDouble(x) + Converter.ToDouble(y)); }
        public DataFrame Subtract(DataFrame b) { return Apply(b, (x, y) => Converter.ToDouble(x) - Converter.ToDouble(y)); }
        public DataFrame Multiply(DataFrame b) { return Apply(b, (x, y) => Converter.ToDouble(x) * Converter.ToDouble(y)); }
        public DataFrame Divide(DataFrame b) { return Apply(b, (x, y) => Converter.ToDouble(x) / Converter.ToDouble(y)); }

        #endregion

        #region Statistical values

        public double Determinant()
        {
            return ToDoubleArray().Determinant();
        }

        public DataFrame Inverse()
        {
            return DataFrame.Create(ToDoubleArray().Inverse());
        }

        public DataFrame PseudoInverse()
        {
            return DataFrame.Create(ToDoubleArray().PseudoInverse());
        }

        #endregion

        #region Linear Algebra / numerical operations (non-destructive)

        public DataFrame Cross(DataFrame b)
        {
            return Create(ToDoubleArray().Transpose().Dot(b.ToDoubleArray()));
        }

        public DataFrame Dot(DataFrame b)
        {
            return Create(ToDoubleArray().Dot(b.ToDoubleArray()));
        }

        public Vector Dot(Vector b)
        {
            return new Vector(ToDoubleArray().Dot(b.ToDoubleArray()));
        }

        public DataFrame Kronecker(DataFrame b)
        {
            return Create(ToDoubleArray().Kronecker(b.ToDoubleArray()));
        }

        public Vector Solve(Vector rightSide, bool leastSquares = false)
        {
            return Vector.Create(ToDoubleArray().Solve(rightSide.ToDoubleArray(), leastSquares));
        }

        public DataFrame Solve(DataFrame rightSide, bool leastSquares = false)
        {
            return DataFrame.Create(ToDoubleArray().Solve(rightSide.ToDoubleArray(), leastSquares));
        }

        public double Trace()
        {
            return ToDoubleArray().Trace();
        }

        public DataFrame Transpose()
        {
            return DataFrame.Create(ToDoubleArray().Transpose());
        }

        public DataFrame T()
        {
            return Transpose();
        }

        public DataFrame Diagonal()
        {
            var df = DataFrame.Zero(RowCount, ColumnCount);

            var count = Math.Max(RowCount, ColumnCount);
            for (var i = 0; i < count; ++i) {
                df[i, i] = this[i, i];
            }

            return df;
        }

        public DataFrame UpperTriangular(bool diagonal = true)
        {
            var df = new DataFrame(this);

            if (diagonal) {
                for (var row = 0; row < RowCount; ++row) {
                    for (var column = 0; column < row; ++column) {
                        df[row, column] = 0.0;
                    }
                }
            }
            else {
                for (var row = 0; row < RowCount; ++row) {
                    for (var column = 0; column <= row; ++column) {
                        df[row, column] = 0.0;
                    }
                }
            }

            return df;
        }

        public DataFrame LowerTriangular(bool diagonal = true)
        {
            var df = new DataFrame(this);

            if (diagonal) {
                for (var row = 0; row < RowCount; ++row) {
                    for (var column = row + 1; column < ColumnCount; ++column) {
                        df[row, column] = 0.0;
                    }
                }
            }
            else {
                for (var row = 0; row < RowCount; ++row) {
                    for (var column = row; column < ColumnCount; ++column) {
                        df[row, column] = 0.0;
                    }
                }
            }

            return df;
        }

        #endregion

        #region Decompositions

        public class CholeskyWrapper
        {
            private CholeskyDecomposition _ch;

            public CholeskyWrapper(CholeskyDecomposition ch)
            {
                _ch = ch;
            }

            public CholeskyDecomposition Source => _ch;

            public double Determinant => _ch.Determinant;

            public Vector Diagonal => new Vector(_ch.Diagonal);

            public DataFrame DiagonalMatrix => Create(_ch.DiagonalMatrix);

            public bool IsPositiveDefinite => _ch.IsPositiveDefinite;

            public bool IsUndefined => _ch.IsUndefined;

            public DataFrame LeftTriangularFactor => Create(_ch.LeftTriangularFactor);

            public DataFrame A => LeftTriangularFactor;

            public double LogDeterminant => _ch.LogDeterminant;

            public bool Nonsingular => _ch.Nonsingular;
        }

        public CholeskyWrapper Cholesky(bool robust = false, MatrixType valueType = MatrixType.UpperTriangular)
        {
            return new CholeskyWrapper(new CholeskyDecomposition(ToDoubleArray(), robust, false, valueType));
        }

        public class EigenvalueWrapper
        {
            private EigenvalueDecomposition _eigen;

            public EigenvalueWrapper(EigenvalueDecomposition eigen)
            {
                _eigen = eigen;
            }

            public EigenvalueDecomposition Source => _eigen;

            public DataFrame DiagonalMatrix => Create(_eigen.DiagonalMatrix);

            public DataFrame Eigenvectors => Create(_eigen.Eigenvectors);

            public DataFrame Vectors => Eigenvectors;

            public Vector ImaginaryEigenvalues => new Vector(_eigen.ImaginaryEigenvalues);

            public double Rank => _eigen.Rank;

            public Vector RealEigenvalues => new Vector(_eigen.RealEigenvalues);

            public Vector Values => RealEigenvalues;
        }

        public EigenvalueWrapper Eigenvalue(bool assumeSymmetric = false, bool sort = false)
        {
            return new EigenvalueWrapper(new EigenvalueDecomposition(ToDoubleArray(), assumeSymmetric, false, sort));
        }

        public class LuWrapper
        {
            private LuDecomposition _lu;

            public LuWrapper(LuDecomposition lu)
            {
                _lu = lu;
            }

            public LuDecomposition Source => _lu;

            public double Determinant => _lu.Determinant;

            public double LogDeterminant => _lu.LogDeterminant;

            public bool Nonsingular => _lu.Nonsingular;

            public DataFrame LowerTriangularFactor => Create(_lu.LowerTriangularFactor);

            public DataFrame L => LowerTriangularFactor;

            public DataFrame UpperTriangularFactor => Create(_lu.UpperTriangularFactor);

            public DataFrame U => UpperTriangularFactor;

            public Vector PivotPermutationVector => new Vector(_lu.PivotPermutationVector);
        }

        public LuWrapper Lu(bool transpose = false)
        {
            return new LuWrapper(new LuDecomposition(ToDoubleArray(), transpose));
        }

        public class QrWrapper
        {
            private QrDecomposition _qr;

            public QrWrapper(QrDecomposition qr)
            {
                _qr = qr;
            }

            public QrDecomposition Source => _qr;

            public Vector Diagonal => new Vector(_qr.Diagonal);

            public bool FullRank => _qr.FullRank;

            public DataFrame OrthogonalFactor => Create(_qr.OrthogonalFactor);

            public DataFrame Q => OrthogonalFactor;

            public DataFrame UpperTriangularFactor => Create(_qr.UpperTriangularFactor);

            public DataFrame R => UpperTriangularFactor;
        }

        public QrWrapper Qr(bool transpose = false, bool economy = true)
        {
            return new QrWrapper(new QrDecomposition(ToDoubleArray(), transpose, economy, false));
        }

        public class SvdWrapper
        {
            private SingularValueDecomposition _svd;

            public SvdWrapper(SingularValueDecomposition svd)
            {
                _svd = svd;
            }

            public SingularValueDecomposition Source => _svd;

            public double AbsoluteDeterminant => _svd.AbsoluteDeterminant;

            public double Condition => _svd.Condition;

            public Vector Diagonal => new Vector(_svd.Diagonal);

            public DataFrame DiagonalMatrix => Create(_svd.DiagonalMatrix);

            public DataFrame D => DiagonalMatrix;

            public bool IsSingular => _svd.IsSingular;

            public DataFrame LeftSingularVectors => Create(_svd.LeftSingularVectors);

            public DataFrame U => LeftSingularVectors;

            public double LogDeterminant => _svd.LogDeterminant;

            public double LogPseudoDeterminant => _svd.LogPseudoDeterminant;

            public Vector Ordering => new Vector(_svd.Ordering);

            public double PseudoDeteminant => _svd.PseudoDeterminant;

            public double Rank => _svd.Rank;

            public DataFrame RightSingularVector => Create(_svd.RightSingularVectors);

            public DataFrame V => RightSingularVector;

            public double Threshold => _svd.Threshold;

            public double TwoNorm => _svd.TwoNorm;
        }

        public SvdWrapper Svd(bool computeLeftSingularVectors = true, bool computeRightSingularVectors = true, bool autoTranspose = false)
        {
            return new SvdWrapper(new SingularValueDecomposition(ToDoubleArray(), computeLeftSingularVectors, computeRightSingularVectors, autoTranspose, false));
        }

        #endregion

        #region Linear Algebra / numerical operations (destructive)

        #endregion

        #region Aggregation methods

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

        #endregion
    }
}
