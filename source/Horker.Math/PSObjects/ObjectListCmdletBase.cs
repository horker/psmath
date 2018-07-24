using System;
using System.Collections.Generic;
using System.Management.Automation;

namespace Horker.Math
{
    public abstract class ObjectListCmdletBase<T> : PSCmdlet
    {
        [Parameter(ValueFromPipeline = true, Mandatory = false)]
        public T InputObject;

        [Parameter(Position = 0, Mandatory = false)]
        public T[] Data;

        private List<T> _inputObjects;

        protected abstract void Process(IReadOnlyList<T> data);

        protected override void BeginProcessing()
        {
            _inputObjects = new List<T>();
        }

        protected override void ProcessRecord()
        {
            _inputObjects.Add(InputObject);
        }

        protected override void EndProcessing()
        {
            IReadOnlyList<T> data;

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
