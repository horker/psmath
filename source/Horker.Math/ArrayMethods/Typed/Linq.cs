using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;

namespace Horker.Math.ArrayMethods.Typed
{
    public class Linq<T>
    {
        public static T Aggregate(PSObject source, Func<T, T, T> func)
        {
            var array = (T[])source.BaseObject;
            return array.Aggregate(func);
        }

        public static bool All(PSObject source, Func<T, bool> func)
        {
            var array = (T[])source.BaseObject;
            return array.All(func);
        }

        public static bool Any(PSObject source, Func<T, bool> func = null)
        {
            var array = (T[])source.BaseObject;

            if (func == null)
                return array.Any();

            return array.Any(func);
        }

        public static IEnumerable<T> AsEnumerable(PSObject source)
        {
            var array = (T[])source.BaseObject;
            return array.AsEnumerable();
        }

        public static T[] Concat(PSObject first, T[] second)
        {
            var array = first.BaseObject as T[];
            return array.Concat(second).ToArray();
        }

        public static bool Contains(PSObject source, T value, IEqualityComparer<T> comparer = null)
        {
            var array = (T[])source.BaseObject;

            if (comparer == null)
                return array.Contains(value);

            return array.Contains(value, comparer);
        }

        /*
        // Conflict with Count property
        public static int Count(PSObject source, Func<T, bool> predicate = null)
        {
            var array = (T[])source.BaseObject;

            if (predicate == null)
                return array.Count();

            return array.Count(predicate);
        }
        */

        public static T[] DefaultIfEmpty(PSObject source, T defaultValue = default(T))
        {
            var array = (T[])source.BaseObject;
            return array.DefaultIfEmpty(defaultValue).ToArray();
        }

        public static T[] Distinct(PSObject source, IEqualityComparer<T> comparer = null)
        {
            var array = (T[])source.BaseObject;

            if (comparer == null)
                return array.Distinct().ToArray();

            return array.Distinct(comparer).ToArray();
        }

        public static T ElementAt(PSObject source, int index)
        {
            var array = (T[])source.BaseObject;
            return array.ElementAt(index);
        }

        public static T ElementAtOrDefault(PSObject source, int index)
        {
            var array = (T[])source.BaseObject;
            return array.ElementAtOrDefault(index);
        }

        // Skip: Empty

        public static T[] Except(PSObject first, T[] second, IEqualityComparer<T> comparer = null)
        {
            var array = first.BaseObject as T[];

            if (comparer == null)
                return array.Except(second).ToArray();

            return array.Except(second, comparer).ToArray();
        }

        public static T First(PSObject source, Func<T, bool> predicate = null)
        {
            var array = (T[])source.BaseObject;

            if (predicate == null)
                return array.First();

            return array.First(predicate);
        }

        public static T FirstOrDefault(PSObject source, Func<T, bool> predicate = null)
        {
            var array = (T[])source.BaseObject;

            if (predicate == null)
                return array.First();

            return array.FirstOrDefault(predicate);
        }

        /*
        TODO
        public static T[] GroupBy(
            PSObject source,
            Func<T, T> keySelector,
            Func<T, T> elementSelector = null,
            Func<T, IEnumerable<T>, T> resultSelector = null,
            IEqualityComparer<T> comparer = null
        )
        {
            var array = (T[])source.BaseObject;
            IEnumerable<T> result;

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
        */

        public static T[] GroupJoin(
            PSObject outer,
            T[] inner,
            Func<T, T> outerKeySelector,
            Func<T, T> innerKeySelector,
            Func<T, IEnumerable<T>, T> resultSelector,
            IEqualityComparer<T> comparer = null
        )
        {
            var array = outer.BaseObject as T[];

            if (comparer == null)
                return array.GroupJoin(inner, outerKeySelector, innerKeySelector, resultSelector).ToArray();

            return array.GroupJoin(inner, outerKeySelector, innerKeySelector, resultSelector, comparer).ToArray();
        }

        public static T[] Intersect(PSObject first, T[] second, IEqualityComparer<T> comparer = null)
        {
            var array = first.BaseObject as T[];

            if (comparer == null)
                return array.Intersect(second).ToArray();

            return array.Intersect(second, comparer).ToArray();
        }

        public static T[] Join(
            PSObject outer,
            T[] inner,
            Func<T, T> outerKeySelector,
            Func<T, T> innerKeySelector,
            Func<T, T, T> resultSelector,
            IEqualityComparer<T> comparer = null
        )
        {
            var array = outer.BaseObject as T[];

            if (comparer == null)
                return array.Join(inner, outerKeySelector, innerKeySelector, resultSelector).ToArray();

            return array.Join(inner, outerKeySelector, innerKeySelector, resultSelector, comparer).ToArray();
        }

        public static T Last(PSObject source, Func<T, bool> predicate = null)
        {
            var array = (T[])source.BaseObject;

            if (predicate == null)
                return array.Last();

            return array.Last(predicate);
        }

        public static T LastOrDefault(PSObject source, Func<T, bool> predicate = null)
        {
            var array = (T[])source.BaseObject;

            if (predicate == null)
                return array.LastOrDefault();

            return array.LastOrDefault(predicate);
        }

