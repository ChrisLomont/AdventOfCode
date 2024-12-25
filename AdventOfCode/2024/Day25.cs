

namespace Lomont.AdventOfCode._2024
{
    /*
    2024 Day 25 part 1: 3327 in 39399.7 us
    2024 Day 25 part 2: 0 in 4858.5 us
     */
    internal class Day25 : AdventOfCode
    {
        object Run2()
        {
            long answer = 0;

            foreach (var line in ReadLines())
            {
                var nums = Numbers64(line);
            }

            return answer;
        }
        //public override string TestFileSuffix() => "-test1";
        public override string TestFileSuffix() => "";

        object Run1()
        {
            long answer = 0;
            // var (w,h,g) = DigitGrid();
            // var (w,h,g) = CharGrid();
            // ProcessAllLines(new() { ... regex->action list ... });
            List<string> lines = new();

            List<List<int>> keys = new();
            List<List<int>> locks = new();
            foreach (var line in ReadLines())
            {
                if (line == "")
                {
                    Make();
                    lines.Clear();
                }
                else
                    lines.Add(line);
            }

            void Make()
            {
                if (lines.Count == 0) return;
                var w = lines[0].Length;
                var h = lines.Count;
                var g = new char[w,h];
                Apply(g, (i, j, c) =>
                {
                    return lines[j][i];
                });
                if (g[0,0] == '.')
                {
                    keys.Add(C(g,w,h,'.'));
                }
                else
                    locks.Add(C(g,w,h,'#'));

//                Dump(g, noComma: true);
            }

            Make();

            int v = 0;
            foreach (var k in keys)
            foreach (var b in locks)
            {
                var over = false;
                for (int i =0 ; i < k.Count; ++i)
                    if (k[i] + b[i] > 5)
                        over = true;
                if (!over) ++v;
            }

            return v;



            List<int> C(char[,] g, int w, int h, char ch)
            {
                List<int> c = new();
                for (var i = 0; i < w; ++i)
                {
                    var s = 0;
                    for (int j = 0; j < h; ++j)
                    {
                        if (g[i,j] == ch) ++s;
                    }
                    if (ch=='#')
                    c.Add(s-1);
                    else c.Add(5-s+1);
                    
                }

                var m = c.Aggregate("", (a, b) => a + "," + b);
                Console.WriteLine($"{ch}: {m}");
                return c;
            }


            return answer;
        }

        public override object Run(bool part2)
        {
            return part2 ? Run2() : Run1();
        }
    }
}