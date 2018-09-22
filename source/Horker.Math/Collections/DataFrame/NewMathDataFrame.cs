using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Diagnostics;

namespace Horker.Math
{
    [Cmdlet("New", "Math.DataFrame")]
    [Alias("math.df")]
    [OutputType(typeof(DataFrame))]
    public class NewMathDataFrame : PSCmdlet
    {
        [Parameter(ValueFromPipeline = true, Mandatory = false)]
        public PSObject InputObject;

        private DataFrame _df;

        protected override void BeginProcessing()
        {
            _df = new DataFrame();
        }

        protected override void ProcessRecord()
        {
            if (InputObject != null)
                _df.AddRow(InputObject);
        }

        protected override void EndProcessing()
        {
            WriteObject(_df);
        }
    }
}