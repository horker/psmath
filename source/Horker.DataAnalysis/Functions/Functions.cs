using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management.Automation;
using Accord.Math;

namespace Horker.DataAnalysis
{
    public abstract class FunctionCmdletBase : PSCmdlet
    {
        [Parameter(Position = 0, Mandatory = false)]
        public IEnumerable<object> Values;

        [Parameter(ValueFromPipeline = true, Mandatory = false)]
        public object InputObject;

        protected virtual void Initialize() { }

        protected virtual void ProcessInputObject(double value) { }

        protected virtual void ProcessArguments(IEnumerable<double> values)
        {
            foreach (var value in values) {
                ProcessInputObject(value);
            }
        }

        protected virtual void Process() { }

        protected override void BeginProcessing()
        {
            Initialize();
        }

        protected override void ProcessRecord()
        {
            if (InputObject != null) {
                var obj = InputObject;
                if (InputObject is PSObject) {
                    obj = (obj as PSObject).BaseObject;
                }
                var v = Convert.ToDouble(obj);
                ProcessInputObject(v);
            }
        }

        protected override void EndProcessing()
        {
            if (Values != null) {
                var values = Values.Select(x => {
                    if (x is PSObject) {
                        x = (x as PSObject).BaseObject;
                    }
                    return Convert.ToDouble(x);
                });

                ProcessArguments(values);
            }

            Process();
        }
    }

    #region System.Math

    [Cmdlet("Get", "Math.Abs")]
    [Alias("math.abs")]
    public class GetMathAbs : FunctionCmdletBase
    {
        protected override void ProcessInputObject(double value)
        {
            WriteObject(Math.Abs(value));
        }
    }

    [Cmdlet("Get", "Math.Acos")]
    [Alias("math.acos")]
    public class GetMathAcos : FunctionCmdletBase
    {
        protected override void ProcessInputObject(double value)
        {
            WriteObject(Math.Acos(value));
        }
    }

    [Cmdlet("Get", "Math.Asin")]
    [Alias("math.asin")]
    public class GetMathAsin : FunctionCmdletBase
    {
        protected override void ProcessInputObject(double value)
        {
            WriteObject(Math.Asin(value));
        }
    }

    [Cmdlet("Get", "Math.Atan")]
    [Alias("math.atan")]
    public class GetMathAtan : FunctionCmdletBase
    {
        protected override void ProcessInputObject(double value)
        {
            WriteObject(Math.Atan(value));
        }
    }

    [Cmdlet("Get", "Math.Atan2")]
    [Alias("math.atan2")]
    public class GetMathAtan2 : PSCmdlet
    {
        [Parameter(Position = 0, Mandatory = true)]
        public double A;

        [Parameter(Position = 1, Mandatory = true)]
        public double B;

        protected override void EndProcessing()
        {
            WriteObject(Math.Atan2(A, B));
        }
    }

    [Cmdlet("Get", "Math.Cos")]
    [Alias("math.cos")]
    public class GetMathCos : FunctionCmdletBase
    {
        protected override void ProcessInputObject(double value)
        {
            WriteObject(Math.Cos(value));
        }
    }

    [Cmdlet("Get", "Math.Cosh")]
    [Alias("math.cosh")]
    public class GetMathCosh : FunctionCmdletBase
    {
        protected override void ProcessInputObject(double value)
        {
            WriteObject(Math.Cosh(value));
        }
    }

    [Cmdlet("Get", "Math.Ceiling")]
    [Alias("math.ceiling")]
    public class GetMathCeiling : FunctionCmdletBase
    {
        protected override void ProcessInputObject(double value)
        {
            WriteObject(Math.Ceiling(value));
        }
    }

    [Cmdlet("Get", "Math.DivRem")]
    [Alias("math.divrem")]
    public class GetMathDivRem : PSCmdlet
    {
        [Parameter(Position = 0, Mandatory = true)]
        public long A;

        [Parameter(Position = 1, Mandatory = true)]
        public long B;

