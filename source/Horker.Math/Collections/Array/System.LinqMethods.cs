using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;

namespace Horker.Math.ArrayMethods
{
    public class LinqMethods
    {
        public static object Aggregate(PSObject source, Func<object, object, object> func)
        {
            var array = Helper.GetObjectArray(source);
            return array.Aggregate(func);
        }

        public static bool All(PSObject source, Func<object, bool> func)
        {
            var array = Helper.GetObjectArray(source);
            return array.All(func);
        }

        public static bool Any(PSObject source, Func<object, bool> func = null)
        {
            var array = Helper.GetObjectArray(source);

            if (func == null)
                return array.Any();

            return array.Any(func);
        }

        public static IEnumerable<object> AsEnumerable(PSObject source)
        {
            var array = Helper.GetObjectArray(source);
            return array.AsEnumerable();
        }

        public static object[] Concat(PSObject first, object[] second)
        {
            var array = first.BaseObject as object[];
            return array.Concat(second).ToArray();
        }

        public static bool Contains(PSObject source, object value, IEqualityComparer<object> comparer = null)
        {
            var array = Helper.GetObjectArray(source);

            if (comparer == null)
                return array.Contains(value);

            return array.Contains(value, comparer);
        }

        /*
        // Conflict with Count property
        public static int Count(PSObject source, Func<object, bool> predicate = null)
        {
            var array = Helper.GetObjectArray(source);

            if (predicate == null)
                return array.Count();

            return array.Count(predicate);
        }
        */

        public static object[] DefaultIfEmpty(PSObject source, object defaultValue = null)
        {
            var array = Helper.GetObjectArray(source);
            return array.DefaultIfEmpty(defaultValue).ToArray();
        }

        public static object[] Distinct(PSObject source, IEqualityComparer<object> comparer = null)
        {
            var array = Helper.GetObjectArray(source);

            if (comparer == null)
                return array.Distinct().ToArray();

            return array.Distinct(comparer).ToArray();
        }

        public static object ElementAt(PSObject source, int index)
        {
            var array = Helper.GetObjectArray(source);
            return array.ElementAt(index);
        }

        public static object ElementAtOrDefault(PSObject source, int index)
        {
            var array = Helper.GetObjectArray(source);
            return array.ElementAtOrDefault(index);
        }

        // Skip: Empty

        public static object[] Except(PSObject first, object[] second, IEqualityComparer<object> comparer = null)
        {
            var array = first.BaseObject as object[];

            if (comparer == null)
                return array.Except(second).ToArray();

            return array.Except(second, comparer).ToArray();
        }

        public static object First(PSObject source, Func<object, bool> predicate = null)
        {
            var array = Helper.GetObjectArray(source);

            if (predicate == null)
                return array.First();

            return array.First(predicate);
        }

        public static object FirstOrDefault(PSObject source, Func<object, bool> predicate = null)
        {
            var array = Helper.GetObjectArray(source);

            if (predicate == null)
                return array.First();

            return array.FirstOrDefault(predicate);
        }

        public static object[] GroupBy(
            PSObject source,
            Func<object, object> keySelector,
            Func<object, object> elementSelector = null,
            Func<object, IEnumerable<object>, object> resultSelector = null,
            IEqualityComparer<object> comparer = null
        )
        {
            var array = Helper.GetObjectArray(source);
            IEnumerable<object> result;

            if (comparer == null)
            {
                if (resultSelector == null)
                {
                    if (elementSelector == null)
                    {
                        result = array.GroupBy(keySelector);
                    }
                    else
                    {
                        result = array.GroupBy(keySelector, elementSelector);
                    }
                }
                else
                {
                    if (elementSelector == null)
                    {
                        result = array.GroupBy(keySelector, resultSelector);
                    }
                    else
                    {
                        result = array.GroupBy(keySelector, elementSelector, resultSelector);
                    }
                }
            }
            else
            {
                if (resultSelector == null)
                {
                    if (elementSelector == null)
                    {
                        result = array.GroupBy(keySelector, comparer);
                    }
                    else
                    {
                        result = array.GroupBy(keySelector, elementSelector, comparer);
                    }
                }
                else
                {
                    if (elementSelector == null)
                    {
                        result = array.GroupBy(keySelector, resultSelector, comparer);
                    }
                    else
                    {
                        result = array.GroupBy(keySelector, elementSelector, resultSelector, comparer);
                    }
                }
            }

            return result.ToArray();
        }

