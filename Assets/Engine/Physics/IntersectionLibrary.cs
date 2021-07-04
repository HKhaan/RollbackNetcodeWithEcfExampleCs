using FixedMath;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class IntersectionLibrary
{
    public static bool Intersect(Sphere s, Sphere s2)
    {


        var dis = FixVector.Distance(s.position, s2.position);
        var rad = ((s.radius + s2.radius));
        if (dis < rad)
        {
            return true;
        }
        return false;
    }

    public static bool Intersect(Rectangle rectangle, Rectangle toCompare)
    {
        if (Fix.Abs(rectangle.getMin().x - toCompare.getMin().x) < rectangle.dimensions.x + toCompare.dimensions.x)
        {
            if (Fix.Abs(rectangle.getMin().y - toCompare.getMin().y) < rectangle.dimensions.y + toCompare.dimensions.y)
            {
                if (Fix.Abs(rectangle.getMin().z - toCompare.getMin().z) < rectangle.dimensions.z + toCompare.dimensions.z)
                {
                    return true;
                }
            }
        }
        return false;
    }

    public static bool Intersect(Sphere sphere, Rectangle toCompare)
    {
        var dist_squared = sphere.radius * sphere.radius;
        if (sphere.position.x < toCompare.getMin().x) dist_squared -= Fix.Sqrt(sphere.position.x - toCompare.getMin().x);
        else if (sphere.position.x > toCompare.getMax().x) dist_squared -= Fix.Sqrt(sphere.position.x - toCompare.getMax().x);

        if (sphere.position.y < toCompare.getMin().y) dist_squared -= Fix.Sqrt(sphere.position.y - toCompare.getMin().y);
        else if (sphere.position.y > toCompare.getMax().y) dist_squared -= Fix.Sqrt(sphere.position.y - toCompare.getMax().y);

        if (sphere.position.z < toCompare.getMin().z) dist_squared -= Fix.Sqrt(sphere.position.z - toCompare.getMin().z);
        else if (sphere.position.z > toCompare.getMax().z) dist_squared -= Fix.Sqrt(sphere.position.z - toCompare.getMax().z);

        return dist_squared > Fix._0;
    }
}