        protected override void EndProcessing()
        {
            long remainder;
            long quotient = Math.DivRem(A, B, out remainder);

            var result = new PSObject();
            result.Properties.Add(new PSNoteProperty("Quotient", quotient));
            result.Properties.Add(new PSNoteProperty("Remainder", remainder));

            WriteObject(result);
        }
    }

    [Cmdlet("Get", "Math.Exp")]
    [Alias("math.exp")]
    public class GetMathExp : FunctionCmdletBase
    {
        protected override void ProcessInputObject(double value)
        {
            WriteObject(Math.Exp(value));
        }
    }

    [Cmdlet("Get", "Math.Floor")]
    [Alias("math.floor")]
    public class GetMathFloor : FunctionCmdletBase
    {
        protected override void ProcessInputObject(double value)
        {
            WriteObject(Math.Floor(value));
        }
    }

    [Cmdlet("Get", "Math.IEEERemainder")]
    [Alias("math.rem")]
    public class GetMathIEEERemainder : PSCmdlet
    {
        [Parameter(Position = 0, Mandatory = true)]
        public double A;

        [Parameter(Position = 1, Mandatory = true)]
        public double B;

        protected override void EndProcessing()
        {
            WriteObject(Math.IEEERemainder(A, B));
        }
    }

    [Cmdlet("Get", "Math.Log")]
    [Alias("math.log")]
    public class GetMathLog : FunctionCmdletBase
    {
        protected override void ProcessInputObject(double value)
        {
            WriteObject(Math.Log(value));
        }
    }

    [Cmdlet("Get", "Math.Log10")]
    [Alias("math.log10")]
    public class GetMathLog10 : FunctionCmdletBase
    {
        protected override void ProcessInputObject(double value)
        {
            WriteObject(Math.Log10(value));
        }
    }

    [Cmdlet("Get", "Math.Pow")]
    [Alias("math.pow")]
    public class GetMathPow : PSCmdlet
    {
        [Parameter(Position = 0, Mandatory = true)]
        public double A;

        [Parameter(Position = 1, Mandatory = true)]
        public double B;

        protected override void EndProcessing()
        {
            WriteObject(Math.Pow(A, B));
        }
    }

    [Cmdlet("Get", "Math.Round")]
    [Alias("math.round")]
    public class GetMathRound : PSCmdlet
    {
        [Parameter(Position = 0, Mandatory = true)]
        public double Value;

        [Parameter(Position = 1, Mandatory = false)]
        public int Digits = 0;

        [Parameter(Position = 2, Mandatory = false)]
        public MidpointRounding Mode = MidpointRounding.ToEven;

        protected override void EndProcessing()
        {
            WriteObject(Math.Round(Value, Digits, Mode));
        }
    }

    [Cmdlet("Get", "Math.Sign")]
    [Alias("math.sign")]
    public class GetMathSign : FunctionCmdletBase
    {
        protected override void ProcessInputObject(double value)
        {
            WriteObject(Math.Sign(value));
        }
    }

    [Cmdlet("Get", "Math.Sin")]
    [Alias("math.sin")]
    public class GetMathSin : FunctionCmdletBase
    {
        protected override void ProcessInputObject(double value)
        {
            WriteObject(Math.Sin(value));
        }
    }

    [Cmdlet("Get", "Math.Sinh")]
    [Alias("math.sinh")]
    public class GetMathSinh : FunctionCmdletBase
    {
        protected override void ProcessInputObject(double value)
        {
            WriteObject(Math.Sinh(value));
        }
    }

    [Cmdlet("Get", "Math.Sqrt")]
    [Alias("math.sqrt")]
    public class GetMathSqrt : FunctionCmdletBase
    {
        protected override void ProcessInputObject(double value)
        {
            WriteObject(Math.Sqrt(value));
        }
    }

    [Cmdlet("Get", "Math.Tan")]
    [Alias("math.tan")]
    public class GetMathTan : FunctionCmdletBase
    {
        protected override void ProcessInputObject(double value)
        {
            WriteObject(Math.Tan(value));
        }
    }