        public static object[] GroupJoin(
            PSObject outer,
            object[] inner,
            Func<object, object> outerKeySelector,
            Func<object, object> innerKeySelector,
            Func<object, IEnumerable<object>, object> resultSelector,
            IEqualityComparer<object> comparer = null
        )
        {
            var array = outer.BaseObject as object[];

            if (comparer == null)
                return array.GroupJoin(inner, outerKeySelector, innerKeySelector, resultSelector).ToArray();

            return array.GroupJoin(inner, outerKeySelector, innerKeySelector, resultSelector, comparer).ToArray();
        }

        public static object[] Intersect(PSObject first, object[] second, IEqualityComparer<object> comparer = null)
        {
            var array = first.BaseObject as object[];

            if (comparer == null)
                return array.Intersect(second).ToArray();

            return array.Intersect(second, comparer).ToArray();
        }

        public static object[] Join(
            PSObject outer,
            object[] inner,
            Func<object, object> outerKeySelector,
            Func<object, object> innerKeySelector,
            Func<object, object, object> resultSelector,
            IEqualityComparer<object> comparer = null
        )
        {
            var array = outer.BaseObject as object[];

            if (comparer == null)
                return array.Join(inner, outerKeySelector, innerKeySelector, resultSelector).ToArray();

            return array.Join(inner, outerKeySelector, innerKeySelector, resultSelector, comparer).ToArray();
        }

        public static object Last(PSObject source, Func<object, bool> predicate = null)
        {
            var array = Helper.GetObjectArray(source);

            if (predicate == null)
                return array.Last();

            return array.Last(predicate);
        }

        public static object LastOrDefault(PSObject source, Func<object, bool> predicate = null)
        {
            var array = Helper.GetObjectArray(source);

            if (predicate == null)
                return array.LastOrDefault();

            return array.LastOrDefault(predicate);
        }

        public static long LongCount(PSObject source, Func<object, bool> predicate = null)
        {
            var array = Helper.GetObjectArray(source);

            if (predicate == null)
                return array.LongCount();

            return array.LongCount(predicate);
        }

        public static double Max(PSObject source, Func<double, int> selector = null)
        {
            var array = Helper.GetObjectArray(source);

            if (selector == null)
                return array.Select(x => Converter.ToDouble(x)).Max();

            return array.Select(x => Converter.ToDouble(x)).Max(selector);
        }

        public static double Min(PSObject source, Func<double, int> selector = null)
        {
            var array = Helper.GetObjectArray(source);

            if (selector == null)
                return array.Select(x => Converter.ToDouble(x)).Min();

            return array.Select(x => Converter.ToDouble(x)).Min(selector);
        }

        // Skip: OfType

        public static object[] OrderBy(PSObject source, Func<object, object> keySelector, IComparer<object> comparer = null)
        {
            var array = Helper.GetObjectArray(source);

            if (comparer == null)
                return array.OrderBy(keySelector).ToArray();

            return array.OrderBy(keySelector, comparer).ToArray();
        }

        public static object[] OrderByDescending(PSObject source, Func<object, object> keySelector, IComparer<object> comparer = null)
        {
            var array = Helper.GetObjectArray(source);

            if (comparer == null)
                return array.OrderByDescending(keySelector).ToArray();

            return array.OrderByDescending(keySelector, comparer).ToArray();
        }

        // Skip: Range, Repeat

        public static object[] Reverse(PSObject source)
        {
            var array = Helper.GetObjectArray(source);
            return array.Reverse().ToArray();
        }

        public static object[] Select(PSObject source, Func<object, int, object> selector)
        {
            var array = Helper.GetObjectArray(source);
            return array.Select(selector).ToArray();
        }

