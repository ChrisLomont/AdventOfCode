using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Lomont.AdventOfCode.Utils
{
    internal class vecN<IntT> where IntT : INumber<IntT>, IBinaryInteger<IntT>
    {
        #region  constants
        // todo
        #endregion

        #region constructors
        public vecN(int N, params IntT[] vals)
        {
            this.vals = new IntT[N];
            for (var i = 0; i < vals.Length; ++i)
                this.vals[i] = vals[i];
        }
        #endregion


        #region field access

        readonly IntT[] vals;
        int Size => vals.Length;

        public IntT this[int i]
        {
            get => vals[i];
            set => vals[i] = value;
        }

        public IntT x
        {
            get => vals[0];
            set { vals[0] = value; }
        }
        public IntT y
        {
            get => vals[1];
            set { vals[1] = value; }
        }
        public IntT z
        {
            get => vals[2];
            set { vals[2] = value; }
        }
        public IntT w
        {
            get => vals[3];
            set { vals[3] = value; }
        }

        public void Deconstruct(out IntT x, out IntT y)
        {
            x = vals[0];
            y = vals[1];
        }
        public void Deconstruct(out IntT x, out IntT y, out IntT z)
        {
            x = vals[0];
            y = vals[1];
            z = vals[2];
        }
        public void Deconstruct(out IntT x, out IntT y, out IntT z, out IntT w)
        {
            x = vals[0];
            y = vals[1];
            z = vals[2];
            w = vals[3];
        }
        #endregion

        #region vector ops

        public static vecN<IntT> operator +(vecN<IntT> lhs, vecN<IntT> rhs) => Apply(lhs, rhs, (a, b) => a + b);

        public static vecN<IntT> operator -(vecN<IntT> lhs, vecN<IntT> rhs) =>  Apply(lhs, rhs, (a, b) => a - b);

        public static vecN<IntT> operator *(IntT s, vecN<IntT> a) =>  Apply(a, v=>s*v);
        
        public static vecN<IntT> operator *(vecN<IntT> a, IntT s) => s * a;

        public static vecN<IntT> operator -(vecN<IntT> a) => (IntT.Zero-IntT.One)*a;
        public static vecN<IntT> operator +(vecN<IntT> a) => a;
        #endregion

        #region apply
        public static vecN<IntT> Apply(vecN<IntT> a, vecN<IntT> b, Func<IntT, IntT, IntT> f)
        {
            var v = new vecN<IntT>(a.Size);
            for (var i = 0; i< a.Size; ++i)
                v[i] = f(a[i], b[i]);
            return v;
        }


        public static vecN<IntT> Apply(vecN<IntT> a, Func<IntT, IntT> f)
        {
            var v = new vecN<IntT>(a.Size);
            for (var i = 0; i < a.Size; ++i)
                v[i] = f(a[i]);
            return v;
        }

        public vecN<IntT> Apply(Func<IntT, IntT> f)
        {
            var v = new vecN<IntT>(Size);
            for (var i = 0; i < Size; ++i)
                v[i] = f(this[i]);
            return v;
        }

        public static vecN<IntT> Apply(IList<vecN<IntT>> items, Func<vecN<IntT>, vecN<IntT>, vecN<IntT>> f)
        {
            vecN<IntT>? m = null;
            foreach (var r in items)
            {
                if (m == null) m = r;
                else m = f(m, r);
            }
            return m ?? new vecN<IntT>(m.Size); // what if no deref?
        }

        #endregion

        #region component and misc ops

        public vecN<IntT> Sign() => Apply(v => v < IntT.Zero ? -IntT.One : (v > IntT.Zero ? IntT.One : IntT.Zero));

        public static vecN<IntT> Min(vecN<IntT> lhs, vecN<IntT> rhs) =>
            Apply(lhs, rhs, IntT.MinMagnitude);
        public static vecN<IntT> Max(vecN<IntT> lhs, vecN<IntT> rhs) =>
            Apply(lhs, rhs, IntT.MaxMagnitude);
        public static vecN<IntT> Max(IList<vecN<IntT>> items) =>
            Apply(items, Max);
        public static vecN<IntT> Min(IList<vecN<IntT>> items) =>
            Apply(items, Min);
        #endregion

        #region length
        public IntT ManhattanDistance => vals.Select(v => IntT.Abs(v)).Aggregate(IntT.Zero, (a, b) => a + b);

        public IntT LengthSquared => vals.Select(v => v * v).Aggregate(IntT.Zero, (a, b) => a + b);
        #endregion

        #region ordering

        // each left component <= corresponding right
        public static bool operator <=(vecN<IntT> lhs, vecN<IntT> rhs)
        {
            return
                lhs.x <= rhs.x &&
                lhs.y <= rhs.y;
        }
        public static bool operator >=(vecN<IntT> lhs, vecN<IntT> rhs)
        {
            return
                lhs.x >= rhs.x &&
                lhs.y >= rhs.y;
        }
        #endregion

        #region misc

        public IntT AbsMax => vals.Max(v => IntT.Abs(v));
        public IntT AbsMin => vals.Min(v => IntT.Abs(v));

        public static vecN<IntT> Clamp(vecN<IntT> v, vecN<IntT> min, vecN<IntT> max) => Min(Max(v, min), max);

        #endregion


        public override string ToString()
        {
            var s = new StringBuilder();
            s.Append("(");
            for (int i = 0; i < vals.Length; ++i)
            {
                if (i != 0)
                    s.Append(',');
                s.Append(vals[i]);
            }

            s.Append(")");
            return s.ToString();
        }

        #region swizzles
        // todo?

        #endregion

        #region Equality, Inequality, Hash code

        //public static bool operator ==(vec2 lhs, vec2 rhs)
        //{
        //    return (lhs - rhs).LengthSquared == 0;
        //}
        //
        //public static bool operator !=(vec2 lhs, vec2 rhs) => !(lhs == rhs);

        public static bool operator ==(vecN<IntT>? lhs, vecN<IntT>? rhs)
        {
            if (lhs is null)
            {
                if (rhs is null) return true;
                return false;
            }
            return lhs.Equals(rhs);
        }
        public static bool operator !=(vecN<IntT> lhs, vecN<IntT> rhs) => !(lhs == rhs);

        public override int GetHashCode()
        { // Optimized Spatial Hashing for Collision Detection of Deformable objects
          // http://www.beosil.com/download/CollisionDetectionHashing_VMV03.pdf
          // three large primes from paper: 73856093, 19349663, 83492791
          // can take mod N for a size N hash table
          // todo - GIFMaker IThreadPoolWorkItem again well?
          //  return (x * 73856093) ^ (y * 19349663);
          //int hash = 0;
          IntT hash = IntT.Zero;
          foreach (var v in vals)
          {
              hash *= IntT.One;// 12347; // prime, not really special :|;
              hash += v;//Int32.CreateTruncating(v);
          }

          return Convert.ToInt32(hash);
        }

        public bool Equals(vecN<IntT>? other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            if (GetType() != other.GetType()) return false; // exact same type
            return (this - other).LengthSquared == IntT.Zero; // finally, check values
        }

        public override bool Equals(object? obj) => Equals(obj as vec2);

        #endregion


    }
}
