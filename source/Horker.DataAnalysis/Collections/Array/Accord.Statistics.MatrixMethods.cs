using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using Accord.Math;
using Accord.Math.Random;

namespace Horker.DataAnalysis.ArrayMethods
{
    public class MatrixMethods
    {
        public static int ArgMin(
            PSObject values
        )
        {
            var array = Helper.GetDoubleArray(values);
            return Accord.Math.Matrix.ArgMin(array);
        }

        public static int[] ArgSort(
            PSObject values
        )
        {
            var array = Helper.GetDoubleArray(values);
            return Accord.Math.Matrix.ArgSort(array);
        }

        public static int[] Bottom(
            PSObject values,
            int count
        )
        {
            var array = Helper.GetDoubleArray(values);
            return Accord.Math.Matrix.Bottom(array, count);
        }

        public static Matrix Cartesian(
            PSObject sequence1,
            PSObject sequence2
        )
        {
            var s1 = Helper.GetDoubleArray(sequence1);
            var s2 = Helper.GetDoubleArray(sequence2);
            return Matrix.Create(Accord.Math.Matrix.Cartesian(s1, s2));
        }

        public static void Clear(
            PSObject values
        )
        {
            var array = values.BaseObject as object[];
            Accord.Math.Matrix.Clear(array);
        }

        public static double[] Concatenate(
            PSObject a,
            params double[] b
        )
        {
            var array = Helper.GetDoubleArray(a);
            return Accord.Math.Matrix.Concatenate(b);
        }

        public static object[] Copy(
            PSObject vector
        )
        {
            var array = vector.BaseObject as object[];
            var result = new object[array.Length];
            Array.Copy(array, result, array.Length);
            return result;
        }

        public static double[] Cross(
            PSObject a,
            PSObject b
        )
        {
            var a1 = Helper.GetDoubleArray(a);
            var a2 = Helper.GetDoubleArray(b);
            return Accord.Math.Matrix.Cross(a1, a2);
        }

        public static double[] CumulativeSum(
            PSObject vector
        )
        {
            var array = Helper.GetDoubleArray(vector);
            return Accord.Math.Matrix.CumulativeSum(array);
        }

        /*
        public static double[] Distinct(
            PSObject values,
            bool allowNulls = true
        )
        {
            var array = Helper.GetDoubleArray(values);
            return Accord.Math.Matrix.Distinct(array, allowNulls);
        }
        */

        public static int DistinctCount(
            PSObject values
        )
        {
            var array = Helper.GetDoubleArray(values);
            return Accord.Math.Matrix.DistinctCount(array);
        }

        public static double[] Expand(
            PSObject vector,
            int[] count
        )
        {
            var array = Helper.GetDoubleArray(vector);
            return Accord.Math.Matrix.Expand(array, count);
        }

        public static int[] Find(
            PSObject data,
            Func<double, bool> func,
            bool firstOnly = false
        )
        {
            var array = Helper.GetDoubleArray(data);
            return Accord.Math.Matrix.Find(array, func, firstOnly);
        }

        public static double[] First(
            PSObject values,
            int count
        )
        {
            var array = Helper.GetDoubleArray(values);
            return Accord.Math.Matrix.First(array, count);
        }

        public static double[] Get(
            PSObject source,
            int startRow,
            int endRow
        )
        {
            var array = Helper.GetDoubleArray(source);
            return Accord.Math.Matrix.Get(array, startRow, endRow);
        }

        public static bool HasInfinity(
            PSObject matrix
        )
        {
            var array = Helper.GetDoubleArray(matrix);
            return Accord.Math.Matrix.HasInfinity(array);
        }


        public static bool HasNaN(
            PSObject matrix
        )
        {
            var array = Helper.GetDoubleArray(matrix);
            return Accord.Math.Matrix.HasNaN(array);
        }

        public static bool IsEqual(
            PSObject a,
            PSObject b
        )
        {
            return Accord.Math.Matrix.IsEqual(a, b);;
        }

        public static bool IsSorted(
            PSObject values,
            Accord.Math.Comparers.ComparerDirection direction = Accord.Math.Comparers.ComparerDirection.Ascending
        )
        {
            var array = Helper.GetDoubleArray(values);
            return Accord.Math.Matrix.IsSorted(array, direction);
        }


        public static double[] Kronecker(
            PSObject a,
            PSObject b
        )
        {
            var a1  = Helper.GetDoubleArray(a);
            var a2  = Helper.GetDoubleArray(b);
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
            var array = Helper.GetDoubleArray(vector);
            return Accord.Math.Matrix.Normalize(array);
        }

        public static double[][] Null(
            PSObject vector
        )
        {
            var array = Helper.GetDoubleArray(vector);
            return Accord.Math.Matrix.Null(array);
        }

        public static Matrix Outer(
            PSObject a,
            PSObject b
        )
        {
            var a1 = Helper.GetDoubleArray(a);
            var a2 = Helper.GetDoubleArray(b);
            return new Matrix(Accord.Math.Matrix.Outer(a1, a2), true);
        }

        public static double Product(
            PSObject vector
        )
        {
            var array = Helper.GetDoubleArray(vector);
            return Accord.Math.Matrix.Product(array);
        }

        public static double[][] Split(
            PSObject vector,
            int size
        )
        {
            var array = Helper.GetDoubleArray(vector);
            return Accord.Math.Matrix.Split(array, size);
        }


        public static Matrix Stack(
            PSObject a,
            PSObject b
        )
        {
            var a1 = Helper.GetDoubleArray(a);
            var a2 = Helper.GetDoubleArray(b);
            return Accord.Math.Matrix.Stack(a1, a2);
        }

        /*
        public static double[][] Separate(
            PSObject values,
            int[] labels,
            int groups
        )
        {
            var array = Helper.GetDoubleArray(values);
            return Accord.Math.Matrix.Separate(array, labels, groups);
        }
        */

        public static void Swap(
            PSObject values,
            int a,
            int b
        )
        {
            var array = Helper.GetDoubleArray(values);
            Accord.Math.Matrix.Swap(array, a, b);
        }

        public static int[] Top(
            PSObject values,
            int count
        )
        {
            var array = Helper.GetDoubleArray(values);
            return Accord.Math.Matrix.Top(array, count);
        }

        public static double[] Trim(
            PSObject values
        )
        {
            var array = Helper.GetDoubleArray(values);
            return (double[])Accord.Math.Matrix.Trim(array);
        }

    }
}