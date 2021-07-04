using FixMath.NET;
using System;

namespace Differ.Math
{
    //NOTE : Only implements the basics required for the shape transformations
    //NOTE : Not a full implementation, some code copied from openfl/flash/geom/Matrix.hx
    public class Matrix
    {
        public Fix64 a;
        public Fix64 b;
        public Fix64 c;
        public Fix64 d;
        public Fix64 tx;
        public Fix64 ty;

        public Matrix(Fix64 a, Fix64 b, Fix64 c, Fix64 d, Fix64 tx, Fix64 ty)
        {
            this.a = a;
            this.b = b;
            this.c = c;
            this.d = d;
            this.tx = tx;
            this.ty = ty;
        }
        public Matrix()
        {
            this.a = Fix64.One;
            this.b = Fix64.Zero;
            this.c = Fix64.Zero;
            this.d = Fix64.One;
            this.tx = Fix64.Zero;
            this.ty = Fix64.Zero;
        }
        public void identity()
        {
            a = Fix64.One;
            b = Fix64.Zero;
            c = Fix64.Zero;
            d = Fix64.One;
            tx = Fix64.Zero;
            ty = Fix64.One;
        }

        public void translate(Fix64 x, Fix64 y)
        {
            tx += x;
            ty += y;
        }

        public Matrix makeTranslation(Fix64 x, Fix64 y)
        {
            tx = x;
            ty = y;

            return this;
        }

        public void rotate(Fix64 angle)
        {
            var cos = Fix64.Cos(angle);
            var sin = Fix64.Sin(angle);

            var a1 = a * cos - b * sin;
            b = a * sin + b * cos;
            a = a1;

            var c1 = c * cos - d * sin;
            d = c * sin + d * cos;
            c = c1;

            var tx1 = tx * cos - ty * sin;
            ty = tx * sin + ty * cos;
            tx = tx1;
        }

        public void scale(Fix64 x, Fix64 y)
        {
            a *= x;
            b *= y;

            c *= x;
            d *= y;

            tx *= x;
            ty *= y;
        }

        public void compose(Vector position, Fix64 rotation, Vector scale)
        {
            identity();

            this.scale(scale.x, scale.y);
            rotate(rotation);
            makeTranslation(position.x, position.y);
        }

        public override string ToString()
        {
            return string.Format("[Matrix a={0} b={1] c={2} d={3} tx={4} ty={5}]", a, b, c, d, tx, ty);
        }
    }
}