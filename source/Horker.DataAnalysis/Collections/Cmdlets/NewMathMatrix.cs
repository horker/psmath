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
        public object[] Values;

        [Parameter(Position = 1, Mandatory = false)]
        public int RowCount = int.MaxValue;

        [Parameter(Position = 2, Mandatory = false)]
        public int ColumnCount = int.MaxValue;

        [Parameter(Position = 3, Mandatory = false)]
        public SwitchParameter Transpose = false;

        [Parameter(Position = 4, Mandatory = false)]
        public SwitchParameter FromJagged;

        [Parameter(Position = 5, Mandatory = false)]
        public SwitchParameter Diagonal;

        private List<object> _values;

        protected override void BeginProcessing()
        {
            _values = new List<object>();
        }

        protected override void ProcessRecord()
        {
            if (InputObject != null) {
                _values.Add(InputObject);
            }
        }

        protected override void EndProcessing()
        {
            IReadOnlyList<object> v;

            if (Values != null) {
                if (_values.Count > 0) {
                    WriteError(new ErrorRecord(new ArgumentException("Both pipeline and -Value argumetns are given"), "", ErrorCategory.InvalidArgument, null));
                    return;
                }
                v = Values;
            }
            else {
                v = _values;
            }

            Matrix matrix;
            if (FromJagged) {
                matrix = Matrix.Create(Converter.ToDoubleJaggedArray(v));
            }
            else if (Diagonal) {
                if (v.Count != 1) {
                    WriteError(new ErrorRecord(new ArgumentException("Single value is acceptable for -Diagonal"), "", ErrorCategory.InvalidArgument, null));
                    return;
                }
                matrix = Matrix.Diagonal(Converter.ToDouble(v[0]), RowCount, ColumnCount);
            }
            else {
                var numbers = v.Select(x => Converter.ToDouble(x));
                matrix = Matrix.Create(numbers.ToArray(), RowCount, ColumnCount, Transpose);
            }

            WriteObject(matrix);
        }
    }
}
