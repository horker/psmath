using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management.Automation;
using Accord.Math;

namespace Horker.DataAnalysis
{
    #region System.Math

    [Cmdlet("Get", "Math.Abs")]
    [Alias("math.abs")]
    public class GetMathAbs : PSCmdlet
    {
        [Parameter(Position = 0, Mandatory = true)]
        public double A;

        protected override void EndProcessing()
        {
            WriteObject(Math.Abs(A));
        }
    }

    [Cmdlet("Get", "Math.Acos")]
    [Alias("math.acos")]
    public class GetMathAcos : PSCmdlet
    {
        [Parameter(Position = 0, Mandatory = true)]
        public double A;

        protected override void EndProcessing()
        {
            WriteObject(Math.Acos(A));
        }
    }

    [Cmdlet("Get", "Math.Asin")]
    [Alias("math.asin")]
    public class GetMathAsin : PSCmdlet
    {
        [Parameter(Position = 0, Mandatory = true)]
        public double A;

        protected override void EndProcessing()
        {
            WriteObject(Math.Asin(A));
        }
    }

    [Cmdlet("Get", "Math.Atan")]
    [Alias("math.atan")]
    public class GetMathAtan : PSCmdlet
    {
        [Parameter(Position = 0, Mandatory = true)]
        public double A;

        protected override void EndProcessing()
        {
            WriteObject(Math.Atan(A));
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
    public class GetMathCos : PSCmdlet
    {
        [Parameter(Position = 0, Mandatory = true)]
        public double A;

        protected override void EndProcessing()
        {
            WriteObject(Math.Cos(A));
        }
    }

    [Cmdlet("Get", "Math.Cosh")]
    [Alias("math.cosh")]
    public class GetMathCosh : PSCmdlet
    {
        [Parameter(Position = 0, Mandatory = true)]
        public double A;

        protected override void EndProcessing()
        {
            WriteObject(Math.Cosh(A));
        }
    }

    [Cmdlet("Get", "Math.Ceiling")]
    [Alias("math.ceiling")]
    public class GetMathCeiling : PSCmdlet
    {
        [Parameter(Position = 0, Mandatory = true)]
        public double A;

        protected override void EndProcessing()
        {
            WriteObject(Math.Ceiling(A));
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
    public class GetMathExp : PSCmdlet
    {
        [Parameter(Position = 0, Mandatory = true)]
        public double A;

        protected override void EndProcessing()
        {
            WriteObject(Math.Exp(A));
        }
    }

    [Cmdlet("Get", "Math.Floor")]
    [Alias("math.floor")]
    public class GetMathFloor : PSCmdlet
    {
        [Parameter(Position = 0, Mandatory = true)]
        public double A;

        protected override void EndProcessing()
        {
            WriteObject(Math.Floor(A));
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
    public class GetMathLog : PSCmdlet
    {
        [Parameter(Position = 0, Mandatory = true)]
        public double A;

        protected override void EndProcessing()
        {
            WriteObject(Math.Log(A));
        }
    }

    [Cmdlet("Get", "Math.Log10")]
    [Alias("math.log10")]
    public class GetMathLog10 : PSCmdlet
    {
        [Parameter(Position = 0, Mandatory = true)]
        public double A;

        protected override void EndProcessing()
        {
            WriteObject(Math.Log10(A));
        }
    }

    [Cmdlet("Get", "Math.Max")]
    [Alias("math.max")]
    public class GetMathMax : PSCmdlet
    {
        [Parameter(ValueFromPipeline = true, Mandatory = false)]
        public double? InputObject;

        [Parameter(Position = 0, Mandatory = false)]
        public double[] Values;

        private List<double> _values;

        protected override void BeginProcessing()
        {
            _values = new List<double>();
        }

        protected override void ProcessRecord()
        {
            if (InputObject != null) {
                _values.Add(InputObject.Value);
            }
        }

        protected override void EndProcessing()
        {
            if (Values != null) {
                _values.AddRange(Values);
            }

            WriteObject(_values.Max());
        }
    }

    [Cmdlet("Get", "Math.Min")]
    [Alias("math.min")]
    public class GetMathMin : PSCmdlet
    {
        [Parameter(ValueFromPipeline = true, Mandatory = false)]
        public double? InputObject;

        [Parameter(Position = 0, Mandatory = false)]
        public double[] Values;

        private List<double> _values;

        protected override void BeginProcessing()
        {
            _values = new List<double>();
        }

        protected override void ProcessRecord()
        {
            if (InputObject != null) {
                _values.Add(InputObject.Value);
            }
        }

        protected override void EndProcessing()
        {
            if (Values != null) {
                _values.AddRange(Values);
            }

            WriteObject(_values.Min());
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
    public class GetMathSign : PSCmdlet
    {
        [Parameter(Position = 0, Mandatory = true)]
        public double A;

        protected override void EndProcessing()
        {
            WriteObject(Math.Sign(A));
        }
    }

    [Cmdlet("Get", "Math.Sin")]
    [Alias("math.sin")]
    public class GetMathSin : PSCmdlet
    {
        [Parameter(Position = 0, Mandatory = true)]
        public double A;

        protected override void EndProcessing()
        {
            WriteObject(Math.Sin(A));
        }
    }

    [Cmdlet("Get", "Math.Sinh")]
    [Alias("math.sinh")]
    public class GetMathSinh : PSCmdlet
    {
        [Parameter(Position = 0, Mandatory = true)]
        public double A;

        protected override void EndProcessing()
        {
            WriteObject(Math.Sinh(A));
        }
    }

    [Cmdlet("Get", "Math.Sqrt")]
    [Alias("math.sqrt")]
    public class GetMathSqrt : PSCmdlet
    {
        [Parameter(Position = 0, Mandatory = true)]
        public double A;

        protected override void EndProcessing()
        {
            WriteObject(Math.Sqrt(A));
        }
    }

    [Cmdlet("Get", "Math.Tan")]
    [Alias("math.tan")]
    public class GetMathTan : PSCmdlet
    {
        [Parameter(Position = 0, Mandatory = true)]
        public double A;

        protected override void EndProcessing()
        {
            WriteObject(Math.Tan(A));
        }
    }

    [Cmdlet("Get", "Math.Tanh")]
    [Alias("math.tanh")]
    public class GetMathTanh : PSCmdlet
    {
        [Parameter(Position = 0, Mandatory = true)]
        public double A;

        protected override void EndProcessing()
        {
            WriteObject(Math.Tanh(A));
        }
    }

    [Cmdlet("Get", "Math.Truncate")]
    [Alias("math.truncate")]
    public class GetMathTruncate : PSCmdlet
    {
        [Parameter(Position = 0, Mandatory = true)]
        public double A;

        protected override void EndProcessing()
        {
            WriteObject(Math.Truncate(A));
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
    public class GetMathFactorial : PSCmdlet
    {
        [Parameter(Position = 0, Mandatory = true)]
        public double N;

        protected override void EndProcessing()
        {
            WriteObject(Special.Factorial(N));
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
}
