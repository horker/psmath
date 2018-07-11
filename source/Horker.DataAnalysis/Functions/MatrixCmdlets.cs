using System;
using System.Linq;
using System.Collections.Generic;
using System.Management.Automation;
using Accord.Math;

namespace Horker.DataAnalysis
{
    public class MatrixCmdletBase : PSCmdlet
    {
        public Matrix AdjustRow(Matrix matrix, int rowCount)
        {
            if (matrix.RowCount == rowCount) {
                return matrix;
            }

            if (matrix.RowCount != 1) {
                throw new ArgumentException("Matrix sizes are inconsistent");
            }

            return Matrix.Create(matrix.Row(0), rowCount, matrix.ColumnCount, true);
        }

        public Matrix AdjustRowColumn(Matrix matrix, int rowCount, int columnCount)
        {
            if (matrix.RowCount == rowCount) {
                if (matrix.ColumnCount == columnCount) {
                    return matrix;
                }

                if (matrix.ColumnCount == 1) {
                    return Matrix.Create(matrix.Column(0), rowCount, columnCount, false);
                }
            }
            else {
                if (matrix.RowCount == 1 && (matrix.ColumnCount == columnCount || matrix.ColumnCount == 1)) {
                    return Matrix.Create(matrix.Row(0), rowCount, columnCount, true);
                }
            }

            throw new ArgumentException("Matrix sizes are inconsistent");
        }
    }

    public class MatrixUnaryOperatorCmdletBase : MatrixCmdletBase
    {
        [Parameter(Position = 0, Mandatory = true)]
        public object Value;

        [Parameter(Position = 9, Mandatory = false)]
        public SwitchParameter AsArray;

        protected virtual Matrix Process(Matrix value) { return null; }

        protected override void EndProcessing()
        {
            Matrix value = Converter.ToMatrix(Value, true);

            var result = Process(value);

            if (AsArray) {
                WriteObject(result.ToArray());
            }
            else {
                WriteObject(result);
            }
        }
    }

    public class MatrixBinaryOperatorCmdletBase : MatrixCmdletBase
    {
        [Parameter(Position = 0, Mandatory = true)]
        public object Lhs;

        [Parameter(Position = 1, Mandatory = true)]
        public object Rhs;

        [Parameter(Position = 9, Mandatory = false)]
        public SwitchParameter AsArray;

        protected virtual Matrix Process(Matrix lhs, Matrix rhs) { return null; }

        protected override void EndProcessing()
        {
            Matrix lhs = Converter.ToMatrix(Lhs, true);
            Matrix rhs = Converter.ToMatrix(Rhs, true);

            var result = Process(lhs, rhs);

            if (AsArray) {
                WriteObject(result.ToArray());
            }
            else {
                WriteObject(result);
            }
        }
    }

    public class MatrixElementwiseOperatorCmdletBase : MatrixCmdletBase
    {
        [Parameter(Position = 0, Mandatory = true)]
        public object Lhs;

        [Parameter(Position = 1, Mandatory = true)]
        public object Rhs;

        [Parameter(Position = 9, Mandatory = false)]
        public SwitchParameter AsArray;

        protected virtual Matrix Process(Matrix lhs, Matrix rhs) { return null; }

        protected override void EndProcessing()
        {
            Matrix lhs = Converter.ToMatrix(Lhs, true);
            Matrix rhs = Converter.ToMatrix(Rhs, true);
            rhs = AdjustRowColumn(rhs, lhs.RowCount, lhs.ColumnCount);

            var result = Process(lhs, rhs);

            if (AsArray) {
                WriteObject(result.ToArray());
            }
            else {
                WriteObject(result);
            }
        }
    }

    public class MatrixMultiplyOperatorCmdletBase : MatrixBinaryOperatorCmdletBase
    {
        protected override void EndProcessing()
        {
            Matrix lhs = Converter.ToMatrix(Lhs, true);
            Matrix rhs = Converter.ToMatrix(Rhs, false);
            rhs = AdjustRow(rhs, lhs.ColumnCount);

            var result = Process(lhs, rhs);

            if (AsArray) {
                WriteObject(result.ToArray());
            }
            else {
                WriteObject(result);
            }
        }
    }

