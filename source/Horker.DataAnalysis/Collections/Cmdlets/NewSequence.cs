using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;

namespace Horker.DataAnalysis
{
    class SequenceHelper
    {
        public static IEnumerable<double> GetRange(double a, double b, double step, bool inclusive)
        {
            if (Math.Sign(b - a) != Math.Sign(step))
                throw new ArgumentException("Invalid range definition");

            var count = (int)Math.Ceiling(Math.Abs((b - a) / step));
            if (inclusive && (b - a) % step == 0)
                ++count;

            for (var i = 0; i < count; ++i)
            {
                yield return a + step * i;
            }
        }

        public static IEnumerable<double> GetInterval(double a, double b, int count, bool inclusive)
        {
            if (a >= b || count <= 0)
                throw new ArgumentException("Invalid range definition");

            var result = new double[count];

            double step;

            if (inclusive)
            {
                step = (b - a) / (count - 1);
                for (var i = 0; i < count; ++i)
                {
                    yield return a + step * i;
                }
            }
            else
            {
                step = (b - a) / count;
                for (var i = 0; i < count; ++i)
                {
                    yield return a + step * i;
                }
            }
        }
    }

    [Cmdlet("New", "Math.Sequence")]
    [Alias("seq")]
    public class NewMathSequence : PSCmdlet
    {
        [Parameter(Position = 0, Mandatory = true)]
        public double Start = 0;

        [Parameter(Position = 1, Mandatory = false)]
        public double? Stop = null;

        [Parameter(Position = 2, Mandatory = false)]
        public double? Step = null;

        [Parameter(Position = 3, Mandatory = false)]
        public int Count = int.MaxValue;

        [Parameter(Position = 4, Mandatory = false)]
        public SwitchParameter Inclusive = false;

        [Parameter(Position = 5, Mandatory = false)]
        public ScriptBlock[] Func = null;

        protected override void EndProcessing()
        {
            double start, stop, step;
            if (!Step.HasValue)
            {
                if (!Stop.HasValue)
                {
                    start = 0;
                    stop = Start;
                    step = 1;
                }
                else
                {
                    start = 0;
                    stop = Start;
                    step = Stop.Value;
                }
            }
            else
            {
                start = Start;
                stop = Stop.Value;
                step = Step.Value;
            }

            IEnumerable<double> seq;

            if (Count == int.MaxValue)
            {
                seq = SequenceHelper.GetRange(start, stop, step, Inclusive);
            }
            else
            {
                seq = SequenceHelper.GetInterval(start, stop, Count, Inclusive);
            }

            if (Func == null)
            {
                foreach (var x in seq)
                {
                    WriteObject(x);
                }
            }
            else
            {
                var va = new List<PSVariable>() { new PSVariable("x") };

                foreach (var x in seq)
                {
                    var obj = new PSObject();
                    obj.Properties.Add(new PSNoteProperty("x", x));

                    for (int i = 0; i < Func.Length; ++i)
                    {
                        va[0].Value = x;
                        var y = Func[i].InvokeWithContext(null, va, new object[] { x }).Last();

                        obj.Properties.Add(new PSNoteProperty("y" + i, y));
                    }

                    WriteObject(obj);
                }
            }
        }
    }

    [Cmdlet("New", "Math.SequenceWithValues")]
    [Alias("seq.values")]
    public class NewMathSequenceWithValues : PSCmdlet
    {
        [Parameter(Position = 0, Mandatory = true)]
        public int Count;

        [Parameter(Position = 1, Mandatory = false)]
        public object Values = 0;

        protected override void EndProcessing()
        {
            var values = Converter.ToDoubleArray(Values);

            for (var i = 0; i < Count; ++i)
            {
                WriteObject(values[i % values.Length]);
            }
        }
    }

    [Cmdlet("New", "Math.SequenceWithZeros")]
    [Alias("seq.zeros")]
    public class NewMathSequenceWithZeros : PSCmdlet
    {
        [Parameter(Position = 0, Mandatory = true)]
        public int Count;

        protected override void EndProcessing()
        {
            for (var i = 0; i < Count; ++i)
            {
                WriteObject(0);
            }
        }
    }
}
