using OpenTK.Mathematics;

namespace EscapeMaze.Input.Collision;

public static class CollisionHelper
{
    public static bool Intersects(Ray ray, BoundingBox box, out float distance)
    {
        distance = 0f;
        float tmin = (box.Min.X - ray.Origin.X) / ray.Direction.X;
        float tmax = (box.Max.X - ray.Origin.X) / ray.Direction.X;

        if (tmin > tmax)
        {
            (tmin, tmax) = (tmax, tmin);
        }

        float tymin = (box.Min.Y - ray.Origin.Y) / ray.Direction.Y;
        float tymax = (box.Max.Y - ray.Origin.Y) / ray.Direction.Y;

        if (tymin > tymax)
        {
            (tymin, tymax) = (tymax, tymin);
        }

        if ((tmin > tymax) || (tymin > tmax))
        {
            return false;
        }

        if (tymin > tmin)
        {
            tmin = tymin;
        }

        if (tymax < tmax)
        {
            tmax = tymax;
        }

        float tzmin = (box.Min.Z - ray.Origin.Z) / ray.Direction.Z;
        float tzmax = (box.Max.Z - ray.Origin.Z) / ray.Direction.Z;

        if (tzmin > tzmax)
        {
            (tzmin, tzmax) = (tzmax, tzmin);
        }

        if ((tmin > tzmax) || (tzmin > tmax))
        {
            return false;
        }
        
        if (tmax < 0)
        {
            return false;
        }

        distance = tmin < 0 ? 0f : tmin;
        return true;
    }
}
