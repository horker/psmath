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
    #region Base class for math function cmdlets

    public class FunctionCmdletBase : PSCmdlet
    {
        [Parameter(Position = 0, Mandatory = false)]
        public object[] Values;

        [Parameter(ValueFromPipeline = true, Mandatory = false)]
        public object InputObject;

        protected virtual void Initialize() { }

        protected virtual void ProcessInputObject(double value) { }

        protected virtual void Process() { }

        protected override void BeginProcessing()
        {
            Initialize();
        }

        protected override void ProcessRecord()
        {
            if (InputObject != null) {
                if (Values != null && Values.Length > 0) {
                    WriteError(new ErrorRecord(new ArgumentException("Both pipeline and -Value arguments are given"), "", ErrorCategory.InvalidArgument, null));
                    return;
                }

                ProcessInputObject(Converter.ToDouble(InputObject));
            }
        }

        protected override void EndProcessing()
        {
            if (Values != null) {
                foreach (var value in Values) {
                    ProcessInputObject(Converter.ToDouble(value));
                }
            }

            Process();
        }
    }

    public abstract class AggregateFunctionCmdletBase : PSCmdlet
    {
        [Parameter(Position = 0, Mandatory = false)]
        public object[] Values;

        [Parameter(ValueFromPipeline = true, Mandatory = false)]
        public object InputObject;

        private List<double> _inputObjects;

        protected override void ProcessRecord()
        {
            if (InputObject != null) {
                if (_inputObjects == null) {
                    _inputObjects = new List<double>();
                }

                _inputObjects.Add(Converter.ToDouble(InputObject));
            }
        }

        protected abstract void Process(double[] values);

        protected override void EndProcessing()
        {
            double[] values;

            if (_inputObjects != null) {
                if (Values != null && Values.Length > 0) {
                    WriteError(new ErrorRecord(new ArgumentException("Both pipeline and -Value arguments are given"), "", ErrorCategory.InvalidArgument, null));
                    return;
                }
                values = _inputObjects.ToArray();
            }
            else {
                if (Values != null) {
                    values = Values.Apply(x => Converter.ToDouble(x));
                }
                else {
                    values = new double[0];
                }
            }

            Process(values);
        }

    }

    #endregion

    #region Accord.Math.Random

    [Cmdlet("Get", "Math.RandomSeed")]
    public class GetMathRandomSeed : PSCmdlet
    {
        protected override void EndProcessing()
        {
            WriteObject(Generator.Seed);
        }
    }

    [Cmdlet("Set", "Math.RandomSeed")]
    [Alias("math.randomseed")]
    public class SetMathRandomSeed : PSCmdlet
    {
        [AllowNull()]
        [Parameter(Position = 0, Mandatory = true)]
        public int? Seed;

        protected override void EndProcessing()
        {
            Generator.Seed = Seed;
        }
    }

    #endregion

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
        protected override void Process(double[] values)
        {
            var results = Special.Softmax(values);

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
        protected override void Process(double[] values)
        {
            var result = Measures.ContraHarmonicMean(values);

            WriteObject(result);
        }
    }

    [Cmdlet("Get", "Math.CumulativeSum")]
    [Alias("math.cumsum")]
    public class GetMathCumulativeSum : AggregateFunctionCmdletBase
    {
        protected override void Process(double[] values)
        {
            var result = values.CumulativeSum();

            WriteObject(result);
        }
    }

    [Cmdlet("Get", "Math.Entropy")]
    [Alias("math.entropy")]
    public class GetMathEntropy : AggregateFunctionCmdletBase
    {
        protected override void Process(double[] values)
        {
            var result = Measures.Entropy(values);

            WriteObject(result);
        }
    }

    [Cmdlet("Get", "Math.Difference")]
    [Alias("math.diff")]
    public class GetMathDifference : FunctionCmdletBase
    {
        private bool _first;
        private double _prev;

        protected override void Initialize()
        {
            _first = true;
        }

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

        protected override void Process(double[] values)
        {
            var min = values.Min();
            var max = values.Max();

            if (double.IsNaN(BinWidth)) {
                var binCount = Math.Min(100, Math.Ceiling(Math.Sqrt(values.Length)));
                BinWidth = (max - min) / binCount;
            }

            int minBar = (int)Math.Floor((min - Offset) / BinWidth);
            int maxBar = (int)Math.Floor((max - Offset) / BinWidth);

            var hist = new int[maxBar - minBar + 1];

            foreach (var value in values) {
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
        protected override void Process(double[] values)
        {
            var result = Measures.GeometricMean(values);

            WriteObject(result);
        }
    }

    [Cmdlet("Get", "Math.Kurtosis")]
    [Alias("math.kurtosis")]
    public class GetMathKurtosis : AggregateFunctionCmdletBase
    {
        [Parameter(Position = 1, Mandatory = false)]
        SwitchParameter Unbiased = false;

        protected override void Process(double[] values)
        {
            var result = Measures.Kurtosis(values, Unbiased);

            WriteObject(result);
        }
    }

    [Cmdlet("Get", "Math.LogGeometricMean")]
    [Alias("math.loggeometric")]
    public class GetMathLogGeometricMean : AggregateFunctionCmdletBase
    {
        protected override void Process(double[] values)
        {
            var result = Measures.LogGeometricMean(values);

            WriteObject(result);
        }
    }

    [Cmdlet("Get", "Math.LowerQuartile")]
    [Alias("math.lowerq")]
    public class GetMathLowerquartile : AggregateFunctionCmdletBase
    {
        protected override void Process(double[] values)
        {
            var result = Measures.LowerQuartile(values);

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
        protected override void Process(double[] values)
        {
            var result = Measures.Median(values);

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
        protected override void Process(double[] values)
        {
            var result = Measures.Mode(values);

            WriteObject(result);
        }
    }

    [Cmdlet("Get", "Math.MovingMax")]
    [Alias("math.movingmmax")]
    public class GetMathMovingMax : FunctionCmdletBase
    {
        [Parameter(Position = 1, Mandatory = true)]
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
        [Parameter(Position = 1, Mandatory = true)]
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
        [Parameter(Position = 1, Mandatory = true)]
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

    [Cmdlet("Get", "Math.MovingStandardDeviation")]
    [Alias("math.movingstdev")]
    public class GetMathMovingStandardDeviation : FunctionCmdletBase
    {
        [Parameter(Position = 1, Mandatory = true)]
        public int WindowSize;

        private MovingNormalStatistics _state;

        protected override void Initialize()
        {
            _state = new MovingNormalStatistics(WindowSize);
        }

        protected override void ProcessInputObject(double value)
        {
            _state.Push(value);
            WriteObject(_state.StandardDeviation);
        }
    }

    [Cmdlet("Get", "Math.MovingSum")]
    [Alias("math.movingsum")]
    public class GetMathMovingSum : FunctionCmdletBase
    {
        [Parameter(Position = 1, Mandatory = true)]
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
        [Parameter(Position = 1, Mandatory = true)]
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
        [Parameter(Position = 1, Mandatory = true)]
        public double Probabilities;

        [Parameter(Position = 2, Mandatory = false)]
        public QuantileMethod QuantileMethod = QuantileMethod.Default;

        protected override void Process(double[] values)
        {
            var result = values.Quantile(Probabilities, false, QuantileMethod, false);

            WriteObject(result);
        }
    }

    [Cmdlet("Get", "Math.Sample")]
    [Alias("math.sample")]
    public class GetMathSample : AggregateFunctionCmdletBase
    {
        [Parameter(Position = 1, Mandatory = true)]
        public int Size;

        [Parameter(Position = 2, Mandatory = false)]
        public SwitchParameter Replacement;

        protected override void Process(double[] values)
        {
            // ref. https://en.wikipedia.org/wiki/Fisher-Yates_shuffle

            int count = values.Length;

            if (Replacement) {
                for (var i = 0; i < Size; ++i) {
                    var j = Generator.Random.Next(count);
                    WriteObject(values[j]);
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

                        WriteObject(values[j]);
                    }
                }
                else {
                    var table = new double[count];
                    for (var i = 0; i < count; ++i) {
                        var j = Generator.Random.Next(i + 1);
                        if (j != i) {
                            table[i] = table[j];
                        }
                        table[j] = values[i];
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
        [Parameter(Position = 1, Mandatory = true)]
        public double ToMin;

        [Parameter(Position = 2, Mandatory = true)]
        public double ToMax;

        protected override void Process(double[] values)
        {
            var result = values.Scale(ToMin, ToMax);

            WriteObject(result);
        }
    }

    [Cmdlet("Get", "Math.Shuffle")]
    [Alias("math.shuffle")]
    public class GetMathShuffle : AggregateFunctionCmdletBase
    {
        protected override void Process(double[] values)
        {
            // ref. https://en.wikipedia.org/wiki/Fisher-Yates_shuffle

            int count = values.Length;

            var table = new double[count];
            for (var i = 0; i < count; ++i) {
                var j = Generator.Random.Next(i + 1);
                if (j != i) {
                    table[i] = table[j];
                }
                table[j] = values[i];
            }

            for (var i = 0; i < count; ++i) {
                WriteObject(table[i]);
            }
        }
    }

    [Cmdlet("Get", "Math.Skewness")]
    [Alias("math.skewness")]
    public class GetMathSkewness : AggregateFunctionCmdletBase
    {
        [Parameter(Position = 1, Mandatory = false)]
        public SwitchParameter Unbiased = false;

        protected override void Process(double[] values)
        {
            var result = Measures.Skewness(values, Unbiased);

            WriteObject(result);
        }
    }

    [Cmdlet("Get", "Math.StandardDeviation")]
    [Alias("math.stdev")]
    public class GetMathStandardDevidation : AggregateFunctionCmdletBase
    {
        [Parameter(Position = 1, Mandatory = false)]
        public SwitchParameter Unbiased = false;

        protected override void Process(double[] values)
        {
            var result = Measures.StandardDeviation(values, Unbiased);

            WriteObject(result);
        }
    }

    [Cmdlet("Get", "Math.StandardError")]
    [Alias("math.se")]
    public class GetMathStandardError : AggregateFunctionCmdletBase
    {
        protected override void Process(double[] values)
        {
            var result = Measures.StandardError(values);

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
        protected override void Process(double[] values)
        {
            var sorted = values.Sorted();

            var result = new PSObject();
            var props = result.Properties;

            props.Add(new PSNoteProperty("Count", sorted.Length));
            props.Add(new PSNoteProperty("Minimum", sorted[0]));
            props.Add(new PSNoteProperty("LowerQuantile", sorted.Quantile(.25, true)));
            props.Add(new PSNoteProperty("Median", sorted.Median()));
            props.Add(new PSNoteProperty("Mean", sorted.Mean()));
            props.Add(new PSNoteProperty("UpperQuantile", sorted.Quantile(.75, true)));
            props.Add(new PSNoteProperty("Maximum", sorted[sorted.Length - 1]));

            WriteObject(result);
        }
    }

    [Cmdlet("Get", "Math.TruncatedMean")]
    [Alias("math.truncatedmean")]
    public class GetMathTruncatedMean : AggregateFunctionCmdletBase
    {
        [Parameter(Position = 1, Mandatory = true)]
        public double Percent;

        protected override void Process(double[] values)
        {
            var result = Measures.TruncatedMean(values, Percent);

            WriteObject(result);
        }
    }

    [Cmdlet("Get", "Math.UpperQuartile")]
    [Alias("math.upperq")]
    public class GetMathUpperQuartile : AggregateFunctionCmdletBase
    {
        protected override void Process(double[] values)
        {
            var result = Measures.UpperQuartile(values);

            WriteObject(result);
        }
    }

    [Cmdlet("Get", "Math.Variance")]
    [Alias("math.var")]
    public class GetMathVariance : AggregateFunctionCmdletBase
    {
        [Parameter(Position = 1, Mandatory = false)]
        public SwitchParameter Unbiased = false;

        protected override void Process(double[] values)
        {
            var result = Measures.Variance(values, Unbiased);

            WriteObject(result);
        }
    }

    [Cmdlet("Get", "Math.ZScore")]
    [Alias("math.zscore")]
    public class GetMathZScore : AggregateFunctionCmdletBase
    {
        [Parameter(Position = 1, Mandatory = false)]
        public SwitchParameter Unbiased = false;

        protected override void Process(double[] values)
        {
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
        [Parameter(Position = 1, Mandatory = true, ParameterSetName = "Rhs")]
        public double Rhs;

        [Parameter(Position = 1, Mandatory = true, ParameterSetName = "Lhs")]
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
        [Parameter(Position = 1, Mandatory = true, ParameterSetName = "Rhs")]
        public double Rhs;

        [Parameter(Position = 1, Mandatory = true, ParameterSetName = "Lhs")]
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
        [Parameter(Position = 1, Mandatory = true, ParameterSetName = "Rhs")]
        public double Rhs;

        [Parameter(Position = 1, Mandatory = true, ParameterSetName = "Lhs")]
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
        [Parameter(Position = 1, Mandatory = true, ParameterSetName = "Rhs")]
        public double Rhs;

        [Parameter(Position = 1, Mandatory = true, ParameterSetName = "Lhs")]
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

    [Cmdlet("Get", "Math.Reminder")]
    [Alias("math.rem")]
    public class GetMathReminder : FunctionCmdletBase
    {
        [Parameter(Position = 1, Mandatory = true, ParameterSetName = "Rhs")]
        public double Rhs;

        [Parameter(Position = 1, Mandatory = true, ParameterSetName = "Lhs")]
        public double Lhs;

        protected override void ProcessInputObject(double value)
        {
            if (ParameterSetName == "Rhs") {
                WriteObject(value % Rhs);
            }
            else {
                WriteObject(Lhs % value);
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