        public static object[] SelectMany(
            PSObject source,
            Func<object, int, object[]> collectionSelector,
            Func<object, object, object> resultSelector = null
        )
        {
            var array = Helper.GetObjectArray(source);

            if (resultSelector == null)
                return array.SelectMany(collectionSelector).ToArray();

            return array.SelectMany(collectionSelector, resultSelector).ToArray();
        }

        // TODO
        // ToDictionary
        // ToLookup

        public static bool SequenceEqual(PSObject first, object[] second, IEqualityComparer<object> comparer = null)
        {
            var array = first.BaseObject as object[];

            if (comparer == null)
                return array.SequenceEqual(second);

            return array.SequenceEqual(second, comparer);
        }

        public static object Single(PSObject source, Func<object, bool> predicate = null)
        {
            var array = Helper.GetObjectArray(source);

            if (predicate == null)
                return array.Single();

            return array.Single(predicate);
        }

        public static object SingleOrDefault(PSObject source, Func<object, bool> predicate = null)
        {
            var array = Helper.GetObjectArray(source);

            if (predicate == null)
                return array.SingleOrDefault();

            return array.SingleOrDefault(predicate);
        }

        public static object[] Skip(PSObject source, int count)
        {
            var array = Helper.GetObjectArray(source);
            return array.Skip(count).ToArray();
        }

        public static object[] SkipWhile(PSObject source, Func<object, int, bool> predicate)
        {
            var array = Helper.GetObjectArray(source);
            return array.SkipWhile(predicate).ToArray();
        }

        public static double Sum(PSObject source, Func<double, int> selector = null)
        {
            var array = Converter.ToDoubleArray(source);

            if (selector == null)
                return array.Sum();

            return array.Sum(selector);
        }

        public static object[] Take(PSObject source, int count)
        {
            var array = Helper.GetObjectArray(source);
            return array.Take(count).ToArray();
        }

        public static object[] TakeWhile(PSObject source, Func<object, int, bool> predicate)
        {
            var array = Helper.GetObjectArray(source);
            return array.TakeWhile(predicate).ToArray();
        }

        // Skip: ThenBy
        // Skip: ThenByDescending

        public static Dictionary<object, object> ToDictionary(
            PSObject source,
            Func<object, object> keySelector,
            Func<object, object> elementSelector = null,
            IEqualityComparer<object> comparer = null
        )
        {
            var array = Helper.GetObjectArray(source);

            if (comparer == null)
            {
                if (elementSelector == null)
                {
                    return array.ToDictionary(keySelector);
                }
                else
                {
                    return array.ToDictionary(keySelector);
                }
            }
            else
            {
                if (elementSelector == null)
                {
                    return array.ToDictionary(keySelector, comparer);
                }
                else
                {
                    return array.ToDictionary(keySelector, elementSelector);
                }
            }
        }

        public static List<object> ToList(PSObject source)
        {
            var array = Helper.GetObjectArray(source);
            return array.ToList();
        }

        public static ILookup<object, object> ToLookup(
            PSObject source,
            Func<object, object> keySelector,
            Func<object, object> elementSelector,
            IEqualityComparer<object> comparer = null
        )
        {
            var array = Helper.GetObjectArray(source);

            if (comparer == null)
                return array.ToLookup(keySelector, elementSelector);

            return array.ToLookup(keySelector, elementSelector, comparer);
        }

        public static object[] Union(PSObject first, object[] second, IEqualityComparer<object> comparer = null)
        {
            var array = first.BaseObject as object[];

            if (comparer == null)
                return array.Union(second).ToArray();

            return array.Union(second, comparer).ToArray();
        }

        public static object[] Where(PSObject source, Func<object, int, bool> predicate)
        {
            var array = Helper.GetObjectArray(source);
            return array.Where(predicate).ToArray();
        }

        public static object[] Zip(PSObject first, object[] second, Func<object, object, object> resultSelector)
        {
            var array = first.BaseObject as object[];
            return array.Zip(second, resultSelector).ToArray();
        }
    }
}

