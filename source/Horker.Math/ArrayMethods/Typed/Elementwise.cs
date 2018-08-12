using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using Accord.Math;
using Accord.Math.Random;

namespace Horker.Math.ArrayMethods.Typed
{
    public class ElementwiseObject
    {
        public static Double[] ElementAdd(PSObject values, Double other)
        { return Converter.ToDoubleArray(values).Select(x => x + other).ToArray(); }

        public static Double[] ElementSubtract(PSObject values, Double other)
        { return Converter.ToDoubleArray(values).Select(x => x - other).ToArray(); }

        public static Double[] ElementMultiply(PSObject values, Double other)
        { return Converter.ToDoubleArray(values).Select(x => x * other).ToArray(); }

        public static Double[] ElementDivide(PSObject values, Double other)
        { return Converter.ToDoubleArray(values).Select(x => x / other).ToArray(); }

        public static Double[] ElementReminder(PSObject values, Double other)
        { return Converter.ToDoubleArray(values).Select(x => x % other).ToArray(); }

        public static Double[] ElementNegate(PSObject values)
        { return Converter.ToDoubleArray(values).Select(x => -x).ToArray(); }
    }

    public class ElementwiseDouble
    {
        public static Double[] ElementAdd(PSObject values, Double other)
        { return (values.BaseObject as Double[]).Select(x => x + other).ToArray(); }

        public static Double[] ElementSubtract(PSObject values, Double other)
        { return (values.BaseObject as Double[]).Select(x => x - other).ToArray(); }

        public static Double[] ElementMultiply(PSObject values, Double other)
        { return (values.BaseObject as Double[]).Select(x => x * other).ToArray(); }

        public static Double[] ElementDivide(PSObject values, Double other)
        { return (values.BaseObject as Double[]).Select(x => x / other).ToArray(); }

        public static Double[] ElementReminder(PSObject values, Double other)
        { return (values.BaseObject as Double[]).Select(x => x % other).ToArray(); }

        public static Double[] ElementNegate(PSObject values)
        { return (values.BaseObject as Double[]).Select(x => -x).ToArray(); }
    }

    public class ElementwiseSingle
    {
        public static Single[] ElementAdd(PSObject values, Single other)
        { return (values.BaseObject as Single[]).Select(x => x + other).ToArray(); }

        public static Single[] ElementSubtract(PSObject values, Single other)
        { return (values.BaseObject as Single[]).Select(x => x - other).ToArray(); }

        public static Single[] ElementMultiply(PSObject values, Single other)
        { return (values.BaseObject as Single[]).Select(x => x * other).ToArray(); }

        public static Single[] ElementDivide(PSObject values, Single other)
        { return (values.BaseObject as Single[]).Select(x => x / other).ToArray(); }

        public static Single[] ElementReminder(PSObject values, Single other)
        { return (values.BaseObject as Single[]).Select(x => x % other).ToArray(); }

        public static Single[] ElementNegate(PSObject values)
        { return (values.BaseObject as Single[]).Select(x => -x).ToArray(); }
    }

    public class ElementwiseInt64
    {
        public static Int64[] ElementAdd(PSObject values, Int64 other)
        { return (values.BaseObject as Int64[]).Select(x => x + other).ToArray(); }

        public static Int64[] ElementSubtract(PSObject values, Int64 other)
        { return (values.BaseObject as Int64[]).Select(x => x - other).ToArray(); }

        public static Int64[] ElementMultiply(PSObject values, Int64 other)
        { return (values.BaseObject as Int64[]).Select(x => x * other).ToArray(); }

        public static Int64[] ElementDivide(PSObject values, Int64 other)
        { return (values.BaseObject as Int64[]).Select(x => x / other).ToArray(); }

        public static Int64[] ElementReminder(PSObject values, Int64 other)
        { return (values.BaseObject as Int64[]).Select(x => x % other).ToArray(); }

        public static Int64[] ElementNegate(PSObject values)
        { return (values.BaseObject as Int64[]).Select(x => -x).ToArray(); }
    }

    public class ElementwiseInt32
    {
        public static Int32[] ElementAdd(PSObject values, Int32 other)
        { return (values.BaseObject as Int32[]).Select(x => x + other).ToArray(); }

