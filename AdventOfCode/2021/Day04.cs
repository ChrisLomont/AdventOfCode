namespace Lomont.AdventOfCode._2021
{
    //2021 Day 4a: 31424 in 99031.2 us
    //2021 Day 4b: 23042 in 71397.5 us 

    internal class Day04 : AdventOfCode
    {
        private List<long[,]> boards = new();
        private List<bool> wins = new();
        public override object Run(bool part2)
        {
            var lines = ReadLines();

            var moves = GetNumbers(lines[0], false);
            for (var i = 2; i < lines.Count; i += 6)
            {
                var (w, h, g) = NumberGrid(i, i + 5);
                boards.Add(g);
                wins.Add(false);
            }

            int winCount = 0;
            foreach (var m in moves)
            {
                for (var i = 0; i < boards.Count; ++i)
                {
                    var b = boards[i];
                    var t = Move(b, m);
                    if (t.Item1)
                    {
                        if (!part2)
                            return t.Item2 * m;
                        else 
                        {
                            if (!wins[i])
                            {
                                winCount++;
                                wins[i] = true;
                            }
                            if (winCount == boards.Count)
                                return t.Item2 * m;
                        }
                    }

                }
            }


            return -1;

            (bool, long) Move(long[,] g, int m)
            {
#if false
.// todo - get to work?
                var ans = (false, -1L);
                Apply(g, (i, j, t) =>
                    {
                        if (t == m)
                        {
                            var res = Test(g, i, j);
                            if (res.win)
                                ans = (true, res.sum);
                            return t + 1000;
                        }
                        return t;
                    }
                    );
                return ans;
#else

                for (var i = 0; i < 5; ++i)
                for (var j = 0; j < 5; ++j)
                    if (g[i,j] == m)
                    {
                        g[i, j] += 1000;
                        var res = Test(g, i, j);
                        if (res.win)
                            return (true, res.sum);
                    }

                return (false, 0);
#endif
            }

            (bool win, long sum) Test(long[,] g, int i0, int j0)
            {
                // check wins
                bool h = true, v = true;
                Line(0, j0, 4, j0, (i, j) => h &= g[i, j] >= 1000);
                Line(i0, 0, i0, 4, (i, j) => v &= g[i, j] >= 1000);

                //Horiz(g, j0, (t) => h &= t >= 1000);
                //Vert(g, i0, (t) => v &= t >= 1000);

                if (h || v)
                {
                    long sum = 0;
                    Apply(g, t => sum += t < 1000 ? t : 0);
                    return (true, sum);
                }

                return (false, 0);

            }
        }
    }
}