using FixedMath;

public class Sphere : Body
{
    public Fix radius;
    public override bool Intersects(Body toCompare)
    {
        if (toCompare.GetType() == typeof(Sphere)) {
            return IntersectionLibrary.Intersect(this, (Sphere)toCompare);
        }
        if (toCompare.GetType() == typeof(Rectangle))
        {
            return IntersectionLibrary.Intersect(this, (Rectangle)toCompare);
        }
        return false;
    }
}
