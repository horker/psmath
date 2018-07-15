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
            var array = Helper.GetDoubleArray(values);
            return Tools.Center(array, null);
        }

        public static double[] Rank(
            PSObject samples,
            bool alreadySorted = false,
            bool adjustForTies = true
        )
        {
            var array = Helper.GetDoubleArray(samples);
            bool hasTies;
            return Tools.Rank(array, out hasTies, alreadySorted, adjustForTies);
        }

        public static double[] Standardize(
            PSObject values,
            bool inPlace = false
        )
        {
            var array = Helper.GetDoubleArray(values);
            return Tools.Standardize(array, inPlace);
        }

        public static int[] Ties(
            PSObject ranks
        )
        {
            var array = Helper.GetDoubleArray(ranks);
            return Tools.Ties(array);
        }
    }
}
