using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management.Automation;
using Accord.Statistics.Testing;

namespace Horker.Math
{
    [Cmdlet("Invoke", "TTest")]
    [CmdletBinding(DefaultParameterSetName = "Samples")]
    public class InvokeTTest : PSCmdlet
    {
        [Parameter(ValueFromPipeline = true, Mandatory = true, ParameterSetName = "Samples")]
        public double InputObject;

        [Parameter(Position = 0, Mandatory = false, ParameterSetName = "Samples")]
        [Parameter(Position = 3, Mandatory = false, ParameterSetName = "Mean")]
        [Parameter(Position = 3, Mandatory = false, ParameterSetName = "SE")]
        public double HypothesizedMean = 0.0;

        [Parameter(Position = 1, Mandatory = false, ParameterSetName = "Samples")]
        [Parameter(Position = 4, Mandatory = false, ParameterSetName = "Mean")]
        [Parameter(Position = 4, Mandatory = false, ParameterSetName = "SE")]
        [ValidateSet("Different", "Greater", "Smaller")]
        public string Alternate = "Different";

        [Parameter(Position = 2, Mandatory = false, ParameterSetName = "Samples")]
        [Parameter(Position = 5, Mandatory = false, ParameterSetName = "Mean")]
        [Parameter(Position = 5, Mandatory = false, ParameterSetName = "SE")]
        public double Size = .05;

        [Parameter(Position = 0, Mandatory = false, ParameterSetName = "Mean")]
        public double Mean = 0.0;

        [Parameter(Position = 1, Mandatory = false, ParameterSetName = "Mean")]
        public double StdDev = 0.0;

        [Parameter(Position = 2, Mandatory = true, ParameterSetName = "Mean")]
        public int Samples;

        [Parameter(Position = 0, Mandatory = true, ParameterSetName = "SE")]
        public double Value = 0.0;

        [Parameter(Position = 1, Mandatory = true, ParameterSetName = "SE")]
        public double StandardError;

        [Parameter(Position = 2, Mandatory = true, ParameterSetName = "SE")]
        public int DegreesOfFreedom = 0;

        private List<double> _data;

        protected override void BeginProcessing()
        {
            if (ParameterSetName == "Samples") {
                _data = new List<double>();
            }
        }

        protected override void ProcessRecord()
        {
            if (ParameterSetName == "Samples") {
                _data.Add(Converter.ToDouble(InputObject));
            }
        }

        protected override void EndProcessing()
        {
            var hypo = TestingHelper.GetOneSampleHypothesis(Alternate);

            TTest test = null;

            if (ParameterSetName == "Samples") {
                test = new TTest(_data.ToArray(), HypothesizedMean, hypo);
            }
            else if (ParameterSetName == "Mean") {
                test = new TTest(Mean, StdDev, Samples, HypothesizedMean, hypo);
            }
            else if (ParameterSetName == "SE") {
                test = new TTest(Value, StandardError, (double)DegreesOfFreedom, HypothesizedMean, hypo);
            }

            test.Size = Size;

            WriteObject(test);
        }
    }
}
