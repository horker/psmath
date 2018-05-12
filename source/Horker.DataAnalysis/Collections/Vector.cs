using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text.RegularExpressions;
using Accord;
using Accord.Math;
using Accord.Statistics;
using Accord.Math.Random;
using Accord.Statistics.Moving;
using System.Diagnostics;

namespace Horker.DataAnalysis
{
    public enum CodificationType
    {
        OneHotDropFirst,
        OneHot,
        Multilevel,
        Boolean
    }

    public class Vector : List<object>
    {
        private WeakReference _arrayCache;

        #region Constructors

        public Vector()
            : base()
        {
        }

        public Vector(int capacity)
            : base(capacity)
        {
        }

        public Vector(Vector v)
            : base(v)
        {
        }

        public Vector(IEnumerable data)
            : base()
        {
            foreach (var d in data) {
                Add(d);
            }
        }

        #endregion

        #region Factory methods

        public static Vector Create<T>(IEnumerable<T> data)
        {
            var result = new Vector(data.Count());
            foreach (var d in data) {
                result.Add(d);
            }
            return result;
        }

        public static Vector Create<T>(T[] data)
        {
            var result = new Vector(data.Count());
            foreach (var d in data) {
                result.Add(d);
            }
            return result;
        }

        public static Vector GetDoubleRange(double a, double b, double step = 1.0, bool inclusive = true)
        {
            var result = new Vector();

            if (inclusive) {
                if (a <= b) {
                    if (step <= 0.0) {
                        throw new RuntimeException("Range definition inconsistent");
                    }
                    for (var i = a; i <= b; i += step) {
                        result.Add(i);
                    }
                }
                else {
                    if (step >= 0.0) {
                        throw new RuntimeException("Range definition inconsistent");
                    }
                    for (var i = a; i >= b; i += step) {
                        result.Add(i);
                    }
                }
            }
            else {
                if (a <= b) {
                    if (step <= 0.0) {
                        throw new RuntimeException("Range definition inconsistent");
                    }
                    for (var i = a; i < b; i += step) {
                        result.Add(i);
                    }
                }
                else {
                    if (step >= 0.0) {
                        throw new RuntimeException("Range definition inconsistent");
                    }
                    for (var i = a; i > b; i += step) {
                        result.Add(i);
                    }
                }
            }

            return result;
        }

        public static Vector GetDoubleInterval(double a, double b, int n, bool inclusive = true)
        {
            if (a >= b || n <= 0) {
                throw new RuntimeException("Range definition inconsistent");
            }

            var result = new Vector(n);

            Double step;

            if (inclusive) {
                step = (b - a) / (n - 1);
                for (var i = 0; i < n; ++i) {
                    result.Add(a + step * i);
                }
            }
            else {
                step = (b - a) / n;
                for (var i = 0; i < n; ++i) {
                    result.Add(a + step * i);
                }
            }

            return result;
        }

        public static Vector WithValue(double value, int size)
        {
            var result = new Vector(size);

            for (int i = 0; i < size; ++i) {
                result.Add(value);
            }

            return result;
        }

        public static Vector Zero(int size)
        {
            return WithValue(0, size);
        }

        private void InvalidateCache()
        {
            _arrayCache.Target = null;
        }

        #endregion

        #region Conversions

        public double[] ToDoubleArray()
        {
            if (_arrayCache != null) {
                if (_arrayCache.IsAlive && _arrayCache.Target != null) {
                    return (double[])_arrayCache.Target;
                }
            }
            else {
                _arrayCache = new WeakReference(null);
            }

            var result = new double[Count];

            for (var i = 0; i < Count; ++i) {
                result[i] = Converter.ToDouble(this[i]);
            }

            _arrayCache.Target = result;

            return result;
        }

        public void ToDoubleArray(double[] dest)
        {
            for (var i = 0; i < Count; ++i) {
                dest[i] = Converter.ToDouble(this[i]);
            }
        }

        public int[] ToIntArray()
        {
            var result = new int[Count];

            for (var i = 0; i < Count; ++i) {
                result[i] = Convert.ToInt32(this[i]);
            }

            return result;
        }

