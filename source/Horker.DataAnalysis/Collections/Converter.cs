﻿using System;
using System.Globalization;
using System.Management.Automation;
using System.Text;
using System.Text.RegularExpressions;

namespace Horker.DataAnalysis
{
    public class Converter
    {
        static private Regex _numberRe = new Regex(@"(NaN|(?:[+-]?((?:[\d,]+\.?(?:\d*))|(?:\.\d+))(?:[eE][+-]?\d+)?))");

        // ref.
        // Currency Symbols
        // http://www.unicode.org/chars/PDF/U20A0.pdf
        // Halfwidth and fullwidth form
        // http://www.unicode.org/chars/PDF/UFF00.pdf
        //
        // Note: The backslash is displayed as currency sign in several environments.
        static private Regex _currencyRe = new Regex("[\u20a0-\u20cf\u0024\u00a2\u00a3\u00a4\u00a5\u0192\u058f\u060b\u09f2\u09f3\u0af1\u0bf9\u0e3f\u17db\u2133\u5143\u5186\u5706\u5713\ufdfc\uff04\uffe0\uffe1\uffe5\uffe6\\\\]");

        public static string ExtractNumber(string input)
        {
            var match = _numberRe.Match(input);
            return match.Captures[0].Value.Replace(",", "");
        }

        public static double ToDouble(object input)
        {
            if (input is PSObject) {
                input = (input as PSObject).BaseObject;
            }

            if (input is string) {
                var s = input as string;
                s = _currencyRe.Replace(s, "");
                var success = double.TryParse(s, NumberStyles.Any, NumberFormatInfo.InvariantInfo, out double result);
                if (!success) {
                    return double.NaN;
                }
                return result;
            }
            else {
                try {
                    return Convert.ToDouble(input);
                }
                catch {
                    return double.NaN;
                }
            }
        }

        public static object ToDateTime(object input)
        {
            if (input is PSObject) {
                input = (input as PSObject).BaseObject;
            }

            if (input is DateTime) {
                return input;
            }
            else if (input is DateTimeOffset) {
                return ((DateTimeOffset)input).DateTime;
            }
            else if (input is string) {
                var success = DateTime.TryParse(input as string, out DateTime result);
                if (!success) {
                    return null;
                }
                return result;
            }
            else {
                try {
                    return Convert.ToDateTime(input);
                }
                catch {
                    return null;
                }
            }
        }

        public static object ToDateTime(object input, string format)
        {
            if (input is PSObject) {
                input = (input as PSObject).BaseObject;
            }

            if (input is DateTime) {
                return input;
            }
            else if (input is DateTimeOffset) {
                return ((DateTimeOffset)input).DateTime;
            }
            else {
                try {
                    if (input is string) {
                        return DateTime.ParseExact(input as string, format, CultureInfo.CurrentCulture);
                    }
                    else {
                        return Convert.ToDateTime(input);
                    }
                }
                catch {
                    return null;
                }
            }
        }

        public static object ToDateTimeOffset(object input, bool assumeLocal = true)
        {
            if (input is PSObject) {
                input = (input as PSObject).BaseObject;
            }

            if (input is DateTimeOffset) {
                return input;
            }
            else if (input is DateTime) {
                return new DateTimeOffset((DateTime)input);
            }
            else if (input is string) {
                DateTimeStyles style = DateTimeStyles.AllowWhiteSpaces;
                if (assumeLocal) {
                    style |= DateTimeStyles.AssumeLocal;
                }
                else {
                    style |= DateTimeStyles.AssumeUniversal;
                }

                var success = DateTimeOffset.TryParse(input as string, CultureInfo.CurrentCulture, style, out DateTimeOffset result);
                if (!success) {
                    return null;
                }
                return result;
            }
            else {
                try {
                    return Convert.ToDateTime(input);
                }
                catch {
                    return null;
                }
            }
        }

        public static object ToDateTimeOffset(object input, string format)
        {
            if (input is PSObject) {
                input = (input as PSObject).BaseObject;
            }

