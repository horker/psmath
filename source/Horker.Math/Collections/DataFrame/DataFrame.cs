using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Management.Automation;
using System.Text;
using Accord.Math;
using Accord.Math.Random;

namespace Horker.Math
{
    public class DataFrame : IEnumerable<PSObject>
    {
        private List<string> _names;
        private Dictionary<string, DataFrameColumnBase> _columns;
        private PSObject _link;

        #region Constructors

        public DataFrame()
        {
            _names = new List<string>();
            _columns = new Dictionary<string, DataFrameColumnBase>(new StringKeyComparer());
            _link = new PSObject(this);
        }

        public DataFrame(IEnumerable<PSObject> objects)
            : this()
        {
            foreach (var obj in objects)
            {
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
            foreach (var j in jagged)
            {
                if (columnCount < j.Length)
                {
                    columnCount = j.Length;
                }
            }

            var data = new DataFrameColumn<T>[columnCount];

            for (var column = 0; column < columnCount; ++column)
                data[column] = new DataFrameColumn<T>(df, rowCount, 0);

            for (var row = 0; row < rowCount; ++row)
            {
                for (var column = 0; column < jagged[row].Length; ++column)
                {
                    data[column].Add(jagged[row][column]);
                }

                for (var column = jagged[row].Length; column < columnCount; ++column)
                {
                    data[column].Add(default(T));
                }
            }

            for (var column = 0; column < columnCount; ++column)
            {
                df.DefineNewColumn("c" + column, data[column]);
            }

            return df;
        }

        public static DataFrame Create<T>(T[] array, int rowCount = int.MaxValue, int columnCount = int.MaxValue, bool transpose = false)
        {
            var df = new DataFrame();

            if (columnCount == int.MaxValue)
            {
                if (rowCount == int.MaxValue)
                {
                    columnCount = 1;
                    rowCount = array.Length;
                }
                else
                {
                    columnCount = array.Length / rowCount;
                    if (columnCount == 0 || array.Length % rowCount != 0)
                        ++columnCount;
                }
            }
            else
            {
                if (rowCount == int.MaxValue)
                {
                    rowCount = array.Length / columnCount;
                    if (rowCount == 0 || array.Length % columnCount != 0)
                        ++rowCount;
                }
            }

            var data = new DataFrameColumn<T>[columnCount];
            for (int column = 0; column < columnCount; ++column)
                data[column] = new DataFrameColumn<T>(df, rowCount, 0);

            for (int column = 0; column < columnCount; ++column)
            {
                var v = data[column];
                for (int row = 0; row < rowCount; ++row)
                {
                    int index;
                    if (transpose)
                        index = (row * columnCount + column) % array.Length;
                    else
                        index = (column * rowCount + row) % array.Length;
                    v.Add(array[index]);
                }
            }

            for (var column = 0; column < columnCount; ++column)
                df.DefineNewColumn("c" + column, data[column]);

            return df;
        }

        public static DataFrame Create<T>(T[,] matrix)
        {
            var df = new DataFrame();

            int rowCount = matrix.GetLength(0);
            int columnCount = matrix.GetLength(1);

            for (var column = 0; column < columnCount; ++column)
            {
                var v = new DataFrameColumn<T>(df, rowCount, 0);
                for (var row = 0; row < rowCount; ++row)
                    v.Add(matrix[row, column]);
                df.DefineNewColumn("c" + column, v);
            }

            return df;
        }

        public static DataFrame FromDataTable(DataTable table)
        {
            var df = new DataFrame();
            var rowCount = table.Rows.Count;

            foreach (DataColumn column in table.Columns)
            {
                var name = column.ColumnName;
                df.DefineNewColumn(name, DataFrameColumnFactory.Create(column.DataType, df, rowCount, 0));
            }

            var columns = new DataFrameColumnBase[df.ColumnCount];
            for (var i = 0; i < columns.Length; ++i)
                columns[i] = df.GetColumn(i);

            foreach (DataRow row in table.Rows)
            {
                for (var i = 0; i < columns.Length; ++i)
                    columns[i].AddObject(row[i]);
            }

            return df;
        }

        public static DataFrame CreateLike(DataFrame source, int capacity, int fillSize)
        {
            var df = new DataFrame();

            foreach (var name in source.ColumnNames)
                df.DefineNewColumn(name, DataFrameColumnFactory.Create(source.GetColumn(name).DataType, df, capacity, fillSize));

            return df;
        }

        public DataFrame Clone()
        {
            var df = new DataFrame();

            for (var i = 0; i < ColumnCount; ++i)
                df.DefineNewColumn(ColumnNames[i], DataFrameColumnFactory.Clone(GetColumn(i)));

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
                if (_columns.Count == 0)
                    return 0;

                return _columns[_names[0]].Count;
            }
        }

