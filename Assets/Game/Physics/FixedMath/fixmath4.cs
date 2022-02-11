using System.Runtime.CompilerServices;

namespace FixedMath {
    public partial struct fixmath
    { 
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Fix Sum(Fix4 v) {
            return new Fix(v.x.value + v.y.value + v.z.value + v.w.value);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Fix4 Abs(Fix4 v)
        {
            return new Fix4(Abs(v.x), Abs(v.y), Abs(v.z), Abs(v.w));
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Fix Dot(Fix4 a, Fix4 b) {
            return new Fix(((a.x.value * b.x.value) >> fixlut.PRECISION) + ((a.y.value * b.y.value) >> fixlut.PRECISION) + ((a.z.value * b.z.value) >> fixlut.PRECISION) +
                          ((a.w.value * b.w.value) >> fixlut.PRECISION));
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Fix4 Normalize(Fix4 v)
        {
            if (v == Fix4.zero)
                return Fix4.zero;
            
            return v /  Magnitude(v);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Fix4 Normalize(Fix4 v, out Fix magnitude)
        {
            if (v == Fix4.zero)
            {
                magnitude = Fix._0;
                return Fix4.zero;
            }

            magnitude = Magnitude(v);
            return v / magnitude;
        }
        
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Fix Distance(Fix4 p1, Fix4 p2)
        {
            Fix4 v;
            v.x.value = p1.x.value - p2.x.value;
            v.y.value = p1.y.value - p2.y.value;
            v.z.value = p1.z.value - p2.z.value;
            v.w.value = p1.w.value - p2.w.value;

            return Magnitude(v);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Fix DistanceSqr(Fix4 p1, Fix4 p2)
        {
            Fix4 v;
            v.x.value = p1.x.value - p2.x.value;
            v.y.value = p1.y.value - p2.y.value;
            v.z.value = p1.z.value - p2.z.value;
            v.w.value = p1.w.value - p2.w.value;

            return MagnitudeSqr(v);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Fix MagnitudeSqr(Fix4 v)
        {
            Fix r;

            r.value =
                ((v.x.value * v.x.value) >> fixlut.PRECISION) +
                ((v.y.value * v.y.value) >> fixlut.PRECISION) +
                ((v.z.value * v.z.value) >> fixlut.PRECISION) +
                ((v.w.value * v.w.value) >> fixlut.PRECISION);

            return r;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Fix Magnitude(Fix4 v)
        {
            Fix r;

            r.value =
                ((v.x.value * v.x.value) >> fixlut.PRECISION) +
                ((v.y.value * v.y.value) >> fixlut.PRECISION) +
                ((v.z.value * v.z.value) >> fixlut.PRECISION) +
                ((v.w.value * v.w.value) >> fixlut.PRECISION);
            
            return Sqrt(r);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Fix4 MagnitudeClamp(Fix4 v, Fix length)
        {
            var sqrMagnitude = MagnitudeSqr(v);
            if (sqrMagnitude <= length * length)
                return v;

            var magnitude  = Sqrt(sqrMagnitude);
            var normalized = v / magnitude;
            return normalized * length;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Fix4 MagnitudeSet(Fix4 v, Fix length)
        {
            return Normalize(v) * length;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Fix4 Min(Fix4 a, Fix4 b)
        {
            return new Fix4(Min(a.x, b.x), Min(a.y, b.y), Min(a.z, b.z), Min(a.w, b.w));
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Fix4 Max(Fix4 a, Fix4 b)
        {
            return new Fix4(Max(a.x, b.x), Max(a.y, b.y), Max(a.z, b.z), Max(a.w, b.w));
        }
    }
}