        public static Int32[] ElementSubtract(PSObject values, Int32 other)
        { return (values.BaseObject as Int32[]).Select(x => x - other).ToArray(); }

        public static Int32[] ElementMultiply(PSObject values, Int32 other)
        { return (values.BaseObject as Int32[]).Select(x => x * other).ToArray(); }

        public static Int32[] ElementDivide(PSObject values, Int32 other)
        { return (values.BaseObject as Int32[]).Select(x => x / other).ToArray(); }

        public static Int32[] ElementReminder(PSObject values, Int32 other)
        { return (values.BaseObject as Int32[]).Select(x => x % other).ToArray(); }

        public static Int32[] ElementNegate(PSObject values)
        { return (values.BaseObject as Int32[]).Select(x => -x).ToArray(); }
    }

    public class ElementwiseInt16
    {
        public static Int16[] ElementAdd(PSObject values, Int16 other)
        { return (values.BaseObject as Int16[]).Select(x => (Int16)(x + other)).ToArray(); }

        public static Int16[] ElementSubtract(PSObject values, Int16 other)
        { return (values.BaseObject as Int16[]).Select(x => (Int16)(x - other)).ToArray(); }

        public static Int16[] ElementMultiply(PSObject values, Int16 other)
        { return (values.BaseObject as Int16[]).Select(x => (Int16)(x * other)).ToArray(); }

        public static Int16[] ElementDivide(PSObject values, Int16 other)
        { return (values.BaseObject as Int16[]).Select(x => (Int16)(x / other)).ToArray(); }

        public static Int16[] ElementReminder(PSObject values, Int16 other)
        { return (values.BaseObject as Int16[]).Select(x => (Int16)(x % other)).ToArray(); }

        public static Int16[] ElementNegate(PSObject values)
        { return (values.BaseObject as Int16[]).Select(x => (Int16)(-x)).ToArray(); }
    }

    public class ElementwiseByte
    {
        public static Byte[] ElementAdd(PSObject values, Byte other)
        { return (values.BaseObject as Byte[]).Select(x => (Byte)(x + other)).ToArray(); }

        public static Byte[] ElementSubtract(PSObject values, Byte other)
        { return (values.BaseObject as Byte[]).Select(x => (Byte)(x - other)).ToArray(); }

        public static Byte[] ElementMultiply(PSObject values, Byte other)
        { return (values.BaseObject as Byte[]).Select(x => (Byte)(x * other)).ToArray(); }

        public static Byte[] ElementDivide(PSObject values, Byte other)
        { return (values.BaseObject as Byte[]).Select(x => (Byte)(x / other)).ToArray(); }

        public static Byte[] ElementReminder(PSObject values, Byte other)
        { return (values.BaseObject as Byte[]).Select(x => (Byte)(x % other)).ToArray(); }

        public static Byte[] ElementNegate(PSObject values)
        { return (values.BaseObject as Byte[]).Select(x => (Byte)(-x)).ToArray(); }
    }

    public class ElementwiseSByte
    {
        public static SByte[] ElementAdd(PSObject values, SByte other)
        { return (values.BaseObject as SByte[]).Select(x => (SByte)(x + other)).ToArray(); }

        public static SByte[] ElementSubtract(PSObject values, SByte other)
        { return (values.BaseObject as SByte[]).Select(x => (SByte)(x - other)).ToArray(); }

        public static SByte[] ElementMultiply(PSObject values, SByte other)
        { return (values.BaseObject as SByte[]).Select(x => (SByte)(x * other)).ToArray(); }

        public static SByte[] ElementDivide(PSObject values, SByte other)
        { return (values.BaseObject as SByte[]).Select(x => (SByte)(x / other)).ToArray(); }

        public static SByte[] ElementReminder(PSObject values, SByte other)
        { return (values.BaseObject as SByte[]).Select(x => (SByte)(x % other)).ToArray(); }

        public static SByte[] ElementNegate(PSObject values)
        { return (values.BaseObject as SByte[]).Select(x => (SByte)(-x)).ToArray(); }
    }
}
