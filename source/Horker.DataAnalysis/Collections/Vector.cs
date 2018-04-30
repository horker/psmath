using System;
using System.Collections.Generic;
using System.Linq;
using Accord.Math;
using Accord.Statistics;
using Accord.Math.Random;
using System.Management.Automation;
using System.Text.RegularExpressions;

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

        // Constructors

        public Vector()
            : base()
        { }

        public Vector(int capacity)
            : base(capacity)
        { }

        public Vector(object[] data)
            : base(data.Length)
        {
            foreach (var d in data) {
                Add(d);
            }
        }

        public Vector(double[] data)
            : base(data.Length)
        {
            foreach (var d in data) {
                Add(d);
            }
        }

        // Factory methods

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

        // Conversions

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

        // Inplace convertions

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

        // Calculating statistical values

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

        public double Entropy()
        {
            return Measures.Entropy(ToDoubleArray());
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

        public Vector Sample(int size, bool replacement = false)
        {
            // ref. https://en.wikipedia.org/wiki/Fisher%E2%80%93Yates_shuffle

            int count = this.Count;
            var result = new Vector(size);
            if (replacement) {
                for (var i = 0; i < size; ++i) {
                    var j = Generator.Random.Next(this.Count);
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

        // Linear algebra / numeric operations (non-destructive)

        public double Dot(Vector b)
        {
            return ToDoubleArray().Dot(b.ToDoubleArray());
        }

        public double Inner(Vector b)
        {
            return Dot(b);
        }

        public Vector Dot(DataFrame b)
        {
            return new Vector(ToDoubleArray().Dot(b.ToDoubleJaggedArray()));
        }

        public Vector Inner(DataFrame b)
        {
            return Dot(b);
        }

        public DataFrame Outer(Vector b)
        {
            return new DataFrame(this.ToDoubleArray().Outer(b.ToDoubleArray()));
        }

        public Vector Cross(Vector b)
        {
            return new Vector(this.ToDoubleArray().Cross(b.ToDoubleArray()));
        }

        // Linear algebra / numeric operations (destructive)

        public void ElementwiseAdd(Vector b)
        {
            if (this.Count != b.Count) {
                throw new RuntimeException("Vector lengths are not the same");
            }

            for (var i = 0; i < this.Count; ++i) {
                this[i] = Converter.ToDouble(this[i]) + Converter.ToDouble(b[i]);
            }
        }

        public void ElementwiseSubtract(Vector b)
        {
            if (this.Count != b.Count) {
                throw new RuntimeException("Vector lengths are not the same");
            }

            for (var i = 0; i < this.Count; ++i) {
                this[i] = Converter.ToDouble(this[i]) - Converter.ToDouble(b[i]);
            }
        }

        public void ElementwiseMultiply(Vector b)
        {
            if (this.Count != b.Count) {
                throw new RuntimeException("Vector lengths are not the same");
            }

            for (var i = 0; i < this.Count; ++i) {
                this[i] = Converter.ToDouble(this[i]) * Converter.ToDouble(b[i]);
            }
        }

        public void ElementwiseDivide(Vector b)
        {
            if (this.Count != b.Count) {
                throw new RuntimeException("Vector lengths are not the same");
            }

            for (var i = 0; i < this.Count; ++i) {
                this[i] = Converter.ToDouble(this[i]) / Converter.ToDouble(b[i]);
            }
        }

        public void CumulativeSum()
        {
            double sum = 0.0;
            for (var i = 0; i < this.Count; ++i) {
                sum += Converter.ToDouble(this[i]);
                this[i] = sum;
            }
        }

        // General data manupilation

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

    }
}