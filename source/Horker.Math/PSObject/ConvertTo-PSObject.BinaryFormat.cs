using System;
using System.Collections.Generic;
using System.IO;
using System.Management.Automation;
using System.Runtime.Serialization.Formatters.Binary;

namespace Horker.Math.PSObjects
{
    [Cmdlet("ConvertTo", "PSObject.BinaryFormat")]
    [Alias("pso.tobinary")]
    public class ConvertToPSObjectBinaryFormat : PSCmdlet
    {
        [Parameter(ValueFromPipeline = true, Mandatory = false)]
        public object InputObject;

        [Parameter(Position = 0, Mandatory = true)]
        public string Path;

        [Parameter(Position = 1, Mandatory = false)]
        public object Data;

        private List<object> _inputObjects;

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
            object data;

            if (_inputObjects.Count == 0 || (_inputObjects.Count == 1 && _inputObjects[0] == null))
            {
                if (Data == null)
                {
                    WriteError(new ErrorRecord(new ArgumentException("No input specified"), "", ErrorCategory.InvalidArgument, null));
                    return;
                }
                data = Data;
            }
            else
            {
                if (Data != null)
                {
                    WriteError(new ErrorRecord(new ArgumentException("Both pipeline and -Data arguments specified"), "", ErrorCategory.InvalidArgument, null));
                    return;
                }
                data = _inputObjects.ToArray();
            }

            if (!System.IO.Path.IsPathRooted(Path))
            {
                var current = SessionState.Path.CurrentFileSystemLocation;
                Path = SessionState.Path.Combine(current.ToString(), Path);
            }

            using (var stream = new MemoryStream())
            {
                (new BinaryFormatter()).Serialize(stream, data);

                WriteObject(stream.ToArray());
            }
        }
    }
}