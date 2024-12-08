

namespace Lomont.AdventOfCode._2024
{
    internal class Day06 : AdventOfCode
    {
        /*
         * 2024 Day 6 part 1: 4696 in 10630.1 us
2024 Day 6 part 2: 1443 in 27502602.8 us
         */

        object Run2()
        {
            long answer = 0;
            var (w, h, g) = CharGrid();
            this.w = w;
            this.h = h;
            int x = -1, y = -1;
            Apply(g, (i, j, ch) =>
            {
                if (ch == '^')
                {
                    x = i;
                    y = j;

                }

                return ch;
            });


            int sx = x, sy = y;
            for (int i = 0; i < w; ++i)
            for (int j = 0; j < h; ++j)
            {
                if (g[i, j] != '#')
                {
                    g[i, j] = '#';
                    if (IsLoop(g, sx, sy, 2, false))
                        answer++;
                    g[i, j] = '.';
                }

//                    Console.WriteLine($"{i},{j} of {w},{h}");
            }

            return answer;
        }

        object Run2A()
        {
            long answer = 0;
            var (w, h, g) = CharGrid();
            this.w = w; this.h = h;
            int x = -1, y = -1;
            Apply(g, (i, j, ch) =>
            {
                if (ch == '^')
                {
                    x = i;
                    y = j;

                }

                return ch;
            });

            // 1501 too high

            HashSet<(int, int)> loops = new HashSet<(int, int)>();

            // find loops
            int dir = 2;

            while (Legal(x, y))
            {
                var (xn, yn) = NextPos(g, x, y, dir);
                if (!Legal(xn, yn)) break;
                if (g[xn, yn] != '#')
                {
                    g[xn, yn] = '#';
                    if (IsLoop(g, x, y, dir, false))
                    {
                        loops.Add((xn, yn));
                        g[xn, yn] = 'O';
                        //Console.WriteLine($"loop {xn},{yn}");
                        //Dump(g,true);
                    }
                    else
                        g[xn, yn] = '.';
                }

                DoStep(g, ref x, ref y, ref dir);
            }

            return loops.Count;
        }

        object Run1()
        {
            long answer = 0;
            var (w,h,g) = CharGrid();
            this.w = w; this.h=h;
            int x = -1, y = -1;
            Apply(g, (i, j, ch) =>
            {
                if (ch == '^')
                {
                    x = i;
                    y = j;

                }

                return ch;
            });

            int d = 2; // N,E,S,W
            IsLoop(g, x, y, d);

            Apply(g, (i, j, ch) =>
            {
                if (ch == 'X')
                {
                    answer += 1;
                }
                return ch;
            });
           
            return answer;
        }

        int w, h;

        bool IsBlocked(char[,] g, int x, int y, int dir)
        {
            int[] dd = { 0, 1, 1, 0, 0, -1, -1, 0 };
            int x1 = x + dd[dir * 2];
            int y1 = y + dd[dir * 2 + 1];
            if (Legal(x1, y1) && g[x1, y1] == '#')
                return true;
            return false;
        }

        (int x, int y) NextPos(char[,] g, int x, int y, int dir)
        {
            int[] dd = { 0, 1, 1, 0, 0, -1, -1, 0 };
            x+= dd[dir * 2];
            y+= dd[dir * 2 + 1];
            return (x, y);
        }

        void DoStep(char[,] g, ref int x, ref int y, ref int dir)
        {
            if (IsBlocked(g, x, y, dir))
            {
                dir = (dir - 1 + 4) % 4;

            }
            else
            {
                (x,y) = NextPos(g, x, y, dir);
            }
        }
        bool Legal(int i, int j)
        {
            return 0 <= i && 0 <= j && i < w && j < h;
        }
        bool IsLoop(char[,] g, int sx, int sy, int dir, bool mark=true)
        {
            HashSet<(int x, int y, int d)> seen = new();
            int x = sx, y = sy;

            while (Legal(x, y))
            {
                var key = (x,y,dir);
                if (seen.Contains(key))
                    return true;
                seen.Add(key);

                //g[x, y] = 'Q';
                //Console.WriteLine($"{x},{y}");
                //Dump(g, true);
                //Console.WriteLine();
                if (mark)
                    g[x, y] = 'X';

                DoStep(g, ref x, ref y, ref dir);
/*                int[] dd = { 0, 1, 1, 0, 0, -1, -1, 0 };
                int x1 = x + dd[dir * 2];
                int y1 = y + dd[dir * 2 + 1];
                if (Legal(x1, y1) && g[x1, y1] == '#')
                    dir = (dir - 1 + 4) % 4;
                else
                {
                    x = x1;
                    y = y1;
                }*/

            }

            return false;
         
        }


        public override object Run(bool part2)
        {
            return part2 ? Run2() : Run1();
        }
    }
}