using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;
using Accord.Statistics.Testing;

namespace Horker.DataAnalysis.Tests
{
    [Cmdlet("Invoke", "FTest")]
    public class InvokeFTest : PSCmdlet
    {
        [Parameter(Position = 0, Mandatory = true)]
        public double Variance1;

        [Parameter(Position = 1, Mandatory = true)]
        public double Variance2;

        [Parameter(Position = 3, Mandatory = true)]
        public int DegreesOfFreedom1;

        [Parameter(Position = 4, Mandatory = true)]
        public int DegreesOfFreedom2;

        [Parameter(Position = 5, Mandatory = false)]
        [ValidateSet("Different", "Greater", "Smaller")]
        public string Alternate = "Different";

        [Parameter(Position = 6, Mandatory = false)]
        public double Size = .05;

        protected override void EndProcessing()
        {
            var hypo = TestingHelper.GetTwoSampleHypothesis(Alternate);

            var test = new FTest(Variance1, Variance2, DegreesOfFreedom1, DegreesOfFreedom2, hypo) { Size = Size };

            WriteObject(test);
        }
    }
}
