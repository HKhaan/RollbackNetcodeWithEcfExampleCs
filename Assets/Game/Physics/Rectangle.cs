using FixedMath;

public class Rectangle : Body {

    public FixVector dimensions;
    public FixVector getMin() {
        return position - (dimensions / Fix._2);
    }
    public FixVector getMax()
    {
        return position + (dimensions / Fix._2);
    }
    public override bool Intersects(Body toCompare)
    {
        if (toCompare.GetType() == typeof(Sphere))
        {
            return IntersectionLibrary.Intersect((Sphere)toCompare,this);
        }
        if (toCompare.GetType() == typeof(Rectangle))
        {
            return IntersectionLibrary.Intersect(this, (Rectangle)toCompare);
        }
        return false;
    }
}