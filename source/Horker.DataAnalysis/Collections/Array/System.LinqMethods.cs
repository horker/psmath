using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;

namespace Horker.DataAnalysis.ArrayMethods
{
    public class LinqMethods
    {
        public static object Aggregate(PSObject source, Func<object, object, object> func)
        {
            var array = source.BaseObject as object[];
            return array.Aggregate(func);
        }

        public static bool All(PSObject source, Func<object, bool> func)
        {
            var array = source.BaseObject as object[];
            return array.All(func);
        }

        public static bool Any(PSObject source, Func<object, bool> func = null)
        {
            var array = source.BaseObject as object[];

            if (func == null)
                return array.Any();

            return array.Any(func);
        }

        public static IEnumerable<object> AsEnumerable(PSObject source)
        {
            var array = source.BaseObject as object[];
            return array.AsEnumerable();
        }

        public static IEnumerable<object> Concat(PSObject first, IEnumerable<object> second)
        {
            var array = first.BaseObject as object[];
            return array.Concat(second);
        }

        public static bool Contains(PSObject source, object value, IEqualityComparer<object> comparer = null)
        {
            var array = source.BaseObject as object[];

            if (comparer == null)
                return array.Contains(value);

            return array.Contains(value, comparer);
        }

        public static int Count(PSObject source, Func<object, bool> predicate = null)
        {
            var array = source.BaseObject as object[];

            if (predicate == null)
                return array.Count();

            return array.Count(predicate);
        }

        public static IEnumerable<object> DefaultIfEmpty(PSObject source, object defaultValue = null)
        {
            var array = source.BaseObject as object[];
            return array.DefaultIfEmpty(defaultValue);
        }

        public static IEnumerable<object> Distinct(PSObject source, IEqualityComparer<object> comparer = null)
        {
            var array = source.BaseObject as object[];

            if (comparer == null)
                return array.Distinct();

            return array.Distinct(comparer);
        }

        public static object ElementAt(PSObject source, int index)
        {
            var array = source.BaseObject as object[];
            return array.ElementAt(index);
        }

        public static object ElementAtOrDefault(PSObject source, int index)
        {
            var array = source.BaseObject as object[];
            return array.ElementAtOrDefault(index);
        }

        // Skip: Empty

        public static IEnumerable<object> Except(PSObject first, IEnumerable<object> second, IEqualityComparer<object> comparer = null)
        {
            var array = first.BaseObject as object[];

            if (comparer == null)
                return array.Except(second);

            return array.Except(second, comparer);
        }

        public static object First(PSObject source, Func<object, bool> predicate = null)
        {
            var array = source.BaseObject as object[];

            if (predicate == null)
                return array.First();

            return array.First(predicate);
        }

        public static object FirstOrDefault(PSObject source, Func<object, bool> predicate = null)
        {
            var array = source.BaseObject as object[];

            if (predicate == null)
                return array.First();

            return array.FirstOrDefault(predicate);
        }

        public static IEnumerable<object> GroupBy(
            PSObject source,
            Func<object, object> keySelector,
            Func<object, object> elementSelector = null,
            Func<object, IEnumerable<object>, object> resultSelector = null,
            IEqualityComparer<object> comparer = null
        )
        {
            var array = source.BaseObject as object[];

            if (comparer == null)
            {
                if (resultSelector == null)
                {
                    if (elementSelector == null)
                    {
                        return array.GroupBy(keySelector);
                    }
                    else
                    {
                        return array.GroupBy(keySelector, elementSelector);
                    }
                }
                else
                {
                    if (elementSelector == null)
                    {
                        return array.GroupBy(keySelector, resultSelector);
                    }
                    else
                    {
                        return array.GroupBy(keySelector, elementSelector, resultSelector);
                    }
                }
            }
            else
            {
                if (resultSelector == null)
                {
                    if (elementSelector == null)
                    {
                        return array.GroupBy(keySelector, comparer);
                    }
                    else
                    {
                        return array.GroupBy(keySelector, elementSelector, comparer);
                    }
                }
                else
                {
                    if (elementSelector == null)
                    {
                        return array.GroupBy(keySelector, resultSelector, comparer);
                    }
                    else
                    {
                        return array.GroupBy(keySelector, elementSelector, resultSelector, comparer);
                    }
                }
            }
        }

        public static IEnumerable<object> GroupJoin(
            PSObject outer,
            IEnumerable<object> inner,
            Func<object, object> outerKeySelector,
            Func<object, object> innerKeySelector,
            Func<object, IEnumerable<object>, object> resultSelector,
            IEqualityComparer<object> comparer = null
        )
        {
            var array = outer.BaseObject as object[];

            if (comparer == null)
                return array.GroupJoin(inner, outerKeySelector, innerKeySelector, resultSelector);

            return array.GroupJoin(inner, outerKeySelector, innerKeySelector, resultSelector, comparer);
        }

        public static IEnumerable<object> Intersect(PSObject first, IEnumerable<object> second, IEqualityComparer<object> comparer = null)
        {
            var array = first.BaseObject as object[];

            if (comparer == null)
                return array.Intersect(second);

            return array.Intersect(second, comparer);
        }

        public static IEnumerable<object> Join(
            PSObject outer,
            IEnumerable<object> inner,
            Func<object, object> outerKeySelector,
            Func<object, object> innerKeySelector,
            Func<object, object, object> resultSelector,
            IEqualityComparer<object> comparer = null
        )
        {
            var array = outer.BaseObject as object[];

            if (comparer == null)
                return array.Join(inner, outerKeySelector, innerKeySelector, resultSelector);

            return array.Join(inner, outerKeySelector, innerKeySelector, resultSelector, comparer);
        }

