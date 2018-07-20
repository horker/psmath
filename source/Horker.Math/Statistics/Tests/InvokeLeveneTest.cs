using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management.Automation;
using Accord.Math;
using Accord.Statistics.Analysis;
using Accord.Statistics.Testing;

namespace Horker.Math
{
    [Cmdlet("Invoke", "LeveneTest")]
    public class InvokeLeveneTest : PSCmdlet
    {
        [Parameter(ValueFromPipeline = true, Mandatory = true)]
        public PSObject InputObject;

        [Parameter(Position = 0, Mandatory = true)]
        public string CategoryName;

        [Parameter(Position = 1, Mandatory = true)]
        public string ValueName;

        [Parameter(Position = 2, Mandatory = false)]
        public double Size = .05;

        [Parameter(Mandatory = false)]
        public SwitchParameter Median = false;

        [Parameter(Mandatory = false)]
        public double TrimPercent = double.NaN;

        private DataFrame _data;

        protected override void BeginProcessing()
        {
            _data = new DataFrame();
        }

        protected override void ProcessRecord()
        {
            _data.AddRow(InputObject);
        }

        protected override void EndProcessing()
        {
            var g = _data.GroupBy(CategoryName).ToDoubleJaggedArrayOf(ValueName);

            LeveneTest test;

            if (double.IsNaN(TrimPercent)) {
                test = new LeveneTest(g, Median);
            }
            else {
                test = new LeveneTest(g, TrimPercent);
            }

            test.Size = Size;

            WriteObject(test);
        }
    }
}