        public int ColumnCount => _columns.Count;

        public int RowCount => Count;

        public List<String> ColumnNames => _names;

        public DataFrameColumnBase this[string name] => GetColumn(name);

        public DataFrameColumnBase this[int index] => GetColumn(index);

        public object this[string column, int row = -1]
        {
            get
            {
                if (row == -1)
                    return GetColumn(column);

                return GetColumn(column).GetObject(row);
            }

            set
            {
                if (row == -1)
                    throw new ArgumentException("Row should not be null");

                GetColumn(column).SetObject(row, value);
            }
        }

        public object this[int column, int row = -1]
        {
            get
            {
                if (row == -1)
                    return GetColumn(column);

                return GetColumn(column).GetObject(row);
            }

            set
            {
                if (row == -1)
                    throw new ArgumentException("Row should not be null");

                GetColumn(column).SetObject(row, value);
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

            public PSObject Current => _dataFrame.GetRow(_index);

            object IEnumerator.Current => _dataFrame.GetRow(_index);

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

        #region Information

        public DataFrameColumnInfo[] GetColumnInfo()
        {
            return _names.Select(name =>
                new DataFrameColumnInfo()
                {
                    Name = name,
                    DataType = DataFrameTypeHelper.GetDataFrameType(_columns[name].DataType)
                }).ToArray();
        }

        public int GetOrdinalOf(string name)
        {
            return _names.IndexOf(name);
        }

        #endregion

        #region Conversions

        public double[][] ToDoubleJaggedArray()
        {
            var data = new double[Count][];
            for (var i = 0; i < data.Length; ++i)
                data[i] = new double[_names.Count];

            var columnCount = 0;
            foreach (var entry in _columns)
            {
                var column = entry.Value;
                for (var rowCount = 0; rowCount < column.Count; ++rowCount)
                    data[rowCount][columnCount] = Convert.ToDouble(column.GetObject(rowCount));

                ++columnCount;
            }

            return data;
        }

        public double[,] ToDoubleMatrix()
        {
            var data = new double[RowCount, ColumnCount];
            for (var column = 0; column < ColumnCount; ++column)
            {
                var v = GetColumn(column);
                for (var row = 0; row < RowCount; ++row)
                    data[row, column] = Converter.ToDouble(v.GetObject(row));
            }

            return data;
        }

        public int[,] ToIntMatrix()
        {
            var data = new int[this.Count, _names.Count];
            for (var column = 0; column < _names.Count; ++column)
            {
                var v = GetColumn(column);
                for (var row = 0; row < Count; ++row)
                    data[row, column] = Convert.ToInt32(v.GetObject(row));
            }

            return data;
        }

        public double[] ToDoubleArray(bool transpose = false)
        {
            var data = new double[RowCount * ColumnCount];
            if (!transpose)
            {
                for (var column = 0; column < ColumnCount; ++column)
                {
                    var v = GetColumn(column);
                    for (var row = 0; row < RowCount; ++row)
                        data[row * ColumnCount + column] = Converter.ToDouble(v.GetObject(row));
                }
            }
            else
            {
                for (var column = 0; column < ColumnCount; ++column)
                {
                    var v = GetColumn(column);
                    for (var row = 0; row < RowCount; ++row)
                        data[row + column * RowCount] = Converter.ToDouble(v.GetObject(row));
                }
            }

            return data;
        }

        #endregion

        #region Data manipulations

        public bool HasColumn(string name)
        {
            // Use this method to find if a column name exists in the object
            // because column names are case-incensitive.
            return _columns.ContainsKey(name);
        }

        public PSObject GetRow(int index)
        {
            var obj = new PSObject();
            foreach (var name in _names)
            {
                var column = _columns[name];
                object value = null;
                if (index < column.Count)
                    value = column.GetObject(index);

                var prop = new PSNoteProperty(name, value);
                obj.Properties.Add(prop);
            }

            return obj;
        }

        public DataFrameColumnBase GetColumn(string name)
        {
            if (name.ToLower() == "__line__")
            {
                var count = this.Count;
                var column = new DataFrameColumn<Int32>(this, Count, 0);
                for (var i = 0; i < count; ++i)
                    column.Add(i);

                return column;
            }
            return _columns[name];
        }

        public DataFrameColumnBase GetColumn(int index)
        {
            return _columns[_names[index]];
        }

        public void SetRow(PSObject obj)
        {
            throw new NotImplementedException();
        }

        public void AddRow(PSObject obj)
        {
            var names = new HashSet<string>(new StringKeyComparer());

            var lastCount = RowCount;
            foreach (var p in obj.Properties)
            {
                names.Add(p.Name);
                if (_columns.ContainsKey(p.Name))
                    _columns[p.Name].AddObject(p.Value);
                else
                {
                    var column = DataFrameColumnFactory.CreateFromTypeName(p.TypeNameOfValue, this, lastCount + 1, lastCount);
                    column.AddObject(p.Value);
                    DefineNewColumn(p.Name, column);
                }
            }

            foreach (var entry in _columns)
            {
                if (!names.Contains(entry.Key))
                    _columns[entry.Key].AddObject(null);
            }
        }

        public void AddRowsWithDefaultValues(int rowCount)
        {
            foreach (var row in _columns.Values)
                row.AddDefaultValues(rowCount);
        }

        public void DefineNewColumn(string name, DataFrameColumnBase column)
        {
            if (_columns.ContainsKey(name))
                throw new ArgumentException("Column already exists");

            if (_columns.Count > 0 && column.Count != Count)
                throw new ArgumentException("Data length mismatch");

            column.Owner = this;
            _names.Add(name);
            _columns[name] = column;
            _link.Properties.Add(new PSNoteProperty(name, column));
        }

        public void AddColumn<T>(string name)
        {
            DefineNewColumn(name, new DataFrameColumn<T>(this, Count, Count));
        }

        public void AddColumn(string name, DataFrameType dataType)
        {
            DefineNewColumn(name, DataFrameColumnFactory.Create(dataType, this, Count, Count));
        }

        public void AddColumn<T>(string name, ICollection<T> values)
        {
            DefineNewColumn(name, new DataFrameColumn<T>(this, values));
        }

        public void AddColumn<T>(string name, T[] values)
        {
            DefineNewColumn(name, new DataFrameColumn<T>(this, values));
        }

        public void InsertColumn(int index, string name, DataFrameColumnBase column)
        {
            DefineNewColumn(name, column);
            MoveColumn(name, index);
        }

        public void InsertColumn<T>(int index, string name, IEnumerable<T> values)
        {
            DefineNewColumn(name, new DataFrameColumn<T>(this, values));
            MoveColumn(name, index);
        }

        public void InsertColumn<T>(string before, string name, T[] values)
        {
            DefineNewColumn(name, new DataFrameColumn<T>(this, values));
            MoveColumn(name, before);
        }

        public void RemoveColumn(string name)
        {
            if (!_names.Contains(name))
                throw new ArgumentException("No such a column");

            _columns.Remove(name);
            _names.Remove(name);
            _link.Properties.Remove(name);
        }

        public void RenameColumn(string name, string newName)
        {
            var index = _names.FindIndex((x) => StringKeyComparer.Compare(x, name));
            if (index == -1)
            {
                throw new ArgumentException("No such a column");
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
            if (index == -1)
            {
                throw new RuntimeException("No such a column");
            }

            MoveColumn(name, index);
        }

        private void UpdateLinkedObject()
        {
            var names = new List<string>();
            foreach (var p in _link.Properties)
            {
                names.Add(p.Name);
            }

            foreach (var n in names)
            {
                _link.Properties.Remove(n);
            }

            foreach (var n in _names)
            {
                _link.Properties.Add(new PSNoteProperty(n, _columns[n]));
            }
        }

        public void Clear()
        {
            _names.Clear();
            _columns.Clear();
        }

        #endregion

        #region Table-wide reshaping

        public void ExpandToOneHot(string columnName, int total, bool dropFirst = false, bool preserveOldColumn = false)
        {
            var column = GetColumn(columnName);
            var ordinal = GetOrdinalOf(columnName);

            var expanded = column.ToOneHot(total, dropFirst);

            for (var i = 0; i < expanded.Count; ++i)
            {
                var name = columnName + "_" + i;
                DefineNewColumn(name, expanded[i]);
                MoveColumn(name, ordinal + i);
            }

            if (!preserveOldColumn)
                RemoveColumn(columnName);
        }

        public DataFrame Widen(int minDataCount, int maxDataCount, string[] columnNames)
        {
            var df = new DataFrame();

            var count = RowCount;

            if (maxDataCount != -1 && count > maxDataCount)
                count = maxDataCount;

            var i = 0;
            for (; i < count; ++i)
            {
                foreach (var name in columnNames)
                {
                    var column = GetColumn(name);
                    var newColumn = DataFrameColumnFactory.Create(column.DataType, df);
                    newColumn.AddObject(column.GetObject(i));
                    df.DefineNewColumn(name + "_" + i, newColumn);
                }
            }

            for (; i < minDataCount; ++i)
            {
                foreach (var name in columnNames)
                {
                    var column = GetColumn(name);
                    var newColumn = DataFrameColumnFactory.Create(column.DataType, df);
                    newColumn.AddDefaultValues(1);
                    df.DefineNewColumn(name + "_" + i, newColumn);
                }
            }

            return df;
        }

        public DataFrame Widen(int minDataCount, int maxDataCount, object[] columnNames)
        {
            return Widen(minDataCount, maxDataCount, columnNames.Select(x => x.ToString()).ToArray());
        }

        public DataFrame Widen(string keyColumnName, int minDataCount, int maxDataCount, string[] columnNames)
        {
            var g = GroupBy(keyColumnName);

            if (minDataCount == -1)
                foreach (DictionaryEntry entry in g)
                {
                    var count = ((DataFrame)entry.Value).RowCount;
                    if (count > minDataCount)
                        minDataCount = count;
                }

            var keyColumnType = GetColumn(keyColumnName).DataType;

            var dfs = new List<DataFrame>(g.Count);
            foreach (DictionaryEntry entry in g)
            {
                var df = ((DataFrame)entry.Value).Widen(minDataCount, maxDataCount, columnNames);
                var column = DataFrameColumnFactory.Create(keyColumnType, null, 1, 0);
                column.AddObject(entry.Key);
                df.InsertColumn(0, keyColumnName, column);
                dfs.Add(df);
            }

            return Concatenate(dfs.ToArray());
        }

        public DataFrame Widen(string keyColumnName, int minDataCount, int maxDataCount, object[] columnNames)
        {
            return Widen(keyColumnName, minDataCount, maxDataCount, columnNames.Select(x => x.ToString()).ToArray());
        }

        public static DataFrame Concatenate(params DataFrame[] dfs)
        {
            var result = new DataFrame();

            var capacity = dfs.Select(x => x.RowCount).Sum();

            // Create columns

            foreach (var df in dfs)
            {
                foreach (var name in df.ColumnNames)
                {
                    if (!result.HasColumn(name))
                        result.DefineNewColumn(name, DataFrameColumnFactory.Create(df.GetColumn(name).DataType, result, capacity, 0));
                }
            }

            // Concatenate DataFame objects

            foreach (var df in dfs)
            {
                foreach (var name in result.ColumnNames)
                {
                    if (df.HasColumn(name))
                        result.GetColumn(name).AddColumn(df.GetColumn(name));
                    else
                        result.GetColumn(name).AddDefaultValues(df.RowCount);
                }
            }

            return result;
        }

        public DataFrameGroup GroupBy(string columnName)
        {
            var result = new DataFrameGroup();

            var keyColumn = GetColumn(columnName);

            var rowCount = RowCount;
            for (var i = 0; i < rowCount; ++i)
            {
                var group = keyColumn.GetObject(i);
                DataFrame df;
                if (!result.Contains(group))
                {
                    df = DataFrame.CreateLike(this, rowCount, 0);
                    result[group] = df;
                }
                else
                {
                    df = result[group];
                }

                foreach (var name in ColumnNames)
                    df.GetColumn(name).AddObject(GetColumn(name).GetObject(i));
            }

            return result;
        }

        public void Shuffle()
        {
            // ref. https://en.wikipedia.org/wiki/Fisher%E2%80%93Yates_shuffle

            int count = RowCount;
            for (var i = 0; i < count - 1; i++)
            {
                var j = Generator.Random.Next(i, count);
                foreach (var column in _columns.Values)
                {
                    var temp = column.GetObject(i);
                    column.SetObject(i, column.GetObject(j));
                    column.SetObject(j, temp);
                }
            }
        }

        public DataFrame Pivot(string verticalPivotName, string horizontalPivotName, string[] valueColumnNames)
        {
            var verticalPivot = GetColumn(verticalPivotName);
            var horizontalPivot = GetColumn(horizontalPivotName);

            var valueColumns = new DataFrameColumnBase[valueColumnNames.Length];

            for (var i = 0; i < valueColumnNames.Length; ++i)
                valueColumns[i] = GetColumn(valueColumnNames[i]);

            var verticalKeys = verticalPivot.ToStringArray().Distinct();
            var horizontalKeys = horizontalPivot.ToStringArray().Distinct();

            // Intialize rowMap

            var rowMap = new Dictionary<string, int>();
            for (var i = 0; i < verticalKeys.Length; ++i)
                rowMap.Add(verticalKeys[i], i);

            // Initialize columnMap and a result DataFrame.

            var columnMap = new Dictionary<Tuple<string, string>, DataFrameColumnBase>();

            var rowCount = verticalKeys.Length;
            var df = new DataFrame();
            df.AddColumn(verticalPivotName, verticalKeys);

            for (var i = 0; i < horizontalKeys.Length; ++i)
            {
                for (var j = 0; j < valueColumnNames.Length; ++j)
                {
                    var key = new Tuple<string, string>(horizontalKeys[i], valueColumnNames[j]);
                    var value = DataFrameColumnFactory.Create(valueColumns[j].DataType, null, rowCount, rowCount);

                    columnMap.Add(key, value);
                    df.DefineNewColumn(horizontalKeys[i] + "_" + valueColumnNames[j], value);
                }
            }

            // Place values
            // TODO: multiple values for a single cell

            for (var i = 0; i < RowCount; ++i)
            {
                var r = verticalPivot.GetObject(i).ToString();
                var c = horizontalPivot.GetObject(i).ToString();

                var rowIndex = rowMap[r];

                for (var j = 0; j < valueColumnNames.Length; ++j)
                {
                    var column = columnMap[new Tuple<string, string>(c, valueColumnNames[j])];
                    column.SetObject(rowIndex, valueColumns[j].GetObject(i));
                }
            }

            return df;
        }

        #endregion

        #region Elementwise operations
/*
        public DataFrame Apply(Func<object, object> f)
        {
            var df = new DataFrame();

            for (var column = 0; column < ColumnCount; ++column)
            {
                var v = GetColumn(column);
                var newVec = new DataFrameColumn(v.Count);
                for (var row = 0; row < RowCount; ++row)
                {
                    newVec.Add(f.Invoke(v[row]));
                }
                df.DefineNewColumn(ColumnNames[column], newVec);
            }

            return df;
        }

        public DataFrame Apply(Func<object, object, object> f, object arg1)
        {
            var df = new DataFrame();

            for (var column = 0; column < ColumnCount; ++column)
            {
                var v = GetColumn(column);
                var newVec = new DataFrameColumn(v.Count);
                for (var row = 0; row < RowCount; ++row)
                {
                    newVec.Add(f.Invoke(v[row], arg1));
                }
                df.DefineNewColumn(ColumnNames[column], newVec);
            }

            return df;
        }

        public DataFrame Apply(ScriptBlock f)
        {
            var df = new DataFrame();

            for (var column = 0; column < ColumnCount; ++column)
            {
                var v = GetColumn(column);
                var newVec = new DataFrameColumn(v.Count);
                for (var row = 0; row < RowCount; ++row)
                {
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
            for (var column = 0; column < ColumnCount; ++column)
            {
                var v = GetColumn(column);
                for (var row = 0; row < RowCount; ++row)
                {
                    v[row] = f.Invoke(v[row]);
                }
            }
        }

        public void ApplyInPlace(Func<object, object, object> f, object arg1)
        {
            for (var column = 0; column < ColumnCount; ++column)
            {
                var v = GetColumn(column);
                for (var row = 0; row < RowCount; ++row)
                {
                    v[row] = f.Invoke(v[row], arg1);
                }
            }
        }

        public void ApplyInPlace(ScriptBlock f)
        {
            for (var column = 0; column < ColumnCount; ++column)
            {
                var v = GetColumn(column);
                for (var row = 0; row < RowCount; ++row)
                {
                    var va = new List<PSVariable>() { new PSVariable("_") };
                    va[0].Value = v[row];
                    v[row] = f.InvokeWithContext(null, va, null).Last();
                }
            }
        }

        public DataFrame Apply(DataFrame b, Func<object, object, object> f)
        {
            if (ColumnCount > b.ColumnCount || RowCount > b.RowCount)
            {
                throw new RuntimeException("Data sizes are not the same");
            }

            var df = new DataFrame();

            for (var column = 0; column < ColumnCount; ++column)
            {
                var v = GetColumn(column);
                var v2 = b.GetColumn(column);
                var newVec = new DataFrameColumn(v.Count);
                for (var row = 0; row < RowCount; ++row)
                {
                    newVec.Add(f.Invoke(v[row], v2[row]));
                }
                df.DefineNewColumn(ColumnNames[column], newVec);
            }

            return df;
        }

        public DataFrame Apply(DataFrame b, ScriptBlock f)
        {
            if (ColumnCount > b.ColumnCount || RowCount > b.RowCount)
            {
                throw new RuntimeException("Data sizes are not the same");
            }

            var df = new DataFrame();

            for (var column = 0; column < ColumnCount; ++column)
            {
                var v = GetColumn(column);
                var v2 = b.GetColumn(column);
                var newVec = new DataFrameColumn(v.Count);
                for (var row = 0; row < RowCount; ++row)
                {
                    var va = new List<PSVariable>() { new PSVariable("a"), new PSVariable("b") };
                    va[0].Value = v[row];
                    va[1].Value = v2[row];
                    newVec.Add(f.InvokeWithContext(null, va, null).Last());
                }
                df.DefineNewColumn(ColumnNames[column], newVec);
            }

            return df;
        }
*/
        #endregion
    }
}