    public class MatrixTestOperatorCmdletBase : MatrixCmdletBase
    {
        [Parameter(Position = 0, Mandatory = true)]
        public object Value;

        protected virtual bool Process(Matrix value) { return false; }

        protected override void EndProcessing()
        {
            var value = Converter.ToMatrix(Value, true);

            var result = Process(value);

            WriteObject(result);
        }
    }

    [Cmdlet("Get", "Matrix.Dot")]
    [Alias("mat.dot")]
    public class GetMatrixDot : MatrixMultiplyOperatorCmdletBase
    {
        protected override Matrix Process(Matrix lhs, Matrix rhs)
        {
            return new Matrix(Accord.Math.Matrix.Dot(lhs, rhs), true);
        }
    }

    #region Elementwise operators

    [Cmdlet("Get", "Matrix.Abs")]
    [Alias("mat.abs")]
    public class GetMatrixAbs : MatrixUnaryOperatorCmdletBase
    {
        protected override Matrix Process(Matrix value)
        {
            return new Matrix(Accord.Math.Elementwise.Abs(value), true);
        }
    }

    [Cmdlet("Get", "Matrix.Add")]
    [Alias("mat.add")]
    public class GetMatrixAdd : MatrixElementwiseOperatorCmdletBase
    {
        protected override Matrix Process(Matrix lhs, Matrix rhs)
        {
            return new Matrix(Accord.Math.Elementwise.Add(lhs, rhs), true);
        }
    }

    [Cmdlet("Get", "Matrix.Ceiling")]
    [Alias("mat.ceiling")]
    public class GetMatrixCeiling : MatrixUnaryOperatorCmdletBase
    {
        protected override Matrix Process(Matrix value)
        {
            return new Matrix(Accord.Math.Elementwise.Ceiling(value), true);
        }
    }

    [Cmdlet("Get", "Matrix.Divide")]
    [Alias("mat.div")]
    public class GetMatrixDivide : MatrixElementwiseOperatorCmdletBase
    {
        protected override Matrix Process(Matrix lhs, Matrix rhs)
        {
            return new Matrix(Accord.Math.Elementwise.Divide(lhs, rhs), true);
        }
    }

    [Cmdlet("Get", "Matrix.Exp")]
    [Alias("mat.exp")]
    public class GetMatrixExp : MatrixUnaryOperatorCmdletBase
    {
        protected override Matrix Process(Matrix value)
        {
            return new Matrix(Accord.Math.Elementwise.Exp(value), true);
        }
    }

    [Cmdlet("Get", "Matrix.Floor")]
    [Alias("mat.floor")]
    public class GetMatrixFloor : MatrixUnaryOperatorCmdletBase
    {
        protected override Matrix Process(Matrix value)
        {
            return new Matrix(Accord.Math.Elementwise.Floor(value), true);
        }
    }

    [Cmdlet("Get", "Matrix.Log")]
    [Alias("mat.log")]
    public class GetMatrixLog : MatrixUnaryOperatorCmdletBase
    {
        protected override Matrix Process(Matrix value)
        {
            return new Matrix(Accord.Math.Elementwise.Log(value), true);
        }
    }

    [Cmdlet("Get", "Matrix.Multiply")]
    [Alias("mat.mul")]
    public class GetMatrixMultiply : MatrixElementwiseOperatorCmdletBase
    {
        protected override Matrix Process(Matrix lhs, Matrix rhs)
        {
            return new Matrix(Accord.Math.Elementwise.Multiply(lhs, rhs), true);
        }
    }

    [Cmdlet("Get", "Matrix.Pow")]
    [Alias("mat.pow")]
    public class GetMatrixPow : MatrixUnaryOperatorCmdletBase
    {
        [Parameter(Position = 1, Mandatory = true)]
        public double Exp;

        protected override Matrix Process(Matrix value)
        {
            return new Matrix(Accord.Math.Elementwise.Pow(value, Exp), true);
        }
    }

