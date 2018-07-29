using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using Accord.Math;
using Accord.Math.Random;

namespace Horker.Math.ArrayMethods.Typed
{
    public class Additional<T>
    {
        public static T[] ShuffleInternal(IReadOnlyList<T> values)
        {
            // ref. https://en.wikipedia.org/wiki/Fisher-Yates_shuffle

            int count = values.Count;
            var result = new T[count];

            for (var i = 0; i < count; ++i) {
                var j = Generator.Random.Next(i + 1);
                if (j != i) {
                    result[i] = result[j];
                }
                result[j] = values[i];
            }

            return result;
        }

        public static T[] Shuffle(PSObject values)
        {
            var array = (T[])values.BaseObject;

            return ShuffleInternal(array);
        }

        public static T[][] SplitInternal(IReadOnlyList<T> values, object[] rates)
        {
            var ra = rates.Select(x => Converter.ToDouble(x));
            var results = new List<T[]>();

            int total = values.Count;

            int start = 0;
            foreach (var r in ra)
            {
                var size = (int)System.Math.Round(r >= 1 ? r : total * r);
                if (size > total - start)
                    size = total - start;

                var result = new T[size];
                for (var i = 0; i < size; ++i)
                    result[i] = values[start + i];
                results.Add(result);

                start += size;
                if (start >= total)
                    break;
            }

            if (start < total)
            {
                var size = total - start;
                var result = new T[size];
                for (var i = 0; i < size; ++i)
                    result[i] = values[start + i];
                results.Add(result);
            }

            return results.ToArray();
        }

        public static T[][] Split(PSObject values, object rate0, object rate1 = null, object rate2 = null, object rate3 = null, object rate4 = null, object rate5 = null, object rate6 = null, object rate7 = null, object rate8 = null, object rate9 = null)
        {
            var array = (T[])values.BaseObject;

            if (rate1 == null)
                return SplitInternal(array, new object[] { rate0 });
            if (rate2 == null)
                return SplitInternal(array, new object[] { rate0, rate1 });
            if (rate3 == null)
                return SplitInternal(array, new object[] { rate0, rate1, rate2 });
            if (rate4 == null)
                return SplitInternal(array, new object[] { rate0, rate1, rate2, rate3 });
            if (rate5 == null)
                return SplitInternal(array, new object[] { rate0, rate1, rate2, rate3, rate4 });
            if (rate6 == null)
                return SplitInternal(array, new object[] { rate0, rate1, rate2, rate3, rate4, rate5 });
            if (rate7 == null)
                return SplitInternal(array, new object[] { rate0, rate1, rate2, rate3, rate4, rate5, rate6 });
            if (rate8 == null)
                return SplitInternal(array, new object[] { rate0, rate1, rate2, rate3, rate4, rate5, rate6, rate7 });
            if (rate9 == null)
                return SplitInternal(array, new object[] { rate0, rate1, rate2, rate3, rate4, rate5, rate6, rate7, rate8 });
            return SplitInternal(array, new object[] { rate0, rate1, rate2, rate3, rate4, rate5, rate6, rate7, rate8, rate9 });
        }

        public static T[] SliceInternal(T[] array, object[] ranges)
        {
            // Python style slicing
            // (the second argument's 0 indicates the last index)

            var length = array.Length;

            var result = new List<T>();

            foreach (var range in ranges)
            {
                var r = range;
                if (r is PSObject)
                    r = (range as PSObject).BaseObject;

                var indexes = Converter.ToIntArray(r);

                switch (indexes.Length)
                {
                    case 1:
                        // a single value
                        indexes[0] = indexes[0] < 0 ? length + indexes[0] : indexes[0];
                        result.Add(array[indexes[0]]);
                        break;

                    case 2:
                        // (from, to)
                        indexes[0] = indexes[0] < 0 ? length + indexes[0] : indexes[0];
                        indexes[1] = indexes[1] <= 0 ? length + indexes[1] : indexes[1];
                        result.AddRange(new ArraySegment<T>(array, indexes[0], indexes[1] - indexes[0]));
                        break;

                    case 3:
                        // (from, to, step)
                        if (indexes[2] <= 0)
                            throw new ArgumentException("Third argument (step) must be more than zero");
                        indexes[0] = indexes[0] < 0 ? length + indexes[0] : indexes[0];
                        indexes[1] = indexes[1] <= 0 ? length + indexes[1] : indexes[1];
                        for (var i = indexes[0]; i < indexes[1]; i += indexes[2])
                            result.Add(array[i]);
                        break;

                    default:
                        throw new ArgumentException("Array which length is more than three is not supported");
                }
            }

            return result.ToArray();
        }

        public static T[] Slice(PSObject values, object range0, object range1 = null, object range2 = null, object range3 = null, object range4 = null, object range5 = null, object range6 = null, object range7 = null, object range8 = null, object range9 = null)
        {
            var array = (T[])values.BaseObject;

            if (range1 == null)
                return SliceInternal(array, new object[] { range0 });
            if (range2 == null)
                return SliceInternal(array, new object[] { range0, range1 });
            if (range3 == null)
                return SliceInternal(array, new object[] { range0, range1, range2 });
            if (range4 == null)
                return SliceInternal(array, new object[] { range0, range1, range2, range3 });
            if (range5 == null)
                return SliceInternal(array, new object[] { range0, range1, range2, range3, range4 });
            if (range6 == null)
                return SliceInternal(array, new object[] { range0, range1, range2, range3, range4, range5 });
            if (range7 == null)
                return SliceInternal(array, new object[] { range0, range1, range2, range3, range4, range5, range6 });
            if (range8 == null)
                return SliceInternal(array, new object[] { range0, range1, range2, range3, range4, range5, range6, range7 });
            if (range9 == null)
                return SliceInternal(array, new object[] { range0, range1, range2, range3, range4, range5, range6, range7, range8 });
            return SliceInternal(array, new object[] { range0, range1, range2, range3, range4, range5, range6, range7, range8, range9 });
        }
    }
}
