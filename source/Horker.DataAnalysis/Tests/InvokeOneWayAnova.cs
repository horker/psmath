using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management.Automation;
using Accord.Math;
using Accord.Statistics.Analysis;
using Accord.Statistics.Testing;

namespace Horker.DataAnalysis
{
    [Cmdlet("Invoke", "OneWayAnova")]
    public class InvokeOneWayAnova : PSCmdlet
    {
        [Parameter(ValueFromPipeline = true, Mandatory = true)]
        public PSObject InputObject;

        [Parameter(Position = 0, Mandatory = true)]
        public string CategoryName;

        [Parameter(Position = 1, Mandatory = true)]
        public string ValueName;

        [Parameter(Position = 2, Mandatory = false)]
        public double Size = .05;

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
            var groups  = _data.GroupBy(CategoryName);
            var test = new OneWayAnova(groups.ToDoubleJaggedArrayOf(ValueName));

            test.FTest.Size = Size;

            WriteObject(test.Table);
        }
    }
}
