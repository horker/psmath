using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management.Automation;
using Accord.Statistics.Testing;

namespace Horker.DataAnalysis
{
    [Cmdlet("Invoke", "TwoSampleTTest")]
    [CmdletBinding(DefaultParameterSetName = "Pipeline")]
    public class InvokeTwoSampleTTest : PSCmdlet
    {
        [Parameter(ValueFromPipeline = true, Mandatory = true, ParameterSetName = "Pipeline")]
        public PSObject InputObject;

        [Parameter(Position = 0, Mandatory = true, ParameterSetName = "Pipeline")]
        public string CategoryName;

        [Parameter(Position = 1, Mandatory = true, ParameterSetName = "Pipeline")]
        public string ValueName;

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

        [Parameter(Mandatory = false, ParameterSetName = "Pipeline")]
        [Parameter(Mandatory = false, ParameterSetName = "Samples")]
        public SwitchParameter AssumeEqualVariances = false;

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

            TwoSampleTTest test;

            if (ParameterSetName == "Pipeline") {
                var samples = _data.GroupBy(CategoryName).ToDoubleJaggedArrayOf(ValueName);
                if (samples.Length != 2) {
                    WriteError(new ErrorRecord(new RuntimeException("The number of categories is not two"), "", ErrorCategory.InvalidArgument, null));
                    return;
                }
                test = new TwoSampleTTest(samples[0], samples[1], AssumeEqualVariances, HypothesizedDifference, hypo);
            }
            else {
                test = new TwoSampleTTest(Sample1, Sample2, AssumeEqualVariances, HypothesizedDifference, hypo);
            }

            test.Size = Size;

            WriteObject(test);
        }
    }
}