        public static object Last(PSObject source, Func<object, bool> predicate = null)
        {
            var array = source.BaseObject as object[];

            if (predicate == null)
                return array.Last();

            return array.Last(predicate);
        }

        public static object LastOrDefault(PSObject source, Func<object, bool> predicate = null)
        {
            var array = source.BaseObject as object[];

            if (predicate == null)
                return array.LastOrDefault();

            return array.LastOrDefault(predicate);
        }

        public static long LongCount(PSObject source, Func<object, bool> predicate = null)
        {
            var array = source.BaseObject as object[];

            if (predicate == null)
                return array.LongCount();

            return array.LongCount(predicate);
        }

        public static double Max(PSObject source, Func<double, int> selector = null)
        {
            var array = source.BaseObject as object[];

            if (selector == null)
                return array.Select(x => Converter.ToDouble(x)).Max();

            return array.Select(x => Converter.ToDouble(x)).Max(selector);
        }

        public static double Min(PSObject source, Func<double, int> selector = null)
        {
            var array = source.BaseObject as object[];

            if (selector == null)
                return array.Select(x => Converter.ToDouble(x)).Min();

            return array.Select(x => Converter.ToDouble(x)).Min(selector);
        }

        // Skip: OfType

        public static IOrderedEnumerable<object> OrderBy(PSObject source, Func<object, object> keySelector, IComparer<object> comparer = null)
        {
            var array = source.BaseObject as object[];

            if (comparer == null)
                return array.OrderBy(keySelector);

            return array.OrderBy(keySelector, comparer);
        }

        public static IOrderedEnumerable<object> OrderByDescending(PSObject source, Func<object, object> keySelector, IComparer<object> comparer = null)
        {
            var array = source.BaseObject as object[];

            if (comparer == null)
                return array.OrderByDescending(keySelector);

            return array.OrderByDescending(keySelector, comparer);
        }

        // Skip: Range, Repeat

        public static IEnumerable<object> Reverse(PSObject source)
        {
            var array = source.BaseObject as object[];
            return array.Reverse();
        }

        public static IEnumerable<object> Select(PSObject source, Func<object, int, object> selector)
        {
            var array = source.BaseObject as object[];
            return array.Select(selector);
        }

        public static IEnumerable<object> SelectMany(
            PSObject source,
            Func<object, int, IEnumerable<object>> collectionSelector,
            Func<object, object, object> resultSelector = null
        )
        {
            var array = source.BaseObject as object[];

            if (resultSelector == null)
                return array.SelectMany(collectionSelector);

            return array.SelectMany(collectionSelector, resultSelector);
        }

        // TODO
        // ToDictionary
        // ToLookup

        public static bool SequenceEqual(PSObject first, IEnumerable<object> second, IEqualityComparer<object> comparer = null)
        {
            var array = first.BaseObject as object[];

            if (comparer == null)
                return array.SequenceEqual(second);

            return array.SequenceEqual(second, comparer);
        }

        public static object Single(PSObject source, Func<object, bool> predicate = null)
        {
            var array = source.BaseObject as object[];

            if (predicate == null)
                return array.Single();

            return array.Single(predicate);
        }

        public static object SingleOrDefault(PSObject source, Func<object, bool> predicate = null)
        {
            var array = source.BaseObject as object[];

            if (predicate == null)
                return array.SingleOrDefault();

            return array.SingleOrDefault(predicate);
        }

        public static IEnumerable<object> Skip(PSObject source, int count)
        {
            var array = source.BaseObject as object[];
            return array.Skip(count);
        }

        public static IEnumerable<object> SkipWhile(PSObject source, Func<object, int, bool> predicate)
        {
            var array = source.BaseObject as object[];
            return array.SkipWhile(predicate);
        }

        public static double Sum(PSObject source, Func<double, int> selector = null)
        {
            var array = source.BaseObject as object[];

            if (selector == null)
                return array.Select(x => Converter.ToDouble(x)).Sum();

            return array.Select(x => Converter.ToDouble(x)).Sum(selector);
        }

        public static IEnumerable<object> Take(PSObject source, int count)
        {
            var array = source.BaseObject as object[];
            return array.Take(count);
        }

        public static IEnumerable<object> TakeWhile(PSObject source, Func<object, int, bool> predicate)
        {
            var array = source.BaseObject as object[];
            return array.TakeWhile(predicate);
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
            var array = source.BaseObject as object[];

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
            var array = source.BaseObject as object[];
            return array.ToList();
        }

        public static ILookup<object, object> ToLookup(
            PSObject source,
            Func<object, object> keySelector,
            Func<object, object> elementSelector,
            IEqualityComparer<object> comparer = null
        )
        {
            var array = source.BaseObject as object[];

            if (comparer == null)
                return array.ToLookup(keySelector, elementSelector);

            return array.ToLookup(keySelector, elementSelector, comparer);
        }

        public static IEnumerable<object> Union(PSObject first, IEnumerable<object> second, IEqualityComparer<object> comparer = null)
        {
            var array = first.BaseObject as object[];

            if (comparer == null)
                return array.Union(second);

            return array.Union(second, comparer);
        }

        public static IEnumerable<object> Where(PSObject source, Func<object, int, bool> predicate)
        {
            var array = source.BaseObject as object[];
            return array.Where(predicate);
        }

        public static IEnumerable<object> Zip(PSObject first, IEnumerable<object> second, Func<object, object, object> resultSelector)
        {
            var array = first.BaseObject as object[];
            return array.Zip(second, resultSelector);
        }
    }
}

