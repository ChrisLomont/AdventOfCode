

using System.Net.NetworkInformation;

namespace Lomont.AdventOfCode._2024
{
    internal class Day08 : AdventOfCode
    {
        /*
         * 2024 Day 8 part 1: 369 in 5180.9 us
2024 Day 8 part 2: 1169 in 1167.5 us

              --------Part 1--------   --------Part 2--------
Day       Time   Rank  Score       Time   Rank  Score
  8   00:07:11    350      0   00:12:51    428      0
  7   04:10:12  18038      0   04:28:59  16919      0
  6   04:23:59  25434      0   04:56:30  14415      0
  5   07:20:12  38165      0   07:23:07  29704      0
  4   06:38:36  38107      0   06:42:27  32032      0
  3   05:57:52  48687      0   06:01:05  39015      0
  2   05:31:05  44125      0   05:33:59  29570      0
  1   14:02:44  76111      0   14:04:39  70409      0


         */

        //public override string TestFileSuffix() => "-test1";
        public override string TestFileSuffix() => "";

        public override object Run(bool part2)
        {
            var (w, h, g) = CharGrid();
            Dictionary<char, List<vec2>> pts = new();
            Apply(g, (i, j, c) =>
            {
                if (c != '.')
                {
                    if (!pts.ContainsKey(c))
                        pts.Add(c, new());
                    pts[c].Add(new(i, j));
                }
                return c;
            });

            HashSet<vec2> nodes = new();
            foreach (var p in pts.Values)
            {
                for (int i = 0; i < p.Count; i++)
                for (int j = i + 1; j < p.Count; j++)
                {
                    var a = p[i];
                    var b = p[j];
                    var del = b - a;
                    if (!part2)
                    {
                        var n1 = a - del;
                        var n2 = b + del;
                        if (legal(n1))
                            nodes.Add(n1);
                        if (legal(n2))
                            nodes.Add(n2);
                    }
                    else
                    {
                        var (dx, dy) = del;

                        var d = NumberTheory.GCD(Math.Abs(dx), Math.Abs(dy));
                        del = new vec2((int)(dx / d), (int)(dy / d));

                        vec2 d1 = a;
                        while (legal(d1))
                        {
                            nodes.Add(d1);
                            d1 += del;
                        }

                        d1 = a;
                        while (legal(d1))
                        {
                            nodes.Add(d1);
                            d1 -= del;
                        }
                    }


                }
            }

            bool legal(vec2 p)
            {
                return (0 <= p.x && 0 <= p.y && p.x < w && p.y < h);
            }

            return nodes.Count;
        }
    }
}