using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using Accord.Math;
using Accord.Math.Random;

namespace Horker.Math.ArrayMethods
{
    public class Vector
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
           double? toMin = null,
           double? toMax = null
        )
        {
            var array = Converter.ToDoubleArray(values);
            var result = new double[array.Length];

            if (toMin.HasValue && toMax.HasValue)
                return Accord.Math.Vector.Scale(array, fromMin, fromMax, toMin.Value, toMax.Value, result);
            else
                return Accord.Math.Vector.Scale(array, fromMin, fromMax, result);
        }

        /*
        public static double[] Shuffle(PSObject values)
        {
            var array = Converter.ToDoubleArray(values);
            return Accord.Math.Vector.Shuffled(array);
        }
        */

        public static double[] Sort(PSObject values, bool stable = false, bool asc = true)
        {
            var array = Converter.ToDoubleArray(values);
            return Accord.Math.Vector.Sorted(array, stable, asc);
        }

    }
}
