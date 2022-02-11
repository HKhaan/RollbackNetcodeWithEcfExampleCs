using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace FixedMath {
    [StructLayout(LayoutKind.Explicit)]
    public struct Fix4 : IEquatable<Fix4> {
        public const int SIZE = 32;

        [FieldOffset(0)]
        public Fix x;

        [FieldOffset(8)]
        public Fix y;

        [FieldOffset(16)]
        public Fix z;

        [FieldOffset(24)]
        public Fix w;

        public static readonly Fix4 zero;
        public static readonly Fix4 one = new Fix4 {x = Fix._1, y = Fix._1, z = Fix._1, w = Fix._1};
        public static readonly Fix4 minus_one = new Fix4 {x = Fix.minus_one, y = Fix.minus_one, z = Fix.minus_one, w = Fix.minus_one};

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Fix4(Fix x, Fix y, Fix z, Fix w) {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Fix4(Fix2 xy, Fix2 zw) {
            x = xy.x;
            y = xy.y;
            z = zw.x;
            w = zw.y;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Fix4(Fix3 v, Fix w) {
            x      = v.x;
            y      = v.y;
            z      = v.z;
            this.w = w;
        }

        /// <summary>
        /// Initializes Fix4 vector with 48.16 Fix format long values
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal Fix4(long x, long y, long z, long w) {
            this.x.value = x;
            this.y.value = y;
            this.z.value = z;
            this.w.value = w;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Fix4 operator *(Fix4 lhs, Fix4 rhs) {
            return new Fix4((lhs.x.value * rhs.x.value) >> fixlut.PRECISION, (lhs.y.value * rhs.y.value) >> fixlut.PRECISION,
                (lhs.z.value * rhs.z.value) >> fixlut.PRECISION, (lhs.w.value * rhs.w.value) >> fixlut.PRECISION);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Fix4 operator *(Fix4 lhs, Fix rhs) {
            return new Fix4(lhs.x * rhs, lhs.y * rhs, lhs.z * rhs, lhs.w * rhs);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Fix4 operator *(Fix lhs, Fix4 rhs) {
            return new Fix4(lhs * rhs.x, lhs * rhs.y, lhs * rhs.z, lhs * rhs.w);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Fix4 operator +(Fix4 lhs, Fix4 rhs) {
            return new Fix4(lhs.x + rhs.x, lhs.y + rhs.y, lhs.z + rhs.z, lhs.w + rhs.w);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Fix4 operator +(Fix4 lhs, Fix rhs) {
            return new Fix4(lhs.x + rhs, lhs.y + rhs, lhs.z + rhs, lhs.w + rhs);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Fix4 operator +(Fix lhs, Fix4 rhs) {
            return new Fix4(lhs + rhs.x, lhs + rhs.y, lhs + rhs.z, lhs + rhs.w);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Fix4 operator -(Fix4 lhs, Fix4 rhs) {
            return new Fix4(lhs.x - rhs.x, lhs.y - rhs.y, lhs.z - rhs.z, lhs.w - rhs.w);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Fix4 operator -(Fix4 lhs, Fix rhs) {
            return new Fix4(lhs.x - rhs, lhs.y - rhs, lhs.z - rhs, lhs.w - rhs);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Fix4 operator -(Fix lhs, Fix4 rhs) {
            return new Fix4(lhs - rhs.x, lhs - rhs.y, lhs - rhs.z, lhs - rhs.w);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Fix4 operator /(Fix4 lhs, Fix4 rhs) {
            return new Fix4(lhs.x / rhs.x, lhs.y / rhs.y, lhs.z / rhs.z, lhs.w / rhs.w);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Fix4 operator /(Fix4 lhs, Fix rhs) {
            return new Fix4(lhs.x / rhs, lhs.y / rhs, lhs.z / rhs, lhs.w / rhs);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Fix4 operator /(Fix lhs, Fix4 rhs) {
            return new Fix4(lhs / rhs.x, lhs / rhs.y, lhs / rhs.z, lhs / rhs.w);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Fix4 operator %(Fix4 lhs, Fix4 rhs) {
            return new Fix4(lhs.x % rhs.x, lhs.y % rhs.y, lhs.z % rhs.z, lhs.w % rhs.w);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Fix4 operator %(Fix4 lhs, Fix rhs) {
            return new Fix4(lhs.x % rhs, lhs.y % rhs, lhs.z % rhs, lhs.w % rhs);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Fix4 operator %(Fix lhs, Fix4 rhs) {
            return new Fix4(lhs % rhs.x, lhs % rhs.y, lhs % rhs.z, lhs % rhs.w);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(Fix4 a, Fix4 b) {
            return a.x.value == b.x.value && a.y.value == b.y.value && a.z.value == b.z.value && a.w.value == b.w.value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(Fix4 a, Fix4 b) {
            return a.x.value != b.x.value || a.y.value != b.y.value || a.z.value != b.z.value || a.w.value != b.w.value;
        }

        public bool Equals(Fix4 other) {
            return x.Equals(other.x) && y.Equals(other.y) && z.Equals(other.z) && w.Equals(other.w);
        }

        public override bool Equals(object obj) {
            return obj is Fix4 other && Equals(other);
        }

        public override int GetHashCode() {
            unchecked {
                var hashCode = x.GetHashCode();
                hashCode = (hashCode * 397) ^ y.GetHashCode();
                hashCode = (hashCode * 397) ^ z.GetHashCode();
                hashCode = (hashCode * 397) ^ w.GetHashCode();
                return hashCode;
            }
        }
    }
}