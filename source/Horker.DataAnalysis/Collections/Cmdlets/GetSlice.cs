using System;
using System.Collections;
using System.Collections.Generic;
using System.Management.Automation;

namespace Horker.DataAnalysis
{
    [Cmdlet("Get", "Slice")]
    [Alias("slice")]
    public class GetSlice : PSCmdlet
    {
        [Parameter(Position = 0, Mandatory = true)]
        public object[] Array;

        [Parameter(Position = 1, Mandatory = true)]
        public int Offset;

        [Parameter(Position = 2, Mandatory = true)]
        public int Count;

        protected override void EndProcessing()
        {
            WriteObject(new Slice<object>(Array, Offset, Count));
        }
    }
}
