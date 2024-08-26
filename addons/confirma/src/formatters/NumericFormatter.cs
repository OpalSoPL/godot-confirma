using System;
using System.Globalization;
using System.Numerics;
using Confirma.Interfaces;

namespace Confirma.Formatters;

public class NumericFormatter : IFormatter
{
    public string Format(object value)
    {
        return value.GetType() switch
        {
            // Signed integers
            Type t when t == typeof(short) => FormatInteger((short)value),
            Type t when t == typeof(int) => FormatInteger((int)value),
            Type t when t == typeof(long) => FormatInteger((long)value),
            Type t when t == typeof(sbyte) => FormatInteger((sbyte)value),
            Type t when t == typeof(IntPtr) => FormatInteger((IntPtr)value),
            // Unsigned integers
            Type t when t == typeof(ushort) => FormatInteger((ushort)value),
            Type t when t == typeof(uint) => FormatInteger((uint)value),
            Type t when t == typeof(ulong) => FormatInteger((ulong)value),
            Type t when t == typeof(byte) => FormatInteger((byte)value),
            Type t when t == typeof(UIntPtr) => FormatInteger((UIntPtr)value),
            // Floating-point
            Type t when t == typeof(Half) => FormatFloats((Half)value),
            Type t when t == typeof(float) => FormatFloats((float)value),
            Type t when t == typeof(double) => FormatFloats((double)value),
            Type t when t == typeof(decimal) => FormatFloats((decimal)value),
            _ => new DefaultFormatter().Format(value),
        };
    }

    private static string FormatInteger<T>(T number) where T : INumber<T>
    {
        // N0 formats the number with a thousands separator and no decimal places.
        return number.ToString("N0", CultureInfo.InvariantCulture);
    }

    private static string FormatFloats<T>(T number) where T : INumber<T>
    {
        // F2 formats the number with two decimal places.
        return number.ToString("F2", CultureInfo.InvariantCulture);
    }
}
