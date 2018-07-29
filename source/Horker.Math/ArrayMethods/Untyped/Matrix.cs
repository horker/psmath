using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using Accord.Math;
using Accord.Math.Random;

namespace Horker.Math.ArrayMethods.Untyped
{
    public class Matrix
    {
        public static int ArgMin(
            PSObject values
        )
        {
            var array = Converter.ToDoubleArray(values);
            return Accord.Math.Matrix.ArgMin(array);
        }

        public static int[] ArgSort(
            PSObject values
        )
        {
            var array = Converter.ToDoubleArray(values);
            return Accord.Math.Matrix.ArgSort(array);
        }

        public static int[] Bottom(
            PSObject values,
            int count
        )
        {
            var array = Converter.ToDoubleArray(values);
            return Accord.Math.Matrix.Bottom(array, count);
        }

        public static Horker.Math.Matrix Cartesian(
            PSObject sequence1,
            PSObject sequence2
        )
        {
            var s1 = Converter.ToDoubleArray(sequence1);
            var s2 = Converter.ToDoubleArray(sequence2);
            return Horker.Math.Matrix.Create(Accord.Math.Matrix.Cartesian(s1, s2));
        }

        public static double[] Cross(
            PSObject a,
            PSObject b
        )
        {
            var a1 = Converter.ToDoubleArray(a);
            var a2 = Converter.ToDoubleArray(b);
            return Accord.Math.Matrix.Cross(a1, a2);
        }

        public static double[] CumulativeSum(
            PSObject vector
        )
        {
            var array = Converter.ToDoubleArray(vector);
            return Accord.Math.Matrix.CumulativeSum(array);
        }

        /*
        public static double[] Distinct(
            PSObject values,
            bool allowNulls = true
        )
        {
            var array = Converter.ToDoubleArray(values);
            return Accord.Math.Matrix.Distinct(array, allowNulls);
        }
        */

        public static bool HasInfinity(
            PSObject matrix
        )
        {
            var array = Converter.ToDoubleArray(matrix);
            return Accord.Math.Matrix.HasInfinity(array);
        }


        public static bool HasNaN(
            PSObject matrix
        )
        {
            var array = Converter.ToDoubleArray(matrix);
            return Accord.Math.Matrix.HasNaN(array);
        }

        public static double[] Kronecker(
            PSObject a,
            PSObject b
        )
        {
            var a1  = Converter.ToDoubleArray(a);
            var a2  = Converter.ToDoubleArray(b);
            return Accord.Math.Matrix.Kronecker(a1, a2);
        }

        /*
        public static double[] Merge(
            params PSObject[] vectors
        )
        {
            var jagged = vectors.Select(x => Converter.ToDoubleArray(x));
            return Accord.Math.Matrix.Merge<double>(jagged.ToArray());
        }
        */

        public static double[] Normalize(
            PSObject vector
        )
        {
            var array = Converter.ToDoubleArray(vector);
            return Accord.Math.Matrix.Normalize(array);
        }

        public static double[][] Null(
            PSObject vector
        )
        {
            var array = Converter.ToDoubleArray(vector);
            return Accord.Math.Matrix.Null(array);
        }

        public static Horker.Math.Matrix Outer(
            PSObject a,
            PSObject b
        )
        {
            var a1 = Converter.ToDoubleArray(a);
            var a2 = Converter.ToDoubleArray(b);
            return new Horker.Math.Matrix(Accord.Math.Matrix.Outer(a1, a2), true);
        }

        public static double Product(
            PSObject vector
        )
        {
            var array = Converter.ToDoubleArray(vector);
            return Accord.Math.Matrix.Product(array);
        }

        /*
        public static double[][] Split(
            PSObject vector,
            int size
        )
        {
            var array = Converter.ToDoubleArray(vector);
            return Accord.Math.Matrix.Split(array, size);
        }
        */

        public static Horker.Math.Matrix Stack(
            PSObject a,
            PSObject b
        )
        {
            var a1 = Converter.ToDoubleArray(a);
            var a2 = Converter.ToDoubleArray(b);
            return new Horker.Math.Matrix(Accord.Math.Matrix.Stack(a1, a2), true);
        }

        /*
        public static double[][] Separate(
            PSObject values,
            int[] labels,
            int groups
        )
        {
            var array = Converter.ToDoubleArray(values);
            return Accord.Math.Matrix.Separate(array, labels, groups);
        }
        */

        public static int[] Top(
            PSObject values,
            int count
        )
        {
            var array = Converter.ToDoubleArray(values);
            return Accord.Math.Matrix.Top(array, count);
        }

        public static double[] Trim(
            PSObject values
        )
        {
            var array = Converter.ToDoubleArray(values);
            return (double[])Accord.Math.Matrix.Trim(array);
        }

    }
}