    [Cmdlet("Get", "Math.Tanh")]
    [Alias("math.tanh")]
    public class GetMathTanh : FunctionCmdletBase
    {
        protected override void ProcessInputObject(double value)
        {
            WriteObject(Math.Tanh(value));
        }
    }

    [Cmdlet("Get", "Math.Truncate")]
    [Alias("math.truncate")]
    public class GetMathTruncate : FunctionCmdletBase
    {
        protected override void ProcessInputObject(double value)
        {
            WriteObject(Math.Truncate(value));
        }
    }

    [Cmdlet("Get", "Math.E")]
    [Alias("math.e")]
    public class GetMathE : PSCmdlet
    {
        protected override void EndProcessing()
        {
            WriteObject(Math.E);
        }
    }

    [Cmdlet("Get", "Math.PI")]
    [Alias("math.pi")]
    public class GetMathPi : PSCmdlet
    {
        protected override void EndProcessing()
        {
            WriteObject(Math.PI);
        }
    }

    #endregion

    #region Accord.Math.Special

    [Cmdlet("Get", "Math.Binomial")]
    [Alias("math.binomial", "math.choose")]
    public class GetMathBinomial : PSCmdlet
    {
        [Parameter(Position = 0, Mandatory = true)]
        public double N;

        [Parameter(Position = 1, Mandatory = true)]
        public double K;

        protected override void EndProcessing()
        {
            WriteObject(Special.Binomial(N, K));
        }
    }

    [Cmdlet("Get", "Math.Factorial")]
    [Alias("math.factorial")]
    public class GetMathFactorial : FunctionCmdletBase
    {
        protected override void ProcessInputObject(double value)
        {
            WriteObject(Special.Factorial(value));
        }
    }

    #endregion

    #region Accord.Math.Combinatorics

    [Cmdlet("Get", "Math.TruthTable")]
    [Alias("math.truthtable")]
    public class GetMathTruthTable : PSCmdlet
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
                            df.DefineNewColumn("c" + column, new Vector());
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
                    df.DefineNewColumn("c" + column, new Vector(seq));
                    ++column;
                }
            }

            WriteObject(df);
        }
    }


    #endregion

    #region Aggregate functions

    [Cmdlet("Get", "Math.Max")]
    [Alias("math.max")]
    public class GetMathMax : FunctionCmdletBase
    {
        private double _result = double.MinValue;

        protected override void ProcessInputObject(double value)
        {
            if (value > _result) {
                _result = value;
            }
        }

        protected override void Process()
        {
            WriteObject(_result);
        }
    }

    [Cmdlet("Get", "Math.Min")]
    [Alias("math.min")]
    public class GetMathMin : FunctionCmdletBase
    {
        private double _result = double.MaxValue;

        protected override void ProcessInputObject(double value)
        {
            if (value < _result) {
                _result = value;
            }
        }

        protected override void Process()
        {
            WriteObject(_result);
        }
    }

    [Cmdlet("Get", "Math.Sum")]
    [Alias("math.sum")]
    public class GetMathSum : FunctionCmdletBase
    {
        private double _result = 0.0;

        protected override void ProcessInputObject(double value)
        {
            _result += value;
        }

        protected override void Process()
        {
            WriteObject(_result);
        }
    }

    [Cmdlet("Get", "Math.Mean")]
    [Alias("math.mean")]
    public class GetMathMean : FunctionCmdletBase
    {
        private double _result = 0.0;
        private int _count = 0;

        protected override void ProcessInputObject(double value)
        {
            _result += value;
            ++_count;
        }

        protected override void Process()
        {
            WriteObject(_result / _count);
        }
    }

    [Cmdlet("Get", "Math.Softmax")]
    [Alias("math.softmax")]
    public class GetMathSoftmax : FunctionCmdletBase
    {
        private List<double> _results;

        protected override void Initialize()
        {
            _results = new List<double>();
        }

        protected override void ProcessInputObject(double value)
        {
            _results.Add(value);
        }

        protected override void Process()
        {
            var results = Special.Softmax(_results.ToArray());

            foreach (var value in results) {
                WriteObject(value);
            }
        }
    }

    #endregion
}