            if (input is DateTimeOffset) {
                return input;
            }
            else if (input is DateTime) {
                return new DateTimeOffset((DateTime)input);
            }
            else {
                try {
                    if (input is string) {
                        return DateTimeOffset.ParseExact(input as string, format, CultureInfo.CurrentCulture);
                    }
                    else {
                        return new DateTimeOffset(Convert.ToDateTime(input));
                    }
                }
                catch {
                    return null;
                }
            }
        }

        public static double[] ToDoubleArray(object input)
        {
            if (input is PSObject) {
                input = (input as PSObject).BaseObject;
            }

            if (input is double[]) {
                return input as double[];
            }

            if (input is object[]) {
                var array = input as object[];
                var result = new double[array.Length];
                for (var i = 0; i < array.Length; ++i) {
                    result[i] = ToDouble(array[i]);
                }
                return result;
            }
            else if (input is Vector) {
                return (input as Vector).ToDoubleArray();
            }
            else {
                return (double[])input;
            }
        }

        public static double[][] ToDoubleJaggedArray(object input)
        {
            if (input is PSObject) {
                input = (input as PSObject).BaseObject;
            }

            if (input is double[][]) {
                return input as double[][];
            }

            if (input is object[,]) {
                var inputArray = input as object[,];
                var result = new double[inputArray.GetLength(1)][];
                for (var i = 0; i < inputArray.GetLength(1); ++i) {
                    var subarray = new double[inputArray.GetLength(0)];
                    result[i] = subarray;
                    for (var j = 0; j < inputArray.GetLength(0); ++j) {
                        subarray[i] = ToDouble(inputArray[i, j]);
                    }
                }
                return result;
            }
            else if (input is object[] && (input as object[]).Length > 0 && (input as object[])[0] is object[]) {
                var inputArray = input as object[];
                var result = new double[inputArray.Length][];
                for (var i = 0; i < inputArray.Length; ++i) {
                    if (!(inputArray[i] is object[])) {
                        throw new RuntimeException("Cannot convert an object to a jagged array");
                    }
                    var inputSubarray = inputArray[i] as object[];
                    var subarray = new double[inputSubarray.Length];
                    result[i] = subarray;
                    for (var j = 0; j < inputSubarray.Length; ++j) {
                        subarray[i] = ToDouble(inputSubarray[j]);
                    }
                }
                return result;
            }
            else if (input is DataFrame) {
                return (input as DataFrame).ToDoubleJaggedArray();
            }

            return (double[][])input;
        }

        public static double[,] ToDouble2dArray(object input)
        {
            if (input is PSObject) {
                input = (input as PSObject).BaseObject;
            }

            if (input is double[,]) {
                return input as double[,];
            }

            if (input is object[,]) {
                var inputArray = input as object[,];
                var result = new double[inputArray.GetLength(0), inputArray.GetLength(1)];
                for (var i = 0; i < inputArray.GetLength(0); ++i) {
                    for (var j = 0; i < inputArray.GetLength(1); ++j) {
                        result[i, j] = ToDouble(inputArray[i, j]);
                    }
                }
                return result;
            }
            else if (input is object[] && (input as object[]).Length > 0 && (input as object[])[0] is object[]) {
                var inputArray = input as object[];
                var columnCount = inputArray.Length;
                var rowCount = (inputArray[0] as object[]).Length;
                var result = new double[columnCount, rowCount];
                for (var i = 0; i < columnCount; ++i) {
                    if (!(inputArray[i] is object[])) {
                        throw new RuntimeException("Cannot convert an object to a two-dimension array");
                    }
                    var subarray = inputArray[i] as object[];
                    for (var j = 0; j < subarray.Length; ++j) {
                        result[i, j] = ToDouble(subarray[j]);
                    }
                }
                return result;
            }
            else if (input is DataFrame) {
                return (input as DataFrame).ToDoubleArray();
            }

            return (double[,])input;
        }
    }
}
