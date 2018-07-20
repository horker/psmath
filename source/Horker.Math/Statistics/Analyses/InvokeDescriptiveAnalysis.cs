using System;
using System.Linq;
using System.Data;
using System.Collections.Generic;
using System.Management.Automation;
using Accord.Statistics.Analysis;
using Accord.Statistics.Filters;
using Accord.Math;

namespace Horker.Math
{
    [Cmdlet("Invoke", "DescriptiveAnalysis")]
    public class InvokeDescriptiveAnalysis : PSCmdlet
    {
        [Parameter(ValueFromPipeline = true, Mandatory = true)]
        public PSObject InputObject;

        private DataFrame _data;

        protected override void BeginProcessing()
        {
            _data = new DataFrame();
        }

        protected override void ProcessRecord()
        {
            _data.AddRow(InputObject);
        }

        protected override void EndProcessing()
        {
            var d = new DescriptiveAnalysis(_data.ColumnNames.ToArray());

            d.Learn(_data.ToDoubleJaggedArray());

            // TODO: Convert output to prettyprint or define format data

            WriteObject(d);
        }
    }
}