
//  383
// 9854

using Lomont.AdventOfCode.Utils;

namespace Lomont.AdventOfCode._2021
{
    internal class Day19 : AdventOfCode
    {

        class Scanner : List<vec3>
        {
            public bool aligned = false;
            private static int ids = 0;
            public int id = 0;

            public Scanner()
            {
                id = ++ids;
            }

            public override string ToString()
            {
                return "Scanner: " + id;
            }

            public vec3 center = new vec3(0, 0, 0);
        }
        public override object Run(bool part2)
        {
            var scanners = new List<Scanner>();

            foreach (var line in ReadLines())
            {
                if (line.Contains("scanner"))
                {
                    scanners.Add(new());
                }

                if (line.Contains(','))
                {
                    var n = GetNumbers(line);
                    scanners.Last().Add(new vec3(n[0], n[1], n[2]));
                }
            }

            var len = scanners.Count;
            scanners[0].aligned = true; // make this the default one
            while (scanners.Any(s=>!s.aligned))
            {
                for (var i = 0; i < len; ++i)
                {
                    if (!scanners[i].aligned)
                        continue;
                    for (var j = 0; j < len; ++j)
                    {
                        if (i == j) continue;
                        if (scanners[j].aligned) continue;
                        var distsInCommon = Dists(scanners[i], scanners[j]);
                        if (distsInCommon >= 12)
                        {
                            //Console.WriteLine($"{i},{j}->{distsInCommon}");
                            //scanners[j].aligned = true;
                            scanners[j].aligned = TryAlign(scanners[i], scanners[j]);
                            //if (scanners[j].aligned)
                            //    Console.WriteLine($"Aligned {i} to {j}");
                        }
                    }
                }
            }

            HashSet<string> pts = new();
            foreach (var s in scanners)
            foreach (var p in s)
                pts.Add(p.ToString());

            if (part2)
            {
                var mhd = new List<long>();
                foreach (var s1 in scanners)
                foreach (var s2 in scanners)
                {
                    mhd.Add(MH(s1.center- s2.center));
                }

                return mhd.Max();

                long MH(vec3 d)
                {
                    return Math.Abs(d.x) + Math.Abs(d.y) + Math.Abs(d.z);
                }


            }
            return pts.Count; // 768 too high


        }

        private int Dists(Scanner a, Scanner b)
        {
            // dists in common
            var da = Dists(a);
            var db = Dists(b);
            var dd = da.Intersect(db).ToList();
            return dd.Count;

        }

        HashSet<long> Dists(Scanner a)
        {
            var len = a.Count;
            HashSet<long> dists = new();
            for (var i = 0; i < len; ++i)
            for (var j = i + 1; j < len; ++j)
                dists.Add((a[i] - a[j]).LengthSquared);
            return dists;
        }
#if true

        // try to align dst to src by moving dst
        bool TryAlign(Scanner src , Scanner dst)
        {
            var da = Dists(src);
            var db = Dists(dst);
            var dd = da.Intersect(db);

            // find example pairs
            foreach (var d in dd)
                foreach (var (i1,j1) in Dists(src,d))
                foreach (var (i2, j2) in Dists(dst, d))
                    if (Align((src,i1,j1),(dst,i2,j2)))
                        return true;
            return false;
        }

        IEnumerable<(int i, int j)> Dists(Scanner s, long dist)
        {
            var len = s.Count;
            for (var i = 0; i < len; ++i)
            for (var j = i + 1; j < len; ++j)
                if ((s[i] - s[j]).LengthSquared == dist)
                    yield return (i,j);
        }

