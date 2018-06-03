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
    [Cmdlet("Invoke", "ChiSquareTest")]
    [CmdletBinding(DefaultParameterSetName = "Independence")]
    public class InvokeChiSquareTest : PSCmdlet
    {
        [Parameter(ValueFromPipeline = true, Mandatory = true, ParameterSetName = "Independence")]
        public PSObject InputObject;

        [Parameter(Position = 0, Mandatory = false, ParameterSetName = "Independence")]
        [Parameter(Position = 2, Mandatory = false, ParameterSetName = "GoodnessOfFit")]
        public double Size = 0.05;

        [Parameter(Position = 0, Mandatory = true, ParameterSetName = "GoodnessOfFit")]
        public double[] Observed;

        [Parameter(Position = 1, Mandatory = false, ParameterSetName = "GoodnessOfFit")]
        public double[] Expected;

        private DataFrame _data;

        protected override void BeginProcessing()
        {
            if (ParameterSetName == "Independence") {
                _data = new DataFrame();
            }
        }

        protected override void ProcessRecord()
        {
            if (ParameterSetName == "Independence") {
                _data.AddRow(InputObject);
            }
        }

        protected override void EndProcessing()
        {
            ChiSquareTest test = null;

            if (ParameterSetName == "Independence") {
                var mat = new GeneralConfusionMatrix(_data.ToIntMatrix());
                test = new ChiSquareTest(mat, false);
            }
            else {
                if (Expected == null) {
                    Expected = new double[Observed.Length];
                    var ex = Observed.Sum() / Observed.Length;
                    for (var i = 0; i < Expected.Length; ++i) {
                        Expected[i] = ex;
                    }
                }

                test = new ChiSquareTest(Expected, Observed, Observed.Length - 1);
            }

            test.Size = Size;
            WriteObject(test);
        }
    }
}
