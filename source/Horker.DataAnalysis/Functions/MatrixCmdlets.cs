using System;
using System.Collections.Generic;
using System.Management.Automation;
using Accord.Math;

namespace Horker.DataAnalysis
{
    public class MatrixCmdletBase : PSCmdlet
    {
        [Parameter(Position = 9, Mandatory = false)]
        public SwitchParameter AsArray;

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
                    return Matrix.Create(matrix.Row(0), rowCount, columnCount, false);
                }
            }

            throw new ArgumentException("Matrix sizes are inconsistent");
        }
    }

    public class MatrixUnaryOperatorCmdletBase : MatrixCmdletBase
    {
        [Parameter(Position = 0, Mandatory = true)]
        public object Value;

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
    public class GetMatrixExp : MatrixElementwiseOperatorCmdletBase
    {
        protected override Matrix Process(Matrix lhs, Matrix rhs)
        {
            return new Matrix(Accord.Math.Elementwise.Exp(lhs, rhs), true);
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
}
