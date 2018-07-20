using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using Accord.Math;
using Accord.Math.Random;

namespace Horker.Math.ArrayMethods
{
    public class AdditionalMethods
    {
        public static object[] DropNa(PSObject values)
        {
            var array = Helper.GetObjectArray(values);

            var result = new List<object>();
            foreach (var value in array)
            {
                if (value == null)
                    continue;
                if (value is double && double.IsNaN((double)value))
                    continue;
                if (value is float && float.IsNaN((float)value))
                    continue;

                result.Add(value);
            }
            return result.ToArray();
        }

        public static double[] DropNaN(PSObject values)
        {
            var array = Converter.ToDoubleArray(values);
            return array.RemoveAll(double.NaN);
        }

    }
}
