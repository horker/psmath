using System;
using System.Text;
using System.Collections.Generic;
using Accord;
using Accord.Math;
using Accord.Math.Decompositions;
using Accord.Statistics;
using System.Management.Automation;

namespace Horker.DataAnalysis
{
    public class CholeskyWrapper
    {
        private CholeskyDecomposition _ch;

        public CholeskyWrapper(CholeskyDecomposition ch)
        {
            _ch = ch;
        }

        public Matrix L => _ch.LeftTriangularFactor;
        public CholeskyDecomposition Source => _ch;
    }

    public class GramSchmidtOrthogonalizationWrapper
    {
        private GramSchmidtOrthogonalization _gs;

        public GramSchmidtOrthogonalizationWrapper(GramSchmidtOrthogonalization gs)
        {
            _gs = gs;
        }

        public Matrix R => new Matrix(_gs.UpperTriangularFactor);
        public Matrix Q => new Matrix(_gs.OrthogonalFactor);
        public GramSchmidtOrthogonalization Source => _gs;
    }

    public class EigenvalueWrapper
    {
        private EigenvalueDecomposition _eigen;

        public EigenvalueWrapper(EigenvalueDecomposition eigen)
        {
            _eigen = eigen;
        }

        public Matrix Vectors => _eigen.Eigenvectors;
        public Matrix Values => Matrix.AsVector(_eigen.RealEigenvalues, 0);
        public EigenvalueDecomposition Source => _eigen;
    }

    public class LuDecompositionWrapper
    {
        private LuDecomposition _lu;

        public LuDecompositionWrapper(LuDecomposition lu)
        {
            _lu = lu;
        }

        public int[] P => _lu.PivotPermutationVector;
        public Matrix L => _lu.LowerTriangularFactor;
        public Matrix U => _lu.UpperTriangularFactor;
        public LuDecomposition Source => _lu;
    }

    public class NonnegativeMatrixFactorizationWrapper
    {
        private NonnegativeMatrixFactorization _nmf;

        public NonnegativeMatrixFactorizationWrapper(NonnegativeMatrixFactorization nmf)
        {
            _nmf = nmf;
        }

        public Matrix W => _nmf.LeftNonnegativeFactors;
        public Matrix H => _nmf.RightNonnegativeFactors;
        public NonnegativeMatrixFactorization Source => _nmf;
    }

    public class QrDecompositionWrapper
    {
        private QrDecomposition _qr;

        public QrDecompositionWrapper(QrDecomposition qr)
        {
            _qr = qr;
        }

        public Matrix Q => _qr.OrthogonalFactor;
        public Matrix R => _qr.UpperTriangularFactor;
        public QrDecomposition Source => _qr;
    }

    public class SingularValueDecompositionWrapper
    {
        private SingularValueDecomposition _svd;

        public SingularValueDecompositionWrapper(SingularValueDecomposition svd)
        {
            _svd = svd;
        }

        public Matrix U => _svd.LeftSingularVectors;
        public Matrix D => _svd.DiagonalMatrix;
        public Matrix V => _svd.RightSingularVectors;
        public SingularValueDecomposition Source => _svd;
    }

    public class Matrix
    {
        private double[,] _values;

        #region Constructors and factory methods

        public Matrix(int row, int column)
        {
            _values = new double[row, column];
        }

        public Matrix(double[,] values, bool noCopy = false)
        {
            if (noCopy)
            {
                _values = values;
            }
            else
            {
                _values = new double[values.GetLength(0), values.GetLength(1)];
                Array.Copy(values, _values, _values.GetLength(0) * _values.GetLength(1));
            }
        }

        public static Matrix Create(double[][] jagged)
        {
            int rowCount = jagged.Length;

            int columnCount = 0;
            foreach (var j in jagged)
            {
                if (columnCount < j.Length)
                {
                    columnCount = j.Length;
                }
            }

            var matrix = new Matrix(rowCount, columnCount);

            for (var row = 0; row < rowCount; ++row)
            {
                for (var column = 0; column < jagged[row].Length; ++column)
                {
                    matrix[row, column] = jagged[row][column];
                }
            }
            return matrix;
        }

