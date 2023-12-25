using Microsoft.VisualBasic.CompilerServices;

namespace Lomont.AdventOfCode.Utils
{
    public class vec4 : IEquatable<vec4>
    {
        #region constants
        public static vec4 MaxValue = new vec4(int.MaxValue, int.MaxValue, int.MaxValue, int.MinValue);
        public static vec4 MinValue = new vec4(int.MinValue, int.MinValue, int.MinValue, int.MinValue);

        public static vec4 XAxis = new vec4(1, 0, 0,0);
        public static vec4 YAxis = new vec4(0, 1, 0,0);
        public static vec4 ZAxis = new vec4(0, 0, 1,0);
        public static vec4 WAxis = new vec4(0, 0, 0,1);
        public static vec4 Zero  = new vec4(0, 0, 0,0);
        public static vec4 One   = new vec4(1, 1, 1,1);
        #endregion

        #region constructors
        public vec4(int w = 0, int x = 0, int y = 0, int z = 0)
        {
            this.w = w;
            this.x = x;
            this.y = y;
            this.z = z;
    }
        #endregion

        #region field access 
        int[] vals = new int[4];

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
        public int w
        {
            get => vals[3];
            set { vals[3] = value; }
        }

        public void Deconstruct(out int x, out int y, out int z, out int w)
        {
            x = this.x;
            y = this.y;
            z = this.z;
            w = this.w;
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

        public int this[int index]
        {
            get => vals[index];
            set => vals[index] = value;
        }

        #endregion

        #region vector ops
        public static vec4 operator +(vec4 a, vec4 b) =>
            new(a.w + b.w, a.x + b.x, a.y + b.y, a.z + b.z);

        public static vec4 operator -(vec4 a, vec4 b) =>
            new(a.w - b.w, a.x - b.x, a.y - b.y, a.z - b.z);

        public static vec4 operator *(int s, vec4 a) => new(s * a.w, s * a.x, s * a.y, s * a.z);
        public static vec4 operator *(vec4 a, int s) => s*a;

        public static vec4 operator -(vec4 a) => -1 * a;
        public static vec4 operator +(vec4 a) => a;

        #endregion

        #region apply
        public static vec4 Apply(vec4 a, vec4 b, Func<int, int, int> f) =>
            new vec4(f(a.w, b.w), f(a.x, b.x), f(a.y, b.y), f(a.z, b.z));

        public static vec4 Apply(vec4 a, Func<int, int> f) => 
            new vec4(f(a.w), f(a.x), f(a.y), f(a.z));

        public vec4 Apply(Func<int, int> f) => new vec4(f(w), f(x), f(y), f(z));

        public static vec4 Apply(IList<vec4> items, Func<vec4,vec4,vec4> f)
        {
            vec4? m = null;
            foreach (var r in items)
            {
                if (m == null) m = r;
                else m = f(m, r);
            }
            return m ?? new vec4();
        }



        #endregion

        #region component and misc ops
        public static vec4 Max(vec4 a, vec4 b) => Apply(a, b, Math.Max);
        public static vec4 Min(vec4 a, vec4 b) => Apply(a, b, Math.Min);
        public static vec4 Max(IList<vec4> items) =>
            Apply(items,Max);

        public static vec4 Min(IList<vec4> items) =>
            Apply(items, Min);
        #endregion


        #region Length
        public int ManhattanDistance => Math.Abs(x) + Math.Abs(y) + Math.Abs(z) + Math.Abs(w);
        public int LengthSquared => x * x + y * y + z * z + w * w;
        #endregion

        #region ordering
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
        #endregion

        #region misc
        public int AbsMax => Math.Max(Math.Abs(w),Math.Max(Math.Abs(x), Math.Max(Math.Abs(y), Math.Abs(z))));

        public static vec4 Clamp(vec4 v, vec4 min, vec4 max) => Min(Max(v, min), max);

        #endregion

        public override string ToString() =>
            $"({x},{y},{z},{w})";

        #region swizzles

        #endregion

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
