using FixMath.NET;
using System;

namespace Differ.Math
{
	public class Util
	{
		public static Fix64 vec_lengthsq(Fix64 x, Fix64 y) {
	        return x * x + y * y;
	    }

	    public static Fix64 vec_length(Fix64 x, Fix64 y) {
	    	return Fix64.Sqrt(vec_lengthsq(x, y));
	    }

		public static Fix64 vec_normalize(Fix64 length, Fix64 component) {
			if (length == Fix64.Zero) return Fix64.Zero;
	        return component /= length;
	    }

	    public static Fix64 vec_dot(Fix64 x, Fix64 y, Fix64 otherx, Fix64 othery) {
	    	return x * otherx + y * othery;
	    }
	}
}

