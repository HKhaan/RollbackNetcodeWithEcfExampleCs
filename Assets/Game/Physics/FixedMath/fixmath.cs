using System.Runtime.CompilerServices;

namespace FixedMath {
    public partial struct fixmath {
        private static readonly Fix _atan2Number1 = new Fix(-883);
        private static readonly Fix _atan2Number2 = new Fix(3767);
        private static readonly Fix _atan2Number3 = new Fix(7945);
        private static readonly Fix _atan2Number4 = new Fix(12821);
        private static readonly Fix _atan2Number5 = new Fix(21822);
        private static readonly Fix _atan2Number6 = new Fix(65536);
        private static readonly Fix _atan2Number7 = new Fix(102943);
        private static readonly Fix _atan2Number8 = new Fix(205887);
        private static readonly Fix _atanApproximatedNumber1 = new Fix(16036);
        private static readonly Fix _atanApproximatedNumber2 = new Fix(4345);
        private static readonly Fix _pow2Number1 = new Fix(177);
        private static readonly Fix _expNumber1 = new Fix(94548);
        private static readonly byte[] _bsrLookup = {0, 9, 1, 10, 13, 21, 2, 29, 11, 14, 16, 18, 22, 25, 3, 30, 8, 12, 20, 28, 15, 17, 24, 7, 19, 27, 23, 6, 26, 5, 4, 31};

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int BitScanReverse(uint num) {
            num |= num >> 1;
            num |= num >> 2;
            num |= num >> 4;
            num |= num >> 8;
            num |= num >> 16;
            return _bsrLookup[(num * 0x07C4ACDDU) >> 27];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int CountLeadingZeroes(uint num) {
            return num == 0 ? 32 : BitScanReverse(num) ^ 31;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Fix Pow2(Fix num) {
            if (num.value > 1638400) {
                return Fix.max;
            }

            var i = num.AsInt;
            num =  Fractions(num) * _pow2Number1 + Fix._1;
            num *= num;
            num *= num;
            num *= num;
            num *= num;
            num *= num;
            num *= num;
            num *= num;
            return num * num * Fix.Parse(1 << i);
        }

        ///Approximate version of Exp
        /// <param name="num">[0, 24]</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Fix ExpApproximated(Fix num) {
            return Pow2(num * _expNumber1);
        }

        /// <param name="num">Angle in radians</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Fix Sin(Fix num) {
            num.value %= Fix.pi2.value;
            num       *= Fix.one_div_pi2;
            var raw = fixlut.sin(num.value);
            Fix result;
            result.value = raw;
            return result;
        }

        /// <param name="num">Angle in radians</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Fix Cos(Fix num) {
            num.value %= Fix.pi2.value;
            num       *= Fix.one_div_pi2;
            return new Fix(fixlut.cos(num.value));
        }

