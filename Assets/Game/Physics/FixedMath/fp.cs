using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine;

namespace FixedMath
{
    [Serializable]
    //[StructLayout(LayoutKind.Explicit)]
    [StructLayout(LayoutKind.Sequential)]
    
    public struct Fix : IEquatable<Fix>, IComparable<Fix>
    {
        public const int SIZE = 8;

        public static readonly Fix max = new Fix(long.MaxValue);
        public static readonly Fix min = new Fix(long.MinValue);
        public static readonly Fix usable_max = new Fix(2147483648L);

        internal static Fix Pow2(Fix x)
        {
            return fixmath.Pow2(x);
        }

        internal static Fix Sqrt(Fix p)
        {
            return FixedMath.fixmath.Sqrt(p);
        }

        internal static Fix Atan2(Fix z2, Fix x2)
        {
            return fixmath.Atan2(z2,x2);
        }

        public static readonly Fix usable_min = -usable_max;

        internal static Fix Sin(Fix fix)
        {
            return fixmath.Sin(fix);
        }

        internal static Fix Cos(Fix fix)
        {
            return fixmath.Cos(fix);
        }

        public static readonly Fix _0 = 0;
        public static readonly Fix _1 = 1;
        public static readonly Fix _2 = 2;
        public static readonly Fix _3 = 3;
        public static readonly Fix _4 = 4;
        public static readonly Fix _5 = 5;
        public static readonly Fix _6 = 6;
        public static readonly Fix _7 = 7;
        public static readonly Fix _8 = 8;
        public static readonly Fix _9 = 9;
        public static readonly Fix _10 = 10;
        public static readonly Fix _90 = 90;
        public static readonly Fix _99 = 99;
        public static readonly Fix _100 = 100;
        public static readonly Fix _180 = 180;
        public static readonly Fix _200 = 200;
        public static readonly Fix _1000 = 1000;
        public static readonly Fix _0_01 = _1 / _100;
        public static readonly Fix _0_02 = _0_01 * 2;
        public static readonly Fix _0_03 = _0_01 * 3;
        public static readonly Fix _0_04 = _0_01 * 4;
        public static readonly Fix _0_05 = _0_01 * 5;
        public static readonly Fix _0_10 = _1 / 10;
        public static readonly Fix _0_016 = (_10+_6) / 1000;
        public static readonly Fix _0_20 = _0_10 * 2;
        public static readonly Fix _0_25 = _1 / 4;
        public static readonly Fix _0_33 = _1 / 3;
        public static readonly Fix _0_3 = _3 / 10;
        public static readonly Fix _0_50 = _1 / 2;
        public static readonly Fix _0_66 = (_1 / 3)*2;
        public static readonly Fix _0_75 = _1 - _0_25;
        public static readonly Fix _0_95 = _1 - _0_05;
        public static readonly Fix _0_99 = _1 - _0_01;
        public static readonly Fix _1_01 = _1 + _0_01;
        public static readonly Fix _1_10 = _1 + _0_10;
        public static readonly Fix _1_50 = _1 + _0_50;

        public static readonly Fix minus_one = -1;
        public static readonly Fix pi = new Fix(205887L);
        public static readonly Fix pi2 = pi * 2;
        public static readonly Fix pi_quarter = pi * _0_25;
        public static readonly Fix pi_half = pi * _0_50;
        public static readonly Fix one_div_pi2 = 1 / pi2;
        public static readonly Fix deg2rad = new Fix(1143L);
        public static readonly Fix rad2deg = new Fix(3754936L);
        public static readonly Fix epsilon = new Fix(1);
        public static readonly Fix e = new Fix(178145L);

        //[FieldOffset(0)]
        public long value;

        public long AsLong => value >> fixlut.PRECISION;
        public int AsInt => (int)(value >> fixlut.PRECISION);
        public float AsFloat => value / 65536f;
        public float AsFloatRounded => (float)Math.Round(value / 65536f, 5);
        public double AsDouble => value / 65536d;
        public double AsDoubleRounded => Math.Round(value / 65536d, 5);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal Fix(long v)
        {
            value = v;
        }

