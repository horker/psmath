using System;
using System.Collections.Generic;
using System.IO;
using System.Management.Automation;
using System.Runtime.Serialization.Formatters.Binary;

namespace Horker.Math.PSObjects
{
    [Cmdlet("ConvertFrom", "PSObject.BinaryFormat")]
    [Alias("pso.frombinary")]
    public class ConvertFromPSObjectBinaryFormat : PSCmdlet
    {
        [Parameter(ValueFromPipeline = true, Mandatory = false)]
        public byte InputObject;

        [Parameter(Position = 0, Mandatory = true)]
        public string Path;

        [Parameter(Position = 1, Mandatory = false)]
        public byte[] Data;

        private List<byte> _inputObjects;

        protected override void BeginProcessing()
        {
            _inputObjects = new List<byte>();
        }

        protected override void ProcessRecord()
        {
            _inputObjects.Add(InputObject);
        }

        protected override void EndProcessing()
        {
            byte[] data;

            if (_inputObjects.Count == 0)
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

            using (var stream = new MemoryStream(data))
            {
                WriteObject((new BinaryFormatter()).Deserialize(stream));
            }
        }
    }
}