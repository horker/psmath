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
    [Cmdlet("Invoke", "TwoWayAnova")]
    public class InvokeTwoWayAnova : PSCmdlet
    {
        [Parameter(ValueFromPipeline = true, Mandatory = true)]
        public PSObject InputObject;

        [Parameter(Position = 0, Mandatory = true)]
        public string Level1Name;

        [Parameter(Position = 1, Mandatory = true)]
        public string Level2Name;

        [Parameter(Position = 2, Mandatory = true)]
        public string ValueName;

        [Parameter(Position = 3, Mandatory = false)]
        public TwoWayAnovaModel Model = TwoWayAnovaModel.Mixed;

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
            var level1 = _data[Level1Name].ToDummyValues(Level1Name, CodificationType.Multilevel);
            var level2 = _data[Level2Name].ToDummyValues(Level2Name, CodificationType.Multilevel);
            var test = new TwoWayAnova(
                _data[ValueName].ToDoubleArray(),
                level1[Level1Name].ToIntArray(),
                level2[Level2Name].ToIntArray(),
                Model);

            WriteObject(test.Table);
        }
    }
}