        internal Fix(float v)
        {
            value = (long)(v* 65536);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Fix operator -(Fix a)
        {
            a.value = -a.value;
            return a;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Fix operator +(Fix a)
        {
            a.value = +a.value;
            return a;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Fix operator +(Fix a, Fix b)
        {
            a.value += b.value;
            return a;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Fix operator +(Fix a, int b)
        {
            a.value += (long)b << fixlut.PRECISION;
            return a;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Fix operator +(int a, Fix b)
        {
            b.value = ((long)a << fixlut.PRECISION) + b.value;
            return b;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Fix operator -(Fix a, Fix b)
        {
            a.value -= b.value;
            return a;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Fix operator -(Fix a, int b)
        {
            a.value -= (long)b << fixlut.PRECISION;
            return a;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Fix operator -(int a, Fix b)
        {
            b.value = ((long)a << fixlut.PRECISION) - b.value;
            return b;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Fix operator *(Fix a, Fix b)
        {
            a.value = (a.value * b.value) >> fixlut.PRECISION;
            return a;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Fix operator *(Fix a, int b)
        {
            a.value *= b;
            return a;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Fix operator *(int a, Fix b)
        {
            b.value *= a;
            return b;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Fix operator /(Fix a, Fix b)
        {
            if (b.value == 0)
                return _0;
            a.value = (a.value << fixlut.PRECISION) / b.value;
            return a;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Fix operator /(Fix a, int b)
        {
            if (b== 0)
                return _0;
            a.value /= b;
            return a;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Fix operator /(int a, Fix b)
        {
            b.value = ((long)a << 32) / b.value;
            return b;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Fix operator %(Fix a, Fix b)
        {
            a.value %= b.value;
            return a;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Fix operator %(Fix a, int b)
        {
            a.value %= (long)b << fixlut.PRECISION;
            return a;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Fix operator %(int a, Fix b)
        {
            b.value = ((long)a << fixlut.PRECISION) % b.value;
            return b;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator <(Fix a, Fix b)
        {
            return a.value < b.value;
        }

        internal static Fix Abs(Fix fix)
        {
            return FixedMath.fixmath.Abs(fix);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator <(Fix a, int b)
        {
            return a.value < (long)b << fixlut.PRECISION;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator <(int a, Fix b)
        {
            return (long)a << fixlut.PRECISION < b.value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator <=(Fix a, Fix b)
        {
            return a.value <= b.value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator <=(Fix a, int b)
        {
            return a.value <= (long)b << fixlut.PRECISION;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator <=(int a, Fix b)
        {
            return (long)a << fixlut.PRECISION <= b.value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator >(Fix a, Fix b)
        {
            return a.value > b.value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator >(Fix a, int b)
        {
            return a.value > (long)b << fixlut.PRECISION;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator >(int a, Fix b)
        {
            return (long)a << fixlut.PRECISION > b.value;
        }

        internal static Fix Tenth(int v)
        {
            return ((Fix)v) / _10;
        }
        internal static Fix Hundreth(int v)
        {
            return ((Fix)v) / _100;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator >=(Fix a, Fix b)
        {
            return a.value >= b.value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator >=(Fix a, int b)
        {
            return a.value >= (long)b << fixlut.PRECISION;
        }

        internal float ToFloat()
        {
            return value / 65536f;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator >=(int a, Fix b)
        {
            return (long)a << fixlut.PRECISION >= b.value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(Fix a, Fix b)
        {
            return a.value == b.value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(Fix a, int b)
        {
            return a.value == (long)b << fixlut.PRECISION;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(int a, Fix b)
        {
            return (long)a << fixlut.PRECISION == b.value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(Fix a, Fix b)
        {
            return a.value != b.value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(Fix a, int b)
        {
            return a.value != (long)b << fixlut.PRECISION;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(int a, Fix b)
        {
            return (long)a << fixlut.PRECISION != b.value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator Fix(int value)
        {
            Fix f;
            f.value = (long)value << fixlut.PRECISION;
            return f;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator int(Fix value)
        {
            return (int)(value.value >> fixlut.PRECISION);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator long(Fix value)
        {
            return value.value >> fixlut.PRECISION;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator float(Fix value)
        {
            return value.value / 65536f;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator double(Fix value)
        {
            return value.value / 65536d;
        }

        public int CompareTo(Fix other)
        {
            return value.CompareTo(other.value);
        }

        public bool Equals(Fix other)
        {
            return value == other.value;
        }

        public override bool Equals(object obj)
        {
            return obj is Fix other && this == other;
        }

        public override int GetHashCode()
        {
            return value.GetHashCode();
        }

        public override string ToString()
        {
            var corrected = Math.Round(AsDouble, 5);
            return corrected.ToString("F5", CultureInfo.InvariantCulture);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Fix ParseRaw(long value)
        {
            return new Fix(value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Fix Parse(long value)
        {
            return new Fix(value << fixlut.PRECISION);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Fix ParseUnsafe(float value)
        {
            return new Fix((long)(value * fixlut.ONE + 0.5f * (value < 0 ? -1 : 1)));
        }

        public static Fix ParseUnsafe(string value)
        {
            var doubleValue = double.Parse(value, CultureInfo.InvariantCulture);
            var longValue = (long)(doubleValue * fixlut.ONE + 0.5d * (doubleValue < 0 ? -1 : 1));
            return new Fix(longValue);
        }

        public class Comparer : IComparer<Fix>
        {
            public static readonly Comparer instance = new Comparer();

            private Comparer() { }

            int IComparer<Fix>.Compare(Fix x, Fix y)
            {
                return x.value.CompareTo(y.value);
            }
        }

        public class EqualityComparer : IEqualityComparer<Fix>
        {
            public static readonly EqualityComparer instance = new EqualityComparer();

            private EqualityComparer() { }

            bool IEqualityComparer<Fix>.Equals(Fix x, Fix y)
            {
                return x.value == y.value;
            }

            int IEqualityComparer<Fix>.GetHashCode(Fix num)
            {
                return num.value.GetHashCode();
            }
        }

        internal static Fix Floor(Fix fix)
        {
            return FixedMath.fixmath.Floor(fix);
        }
    }
}