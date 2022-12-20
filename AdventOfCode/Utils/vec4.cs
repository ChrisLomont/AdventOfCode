namespace Lomont.AdventOfCode.Utils
{
    public class vec4 : IEquatable<vec4>
    {
        public int w, x, y, z;
        public vec4(int w = 0, int x = 0, int y = 0, int z = 0)
        {
            this.w = w;
            this.x = x;
            this.y = y;
            this.z = z;
    }
    public static bool operator <=(vec4 a, vec4 b)
        {
            return
                a.w <= b.w &&
                a.x <= b.x &&
                a.y <= b.y &&
                a.z <= b.z
                ;
        }

        public static bool operator >=(vec4 a, vec4 b) => !(a <= b) || (a == b);

        public static vec4 operator +(vec4 a, vec4 b) =>
            new(a.w + b.w, a.x + b.x, a.y + b.y, a.z + b.z);

        public static vec4 operator -(vec4 a, vec4 b) =>
            new(a.w - b.w, a.x - b.x, a.y - b.y, a.z - b.z);

        public static vec4 operator *(int s, vec4 a) => new(s * a.w, s * a.x, s * a.y, s * a.z);

        public static vec4 Apply(vec4 a, vec4 b, Func<int, int, int> f) =>
            new vec4(f(a.w, b.w), f(a.x, b.x), f(a.y, b.y), f(a.z, b.z));

        public static vec4 Apply(vec4 a, Func<int, int> f) => new vec4(f(a.w), f(a.x), f(a.y), f(a.z));

        public static vec4 Max(vec4 a, vec4 b) => Apply(a, b, Math.Max);
        public static vec4 Min(vec4 a, vec4 b) => Apply(a, b, Math.Min);

        public static vec4 Max(IList<vec4> items)
        {
            vec4? m = null;
            foreach (var r in items)
            {
                if (m == null) m = r;
                else m = Max(m, r);
            }

            return m ?? new vec4();
        }

        public int LengthSquared => x * x + y * y + z * z + w * w;

        public static vec4 Clamp(vec4 v, vec4 min, vec4 max) => Min(Max(v, min), max);


        #region Equality, Inequality, Hash code


        public static bool operator ==(vec4? lhs, vec4? rhs)
        {
            if (lhs is null)
            {
                if (rhs is null) return true;
                return false;
            }
            return lhs.Equals(rhs);
        }
        public static bool operator !=(vec4 lhs, vec4 rhs) => !(lhs == rhs);

        public override int GetHashCode()
        { // Optimized Spatial Hashing for Collision Detection of Deformable objects
            // http://www.beosil.com/download/CollisionDetectionHashing_VMV03.pdf
            // three large primes(not!) from paper: 73856093, 19349663=41*471943, 83492791,
            // can take mod N for a size N hash table
            // i picked proper primes: 73856093, 19349669, 83492791, 12312347
            return (x * 73856093) ^ (y * 19349669) ^ (z * 83492791) ^ (w* 12312347);
        }

        public bool Equals(vec4? other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            if (GetType() != other.GetType()) return false; // exact same type
            return (this - other).LengthSquared == 0; // finally, check values
        }

        public override bool Equals(object? obj) => Equals(obj as vec3);

        #endregion


    }
}
