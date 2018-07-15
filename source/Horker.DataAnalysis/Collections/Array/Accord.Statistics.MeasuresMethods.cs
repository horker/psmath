using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using Accord.Statistics;

namespace Horker.DataAnalysis.ArrayMethods
{
    internal class Helper
    {
        internal static double[] GetDoubleArray(PSObject values, bool ensureCopy = false)
        {
            var array = values.BaseObject;

            if (array is double[])
            {
                if (ensureCopy)
                {
                    var source = array as double[];
                    var result = new double[source.Length];
                    Array.Copy(source, result, source.Length);
                    return result;
                }
                return array as double[];
            }

            return ((object[])array).Select(x => Converter.ToDouble(x)).ToArray();
        }
    }

    public class MeasuresMethods
    {
        public static double ContraHarmonicMean(PSObject values, int order = 1)
        {
            var array = Helper.GetDoubleArray(values);
            return Measures.ContraHarmonicMean(array, order);
        }

        public static double Entropy(PSObject values)
        {
            var array = Helper.GetDoubleArray(values);
            return Measures.Entropy(array);
        }

        public static double ExponentialWeightedMean(
            PSObject values,
            int window,
            double alpha = 0
        )
        {
            var array = Helper.GetDoubleArray(values);
            return Measures.ExponentialWeightedMean(array, window, alpha);
        }

        public static double ExponentialWeightedVariance(
            PSObject values,
            int window,
            double alpha = 0,
            bool unbiased = false
        )
        {
            var array = Helper.GetDoubleArray(values);
            return Measures.ExponentialWeightedVariance(array, window, alpha, unbiased);
        }


        public static double GeometricMean(
            PSObject values
        )
        {
            var array = Helper.GetDoubleArray(values);
            return Measures.GeometricMean(array);
        }

        public static double Kurtosis(
            PSObject values,
            bool unbiased = true
        )
        {
            var array = Helper.GetDoubleArray(values);
            return Measures.Kurtosis(array, unbiased);
        }

        public static double LogGeometricMean(
            PSObject values
        )
        {
            var array = Helper.GetDoubleArray(values);
            return Measures.LogGeometricMean(array);
        }

        public static double LowerQuartile(
            PSObject values,
            bool alreadySorted = false,
            QuantileMethod type = QuantileMethod.Default,
            bool inPlace = false
        )
        {
            var array = Helper.GetDoubleArray(values);
            return Measures.LowerQuartile(array, alreadySorted, type, inPlace);
        }

        public static double Mean(
            PSObject values
        )
        {
            var array = Helper.GetDoubleArray(values);
            return Measures.Mean(array);
        }

        public static double Median(
            PSObject values,
            bool alreadySorted = false,
            QuantileMethod type = QuantileMethod.Default,
            bool inPlace = false
        )
        {
            var array = Helper.GetDoubleArray(values);
            return Measures.Median(array, alreadySorted, type, inPlace);
        }

        public static double Mode(
            PSObject values,
            bool inPlace = false,
    bool alreadySorted = false
)
        {
            var array = Helper.GetDoubleArray(values);
            return Measures.Mode(array, inPlace, alreadySorted);
        }

        public static double Quantile(
            PSObject values,
            double probabilities,
            bool alreadySorted = false,
            QuantileMethod type = QuantileMethod.Default,
            bool inPlace = false
        )
        {
            var array = Helper.GetDoubleArray(values);
            return Measures.Quantile(array, probabilities, alreadySorted, type, inPlace);
        }

        public static double[] Quantiles(
            PSObject values,
            double[] probabilities,
            bool alreadySorted = false,
            QuantileMethod type = QuantileMethod.Default,
            bool inPlace = false
        )
        {
            var array = Helper.GetDoubleArray(values);
            return Measures.Quantiles(array, probabilities, alreadySorted, type, inPlace);
        }

        public static double Quartiles(
            PSObject values,
            out double q1,
            out double q3,
            bool alreadySorted = false,
            QuantileMethod type = QuantileMethod.Default,
            bool inPlace = false
        )
        {
            var array = Helper.GetDoubleArray(values);
            return Measures.Quartiles(array, out q1, out q3, alreadySorted, type, inPlace);
        }

        public static double Skewness(
            PSObject values,
            bool unbiased = true
        )
        {
            var array = Helper.GetDoubleArray(values);
            return Measures.Skewness(array, unbiased);
        }

        public static double StandardDeviation(
            PSObject values,
            bool unbiased = true
        )
        {
            var array = Helper.GetDoubleArray(values);
            return Measures.StandardDeviation(array, unbiased);
        }

        public static double StandardError(
            PSObject values
        )
        {
            var array = Helper.GetDoubleArray(values);
            return Measures.StandardError(array);
        }

        public static double TruncatedMean(
            PSObject values,
            double percent,
            bool inPlace = false,
            bool alreadySorted = false
        )
        {
            var array = Helper.GetDoubleArray(values);
            return Measures.TruncatedMean(array, percent, inPlace, alreadySorted);
        }

        public static double UpperQuartile(
            PSObject values,
            bool alreadySorted = false,
            QuantileMethod type = QuantileMethod.Default,
            bool inPlace = false
        )
        {
            var array = Helper.GetDoubleArray(values);
            return Measures.UpperQuartile(array, alreadySorted, type, false);
        }

        public static double Variance(
            PSObject values,
            bool unbiased
        )
        {
            var array = Helper.GetDoubleArray(values);
            return Measures.Variance(array, unbiased);
        }
    }
}
