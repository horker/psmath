using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;

namespace Horker.DataAnalysis
{
    [Cmdlet("New", "Sequence")]
    public class NewSequence : PSCmdlet
    {
        [Parameter(Position = 0, Mandatory = true)]
        public double Start = 0;

        [Parameter(Position = 1, Mandatory = false)]
        public double Stop = double.NaN;

        [Parameter(Position = 2, Mandatory = false)]
        public double Step = 1;

        [Parameter(Position = 3, Mandatory = false)]
        public int Size = int.MaxValue;

        [Parameter(Position = 4, Mandatory = false)]
        public SwitchParameter Inclusive = false;

        [Parameter(Position = 5, Mandatory = false)]
        public SwitchParameter AsVector = false;

        [Parameter(Position = 6, Mandatory = false)]
        public SwitchParameter AsDataFrame = false;

        [Parameter(Position = 7, Mandatory = false)]
        public double? Value = null;

        [Parameter(Position = 8, Mandatory = false)]
        public ScriptBlock[] F = null;

        protected override void EndProcessing()
        {
            Vector seq;

            if (!Value.HasValue) {
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
                if (Size == int.MaxValue) {
                    Size = (int)Start;
                }
                seq = Vector.WithValue(Value.Value, Size);
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
