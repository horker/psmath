using System;
using System.Linq;
using System.Collections.Generic;
using System.Management.Automation;
using Accord.Math;
using Accord.Math.Decompositions;

namespace Horker.DataAnalysis
{
    #region Helper classes

    public class MatrixCmdletBase : PSCmdlet
    {
        public Matrix AdjustRow(Matrix matrix, int rowCount)
        {
            if (matrix.Rows == rowCount) {
                return matrix;
            }

            if (matrix.Rows != 1) {
                throw new ArgumentException("Matrix sizes are inconsistent");
            }

            return Matrix.Create(matrix.GetRow(0), rowCount, matrix.Columns, true);
        }

        public Matrix AdjustRowColumn(Matrix matrix, int rowCount, int columnCount)
        {
            if (matrix.Rows == rowCount) {
                if (matrix.Columns == columnCount) {
                    return matrix;
                }

                if (matrix.Columns == 1) {
                    return Matrix.Create(matrix.GetColumn(0), rowCount, columnCount, false);
                }
            }
            else {
                if (matrix.Rows == 1 && (matrix.Columns == columnCount || matrix.Columns == 1)) {
                    return Matrix.Create(matrix.GetRow(0), rowCount, columnCount, true);
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
                WriteObject(result.ToFlatArray());
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
                WriteObject(result.ToFlatArray());
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
            rhs = AdjustRowColumn(rhs, lhs.Rows, lhs.Columns);

            var result = Process(lhs, rhs);

            if (AsArray) {
                WriteObject(result.ToFlatArray());
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
            Matrix lhs = Converter.ToMatrix(Lhs, false);
            Matrix rhs = Converter.ToMatrix(Rhs, true);
            rhs = AdjustRow(rhs, lhs.Columns);

            var result = Process(lhs, rhs);

            if (AsArray) {
                WriteObject(result.ToFlatArray());
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

    #endregion

    #region Factory cmdlets

    [Cmdlet("New", "Matrix.Identity")]
    [Alias("mat.identity")]
    public class NewMatrixIdentity : MatrixCmdletBase
    {
        [Parameter(Position = 0, Mandatory = true)]
        public int Size;

        protected override void EndProcessing()
        {
            WriteObject(Matrix.Identity(Size));
        }
    }

    [Cmdlet("New", "Matrix.Magic")]
    [Alias("mat.magic")]
    public class NewMatrixMagic : MatrixCmdletBase
    {
        [Parameter(Position = 0, Mandatory = true)]
        public int Size;

        protected override void EndProcessing()
        {
            var result = Accord.Math.Matrix.Magic(Size);
            WriteObject(new Matrix(result, true));
        }
    }

    [Cmdlet("New", "Matrix.Mesh")]
    [Alias("mat.mesh")]
    public class NewMatrixMesh : MatrixCmdletBase
    {
        [Parameter(Position = 0, Mandatory = true)]
        public double RowMinimum;

        [Parameter(Position = 1, Mandatory = true)]
        public double RowMaximum;

        [Parameter(Position = 2, Mandatory = true)]
        public int Rows;

        [Parameter(Position = 3, Mandatory = true)]
        public double ColumnMinimum;

        [Parameter(Position = 4, Mandatory = true)]
        public double ColumnMaximum;

        [Parameter(Position = 5, Mandatory = true)]
        public int Columns;

        protected override void EndProcessing()
        {
            var result = Accord.Math.Matrix.Mesh(
                new Accord.DoubleRange(RowMinimum, RowMaximum), Rows,
                new Accord.DoubleRange(ColumnMinimum, ColumnMaximum), Columns
            );
            WriteObject(Matrix.Create(result));
        }
    }

    [Cmdlet("New", "Matrix.MeshGrid")]
    [Alias("mat.meshgrid")]
    public class NewMatrixMeshGrid : MatrixCmdletBase
    {
        [Parameter(Position = 0, Mandatory = true)]
        public object[] X;

        [Parameter(Position = 1, Mandatory = true)]
        public object[] Y;

        protected override void EndProcessing()
        {
            var xarray = X.Select(x => Converter.ToDouble(x)).ToArray();
            var yarray = Y.Select(x => Converter.ToDouble(x)).ToArray();

            var grid = Accord.Math.Matrix.MeshGrid<double>(xarray, yarray);

            var result = new PSObject();
            result.Properties.Add(new PSNoteProperty("X", new Matrix(grid.Item1, true)));
            result.Properties.Add(new PSNoteProperty("Y", new Matrix(grid.Item2, true)));

            WriteObject(result);
        }
    }

    [Cmdlet("New", "Matrix.OneHot")]
    [Alias("mat.onehot")]
    public class NewMatrixOneHot : AggregateFunctionCmdletBase
    {
        protected override void Process(double[] values)
        {
            var indexes = values.Select(x => (int)x).ToArray();
            var result = Accord.Math.Matrix.OneHot(indexes);
            WriteObject(new Matrix(result, true));
        }
    }

    [Cmdlet("New", "Matrix.Diagonal")]
    [Alias("mat.diagonal")]
    public class NewMatrixDiagonal : AggregateFunctionCmdletBase
    {
        [Parameter(Position = 1, Mandatory = true)]
        public int Rows = int.MaxValue;

        [Parameter(Position = 2, Mandatory = false)]
        public int Columns = int.MaxValue;

        protected override void Process(double[] values)
        {
            WriteObject(Matrix.Diagonal(values, Rows, Columns));
        }
    }

    [Cmdlet("New", "Matrix.Zeros")]
    [Alias("mat.zeros")]
    public class NewMatrixZero : MatrixCmdletBase
    {
        [Parameter(Position = 0, Mandatory = true)]
        public int Size;

        protected override void EndProcessing()
        {
            WriteObject(new Matrix(Size, Size));
        }
    }

    [Cmdlet("New", "Matrix.TruthTable")]
    [Alias("mat.truthtable")]
    public class NewMatrixTruthTable : PSCmdlet
    {
        [Parameter(Position = 0, Mandatory = true)]
        public object[] Symbols;

        protected override void EndProcessing()
        {
            int[] sym = Symbols.Select(x => Convert.ToInt32(x)).ToArray();
            WriteObject(Matrix.TruthTable(sym));
        }
    }

    [Cmdlet("ConvertFrom", "Matrix.Object")]
    [Alias("mat.fromobject")]
    public class ConvertFromMatrixObject : MatrixCmdletBase
    {
        [Parameter(ValueFromPipeline = true, Mandatory = false)]
        public PSObject InputObject;

        [Parameter(Position = 0, Mandatory = false)]
        public PSObject[] Objects;

        private List<PSObject> _inputObjects;

        protected override void BeginProcessing()
        {
            _inputObjects = new List<PSObject>();
        }

        protected override void ProcessRecord()
        {
            if (InputObject != null)
            {
                _inputObjects.Add(InputObject);
            }
        }

        protected override void EndProcessing()
        {
            if (Objects != null && Objects.Length > 0)
            {
                if (_inputObjects.Count > 0)
                {
                    WriteError(new ErrorRecord(new ArgumentException("Both pipeline and -Object argumetns are given"), "", ErrorCategory.InvalidArgument, null));
                    return;
                }

                WriteObject(Matrix.Create(Objects));
            }
            else
            {
                WriteObject(Matrix.Create(_inputObjects.ToArray()));
            }
        }
    }

    #endregion
}