using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management.Automation;
using Accord.Statistics.Testing;

namespace Horker.DataAnalysis
{
    [Cmdlet("Invoke", "TwoSampleSignTest")]
    [CmdletBinding(DefaultParameterSetName = "Pipeline")]
    public class InvokeTwoSampleSignTest : PSCmdlet
    {
        [Parameter(ValueFromPipeline = true, Mandatory = true, ParameterSetName = "Pipeline")]
        public PSObject InputObject;

        [Parameter(Position = 0, Mandatory = true, ParameterSetName = "Pipeline")]
        public string Sample1Name;

        [Parameter(Position = 1, Mandatory = true, ParameterSetName = "Pipeline")]
        public string Sample2Name;

        [Parameter(Position = 2, Mandatory = false, ParameterSetName = "Pipeline")]
        [Parameter(Position = 2, Mandatory = false, ParameterSetName = "Samples")]
        public double HypothesizedDifference = 0;

        [Parameter(Position = 3, Mandatory = false, ParameterSetName = "Pipeline")]
        [Parameter(Position = 3, Mandatory = false, ParameterSetName = "Samples")]
        [ValidateSet("Different", "Greater", "Smaller")]
        public string Alternate = "Different";

        [Parameter(Position = 4, Mandatory = false, ParameterSetName = "Pipeline")]
        [Parameter(Position = 4, Mandatory = false, ParameterSetName = "Samples")]
        public double Size = 0.05;

        [Parameter(Position = 0, Mandatory = true, ParameterSetName = "Samples")]
        public double[] Sample1;

        [Parameter(Position = 1, Mandatory = true, ParameterSetName = "Samples")]
        public double[] Sample2;

        private DataFrame _data;

        protected override void BeginProcessing()
        {
            if (ParameterSetName == "Pipeline") {
                _data = new DataFrame();
            }
        }

        protected override void ProcessRecord()
        {
            if (ParameterSetName == "Pipeline") {
                _data.AddRow(InputObject);
            }
        }

        protected override void EndProcessing()
        {
            var hypo = TestingHelper.GetTwoSampleHypothesis(Alternate);

            TwoSampleSignTest test;

            if (ParameterSetName == "Pipeline") {
                test = new TwoSampleSignTest(_data[Sample1Name].ToDoubleArray(), _data[Sample2Name].ToDoubleArray(), hypo);
            }
            else {
                test = new TwoSampleSignTest(Sample1, Sample2, hypo);
            }

            test.Size = Size;

            WriteObject(test);
        }
    }
}
