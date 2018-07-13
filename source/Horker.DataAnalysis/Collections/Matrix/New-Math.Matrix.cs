using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Diagnostics;

namespace Horker.DataAnalysis
{
    [Cmdlet("New", "Math.Matrix")]
    [Alias("mat")]
    public class NewMathMatrix : PSCmdlet
    {
        [Parameter(ValueFromPipeline = true, Mandatory = false)]
        public object InputObject;

        [Parameter(Position = 0, Mandatory = false)]
        public object Values;

        [Parameter(Position = 1, Mandatory = false)]
        public int RowCount = int.MaxValue;

        [Parameter(Position = 2, Mandatory = false)]
        public int ColumnCount = int.MaxValue;

        [Parameter(Position = 3, Mandatory = false)]
        public SwitchParameter Transpose = false;

        [Parameter(Position = 4, Mandatory = false)]
        public SwitchParameter FromJagged;

        private List<object> _inputObjects;

        protected override void BeginProcessing()
        {
            _inputObjects = new List<object>();
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
            IReadOnlyList<object> array;
            Matrix matrix = null;

            if (Values != null)
            {
                if (_inputObjects.Count > 0)
                {
                    WriteError(new ErrorRecord(new ArgumentException("Both pipeline and -Value argumetns are given"), "", ErrorCategory.InvalidArgument, null));
                    return;
                }

                if (Values is IReadOnlyList<object>)
                {
                    array = (IReadOnlyList<object>)Values;
                }
                else
                {
                    array = new object[1] { Values };
                }
            }
            else
            {
                if (_inputObjects.Count == 0)
                {
                    WriteError(new ErrorRecord(new ArgumentException("Values are required"), "", ErrorCategory.InvalidArgument, null));
                    return;
                }
                array = _inputObjects;
            }

            if (FromJagged)
            {
                matrix = Matrix.Create(Converter.ToDoubleJaggedArray(array));
            }
            else
            {
                if (array.Count == 1)
                {
                    if (array[0] is double[,])
                    {
                        matrix = new Matrix(array[0] as double[,]);
                    }
                    else if (array[0] is object[,] || array[0] is float[,])
                    {
                        var a = array[0] as object[,];
                        var da = new double[a.GetLength(0), a.GetLength(1)];

                        for (var column = 0; column < a.GetLength(1); ++column)
                        {
                            for (var row = 0; row < a.GetLength(0); ++row)
                            {
                                da[row, column] = Converter.ToDouble(a[row, column]);
                            }
                        }
                        matrix = new Matrix(da);
                    }
                }
            }

            if (matrix == null)
            {
                var da = array.Select(x => Converter.ToDouble(x));
                matrix = Matrix.Create(da.ToArray(), RowCount, ColumnCount);
            }

            if (Transpose)
            {
                matrix = matrix.T;
            }

            WriteObject(matrix);
        }
    }
}
