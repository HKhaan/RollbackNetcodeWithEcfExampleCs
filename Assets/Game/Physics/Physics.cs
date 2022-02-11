using FixedMath;
using RollBackExample;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
[Serializable]
public struct FixVector
{

    public FixVector(Fix x, Fix y, Fix z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    public FixVector(Vector3 position)
    {
        this.x = new Fix(position.x);
        this.y = new Fix(position.y);
        this.z = new Fix(position.z);
    }

    public Fix x;
    public Fix y;
    public Fix z;

    public Fix magnitude { get { return Fix.Sqrt((x * x) + (y * y) + (z * z)); } }
    public FixVector Normalize()
    {
        var m = magnitude;
        return new FixVector(x / m, y / m, z / m);
    }
    public FixVector LookAt(FixVector a)
    {
        var x2 = a.x - this.x;
        var z2 = a.z - this.z;
        var angle = Fix.Atan2(z2, x2) * Fix.rad2deg;
        angle = Fix.minus_one * (angle - Fix._90);
        return new FixVector(Fix._0, angle, Fix._0);
    }

    public FixVector Forward()
    {
        return new FixVector(Fix.Sin((this.y) * Fix.deg2rad), Fix._0, Fix.Cos((this.y) * Fix.deg2rad));
    }
    public FixVector Up()
    {
        return FixVector.up;
    }
    public FixVector Right()
    {
        return new FixVector(Fix.Sin((this.y + Fix._90) * Fix.deg2rad), Fix._0, Fix.Cos((this.y + Fix._90) * Fix.deg2rad));
    }

    internal static FixVector FromVector3(Vector3 hitboxPos)
    {
        return new FixVector(new Fix(hitboxPos.x), new Fix(hitboxPos.y), new Fix(hitboxPos.z));
    }

    public static FixVector operator +(FixVector a, FixVector b)
    {
        FixVector c = new FixVector(a.x + b.x, a.y + b.y, a.z + b.z);
        return c;
    }

    //public static Vector2 operator -(Vector2 a);
    public static FixVector operator -(FixVector a, FixVector b)
    {
        FixVector c = new FixVector(a.x - b.x, a.y - b.y, a.z - b.z);
        return c;
    }


    public static FixVector operator *(FixVector a, Fix d)
    {
        FixVector c = new FixVector(a.x * d, a.y * d, a.z * d);
        return c;
    }
    public static FixVector operator *(Fix d, FixVector a)
    {
        FixVector c = new FixVector(a.x * d, a.y * d, a.z * d);
        return c;
    }
    public static FixVector Lerp(FixVector a, FixVector b, Fix f)
    {
        var ret = new FixVector();
        ret.x = a.x + f * (b.x - a.x);
        ret.y = a.y + f * (b.y - a.y);
        ret.z = a.z + f * (b.z - a.z);
        return ret;
    }

    public static FixVector operator /(FixVector a, Fix d)
    {
        FixVector c = new FixVector(a.x / d, a.y / d, a.z / d);
        return c;
    }
    public static FixVector operator /(FixVector a, FixVector b)
    {
        FixVector c = new FixVector(a.x / b.x, a.y / b.y, a.z / b.z);
        return c;
    }
    public static FixVector operator *(FixVector a, FixVector b)
    {
        FixVector c = new FixVector(a.x * b.x, a.y * b.y, a.z * b.z);
        return c;
    }

    public static FixVector right { get; } = new FixVector(Fix._1, Fix._0, Fix._0);
    public static FixVector left { get; } = new FixVector(Fix.minus_one, Fix._0, Fix._0);
    public static FixVector down { get; } = new FixVector(Fix._0, Fix.minus_one, Fix._0);
    public static FixVector up { get; } = new FixVector(Fix._0, Fix._1, Fix._0);
    public static FixVector forward { get; } = new FixVector(Fix._0, Fix._0, Fix._1);
    public static FixVector back { get; } = new FixVector(Fix._0, Fix._0, Fix.minus_one);
    public static FixVector one { get; } = new FixVector(Fix._1, Fix._1, Fix._1);
    public static FixVector zero { get; } = new FixVector(Fix._0, Fix._0, Fix._0);
    public static Fix IntPow(Fix x, int pow = 2)
    {
        Fix ret = x;
        for (int i = 1; i < pow; i++)
        {
            ret *= x;
        }
        return ret;
    }
    internal static Fix Distance(FixVector cob, FixVector cob2)
    {
        return Fix.Sqrt(IntPow(cob2.x - cob.x) + IntPow(cob2.z - cob.z) + IntPow(cob2.y - cob.y));
    }



    public Vector3 ToUnityVec()
    {
        //As float didn't work for some reason.
        return new Vector3((float)x.AsDouble, (float)y.AsDouble, (float)z.AsDouble);
    }
    public new string ToString => $"({x.AsDouble}, {y.AsDouble}, {z.AsDouble})";
    public Quaternion ToQuat()
    {
        return Quaternion.Euler(x.ToFloat(), y.ToFloat(), z.ToFloat());
    }
}



public class CapsuleObject
{
    public int id;


