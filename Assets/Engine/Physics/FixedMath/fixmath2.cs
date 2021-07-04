using System.Runtime.CompilerServices;

namespace FixedMath
{
    public partial struct fixmath
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Fix Sum(Fix2 v) {
            return new Fix(v.x.value + v.y.value);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Fix2 Min(Fix2 a, Fix2 b)
        {
            Fix ret;
            if (a.x.value < b.x.value)
            {
                ret = a.x;
            }
            else
            {
                ret = b.x;
            }

            Fix ret1;
            if (a.y.value < b.y.value)
            {
                ret1 = a.y;
            }
            else
            {
                ret1 = b.y;
            }

            return new Fix2(ret, ret1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Fix2 Max(Fix2 a, Fix2 b)
        {
            Fix ret;
            if (a.x.value > b.x.value)
            {
                ret = a.x;
            }
            else
            {
                ret = b.x;
            }

            Fix ret1;
            if (a.y.value > b.y.value)
            {
                ret1 = a.y;
            }
            else
            {
                ret1 = b.y;
            }

            return new Fix2(ret, ret1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Fix Dot(Fix2 a, Fix2 b)
        {
            Fix2 a1 = a;
            Fix2 b1 = b;
            var x = ((a1.x.value * b1.x.value) >> fixlut.PRECISION);
            var z = ((a1.y.value * b1.y.value) >> fixlut.PRECISION);

            Fix r;

            r.value = x + z;

            return r;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Fix Cross(Fix2 a, Fix2 b)
        {
            return (a.x * b.y) - (a.y * b.x);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Fix2 Cross(Fix2 a, Fix s)
        {
            return new Fix2(s * a.y, -s * a.x);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Fix2 Cross(Fix s, Fix2 a)
        {
            return new Fix2(-s * a.y, s * a.x);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Fix2 Clamp(Fix2 num, Fix2 min, Fix2 max)
        {
            Fix2 r;

            if (num.x.value < min.x.value)
            {
                r.x = min.x;
            }
            else
            {
                if (num.x.value > max.x.value)
                {
                    r.x = max.x;
                }
                else
                {
                    r.x = num.x;
                }
            }

            if (num.y.value < min.y.value)
            {
                r.y = min.y;
            }
            else
            {
                if (num.y.value > max.y.value)
                {
                    r.y = max.y;
                }
                else
                {
                    r.y = num.y;
                }
            }

            return r;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Fix2 ClampMagnitude(Fix2 v, Fix length)
        {
            Fix2 value = v;
            Fix r;

            r.value =
                ((value.x.value * value.x.value) >> fixlut.PRECISION) +
                ((value.y.value * value.y.value) >> fixlut.PRECISION);
            if (r.value <= ((length.value * length.value) >> fixlut.PRECISION))
            {
            }
            else
            {
                Fix2 v1 = value;
                Fix m = default;
                Fix r2;

                r2.value =
                    ((v1.x.value * v1.x.value) >> fixlut.PRECISION) +
                    ((v1.y.value * v1.y.value) >> fixlut.PRECISION);
                Fix r1;

                if (r2.value == 0)
                {
                    r1.value = 0;
                }
                else
                {
                    var b = (r2.value >> 1) + 1L;
                    var c = (b + (r2.value / b)) >> 1;

                    while (c < b)
                    {
                        b = c;
                        c = (b + (r2.value / b)) >> 1;
                    }

                    r1.value = b << (fixlut.PRECISION >> 1);
                }

                m = r1;

                if (m.value <= Fix.epsilon.value)
                {
                    v1 = default;
                }
                else
                {
                    v1.x.value = ((v1.x.value << fixlut.PRECISION) / m.value);
                    v1.y.value = ((v1.y.value << fixlut.PRECISION) / m.value);
                }

                value = v1 * length;
            }

            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Fix Magnitude(Fix2 v)
        {
            Fix r;

            r.value =
                ((v.x.value * v.x.value) >> fixlut.PRECISION) +
                ((v.y.value * v.y.value) >> fixlut.PRECISION);
            Fix r1;

            if (r.value == 0)
            {
                r1.value = 0;
            }
            else
            {
                var b = (r.value >> 1) + 1L;
                var c = (b + (r.value / b)) >> 1;

                while (c < b)
                {
                    b = c;
                    c = (b + (r.value / b)) >> 1;
                }

                r1.value = b << (fixlut.PRECISION >> 1);
            }

            return r1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Fix MagnitudeSqr(Fix2 v)
        {
            Fix r;

            r.value =
                ((v.x.value * v.x.value) >> fixlut.PRECISION) +
                ((v.y.value * v.y.value) >> fixlut.PRECISION);

            return r;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Fix Distance(Fix2 a, Fix2 b)
        {
            Fix2 v;

            v.x.value = a.x.value - b.x.value;
            v.y.value = a.y.value - b.y.value;

            Fix r;

            r.value =
                ((v.x.value * v.x.value) >> fixlut.PRECISION) +
                ((v.y.value * v.y.value) >> fixlut.PRECISION);
            Fix r1;

            if (r.value == 0)
            {
                r1.value = 0;
            }
            else
            {
                var b1 = (r.value >> 1) + 1L;
                var c  = (b1 + (r.value / b1)) >> 1;

                while (c < b1)
                {
                    b1 = c;
                    c  = (b1 + (r.value / b1)) >> 1;
                }

                r1.value = b1 << (fixlut.PRECISION >> 1);
            }

            return r1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Fix DistanceSqr(Fix2 a, Fix2 b)
        {
            var x = a.x.value - b.x.value;
            var z = a.y.value - b.y.value;

            Fix r;
            r.value = ((x * x) >> fixlut.PRECISION) + ((z * z) >> fixlut.PRECISION);
            return r;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Fix2 Normalize(Fix2 v)
        {
            Fix2 v1 = v;
            Fix m = default;
            Fix r;

            r.value =
                ((v1.x.value * v1.x.value) >> fixlut.PRECISION) +
                ((v1.y.value * v1.y.value) >> fixlut.PRECISION);
            Fix r1;

            if (r.value == 0)
            {
                r1.value = 0;
            }
            else
            {
                var b = (r.value >> 1) + 1L;
                var c = (b + (r.value / b)) >> 1;

                while (c < b)
                {
                    b = c;
                    c = (b + (r.value / b)) >> 1;
                }

                r1.value = b << (fixlut.PRECISION >> 1);
            }

            m = r1;

            if (m.value <= Fix.epsilon.value)
            {
                v1 = default;
            }
            else
            {
                v1.x.value = ((v1.x.value << fixlut.PRECISION) / m.value);
                v1.y.value = ((v1.y.value << fixlut.PRECISION) / m.value);
            }

            return v1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Fix2 Lerp(Fix2 from, Fix2 to, Fix t) {
            t = Clamp01(t);
            return new Fix2(LerpUnclamped(from.x, to.x, t), LerpUnclamped(from.y, to.y, t));
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Fix2 LerpUnclamped(Fix2 from, Fix2 to, Fix t)
        {
            return new Fix2(LerpUnclamped(from.x, to.x, t), LerpUnclamped(from.y, to.y, t));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Fix Angle(Fix2 a, Fix2 b)
        {
            Fix2 v = a;
            Fix m = default;
            Fix r2;

            r2.value =
                ((v.x.value * v.x.value) >> fixlut.PRECISION) +
                ((v.y.value * v.y.value) >> fixlut.PRECISION);
            Fix r1;

            if (r2.value == 0)
            {
                r1.value = 0;
            }
            else
            {
                var b2 = (r2.value >> 1) + 1L;
                var c  = (b2 + (r2.value / b2)) >> 1;

                while (c < b2)
                {
                    b2 = c;
                    c  = (b2 + (r2.value / b2)) >> 1;
                }

                r1.value = b2 << (fixlut.PRECISION >> 1);
            }

            m = r1;

            if (m.value <= Fix.epsilon.value)
            {
                v = default;
            }
            else
            {
                v.x.value = ((v.x.value << fixlut.PRECISION) / m.value);
                v.y.value = ((v.y.value << fixlut.PRECISION) / m.value);
            }

            Fix2 v1 = b;
            Fix m1 = default;
            Fix r3;

            r3.value =
                ((v1.x.value * v1.x.value) >> fixlut.PRECISION) +
                ((v1.y.value * v1.y.value) >> fixlut.PRECISION);
            Fix r4;

            if (r3.value == 0)
            {
                r4.value = 0;
            }
            else
            {
                var b3 = (r3.value >> 1) + 1L;
                var c1 = (b3 + (r3.value / b3)) >> 1;

                while (c1 < b3)
                {
                    b3 = c1;
                    c1 = (b3 + (r3.value / b3)) >> 1;
                }

                r4.value = b3 << (fixlut.PRECISION >> 1);
            }

            m1 = r4;

            if (m1.value <= Fix.epsilon.value)
            {
                v1 = default;
            }
            else
            {
                v1.x.value = ((v1.x.value << fixlut.PRECISION) / m1.value);
                v1.y.value = ((v1.y.value << fixlut.PRECISION) / m1.value);
            }

            Fix2 a1 = v;
            Fix2 b1 = v1;
            var x = ((a1.x.value * b1.x.value) >> fixlut.PRECISION);
            var z = ((a1.y.value * b1.y.value) >> fixlut.PRECISION);

            Fix r;

            r.value = x + z;
            var dot = r;
            Fix min = -Fix._1;
            Fix max = +Fix._1;
            Fix ret;
            if (dot.value < min.value)
            {
                ret = min;
            }
            else
            {
                if (dot.value > max.value)
                {
                    ret = max;
                }
                else
                {
                    ret = dot;
                }
            }

            return new Fix(fixlut.acos(ret.value)) * Fix.rad2deg;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Fix Radians(Fix2 a, Fix2 b)
        {
            Fix2 v = a;
            Fix m = default;
            Fix r2;

            r2.value =
                ((v.x.value * v.x.value) >> fixlut.PRECISION) +
                ((v.y.value * v.y.value) >> fixlut.PRECISION);
            Fix r1;

            if (r2.value == 0)
            {
                r1.value = 0;
            }
            else
            {
                var b2 = (r2.value >> 1) + 1L;
                var c  = (b2 + (r2.value / b2)) >> 1;

                while (c < b2)
                {
                    b2 = c;
                    c  = (b2 + (r2.value / b2)) >> 1;
                }

                r1.value = b2 << (fixlut.PRECISION >> 1);
            }

            m = r1;

            if (m.value <= Fix.epsilon.value)
            {
                v = default;
            }
            else
            {
                v.x.value = ((v.x.value << fixlut.PRECISION) / m.value);
                v.y.value = ((v.y.value << fixlut.PRECISION) / m.value);
            }

            Fix2 v1 = b;
            Fix m1 = default;
            Fix r3;

            r3.value =
                ((v1.x.value * v1.x.value) >> fixlut.PRECISION) +
                ((v1.y.value * v1.y.value) >> fixlut.PRECISION);
            Fix r4;

            if (r3.value == 0)
            {
                r4.value = 0;
            }
            else
            {
                var b3 = (r3.value >> 1) + 1L;
                var c1 = (b3 + (r3.value / b3)) >> 1;

                while (c1 < b3)
                {
                    b3 = c1;
                    c1 = (b3 + (r3.value / b3)) >> 1;
                }

                r4.value = b3 << (fixlut.PRECISION >> 1);
            }

            m1 = r4;

            if (m1.value <= Fix.epsilon.value)
            {
                v1 = default;
            }
            else
            {
                v1.x.value = ((v1.x.value << fixlut.PRECISION) / m1.value);
                v1.y.value = ((v1.y.value << fixlut.PRECISION) / m1.value);
            }

            Fix2 a1 = v;
            Fix2 b1 = v1;
            var x = ((a1.x.value * b1.x.value) >> fixlut.PRECISION);
            var z = ((a1.y.value * b1.y.value) >> fixlut.PRECISION);

            Fix r;

            r.value = x + z;
            var dot = r;
            Fix min = -Fix._1;
            Fix max = +Fix._1;
            Fix ret;
            if (dot.value < min.value)
            {
                ret = min;
            }
            else
            {
                if (dot.value > max.value)
                {
                    ret = max;
                }
                else
                {
                    ret = dot;
                }
            }

            return new Fix(fixlut.acos(ret.value));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Fix RadiansSigned(Fix2 a, Fix2 b)
        {
            Fix2 v = a;
            Fix m = default;
            Fix r2;

            r2.value =
                ((v.x.value * v.x.value) >> fixlut.PRECISION) +
                ((v.y.value * v.y.value) >> fixlut.PRECISION);
            Fix r1;

            if (r2.value == 0)
            {
                r1.value = 0;
            }
            else
            {
                var b2 = (r2.value >> 1) + 1L;
                var c  = (b2 + (r2.value / b2)) >> 1;

                while (c < b2)
                {
                    b2 = c;
                    c  = (b2 + (r2.value / b2)) >> 1;
                }

                r1.value = b2 << (fixlut.PRECISION >> 1);
            }

            m = r1;

            if (m.value <= Fix.epsilon.value)
            {
                v = default;
            }
            else
            {
                v.x.value = ((v.x.value << fixlut.PRECISION) / m.value);
                v.y.value = ((v.y.value << fixlut.PRECISION) / m.value);
            }

            Fix2 v1 = b;
            Fix m1 = default;
            Fix r3;

            r3.value =
                ((v1.x.value * v1.x.value) >> fixlut.PRECISION) +
                ((v1.y.value * v1.y.value) >> fixlut.PRECISION);
            Fix r4;

            if (r3.value == 0)
            {
                r4.value = 0;
            }
            else
            {
                var b3 = (r3.value >> 1) + 1L;
                var c1 = (b3 + (r3.value / b3)) >> 1;

                while (c1 < b3)
                {
                    b3 = c1;
                    c1 = (b3 + (r3.value / b3)) >> 1;
                }

                r4.value = b3 << (fixlut.PRECISION >> 1);
            }

            m1 = r4;

            if (m1.value <= Fix.epsilon.value)
            {
                v1 = default;
            }
            else
            {
                v1.x.value = ((v1.x.value << fixlut.PRECISION) / m1.value);
                v1.y.value = ((v1.y.value << fixlut.PRECISION) / m1.value);
            }

            Fix2 a1 = v;
            Fix2 b1 = v1;
            var x = ((a1.x.value * b1.x.value) >> fixlut.PRECISION);
            var z = ((a1.y.value * b1.y.value) >> fixlut.PRECISION);

            Fix r;

            r.value = x + z;
            var dot = r;
            Fix min = -Fix._1;
            Fix max = +Fix._1;
            Fix ret;
            if (dot.value < min.value)
            {
                ret = min;
            }
            else
            {
                if (dot.value > max.value)
                {
                    ret = max;
                }
                else
                {
                    ret = dot;
                }
            }

            var rad  = new Fix(fixlut.acos(ret.value));
            var sign = ((a.x * b.y - a.y * b.x).value <  fixlut.ZERO) ? Fix.minus_one : Fix._1;

            return rad * sign;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Fix RadiansSkipNormalize(Fix2 a, Fix2 b)
        {
            Fix2 a1 = a;
            Fix2 b1 = b;
            var x = ((a1.x.value * b1.x.value) >> fixlut.PRECISION);
            var z = ((a1.y.value * b1.y.value) >> fixlut.PRECISION);

            Fix r;

            r.value = x + z;
            var dot = r;
            Fix min = -Fix._1;
            Fix max = +Fix._1;
            Fix ret;
            if (dot.value < min.value)
            {
                ret = min;
            }
            else
            {
                if (dot.value > max.value)
                {
                    ret = max;
                }
                else
                {
                    ret = dot;
                }
            }

            return new Fix(fixlut.acos(ret.value));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Fix RadiansSignedSkipNormalize(Fix2 a, Fix2 b)
        {
            Fix2 a1 = a;
            Fix2 b1 = b;
            var x = ((a1.x.value * b1.x.value) >> fixlut.PRECISION);
            var z = ((a1.y.value * b1.y.value) >> fixlut.PRECISION);

            Fix r;

            r.value = x + z;
            var dot = r;
            Fix min = -Fix._1;
            Fix max = +Fix._1;
            Fix ret;
            if (dot.value < min.value)
            {
                ret = min;
            }
            else
            {
                if (dot.value > max.value)
                {
                    ret = max;
                }
                else
                {
                    ret = dot;
                }
            }

            var rad  = new Fix(fixlut.acos(ret.value));
            var sign = ((a.x * b.y - a.y * b.x).value < fixlut.ZERO) ? Fix.minus_one : Fix._1;

            return rad * sign;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Fix2 Reflect(Fix2 vector, Fix2 normal)
        {
            Fix dot = (vector.x * normal.x) + (vector.y * normal.y);

            Fix2 result;

            result.x = vector.x - ((Fix._2 * dot) * normal.x);
            result.y = vector.y - ((Fix._2 * dot) * normal.y);

            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Fix2 Rotate(Fix2 vector, Fix angle)
        {
            Fix2 vector1 = vector;
            var cs = Cos(angle);
            var sn = Sin(angle);

            var px = (vector1.x * cs) - (vector1.y * sn);
            var pz = (vector1.x * sn) + (vector1.y * cs);

            vector1.x = px;
            vector1.y = pz;

            return vector1;
        }
    }
}