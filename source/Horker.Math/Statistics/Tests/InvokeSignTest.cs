using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management.Automation;
using Accord.Statistics.Testing;

namespace Horker.Math
{
    [Cmdlet("Invoke", "SignTest")]
    public class InvokeSignTest : PSCmdlet
    {
        [Parameter(ValueFromPipeline = true, Mandatory = true)]
        public double InputObject;

        [Parameter(Position = 0, Mandatory = false)]
        public double HypothesizedMedian = 0.0;

        [Parameter(Position = 1, Mandatory = false)]
        [ValidateSet("Different", "Greater", "Smaller")]
        public string Alternate = "Different";

        [Parameter(Position = 2, Mandatory = false)]
        public double Size = .05;

        private List<double> _data;

        protected override void BeginProcessing()
        {
            _data = new List<double>();
        }

        protected override void ProcessRecord()
        {
            _data.Add(Converter.ToDouble(InputObject));
        }

        protected override void EndProcessing()
        {
            var hypo = TestingHelper.GetOneSampleHypothesis(Alternate);

            var test = new SignTest(_data.ToArray(), HypothesizedMedian, hypo) { Size = Size };

            WriteObject(test);
        }
    }
}
