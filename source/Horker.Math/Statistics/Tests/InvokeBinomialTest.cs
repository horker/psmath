using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;
using Accord.Statistics.Testing;

namespace Horker.Math
{
    [Cmdlet("Invoke", "BinomialTest")]
    public class InvokeBinomialTest : PSCmdlet
    {
        [Parameter(Position = 0, Mandatory = true)]
        public int SuccesCount;

        [Parameter(Position = 1, Mandatory = true)]
        public int TrialCount;

        [Parameter(Position = 2, Mandatory = false)]
        public double Probability = .5;

        [Parameter(Position = 3, Mandatory = false)]
        [ValidateSet("Different", "Greater", "Smaller")]
        public string Alternate = "Different";

        [Parameter(Position = 4, Mandatory = false)]
        public double Size = .05;

        protected override void EndProcessing()
        {
            var hypo = TestingHelper.GetOneSampleHypothesis(Alternate);

            var test = new BinomialTest(SuccesCount, TrialCount, Probability, hypo) { Size = Size };

            WriteObject(test);
        }
    }
}
