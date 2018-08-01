using System;
using System.Collections.Generic;
using System.IO;
using System.Management.Automation;
using System.Runtime.Serialization.Formatters.Binary;

namespace Horker.Math.PSObjects
{
    [Cmdlet("Import", "PSObject.BinaryFormat")]
    [Alias("pso.importbinary")]
    public class ImportPSObjectBinaryFormat : PSCmdlet
    {
        [Parameter(Position = 0, Mandatory = true)]
        public string Path;

        protected override void EndProcessing()
        {
            if (!System.IO.Path.IsPathRooted(Path))
            {
                var current = SessionState.Path.CurrentFileSystemLocation;
                Path = SessionState.Path.Combine(current.ToString(), Path);
            }

            using (var stream = new FileStream(Path, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                WriteObject((new BinaryFormatter()).Deserialize(stream));
            }
        }
    }
}