        // align 2nd to first if possible, return true if done
        private bool Align((Scanner sc, int i, int j) src, (Scanner sc, int i, int j) dst)
        {
            bool show = false;
            var sc1 = src.sc;
            var sc2 = dst.sc;
            var a1 = sc1[src.i];
            var b1 = sc1[src.j];
            var a2 = sc2[dst.i];
            var b2 = sc2[dst.j];

            var d1 = a1 - b1;
            var d2 = a2 - b2;
            Trace.Assert(d1.LengthSquared == d2.LengthSquared);

            var l1 = new List<int> {d1.x, d1.y, d1.z};
            var l2 = new List<int> { d2.x, d2.y, d2.z };
            l1 = l1.Select(a => Math.Abs(a)).Order().ToList();
            l2 = l2.Select(a => Math.Abs(a)).Order().ToList();
            if (l1[0] == l1[1] || l1[0] == l1[2] || l1[1] == l1[2])
                return false; // not unique
            if (l1[0] != l2[0] || l1[1] != l2[1] || l1[2] != l2[2])
                return false; // not same

            // try both orientations of dirs
            for (var pass = 0; pass < 2; ++pass)
            {
                HashSet<long> seen = new();
                for (var rx = 0; rx < 4; ++rx)
                for (var ry = 0; ry < 4; ++ry)
                for (var rz = 0; rz < 4; ++rz)
                {
                    var d3 = d2.RotXYZ(rx, ry, rz);
                    var d = (d3.x + 1000) * 5000 * 5000 + (d3.y + 1000) * 5000 + (d3.z + 1000);
                    seen.Add(d);
                    if (d3 == d1)
                    {
                        if (show)
                            Console.WriteLine($"Matched {d1} {d2} {d3}");


                        var cnt1 = Intersection(sc2, sc2);
                        if (show)
                            Console.WriteLine($"Intersection mid {Intersection(sc1, sc2)} [{cnt1}]");
                        Apply(rx, ry, rz, b2, b1, sc2);
                        var cnt2 = Intersection(sc1, sc2);

                        if (show)
                            Console.WriteLine($"Intersection mid {Intersection(sc1, sc2)}");

                        if (cnt2 >= 12) // threshold in problem
                        {
                            Trace.Assert(a1 == sc2[dst.i]);
                            Trace.Assert(b1 == sc2[dst.j]);

                            dst.sc.aligned = true;
                            return true;
                        }
                        else
                        {

                            Invert(rx, ry, rz, b2, b1, sc2);
                            var cnt3 = Intersection(sc2, sc2);
                            Trace.Assert(cnt1 == cnt3 && cnt1 == sc2.Count);
                            if (show)
                                Console.WriteLine($"Intersection post {Intersection(sc1, sc2)} [{cnt3}]");
                        }

                    }
                }

                Trace.Assert(seen.Count == 24);
                //(a2, b2) = (b2, a2);
                //d2 = a2 - b2;
                (a1, b1) = (b1, a1);
                d1 = a1 - b1;
            }
            if (show)
                Console.WriteLine($"No match {d1} {d2}");

            return false;

        }

        int Intersection(Scanner sc1, Scanner sc2)
        {
            int count = 0;
            foreach (var p in sc1)
                foreach (var q in sc2)
                    if (p == q)
                        count++;
            return count;


        }

        // sub v1 from all, apply rotation, add v2
        void Apply(int rx, int ry, int rz, vec3 v1, vec3 v2, Scanner scanner)
        {
            var vecs = scanner.Select(
                s => 
                    (s - v1).RotXYZ(rx,ry,rz)+v2
                    ).ToList();
            scanner.Clear();
            scanner.AddRange(vecs);
            scanner.center = (scanner.center - v1).RotXYZ(rx,ry,rz) + v2;
        }

        void Invert(int rx, int ry, int rz, vec3 v1, vec3 v2, Scanner scanner)
        {
            var zero = new vec3(0,0,0);
            // invert each step
            Apply(0, 0, 4 - (rz % 4), v2, zero, scanner);
            Apply(0, 4 - (ry % 4), 0, zero, zero, scanner);
            Apply(4 - (rx % 4), 0, 0, zero, v1, scanner);
            Trace.Assert(scanner.center == new vec3(0,0,0));
        }
#endif

    }
}