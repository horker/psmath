using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management.Automation;
using Accord.Statistics.Testing;
using Accord.Statistics.Distributions;

namespace Horker.DataAnalysis
{
    [Cmdlet("Invoke", "KolmogorovSmirnovTest")]
    public class InvokeKolmogorovSmirnovTest : PSCmdlet
    {
        [Parameter(ValueFromPipeline = true, Mandatory = true)]
        public double InputObject;

        [Parameter(Position = 0, Mandatory = true)]
        public IDistribution<double> HypothesizedDistribution;

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
            var hypo = TestingHelper.GetKolmogorovSmirnovTestHypothesis(Alternate);

            var test = new KolmogorovSmirnovTest(_data.ToArray(), HypothesizedDistribution, hypo) { Size = Size };

            WriteObject(test);
        }
    }
}