    [Cmdlet("Get", "Matrix.Round")]
    [Alias("mat.round")]
    public class GetMatrixRound : MatrixUnaryOperatorCmdletBase
    {
        protected override Matrix Process(Matrix value)
        {
            return new Matrix(Accord.Math.Elementwise.Round(value), true);
        }
    }

    [Cmdlet("Get", "Matrix.Sign")]
    [Alias("mat.sign")]
    public class GetMatrixSign : MatrixUnaryOperatorCmdletBase
    {
        protected override Matrix Process(Matrix value)
        {
            return new Matrix(Accord.Math.Elementwise.Sign(value), true);
        }
    }

    [Cmdlet("Get", "Matrix.SignedPow")]
    [Alias("mat.signedpow")]
    public class GetMatrixSignedPow : MatrixUnaryOperatorCmdletBase
    {
        [Parameter(Position = 1, Mandatory = true)]
        public double Exp;

        protected override Matrix Process(Matrix value)
        {
            return new Matrix(Accord.Math.Elementwise.SignedPow(value, Exp), true);
        }
    }

    [Cmdlet("Get", "Matrix.SignSqrt")]
    [Alias("mat.signsqrt")]
    public class GetMatrixSignSqrt : MatrixUnaryOperatorCmdletBase
    {
        protected override Matrix Process(Matrix value)
        {
            return new Matrix(Accord.Math.Elementwise.SignSqrt(value), true);
        }
    }

    [Cmdlet("Get", "Matrix.Sqrt")]
    [Alias("mat.sqrt")]
    public class GetMatrixSqrt : MatrixUnaryOperatorCmdletBase
    {
        protected override Matrix Process(Matrix value)
        {
            return new Matrix(Accord.Math.Elementwise.Sqrt(value), true);
        }
    }

    [Cmdlet("Get", "Matrix.Subtract")]
    [Alias("mat.sub")]
    public class GetMatrixSubtract : MatrixElementwiseOperatorCmdletBase
    {
        protected override Matrix Process(Matrix lhs, Matrix rhs)
        {
            return new Matrix(Accord.Math.Elementwise.Subtract(lhs, rhs), true);
        }
    }

    #endregion

    #region Operators

    [Cmdlet("Get", "Matrix.Apply")]
    [Alias("mat.apply")]
    public class GetMatrixApply : MatrixUnaryOperatorCmdletBase
    {
        [Parameter(Position = 1, Mandatory = true)]
        public Func<double, double> Func;

        protected override Matrix Process(Matrix value)
        {
            return new Matrix(Accord.Math.Matrix.Apply(value, Func));
        }
    }

    [Cmdlet("Get", "Matrix.CumlativeSum")]
    [Alias("mat.cumsum")]
    public class GetMatrixCumlativeSum : MatrixUnaryOperatorCmdletBase
    {
        [Parameter(Position = 1, Mandatory = false)]
        public int Dimension = 0;

        protected override Matrix Process(Matrix value)
        {
            return new Matrix(Accord.Math.Matrix.CumulativeSum(value, Dimension));
        }
    }

    [Cmdlet("Get", "Matrix.Decompose")]
    [Alias("mat.decompose")]
    public class GetMatrixDecompose : MatrixCmdletBase
    {
        [Parameter(Position = 0, Mandatory = true)]
        public object Value;

        [Parameter(Position = 1, Mandatory = false)]
        public SwitchParameter LeastSquares;

        protected override void EndProcessing()
        {
            var value = Converter.ToMatrix(Value, true);
            WriteObject(Accord.Math.Matrix.Decompose(value, LeastSquares));
        }
    }

    [Cmdlet("Get", "Matrix.Determinant")]
    [Alias("mat.det")]
    public class GetMatrixDeterminant : MatrixCmdletBase
    {
        [Parameter(Position = 0, Mandatory = true)]
        public object Value;

        protected override void EndProcessing()
        {
            var value = Converter.ToMatrix(Value, true);
            WriteObject(Accord.Math.Matrix.Determinant(value));
        }
    }

