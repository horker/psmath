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

            if (b >= a)
            {
                if (inclusive)
                {
                    for (var i = 0; a + step * i <= b; ++i)
                    {
                        yield return a + step * i;
                    }
                }
                else
                {
                    for (var i = 0; a + step * i < b; ++i)
                    {
                        yield return a + step * i;
                    }
                }
            }
            else
            {
                if (inclusive)
                {
                    for (var i = 0; a + step * i >= b; ++i)
                    {
                        yield return a + step * i;
                    }
                }
                else
                {
                    for (var i = 0; a + step * i > b; ++i)
                    {
                        yield return a + step * i;
                    }
                }
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

    public abstract class SequenceCmdletBase : PSCmdlet
    {
        [Parameter(Position = 10, Mandatory = false)]
        public ScriptBlock[] Func = null;

        [Parameter(Position = 11, Mandatory = false)]
        public SwitchParameter NoSeq;

        [Parameter(Position = 12, Mandatory = false)]
        public SwitchParameter AsMatrix;

        protected abstract IEnumerable<double> GetSequence();

        protected override void EndProcessing()
        {
            IEnumerable<double> seq = GetSequence();

            if (!AsMatrix)
            {
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

                        if (!NoSeq)
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
            else
            {
                if (Func == null)
                {
                    WriteObject(Matrix.Create(seq.ToArray(), int.MaxValue));
                }
                else
                {
                    var s = NoSeq ? 0 : 1;

                    var seqarray = seq.ToArray();
                    var result = new double[seqarray.Length, Func.Length + s];

                    var va = new List<PSVariable>() { new PSVariable("x") };

                    for (var i = 0; i < seqarray.Length; ++i)
                    {
                        var x = seqarray[i];

                        if (!NoSeq)
                            result[i, 0] = x;

                        for (int j = 0; j < Func.Length; ++j)
                        {
                            va[0].Value = x;
                            var y = Func[j].InvokeWithContext(null, va, new object[] { x }).Last();
                            result[i, j + s] = (double)y.BaseObject;
                        }
                    }

                    WriteObject(new Matrix(result, true));
                }
            }
        }
    }

    [Cmdlet("New", "Math.Sequence")]
    [Alias("seq")]
    public class NewMathSequence : SequenceCmdletBase
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

        protected override IEnumerable<double> GetSequence()
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

            return SequenceHelper.GetRange(start, stop, step, Inclusive);
        }
    }

    [Cmdlet("New", "Math.SequenceInLinearSpace")]
    [Alias("seq.linspace")]
    public class NewMathSequenceInLinearSpace : SequenceCmdletBase
    {
        [Parameter(Position = 0, Mandatory = true)]
        public double Start = 0;

        [Parameter(Position = 1, Mandatory = false)]
        public double? Stop = null;

        [Parameter(Position = 2, Mandatory = false)]
        public int? Count = null;

        [Parameter(Position = 3, Mandatory = false)]
        public SwitchParameter Inclusive = false;

        protected override IEnumerable<double> GetSequence()
        {
            double start, stop;
            int count;

            if (!Stop.HasValue)
            {
                if (!Count.HasValue)
                {
                    start = 0;
                    stop = Start;
                    count = 100;
                }
                else
                {
                    start = 0;
                    stop = Start;
                    count = (int)Stop.Value;
                }
            }
            else
            {
                start = Start;
                stop = Stop.Value;
                count = Count.Value;
            }

            return SequenceHelper.GetInterval(start, stop, count, Inclusive);
        }
    }

    [Cmdlet("New", "Math.SequenceWithValues")]
    [Alias("seq.value")]
    public class NewMathSequenceWithValues : SequenceCmdletBase
    {
        [Parameter(Position = 0, Mandatory = true)]
        public object Values = 0;

        [Parameter(Position = 1, Mandatory = false)]
        public int Count = int.MaxValue;

        protected override IEnumerable<double> GetSequence()
        {
            var values = Converter.ToDoubleArray(Values);

            if (Count == int.MaxValue)
            {
                Count = values.Length;
            }

            for (var i = 0; i < Count; ++i)
            {
                yield return values[i % values.Length];
            }
        }
    }

    [Cmdlet("New", "Math.ZeroSequence")]
    [Alias("seq.zero")]
    public class NewMathZeroSequence : SequenceCmdletBase
    {
        [Parameter(Position = 0, Mandatory = true)]
        public int Count;

        protected override IEnumerable<double> GetSequence()
        {
            for (var i = 0; i < Count; ++i)
            {
                yield return 0;
            }
        }
    }
}
