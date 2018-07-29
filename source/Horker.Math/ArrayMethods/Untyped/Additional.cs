using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using Accord.Math;
using Accord.Math.Random;

namespace Horker.Math.ArrayMethods.Untyped
{
    public class HistogramBin
    {
        public double Lower;
        public double Upper;
        public int Count;
    }

    public class Additional
    {
        public static object[] DropNa(PSObject values)
        {
            var array = Helper.GetObjectArray(values);

            var result = new List<object>();
            foreach (var value in array)
            {
                if (value == null)
                    continue;
                if (value is double && double.IsNaN((double)value))
                    continue;
                if (value is float && float.IsNaN((float)value))
                    continue;

                result.Add(value);
            }
            return result.ToArray();
        }

        public static double[] DropNaN(PSObject values)
        {
            var array = Converter.ToDoubleArray(values);
            return array.RemoveAll(double.NaN);
        }

        public static HistogramBin[] HistogramInternal(double[] values, double binWidth = double.NaN, double offset = 0.0)
        {
            var min = values.Min();
            var max = values.Max();

            if (double.IsNaN(binWidth)) {
                var binCount = System.Math.Min(100, System.Math.Ceiling(System.Math.Sqrt(values.Length)));
                binWidth = (max - min) / binCount;
            }

            int minBar = (int)System.Math.Floor((min - offset) / binWidth);
            int maxBar = (int)System.Math.Floor((max - offset) / binWidth);

            var hist = new int[maxBar - minBar + 1];

            foreach (var value in values) {
                var bin = (int)System.Math.Floor((value - offset) / binWidth) - minBar;
                ++hist[bin];
            }

            var result = new HistogramBin[maxBar - minBar + 1];
            for (var i = minBar; i <= maxBar; ++i) {
                var bin = new HistogramBin();
                bin.Lower = i * binWidth + offset;
                bin.Upper = (i + 1) * binWidth + offset;
                bin.Count = hist[i - minBar];
                result[i - minBar] = bin;
            }

            return result;
        }

        public static HistogramBin[] Histogram(PSObject values, double binWidth = double.NaN, double offset = 0.0)
        {
            var array = Converter.ToDoubleArray(values);
            return HistogramInternal(array, binWidth, offset);
        }
    }
}
