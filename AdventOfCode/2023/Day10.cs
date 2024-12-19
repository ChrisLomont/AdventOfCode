namespace Lomont.AdventOfCode._2023
{
    internal class Day10 : AdventOfCode
    {
        // 6714
        // 429 part 2, S was vert :|

        /*
Day       Time    Rank  Score       Time   Rank  Score
 10   01:24:17    5932      0   02:12:30   2882      0
  9   00:10:00    1239      0   00:13:20   1181      0
        */



        public override object Run(bool part2)
        {

            var (N, S, E, W) = (new vec2(0, -1), new vec2(0, 1), new vec2(1, 0), new vec2(-1, 0));

            string ch1 = "|-LJ7FS."; // chars in puzzle
            string ch2 = "│─└┘┐┌S "; // ANSI, old codepage
            var (NS, EW, NE, NW, SW, SE) =
                (ch2[0], ch2[1], ch2[2], ch2[3], ch2[4], ch2[5]);

            Dictionary<char, (vec2, vec2)> tileDirs = new()
            {
                [NS] = (N, S), [EW] = (E, W), [NE] = (N, E), [NW] = (N, W), [SW] = (S, W), [SE] = (S, E)
            };


            var (w, h, g) = CharGrid();

            bool Paired(vec2 src, vec2 dir)
            {
                var c = Get(g, src + dir, ' ');
                var (p1, p2) = tileDirs.ContainsKey(c) ? tileDirs[c] : (vec2.Zero, vec2.Zero);
                return p1 == -dir || p2 == -dir;
            }

            // remap puzzle chars, find start
            vec2 start = new(-1, -1);
            Apply(g, (i, j, c) =>
                {
                    if (c == 'S') start = new vec2(i, j);
                    return ch2[ch1.IndexOf(c)];
                }
            );

            // change 'S' to proper tile character
            foreach (var (key, (d1, d2)) in tileDirs)
                if (Paired(start, d1) && Paired(start, d2))
                    g[start.x, start.y] = key;

            Dictionary<vec2, int> path = new();
            List<vec2> lpath = new();
            var cur = start;
            var prev = new vec2(-5, -5); // far off screen
            do
            {
                path.Add(cur, path.Count + 1);
                lpath.Add(cur);
                var (m1, m2) = tileDirs[g[cur.x, cur.y]];
                (prev, cur) = (cur, (cur + m1 != prev) ? cur + m1 : cur + m2);
            } while (cur != start);

            if (!part2)
                return path.Count / 2;


            HashSet<vec2> insides = new();
            char prevCorner = ' ';
            int crossings = 0;
            for (var j = 0; j < h; ++j)
            for (var i = 0; i < w; ++i)
            {
                var p = new vec2(i, j);
                if (path.ContainsKey(new(i, j)))
                {
                    if (g[i, j] == SE || g[i, j] == NE) 
                        prevCorner = g[i, j];
                    if ((g[i, j] == NS) || 
                        (g[i, j] == NW && prevCorner == SE) ||
                        (g[i, j] == SW && prevCorner == NE)) 
                        ++crossings;
                }
                else if ((crossings & 1) == 1)
                    insides.Add(p);
            }

            DumpColors<char> colors = new()
            {
                (c, i, j) =>
                {
                    var p = new vec2(i, j);
                    if (insides.Contains(p))
                        return (true, "#000000;#FFFFFF");
                    if (path.ContainsKey(p))
                    {
                        var d = path[p];
                        var hue = 360.0 * d * 3.0 / (float)(path.Count);
                        var (r, g, b) = HSL2RGB(hue, 1.0f, 0.5f);
                        //color = "#00FFFF";
                        string color = "";
                        if (p == start)
                        {
                            r = 1 - r;
                            g = 1 - g;
                            b = 1 - b;
                            color = $"#FF00FF;#{(int)(r * 255.0):X2}{(int)(g * 255.0):X2}{(int)(b * 255.0):X2}";
                        }
                        else
                        {
                            color = $"#000000;#{(int)(r * 255.0):X2}{(int)(g * 255.0):X2}{(int)(b * 255.0):X2}";
                        }

                        return (true, color);
                    }

                    return (true, "#808080;#000000");
                }
            };

            Console.WriteLine("-----");
            Dump(g, noComma: true, colors);
            Console.WriteLine("-----");

            // Shoelace formula and Pick's Theorem
            // 2A=sum det((xi xi+1),(yi yi+1) and A=i+b/2-1
            long ans = 0;
            for (int i = 0; i < lpath.Count; ++i)
            {
                var p1 = lpath[i];
                var p2 = lpath[(i + 1) % lpath.Count];
                ans += p1.x * p2.y - p1.y * p2.x;
            }

            // (sum det)/2 = A = i + (pathlen)/2-1
            
            // i = ans/2 - pathlen/2 + 1
            var ins = (Math.Abs(ans) - lpath.Count) / 2 + 1;
            Trace.Assert(ins == insides.Count);


            return insides.Count;

        }


    }
}