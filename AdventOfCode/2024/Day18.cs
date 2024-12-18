

using System.Data.SqlTypes;

namespace Lomont.AdventOfCode._2024
{
    internal class Day18 : AdventOfCode
    {
        /*
2024 Day 18 part 1: 308 in 9299.4 us
2024 Day 18 part 2: 46,28 in 2065641.1 us
        */

        object Run2()
        {
            int size = 70;
            int stop = 1024;
            //(size,stop) = (6,12);
            long answer = 0;
            var g = new int[size + 1, size + 1];
            int pass = 0;
            foreach (var line in ReadLines())
            {
                var nums = Numbers(line);
                var (x, y) = (nums[0], nums[1]);
                g[x, y] = 1;
                ++pass;
                var d = Solve();
                if (d < 0)
                    return $"{x},{y}";
            }

            return -1;

            int Solve()
            {
                HashSet<(int, int)> seen = new();
                Queue<(int x, int y, int depth)> frontier = new();
                frontier.Enqueue((0, 0, 0));
                while (frontier.Any())
                {
                    var (x, y, depth) = frontier.Dequeue();
                    if (x == size && y == size)
                    {
                        return depth;
                    }

                    if (seen.Contains((x, y))) continue;
                    seen.Add((x, y));
                    Try(x + 1, y, depth + 1);
                    Try(x - 1, y, depth + 1);
                    Try(x, y + 1, depth + 1);
                    Try(x, y - 1, depth + 1);
                }

                return -1;

                void Try(int x, int y, int left)
                {
                    if (0 <= x && 0 <= y && x <= size && y <= size
                        && g[x, y] == 0 && !seen.Contains((x, y))
                       )
                        frontier.Enqueue((x, y, left));
                }
            }
        }

        //public override string TestFileSuffix() => "-test1";
        public override string TestFileSuffix() => "";

        object Run1()
        {
            var (size, stop) = (70, 1024);
            //(size,stop) = (6,12); // testing
            var g = new int[size + 1, size + 1];
            int pass = 0;
            foreach (var line in ReadLines())
            {
                var nums = Numbers(line);
                var (x, y) = (nums[0], nums[1]);
                g[x, y] = 1;
                ++pass;
                if (pass == stop)
                    break;
            }

            return Solve();

            // return depth, or -1 if none
            int Solve()
            {
                HashSet<(int, int)> seen = new();
                Queue<(int x, int y, int depth)> frontier = new();
                frontier.Enqueue((0, 0, 0));
                while (frontier.Any())
                {
                    var (x, y, depth) = frontier.Dequeue();
                    if (x == size && y == size)
                        return depth;
                    if (seen.Contains((x, y))) continue;
                    seen.Add((x, y));
                    Try(x + 1, y, depth + 1);
                    Try(x - 1, y, depth + 1);
                    Try(x, y + 1, depth + 1);
                    Try(x, y - 1, depth + 1);
                }

                return -1;

                void Try(int x, int y, int left)
                {
                    if (0 <= x && 0 <= y && x <= size && y <= size
                        && g[x, y] == 0 && !seen.Contains((x, y))
                       )
                        frontier.Enqueue((x, y, left));
                }
            }

        }

        public override object Run(bool part2)
        {
            return part2 ? Run2() : Run1();
        }
    }
}