    [Cmdlet("Get", "Matrix.Distinct")]
    [Alias("mat.distinct")]
    public class GetMatrixDictinct : MatrixCmdletBase
    {
        [Parameter(Position = 0, Mandatory = true)]
        public object Value;

        protected override void EndProcessing()
        {
            var value = Converter.ToMatrix(Value, true);
            WriteObject(Accord.Math.Matrix.Distinct<double>(value));
        }
    }

    [Cmdlet("Get", "Matrix.DistinctCount")]
    [Alias("mat.distinctcount")]
    public class GetMatrixDictinctCount : MatrixCmdletBase
    {
        [Parameter(Position = 0, Mandatory = true)]
        public object Value;

        protected override void EndProcessing()
        {
            var value = Converter.ToMatrix(Value, true);
            WriteObject(Accord.Math.Matrix.DistinctCount<double>(value));
        }
    }

    [Cmdlet("Get", "Matrix.DivideByDiagonal")]
    [Alias("mat.divdiagonal")]
    public class GetMatrixDivideByDiagonal : MatrixUnaryOperatorCmdletBase
    {
        [Parameter(Position = 1, Mandatory = true)]
        public object B;

        protected override Matrix Process(Matrix value)
        {
            var b = Converter.ToDoubleArray(B);

            return new Matrix(Accord.Math.Matrix.DivideByDiagonal(value, b), true);
        }
    }

    [Cmdlet("Get", "Matrix.Identity")]
    [Alias("mat.identity")]
    public class GetMatrixIdentity : MatrixCmdletBase
    {
        [Parameter(Position = 0, Mandatory = true)]
        public int Size;

        protected override void EndProcessing()
        {
            WriteObject(Matrix.Identity(Size));
        }
    }

    [Cmdlet("Get", "Matrix.LowerTriangle")]
    [Alias("mat.lowertri")]
    public class GetMatrixLowerTriangle : MatrixUnaryOperatorCmdletBase
    {
        [Parameter(Position = 1, Mandatory = false)]
        public SwitchParameter ExcludeDiagonal;

        protected override Matrix Process(Matrix value)
        {
            return new Matrix(Accord.Math.Matrix.GetLowerTriangle<double>(value, !ExcludeDiagonal), true);
        }
    }

    [Cmdlet("Get", "Matrix.Symmetric")]
    [Alias("mat.symmetric")]
    public class GetMatrixSymmetric : MatrixUnaryOperatorCmdletBase
    {
        [Parameter(Position = 1, Mandatory = false)]
        public Accord.Math.MatrixType Type = MatrixType.UpperTriangular;

        protected override Matrix Process(Matrix value)
        {
            return new Matrix(Accord.Math.Matrix.GetSymmetric<double>(value, Type, null), true);
        }
    }

    [Cmdlet("Get", "Matrix.UpperTriangle")]
    [Alias("mat.uppertri")]
    public class GetMatrixUpperTriangle : MatrixUnaryOperatorCmdletBase
    {
        [Parameter(Position = 1, Mandatory = false)]
        public SwitchParameter ExcludeDiagonal;

        protected override Matrix Process(Matrix value)
        {
            return new Matrix(Accord.Math.Matrix.GetUpperTriangle<double>(value, !ExcludeDiagonal), true);
        }
    }

    [Cmdlet("Get", "Matrix.Inverse")]
    [Alias("mat.inv")]
    public class GetMatrixInverse : MatrixUnaryOperatorCmdletBase
    {
        protected override Matrix Process(Matrix value)
        {
            return new Matrix(Accord.Math.Matrix.Inverse(value), true);
        }
    }

    [Cmdlet("Get", "Matrix.Kronecker")]
    [Alias("mat.kronecker")]
    public class GetMatrixKronecker : MatrixBinaryOperatorCmdletBase
    {
        protected override Matrix Process(Matrix lhs, Matrix rhs)
        {
            return new Matrix(Accord.Math.Matrix.Kronecker(lhs, rhs), true);
        }
    }

