using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace FixedMath {
    [Serializable]
    [StructLayout(LayoutKind.Explicit)]
    public struct Fix2 : IEquatable<Fix2> {
        public const int SIZE = 16;

        public static readonly Fix2 left  = new Fix2(-Fix._1, Fix._0);
        public static readonly Fix2 right = new Fix2(+Fix._1, Fix._0);
        public static readonly Fix2 up    = new Fix2(Fix._0, +Fix._1);
        public static readonly Fix2 down  = new Fix2(Fix._0, -Fix._1);
        public static readonly Fix2 one = new Fix2(Fix._1, Fix._1);
        public static readonly Fix2 minus_one = new Fix2(Fix.minus_one, Fix.minus_one);
        public static readonly Fix2 zero  = new Fix2(Fix._0, Fix._0);

        [FieldOffset(0)]
        public Fix x;

        [FieldOffset(8)]
        public Fix y;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Fix2(Fix x, Fix y) {
            this.x.value = x.value;
            this.y.value = y.value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal Fix2(long x, long y) {
            this.x.value = x;
            this.y.value = y;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Fix2 X(Fix x) {
            Fix2 r;

            r.x.value = x.value;
            r.y.value = 0;

            return r;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Fix2 Y(Fix y) {
            Fix2 r;

            r.x.value = 0;
            r.y.value = y.value;

            return r;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Fix2 operator +(Fix2 a, Fix2 b) {
            Fix2 r;

            r.x.value = a.x.value + b.x.value;
            r.y.value = a.y.value + b.y.value;

            return r;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Fix2 operator -(Fix2 a, Fix2 b) {
            Fix2 r;

            r.x.value = a.x.value - b.x.value;
            r.y.value = a.y.value - b.y.value;

            return r;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Fix2 operator -(Fix2 a) {
            a.x.value = -a.x.value;
            a.y.value = -a.y.value;

            return a;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Fix2 operator *(Fix2 a, Fix2 b) {
            Fix2 r;

            r.x.value = (a.x.value * b.x.value) >> fixlut.PRECISION;
            r.y.value = (a.y.value * b.y.value) >> fixlut.PRECISION;

            return r;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Fix2 operator *(Fix2 a, Fix b) {
            Fix2 r;

            r.x.value = (a.x.value * b.value) >> fixlut.PRECISION;
            r.y.value = (a.y.value * b.value) >> fixlut.PRECISION;

            return r;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Fix2 operator *(Fix b, Fix2 a) {
            Fix2 r;

            r.x.value = (a.x.value * b.value) >> fixlut.PRECISION;
            r.y.value = (a.y.value * b.value) >> fixlut.PRECISION;

            return r;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Fix2 operator /(Fix2 a, Fix2 b) {
            Fix2 r;

            r.x.value = (a.x.value << fixlut.PRECISION) / b.x.value;
            r.y.value = (a.y.value << fixlut.PRECISION) / b.y.value;

            return r;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Fix2 operator /(Fix2 a, Fix b) {
            Fix2 r;

            r.x.value = (a.x.value << fixlut.PRECISION) / b.value;
            r.y.value = (a.y.value << fixlut.PRECISION) / b.value;

            return r;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Fix2 operator /(Fix b, Fix2 a) {
            Fix2 r;

            r.x.value = (a.x.value << fixlut.PRECISION) / b.value;
            r.y.value = (a.y.value << fixlut.PRECISION) / b.value;

            return r;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(Fix2 a, Fix2 b) {
            return a.x.value == b.x.value && a.y.value == b.y.value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(Fix2 a, Fix2 b) {
            return a.x.value != b.x.value || a.y.value != b.y.value;
        }

        public override bool Equals(object obj) {
            return obj is Fix2 other && this == other;
        }

        public bool Equals(Fix2 other) {
            return this == other;
        }

        public override int GetHashCode() {
            unchecked {
                return (x.GetHashCode() * 397) ^ y.GetHashCode();
            }
        }

        public override string ToString() {
            return $"({x}, {y})";
        }

        public class EqualityComparer : IEqualityComparer<Fix2> {
            public static readonly EqualityComparer instance = new EqualityComparer();

            private EqualityComparer() { }

            bool IEqualityComparer<Fix2>.Equals(Fix2 x, Fix2 y) {
                return x == y;
            }

            int IEqualityComparer<Fix2>.GetHashCode(Fix2 obj) {
                return obj.GetHashCode();
            }
        }
    }
}