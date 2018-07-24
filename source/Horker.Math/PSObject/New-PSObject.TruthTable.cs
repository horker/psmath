using System.Management.Automation;
using Accord.Math;

namespace Horker.Math.PSObjects
{
    [Cmdlet("Get", "PSObject.TruthTable")]
    [Alias("pso.truthtable")]
    public class GetPSObjectTruthTable : PSCmdlet
    {
        [Parameter(Position = 0, Mandatory = true)]
        public int[] Symbols;

        [Parameter(Mandatory = false)]
        public SwitchParameter Transpose = false;

        protected override void EndProcessing()
        {
            var df = new DataFrame();

            if (Transpose) {
                bool first = true;
                foreach (int[] seq in Combinatorics.Sequences(Symbols, true)) {
                    if (first) {
                        for (var column = 0; column < seq.Length; ++column) {
                            df.DefineNewColumn("c" + column, new DataFrameColumn());
                        }
                        first = false;
                    }

                    for (var column = 0; column < seq.Length; ++column) {
                        df.GetColumn(column).Add(seq[column]);
                    }
                }
            }
            else {
                int column = 0;
                foreach (int[] seq in Combinatorics.Sequences(Symbols, true)) {
                    df.DefineNewColumn("c" + column, new DataFrameColumn(seq));
                    ++column;
                }
            }

            WriteObject(df);
        }
    }
}