        /// <param name="num">Angle in radians</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Fix Tan(Fix num) {
            num.value %= Fix.pi2.value;
            num       *= Fix.one_div_pi2;
            return new Fix(fixlut.tan(num.value));
        }

        /// <param name="num">Cos [-1, 1]</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Fix Acos(Fix num) {
            num.value += fixlut.ONE;
            num       *= Fix._0_50;
            return new Fix(fixlut.acos(num.value));
        }

        /// <param name="num">Sin [-1, 1]</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Fix Asin(Fix num) {
            num.value += fixlut.ONE;
            num       *= Fix._0_50;
            return new Fix(fixlut.asin(num.value));
        }

        /// <param name="num">Tan</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Fix Atan(Fix num) {
            return Atan2(num, Fix._1);
        }

        /// <param name="num">Tan [-1, 1]</param>
        /// Max error ~0.0015
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Fix AtanApproximated(Fix num) {
            var absX = Abs(num);
            return Fix.pi_quarter * num - num * (absX - Fix._1) * (_atanApproximatedNumber1 + _atanApproximatedNumber2 * absX);
        }

        /// <param name="x">Denominator</param>
        /// <param name="y">Numerator</param>
        public static Fix Atan2(Fix y, Fix x) {
            var absX = Abs(x);
            var absY = Abs(y);
            var t3   = absX;
            var t1   = absY;
            var t0   = Max(t3, t1);
            t1 = Min(t3, t1);
            t3 = Fix._1 / t0;
            t3 = t1 * t3;
            var t4 = t3 * t3;
            t0 = _atan2Number1;
            t0 = t0 * t4 + _atan2Number2;
            t0 = t0 * t4 - _atan2Number3;
            t0 = t0 * t4 + _atan2Number4;
            t0 = t0 * t4 - _atan2Number5;
            t0 = t0 * t4 + _atan2Number6;
            t3 = t0 * t3;
            t3 = absY > absX ? _atan2Number7 - t3 : t3;
            t3 = x < Fix._0 ? _atan2Number8 - t3 : t3;
            t3 = y < Fix._0 ? -t3 : t3;
            return t3;
        }

        /// <param name="num">Angle in radians</param>
        public static void SinCos(Fix num, out Fix sin, out Fix cos) {
            num.value %= Fix.pi2.value;
            num       *= Fix.one_div_pi2;
            fixlut.sin_cos(num.value, out var sinVal, out var cosVal);
            sin.value = sinVal;
            cos.value = cosVal;
        }

        /// <param name="num">Angle in radians</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SinCosTan(Fix num, out Fix sin, out Fix cos, out Fix tan) {
            num.value %= Fix.pi2.value;
            num       *= Fix.one_div_pi2;
            fixlut.sin_cos_tan(num.value, out var sinVal, out var cosVal, out var tanVal);
            sin.value = sinVal;
            cos.value = cosVal;
            tan.value = tanVal;
        }

        public static Fix Rcp(Fix num) {
            //(Fix.one << 16)
            return new Fix(4294967296 / num.value);
        }
        
        public static Fix Rsqrt(Fix num) {
            //(Fix.one << 16)
            return new Fix(4294967296 / Sqrt(num).value);
        }

        public static Fix Sqrt(Fix num) {
            Fix r;

            if (num.value == 0) {
                r.value = 0;
            }
            else {
                var b = (num.value >> 1) + 1L;
                var c = (b + (num.value / b)) >> 1;

                while (c < b) {
                    b = c;
                    c = (b + (num.value / b)) >> 1;
                }

                r.value = b << (fixlut.PRECISION >> 1);
            }

            return r;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Fix Floor(Fix num) {
            num.value = num.value >> fixlut.PRECISION << fixlut.PRECISION;
            return num;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Fix Ceil(Fix num) {
            var fractions = num.value & 0x000000000000FFFFL;

            if (fractions == 0) {
                return num;
            }

            num.value = num.value >> fixlut.PRECISION << fixlut.PRECISION;
            num.value += fixlut.ONE;
            return num;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Fix Fractions(Fix num) {
            return new Fix(num.value & 0x000000000000FFFFL);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int RoundToInt(Fix num) {
            var fraction = num.value & 0x000000000000FFFFL;

            if (fraction >= fixlut.HALF) {
                return num.AsInt + 1;
            }

            return num.AsInt;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Fix Min(Fix a, Fix b) {
            return a.value < b.value ? a : b;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Fix Max(Fix a, Fix b) {
            return a.value > b.value ? a : b;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Fix Abs(Fix num) {
            return new Fix(num.value < 0 ? -num.value : num.value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Fix Clamp(Fix num, Fix min, Fix max) {
            if (num.value < min.value) {
                return min;
            }

            if (num.value > max.value) {
                return max;
            }

            return num;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Fix Clamp01(Fix num) {
            if (num.value < 0) {
                return Fix._0;
            }

            return num.value > Fix._1.value ? Fix._1 : num;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Fix Lerp(Fix from, Fix to, Fix t) {
            t = Clamp01(t);
            return from + (to - from) * t;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Fix Repeat(Fix value, Fix length) {
            return Clamp(value - Floor(value / length) * length, 0, length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Fix LerpAngle(Fix from, Fix to, Fix t) {
            var num = Repeat(to - from, Fix.pi2);
            return Lerp(from, from + (num > Fix.pi ? num - Fix.pi2 : num), t);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Fix NormalizeRadians(Fix angle) {
            angle.value %= fixlut.PI;
            return angle;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Fix LerpUnclamped(Fix from, Fix to, Fix t) {
            return from + (to - from) * t;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Fix Sign(Fix num) {
            return num.value < fixlut.ZERO ? Fix.minus_one : Fix._1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsOppositeSign(Fix a, Fix b) {
            return ((a.value ^ b.value) < 0);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Fix SetSameSign(Fix target, Fix reference) {
            return IsOppositeSign(target, reference) ? target * Fix.minus_one : target;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Fix Pow2(int power) {
            return new Fix(fixlut.ONE << power);
        }

        public static Fix Exp(Fix num) {
            if (num == Fix._0) return Fix._1;
            if (num == Fix._1) return Fix.e;
            if (num.value >= 2097152) return Fix.max;
            if (num.value <= -786432) return Fix._0;

            var neg      = num.value < 0;
            if (neg) num = -num;

            var result = num + Fix._1;
            var term   = num;

            for (var i = 2; i < 30; i++) {
                term   *= num / Fix.Parse(i);
                result += term;

                if (term.value < 500 && ((i > 15) || (term.value < 20)))
                    break;
            }

            if (neg) result = Fix._1 / result;

            return result;
        }
    }
}