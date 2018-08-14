using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using Accord.Math;
using Accord.Math.Random;

namespace Horker.Math.ArrayMethods.Typed
{
    public class Matrix<T>
    {
        public static void Clear(
            PSObject values
        )
        {
            var array = (T[])values.BaseObject;
            Accord.Math.Matrix.Clear(array);
        }

        public static T[] Concatenate(
            PSObject a,
            T[] b
        )
        {
            var array = (T[])a.BaseObject;
            return Accord.Math.Matrix.Concatenate(b);
        }

        public static T[] Copy(
            PSObject vector
        )
        {
            var array = (T[])vector.BaseObject;
            var result = new T[array.Length];
            Array.Copy(array, result, array.Length);
            return result;
        }

        public static int DistinctCount(
            PSObject values
        )
        {
            var array = (T[])values.BaseObject;
            return Accord.Math.Matrix.DistinctCount(array);
        }

        public static T[] Expand(
            PSObject vector,
            int[] count
        )
        {
            var array = (T[])vector.BaseObject;
            return Accord.Math.Matrix.Expand(array, count);
        }

        public static int[] Find(
            PSObject data,
            Func<T, bool> func,
            bool firstOnly = false
        )
        {
            var array = (T[])data.BaseObject;
            return Accord.Math.Matrix.Find(array, func, firstOnly);
        }

        public static T[] First(
            PSObject values,
            int count
        )
        {
            var array = (T[])values.BaseObject;
            return Accord.Math.Matrix.First(array, count);
        }

        public static T[] Get(
            PSObject source,
            int startRow,
            int endRow
        )
        {
            var array = (T[])source.BaseObject;
            return Accord.Math.Matrix.Get(array, startRow, endRow);
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
            var array = Converter.ToDoubleArray(values);
            return Accord.Math.Matrix.IsSorted(array, direction);
        }

        public static Matrix OneHot(PSObject values, int? columns = null)
        {
            var array = (T[])values.BaseObject;

            if (columns.HasValue)
                return Accord.Math.Matrix.OneHot<double>(array.Apply(x => Convert.ToInt32(x)), columns.Value);
            else
                return Accord.Math.Matrix.OneHot<double>(array.Apply(x => Convert.ToInt32(x)));
        }

        public static void Swap(
            PSObject values,
            int a,
            int b
        )
        {
            var array = (T[])values.BaseObject;
            Accord.Math.Matrix.Swap(array, a, b);
        }
    }
}