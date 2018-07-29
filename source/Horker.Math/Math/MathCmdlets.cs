using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using Accord.Math;
using Accord.Statistics;
using Accord.Math.Random;
using Accord.Statistics.Moving;

namespace Horker.Math
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
    [Alias("math.seed")]
    public class GetMathRandomSeed : PSCmdlet
    {
        protected override void EndProcessing()
        {
            WriteObject(Generator.Seed);
        }
    }

    [Cmdlet("Set", "Math.RandomSeed")]
    [Alias("math.setseed")]
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

    [Cmdlet("Get", "Math.Random")]
    [Alias("math.rand")]
    public class GetMathRandom : PSCmdlet
    {
        [Parameter(Position = 0, Mandatory = false)]
        public double? Lower;

        [Parameter(Position = 1, Mandatory = false)]
        public double? Upper;

        [Parameter(Position = 2, Mandatory = false)]
        public int Count = 1;

        protected override void EndProcessing()
        {
            var r = Generator.Random;

            if (!Upper.HasValue)
            {
                if (!Lower.HasValue)
                {
                    for (var i = 0; i < Count; ++i)
                        WriteObject(r.NextDouble());
                }
                else
                {
                    for (var i = 0; i < Count; ++i)
                        WriteObject(r.NextDouble() * Lower.Value);
                }
            }
            else
            {
                var range = Upper.Value - Lower.Value;
                for (var i = 0; i < Count; ++i)
                    WriteObject(r.NextDouble() * range + Lower.Value);
            }
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
            WriteObject(System.Math.Abs(value));
        }
    }

    [Cmdlet("Get", "Math.Acos")]
    [Alias("math.acos")]
    public class GetMathAcos : FunctionCmdletBase
    {
        protected override void ProcessInputObject(double value)
        {
            WriteObject(System.Math.Acos(value));
        }
    }

    [Cmdlet("Get", "Math.Asin")]
    [Alias("math.asin")]
    public class GetMathAsin : FunctionCmdletBase
    {
        protected override void ProcessInputObject(double value)
        {
            WriteObject(System.Math.Asin(value));
        }
    }

    [Cmdlet("Get", "Math.Atan")]
    [Alias("math.atan")]
    public class GetMathAtan : FunctionCmdletBase
    {
        protected override void ProcessInputObject(double value)
        {
            WriteObject(System.Math.Atan(value));
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
            WriteObject(System.Math.Atan2(value, Arg));
        }
    }

    [Cmdlet("Get", "Math.Cos")]
    [Alias("math.cos")]
    public class GetMathCos : FunctionCmdletBase
    {
        protected override void ProcessInputObject(double value)
        {
            WriteObject(System.Math.Cos(value));
        }
    }

    [Cmdlet("Get", "Math.Cosh")]
    [Alias("math.cosh")]
    public class GetMathCosh : FunctionCmdletBase
    {
        protected override void ProcessInputObject(double value)
        {
            WriteObject(System.Math.Cosh(value));
        }
    }

    [Cmdlet("Get", "Math.Ceiling")]
    [Alias("math.ceiling")]
    public class GetMathCeiling : FunctionCmdletBase
    {
        protected override void ProcessInputObject(double value)
        {
            WriteObject(System.Math.Ceiling(value));
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
            long quotient = System.Math.DivRem(A, B, out remainder);

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
            WriteObject(System.Math.Exp(value));
        }
    }

    [Cmdlet("Get", "Math.Floor")]
    [Alias("math.floor")]
    public class GetMathFloor : FunctionCmdletBase
    {
        protected override void ProcessInputObject(double value)
        {
            WriteObject(System.Math.Floor(value));
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
            WriteObject(System.Math.IEEERemainder(value, Arg));
        }
    }

    [Cmdlet("Get", "Math.Log")]
    [Alias("math.log")]
    public class GetMathLog : FunctionCmdletBase
    {
        protected override void ProcessInputObject(double value)
        {
            WriteObject(System.Math.Log(value));
        }
    }

    [Cmdlet("Get", "Math.Log10")]
    [Alias("math.log10")]
    public class GetMathLog10 : FunctionCmdletBase
    {
        protected override void ProcessInputObject(double value)
        {
            WriteObject(System.Math.Log10(value));
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
            WriteObject(System.Math.Pow(value, Arg));
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
            WriteObject(System.Math.Round(value, Digits, Mode));
        }
    }

    [Cmdlet("Get", "Math.Sigmoid")]
    [Alias("math.sigmoid")]
    public class GetMathSigmoid : FunctionCmdletBase
    {
        protected override void ProcessInputObject(double value)
        {
            WriteObject(1.0 / (1.0 + System.Math.Exp(-value)));
        }
    }

    [Cmdlet("Get", "Math.Sign")]
    [Alias("math.sign")]
    public class GetMathSign : FunctionCmdletBase
    {
        protected override void ProcessInputObject(double value)
        {
            WriteObject(System.Math.Sign(value));
        }
    }

    [Cmdlet("Get", "Math.Sin")]
    [Alias("math.sin")]
    public class GetMathSin : FunctionCmdletBase
    {
        protected override void ProcessInputObject(double value)
        {
            WriteObject(System.Math.Sin(value));
        }
    }

    [Cmdlet("Get", "Math.Sinh")]
    [Alias("math.sinh")]
    public class GetMathSinh : FunctionCmdletBase
    {
        protected override void ProcessInputObject(double value)
        {
            WriteObject(System.Math.Sinh(value));
        }
    }

    [Cmdlet("Get", "Math.Sqrt")]
    [Alias("math.sqrt")]
    public class GetMathSqrt : FunctionCmdletBase
    {
        protected override void ProcessInputObject(double value)
        {
            WriteObject(System.Math.Sqrt(value));
        }
    }

    [Cmdlet("Get", "Math.Tan")]
    [Alias("math.tan")]
    public class GetMathTan : FunctionCmdletBase
    {
        protected override void ProcessInputObject(double value)
        {
            WriteObject(System.Math.Tan(value));
        }
    }

    [Cmdlet("Get", "Math.Tanh")]
    [Alias("math.tanh")]
    public class GetMathTanh : FunctionCmdletBase
    {
        protected override void ProcessInputObject(double value)
        {
            WriteObject(System.Math.Tanh(value));
        }
    }

    [Cmdlet("Get", "Math.Truncate")]
    [Alias("math.truncate")]
    public class GetMathTruncate : FunctionCmdletBase
    {
        protected override void ProcessInputObject(double value)
        {
            WriteObject(System.Math.Truncate(value));
        }
    }

    [Cmdlet("Get", "Math.E")]
    [Alias("math.e")]
    public class GetMathE : PSCmdlet
    {
        protected override void EndProcessing()
        {
            WriteObject(System.Math.E);
        }
    }

    [Cmdlet("Get", "Math.PI")]
    [Alias("math.pi")]
    public class GetMathPi : PSCmdlet
    {
        protected override void EndProcessing()
        {
            WriteObject(System.Math.PI);
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

    #region Math.Measures

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

    [Cmdlet("Get", "Math.ExponetialWeightedMean")]
    [Alias("math.ewmean")]
    public class GetMathExponentialWeightedMean : AggregateFunctionCmdletBase
    {
        [Parameter(Position = 1, Mandatory = false)]
        public int Window = -1;

        [Parameter(Position = 2, Mandatory = false)]
        public double Alpha;

        protected override void Process(double[] values)
        {
            double result;

            if (Window == -1)
            {
                result = Measures.ExponentialWeightedMean(values, Alpha);
            }
            else
            {
                result = Measures.ExponentialWeightedMean(values, Window, Alpha);
            }

            WriteObject(result);
        }
    }

    [Cmdlet("Get", "Math.ExponetialWeightedVariance")]
    [Alias("math.ewvar")]
    public class GetMathExponentialWeightedVariance : AggregateFunctionCmdletBase
    {
        [Parameter(Position = 1, Mandatory = false)]
        public int Window = -1;

        [Parameter(Position = 2, Mandatory = false)]
        public double Alpha;

        protected override void Process(double[] values)
        {
            double result;

            if (Window == -1)
            {
                result = Measures.ExponentialWeightedVariance(values, Alpha);
            }
            else
            {
                result = Measures.ExponentialWeightedVariance(values, Window, Alpha);
            }

            WriteObject(result);
        }
    }

    [Cmdlet("Get", "Math.Difference")]
    [Alias("math.diff")]
    public class GetMathDifference : FunctionCmdletBase
    {
        [Parameter(Position = 1, Mandatory = false)]
        public SwitchParameter NoFirstZero;

        private bool _first;
        private double _prev;

        protected override void Initialize()
        {
            _first = true;
        }

        protected override void ProcessInputObject(double value)
        {
            if (_first) {
                if (!NoFirstZero)
                    WriteObject(0.0);
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
            var hist = ArrayMethods.Untyped.Additional.HistogramInternal(values, BinWidth, Offset);
            foreach (var bin in hist)
                WriteObject(bin);
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
    [Alias("math.stddev")]
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
            var result = new DataSummary(values);

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

    // TODO:
    // WeightedEntropy
    // WeightedMax
    // WeightedMean
    // WeightedMin
    // WeightedMode
    // WeightedScatter
    // WeightedStandardDeviation
    // WeightedVariance

    #endregion

    #region Accord.Math.Matrix

    [Cmdlet("Get", "Math.ArgMax")]
    [Alias("math.argmax")]
    public class GetMathArgMax : AggregateFunctionCmdletBase
    {
        protected override void Process(double[] values)
        {
            var result = Accord.Math.Matrix.ArgMax(values);

            WriteObject(result);
        }
    }

    [Cmdlet("Get", "Math.ArgMin")]
    [Alias("math.argmin")]
    public class GetMathArgMin : AggregateFunctionCmdletBase
    {
        protected override void Process(double[] values)
        {
            var result = Accord.Math.Matrix.ArgMin(values);

            WriteObject(result);
        }
    }

    [Cmdlet("Get", "Math.ArgSort")]
    [Alias("math.argsort")]
    public class GetMathArgSort : AggregateFunctionCmdletBase
    {
        protected override void Process(double[] values)
        {
            var result = Accord.Math.Matrix.ArgSort(values);

            WriteObject(result);
        }
    }

    [Cmdlet("Get", "Math.Distinct")]
    [Alias("math.distinct")]
    public class GetMathDistinct : AggregateFunctionCmdletBase
    {
        protected override void Process(double[] values)
        {
            var result = Accord.Math.Matrix.Distinct(values);

            foreach (var value in result) {
                WriteObject(value);
            }
        }
    }

    [Cmdlet("Get", "Math.DistinctCount")]
    [Alias("math.distinctCount")]
    public class GetMathDistinctCount : AggregateFunctionCmdletBase
    {
        protected override void Process(double[] values)
        {
            var result = Accord.Math.Matrix.DistinctCount(values);

            WriteObject(result);
        }
    }

    [Cmdlet("Test", "Math.Sorted")]
    [Alias("math.issorted")]
    public class TestMathSorted : AggregateFunctionCmdletBase
    {
        protected override void Process(double[] values)
        {
            var result = Accord.Math.Matrix.IsSorted(values);

            WriteObject(result);
        }
    }

    [Cmdlet("Get", "Math.Normalize")]
    [Alias("math.normalize")]
    public class GetMathNormalize : AggregateFunctionCmdletBase
    {
        [Parameter(Position = 1, Mandatory = false)]
        public SwitchParameter Unbiased = false;

        protected override void Process(double[] values)
        {
            var result = Accord.Math.Matrix.Normalize(values);

            foreach (var value in result) {
                WriteObject(value);
            }
        }
    }

    [Cmdlet("Get", "Math.Reverse")]
    [Alias("math.reverse")]
    public class GetMathReverse : AggregateFunctionCmdletBase
    {
        protected override void Process(double[] values)
        {
            var result = Accord.Math.Matrix.Reversed(values);

            foreach (var value in values) {
                WriteObject(value);
            }
        }
    }

    #endregion

    #region Accord.Statistics.Tools

    [Cmdlet("Get", "Math.Center")]
    [Alias("math.center")]
    public class GetMathCenter : AggregateFunctionCmdletBase
    {
        protected override void Process(double[] values)
        {
            var result = Accord.Statistics.Tools.Center(values);

            foreach (var value in result) {
                WriteObject(value);
            }
        }
    }

    [Cmdlet("Get", "Math.Rank")]
    [Alias("math.rank")]
    public class GetMathRank : AggregateFunctionCmdletBase
    {
        [Parameter(Position = 1, Mandatory = false)]
        public SwitchParameter AdjustForTies;

        protected override void Process(double[] values)
        {
            var result = Accord.Statistics.Tools.Rank(values, false, AdjustForTies);

            foreach (var value in result) {
                WriteObject(value);
            }
        }
    }

    [Cmdlet("Get", "Math.Standardize")]
    [Alias("math.standardize")]
    public class GetMathStandardize : AggregateFunctionCmdletBase
    {
        protected override void Process(double[] values)
        {
            var result = Accord.Statistics.Tools.Standardize(values);

            foreach (var value in result) {
                WriteObject(value);
            }
        }
    }

    [Cmdlet("Get", "Math.Ties")]
    [Alias("math.ties")]
    public class GetMathTies : AggregateFunctionCmdletBase
    {
        protected override void Process(double[] values)
        {
            var result = Accord.Statistics.Tools.Ties(values);

            foreach (var value in result) {
                WriteObject(value);
            }
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
            var v = new double[values.Length, 1];
            Array.Copy(values, v, values.Length);

            var result = Accord.Statistics.Tools.ZScores(v);

            foreach (var value in result)
            {
                WriteObject(value);
            }
        }
    }

    #endregion

}