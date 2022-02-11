using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace FixedMath {
    [Serializable]
    [StructLayout(LayoutKind.Explicit)]
    public struct Fix3 : IEquatable<Fix3> {
        public const int SIZE = 24;

        public static readonly Fix3 left     = new Fix3(-Fix._1, Fix._0, Fix._0);
        public static readonly Fix3 right    = new Fix3(+Fix._1, Fix._0, Fix._0);
        public static readonly Fix3 up       = new Fix3(Fix._0, +Fix._1, Fix._0);
        public static readonly Fix3 forward  = new Fix3(Fix._0, Fix._0, Fix._1);
        public static readonly Fix3 backward = new Fix3(Fix._0, Fix._0, Fix.minus_one);
        public static readonly Fix3 one = new Fix3(Fix._1, Fix._1, Fix._1);
        public static readonly Fix3 minus_one = new Fix3(Fix.minus_one, Fix.minus_one, Fix.minus_one);
        public static readonly Fix3 zero     = new Fix3(Fix._0, Fix._0, Fix._0);

        [FieldOffset(0)]
        public Fix x;

        [FieldOffset(8)]
        public Fix y;

        [FieldOffset(16)]
        public Fix z;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Fix3(Fix x, Fix y, Fix z) {
            this.x.value = x.value;
            this.y.value = y.value;
            this.z.value = z.value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Fix3(Fix2 xy, Fix z) {
            x.value      = xy.x.value;
            y.value      = xy.y.value;
            this.z.value = z.value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal Fix3(long x, long y, long z) {
            this.x.value = x;
            this.y.value = y;
            this.z.value = z;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Fix3 X(Fix x) {
            Fix3 r;

            r.x.value = x.value;
            r.y.value = 0;
            r.z.value = 0;

            return r;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Fix3 Y(Fix y) {
            Fix3 r;

            r.x.value = 0;
            r.y.value = y.value;
            r.z.value = 0;

            return r;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Fix3 Z(Fix z) {
            Fix3 r;

            r.x.value = 0;
            r.y.value = 0;
            r.z.value = z.value;

            return r;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Fix3 operator +(Fix3 a, Fix3 b) {
            Fix3 r;

            r.x.value = a.x.value + b.x.value;
            r.y.value = a.y.value + b.y.value;
            r.z.value = a.z.value + b.z.value;

            return r;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Fix3 operator -(Fix3 a, Fix3 b) {
            Fix3 r;

            r.x.value = a.x.value - b.x.value;
            r.y.value = a.y.value - b.y.value;
            r.z.value = a.z.value - b.z.value;

            return r;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Fix3 operator -(Fix3 a) {
            a.x.value = -a.x.value;
            a.y.value = -a.y.value;
            a.z.value = -a.z.value;

            return a;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Fix3 operator *(Fix3 a, Fix3 b) {
            Fix3 r;

            r.x.value = (a.x.value * b.x.value) >> fixlut.PRECISION;
            r.y.value = (a.y.value * b.y.value) >> fixlut.PRECISION;
            r.z.value = (a.z.value * b.z.value) >> fixlut.PRECISION;

            return r;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Fix3 operator *(Fix3 a, Fix b) {
            Fix3 r;

            r.x.value = (a.x.value * b.value) >> fixlut.PRECISION;
            r.y.value = (a.y.value * b.value) >> fixlut.PRECISION;
            r.z.value = (a.z.value * b.value) >> fixlut.PRECISION;

            return r;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Fix3 operator *(Fix b, Fix3 a) {
            Fix3 r;

            r.x.value = (a.x.value * b.value) >> fixlut.PRECISION;
            r.y.value = (a.y.value * b.value) >> fixlut.PRECISION;
            r.z.value = (a.z.value * b.value) >> fixlut.PRECISION;

            return r;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Fix3 operator /(Fix3 a, Fix3 b) {
            Fix3 r;

            r.x.value = (a.x.value << fixlut.PRECISION) / b.x.value;
            r.y.value = (a.y.value << fixlut.PRECISION) / b.y.value;
            r.z.value = (a.z.value << fixlut.PRECISION) / b.z.value;

            return r;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Fix3 operator /(Fix3 a, Fix b) {
            Fix3 r;

            r.x.value = (a.x.value << fixlut.PRECISION) / b.value;
            r.y.value = (a.y.value << fixlut.PRECISION) / b.value;
            r.z.value = (a.z.value << fixlut.PRECISION) / b.value;

            return r;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Fix3 operator /(Fix b, Fix3 a) {
            Fix3 r;

            r.x.value = (a.x.value << fixlut.PRECISION) / b.value;
            r.y.value = (a.y.value << fixlut.PRECISION) / b.value;
            r.z.value = (a.z.value << fixlut.PRECISION) / b.value;

            return r;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(Fix3 a, Fix3 b) {
            return a.x.value == b.x.value && a.y.value == b.y.value && a.z.value == b.z.value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(Fix3 a, Fix3 b) {
            return a.x.value != b.x.value || a.y.value != b.y.value || a.z.value != b.z.value;
        }

        public bool Equals(Fix3 other) {
            return x.Equals(other.x) && y.Equals(other.y) && z.Equals(other.z);
        }

        public override bool Equals(object obj) {
            return obj is Fix3 other && this == other;
        }

        public override int GetHashCode() {
            unchecked {
                var hashCode = x.GetHashCode();
                hashCode = (hashCode * 397) ^ y.GetHashCode();
                hashCode = (hashCode * 397) ^ z.GetHashCode();
                return hashCode;
            }
        }

        public override string ToString() {
            return $"({x}, {y}, {z})";
        }

        public class EqualityComparer : IEqualityComparer<Fix3> {
            public static readonly EqualityComparer instance = new EqualityComparer();

            private EqualityComparer() { }

            bool IEqualityComparer<Fix3>.Equals(Fix3 x, Fix3 y) {
                return x == y;
            }

            int IEqualityComparer<Fix3>.GetHashCode(Fix3 obj) {
                return obj.GetHashCode();
            }
        }
    }
}