using System.Collections;

// todo - move to Lomont algorithms and data structures
namespace Lomont.AdventOfCode.Utils
{
    public class Set<T> : IEnumerable<T>, IEquatable<Set<T>>

    /*
     todo - do all :)
    System.Collections.Generic.ICollection<T>, 
    X System.Collections.Generic.IEnumerable<T>, 
    System.Collections.Generic.IReadOnlyCollection<T>, 
    System.Collections.Generic.IReadOnlySet<T>, 
    System.Collections.Generic.ISet<T>, 
    System.Runtime.Serialization.IDeserializationCallback, 
    System.Runtime.Serialization.ISerializable     
     */

    {
        /*
        Figure out how to emulate nicely things like &=, |=, etc. in place operations

        
        nice constructors
        add, 
        addrange

        X Intersection, &
        X Union, |
        X Difference, - 
        X SymmetricDifference ^

        X enumerate, 
        X count, 
        X contains, 
        
        X subset <=, >=
        X strict subset <, >
        X equals == 
         */
        #region Constructors
        public Set(params T[] items)
        {
            foreach (var i in items)
                Add(i);
        }

        public Set(IEnumerable<T> items)
        {
            foreach (var i in items)
                Add(i);
        }
        #endregion

        #region Add, Remove, Contains, Count...
        public void Add(T item) => values.Add(item);
        public void Remove(T item) => values.Remove(item);

        public bool Contains(T item) => values.Contains(item);
        public int Count => values.Count;
        #endregion

        public void Clear() => values.Clear();
        public Set<T> Clone()
        {
            var s = new Set<T>();
            foreach (var i in values)
                s.Add(i);
            return s;
        }

        #region Intersection

        public static Set<T> operator &(Set<T> lhs, Set<T> rhs) => Intersection(lhs, rhs);
        public static Set<T> Intersection(params IEnumerable<T>[] sets) => IntersectHelp(sets);
        public static Set<T> Intersection(IEnumerable<Set<T>> sets) => IntersectHelp(sets);
        static Set<T> IntersectHelp(IEnumerable<IEnumerable<T>> sets)
        {
            Set<T>? ans = null;
            foreach (var s in sets)
            {
                if (ans == null) ans = new Set<T>(s);
                else
                    ans.values.IntersectWith(s);
            }
            return ans!;
        }
        #endregion

        #region Union
        public static Set<T> operator |(Set<T> lhs, Set<T> rhs) => Union(lhs, rhs);
        public static Set<T> Union(params IEnumerable<T>[] sets) => UnionHelp(sets);
        public static Set<T> Union(IEnumerable<Set<T>> sets) => UnionHelp(sets);
        static Set<T> UnionHelp(IEnumerable<IEnumerable<T>> sets)
        {
            Set<T>? ans = null;
            foreach (var s in sets)
            {
                if (ans == null) ans = new Set<T>(s);
                else
                    ans.values.UnionWith(s);
            }
            return ans!;
        }
        #endregion

        #region Difference
        public static Set<T> operator -(Set<T> lhs, Set<T> rhs) => Difference(lhs, rhs);
        public static Set<T> Difference(params IEnumerable<T>[] sets) => DifferenceHelp(sets);
        public static Set<T> Difference(IEnumerable<Set<T>> sets) => DifferenceHelp(sets);
        static Set<T> DifferenceHelp(IEnumerable<IEnumerable<T>> sets)
        {
            Set<T>? ans = null;
            foreach (var s in sets)
            {
                if (ans == null) ans = new Set<T>(s);
                else
                    ans.values.ExceptWith(s);
            }
            return ans!;
        }
        #endregion

        #region SymmetricDifference
        public static Set<T> operator ^(Set<T> lhs, Set<T> rhs) => SymmetricDifference(lhs, rhs);

        public static Set<T> SymmetricDifference(Set<T> lhs, Set<T> rhs)
        {
            var ans = new Set<T>(lhs);
            ans.values.SymmetricExceptWith(rhs);
            return ans;
        }
        #endregion

        #region Subset, Superset

        public static bool operator <=(Set<T> lhs, Set<T> rhs) => lhs.IsSubsetOf(rhs);
        public static bool operator >=(Set<T> lhs, Set<T> rhs) => rhs.IsSubsetOf(lhs);
        public static bool operator <(Set<T> lhs, Set<T> rhs) => lhs.IsProperSubsetOf(rhs);
        public static bool operator >(Set<T> lhs, Set<T> rhs) => rhs.IsProperSubsetOf(lhs);


        public bool IsSubsetOf(IEnumerable<T> items) => values.IsSubsetOf(items);
        public bool IsProperSubsetOf(IEnumerable<T> items) => values.IsProperSubsetOf(items);

        #endregion

        #region Equality, Inequality, Hash code

        public static bool operator ==(Set<T>? lhs, Set<T>? rhs)
        {
            if (lhs is null)
            {
                if (rhs is null) return true;
                return false;
            }
            return lhs.Equals(rhs);
        }
        public static bool operator !=(Set<T> lhs, Set<T> rhs) => !(lhs == rhs);

        public override int GetHashCode() => values.GetHashCode();
        public bool Equals(Set<T>? other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            if (GetType() != other.GetType()) return false; // exact same type
            return values.SetEquals(other.values); // finally, check values
        }

        public override bool Equals(object? obj) => Equals(obj as Set<T>);

        #endregion


        #region Interfaces
        public IEnumerator<T> GetEnumerator()
        {
            return values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #region Implementation
        private readonly HashSet<T> values = new();

        #endregion

        // Simple test
        public static void TestSet()
        {
            var i1 = new Set<int>(1, 2, 3, 4);
            var i2 = new Set<int>(2, 4, 6, 7, 8);
            var i3 = new Set<int>(i1);
            Assert(i1.Count == 4);
            Assert(i2.Count == 5);
            Assert(i3.Count == 4);

            Assert(i1 == i3);
            i3.Remove(10);
            Assert(i3.Count == 4);
            i3.Add(2);
            Assert(i3.Count == 4);
            i3.Remove(1);
            Assert(i3.Count == 3);
            Assert(i1 != i3);
            i3.Add(11);
            Assert(i3.Count == 4);
            Assert(i1 != i3);
            i3.Remove(11);
            i3.Add(1);
            Assert(i1 == i3);

            var i4 = Set<int>.Union(i1, i2);
            var i5 = i1 | i2;
            var i6 = new Set<int>(5, 9, 10);
            var i7 = Set<int>.Union(new List<Set<int>>() { i1, i2, i3, i4, i5, i6 });
            Assert(i4.Count == 7); // 1,2,3,4,6,7,8
            Assert(i5.Count == 7);
            Assert(i4 == i5);
            Assert(i7.Count == 10);

            Assert(i7.Contains(5));
            Assert(i7.Contains(1));
            Assert(!i7.Contains(11));
            Assert(i7.Contains(2));
            i7.Remove(2);
            Assert(!i7.Contains(2));
            var i8 = i7.Clone();
            i7.Clear();
            Assert(i7.Count == 0);
            Assert(i8.Count == 9);

            var i9 = Set<int>.Intersection(i1, i2); // 2 4
            var i10 = i1 & i2; // 2 4
            var i11 = Set<int>.Intersection(new List<Set<int>>() { i1, i2, i8 }); // 4
            Assert(i9.Count == 2);
            Assert(i10 == i9);
            Assert(i11.Count == 1);
            Assert(i11.Contains(4));

            i1.Add(9);
            i1.Add(12);
            var i12 = i8 - i1;
            var i13 = i1 - i8;
            var i14 = Set<int>.SymmetricDifference(i1, i8); // 2 6 7 8 5 9 10 
            var i15 = i1 ^ i8;
            var i16 = i12 | i13;

            //AdventOfCode.Dump(i1); //1 2 3 4 9 12
            //AdventOfCode.Dump(i8); // 1 3 4 6 7 8 5 9 10

            Assert(i12.Count == 5);
            Assert(i13.Count == 2);
            Assert(i14.Count == 7); // 2 6 7 8 5 10 12
            Assert(i15.Count == 7);
            Assert(i16.Count == 7);
            Assert(i14 == i15);
            Assert(i14 == i16);

            // superset, subsets
            var s1 = new Set<int>(1, 2, 3);
            var s2 = new Set<int>(1, 2, 3, 4);
            var s3 = new Set<int>(1, 2);
            var s4 = new Set<int>(1, 2, 5);
            var s5 = new Set<int>(1, 2, 3);

            Assert(s1 <= s2);
            Assert(s1 < s2);
            Assert(s1 >= s3);
            Assert(s1 > s3);

            Assert(s1 <= s5);
            Assert(!(s1 < s5));
            Assert(s1 >= s5);
            Assert(!(s1 > s5));
            Assert(s1 == s5);

            Assert(!(s1 <= s4));
            Assert(!(s1 < s4));
            Assert(!(s1 >= s4));
            Assert(!(s1 > s4));

            var d1 = new Set<int>(1, 2, 3, 4, 5, 6, 7);
            var d2 = new Set<int>(1, 2, 3);
            var d3 = new Set<int>(3, 4, 5);
            var d4 = new Set<int>(5, 6, 7, 8, 9);
            var d5 = Set<int>.Difference(d1, d2, d3, d4);
            var d6 = Set<int>.Difference(d1, d3, d4);
            Assert(d5.Count == 0);
            Assert(d6.Count == 2);

            var c1 = new Set<char>("bob");
            var c2 = new Set<char>("Bab");
            Assert(c1.Count == 2);
            Assert(c2.Count == 3);
            Assert((c1 | c2).Count == 4);

            void Assert(bool t) => Trace.Assert(t);

            Console.WriteLine("Set test ok");
        }


    }
}
