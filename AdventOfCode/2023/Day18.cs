namespace Lomont.AdventOfCode._2023
{
    internal class Day18 : AdventOfCode
    {
        /*
First hundred users to get both stars on Day 18:

  1) Dec 18  00:05:31  boboquack
  2) Dec 18  00:06:34  5space
  3) Dec 18  00:07:18  Måns Magnusson
  4) Dec 18  00:07:33  tenth
  5) Dec 18  00:08:05  Rayson Su
  6) Dec 18  00:09:06  Noble Mushtak
  7) Dec 18  00:09:14  phenomist
  8) Dec 18  00:09:31  hyper-neutrino
  9) Dec 18  00:09:35  coolcomputery

 96) Dec 18  00:20:29  Tam An Le Quang
 97) Dec 18  00:20:35  SeptaCube
 98) Dec 18  00:20:37  Tom Sirgedas
 99) Dec 18  00:20:41  timratigan
100) Dec 18  00:20:55  stazarzxy
First hundred users to get the first star on Day 18:

  1) Dec 18  00:02:35  5space
  2) Dec 18  00:02:44  Looking for an internship! (AoC++)
  3) Dec 18  00:03:44  Kevin Wang (AoC++)
  4) Dec 18  00:03:49  Kroppeb (AoC++)
  5) Dec 18  00:04:00  TimHuisman1703 (AoC++)
  6) Dec 18  00:04:04  boboquack
  7) Dec 18  00:04:15  Andrew Macheret

      --------Part 1---------   --------Part 2--------
Day       Time    Rank  Score       Time   Rank  Score
 18   00:14:50     440      0   00:34:15    423      0
 17   01:27:41    2105      0   01:30:16   1666      0
 16   00:27:12    1318      0   00:33:02   1167      0        

 95) Dec 18  00:08:18  Nicolas Normand
 96) Dec 18  00:08:18  BradfordC
 97) Dec 18  00:08:18  kdvkrs (AoC++)
 98) Dec 18  00:08:21  jwseph
 99) Dec 18  00:08:21  Søren Fuglede Jørgensen (AoC++)
100) Dec 18  00:08:21  Ethan

2023 Day 18 part 1: 49061 in 487032.5 us
2023 Day 18 part 2: 92556825427032 in 11940.6 us

         */

        public override object Run(bool part2)
        {
            List< (long x,long y)> pts= new ();
            long x = 0L, y = 0;
            pts.Add((x,y));
            checked
            {
                foreach (var line in ReadLines())
                {
                    if (!part2)
                    {
                        // U 4 (#06a063)

                        var dir = line[0];
                        var n = Numbers64(line);
                        var len = n[0];
                        if (dir == 'R') x += len;
                        if (dir == 'L') x -= len;
                        if (dir == 'U') y += len;
                        if (dir == 'D') y -= len;

                        pts.Add((x,y));
                    }
                    if (part2)
                    {
                        // U 4 (#06a063)

                        var w = line.Split(' ')[2];
                        var n = w.Substring(2, 6);

                        long len = Hex(n.Substring(0, 5));

                        var dir = n[5] - '0'; // 0123 = RDLU

                        var dirs = new (int x, int y)[]
                        {
                            (1, 0), //R
                            (0, 1), //D
                            (-1, 0), //L
                            (0, -1), //U
                        };

                        var dd = dirs[dir];
                        x += dd.x * len;
                        y += dd.y * len;

                        pts.Add((x, y));
                        //Console.WriteLine($"{n} -> {len} & {dir}");

                        long Hex(string h)
                        {
                            h = h.ToLower();
                            long val = 0;
                            for (int i = 0; i < h.Length; ++i)
                                val = 16 * val + "0123456789abcdef".IndexOf(h[i]);
                            return val;
                        }
                    }
                }
            }

            if (pts.Last() == (0,0))
                pts.RemoveAt(pts.Count-1);

          //  Console.WriteLine($"pts {pts.Count}");

            // combine picks theorem and area formula of polygon, solve for interior points, add perimeter
            long perim = 0;
            long area = 0;
            for (int i =0 ; i < pts.Count; ++i)
            {
                var p = pts[i];
                var q = pts[(i + 1) % pts.Count];
                area += p.x * q.y - p.y * q.x; // computes 2 * area
                perim += Math.Abs(p.x - q.x) + Math.Abs(p.y - q.y);
            }
            area = Math.Abs(area) / 2;

            var interior = area + 1 - perim / 2; // picks theorem
            
            return interior + perim;
        }

#if false
        public object RunOLD(bool part2)
        {
            //if (part2)
                return Run(part2);
            long answer = 0;

            // var (w,h,g) = DigitGrid();
            // var (w,h,g) = CharGrid();
            // ProcessAllLines(new() { ... regex->action list ... });

            int w = 2500, h = 2500;
            int[,] g = new int[w, h];
            int x = w / 2, y = h / 2;
            foreach (var line in ReadLines())
            {
                var dir = line[0];
                var n = Numbers64(line);
                var len = n[0];

                while (len-- > 0)
                {
                    g[x, y] = 1;
                    if (dir == 'R') x++;
                    if (dir == 'D') y++;
                    if (dir == 'L') x--;
                    if (dir == 'U') y--;
                }
            }

            bool legal(int x, int y) => 0 <= x && 0 <= y && x< w && y < h;
                

            var p1 = count();
            Fill(0, 0);
            var p2 = count();
            var p3 = w * h;

            var inside = p3 - p2 + p1;
            return inside;

            void Fill(int x1, int y1)
            {
                var left = new Queue<(int x, int y)>();
                left.Enqueue((x1,y1));
                while (left.TryDequeue(out var p))
                {
                    var (x, y) = p;
                    if (g[x, y] == 1) continue;
                    g[x, y] = 1;
                    if (legal(x + 1, y) && g[x + 1, y] == 0)
                        left.Enqueue((x + 1, y));
                    if (legal(x - 1, y) && g[x - 1, y] == 0)
                        left.Enqueue((x - 1, y));
                    if (legal(x , y+1) && g[x , y+1] == 0)
                        left.Enqueue((x, y+1));
                    if (legal(x, y - 1) && g[x, y - 1] == 0)
                        left.Enqueue((x, y - 1));
                }
            }

            int count()
            {

            var per = 0;
            for (int j = 0; j < h; ++j)
            for (int i = 0; i < w; ++i)
            {
                if (g[i, j] != 0) per++;

            }

            return per;
            }


            return answer;
        }
#endif
    }
}