namespace Lomont.AdventOfCode._2018
{
    internal class Day13 : AdventOfCode
    {

        /*
       2018 Day 13 part 1: 38,57 in 31631 us
2018 Day 13 part 2: 4,92 in 1471278.8 us
         */

        record Cart(int i, int j);


        public override object Run(bool part2)
        {
            List<(int x, int y, int dir, int turn)> carts = new();

            // get carts, replace track
            var (w, h, g) = CharGrid();
            var g2 = new char[w, h]; // draw them here
            Apply(g, (i, j, c) =>
            {
                var dir = "^>v<".IndexOf(c);
                if (dir >= 0)
                {
                    g2[i, j] = c;
                    carts.Add((i, j, dir, -1));
                    c = "|-|-"[dir];
                }
                return c;
            });


            return Sim(g,g2,w,h,carts,part2);

        }

        string Sim(char[,] g, char[,] g2, int w, int h, List<(int x, int y, int dir, int turn)> carts, bool part2)
        {
            //  Console.WriteLine($"{carts.Count} carts");

            bool show = false;

            if (show)
                Dump(g, g2);


            // dir: NESW
            int[] dx = { +0, +1, +0, -1 };
            int[] dy = { -1, +0, +1, +0 };

            // simulate till crash
            int pass = 0;
            while (true)
            {

                ++pass;

                var g3 = new char[w, h]; // draw them here

                for (int j = 0; j < h; j++)
                for (int i = 0; i < w; i++)
                {
                    if (g2[i, j] == 0)
                        continue;

                    var ind = carts.FindIndex(
                        c => c.x == i && c.y == j
                    );
                    if (ind >= 0)
                    {

                        var (x, y, dir, turn) = carts[ind];
                        var ch = g[x, y]; // what's going on
                        //  Console.WriteLine($"{x} {y} - ");
                        if (ch == '-' || ch == '|')
                        {
                            x += dx[dir];
                            y += dy[dir];
                        }
                        else if (ch == '+')
                        {
                            dir = (dir + turn + 4) % 4;
                            x += dx[dir];
                            y += dy[dir];
                            ++turn;
                            if (turn == 2) turn = -1;
                        }
                        else if ("/\\".Contains(ch))
                        {
                            // turn
                            /*
                             NESW \ -> WSEN
                             NESW / -> ENWS
                             */
                            if (ch == '\\')
                                dir = (-dir + 4 - 1 + 4) % 4;
                            else
                                dir = (-dir + 4 + 2 - 1 + 4) % 4;
                            x += dx[dir];
                            y += dy[dir];
                        }
                        else
                        {
                            throw new NotImplementedException();
                        }

                        // Console.WriteLine($"     {x} {y}");
                        if (g3[x, y] != 0 || carts.Any(c => c.x == x && c.y == y))
                        {
                            if (!part2)
                            {
                                //    Console.WriteLine($"crash {x} {y}");
                                return $"{x},{y}";
                            }


                            // remove both (i,j) = this and (x,y) older
                            var ind1 = carts.FindIndex(c => c.x == i && c.y == j);
                            var ind2 = carts.FindIndex(c => c.x == x && c.y == y);
                            Trace.Assert(ind1 >= 0 && ind2 >= 0 && ind1 != ind2);
                            carts.RemoveAt(Math.Max(ind1, ind2));
                            carts.RemoveAt(Math.Min(ind1, ind2));

                            g3[x, y] = (char)(0);

                            //   Console.WriteLine($"crash {x} {y} time {pass}");
                            //return -1;
                        }
                        else
                        {

                            g3[x, y] = "^>v<"[dir];
                            carts[ind] = (x, y, dir, turn);
                        }
                    }
                }

                g2 = g3;
                if (show)
                    Dump(g, g2);

                if (carts.Count == 1)
                {
                    var ct = carts[0];
                    //Console.WriteLine($"pass {pass} at {ct.x} {ct.y}");
                    return $"{ct.x},{ct.y}";
                }
            } // while
        } // Sim


        void Dump(char[,] g, char[,] g2)
        {
            for (int j = 0; j < g.GetLength(1); ++j)
            {
                for (int i = 0; i < g.GetLength(0); ++i)
                {
                    var maze = g2[i, j] == 0;

                    if (!maze)
                    {
                        var b = Console.BackgroundColor;
                        Console.BackgroundColor = ConsoleColor.Yellow;

                        Console.Write(g2[i, j]);
                        Console.BackgroundColor = b;
                    }
                    else
                    {
                        Console.Write(g[i, j]);
                    }
                }

                Console.WriteLine();
            }

            Console.WriteLine();
        }
    }
}