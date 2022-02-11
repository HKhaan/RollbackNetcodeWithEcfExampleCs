using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace FixedMath {
    [StructLayout(LayoutKind.Explicit)]
    public struct Random {
        public const int SIZE = 4;

        [FieldOffset(0)]
        public uint state;
        
        /// <summary>
        /// Seed must be non-zero
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Random(uint seed)
        {
            state = seed;
            NextState();
        }

        /// <summary>
        /// Seed must be non-zero
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetState(uint seed)
        {
            state = seed;
            NextState();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private uint NextState() {
            var t  = state;
            state ^= state << 13;
            state ^= state >> 17;
            state ^= state << 5;
            return t;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool NextBool()
        {
            return (NextState() & 1) == 1;
        }

        /// <summary>Returns value in range [-2147483647, 2147483647]</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int NextInt()
        {
            return (int)NextState() ^ -2147483648;
        }
        
        /// <summary>Returns value in range [0, max]</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int NextInt(int max)
        {
            return (int)((NextState() * (ulong)max) >> 32);
        }
        
        /// <summary>Returns value in range [min, max].</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int NextInt(int min, int max)
        {
            var range = (uint)(max - min);
            return (int)(NextState() * (ulong)range >> 32) + min;
        }
        
        /// <summary>Returns value in range [0, 1]</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Fix NextFix() {
            return new Fix(NextInt(0, 65535));
        }
        
        /// <summary>Returns vector with all components in range [0, 1]</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Fix2 NextFix2() {
            return new Fix2(NextFix(), NextFix());
        }
        
        /// <summary>Returns vector with all components in range [0, 1]</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Fix3 NextFix3() {
            return new Fix3(NextFix(), NextFix(), NextFix());
        }
        
        /// <summary>Returns vector with all components in range [0, 1]</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Fix4 NextFix4() {
            return new Fix4(NextFix(), NextFix(), NextFix(), NextFix());
        }


        /// <summary>Returns value in range [0, max]</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Fix NextFix(Fix max) {
            return NextFix() * max;
        }
        
        /// <summary>Returns vector with all components in range [0, max]</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Fix2 NextFix2(Fix2 max) {
            return NextFix2() * max;
        }
        
        /// <summary>Returns vector with all components in range [0, max]</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Fix3 NextFix3(Fix3 max) {
            return NextFix3() * max;
        }
        
        /// <summary>Returns vector with all components in range [0, max]</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Fix4 NextFix4(Fix4 max) {
            return NextFix4() * max;
        }

        /// <summary>Returns value in range [min, max]</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Fix NextFix(Fix min, Fix max) {
            return NextFix() * (max - min) + min;
        }

        /// <summary>Returns vector with all components in range [min, max]</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Fix2 NextFix2(Fix2 min, Fix2 max) {
            return NextFix2() * (max - min) + min;
        }
        
        /// <summary>Returns vector with all components in range [min, max]</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Fix3 NextFix3(Fix3 min, Fix3 max) {
            return NextFix3() * (max - min) + min;
        }
        
        /// <summary>Returns vector with all components in range [min, max]</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Fix4 NextFix4(Fix4 min, Fix4 max) {
            return NextFix4() * (max - min) + min;
        }
        
        /// <summary>Returns a normalized 2D direction</summary>
        public Fix2 NextDirection2D() {
            var angle = NextFix() * Fix.pi * Fix._2;
            fixmath.SinCos(angle, out var sin, out var cos);
            return new Fix2(sin,cos);
        }
        
        /// <summary>Returns a normalized 3D direction</summary>
        public Fix3 NextDirection3D() {
            var z = NextFix(Fix._2) - Fix._1;
            var r = fixmath.Sqrt(fixmath.Max(Fix._1 - z * z, Fix._0));
            var angle = NextFix(Fix.pi2);
            fixmath.SinCos(angle, out var sin, out var cos);
            return new Fix3(cos * r, sin * r, z);
        }
    }
}