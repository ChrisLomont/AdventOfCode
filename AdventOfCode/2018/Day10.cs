

namespace Lomont.AdventOfCode._2018
{
    internal class Day10 : AdventOfCode
    {   // inspection LXJFKAXA
        // 10312
        //
        object Run2()
        {
            long answer = 0;

            foreach (var line in ReadLines())
            {
                var nums = Numbers64(line);
            }

            return answer;
        }

        object Run1()
        {
            long answer = 0;
            List<(int x, int y)> points = new();
            List<(int dx, int dy)> dirs= new();
            foreach (var line in ReadLines())
            {
                var nums = Numbers(line);
                points.Add((nums[0], nums[1]));
                dirs.Add((nums[2], nums[3]));
            }

            int t = 0;
            bool small = false;
            while (true)
            {
                var x1 = points.Min(p => p.x);
                var x2 = points.Max(p => p.x);
                var y1 = points.Min(p => p.y);
                var y2 = points.Max(p => p.y);

                var (w, h) = (x2 - x1 + 1, y2 - y1 + 1);
                Console.WriteLine($"{t}: {w}x{h}");

                if (w < 100 && h < 100)
                {
                    small = true;

                    var g = new char[w, h];
                    Apply(g, c => '.');

                    for (int k = 0; k < points.Count; ++k)
                    {
                        g[points[k].x - x1, points[k].y - y1] = '#';
                    }

                    Dump(g);
                    Console.WriteLine();
                }
                else if (small)
                    break;
                var list2=new List<(int x, int y)>();
                for (int k = 0; k < points.Count; ++k)
                {
                    var x = points[k].x + dirs[k].dx;
                    var y = points[k].y + dirs[k].dy;
                    list2.Add((x, y));
                }
                points=list2;

                t++;

            }


            return answer;
        }

        public override object Run(bool part2)
        {
            return Run1();
            //return part2 ? Run2() : Run1();
        }
    }
}