    [Cmdlet("Get", "Matrix.LogDeterminant")]
    [Alias("mat.logdet")]
    public class GetMatrixLogDeterminant : MatrixCmdletBase
    {
        [Parameter(Position = 0, Mandatory = true)]
        public object Value;

        protected override void EndProcessing()
        {
            var value = Converter.ToMatrix(Value, true);
            WriteObject(Accord.Math.Matrix.LogDeterminant(value));
        }
    }

    [Cmdlet("Get", "Matrix.LogPseudoDeterminant")]
    [Alias("mat.logpseudodet")]
    public class GetMatrixLogPseudoDeterminant : MatrixCmdletBase
    {
        [Parameter(Position = 0, Mandatory = true)]
        public object Value;

        protected override void EndProcessing()
        {
            var value = Converter.ToMatrix(Value, true);
            WriteObject(Accord.Math.Matrix.LogPseudoDeterminant(value));
        }
    }

    [Cmdlet("Get", "Matrix.Magic")]
    [Alias("mat.magic")]
    public class GetMatrixMagic : MatrixCmdletBase
    {
        [Parameter(Position = 0, Mandatory = true)]
        public int Size;

        protected override void EndProcessing()
        {
            var result = Accord.Math.Matrix.Magic(Size);
            WriteObject(new Matrix(result, false));
        }
    }

    [Cmdlet("Get", "Matrix.Mesh")]
    [Alias("mat.mesh")]
    public class GetMatrixMesh : MatrixCmdletBase
    {
        [Parameter(Position = 0, Mandatory = true)]
        public double RowMinimum;

        [Parameter(Position = 1, Mandatory = true)]
        public double RowMaximum;

        [Parameter(Position = 2, Mandatory = true)]
        public int RowCount;

        [Parameter(Position = 3, Mandatory = true)]
        public double ColumnMinimum;

        [Parameter(Position = 4, Mandatory = true)]
        public double ColumnMaximum;

        [Parameter(Position = 5, Mandatory = true)]
        public int ColumnCount;

        protected override void EndProcessing()
        {
            var result = Accord.Math.Matrix.Mesh(
                new Accord.DoubleRange(RowMinimum, RowMaximum), RowCount,
                new Accord.DoubleRange(ColumnMinimum, ColumnMaximum), ColumnCount
            );
            WriteObject(Matrix.Create(result));
        }
    }

    [Cmdlet("Get", "Matrix.OneHot")]
    [Alias("mat.onehot")]
    public class GetMatrixOneHot : AggregateFunctionCmdletBase
    {
        protected override void Process(double[] values)
        {
            var indexes = values.Select(x => (int)x).ToArray();
            var result = Accord.Math.Matrix.OneHot(indexes);
            WriteObject(new Matrix(result, false));
        }
    }

    [Cmdlet("Get", "Matrix.Product")]
    [Alias("mat.product")]
    public class GetMatrixProduct : MatrixCmdletBase
    {
        [Parameter(Position = 0, Mandatory = true)]
        public object Value;

        protected override void EndProcessing()
        {
            var value = Converter.ToMatrix(Value, true);
            WriteObject(Accord.Math.Matrix.Product(value));
        }
    }

    [Cmdlet("Get", "Matrix.PseudoInverse")]
    [Alias("mat.pseudoinv")]
    public class GetMatrixPseudoInverse : MatrixUnaryOperatorCmdletBase
    {
        protected override Matrix Process(Matrix value)
        {
            return new Matrix(Accord.Math.Matrix.PseudoInverse(value), true);
        }
    }

    [Cmdlet("Get", "Matrix.Rank")]
    [Alias("mat.rank")]
    public class GetMatrixRank : MatrixCmdletBase
    {
        [Parameter(Position = 0, Mandatory = true)]
        public object Value;

        protected override void EndProcessing()
        {
            var value = Converter.ToMatrix(Value, true);
            WriteObject(Accord.Math.Matrix.Rank(value));
        }
    }

    [Cmdlet("Get", "Matrix.Solve")]
    [Alias("mat.solve")]
    public class GetMatrixSolve : MatrixBinaryOperatorCmdletBase
    {
        [Parameter(Position = 0, Mandatory = false)]
        public SwitchParameter LeastSquares;

