using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;

namespace Horker.DataAnalysis
{
    [Cmdlet("New", "Sequence")]
    [CmdletBinding(DefaultParameterSetName = "Range")]
    public class NewSequence : PSCmdlet
    {
        [Parameter(Position = 0, Mandatory = true, ParameterSetName = "Range")]
        public double Start = 0;

        [Parameter(Position = 1, Mandatory = false, ParameterSetName = "Range")]
        public double Stop = double.NaN;

        [Parameter(Position = 2, Mandatory = false, ParameterSetName = "Range")]
        public double Step = 1;

        [Parameter(Mandatory = false, ParameterSetName = "Range")]
        [Parameter(Position = 1, Mandatory = true, ParameterSetName = "Value")]
        public int Size = int.MaxValue;

        [Parameter(Mandatory = false, ParameterSetName = "Range")]
        public SwitchParameter Inclusive = false;

        [Parameter(Mandatory = false, ParameterSetName = "Range")]
        [Parameter(Mandatory = false, ParameterSetName = "Value")]
        public SwitchParameter AsVector = false;

        [Parameter(Mandatory = false, ParameterSetName = "Range")]
        [Parameter(Mandatory = false, ParameterSetName = "Value")]
        public SwitchParameter AsDataFrame = false;

        [Parameter(Mandatory = true, ParameterSetName = "Value")]
        public double Value;

        [Parameter(Mandatory = false)]
        public ScriptBlock[] F = null;

        protected override void EndProcessing()
        {
            Vector seq;

            if (ParameterSetName == "Range") {
                if (Double.IsNaN(Stop)) {
                    Stop = Start;
                    Start = 0;
                }

                if (Size == int.MaxValue) {
                    seq = Vector.GetDoubleRange(Start, Stop, Step, Inclusive);
                }
                else {
                    seq = Vector.GetDoubleInterval(Start, Stop, Size, Inclusive);
                }
            }
            else {
                // ParameterSetName == "Value"
                seq = Vector.WithValue(Value, Size);
            }

            DataFrame df;
            if (F == null) {
                if (AsVector) {
                    WriteObject(seq);
                }
                else if (AsDataFrame) {
                    df = new DataFrame();
                    df.DefineNewColumn("x", seq);
                    WriteObject(df);
                }
                else {
                    foreach (var value in seq) {
                        WriteObject(value);
                    }
                }

                return;
            }

            df = new DataFrame();
            df.DefineNewColumn("x", seq);

            var va = new List<PSVariable>() { new PSVariable("x") };
            for (int i = 0; i < F.Length; ++i) {
                var yseq = new Vector(seq.Count);
                foreach (var x in seq) {
                    va[0].Value = x;
                    var y = F[i].InvokeWithContext(null, va, null).Last();
                    yseq.Add(y);
                }

                if (F.Length == 1) {
                    df.DefineNewColumn("y", yseq);
                }
                else {
                    df.DefineNewColumn("y" + (i + 1), yseq);
                }
            }

            if (AsVector) {
                WriteObject(df.GetColumn(1));
            }
            else if (AsDataFrame) {
                WriteObject(df);
            }
            else {
                for (var i = 0; i < seq.Count; ++i) {
                    var obj = new PSObject();
                    obj.Properties.Add(new PSNoteProperty("x", seq[i]));
                    for (var j = 0; j < df.ColumnCount; ++j) {
                        obj.Properties.Add(new PSNoteProperty(df.ColumnNames[j], df.GetColumn(j)[i]));
                    }
                    WriteObject(obj);
                }
            }
        }

    }
}
