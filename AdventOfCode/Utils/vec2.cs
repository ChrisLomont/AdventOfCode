namespace Lomont.AdventOfCode.Utils
{
    internal class vec2
#if false
        : vecN<int>
    {
        public vec2(int x = 0, int y = 0) : base(2, x, y)
        {
        }
#else
        : IEquatable<vec2>
    {


        #region constants
        public static vec2 MaxValue = new vec2(int.MaxValue, int.MaxValue);
        public static vec2 MinValue = new vec2(int.MinValue, int.MinValue);

        public static vec2 XAxis = new vec2(1, 0);
        public static vec2 YAxis = new vec2(0, 1);
        public static vec2 Zero = new vec2(0, 0);
        public static vec2 One = new vec2(1, 1);
        #endregion

        #region constructors
        public vec2(int x = 0, int y = 0)
        {
            this.x = x;
            this.y = y;
        }
        #endregion

        #region field access
        int[] vals = new int[2];

        public int this[int i]
        {
            get => i switch { 0 => x, 1 => y, _ => throw new IndexOutOfRangeException() };
            set
            {
                if (i == 0) x = value;
                else if (i == 1) y = value;
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


        public void Deconstruct(out int x, out int y)
        {
            x = this.x;
            y = this.y;
        }
        #endregion

        #region vector ops
        public static vec2 operator +(vec2 lhs, vec2 rhs) =>
            new vec2(lhs.x + rhs.x, lhs.y + rhs.y);

        public static vec2 operator -(vec2 lhs, vec2 rhs) =>
            new vec2(lhs.x - rhs.x, lhs.y - rhs.y);

        public static vec2 operator *(int s, vec2 a) =>
            new vec2(s * a.x, s * a.y);

        public static vec2 operator *(vec2 a, int s) => s * a;

        public static vec2 operator -(vec2 a) => -1 * a;
        public static vec2 operator +(vec2 a) => a;
        #endregion

        #region apply
        public static vec2 Apply(vec2 a, vec2 b, Func<int, int, int> f) =>
            new vec2(f(a.x, b.x), f(a.y, b.y));

        public static vec2 Apply(vec2 a, Func<int, int> f) =>
            new vec2(f(a.x), f(a.y));


        public vec2 Apply(Func<int, int> f) => new vec2(f(x), f(y));

        public static vec2 Apply(IList<vec2> items, Func<vec2, vec2, vec2> f)
        {
            vec2? m = null;
            foreach (var r in items)
            {
                if (m == null) m = r;
                else m = f(m, r);
            }
            return m ?? new vec2();
        }

        #endregion

        #region component and misc ops
        public vec2 Sign() => Apply(Math.Sign);

        public static vec2 Min(vec2 lhs, vec2 rhs) =>
            Apply(lhs, rhs, Math.Min);
        public static vec2 Max(vec2 lhs, vec2 rhs) =>
            Apply(lhs, rhs, Math.Max);
        public static vec2 Max(IList<vec2> items) =>
            Apply(items, Max);
        public static vec2 Min(IList<vec2> items) =>
            Apply(items, Min);
        #endregion

        #region length
        public int ManhattanDistance => Math.Abs(x) + Math.Abs(y);

        public long LengthSquared => x * x + y * y;
        #endregion

        #region ordering

        // each left component <= corresponding right
        public static bool operator <=(vec2 lhs, vec2 rhs)
        {
            return
                lhs.x <= rhs.x &&
                lhs.y <= rhs.y; 
        }
        public static bool operator >=(vec2 lhs, vec2 rhs)
        {
            return
                lhs.x >= rhs.x &&
                lhs.y >= rhs.y;
        }
        #endregion

        #region misc

        public int AbsMax => Math.Max(Math.Abs(x), Math.Abs(y));

        public static vec2 Clamp(vec2 v, vec2 min, vec2 max) => Min(Max(v, min), max);

        //apply rotations, return new vec
        public vec2 RotXYZ(int rx, int ry, int rz)
        {
            var (x1, y1) = (x, y);
            while (rz-- > 0)
                (x1, y1) = (-y1, x1);

            return new vec2(x1, y1);
        }

        #endregion


        public override string ToString() =>
            $"({x},{y})";

        #region swizzles
        // CUDA like reads and writes
        public (int x, int y) xy { get => (x, y); set => (x, y) = value; }
        public (int y, int x) yx { get => (y, x); set => (y, x) = value; }

        #endregion

        // 2d "cross" product, is z component of 3D cross product
        public static int Cross(vec2 a, vec2 b) => a.x * b.y - a.y * b.x;

        #region Equality, Inequality, Hash code

        //public static bool operator ==(vec2 lhs, vec2 rhs)
        //{
        //    return (lhs - rhs).LengthSquared == 0;
        //}
        //
        //public static bool operator !=(vec2 lhs, vec2 rhs) => !(lhs == rhs);

        public static bool operator ==(vec2? lhs, vec2? rhs)
        {
            if (lhs is null)
            {
                if (rhs is null) return true;
                return false;
            }
            return lhs.Equals(rhs);
        }
        public static bool operator !=(vec2 lhs, vec2 rhs) => !(lhs == rhs);

        public override int GetHashCode()
        { // Optimized Spatial Hashing for Collision Detection of Deformable objects
          // http://www.beosil.com/download/CollisionDetectionHashing_VMV03.pdf
          // three large primes from paper: 73856093, 19349663, 83492791
          // can take mod N for a size N hash table
            return (x * 73856093) ^ (y * 19349663);
        }

        public bool Equals(vec2? other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            if (GetType() != other.GetType()) return false; // exact same type
            return (this - other).LengthSquared == 0; // finally, check values
        }

        public override bool Equals(object? obj) => Equals(obj as vec2);

        #endregion
#endif
    }
}