        protected override Matrix Process(Matrix lhs, Matrix rhs)
        {
            return new Matrix(Accord.Math.Matrix.Solve(lhs, rhs, LeastSquares), true);
        }
    }

    [Cmdlet("Get", "Matrix.Sum")]
    [Alias("mat.sum")]
    public class GetMatrixSum : MatrixCmdletBase
    {
        [Parameter(Position = 0, Mandatory = true)]
        public object Value;

        protected override void EndProcessing()
        {
            var value = Converter.ToMatrix(Value, true);
            WriteObject(Accord.Math.Matrix.Sum(value));
        }
    }

    [Cmdlet("Get", "Matrix.Trace")]
    [Alias("mat.trace")]
    public class GetMatrixTrace : MatrixCmdletBase
    {
        [Parameter(Position = 0, Mandatory = true)]
        public object Value;

        protected override void EndProcessing()
        {
            var value = Converter.ToMatrix(Value, true);
            WriteObject(Accord.Math.Matrix.Trace(value));
        }
    }

    [Cmdlet("Get", "Matrix.Transpose")]
    [Alias("mat.t")]
    public class GetMatrixTranspose : MatrixUnaryOperatorCmdletBase
    {
        protected override Matrix Process(Matrix value)
        {
            return new Matrix(Accord.Math.Matrix.Transpose<double>(value), true);
        }
    }

    #endregion

    #region Tester

    [Cmdlet("Test", "Matrix.Infinity")]
    [Alias("mat.hasinf")]
    public class TestMatrixInfinity : MatrixTestOperatorCmdletBase
    {
        protected override bool Process(Matrix value)
        {
            return Accord.Math.Matrix.HasInfinity(value);
        }
    }

    [Cmdlet("Test", "Matrix.NaN")]
    [Alias("mat.hasnan")]
    public class TestMatrixNaN : MatrixTestOperatorCmdletBase
    {
        protected override bool Process(Matrix value)
        {
            return Accord.Math.Matrix.HasNaN(value);
        }
    }

    [Cmdlet("Test", "Matrix.Diagonal")]
    [Alias("mat.isdiagonal")]
    public class TestMatrixDiagonal : MatrixTestOperatorCmdletBase
    {
        protected override bool Process(Matrix value)
        {
            return Accord.Math.Matrix.IsDiagonal<double>(value);
        }
    }

    [Cmdlet("Test", "Matrix.LowerTriangular")]
    [Alias("mat.islowertri")]
    public class TestMatrixLowerTriangular : MatrixTestOperatorCmdletBase
    {
        protected override bool Process(Matrix value)
        {
            return Accord.Math.Matrix.IsLowerTriangular<double>(value);
        }
    }

    [Cmdlet("Test", "Matrix.UpperTriangular")]
    [Alias("mat.isuppertri")]
    public class TestMatrixUpperTriangular : MatrixTestOperatorCmdletBase
    {
        protected override bool Process(Matrix value)
        {
            return Accord.Math.Matrix.IsUpperTriangular<double>(value);
        }
    }

    [Cmdlet("Test", "Matrix.PositiveDefinite")]
    [Alias("mat.ispositive")]
    public class TestMatrixPositiveDefinite : MatrixTestOperatorCmdletBase
    {
        protected override bool Process(Matrix value)
        {
            return Accord.Math.Matrix.IsPositiveDefinite(value);
        }
    }

    [Cmdlet("Test", "Matrix.Singular")]
    [Alias("mat.issingular")]
    public class TestMatrixSingular : MatrixTestOperatorCmdletBase
    {
        protected override bool Process(Matrix value)
        {
            return Accord.Math.Matrix.IsSingular(value);
        }
    }

    [Cmdlet("Test", "Matrix.Symmetric")]
    [Alias("mat.issymmetric")]
    public class TestMatrixSymmetric : MatrixTestOperatorCmdletBase
    {
        protected override bool Process(Matrix value)
        {
            return Accord.Math.Matrix.IsSymmetric(value);
        }
    }

    #endregion
}