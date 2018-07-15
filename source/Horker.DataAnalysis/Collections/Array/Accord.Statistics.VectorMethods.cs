using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using Accord.Math;
using Accord.Math.Random;

namespace Horker.DataAnalysis
{
    public class VectorMethods
    {
        public static object[] Sample(PSObject values, int sampleSize)
        {
            var array = (values.BaseObject) as object[];
            var populationSize = array.Length;

            var samples = Accord.Math.Vector.Sample(sampleSize, populationSize);

            var result = new object[sampleSize];
            for (var i = 0; i < sampleSize; ++i)
            {
                result[i] = array[samples[i]];
            }

            return result;
        }

        public static double[] Scale(
           PSObject values,
           double fromMin,
           double fromMax,
           double toMin,
           double toMax
        )
        {
            var array = Helper.GetDoubleArray(values);
            return Accord.Math.Vector.Scale(array, fromMin, fromMax, toMin, toMax, null);
        }

        public static double[] Shuffled(PSObject values)
        {
            var array = Helper.GetDoubleArray(values);
            return Accord.Math.Vector.Shuffled(array);
        }

        public static double[] Sorted(PSObject values, bool stable = false, bool asc = true)
        {
            var array = Helper.GetDoubleArray(values);
            return Accord.Math.Vector.Sorted(array);
        }

    }
}
