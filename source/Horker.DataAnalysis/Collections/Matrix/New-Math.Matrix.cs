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
        public int Rows = int.MaxValue;

        [Parameter(Position = 2, Mandatory = false)]
        public int Columns = int.MaxValue;

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
            object values;

            if (Values != null)
            {
                if (_inputObjects.Count > 1 && !(_inputObjects.Count == 1 && _inputObjects[0] == null))
                {
                    WriteError(new ErrorRecord(new ArgumentException("Both pipeline and -Value argumetns are given"), "", ErrorCategory.InvalidArgument, null));
                    return;
                }
                values = Values;
            }
            else
            {
                if (_inputObjects.Count == 0 || (_inputObjects.Count == 1 && _inputObjects[0] == null))
                {
                    WriteError(new ErrorRecord(new ArgumentException("Values are required"), "", ErrorCategory.InvalidArgument, null));
                    return;
                }
                values = _inputObjects;
            }

            Matrix matrix = null;
            if (FromJagged)
                matrix = Matrix.Create(Converter.ToDoubleJaggedArray(values));
            else
                matrix = Matrix.Create(Converter.ToDoubleArray(values), Rows, Columns);

            if (Transpose)
                matrix = matrix.T;

            WriteObject(matrix);
        }
    }
}
