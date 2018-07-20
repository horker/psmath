using System;
using System.Management.Automation;
using Accord.Statistics.Testing;

namespace Horker.Math
{
    [Cmdlet("Invoke", "TwoSampleWilcoxonSignedRankTest")]
    [CmdletBinding(DefaultParameterSetName = "Pipeline")]
    public class InvokeTwoSampleWilcoxonSignedRankTest : PSCmdlet
    {
        [Parameter(ValueFromPipeline = true, Mandatory = true, ParameterSetName = "Pipeline")]
        public PSObject InputObject;

        [Parameter(Position = 0, Mandatory = true, ParameterSetName = "Pipeline")]
        public string Sample1Name;

        [Parameter(Position = 1, Mandatory = true, ParameterSetName = "Pipeline")]
        public string Sample2Name;

        [Parameter(Position = 2, Mandatory = false, ParameterSetName = "Pipeline")]
        [Parameter(Position = 2, Mandatory = false, ParameterSetName = "Samples")]
        [ValidateSet("Different", "Greater", "Smaller")]
        public string Alternate = "Different";

        [Parameter(Position = 3, Mandatory = false, ParameterSetName = "Pipeline")]
        [Parameter(Position = 3, Mandatory = false, ParameterSetName = "Samples")]
        public double Size = 0.05;

        [Parameter(Mandatory = false, ParameterSetName = "Pipeline")]
        [Parameter(Mandatory = false, ParameterSetName = "Samples")]
        public SwitchParameter Exact = false;

        [Parameter(Mandatory = false, ParameterSetName = "Pipeline")]
        [Parameter(Mandatory = false, ParameterSetName = "Samples")]
        public SwitchParameter NoAdjustForTies = false;

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

            TwoSampleWilcoxonSignedRankTest test;

            if (ParameterSetName == "Pipeline") {
                test = new TwoSampleWilcoxonSignedRankTest(
                    _data[Sample1Name].ToDoubleArray(),
                    _data[Sample2Name].ToDoubleArray(),
                    hypo,
                    (Exact ? true : (Nullable<bool>)null),
                    !NoAdjustForTies);
            }
            else {
                test = new TwoSampleWilcoxonSignedRankTest(
                    Sample1,
                    Sample2,
                    hypo,
                    (Exact ? true : (Nullable<bool>)null),
                    !NoAdjustForTies);
            }

            test.Size = Size;

            WriteObject(test);
        }
    }
}
