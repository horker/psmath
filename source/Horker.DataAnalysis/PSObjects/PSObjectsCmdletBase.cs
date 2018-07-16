using System;
using System.Collections.Generic;
using System.Management.Automation;

namespace Horker.DataAnalysis.PSObjects
{
    public abstract class PSObjectsCmdletBase : PSCmdlet
    {
        [Parameter(ValueFromPipeline = true, Mandatory = false)]
        public PSObject InputObject;

        [Parameter(Position = 0, Mandatory = false)]
        public PSObject[] Data;

        private List<PSObject> _inputObjects;

        protected override void BeginProcessing()
        {
            _inputObjects = new List<PSObject>();
        }

        protected override void ProcessRecord()
        {
            _inputObjects.Add(InputObject);
        }

        protected abstract void Process(IReadOnlyList<PSObject> data);

        protected override void EndProcessing()
        {
            IReadOnlyList<PSObject> data;

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