        public DataFrame ToDummyValues(DataFrame dataFrame, string baseName, CodificationType codificationType = CodificationType.OneHotDropFirst)
        {
            HashSet<object> set = new HashSet<object>(new ObjectKeyComparer());
            foreach (var e in this) {
                set.Add(e);
            }

            var factors = new List<object>();
            foreach (var e in set) {
                factors.Add(e);
            }
            factors.Sort();

            var comparer = new ObjectKeyComparer();
            if (codificationType == CodificationType.Multilevel) {
                var values = new List<object>(Count);
                foreach (var e in this) {
                    var index = factors.FindIndex((x) => comparer.Equals(x, e));
                    values.Add(index);
                }
                dataFrame.AddColumn(baseName, values);
            }
            else {
                bool excludeFirst = codificationType == CodificationType.OneHotDropFirst;
                bool asBoolean = codificationType == CodificationType.Boolean;

                var values = new List<object>[factors.Count - (excludeFirst ? 1 : 0)];
                for (var i = 0; i < values.Length; ++i) {
                    values[i] = new List<object>(Count);
                }

                foreach (var e in this) {
                    var index = factors.FindIndex((x) => comparer.Equals(x, e));
                    if (excludeFirst) {
                        --index;
                    }
                    for (var i = 0; i < values.Length; ++i) {
                        if (asBoolean) {
                            values[i].Add(i == index);
                        }
                        else {
                            values[i].Add(i == index ? 1 : 0);
                        }
                    }
                }

                for (var i = 0; i < values.Length; ++i) {
                    var name = baseName + factors[i + (excludeFirst ? 1 : 0)].ToString();
                    dataFrame.AddColumn(name, values[i]);
                }
            }

            return dataFrame;
        }

        public DataFrame ToDummyValues(string baseName, CodificationType codificationType = CodificationType.OneHotDropFirst)
        {
            var dataFrame = new DataFrame();
            return ToDummyValues(dataFrame, baseName, codificationType);
        }

        #endregion

        #region Inplace convertions

        public void AsDouble()
        {
            for (var i = 0; i < this.Count; ++i) {
                this[i] = Converter.ToDouble(this[i]);
            }
        }

        public void AsBoolean()
        {
            for (var i = 0; i < this.Count; ++i) {
                this[i] = Convert.ToBoolean(this[i]);
            }
        }

        public void AsString()
        {
            for (var i = 0; i < this.Count; ++i) {
                this[i] = Convert.ToString(this[i]);
            }
        }

        public void AsDateTime()
        {
            for (var i = 0; i < this.Count; ++i) {
                this[i] = Converter.ToDateTime(this[i]);
            }
        }

        public void AsDateTime(string format)
        {
            for (var i = 0; i < this.Count; ++i) {
                this[i] = Converter.ToDateTime(this[i], format);
            }
        }

        public void AsDateTimeFromUnixTime()
        {
            for (var i = 0; i < this.Count; ++i) {
                var sec = Converter.ToDouble(this[i]);
                this[i] = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(sec);
            }
        }

        public void AsDateTimeOffset()
        {
            for (var i = 0; i < this.Count; ++i) {
                this[i] = Converter.ToDateTimeOffset(this[i]);
            }
        }

        public void AsDateTimeOffset(string format)
        {
            for (var i = 0; i < this.Count; ++i) {
                this[i] = Converter.ToDateTimeOffset(this[i], format);
            }
        }

        public void AsDateTimeOffsetFromUnixTime()
        {
            var epoch = new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero);
            for (var i = 0; i < this.Count; ++i) {
                var sec = Converter.ToDouble(this[i]);
                this[i] = epoch.AddSeconds(sec);
            }
        }

