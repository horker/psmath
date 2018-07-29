using System.Collections.Generic;
using Accord.Math;
using Accord.Statistics;

namespace Horker.Math
{
    public class DataSummary
    {
        public string Name;
        public int Count;
        // public int Unique;
        public double Minimum;
        public double LowerQuantile;
        public double Median;
        public double Mean;
        public double UpperQuantile;
        public double Maximum;

        public DataSummary(IList<double> values, string name = null)
        {
            var sorted = values.Sorted();

            Name = name;
            Count = sorted.Length;
            // Unique = sorted.DistinctCount(); // too time-consuming
            Minimum = sorted[0];
            LowerQuantile = sorted.Quantile(.25, true);
            Median = sorted.Median(true);
            Mean = sorted.Mean();
            UpperQuantile = sorted.Quantile(.75, true);
            Maximum = sorted[sorted.Length - 1];
        }
    }
}