    public Fix radius;
    public int stack;

    public CapsuleObject()
    {
    }
    //public static bool Intersect(Body cob, Body cob2)
    //{
    //    //var fixstack = new Fix32(cob.stack);
    //    //for (Fix32 x = Fix32.Zero; x < fixstack; x += Fix32.One)
    //    Fix x = Fix._0;
    //    {
    //        var c1 = cob;
    //        c1.position.y = c1.position.y + (c1.Shape.radius * x);
    //        for (int x2 = 0; x2 < cob2.Shape.stack; x2++)
    //        {
    //            var c2 = cob2;
    //            c2.position.y = c2.position.y + (c2.Shape.radius * x);
    //            var dis = FixVector.Distance(c1.position, c2.position);
    //            var rad = ((cob.Shape.radius + cob2.Shape.radius));// / Fix32.Two);
    //            var t = dis > rad;
    //            if (!t)
    //            {
    //                return true;
    //            }
    //        }
    //    }
    //    return false;
    //}


}
public class World
{
    private static World instance;
    public static void StartLevel()
    {
        instance = new World();
        instance.bodies.Clear();
    }
    public static World Instance
    {
        get
        {
            return instance;
        }
    }
    private List<Body> bodies = new List<Body>();
    public void AddCapsule(Body capsule) { bodies.Add(capsule); }
    //public int precision = 2;
    public void Step()
    {
        for (int i = 0; (i < bodies.Count); i += 1)
        {
            if (!bodies[i].enabled)
                continue;
            if (bodies[i].force.x == Fix._0 && bodies[i].force.y == Fix._0 && bodies[i].force.z == Fix._0)
            {
                continue;
            }
            if (bodies[i].force.y > Fix._0_01)
            {
                bodies[i].grounded = false;
            }
            //TODO: put this somewhere else where it can be configured
            var steps = Fix._3;
            for (Fix s = Fix._0; s < steps; s += Fix._1)
            {
                for (int ax = 0; ax < 2; ax++)
                {

                    var prevPos = bodies[i].position;
                    bool ok = true;
                    if (ax == 0)
                        bodies[i].position.y += bodies[i].force.y / steps;
                    bodies[i].position.z = Fix._0;
                    if (ax == 1)
                        bodies[i].position.x += bodies[i].force.x / steps;
                    foreach (var cob2 in bodies)
                    {
                        if (!cob2.enabled)
                            continue;
                        if (bodies[i] == cob2)
                            continue;
                        var collided = bodies[i].Intersects(cob2);
                        ok = ok && !collided;
                        if (collided)
                        {
                            var beforeColCheckPos = bodies[i].position;
                            ExecuteCollisions(i, ax, cob2);
                            if (!beforeColCheckPos.Equals(bodies[i].position))
                            {
                                //Collision notifiers have changed our position so lets ignore the collisions
                                bodies[i].force.x = Fix._0;
                                bodies[i].force.z = Fix._0;
                                bodies[i].force.y = Fix._0;
                                return;
                            }
                        }
                    }
                    if (!ok)
                    {
                        bodies[i].position = prevPos;

                        //s = steps + Fix._1;
                    }
                }
            }
            bodies[i].force.x = Fix._0;
            bodies[i].force.z = Fix._0;
            bodies[i].force.y = Fix._0;

        }
    }

    private void ExecuteCollisions(int i, int ax, Body cob2)
    {
        if (bodies[i].force.y < Fix._0 && ax == 0)
        {
            foreach (var collideCallback in bodies[i].CollidedBottom)
            {
                collideCallback.Invoke(cob2.Owner);
            }
            foreach (var collideCallback2 in cob2.CollidedTop)
                collideCallback2.Invoke(bodies[i].Owner);
        }
        if (bodies[i].force.y > Fix._0 && ax == 0)
        {
            foreach (var collideCallback in bodies[i].CollidedTop)
            {
                collideCallback.Invoke(cob2.Owner);
            }
            foreach (var collideCallback2 in cob2.CollidedBottom)
                collideCallback2.Invoke(bodies[i].Owner);
        }

        if (bodies[i].force.x < Fix._0 && ax == 1)
        {
            foreach (var collideCallback in bodies[i].CollidedLeft)
            {
                collideCallback.Invoke(cob2.Owner);
            }
            foreach (var collideCallback2 in cob2.CollidedRight)
                collideCallback2.Invoke(bodies[i].Owner);
        }
        if (bodies[i].force.x > Fix._0 && ax == 1)
        {
            foreach (var collideCallback in bodies[i].CollidedRight)
            {
                collideCallback.Invoke(cob2.Owner);
            }
            foreach (var collideCallback2 in cob2.CollidedLeft)
                collideCallback2.Invoke(bodies[i].Owner);
        }
    }
}
