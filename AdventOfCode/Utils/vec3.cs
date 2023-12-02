namespace Lomont.AdventOfCode.Utils;

public class vec3 : IEquatable<vec3>
{

    #region constants
    public static vec3 MaxValue = new vec3(int.MaxValue, int.MaxValue, int.MaxValue);
    public static vec3 MinValue = new vec3(int.MinValue, int.MinValue, int.MinValue);

    public static vec3 XAxis = new vec3(1,0,0);
    public static vec3 YAxis = new vec3(0, 1, 0);
    public static vec3 ZAxis = new vec3(0, 0, 1);
    public static vec3 Zero = new vec3(0, 0, 0);
    public static vec3 One = new vec3(1, 1, 1);
    #endregion

    #region constructors
    public vec3(int x=0, int y=0, int z=0)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }
    #endregion

    #region field access
    int [] vals = new int[3];

    public int this[int i]
    {
        get => i switch {0 => x, 1 => y, 2 => z, _ => throw new IndexOutOfRangeException()};
        set
        {
            if (i == 0) x = value;
            else if (i == 1) y = value;
            else if (i == 2) z = value;
            else throw new IndexOutOfRangeException();
        }
    }

    public int x
    {
        get => vals[0];
        set { vals[0] = value; }
    }
    public int y
    {
        get => vals[1];
        set { vals[1] = value; }
    }

    public int z
    {
        get => vals[2];
        set { vals[2] = value; }
    }


    public void Deconstruct(out int x, out int y, out int z)
    {
        x = this.x;
        y = this.y;
        z = this.z;
    }
    public void Deconstruct(out int x, out int y)
    {
        x = this.x;
        y = this.y;
    }
    #endregion

    #region vector ops
    public static vec3 operator +(vec3 lhs, vec3 rhs) =>
        new vec3(lhs.x + rhs.x, lhs.y + rhs.y, lhs.z + rhs.z);
    
    public static vec3 operator -(vec3 lhs, vec3 rhs) =>
        new vec3(lhs.x - rhs.x, lhs.y - rhs.y, lhs.z - rhs.z);

    public static vec3 operator *(int s, vec3 a) =>
        new vec3(s * a.x, s * a.y, s * a.z);

    public static vec3 operator *(vec3 a, int s) => s * a;

    public static vec3 operator -(vec3 a) => -1 * a;
    public static vec3 operator +(vec3 a) => a;
    #endregion

    #region apply
    public static vec3 Apply(vec3 a, vec3 b, Func<int, int, int> f) =>
        new vec3(f(a.x, b.x), f(a.y, b.y), f(a.z, b.z));

    public static vec3 Apply(vec3 a, Func<int, int> f) => 
        new vec3(f(a.x), f(a.y), f(a.z));


    public vec3 Apply(Func<int, int> f) => new vec3(f(x), f(y), f(z));

    public static vec3 Apply(IList<vec3> items, Func<vec3, vec3, vec3> f)
    {
        vec3? m = null;
        foreach (var r in items)
        {
            if (m == null) m = r;
            else m = f(m, r);
        }
        return m ?? new vec3();
    }

    #endregion

    #region component and misc ops
    public vec3 Sign() => Apply(Math.Sign);

    public static vec3 Min(vec3 lhs, vec3 rhs) =>
        Apply(lhs,rhs,Math.Min);
    public static vec3 Max(vec3 lhs, vec3 rhs) =>
        Apply(lhs, rhs, Math.Max);
    public static vec3 Max(IList<vec3> items) =>
        Apply(items, Max);
    public static vec3 Min(IList<vec3> items) =>
        Apply(items, Min);
    #endregion

    #region length
    public int ManhattanDistance => Math.Abs(x) + Math.Abs(y) + Math.Abs(z);

    public long LengthSquared => x * x + y * y + z * z;
    #endregion



    #region ordering

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
    #endregion

    #region misc

    public int AbsMax => Math.Max(Math.Abs(x), Math.Max(Math.Abs(y), Math.Abs(z)));

    public static vec3 Clamp(vec3 v, vec3 min, vec3 max) => Min(Max(v, min), max);

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

    #endregion


    public override string ToString() =>
        $"({x},{y},{z})";

    #region swizzles
    // CUDA like reads and writes
    public (int x, int y) xy { get => (x, y); set => (x, y) = value; }
    public (int y, int x) yx { get => (y, x); set => (y, x) = value; }
    public (int y, int z) yz { get => (y, z); set => (y, z) = value; }
    public (int z, int y) zy { get => (z, y); set => (z, y) = value; }
    public (int z, int x) zx { get => (z, x); set => (z, x) = value; }
    public (int x, int z) xz { get => (x, z); set => (x, z) = value; }

    public (int x, int y, int z) xyz { get => (x, y, z); set => (x, y, z) = value; }
    public (int x, int z, int y) xzy { get => (x, z, y); set => (x, z, y) = value; }
    public (int y, int x, int z) yxz { get => (y, x, z); set => (y, x, z) = value; }
    public (int y, int z, int x) yzx { get => (y, z, x); set => (y, z, x) = value; }
    public (int z, int x, int y) zxy { get => (z, x, y); set => (z, x, y) = value; }
    public (int z, int y, int x) zyx { get => (z, y, x); set => (z, y, x) = value; }
    #endregion

    #region Equality, Inequality, Hash code

    //public static bool operator ==(vec3 lhs, vec3 rhs)
    //{
    //    return (lhs - rhs).LengthSquared == 0;
    //}
    //
    //public static bool operator !=(vec3 lhs, vec3 rhs) => !(lhs == rhs);

    public static bool operator ==(vec3? lhs, vec3? rhs)
    {
        if (lhs is null)
        {
            if (rhs is null) return true;
            return false;
        }
        return lhs.Equals(rhs);
    }
    public static bool operator !=(vec3 lhs, vec3 rhs) => !(lhs == rhs);

    public override int GetHashCode()
    { // Optimized Spatial Hashing for Collision Detection of Deformable objects
      // http://www.beosil.com/download/CollisionDetectionHashing_VMV03.pdf
      // three large primes from paper: 73856093, 19349663, 83492791
      // can take mod N for a size N hash table
      return (x * 73856093) ^ (y * 19349663) ^ (z * 83492791);
    }

    public bool Equals(vec3? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        if (GetType() != other.GetType()) return false; // exact same type
        return (this-other).LengthSquared==0; // finally, check values
    }

    public override bool Equals(object? obj) => Equals(obj as vec3);

    #endregion


}