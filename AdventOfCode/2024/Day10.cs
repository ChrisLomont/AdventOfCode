

namespace Lomont.AdventOfCode._2024
{
    internal class Day10 : AdventOfCode
    {
        object Run2()
        {
            long answer = 0;
            var (w, h, g) = DigitGrid();

            HashSet<string> paths= new();

            Apply(g, (i, j, c) =>
            {
                if (c == 0)
                {
                    paths.Clear();
                    //    Console.WriteLine($"Start {i},{j}");
                    Score(i, j, -1, "");
                    //   foreach (var (a, b) in ht9)
                    //       Console.WriteLine($"  {a},{b}");
                    long sc = paths.Count;
                    answer += sc;
                }

                return c;
            });


            return answer;

            void Score(int i, int j, int ht, string sofar)
            {
                if (i < 0 || j < 0 || w <= i || h <= j) return;
                var h2 = g[i, j];
                if (h2 != ht + 1) return;
                sofar += new vec2(i,j).ToString();
                if (h2 == 9)
                {
                    paths.Add(sofar);
                    /*
                    var dc = new DumpColors<int> { };
                    Func<int, int, int, (bool, string)> f =
                        (c1, i1, j1) =>
                        {
                            for (int k = 0; k < 9; ++k)
                                if (moves[k] == new vec2(i1, j1))
                                    return (true, "FF0000");
                            return (false, "000000");
                        };
                    dc.Add(f);

                    //                    Dump(g, noComma: true,colors:dc);
                    */
                }
                else
                {
                    //moves[h2] = new(i, j);
                    Score(i - 1, j, h2, sofar);
                    Score(i + 1, j, h2, sofar);
                    Score(i, j - 1, h2, sofar);
                    Score(i, j + 1, h2, sofar);
                }

            }
        }

        object Run1()
        {
            long answer = 0;
            var (w,h,g) = DigitGrid();

            HashSet<(int, int)> ht9 = new();

            Apply(g, (i, j, c) =>
            {
                if (c == 0)
                {
                    ht9.Clear();
                //    Console.WriteLine($"Start {i},{j}");
                    Score(i, j, -1, new vec2[10]);
                 //   foreach (var (a, b) in ht9)
                 //       Console.WriteLine($"  {a},{b}");
                    long sc = ht9.Count;
                    answer += sc;
                }

                return c;
            });


            return answer;

            void Score(int i, int j, int ht, vec2[] moves)
            {
                if (i < 0 || j < 0 || w <= i || h <= j) return;
                var h2 = g[i, j];
                if (h2 != ht + 1) return;
                if (h2 == 9)
                {
                    moves[h2] = new(i, j);
                    ht9.Add((i, j));
                    var dc = new DumpColors<int> { };
                    Func<int,int,int,(bool,string)> f =
                        (c1, i1, j1) =>
                        {
                            for (int k = 0; k < 9; ++k)
                                if (moves[k] == new vec2(i1,j1))
                                    return (true, "FF0000");
                            return (false,"000000");
                        };
                    dc.Add(f);

//                    Dump(g, noComma: true,colors:dc);
                }
                else {
                    moves[h2] = new (i, j);
                    Score(i - 1, j, h2, moves);
                    Score(i + 1, j, h2, moves);
                    Score(i, j - 1, h2, moves);
                    Score(i, j + 1, h2, moves);
                }

            }
        }


        //public override string TestFileSuffix() => "-test1";
        public override string TestFileSuffix() => "";

        public override object Run(bool part2)
        {
            return part2 ? Run2() : Run1();
        }
    }
}