using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using Accord.Statistics;

namespace Horker.DataAnalysis.ArrayMethods
{
    public class ToolsMethods
    {
        public static double[] Center(
            PSObject values
        )
        {
            var array = Converter.ToDoubleArray(values);
            return Tools.Center(array, null);
        }

        public static double[] Rank(
            PSObject samples,
            bool alreadySorted = false,
            bool adjustForTies = true
        )
        {
            var array = Converter.ToDoubleArray(samples);
            bool hasTies;
            return Tools.Rank(array, out hasTies, alreadySorted, adjustForTies);
        }

        public static double[] Standardize(
            PSObject values,
            bool inPlace = false
        )
        {
            var array = Converter.ToDoubleArray(values);
            return Tools.Standardize(array, inPlace);
        }

        public static int[] Ties(
            PSObject ranks
        )
        {
            var array = Converter.ToDoubleArray(ranks);
            return Tools.Ties(array);
        }
    }
}
