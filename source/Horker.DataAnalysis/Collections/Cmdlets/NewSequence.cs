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
        [Parameter(Mandatory = false, ParameterSetName = "Value")]
        public SwitchParameter AsVector = false;

        [Parameter(Mandatory = false, ParameterSetName = "Range")]
        public SwitchParameter Inclusive = false;

        [Parameter(Mandatory = true, ParameterSetName = "Value")]
        public double Value;

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

            if (AsVector) {
                WriteObject(seq);
            }
            else {
                foreach (var value in seq) {
                    WriteObject(value);
                }
            }
        }
    }
}