        public void AsUnixTime()
        {
            var epoch = new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero);
            for (var i = 0; i < this.Count; ++i) {
                var d = Converter.ToDateTimeOffset(this[i]);
                if (d == null) {
                    continue;
                }

                this[i] = ((DateTimeOffset)d - epoch).TotalSeconds;
            }
        }

        #endregion

        #region Data manupilations

        public void AddRange<T>(IEnumerable<T> data)
        {
            foreach (var d in data) {
                Add(d);
            }
        }

        public void AddRange<T>(T[] data)
        {
            foreach (var d in data) {
                Add(d);
            }
        }

        public void Replace<T>(IEnumerable<T> data)
        {
            Clear();
            foreach (var d in data) {
                Add(d);
            }
        }

        public void Replace<T>(T[] data)
        {
            Clear();
            foreach (var d in data) {
                Add(d);
            }
        }

        public void Replace(string pattern, string replacement)
        {
            var re = new Regex(pattern);

            for (var i = 0; i < this.Count; ++i) {
                this[i] = re.Replace(this[i].ToString(), replacement);
            }
        }

        public void Replace(Regex re, string replacement)
        {
            for (var i = 0; i < this.Count; ++i) {
                this[i] = re.Replace(this[i].ToString(), replacement);
            }
        }

        public void Shuffle()
        {
            // ref. https://en.wikipedia.org/wiki/Fisher%E2%80%93Yates_shuffle

            int count = this.Count;
            for (var i = 0; i < count - 1; i++) {
                var j = Generator.Random.Next(i, count);
                var temp = base[i];
                base[i] = base[j];
                base[j] = temp;
            }
        }

        public void Sort(bool Ascending = false)
        {
            base.Sort();

            if (Ascending) {
                base.Reverse();
            }
        }

        public void ExtractNumber()
        {
            for (var i = 0; i < this.Count; ++i) {
                this[i] = Converter.ExtractNumber(this[i].ToString());
            }
        }

        #endregion

        #region Elementwise operations

        public Vector Apply(Func<object, object> f)
        {
            var result = new Vector(Count);

            for (var i = 0; i < Count; ++i) {
                result.Add(f.Invoke(this[i]));
            }

            return result;
        }

        public Vector Apply(Func<object, object, object> f, object arg1)
        {
            var result = new Vector(Count);

            for (var i = 0; i < Count; ++i) {
                result.Add(f.Invoke(this[i], arg1));
            }

            return result;
        }

        public Vector Apply(ScriptBlock f)
        {
            var result = new Vector(Count);

            for (var i = 0; i < Count; ++i) {
                var va = new List<PSVariable>() { new PSVariable("_") };
                va[0].Value = this[i];
                result.Add(f.InvokeWithContext(null, va, null).Last());
            }

            return result;
        }

        public void ApplyInPlace(Func<object, object> f)
        {
            for (var i = 0; i < Count; ++i) {
                this[i] = f.Invoke(this[i]);
            }
        }

        public void ApplyInPlace(Func<object, object, object> f, object arg1)
        {
            for (var i = 0; i < Count; ++i) {
                this[i] = f.Invoke(this[i], arg1);
            }
        }

        public void ApplyInPlace(ScriptBlock f)
        {
            for (var i = 0; i < Count; ++i) {
                var va = new List<PSVariable>() { new PSVariable("_") };
                va[0].Value = this[i];
                this[i] = f.InvokeWithContext(null, va, null).Last();
            }
        }

        public Vector Apply(Vector b, Func<object, object, object> f)
        {
            if (Count > b.Count) {
                throw new RuntimeException("Vector lengths are not the same");
            }

            var result = new Vector(Count);

            for (var i = 0; i < Count; ++i) {
                result.Add(f.Invoke(this[i], b[i]));
            }

            return result;
        }

        public void ApplyInPlace(Vector b, Func<object, object, object> f)
        {
            if (Count > b.Count) {
                throw new RuntimeException("Vector lengths are not the same");
            }

            for (var i = 0; i < Count; ++i) {
                this[i] = f.Invoke(this[i], b[i]);
            }
        }

        // Scalar arithmetics

        public Vector Plus(double arg1) { return Apply(x => Converter.ToDouble(x) + arg1); }
        public Vector Minus(double arg1) { return Apply(x => Converter.ToDouble(x) - arg1); }
        public Vector Multiply(double arg1) { return Apply(x => Converter.ToDouble(x) * arg1); }
        public Vector Divide(double arg1) { return Apply(x => Converter.ToDouble(x) / arg1); }
        public Vector Minus() { return Apply(x => -Converter.ToDouble(x)); }

        // System.Math functions

        public Vector Abs() { return Apply(x => Math.Abs(Converter.ToDouble(x))); }
        public Vector Acos() { return Apply(x => Math.Acos(Converter.ToDouble(x))); }
        public Vector Asin() { return Apply(x => Math.Asin(Converter.ToDouble(x))); }
        public Vector Atan() { return Apply(x => Math.Atan(Converter.ToDouble(x))); }
        public Vector Atan2(double arg) { return Apply(x => Math.Atan2(Converter.ToDouble(x), arg)); }
        public Vector Ceiling() { return Apply(x => Math.Ceiling(Converter.ToDouble(x))); }
        public Vector Cos() { return Apply(x => Math.Cos(Converter.ToDouble(x))); }
        public Vector Cosh() { return Apply(x => Math.Cosh(Converter.ToDouble(x))); }
        public Vector Exp() { return Apply(x => Math.Exp(Converter.ToDouble(x))); }
        public Vector Floor() { return Apply(x => Math.Floor(Converter.ToDouble(x))); }
        public Vector IEEERemainder(double arg) { return Apply(x => Math.IEEERemainder(Converter.ToDouble(x), arg)); }
        public Vector Log() { return Apply(x => Math.Log(Converter.ToDouble(x))); }
        public Vector Log(double arg) { return Apply(x => Math.Log(Converter.ToDouble(x), arg)); }
        public Vector Log10() { return Apply(x => Math.Log10(Converter.ToDouble(x))); }
        public Vector Max(double arg) { return Apply(x => Math.Max(Converter.ToDouble(x), arg)); }
        public Vector Min(double arg) { return Apply(x => Math.Min(Converter.ToDouble(x), arg)); }
        public Vector Pow(double arg) { return Apply(x => Math.Pow(Converter.ToDouble(x), arg)); }
        public Vector Round() { return Apply(x => Math.Round(Converter.ToDouble(x))); }
        public Vector Round(int arg) { return Apply(x => Math.Round(Converter.ToDouble(x), arg)); }
        public Vector Round(int arg, MidpointRounding arg2) { return Apply(x => Math.Round(Converter.ToDouble(x), arg, arg2)); }
        public Vector Sign() { return Apply(x => Math.Sign(Converter.ToDouble(x))); }
        public Vector Sin() { return Apply(x => Math.Sin(Converter.ToDouble(x))); }
        public Vector Sinh() { return Apply(x => Math.Sinh(Converter.ToDouble(x))); }
        public Vector Sqrt() { return Apply(x => Math.Sqrt(Converter.ToDouble(x))); }
        public Vector Tan() { return Apply(x => Math.Tan(Converter.ToDouble(x))); }
        public Vector Tanh() { return Apply(x => Math.Tanh(Converter.ToDouble(x))); }
        public Vector Truncate() { return Apply(x => Math.Truncate(Converter.ToDouble(x))); }

        // Vector arithmetics

        public Vector Plus(Vector b) { return Apply(b, (x, y) => Converter.ToDouble(x) + Converter.ToDouble(y)); }
        public Vector Minus(Vector b) { return Apply(b, (x, y) => Converter.ToDouble(x) - Converter.ToDouble(y)); }
        public Vector Multiply(Vector b) { return Apply(b, (x, y) => Converter.ToDouble(x) * Converter.ToDouble(y)); }
        public Vector Divide(Vector b) { return Apply(b, (x, y) => Converter.ToDouble(x) / Converter.ToDouble(y)); }

        #endregion

        #region Statistical values

        public double ContraHarmonicMean()
        {
            return Measures.ContraHarmonicMean(ToDoubleArray());
        }

        public double Correlation(Vector b)
        {
            if (this.Count != b.Count) {
                throw new RuntimeException("Vector sizes are not equal");
            }

            var array = new double[][] {
                this.ToDoubleArray(),
                b.ToDoubleArray()
            };

            return Measures.Correlation(array)[0][1];
        }

        public Vector CumulativeSum()
        {
            return Vector.Create(ToDoubleArray().CumulativeSum());
        }

        public double Entropy()
        {
            return Measures.Entropy(ToDoubleArray());
        }

        public DataFrame DiscreteHistogram()
        {
            var hist = new Dictionary<object, int>();

            foreach (var value in this) {
                if (hist.ContainsKey(value)) {
                    ++hist[value];
                }
                else {
                    hist[value] = 1;
                }
            }

            var labels = new Vector(hist.Keys.ToArray());
            labels.Sort();

            var counts = new Vector(labels.Count);
            foreach (var label in labels) {
                counts.Add(hist[label]);
            }

            var df = new DataFrame();
            df.DefineNewColumn("Label", labels);
            df.DefineNewColumn("Count", counts);

            return df;
        }

        public DataFrame Histogram(double binWidth = double.NaN, double offset = 0.0)
        {
            if (double.IsNaN(binWidth)) {
                var binCount = Math.Min(100, Math.Ceiling(Math.Sqrt(Count)));
                binWidth = (Max() - Min()) / binCount;
            }

            var hist = new Dictionary<double, int>();

            foreach (var value in this) {
                var bin = Math.Floor((Converter.ToDouble(value) - offset) / binWidth);

                if (hist.ContainsKey(bin)) {
                    ++hist[bin];
                }
                else {
                    hist[bin] = 1;
                }
            }

            var labels = hist.Keys.ToArray();
            labels.Sort();

            var counts = new Vector(labels.Length);
            foreach (var label in labels) {
                counts.Add(hist[label]);
            }

            var df = new DataFrame();
            df.DefineNewColumn("Label", new Vector(labels.Apply(x => x * binWidth)));
            df.DefineNewColumn("Count", counts);

            return df;
        }

        public ContinuousHistogram HistogramStatistics(double min, double max)
        {
            return new ContinuousHistogram(ToIntArray(), new Range((float)min, (float)max));
        }

        public double GeometricMean()
        {
            return Measures.GeometricMean(ToDoubleArray());
        }

        public double Kurtosis(bool unbiased = true)
        {
            return Measures.Kurtosis(ToDoubleArray(), unbiased);
        }

        public double LogGeometricMean()
        {
            return Measures.LogGeometricMean(ToDoubleArray());
        }

        public double LowerQuartile()
        {
            return Measures.LowerQuartile(ToDoubleArray());
        }

        public double Max()
        {
            return ToDoubleArray().Max();
        }

        public double Mean()
        {
            return ToDoubleArray().Mean();
        }

        public double Median()
        {
            return ToDoubleArray().Median();
        }

        public double Min()
        {
            return ToDoubleArray().Min();
        }

        public double Mode()
        {
            return ToDoubleArray().Mode();
        }

        public DataFrame MovingStatistics(int windowSize)
        {
            var ms = new MovingNormalStatistics(windowSize);

            var count = new Vector(Count);
            var max = new Vector(Count);
            var min = new Vector(Count);
            var mean = new Vector(Count);
            var variance = new Vector(Count);
            var stdDev = new Vector(Count);
            var sum = new Vector(Count);

            for (var i = 0; i < Count; ++i) {
                ms.Push(Converter.ToDouble(this[i]));
                count.Add(ms.Count);
                max.Add(ms.Max());
                min.Add(ms.Min());
                mean.Add(ms.Mean);
                variance.Add(ms.Variance);
                stdDev.Add(ms.StandardDeviation);
                sum.Add(ms.Sum);
            }

            var df = new DataFrame();
            df.DefineNewColumn("Value", new Vector(this));
            df.DefineNewColumn("Count", count);
            df.DefineNewColumn("Max", max);
            df.DefineNewColumn("Min", min);
            df.DefineNewColumn("Mean", mean);
            df.DefineNewColumn("Variance", variance);
            df.DefineNewColumn("StdDev", stdDev);
            df.DefineNewColumn("Sum", sum);

            return df;
        }

        public double Quantile(
            double probabilities,
            bool alreadySorted = false,
            QuantileMethod type = QuantileMethod.Default)
        {
            // 'inPlace' should be false becuase ToDoubleArray() can be cached.
            return ToDoubleArray().Quantile(probabilities, alreadySorted, type, false);
        }

        public double Skewness(bool unbiased = true)
        {
            return Measures.Skewness(ToDoubleArray(), unbiased);
        }

        public double StandardDeviation(bool unbiased = true)
        {
            return ToDoubleArray().StandardDeviation(unbiased);
        }

        public double StdDev(bool unbiased = true)
        {
            return StandardDeviation(unbiased);
        }

        public double StandardError()
        {
            return Measures.StandardError(ToDoubleArray());
        }

        public double Sum()
        {
            return ToDoubleArray().Sum();
        }

        public PSObject Summary()
        {
            var values = ToDoubleArray();
            values.Sort();

            var result = new PSObject();
            var props = result.Properties;

            props.Add(new PSNoteProperty("Count", values.Length));
            props.Add(new PSNoteProperty("Minimum", values[0]));
            props.Add(new PSNoteProperty("1stQuantile", values.Quantile(.25, true)));
            props.Add(new PSNoteProperty("Mean", values.Mean()));
            props.Add(new PSNoteProperty("Median", values.Median()));
            props.Add(new PSNoteProperty("Mode", values.Mode()));
            props.Add(new PSNoteProperty("3rdQuantile", values.Quantile(.75, true)));
            props.Add(new PSNoteProperty("Maximum", values[values.Length - 1]));
            props.Add(new PSNoteProperty("StdDev (pop, unbiased)", new double[2] { values.StandardDeviation(false), values.StandardDeviation(true) }));
            props.Add(new PSNoteProperty("Variance (pop, unbiased)", new double[2] { values.Variance(false), values.Variance(true) }));
            props.Add(new PSNoteProperty("Skewness (pop, unbiased)", new double[2] { values.Skewness(false), values.Skewness(true) }));
            props.Add(new PSNoteProperty("Kurtosis (pop, unbiased)", new double[2] { values.Kurtosis(false), values.Kurtosis(true) }));

            return result;
        }

        public double TruncatedMean(double percent, bool alreadySorted = false)
        {
            return Measures.TruncatedMean(ToDoubleArray(), percent, false, alreadySorted);
        }

        public double UpperQuartile()
        {
            return Measures.UpperQuartile(ToDoubleArray());
        }

        public double Variance(bool unbiased = true)
        {
            return ToDoubleArray().Variance(unbiased);
        }

        #endregion

        #region Linear algebra / numerical operations (non-destructive)

        public DataFrame Cartesian(Vector b)
        {
            return DataFrame.Create(ToDoubleArray().Cartesian(b.ToDoubleArray()));
        }

        public DataFrame Cartesian(Vector b, ScriptBlock f)
        {
            var x = new Vector(Count * b.Count);
            var y = new Vector(Count * b.Count);
            var z = new Vector(Count * b.Count);

            var va = new List<PSVariable>() { new PSVariable("a"), new PSVariable("b") };
            for (var i = 0; i < Count; ++i) {
                for (var j = 0; j < b.Count; ++j) {
                    x.Add(this[i]);
                    y.Add(b[j]);

                    va[0].Value = this[i];
                    va[1].Value = b[j];
                    z.Add(f.InvokeWithContext(null, va, null).Last());
                }
            }

            var df = new DataFrame();
            df.DefineNewColumn("a", x);
            df.DefineNewColumn("b", y);
            df.DefineNewColumn("c", z);

            return df;
        }

        public double Dot(Vector b)
        {
            return ToDoubleArray().Dot(b.ToDoubleArray());
        }

        public Vector Dot(DataFrame b)
        {
            return Vector.Create(ToDoubleArray().Dot(b.ToDoubleJaggedArray()));
        }

        public Vector Kronecker(Vector b)
        {
            return new Vector(ToDoubleArray().Kronecker(b.ToDoubleArray()));
        }

        public DataFrame Outer(Vector b)
        {
            return DataFrame.Create(ToDoubleArray().Outer(b.ToDoubleArray()));
        }

        public DataFrame Outer(Vector b, Func<object, object, object> f)
        {
            var df = new DataFrame();

            for (var column = 0; column < b.Count; ++column) {
                var v = new Vector(Count);
                for (var row = 0; row < Count; ++row) {
                    v.Add(f.Invoke(this[row], b[column]));
                }
                df.DefineNewColumn("c" + column, v);
            }

            return df;
        }

        public DataFrame Outer(Vector b, ScriptBlock f)
        {
            var df = new DataFrame();

            var va = new List<PSVariable>() { new PSVariable("a"), new PSVariable("b") };
            for (var column = 0; column < b.Count; ++column) {
                var v = new Vector(Count);
                for (var row = 0; row < Count; ++row) {
                    va[0].Value = this[row];
                    va[1].Value = b[column];
                    v.Add(f.InvokeWithContext(null, va, null).Last());
                }
                df.DefineNewColumn("c" + column, v);
            }

            return df;
        }

        public Vector Cross(Vector b)
        {
            return Vector.Create(ToDoubleArray().Cross(b.ToDoubleArray()));
        }

        public Vector Sample(int size, bool replacement = false)
        {
            // ref. https://en.wikipedia.org/wiki/Fisher%E2%80%93Yates_shuffle

            int count = Count;
            var result = new Vector(size);
            if (replacement) {
                for (var i = 0; i < size; ++i) {
                    var j = Generator.Random.Next(Count);
                    result.Add(this[j]);
                }
            }
            else {
                if (size > count) {
                    throw new RuntimeException("Sample size too large");
                }

                for (var i = 0; i < size; i++) {
                    result.Add(double.NaN);
                    var j = Generator.Random.Next(i + 1);
                    if (j != i) {
                        result[i] = result[j];
                    }
                    result[j] = this[i];
                }
            }

            return result;
        }

        public Vector Scale(double fromMin, double fromMax, double toMin, double toMax)
        {
            return new Vector(ToDoubleArray().Scale(fromMin, fromMax, toMin, toMax));
        }

        public Vector Scale(double toMin, double toMax)
        {
            return new Vector(ToDoubleArray().Scale(toMin, toMax));
        }

        public Vector Softmax()
        {
            return new Vector(Special.Softmax(ToDoubleArray()));
        }

        public Vector ZScore(bool unbiased = true)
        {
            var values = ToDoubleArray();
            var mean = values.Mean();
            var sd = values.StandardDeviation(unbiased);

            for (var i = 0; i < values.Length; ++i) {
                values[i] = (values[i] - mean) / sd;
            }

            return new Vector(values);
        }

        public DataFrame Combinations(int k = 0)
        {
            if (k == 0) {
                k = Count;
            }

            var df = new DataFrame();

            int column = 0;
            foreach (object[] seq in Combinatorics.Combinations(this.ToArray(), k, true)) {
                df.DefineNewColumn("c" + column, new Vector(seq));
                ++column;
            }

            return df;
        }

        public DataFrame Permutations()
        {
            var df = new DataFrame();

            int column = 0;
            foreach (object[] seq in Combinatorics.Permutations(this.ToArray(), true)) {
                df.DefineNewColumn("c" + column, new Vector(seq));
                ++column;
            }

            return df;
        }

        public DataFrame Subsets(int k = 0)
        {
            if (k == 0) {
                k = Count;
            }

            var df = new DataFrame();

            int column = 0;
            foreach (SortedSet<object> seq in Combinatorics.Subsets(new HashSet<object>(this), k, true)) {
                df.DefineNewColumn("c" + column, new Vector(seq));
                ++column;
            }

            return df;
        }

        #endregion

        #region Linear algebra / numerical operations (destructive)

        #endregion

    }
}
