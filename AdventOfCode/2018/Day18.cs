

namespace Lomont.AdventOfCode._2018
{
    internal class Day18 : AdventOfCode
    {
        /*
         2018 Day 18 part 1: 486878 in 8252.2 us
         2018 Day 18 part 2: 190836 in 161551.7 us
        */
        public override object Run(bool part2)
        {
            var (w, h, g) = CharGrid();

            // cycle detection
            List<(int score, int time)> history = new();

            var maxTime = 1_000_000_000L;
            if (!part2) maxTime = 10;
            for (var time = 0; time < maxTime; ++time)
            {
                history.Add((Score(), time));
                var finalCost = TryScoreCycle();
                if (finalCost > -1 && part2)
                    return finalCost;

                DoStep();
            }

            return Score();

            void DoStep()
            {
                var g2 = new char[w, h];
                Apply(g2, (i, j, c) => {
                    c = g[i, j];
                    var c1 = g[i, j];
                    if (c1 == '.' && CountNbrs(i, j, '|') >= 3) c = '|';
                    if (c1 == '|' && CountNbrs(i, j, '#') >= 3) c = '#';
                    if (c1 == '#' && (CountNbrs(i, j, '#') < 1 || CountNbrs(i, j, '|') < 1)) c = '.';
                    return c;
                });
                g = g2;
            }

            int CountNbrs(int i, int j, char c)
            {
                var s = 0;
                for (var x = -1; x <= 1; ++x)
                for (var y = -1; y <= 1; ++y)
                    if ((x!=0 || y != 0) && Ok(i + x, j + y) && g[i + x, j + y] == c) s++;
                return s;

                bool Ok(int x, int y) => 0 <= x && 0 <= y && x < w && y < h;
            }

            // cycle score, else -1
            int TryScoreCycle()
            {
                int len = 100; // how far back to look
                if (history.Count < len) return -1;
                
                // look for end point
                var ep = history.Last().score;
                var n = history.Count-1;
                var dist = 1;
                while (dist < len)
                {
                    if (history[n - dist].score == ep)
                    { // check repeat
                        var rep = true;
                        for (int t = 0; rep && t < dist; ++t) // lots of repeats
                            rep &= history[n - t].score == history[n - dist - t].score;
                        if (rep)
                        {
                            // find final score
                            
                            // t = cycle start time, l = cycle len, m = max time
                            // want max periods p st
                            // t + k*l <= m, i.e., 
                            // k = (m-t)/l
                            // then last start time e = t+k*l, must jump ahead into cycle by
                            // excess = m-e

                            long period = dist;
                            long start  = history[n - dist].time;

                            long periodsLeft = (maxTime - start ) / period;
                            long lastStart   = start  + periodsLeft * period;
                            long shortfall   = maxTime - lastStart;
                            return history[(int)(n - dist + shortfall)].score;
                        }
                    }
                    dist++;
                }

                return -1; // none
            }

            int Score()
            {

                int woods = 0, yards = 0;
                Apply(g, c =>
                {
                    if (c == '|') woods++;
                    if (c == '#') yards++;
                    return c;

                });
                return woods * yards;
            }
        }
    }
}