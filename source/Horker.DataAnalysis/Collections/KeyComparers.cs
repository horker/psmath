using System.Collections.Generic;

namespace Horker.DataAnalysis
{
    public class ObjectKeyComparer : IEqualityComparer<object>
    {
        public static bool Compare(object o1, object o2)
        {
            if (o1 == null && o2 == null) {
                return true;
            }
            else if (o1 == null || o2 == null) {
                return false;
            }

            if (o1 is string && o2 is string) {
                return (o1 as string).ToLower() == (o2 as string).ToLower();
            }

            return o1 == o2;
        }

        public new bool Equals(object o1, object o2)
        {
            return Compare(o1, o2);
        }

        public int GetHashCode(object o)
        {
            if (o is string) {
                return (o as string).ToLower().GetHashCode();
            }
            return o.GetHashCode();
        }
    }

    public class StringKeyComparer : IEqualityComparer<string>
    {
        public static bool Compare(string s1, string s2)
        {
            if (s1 == null && s2 == null) {
                return true;
            }
            else if (s1 == null || s2 == null) {
                return false;
            }

            if (s1.ToLower() == s2.ToLower()) {
                return true;
            }

            return false;
        }

        public bool Equals(string s1, string s2)
        {
            return Compare(s1, s2);
        }

        public int GetHashCode(string s)
        {
            return s.ToLower().GetHashCode();
        }
    }
}
