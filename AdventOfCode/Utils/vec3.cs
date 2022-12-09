using System.Reflection.Metadata.Ecma335;
using static System.Math;

namespace Lomont.AdventOfCode.Utils;



public class vec3
{
    public int AbsMax => Math.Max(Math.Abs(x), Math.Max(Math.Abs(y), Math.Abs(z)));
    public vec3(int x, int y, int z=0)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }
    public int x, y, z;

    // each left component <= corresponding right
    public static bool operator <=(vec3 lhs, vec3 rhs)
    {
        return
            lhs.x <= rhs.x &&
            lhs.y <= rhs.y &&
            lhs.z <= rhs.z;

    }
    public static bool operator >=(vec3 lhs, vec3 rhs)
    {
        return
            lhs.x >= rhs.x &&
            lhs.y >= rhs.y &&
            lhs.z >= rhs.z;

    }

    public vec3 Apply(Func<int, int> f) => new vec3(f(x), f(y), f(z));


    public vec3 Sign() => Apply(Math.Sign);

    public static vec3 Min(vec3 lhs, vec3 rhs)
    {
        var (x1, y1, z1) = lhs;
        var (x2, y2, z2) = rhs;
        return new vec3(Math.Min(x1, x2), Math.Min(y1, y2), Math.Min(z1, z2));
    }
    public static vec3 Max(vec3 lhs, vec3 rhs)
    {
        var (x1, y1, z1) = lhs;
        var (x2, y2, z2) = rhs;
        return new vec3(Math.Max(x1, x2), Math.Max(y1, y2), Math.Max(z1, z2));
    }

    public void Deconstruct(out int x, out int y, out int z)
    {
        x = this.x;
        y = this.y;
        z = this.z;

    }
    public static vec3 operator -(vec3 lhs, vec3 rhs)
    {
        return new vec3(lhs.x - rhs.x, lhs.y - rhs.y, lhs.z - rhs.z);
    }
    public static vec3 operator +(vec3 lhs, vec3 rhs)
    {
        return new vec3(lhs.x + rhs.x, lhs.y + rhs.y, lhs.z + rhs.z);
    }
    public long LengthSquared => x * x + y * y + z * z;


    public override string ToString()
    {
        return $"({x},{y},{z})";
    }

    public static bool operator ==(vec3 lhs, vec3 rhs)
    {
        return (lhs - rhs).LengthSquared == 0;
    }

    public static bool operator !=(vec3 lhs, vec3 rhs) => !(lhs == rhs);

    //apply rotations, return new vec
    public vec3 RotXYZ(int rx, int ry, int rz)
    {
        var (x1, y1, z1) = (x, y, z);
        while (rx-- > 0)
            (y1, z1) = (-z1, y1);
        while (ry-- > 0)
            (x1, z1) = (z1, -x1);
        while (rz-- > 0)
            (x1, y1) = (-y1, x1);

        return new vec3(x1, y1, z1);
    }

}