        public static long LongCount(PSObject source, Func<T, bool> predicate = null)
        {
            var array = (T[])source.BaseObject;

            if (predicate == null)
                return array.LongCount();

            return array.LongCount(predicate);
        }

        public static double Max(PSObject source, Func<double, int> selector = null)
        {
            var array = (T[])source.BaseObject;

            if (selector == null)
                return array.Select(x => Converter.ToDouble(x)).Max();

            return array.Select(x => Converter.ToDouble(x)).Max(selector);
        }

        public static double Min(PSObject source, Func<double, int> selector = null)
        {
            var array = (T[])source.BaseObject;

            if (selector == null)
                return array.Select(x => Converter.ToDouble(x)).Min();

            return array.Select(x => Converter.ToDouble(x)).Min(selector);
        }

        // Skip: OfType

        public static T[] OrderBy(PSObject source, Func<T, T> keySelector, IComparer<T> comparer = null)
        {
            var array = (T[])source.BaseObject;

            if (comparer == null)
                return array.OrderBy(keySelector).ToArray();

            return array.OrderBy(keySelector, comparer).ToArray();
        }

        public static T[] OrderByDescending(PSObject source, Func<T, T> keySelector, IComparer<T> comparer = null)
        {
            var array = (T[])source.BaseObject;

            if (comparer == null)
                return array.OrderByDescending(keySelector).ToArray();

            return array.OrderByDescending(keySelector, comparer).ToArray();
        }

        // Skip: Range, Repeat

        public static T[] Reverse(PSObject source)
        {
            var array = (T[])source.BaseObject;
            return array.Reverse().ToArray();
        }

        public static T[] Select(PSObject source, Func<T, int, T> selector)
        {
            var array = (T[])source.BaseObject;
            return array.Select(selector).ToArray();
        }

        public static T[] SelectMany(
            PSObject source,
            Func<T, int, T[]> collectionSelector,
            Func<T, T, T> resultSelector = null
        )
        {
            var array = (T[])source.BaseObject;

            if (resultSelector == null)
                return array.SelectMany(collectionSelector).ToArray();

            return array.SelectMany(collectionSelector, resultSelector).ToArray();
        }

        // TODO
        // ToDictionary
        // ToLookup

        public static bool SequenceEqual(PSObject first, T[] second, IEqualityComparer<T> comparer = null)
        {
            var array = first.BaseObject as T[];

            if (comparer == null)
                return array.SequenceEqual(second);

            return array.SequenceEqual(second, comparer);
        }

        public static T Single(PSObject source, Func<T, bool> predicate = null)
        {
            var array = (T[])source.BaseObject;

            if (predicate == null)
                return array.Single();

            return array.Single(predicate);
        }

        public static T SingleOrDefault(PSObject source, Func<T, bool> predicate = null)
        {
            var array = (T[])source.BaseObject;

            if (predicate == null)
                return array.SingleOrDefault();

            return array.SingleOrDefault(predicate);
        }

        public static T[] Skip(PSObject source, int count)
        {
            var array = (T[])source.BaseObject;
            return array.Skip(count).ToArray();
        }

        public static T[] SkipWhile(PSObject source, Func<T, int, bool> predicate)
        {
            var array = (T[])source.BaseObject;
            return array.SkipWhile(predicate).ToArray();
        }

        public static double Sum(PSObject source, Func<double, int> selector = null)
        {
            var array = Converter.ToDoubleArray(source);

            if (selector == null)
                return array.Sum();

            return array.Sum(selector);
        }

        public static T[] Take(PSObject source, int count)
        {
            var array = (T[])source.BaseObject;
            return array.Take(count).ToArray();
        }

        public static T[] TakeWhile(PSObject source, Func<T, int, bool> predicate)
        {
            var array = (T[])source.BaseObject;
            return array.TakeWhile(predicate).ToArray();
        }

        // Skip: ThenBy
        // Skip: ThenByDescending

        public static Dictionary<T, T> ToDictionary(
            PSObject source,
            Func<T, T> keySelector,
            Func<T, T> elementSelector = null,
            IEqualityComparer<T> comparer = null
        )
        {
            var array = (T[])source.BaseObject;

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

        public static List<T> ToList(PSObject source)
        {
            var array = (T[])source.BaseObject;
            return array.ToList();
        }

        public static ILookup<T, T> ToLookup(
            PSObject source,
            Func<T, T> keySelector,
            Func<T, T> elementSelector,
            IEqualityComparer<T> comparer = null
        )
        {
            var array = (T[])source.BaseObject;

            if (comparer == null)
                return array.ToLookup(keySelector, elementSelector);

            return array.ToLookup(keySelector, elementSelector, comparer);
        }

        public static T[] Union(PSObject first, T[] second, IEqualityComparer<T> comparer = null)
        {
            var array = first.BaseObject as T[];

            if (comparer == null)
                return array.Union(second).ToArray();

            return array.Union(second, comparer).ToArray();
        }

        public static T[] Where(PSObject source, Func<T, int, bool> predicate)
        {
            var array = (T[])source.BaseObject;
            return array.Where(predicate).ToArray();
        }

        public static T[] Zip(PSObject first, T[] second, Func<T, T, T> resultSelector)
        {
            var array = first.BaseObject as T[];
            return array.Zip(second, resultSelector).ToArray();
        }
    }
}

