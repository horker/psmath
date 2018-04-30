using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management.Automation;
using Accord.Statistics.Testing;

namespace Horker.DataAnalysis
{
    [Cmdlet("Invoke", "ShapiroWilkTest")]
    public class InvokeShapiroWilkTest : PSCmdlet
    {
        [Parameter(ValueFromPipeline = true, Mandatory = true)]
        public double InputObject;

        [Parameter(Position = 0, Mandatory = false)]
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
            var test = new ShapiroWilkTest(_data.ToArray()) { Size = Size };

            WriteObject(test);
        }
    }
}