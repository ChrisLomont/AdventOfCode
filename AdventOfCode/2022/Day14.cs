
namespace Lomont.AdventOfCode._2022
{
    internal class Day14 : AdventOfCode
    {
        // rank 1-100: 5:56 - 13:54 both
        // rank 1-100: 4:53-10:33 single

        // 2022 Day 14 part 1: 828 in 22278.2 us
        // 2022 Day 14 part 2: 25500 in 66796.5 us

        //14   00:29:48    1766      0   00:49:13    2658      0
        //13   00:34:29    2304      0   00:39:48    1837      0

        public override object Run(bool part2)
        {
            long score = 0;
            var lines = ReadLines();
            List<int> nums = new();
            List<List<vec3>> paths = new();
            var min = new vec3(Int32.MaxValue,int.MaxValue);
            var max = new vec3(int.MinValue,int.MinValue);
            for (var index = 0; index < lines.Count; ++index)
            {
                var line = lines[index];
                var n = Numbers(line);
                var vecs = n.Chunk(2).Select(p => new vec3(p[0], p[1]))
                    .ToList();
                min = vecs.Aggregate(min, (a, b) => vec3.Min(a, b));
                max = vecs.Aggregate(max, (a, b) => vec3.Max(a, b));

                paths.Add(vecs);;
                nums.AddRange(n);
            }

            min.y = 0;

            var del = max - min;
            // picking numbers here to make it wide enough killed me timewise - kept getting
            // items falling off left/right, so not filling
            var mxx = Math.Max(del.x, del.y);
            var scale = 12;//part2 ? 12 : 4;
            var w = scale * mxx;
            var h = del.y+3;
            var c = new char[w,h];
            Apply(c, (i, j, v) => '.');

            var dx = (max+min).x/2-min.x+1 + scale*mxx/2;

            foreach (var p in paths)
            {
                for (var k = 0; k < p.Count-1; ++k)
                {
                    var p1 = p[k]-min;
                    var p2 = p[k + 1]-min;
                    DDA.Dim2(p1.x, p1.y, p2.x, p2.y, (i, j) => c[i+dx, j] = 'X');
                }
            }

            if (part2)
            {
                var p1 = new vec3(0,h-1);
                var p2 = new vec3(w-1, h - 1);
                DDA.Dim2(p1.x, p1.y, p2.x, p2.y, (i, j) => c[i, j] = 'X');
            }

            // time
            int count = 0;
            bool abyss = false;
            while (true)
            {
                abyss = false;
                var (sx, sy) = (500-min.x+dx, 0);
                var (ex, ey) = (sx, sy);
                while (true) // motion
                {
                    if (Open(sx,sy+1))
                    {
                        sy++; // drop; - check abyss?
                    }
                    else if (Open(sx-1, sy +1))
                    {
                        sx--;
                        sy++;
                    }
                    else if (Open(sx+1, sy + 1))
                    {
                        sx++;
                        sy++;
                    }
                    else break;
                }

                c[sx, sy] = 'o';

                if (part2 && ex==sx&&ey==sy)
                    break;
                if (abyss)
                    break;
                ++count;
                bool Open(int i, int j)
                {
                    if (j >= h)
                    {
                        abyss = true;
                        return false;
                    }
                    if (i < 0) Console.WriteLine("Below zero");
                    if (i < 0 || i >= w) throw new Exception();
                    if (i < 0 || j < 0 || w <= i || h <= j) return false;
                    if (c[i, j] == '.') return true;
                    return false;
                }
            }

            //if (!part2)
            //    DumpMap(w, h, c);
            //return -123;


            if (part2)
            {
                count = 0;

                Apply(c, (i, j, v) =>
                {
                    if (v == 'o') count++;
                    return v;
                });

               // Dump(c, noComma: true);
            }

            Console.WriteLine(count);

            if (part2) return count;//19179 not it
            return count;

            void DumpMap(int w, int h, char[,] g)
            {
                // trim
                var minX = int.MaxValue;
                var maxX = int.MinValue;
                Apply(g, (i, j, v) =>
                {
                    if (v != '.')
                    {
                        minX = Math.Min(minX, i);
                        maxX = Math.Max(maxX, i);
                    }

                    return v;
                });
                Console.WriteLine($"min max {minX} {maxX}");
                for (var j = 0; j < h; ++j)
                {
                    for (var i = minX - 5; i <= maxX + 5; ++i)
                    {
                        Console.Write(g[i,j]);
                    }

                    Console.WriteLine();
                }
            }

        }
    }
}