using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accord.Statistics;
using System.Management.Automation;
using Accord.Math;
using Accord.Statistics.Analysis;
using Accord.Statistics.Testing;

namespace Horker.DataAnalysis
{
    [Cmdlet("Invoke", "FisherExactTest")]
    public class InvokeFisherExactTest : PSCmdlet
    {
        [Parameter(ValueFromPipeline = true, Mandatory = true)]
        public PSObject InputObject;

        [Parameter(Position = 0, Mandatory = false)]
        [ValidateSet("Different", "Greater", "Smaller")]
        public string Alternate = "Different";

        [Parameter(Position = 1, Mandatory = false)]
        public double Size = 0.05;

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
            var hypo = TestingHelper.GetOneSampleHypothesis(Alternate);

            var mat = new ConfusionMatrix(_data.ToIntArray());
            var test = new FisherExactTest(mat, hypo) { Size = Size };

            WriteObject(test);
        }
    }
}
