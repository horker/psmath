using System;
using System.Collections.Generic;
using System.Management.Automation;

namespace Horker.Math
{
    public abstract class ObjectListCmdletBase : PSCmdlet
    {
        [Parameter(ValueFromPipeline = true, Mandatory = false)]
        public object InputObject;

        [Parameter(Position = 0, Mandatory = false)]
        public object[] Data;

        private List<object> _inputObjects;

        protected abstract void Process(IReadOnlyList<object> data);

        protected override void BeginProcessing()
        {
            _inputObjects = new List<object>();
        }

        protected override void ProcessRecord()
        {
            _inputObjects.Add(InputObject);
        }

        protected override void EndProcessing()
        {
            IReadOnlyList<object> data;

            if (_inputObjects.Count == 0 || (_inputObjects.Count == 1 && _inputObjects[0] == null))
            {
                if (Data == null || Data.Length == 0)
                {
                    WriteError(new ErrorRecord(new ArgumentException("No input specified"), "", ErrorCategory.InvalidArgument, null));
                    return;
                }
                data = Data;
            }
            else
            {
                if (Data != null && Data.Length > 0)
                {
                    WriteError(new ErrorRecord(new ArgumentException("Both pipeline and -Data arguments specified"), "", ErrorCategory.InvalidArgument, null));
                    return;
                }
                data = _inputObjects;
            }

            Process(data);
        }
    }
}
