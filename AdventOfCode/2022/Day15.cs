
namespace Lomont.AdventOfCode._2022
{
    internal class Day15 : AdventOfCode
    {
        //Day Time    Rank Score       Time Rank  Score
        //15   00:21:41     758      0   01:02:06    1273      0
        //14   00:29:48    1766      0   00:49:13    2658      0

        // rank 1-100 12:08 - 27:14 both
        // 3:06-10:40 first

        //2022 Day 15 part 1: 5125700 in 327075.1 us
        //2022 Day 15 part 2: 11379394658764 in 1875351.3 us

        public override object Run(bool part2)
        {
            HashSet<vec3> beacons = new();
            var pairs = new List<(vec3 s, vec3 b)>();
            foreach (var line in ReadLines()) 
            {
                var n = Numbers(line);
                var sensor = new vec3(n[0], n[1]);
                var beacon = new vec3(n[2], n[3]);
                pairs.Add((sensor,beacon));
                beacons.Add(beacon);
            }

            if (!part2)
            {
                int y = 2_000_000;

                var ranges = pairs.Select(p => GetRange(p, y)).Where(p => p.ok).ToList();

                var xMin = ranges.Min(r => r.a);
                var xMax = ranges.Max(r => r.b);

                var yRow = new bool[xMax - xMin + 1];
                foreach (var r in ranges)
                {
                    for (var i = r.a; i <= r.b; ++i)
                        yRow[i - xMin] = !beacons.Contains(new vec3(i, y));
                }
                return yRow.Sum(c => c ? 1 : 0);
            }

            // part2
                int tx = 0, ty = 0;
                bool solved = false;
                var s = 4_000_000;

                Recurse(new vec3(), new vec3(s, s));

                return (long)tx * 4_000_000 + ty;

                void Recurse(vec3 min, vec3 max)
                {
                    if (solved) return;
                    if (Contained(min, max))
                        return;
                    if (max == min)
                    {
                        if (!beacons.Contains(max))
                        {
                            (tx, ty, _) = max;
                            // Console.WriteLine($"Solved {tx} {ty}");
                            solved = true;
                        }

                        return;
                    }

                    var (x0, y0, _) = min;
                    var (x2, y2, _) = max;
                    var x1 = (x0 + x2) / 2;
                    var y1 = (y0 + y2) / 2;
                    Recurse(new vec3(x0, y0), new vec3(x1, y1));
                    Recurse(new vec3(x0, y1 + 1), new vec3(x1, y2));
                    Recurse(new vec3(x1 + 1, y0), new vec3(x2, y1));
                    Recurse(new vec3(x1 + 1, y1 + 1), new vec3(x2, y2));
                }



                bool Contained(vec3 min, vec3 max)
            {
                var p2 = new vec3(min.x, max.y);
                var p3 = new vec3(max.x, min.y);
                foreach (var p in pairs)
                {
                    var d = D(p.s, p.b);
                    var c1 = D(p.s, min);
                    var c2 = D(p.s, p2);
                    var c3 = D(p.s, max);
                    var c4 = D(p.s, p3);
                    //Console.WriteLine($"{d} {c1} {c3} {min} {max} {p.s} {p.b}");
                    if (c1 <= d && c3 <= d && c2 <= d && c4 <= d)
                        return true;
                }
                return false;
            }

            (bool  ok, int a, int b) GetRange((vec3 s,vec3 b) p,int y)
            {
                var h = Math.Abs(p.s.y - y);
                var d = D(p.s, p.b);
                if (h >= d)
                    return (false, 0, 0); // no intersection
                var ex = d - h;
                return (true, p.s.x - ex, p.s.x + ex);


            }

            int D(vec3 a, vec3 b)
            {
                var (dx, dy, _) = (a - b);
                return Math.Abs(dx) + Math.Abs(dy);

            }
        }
    }
}