        public static Matrix Create(double[] array, int rowCount = int.MaxValue, int columnCount = int.MaxValue, bool transpose = false)
        {
            if (array.Length == 0)
            {
                array = new double[1];
            }

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
                    {
                        ++columnCount;
                    }
                }
            }
            else
            {
                if (rowCount == int.MaxValue)
                {
                    rowCount = array.Length / columnCount;
                    if (rowCount == 0 || array.Length % columnCount != 0)
                    {
                        ++rowCount;
                    }
                }
            }

            var matrix = new Matrix(rowCount, columnCount);

            for (int column = 0; column < columnCount; ++column)
            {
                for (int row = 0; row < rowCount; ++row)
                {
                    int index;
                    if (transpose)
                    {
                        index = (row * columnCount + column) % array.Length;
                    }
                    else
                    {
                        index = (column * rowCount + row) % array.Length;
                    }
                    matrix[row, column] = array[index];
                }
            }

            return matrix;
        }

        public static Matrix Diagonal(double[] values, int rowCount, int columnCount = int.MaxValue)
        {
            if (columnCount == int.MaxValue)
            {
                columnCount = rowCount;
            }

            var matrix = new Matrix(rowCount, columnCount);
            var limit = Math.Min(rowCount, columnCount);

            for (var i = 0; i < limit; ++i)
            {
                matrix[i, i] = values[i % values.Length];
            }

            return matrix;
        }

        public static Matrix Identity(int count)
        {
            return Diagonal(new double[] { 1.0 }, count, count);
        }

        public static Matrix WithValue(double value, int rowCount, int columnCount = int.MaxValue)
        {
            if (rowCount == int.MaxValue)
            {
                rowCount = 1;
            }

            if (columnCount == int.MaxValue)
            {
                columnCount = rowCount;
            }

            var matrix = new Matrix(rowCount, columnCount);
            for (var column = 0; column < columnCount; ++column)
            {
                for (var row = 0; row < rowCount; ++row)
                {
                    matrix[row, column] = value;
                }
            }

            return matrix;
        }

        public static Matrix CreateAs(Matrix source)
        {
            return new Matrix(source.RowCount, source.ColumnCount);
        }

        public static Matrix AsVector(double[] array, int dimension)
        {
            if (dimension == 0)
            {
                return Create(array, int.MaxValue);
            }
            else
            {
                return Create(array, 1);
            }
        }

        public Matrix Clone()
        {
            return new Matrix(_values);
        }

        #endregion

        #region Accessors

        public int RowCount => _values.GetLength(0);
        public int ColumnCount => _values.GetLength(1);
        public double[,] Values => _values;

        public double this[int row, int column]
        {
            get => _values[row, column];
            set
            {
                _values[row, column] = value;
            }
        }

        public double[] Row(int rowIndex)
        {
            var row = new double[ColumnCount];
            for (var i = 0; i < ColumnCount; ++i)
            {
                row[i] = _values[rowIndex, i];
            }

            return row;
        }

        public Matrix Rows(params int[] rowIndexes)
        {
            var matrix = new double[rowIndexes.Length, ColumnCount];

            for (var column = 0; column < ColumnCount; ++column)
            {
                for (var row = 0; row < rowIndexes.Length; ++row)
                {
                    matrix[row, column] = _values[rowIndexes[row], column];
                }
            }

            return matrix;
        }

        public double[] Column(int columnIndex)
        {
            var column = new double[RowCount];
            for (var i = 0; i < RowCount; ++i)
            {
                column[i] = _values[i, columnIndex];
            }

            return column;
        }

        public Matrix Columns(params int[] columnIndexes)
        {
            var matrix = new double[RowCount, columnIndexes.Length];

            for (var column = 0; column < columnIndexes.Length; ++column)
            {
                for (var row = 0; row < RowCount; ++row)
                {
                    matrix[row, column] = _values[row, columnIndexes[column]];
                }
            }

            return matrix;
        }

        #endregion

        #region Object methods

        public override bool Equals(object other)
        {
            if (other is double[])
            {
                return Accord.Math.Matrix.IsEqual(_values, other as double[]);
            }

            if (other is Matrix)
            {
                return Accord.Math.Matrix.IsEqual(_values, other as Matrix);
            }

            return false;
        }

        public override int GetHashCode()
        {
            int hash = 123456789;
            for (var row = 0; row < RowCount; ++row)
            {
                for (var column = 0; column < ColumnCount; ++column)
                {
                    hash ^= (int)this[row, column];
                }
            }

            return hash;
        }

        #endregion

        #region Convertors

        public string AsString()
        {
            // Long format

            string[,] elements = new string[RowCount, ColumnCount];
            int maxLength = int.MinValue;

            for (var column = 0; column < ColumnCount; ++column)
            {
                for (var row = 0; row < RowCount; ++row)
                {
                    var s = this[row, column].ToString("0.#####");
                    elements[row, column] = s;
                    if (maxLength < s.Length)
                    {
                        maxLength = s.Length;
                    }
                }
            }

            string delim = "\r\n";

            var builder = new StringBuilder();
            builder.AppendFormat("[{0} x {1}]{2}", RowCount, ColumnCount, delim);

            for (var row = 0; row < RowCount; ++row)
            {
                for (var column = 0; column < ColumnCount; ++column)
                {
                    builder.Append(elements[row, column].PadLeft(maxLength + 1));
                }
                if (row < RowCount - 1)
                {
                    builder.Append(delim);
                }
            }

            return builder.ToString();
        }

        public override string ToString()
        {
            // Short format

            var builder = new StringBuilder();
            builder.AppendFormat("[{0} x {1}]", RowCount, ColumnCount);

            for (var row = 0; row < RowCount; ++row)
            {
                builder.Append(" [");
                for (var column = 0; column < ColumnCount; ++column)
                {
                    builder.Append(' ');
                    builder.Append(this[row, column].ToString("0.#####"));
                }
                builder.Append(" ]");
            }

            return builder.ToString();
        }

        public double[] ToFlatArray()
        {
            if (RowCount == 1)
            {
                return Row(0);
            }
            else if (ColumnCount == 1)
            {
                return Column(0);
            }

            var result = new double[RowCount * ColumnCount];
            for (var column = 0; column < ColumnCount; ++column)
            {
                for (var row = 0; row < RowCount; ++row)
                {
                    result[row + RowCount * column] = this[row, column];
                }
            }

            return result;
        }

        public double[][] ToJagged(bool transpose = false)
        {
            return _values.ToJagged(transpose);
        }

        public double[][] ToJagged(int dimension)
        {
            bool transpose = dimension == 1;
            return _values.ToJagged(transpose);
        }

        public IEnumerable<PSObject> ToPSObject(bool transpose = false)
        {
            if (!transpose)
            {
                for (var row = 0; row < RowCount; ++row)
                {
                    var obj = new PSObject();
                    for (var column = 0; column < ColumnCount; ++column)
                    {
                        obj.Properties.Add(new PSNoteProperty("c" + column, this[row, column]));
                    }
                    yield return obj;
                }
            }
            else
            {
                for (var column = 0; column < ColumnCount; ++column)
                {
                    var obj = new PSObject();
                    for (var row = 0; row < RowCount; ++row)
                    {
                        obj.Properties.Add(new PSNoteProperty("c" + row, this[row, column]));
                    }
                    yield return obj;
                }
            }
        }

        public static implicit operator double[,] (Matrix matrix)
        {
            return matrix.Values;
        }

        public static implicit operator Matrix(double[,] values)
        {
            return new Matrix(values);
        }

        public static implicit operator Matrix(double[][] jagged)
        {
            return Matrix.Create(jagged);
        }

        #endregion

        #region Algebra computations

        public Matrix T
        {
            get => _values.Transpose();
        }

        public Matrix Inv
        {
            get => _values.Inverse();
        }

        internal static Matrix AdjustRow(Matrix matrix, int rowCount)
        {
            if (matrix.RowCount == rowCount)
            {
                return matrix;
            }

            if (matrix.RowCount != 1)
            {
                throw new ArgumentException("Matrix sizes are inconsistent");
            }

            return Matrix.Create(matrix.Row(0), rowCount, matrix.ColumnCount, true);
        }

        internal Matrix AdjustRow(Matrix matrix)
        {
            return AdjustRow(matrix, RowCount);
        }

        internal static Matrix AdjustColumn(Matrix matrix, int columnCount)
        {
            if (matrix.ColumnCount == columnCount)
            {
                return matrix;
            }

            if (matrix.ColumnCount != 1)
            {
                throw new ArgumentException("Matrix sizes are inconsistent");
            }

            return Matrix.Create(matrix.Column(0), matrix.RowCount, columnCount, false);
        }

        internal Matrix AdjustColumn(Matrix matrix)
        {
            return AdjustColumn(matrix, ColumnCount);
        }

        internal static Matrix AdjustRowColumn(Matrix matrix, int rowCount, int columnCount)
        {
            if (matrix.RowCount == rowCount)
            {
                if (matrix.ColumnCount == columnCount)
                {
                    return matrix;
                }

                if (matrix.ColumnCount == 1)
                {
                    return Matrix.Create(matrix.Column(0), rowCount, columnCount, false);
                }
            }
            else
            {
                if (matrix.RowCount == 1 && (matrix.ColumnCount == columnCount || matrix.ColumnCount == 1))
                {
                    return Matrix.Create(matrix.Row(0), rowCount, columnCount, true);
                }
            }

            throw new ArgumentException("Matrix sizes are inconsistent");
        }

        internal Matrix AdjustRowColumn(Matrix matrix)
        {
            return AdjustRowColumn(matrix, RowCount, ColumnCount);
        }

        internal static double[] EnsureSequence(Matrix matrix)
        {
            if (matrix.RowCount == 1)
            {
                return matrix.Row(0);
            }

            if (matrix.ColumnCount == 1)
            {
                return matrix.Column(0);
            }

            throw new ArgumentException("[n x 1] or [1 x n] matrix is required");
        }

        internal static Matrix EnsureMatrix(double[] values, Matrix origin)
        {
            if (origin.RowCount == 1)
            {
                return Matrix.Create(values, 1);
            }
            return Matrix.Create(values, int.MaxValue, 1);
        }

        internal Matrix EnsureMatrix(double[] values)
        {
            return EnsureMatrix(values, _values);
        }

        public Matrix Apply(Func<double, double> func)
        {
            return _values.Apply(func);
        }

        public Tuple<int, int> ArgMax()
        {
            return _values.ArgMax();
        }

        public Tuple<int, int> ArgMin()
        {
            return _values.ArgMin();
        }

        public int[] ArgSort()
        {
            return EnsureSequence(_values).ArgSort();
        }

        public int[] Bottom(int count)
        {
            return EnsureSequence(_values).Bottom(count);
        }

        public Matrix Cartesian()
        {
            // TODO
            return null;
        }

        public Matrix Concatenate(Matrix b)
        {
            return _values.Concatenate(AdjustRow(b));
        }

        public Matrix Convolve(Matrix kernel, bool trim = false)
        {
            return EnsureMatrix(EnsureSequence(_values).Convolve(EnsureSequence(kernel), trim));
        }

        public int Count(Func<double, bool> func)
        {
            return EnsureSequence(_values).Count(func);
        }

        public Matrix Cross(Matrix b)
        {
            return EnsureMatrix(EnsureSequence(_values).Cross(EnsureSequence(b)));
        }

        public Matrix CumulativeSum(int dimension)
        {
            // TODO: correct?
            return _values.CumulativeSum(dimension);
        }

        public ISolverMatrixDecomposition<double> Decompose(bool leastSquares = false)
        {
            return _values.Decompose(leastSquares);
        }

        public double Determinant(bool symmetric = false)
        {
            return _values.Determinant(symmetric);
        }

        public double[][] Distinct()
        {
            return _values.Distinct();
        }

        public int[] DistinctCount()
        {
            return _values.DistinctCount();
        }

        public Matrix Dot(Matrix b)
        {
            b = AdjustRow(b, ColumnCount);
            return _values.Dot(b);
        }

        public Matrix Expand(int[] count)
        {
            return Accord.Math.Matrix.Expand(_values, count);
        }

        public int[][] Find(Func<double, bool> func)
        {
            return _values.Find(func);
        }

        public Matrix First(int count)
        {
            return EnsureMatrix(EnsureSequence(_values).First(count));
        }

        public int First(Func<double, bool> func)
        {
            return EnsureSequence(_values).First(func);
        }

        public int? FirstOrNull(Func<double, bool> func)
        {
            return EnsureSequence(_values).FirstOrNull(func);
        }

        public Matrix Flatten(MatrixOrder order = MatrixOrder.CRowMajor)
        {
            return EnsureMatrix(_values.Flatten(order));
        }

        public Matrix GetLowerTriangle(bool includeDiagonal = true)
        {
            return _values.GetLowerTriangle(includeDiagonal);
        }

        public int GetNumberOfElements()
        {
            return _values.GetNumberOfElements();
        }

        public DoubleRange GetRange()
        {
            return _values.GetRange();
        }

        public Matrix GetSymmetric(MatrixType type = MatrixType.UpperTriangular)
        {
            // Accord.NET implementation has a bug

            var result = Matrix.CreateAs(this);

            switch (type)
            {
                case MatrixType.LowerTriangular:
                    for (int i = 0; i < RowCount; i++)
                    {
                        for (int j = 0; j <= i; j++)
                        {
                            result[i, j] = result[j, i] = this[i, j];
                        }
                    }
                    break;

                case MatrixType.UpperTriangular:
                    for (int i = 0; i < RowCount; i++)
                    {
                        for (int j = i; j < ColumnCount; j++)
                        {
                            result[i, j] = result[j, i] = this[i, j];
                        }
                    }
                    break;

                default:
                    throw new Exception("Matrix type can be either LowerTriangular or UpperTrianguler.");
            }

            return result;
        }

        public int GetTotalLength()
        {
            return _values.GetTotalLength();
        }

        public Matrix GetUpperTriangle(bool includeDiagonal = true)
        {
            return _values.GetUpperTriangle(includeDiagonal);
        }

        public bool HasInfinity()
        {
            return _values.HasInfinity();
        }

        public bool HasNaN()
        {
            return _values.HasNaN();
        }

        public Matrix Inverse()
        {
            return _values.Inverse();
        }

        public bool IsDiagonal()
        {
            return _values.IsDiagonal();
        }

        public bool IsLowerTriangular()
        {
            return _values.IsLowerTriangular();
        }

        public bool IsPositiveDefinite()
        {
            return _values.IsPositiveDefinite();
        }

        public bool IsSingular()
        {
            return _values.IsSingular();
        }

        public bool IsSymmetric()
        {
            return _values.IsSymmetric();
        }

        public bool IsUpperTriangular()
        {
            return _values.IsUpperTriangular();
        }

        public Matrix Kronecker(Matrix b)
        {
            return _values.Kronecker(b);
        }

        public Matrix Last(int count)
        {
            return EnsureMatrix(EnsureSequence(_values).Last(count));
        }

        public double LogDeterminant(bool symmetric = false)
        {
            return _values.LogDeterminant(symmetric);
        }

        public double LogPseudoDeterminant()
        {
            return _values.LogPseudoDeterminant();
        }

        public double Max()
        {
            return _values.Max();
        }

        public Matrix Merge(Matrix b)
        {
            return EnsureMatrix(Accord.Math.Matrix.Merge(new double[][] { EnsureSequence(_values), EnsureSequence(b) }));
        }

        public double Min()
        {
            return _values.Min();
        }

        public Matrix Normalize()
        {
            return EnsureMatrix(EnsureSequence(_values).Normalize(true));
        }

        public Matrix Null()
        {
            return _values.Null();
        }

        public Matrix OneHot()
        {
            return Accord.Math.Matrix.OneHot<double>(EnsureSequence(_values).Apply(x => (int)x));
        }

        public Matrix Outer(Matrix b)
        {
            return EnsureSequence(_values).Outer(EnsureSequence(b));
        }

        public Matrix Pad(int all)
        {
            return _values.Pad(all);
        }

        public Matrix Pad(int topBottom, int rightLeft)
        {
            return _values.Pad(topBottom, rightLeft);
        }

        public Matrix Pad(int top, int right, int bottom, int left)
        {
            return _values.Pad(top, right, bottom, left);
        }

        public double Product()
        {
            return _values.Product();
        }

        public double PseudoDeterminant()
        {
            return _values.PseudoDeterminant();
        }

        public int Rank()
        {
            return _values.Rank();
        }

        public Matrix Reversed()
        {
            return EnsureMatrix(EnsureSequence(_values).Reversed());
        }

        public Matrix Shuffle()
        {
            var seq = EnsureSequence(_values);
            seq.Shuffle();
            return EnsureMatrix(seq);
        }

        public Matrix Solve(Matrix rightSide, bool leastSquares = false)
        {
            return _values.Solve(rightSide, leastSquares);
        }

        public Matrix Sort(Matrix keys)
        {
            return Accord.Math.Matrix.Sort(EnsureSequence(keys), _values);
        }

        public Matrix Split(int size)
        {
            return Matrix.Create(EnsureSequence(_values).Split(size));
        }

        public Matrix Stack(Matrix b)
        {
            return Accord.Math.Matrix.Stack(_values, AdjustColumn(b));
        }

        public double Sum()
        {
            return _values.Sum();
        }

        public int[] Top(int count)
        {
            return EnsureSequence(_values).Top(count);
        }

        public double Trace()
        {
            return _values.Trace();
        }

        public Matrix Transpose()
        {
            return _values.Transpose();
        }

        #endregion

        #region Elementwise operations

        public Matrix Abs(Matrix b)
        {
            return Elementwise.Abs(_values, AdjustRowColumn(b));
        }

        public Matrix Add(Matrix b)
        {
            return Elementwise.Add(_values, AdjustRowColumn(b));
        }

        public Matrix Ceiling()
        {
            return Elementwise.Ceiling(_values);
        }

        public Matrix Divide(Matrix b)
        {
            return Elementwise.Divide(_values, AdjustRowColumn(b));
        }

        public Matrix DivideByDiagonal(Matrix b)
        {
            return Elementwise.DivideByDiagonal(_values, EnsureSequence(b));
        }

        public Matrix Exp()
        {
            return Elementwise.Exp(_values);
        }

        public Matrix Floor()
        {
            return Elementwise.Floor(_values);
        }

        public Matrix Log()
        {
            return Elementwise.Log(_values);
        }

        public Matrix Multiply(Matrix b)
        {
            return Elementwise.Multiply(_values, AdjustRowColumn(b));
        }

        public Matrix Pow(double exp)
        {
            return Elementwise.Pow(_values, exp);
        }

        public Matrix Round()
        {
            return Elementwise.Round(_values);
        }

        public Matrix Sign()
        {
            return Elementwise.Sign(_values);
        }

        public Matrix SignedPow(double exp)
        {
            return Elementwise.SignedPow(_values, exp);
        }

        public Matrix SignSqrt()
        {
            return Elementwise.SignSqrt(_values);
        }

        public Matrix Sqrt()
        {
            return Elementwise.Sqrt(_values);
        }

        public Matrix Subtract(Matrix b)
        {
            return Elementwise.Subtract(_values, AdjustRowColumn(b));
        }

        #endregion

        #region Accord.Math.Combinatoricss

        public Matrix Combinations(int k = -1)
        {
            if (k == -1)
            {
                k = _values.Length;
            }

            var seq = EnsureSequence(_values);
            var rowCount = (int)Special.Binomial(seq.Length, k);

            var result = new double[rowCount, k];
            var row = 0;
            foreach (var set in Combinatorics.Combinations(seq, k))
            {
                for (var column = 0; column < k; ++column)
                {
                    result[row, column] = set[column];
                }
                ++row;
            }

            return result;
        }

        public Matrix Permutations()
        {
            var seq = EnsureSequence(_values);
            var rowCount = (int)Special.Factorial(seq.Length);

            var result = new double[rowCount, seq.Length];
            var row = 0;
            foreach (var set in Combinatorics.Permutations(seq))
            {
                for (var column = 0; column < seq.Length; ++column)
                {
                    result[row, column] = set[column];
                }
                ++row;
            }

            return result;
        }

        public Matrix Subsets(int k)
        {
            // TODO
            return null;
        }

        #endregion

        #region Operators

        public static Matrix operator *(Matrix a, Matrix b)
        {
            return a.Dot(b);
        }

        public static Matrix operator +(Matrix a, Matrix b)
        {
            return a.Add(b);
        }

        public static Matrix operator -(Matrix a, Matrix b)
        {
            return a.Subtract(b);
        }

        #endregion

        #region Accord.Math.Decompositions

        public CholeskyWrapper Cholesky(bool robust = false, MatrixType type = MatrixType.UpperTriangular)
        {
            return new CholeskyWrapper(new CholeskyDecomposition(_values, robust, false, type));
        }

        public GramSchmidtOrthogonalizationWrapper gramSchmidtOrthogonalizationWrapper(bool modified = true)
        {
            return new GramSchmidtOrthogonalizationWrapper(new GramSchmidtOrthogonalization(_values, modified));
        }

        public EigenvalueWrapper Eigenvalue(bool assumeSymmetric = false, bool sort = false)
        {
            return new EigenvalueWrapper(new EigenvalueDecomposition(_values, assumeSymmetric, false, sort));
        }

        public LuDecompositionWrapper Lu(bool transpose = false)
        {
            return new LuDecompositionWrapper(new LuDecomposition(_values, transpose));
        }

        public NonnegativeMatrixFactorizationWrapper Nmf(int reduceDimension, int maxIter = 10000)
        {
            return new NonnegativeMatrixFactorizationWrapper(new NonnegativeMatrixFactorization(_values, reduceDimension, maxIter));
        }

        public QrDecompositionWrapper Qr(bool transpose = false, bool economy = true)
        {
            return new QrDecompositionWrapper(new QrDecomposition(_values, transpose, economy, false));
        }

        public SingularValueDecompositionWrapper Svd(bool computeLeft = true, bool computeRight = true, bool autoTranspose = false)
        {
            return new SingularValueDecompositionWrapper(new SingularValueDecomposition(_values, computeLeft, computeRight, autoTranspose));
        }

        #endregion

        #region Accord.Statistics.Measures

        public Matrix Correlation()
        {
            return _values.Correlation();
        }

        public Matrix Covariance()
        {
            return _values.Covariance();
        }

        public double Entropy(double eps = 0)
        {
            return _values.Entropy(eps);
        }

        public Matrix ExponentialWeightedCovariance(int window, double alpha = 0, bool unbiased = false)
        {
            return ToJagged(true).ExponentialWeightedCovariance(window, alpha, unbiased);
        }

        public Matrix Mean(int dimension = 0)
        {
            return Matrix.AsVector(_values.Mean(dimension), 1 - dimension);
        }

        public Matrix Median(QuantileMethod type = QuantileMethod.Default)
        {
            return Matrix.AsVector(_values.Median(type), 1);
        }

        public Matrix Mode()
        {
            return Matrix.AsVector(_values.Mode(), 1);
        }

        public Matrix Skewness(bool unbiased = false)
        {
            return Matrix.AsVector(_values.Skewness(unbiased), 1);
        }

        public Matrix StanardDeviation()
        {
            return Matrix.AsVector(_values.StandardDeviation(), 1);
        }

        public Matrix StanardError()
        {
            return Matrix.AsVector(Measures.StandardError(_values), 1);
        }

        public Matrix Variance()
        {
            return Matrix.AsVector(_values.Variance(), 1);
        }

        public Matrix WeightedCovariance(double[] weights, int dimension = 0)
        {
            return Measures.WeightedCovariance(ToJagged(dimension), weights, 1 - dimension);
        }

        #endregion

        #region Accord.Statistics.Tools

        public Matrix Whitening(out Matrix transformMatrix)
        {
            double[,] trans;

            var result = Accord.Statistics.Tools.Whitening(_values, out trans);
            transformMatrix = trans;

            return result;
        }

        public Matrix ZScores()
        {
            return Accord.Statistics.Tools.ZScores(_values);
        }

        #endregion
    }
}
