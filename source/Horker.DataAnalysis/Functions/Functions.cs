using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management.Automation;
using Accord.Math;
using Accord.Statistics;
using Accord.Math.Random;
using Accord.Statistics.Moving;

namespace Horker.DataAnalysis
{
    public class FunctionCmdletBase : PSCmdlet
    {
        [Parameter(Position = 0, Mandatory = false)]
        public object[] Values;

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

    public class AggregateFunctionCmdletBase : FunctionCmdletBase
    {
        protected List<double> _values;

        protected override void Initialize()
        {
            _values = new List<double>();
        }

        protected override void ProcessInputObject(double value)
        {
            _values.Add(value);
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
    public class GetMathAtan2 : FunctionCmdletBase
    {
        [Parameter(Position = 1, Mandatory = true)]
        public double Arg;

        protected override void ProcessInputObject(double value)
        {
            WriteObject(Math.Atan2(value, Arg));
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
    public class GetMathIEEERemainder : FunctionCmdletBase
    {
        [Parameter(Position = 1, Mandatory = true)]
        public double Arg;

        protected override void ProcessInputObject(double value)
        {
            WriteObject(Math.IEEERemainder(value, Arg));
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
    public class GetMathPow : FunctionCmdletBase
    {
        [Parameter(Position = 1, Mandatory = true)]
        public double Arg;

        protected override void ProcessInputObject(double value)
        {
            WriteObject(Math.Pow(value, Arg));
        }
    }

    [Cmdlet("Get", "Math.Round")]
    [Alias("math.round")]
    public class GetMathRound : FunctionCmdletBase
    {
        [Parameter(Position = 1, Mandatory = false)]
        public int Digits = 0;

        [Parameter(Position = 2, Mandatory = false)]
        public MidpointRounding Mode = MidpointRounding.ToEven;

        protected override void ProcessInputObject(double value)
        {
            WriteObject(Math.Round(value, Digits, Mode));
        }
    }

    [Cmdlet("Get", "Math.Sigmoid")]
    [Alias("math.sigmoid")]
    public class GetMathSigmoid : FunctionCmdletBase
    {
        protected override void ProcessInputObject(double value)
        {
            WriteObject(1.0 / (1.0 + Math.Exp(-value)));
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

    [Cmdlet("Get", "Math.Softmax")]
    [Alias("math.softmax")]
    public class GetMathSoftmax : AggregateFunctionCmdletBase
    {
        protected override void Process()
        {
            var results = Special.Softmax(_values.ToArray());

            foreach (var value in results) {
                WriteObject(value);
            }
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

    #region Statistical values

    [Cmdlet("Get", "Math.ContraHarmonicMean")]
    [Alias("math.contraharmonicmean")]
    public class GetMathContraHarmonicMean : AggregateFunctionCmdletBase
    {
        protected override void Process()
        {
            var result = Measures.ContraHarmonicMean(_values.ToArray());

            WriteObject(result);
        }
    }

    [Cmdlet("Get", "Math.CumulativeSum")]
    [Alias("math.cumsum")]
    public class GetMathCumulativeSum : AggregateFunctionCmdletBase
    {
        protected override void Process()
        {
            var result = _values.ToArray().CumulativeSum();

            WriteObject(result);
        }
    }

    [Cmdlet("Get", "Math.Entropy")]
    [Alias("math.entropy")]
    public class GetMathEntropy : AggregateFunctionCmdletBase
    {
        protected override void Process()
        {
            var result = Measures.Entropy(_values.ToArray());

            WriteObject(result);
        }
    }

    [Cmdlet("Get", "Math.Difference")]
    [Alias("math.diff")]
    public class GetMathDifference : FunctionCmdletBase
    {
        private bool _first;
        private double _prev;

        protected override void ProcessInputObject(double value)
        {
            if (_first) {
                _first = false;
            }
            else {
                WriteObject(value - _prev);
            }

            _prev = value;
        }
    }

    [Cmdlet("Get", "Math.Histogram")]
    [Alias("math.hist")]
    public class GetMathHistogram : AggregateFunctionCmdletBase
    {
        [Parameter(Position = 1, Mandatory = false)]
        public double BinWidth = double.NaN;

        [Parameter(Position = 2, Mandatory = false)]
        public double Offset = 0.0;

        protected override void Process()
        {
            var min = _values.Min();
            var max = _values.Max();

            if (double.IsNaN(BinWidth)) {
                var binCount = Math.Min(100, Math.Ceiling(Math.Sqrt(_values.Count)));
                BinWidth = (max - min) / binCount;
            }

            int minBar = (int)Math.Floor((min - Offset) / BinWidth);
            int maxBar = (int)Math.Floor((max - Offset) / BinWidth);

            var hist = new int[maxBar - minBar + 1];

            foreach (var value in _values) {
                var bin = (int)Math.Floor((value - Offset) / BinWidth) - minBar;
                ++hist[bin];
            }

            for (var i = minBar; i <= maxBar; ++i) {
                var result = new PSObject();
                result.Properties.Add(new PSNoteProperty("Lower", i * BinWidth + Offset));
                result.Properties.Add(new PSNoteProperty("Upper", (i + 1) * BinWidth + Offset));
                result.Properties.Add(new PSNoteProperty("Count", hist[i - minBar]));

                WriteObject(result);
            }
        }
    }

    [Cmdlet("Get", "Math.GeometricMean")]
    [Alias("math.geometric")]
    public class GetMathGeometricMean : AggregateFunctionCmdletBase
    {
        protected override void Process()
        {
            var result = Measures.GeometricMean(_values.ToArray());

            WriteObject(result);
        }
    }

    [Cmdlet("Get", "Math.Kurtosis")]
    [Alias("math.kurtosis")]
    public class GetMathKurtosis : AggregateFunctionCmdletBase
    {
        [Parameter(Position = 2, Mandatory = false)]
        SwitchParameter Unbiased = false;

        protected override void Process()
        {
            var result = Measures.Kurtosis(_values.ToArray(), Unbiased);

            WriteObject(result);
        }
    }

    [Cmdlet("Get", "Math.LogGeometricMean")]
    [Alias("math.loggeometric")]
    public class GetMathLogGeometricMean : AggregateFunctionCmdletBase
    {
        protected override void Process()
        {
            var result = Measures.LogGeometricMean(_values.ToArray());

            WriteObject(result);
        }
    }

    [Cmdlet("Get", "Math.LowerQuartile")]
    [Alias("math.lowerq")]
    public class GetMathLowerquartile : AggregateFunctionCmdletBase
    {
        protected override void Process()
        {
            var result = Measures.LowerQuartile(_values.ToArray());

            WriteObject(result);
        }
    }

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

    [Cmdlet("Get", "Math.Median")]
    [Alias("math.median")]
    public class GetMathMedian : AggregateFunctionCmdletBase
    {
        protected override void Process()
        {
            var result = Measures.Median(_values.ToArray());

            WriteObject(result);
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

    [Cmdlet("Get", "Math.Mode")]
    [Alias("math.mode")]
    public class GetMathMode : AggregateFunctionCmdletBase
    {
        protected override void Process()
        {
            var result = Measures.Mode(_values.ToArray());

            WriteObject(result);
        }
    }

    [Cmdlet("Get", "Math.MovingMax")]
    [Alias("math.movingmmax")]
    public class GetMathMovingMax : FunctionCmdletBase
    {
        [Parameter(Position = 2, Mandatory = true)]
        public int WindowSize;

        private MovingNormalStatistics _state;

        protected override void Initialize()
        {
            _state = new MovingNormalStatistics(WindowSize);
        }

        protected override void ProcessInputObject(double value)
        {
            _state.Push(value);
            WriteObject(_state.Max());
        }
    }

    [Cmdlet("Get", "Math.MovingMean")]
    [Alias("math.movingmean")]
    public class GetMathMovingMean : FunctionCmdletBase
    {
        [Parameter(Position = 2, Mandatory = true)]
        public int WindowSize;

        private MovingNormalStatistics _state;

        protected override void Initialize()
        {
            _state = new MovingNormalStatistics(WindowSize);
        }

        protected override void ProcessInputObject(double value)
        {
            _state.Push(value);
            WriteObject(_state.Mean);
        }
    }

    [Cmdlet("Get", "Math.MovingMin")]
    [Alias("math.movingmmin")]
    public class GetMathMovingMin : FunctionCmdletBase
    {
        [Parameter(Position = 2, Mandatory = true)]
        public int WindowSize;

        private MovingNormalStatistics _state;

        protected override void Initialize()
        {
            _state = new MovingNormalStatistics(WindowSize);
        }

        protected override void ProcessInputObject(double value)
        {
            _state.Push(value);
            WriteObject(_state.Min());
        }
    }

    [Cmdlet("Get", "Math.MovingSum")]
    [Alias("math.movingsum")]
    public class GetMathMovingSum : FunctionCmdletBase
    {
        [Parameter(Position = 2, Mandatory = true)]
        public int WindowSize;

        private MovingNormalStatistics _state;

        protected override void Initialize()
        {
            _state = new MovingNormalStatistics(WindowSize);
        }

        protected override void ProcessInputObject(double value)
        {
            _state.Push(value);
            WriteObject(_state.Sum);
        }
    }

    [Cmdlet("Get", "Math.MovingVariance")]
    [Alias("math.movingvar")]
    public class GetMathMovingVariance : FunctionCmdletBase
    {
        [Parameter(Position = 2, Mandatory = true)]
        public int WindowSize;

        private MovingNormalStatistics _state;

        protected override void Initialize()
        {
            _state = new MovingNormalStatistics(WindowSize);
        }

        protected override void ProcessInputObject(double value)
        {
            _state.Push(value);
            WriteObject(_state.Variance);
        }
    }

    [Cmdlet("Get", "Math.Quantile")]
    [Alias("math.quantile")]
    public class GetMathQuantile : AggregateFunctionCmdletBase
    {
        [Parameter(Position = 2, Mandatory = true)]
        public double Probabilities;

        [Parameter(Position = 3, Mandatory = false)]
        public QuantileMethod QuantileMethod = QuantileMethod.Default;

        protected override void Process()
        {
            var result = _values.ToArray().Quantile(Probabilities, false, QuantileMethod, false);

            WriteObject(result);
        }
    }

    [Cmdlet("Get", "Math.Sample")]
    [Alias("math.sample")]
    public class GetMathSample : AggregateFunctionCmdletBase
    {
        [Parameter(Position = 2, Mandatory = true)]
        public int Size;

        [Parameter(Position = 3, Mandatory = false)]
        public SwitchParameter Replacement;

        protected override void Process()
        {
            // ref. https://en.wikipedia.org/wiki/Fisher-Yates_shuffle

            int count = _values.Count;

            if (Replacement) {
                for (var i = 0; i < Size; ++i) {
                    var j = Generator.Random.Next(count);
                    WriteObject(_values[j]);
                }
            }
            else {
                if (Size > count) {
                    WriteError(new ErrorRecord(new ArgumentException("Sample size too large"), "", ErrorCategory.InvalidArgument, null));
                }

                if (count / Size >= 5) {
                    var samples = new HashSet<double>();
                    for (var i = Size; i > 0;) {
                        var j = Generator.Random.Next(count);
                        if (samples.Contains(j)) {
                            continue;
                        }

                        samples.Add(j);
                        --i;

                        WriteObject(_values[j]);
                    }
                }
                else {
                    var table = new double[count];
                    for (var i = 0; i < count; ++i) {
                        var j = Generator.Random.Next(i + 1);
                        if (j != i) {
                            table[i] = table[j];
                        }
                        table[j] = _values[i];
                    }

                    for (var i = 0; i < Size; ++i) {
                        WriteObject(table[i]);
                    }
                }
            }
        }
    }

    [Cmdlet("Get", "Math.Scale")]
    [Alias("math.scale")]
    public class GetMathScale : AggregateFunctionCmdletBase
    {
        [Parameter(Position = 2, Mandatory = true)]
        public double ToMin;

        [Parameter(Position = 3, Mandatory = true)]
        public double ToMax;

        protected override void Process()
        {
            var result = _values.ToArray().Scale(ToMin, ToMax);

            WriteObject(result);
        }
    }

    [Cmdlet("Get", "Math.Skewness")]
    [Alias("math.skewness")]
    public class GetMathSkewness : AggregateFunctionCmdletBase
    {
        [Parameter(Position = 2, Mandatory = false)]
        public SwitchParameter Unbiased = false;

        protected override void Process()
        {
            var result = Measures.Skewness(_values.ToArray(), Unbiased);

            WriteObject(result);
        }
    }

    [Cmdlet("Get", "Math.StandardDeviation")]
    [Alias("math.stdev")]
    public class GetMathStandardDevidation : AggregateFunctionCmdletBase
    {
        [Parameter(Position = 2, Mandatory = false)]
        public SwitchParameter Unbiased = false;

        protected override void Process()
        {
            var result = Measures.StandardDeviation(_values.ToArray(), Unbiased);

            WriteObject(result);
        }
    }

    [Cmdlet("Get", "Math.StandardError")]
    [Alias("math.se")]
    public class GetMathStandardError : AggregateFunctionCmdletBase
    {
        protected override void Process()
        {
            var result = Measures.StandardError(_values.ToArray());

            WriteObject(result);
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

    [Cmdlet("Get", "Math.Summary")]
    [Alias("math.summary")]
    public class GetMathSummary : AggregateFunctionCmdletBase
    {
        protected override void Process()
        {
            var values = _values.ToArray();
            values.Sort();

            var result = new PSObject();
            var props = result.Properties;

            props.Add(new PSNoteProperty("Count", values.Length));
            props.Add(new PSNoteProperty("Minimum", values[0]));
            props.Add(new PSNoteProperty("LowerQuantile", values.Quantile(.25, true)));
            props.Add(new PSNoteProperty("Median", values.Median()));
            props.Add(new PSNoteProperty("Mean", values.Mean()));
            props.Add(new PSNoteProperty("UpperQuantile", values.Quantile(.75, true)));
            props.Add(new PSNoteProperty("Maximum", values[values.Length - 1]));

            WriteObject(result);
        }
    }

    [Cmdlet("Get", "Math.TruncatedMean")]
    [Alias("math.truncatedmean")]
    public class GetMathTruncatedMean : AggregateFunctionCmdletBase
    {
        [Parameter(Position = 2, Mandatory = true)]
        public double Percent;

        protected override void Process()
        {
            var result = Measures.TruncatedMean(_values.ToArray(), Percent);

            WriteObject(result);
        }
    }

    [Cmdlet("Get", "Math.UpperQuartile")]
    [Alias("math.upperq")]
    public class GetMathUpperQuartile : AggregateFunctionCmdletBase
    {
        protected override void Process()
        {
            var result = Measures.UpperQuartile(_values.ToArray());

            WriteObject(result);
        }
    }

    [Cmdlet("Get", "Math.Variance")]
    [Alias("math.var")]
    public class GetMathVariance : AggregateFunctionCmdletBase
    {
        [Parameter(Position = 2, Mandatory = false)]
        public SwitchParameter Unbiased = false;

        protected override void Process()
        {
            var result = Measures.Variance(_values.ToArray(), Unbiased);

            WriteObject(result);
        }
    }

    [Cmdlet("Get", "Math.ZScore")]
    [Alias("math.zscore")]
    public class GetMathZScore : AggregateFunctionCmdletBase
    {
        [Parameter(Position = 2, Mandatory = false)]
        public SwitchParameter Unbiased = false;

        protected override void Process()
        {
            var values = _values.ToArray();
            var mean = values.Mean();
            var sd = values.StandardDeviation(Unbiased);

            for (var i = 0; i < values.Length; ++i) {
                WriteObject((values[i] - mean) / sd);
            }
        }
    }

    #endregion

    #region Scalar arithmetics

    [Cmdlet("Get", "Math.Add")]
    [Alias("math.add")]
    public class GetMathPlus : FunctionCmdletBase
    {
        [Parameter(Position = 2, Mandatory = true, ParameterSetName = "Rhs")]
        public double Rhs;

        [Parameter(Position = 2, Mandatory = true, ParameterSetName = "Lhs")]
        public double Lhs;

        protected override void ProcessInputObject(double value)
        {
            if (ParameterSetName == "Rhs") {
                WriteObject(value + Rhs);
            }
            else {
                WriteObject(Lhs + value);
            }
        }
    }

    [Cmdlet("Get", "Math.Subtract")]
    [Alias("math.sub")]
    public class GetMathMinus : FunctionCmdletBase
    {
        [Parameter(Position = 2, Mandatory = true, ParameterSetName = "Rhs")]
        public double Rhs;

        [Parameter(Position = 2, Mandatory = true, ParameterSetName = "Lhs")]
        public double Lhs;

        protected override void ProcessInputObject(double value)
        {
            if (ParameterSetName == "Rhs") {
                WriteObject(value - Rhs);
            }
            else {
                WriteObject(Lhs - value);
            }
        }
    }

    [Cmdlet("Get", "Math.Multiply")]
    [Alias("math.mul")]
    public class GetMathMultiply : FunctionCmdletBase
    {
        [Parameter(Position = 2, Mandatory = true, ParameterSetName = "Rhs")]
        public double Rhs;

        [Parameter(Position = 2, Mandatory = true, ParameterSetName = "Lhs")]
        public double Lhs;

        protected override void ProcessInputObject(double value)
        {
            if (ParameterSetName == "Rhs") {
                WriteObject(value * Rhs);
            }
            else {
                WriteObject(Lhs * value);
            }
        }
    }

    [Cmdlet("Get", "Math.Divide")]
    [Alias("math.div")]
    public class GetMathDivide : FunctionCmdletBase
    {
        [Parameter(Position = 2, Mandatory = true, ParameterSetName = "Rhs")]
        public double Rhs;

        [Parameter(Position = 2, Mandatory = true, ParameterSetName = "Lhs")]
        public double Lhs;

        protected override void ProcessInputObject(double value)
        {
            if (ParameterSetName == "Rhs") {
                WriteObject(value / Rhs);
            }
            else {
                WriteObject(Lhs / value);
            }
        }
    }

    [Cmdlet("Get", "Math.Negate")]
    [Alias("math.neg")]
    public class GetMathNegate : FunctionCmdletBase
    {
        protected override void ProcessInputObject(double value)
        {
            WriteObject(-value);
        }
    }

    [Cmdlet("Get", "Math.Inverse")]
    [Alias("math.inv")]
    public class GetMathInverse : FunctionCmdletBase
    {
        protected override void ProcessInputObject(double value)
        {
            WriteObject(1.0 / value);
        }
    }

    #endregion
}