

namespace Lomont.AdventOfCode._2024
{
    /*
     2024 Day 4 part 1: 2464 in 5125.3 us
    2024 Day 4 part 2: 1982 in 6142.5 us

     */

    internal class Day04 : AdventOfCode
    {
        object Run2()
        {
            long answer = 0;
            var (w, h, g) = CharGrid();
            var centers = new HashSet<(int, int)>();
            for (var di = -1; di <= 1; di+=2)
            for (var dj = -1; dj <= 1; dj+=2)
            {
                for (var i = 0; i < w; ++i)
                for (var j = 0; j < h; ++j)
                {
                    var ei = i + 2 * di; // last needs in bounds
                    var ej = j + 2 * dj;
                    if (ei < 0 || ej < 0 || w <= ei || h <= ej) continue;

                    if (Match(g, i, j, di, dj, "MAS"))
                    {
                        var c = (i+di,j+dj);
                        if (centers.Contains(c))
                            answer++;
                        centers.Add(c);
                    }
                }
            }
            return answer;
        }

        bool Match(char[,] g, int i, int j, int di, int dj, string msg = "XMAS")
        {
            foreach (var c in msg)
            {
                if (g[i, j] != c) return false;
                i += di;
                j += dj;
            }

            return true;
        }

        object Run1()
        {
            long answer = 0;
             var (w,h,g) = CharGrid();
             for (var di = -1; di <=1; ++di)
             for (var dj = -1; dj <= 1; ++dj)
             {
                 if (di == 0 && dj == 0) continue;
                 for (var i = 0; i < w; ++i)
                 for (var j = 0; j < h; ++j)
                 {
                     var ei = i + 3 * di; // last needs in bounds
                     var ej = j + 3 * dj;
                     if (ei < 0 || ej < 0 || w <= ei || h <= ej) continue;

                            if (Match(g, i, j, di, dj))
                         answer++;
                 }
             }
        return answer;
        }

        public override object Run(bool part2)
        {
            return part2 ? Run2() : Run1();
        }
    }
}