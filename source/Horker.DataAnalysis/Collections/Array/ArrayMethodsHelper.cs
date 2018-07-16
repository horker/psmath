using System;
using System.Linq;
using System.Management.Automation;

namespace Horker.DataAnalysis.ArrayMethods
{
    internal class Helper
    {
        internal static double[] GetDoubleArray(PSObject values)
        {
            var array = values.BaseObject;

            if (array is double[])
                return array as double[];
            else if (array is object[])
                return (array as object[]).Select(x => Converter.ToDouble(x)).ToArray();
            else if (array is float[])
                return (array as float[]).Select(x => (double)x).ToArray();
            else if (array is Int64[])
                return (array as Int64[]).Select(x => (double)x).ToArray();
            else if (array is Int32[])
                return (array as Int32[]).Select(x => (double)x).ToArray();
            else if (array is Int16[])
                return (array as Int16[]).Select(x => (double)x).ToArray();
            else if (array is Byte[])
                return (array as Byte[]).Select(x => (double)x).ToArray();
            else if (array is SByte[])
                return (array as SByte[]).Select(x => (double)x).ToArray();

            throw new ArgumentException("Failed to convert to double[]");
        }

        internal static object[] GetObjectArray(PSObject values)
        {
            var array = values.BaseObject;
            if (array is object[])
                return array as object[];
            else if (array is double[])
                return (array as double[]).Select(x => (object)x).ToArray();
            else if (array is float[])
                return (array as float[]).Select(x => (object)x).ToArray();
            else if (array is Int64[])
                return (array as Int64[]).Select(x => (object)x).ToArray();
            else if (array is Int32[])
                return (array as Int32[]).Select(x => (object)x).ToArray();
            else if (array is Int16[])
                return (array as Int16[]).Select(x => (object)x).ToArray();
            else if (array is Byte[])
                return (array as Byte[]).Select(x => (object)x).ToArray();
            else if (array is SByte[])
                return (array as SByte[]).Select(x => (object)x).ToArray();

            throw new ArgumentException("Failed to convert to object[]");
        }
    }
}
