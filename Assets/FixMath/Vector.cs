using Differ.Math;
using FixMath.NET;
using System;
using UnityEngine;
//using System.Numerics;

//NOTE : Only implements the basics required for the collision code.
//The goal is to make this library as simple and unencumbered as possible, making it easier to integrate
//into an existing codebase. This means that using abstracts or similar you can add a function like "toMyEngineVectorFormat()"
//or simple an adapter pattern to convert to your preferred format. It simplifies usage and handles internals, nothing else.
//This also means that ALL of these functions are used and are needed.


[Serializable]
public struct Vector
{
    public Fix64 x;
    public Fix64 y;
    internal static Vector zero = new Vector();

    public Fix64 length
    {
        get
        {
            return Fix64.Sqrt(lengthsq);
        }

        set
        {
            Fix64 ep = Fix64.Epsilon;
            Fix64 angle = Fix64.Atan2(y, x);

            x = Fix64.Cos(angle) * value;
            y = Fix64.Sin(angle) * value;

            if (Fix64.Abs(x) < ep) x = Fix64.Zero;
            if (Fix64.Abs(y) < ep) y = Fix64.Zero;
        }
    }

    public Fix64 lengthsq { get { return x * x + y * y; } }
    public Vector(int x, int y) {
        this.x = new Fix64(x)*Fix64.Tenth*Fix64.Tenth;
        this.y = new Fix64(y)*Fix64.Tenth*Fix64.Tenth;
    }

    public Vector(Fix64 x, Fix64 y)
    {
        this.x = x;
        this.y = y;
    }
   
    /** Copy, returns a new vector instance from this vector. */
    public Vector clone()
    {
        return new Vector(x, y);
    }

    /** Transforms Vector based on the given Matrix. Returns this vector, cloned and modified. */
    public Vector transform(Matrix matrix)
    {
        var v = this;//clone();

        v.x = x * matrix.a + y * matrix.c + matrix.tx;
        v.y = x * matrix.b + y * matrix.d + matrix.ty;

        return v;
    }

    /** Sets the vector's length to 1. Returns this vector, modified. */
    public Vector normalize()
    {

        // Weird hack from original?
        if (length == Fix64.Zero)
        {
            x =Fix64.One;
            return this;
        }

        var len = length;

        x = x / len;
        y = y / len;

        return this;
    }

    /** Sets the length to fit under the given maximum value.
        Nothing is done if the vector is already shorter.
        Returns this vector, modified. */
    public Vector truncate(Fix64 max)
    {
        length = Fix64.Min(max, length);
        return this;
    }
    public static Vector operator -(Vector a, Vector b)
    {
        a.x -= b.x;
        a.y -= b.y;
        return a;
    }

    public static Vector operator +(Vector a, Vector b)
    {
        a.x += b.x;
        a.y += b.y;
        return a;
    }
    /** Invert this vector. Returns this vector, modified. */
    public Vector invert()
    {
        x = -x;
        y = -y;

        return this;
    }

    /** Return the dot product of this vector and another vector. */
    public Fix64 dot(Vector other)
    {
        return x * other.x + y * other.y;
    }

    /** Return the cross product of this vector and another vector. */
    public Fix64 cross(Vector other)
    {
        return x * other.x - y * other.y;
    }

    /** Add a vector to this vector. Returns this vector, modified. */
    public Vector add(Vector other)
    {

        x += other.x;
        y += other.y;

        return this;
    }

    /** Subtract a vector from this one. Returns this vector, modified. */
    public Vector subtract(Vector other)
    {

        x -= other.x;
        y -= other.y;

        return this;
    }

    public override string ToString()
    {
        return string.Format("[Vector: x={0} y={1} length={2}]", x, y, length);
    }

    public static Fix64 Distance(Vector a, Vector b)
    {
        return Fix64.Sqrt(Fix64.Pow(a.x - b.x, new Fix64(2)) + Fix64.Pow(a.y - b.y, new Fix64(2)));
    }

    public static Fix64 Dot(Vector axis, Vector vector)
    {
        return axis.dot(vector);
    }

    public Vector3 ToVector3()
    {
        return new Vector3((float)x, (float)y, 0);
    }
    public Vector2 ToVector2()
    {
        return new Vector2((float)x, (float)y);
    }
    //internal Vec2 ToVec2()
    //{
    //    return new Vec2(x, y);
    //}
}

//[Serializable]
//public struct Vec2
//{
//    public Fix64 x;
//    public Fix64 y;
//    public long x;
//    public long y;
//    public Vec2(Fix64 x, Fix64 y) { this.x = x; this.y = y; this.x = (int)x;this.y = (int)y; }
//    public Vec2(int x, int y) { this.x = new Fix64(x) / new Fix64(100); this.y = new Fix64(y) / new Fix64(100); this.x = (int)x; this.y = (int)y; }
//    public Vector2 ToVector2() => new Vector2((float)x, (float)y);
//    public Vector3 ToVector3() => new Vector3((float)x, (float)y, 0);
//    public static bool operator ==(Vec2 a, Vec2 b) => a.x == b.x && a.y == b.y;
//    public static bool operator !=(Vec2 a, Vec2 b) => !(a.x == b.x && a.y == b.y);
//    public static Vec2 operator +(Vec2 a, Vec2 b) => new Vec2(a.x+ b.x ,a.y +b.y);
//    public Vec2 clone()
//    {
//        return this;
//    }
//    public Vec2 transform(Matrix matrix)
//    {
//        var v = clone();

//        v.x = x * matrix.a + y * matrix.c + matrix.tx;
//        v.y = x * matrix.b + y * matrix.d + matrix.ty;

//        return v;
//    }

//    //public static Vec2 FromVector2(Vector2 vector)
//    //{
//    //    return new Vec2((long)(vector.x * 1000), (long)(vector.y * 1000));
//    //}

//    public void AddX(Fix64 x)
//    {
//        this.x += x;
//    }
//    public void AddY(Fix64 y)
//    {
//        this.y += y;
//    }
//}
