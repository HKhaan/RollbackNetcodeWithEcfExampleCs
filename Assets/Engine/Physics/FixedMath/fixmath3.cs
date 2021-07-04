using System.Runtime.CompilerServices;

namespace FixedMath
{
    public partial struct fixmath
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Fix Sum(Fix3 v) {
            return new Fix(v.x.value + v.y.value + v.z.value);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Fix3 Abs(Fix3 v)
        {
            return new Fix3(Abs(v.x), Abs(v.y), Abs(v.z));
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Fix Dot(Fix3 a, Fix3 b)
        {
            return new Fix(((a.x.value * b.x.value) >> fixlut.PRECISION) + ((a.y.value * b.y.value) >> fixlut.PRECISION) + ((a.z.value * b.z.value) >> fixlut.PRECISION));
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Fix3 Cross(Fix3 a, Fix3 b)
        {
            Fix3 r;

            r.x = a.y * b.z - a.z * b.y;
            r.y = a.z * b.x - a.x * b.z;
            r.z = a.x * b.y - a.y * b.x;
            
            return r;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Fix Distance(Fix3 p1, Fix3 p2)
        {
            Fix3 v;
            v.x.value = p1.x.value - p2.x.value;
            v.y.value = p1.y.value - p2.y.value;
            v.z.value = p1.z.value - p2.z.value;

            return Magnitude(v);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Fix DistanceSqr(Fix3 p1, Fix3 p2)
        {
            Fix3 v;
            v.x.value = p1.x.value - p2.x.value;
            v.y.value = p1.y.value - p2.y.value;
            v.z.value = p1.z.value - p2.z.value;

            return MagnitudeSqr(v);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Fix3 Lerp(Fix3 from, Fix3 to, Fix t)
        {
            t = Clamp01(t);
            return new Fix3(LerpUnclamped(from.x, to.x, t), LerpUnclamped(from.y, to.y, t), LerpUnclamped(from.z, to.z, t));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Fix3 LerpUnclamped(Fix3 from, Fix3 to, Fix t)
        {
            return new Fix3(LerpUnclamped(from.x, to.x, t), LerpUnclamped(from.y, to.y, t), LerpUnclamped(from.z, to.z, t));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Fix3 Reflect(Fix3 vector, Fix3 normal)
        {
            var num = -Fix._2 * Dot(normal, vector);
            return new Fix3(num * normal.x + vector.x, num * normal.y + vector.y, num * normal.z + vector.z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Fix3 Project(Fix3 vector, Fix3 normal)
        {
            var sqrMag = MagnitudeSqr(normal);
            if (sqrMag < Fix.epsilon)
                return Fix3.zero;

            var dot = Dot(vector, normal);
            return normal * dot / sqrMag;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Fix3 ProjectOnPlane(Fix3 vector, Fix3 planeNormal)
        {
            var sqrMag = MagnitudeSqr(planeNormal);
            if (sqrMag < Fix.epsilon)
                return vector;

            var dot = Dot(vector, planeNormal);
            return vector - planeNormal * dot / sqrMag;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Fix3 MoveTowards(Fix3 current, Fix3 target, Fix maxDelta)
        {
            var v = target - current;
            var sqrMagnitude = MagnitudeSqr(v);
            if (v == Fix3.zero || maxDelta >= Fix._0 && sqrMagnitude <= maxDelta * maxDelta)
                return target;

            var magnitude = Sqrt(sqrMagnitude);
            return current + v / magnitude * maxDelta;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Fix Angle(Fix3 a, Fix3 b)
        {
            var sqr = MagnitudeSqr(a) * MagnitudeSqr(b);
            var n = Sqrt(sqr);

            if (n < Fix.epsilon)
            {
                return Fix._0;
            }

            return Acos(Clamp(Dot(a, b) / n, Fix.minus_one, Fix._1)) * Fix.rad2deg;
        }

        public static Fix AngleSigned(Fix3 a, Fix3 b, Fix3 axis)
        {
            var angle = Angle(a, b);
            long num2 = ((a.y.value * b.z.value) >> fixlut.PRECISION) - ((a.z.value * b.y.value) >> fixlut.PRECISION);
            long num3 = ((a.z.value * b.x.value) >> fixlut.PRECISION) - ((a.x.value * b.z.value) >> fixlut.PRECISION);
            long num4 = ((a.x.value * b.y.value) >> fixlut.PRECISION) - ((a.y.value * b.x.value) >> fixlut.PRECISION);
            var sign = (((axis.x.value * num2) >> fixlut.PRECISION) +
                        ((axis.y.value * num3) >> fixlut.PRECISION) +
                        ((axis.z.value * num4) >> fixlut.PRECISION)) < 0
                ? Fix.minus_one
                : Fix._1;
            return angle * sign;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Fix Radians(Fix3 a, Fix3 b)
        {
            return Acos(Clamp(Dot(Normalize(a), Normalize(b)), Fix.minus_one, Fix._1));
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Fix RadiansSkipNormalize(Fix3 a, Fix3 b)
        {
            return Acos(Clamp(Dot(a, b), Fix.minus_one, Fix._1));
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Fix MagnitudeSqr(Fix3 v)
        {
            Fix r;

            r.value =
                ((v.x.value * v.x.value) >> fixlut.PRECISION) +
                ((v.y.value * v.y.value) >> fixlut.PRECISION) +
                ((v.z.value * v.z.value) >> fixlut.PRECISION);

            return r;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Fix Magnitude(Fix3 v)
        {
            Fix r;

            r.value =
                ((v.x.value * v.x.value) >> fixlut.PRECISION) +
                ((v.y.value * v.y.value) >> fixlut.PRECISION) +
                ((v.z.value * v.z.value) >> fixlut.PRECISION);
            
            return Sqrt(r);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Fix3 MagnitudeClamp(Fix3 v, Fix length)
        {
            var sqrMagnitude = MagnitudeSqr(v);
            if (sqrMagnitude <= length * length)
                return v;

            var magnitude = Sqrt(sqrMagnitude);
            var normalized = v / magnitude;
            return normalized * length;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Fix3 MagnitudeSet(Fix3 v, Fix length)
        {
            return Normalize(v) * length;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Fix3 Min(Fix3 a, Fix3 b)
        {
            return new Fix3(Min(a.x, b.x), Min(a.y, b.y), Min(a.z, b.z));
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Fix3 Max(Fix3 a, Fix3 b)
        {
            return new Fix3(Max(a.x, b.x), Max(a.y, b.y), Max(a.z, b.z));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Fix3 Normalize(Fix3 v)
        {
            if (v == Fix3.zero)
                return Fix3.zero;
            
            return v /  Magnitude(v);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Fix3 Normalize(Fix3 v, out Fix magnitude)
        {
            if (v == Fix3.zero)
            {
                magnitude = Fix._0;
                return Fix3.zero;
            }

            magnitude = Magnitude(v);
            return v / magnitude;
        }
    }
}