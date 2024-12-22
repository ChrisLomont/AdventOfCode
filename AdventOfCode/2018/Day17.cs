

namespace Lomont.AdventOfCode._2018
{
    internal class Day17 : AdventOfCode
    {
        /*
2018 Day 17 part 1: 39557 in 41303.9 us
2018 Day 17 part 2: 32984 in 28279.8 us         */
        public override object Run(bool part2)
        {

            int CLAY = 0;
            int WATER = 1;

            // (x,y) to item 0,1
            Dictionary<(int x, int y), int> blocked = new();

            void Block(int x, int y, int type)
            {
                if (!blocked.ContainsKey((x, y)))
                    blocked.Add((x, y), type);
                blocked[(x, y)] = type;
            }

            bool Blocked(int x, int y) => blocked.ContainsKey((x, y));

            HashSet<(int x, int y)> visited = new();
            bool Visited(int x, int y) => visited.Contains((x, y));

            foreach (var line in ReadLines())
            {
                var n = Numbers(line);

                if (line[0] == 'x')
                    Line(n[0], n[1], n[0], n[2], 0, 1, CLAY);
                else if (line[0] == 'y')
                    Line(n[1], n[0], n[2], n[0], 1, 0, CLAY);
                else throw new NotImplementedException("P17");
            }

//            Console.WriteLine();

            var yMin = blocked.Keys.Min(p => p.y);
            var yMax = blocked.Keys.Max(p => p.y);

            // flood fill 
            Drop(500, yMin - 1);

            if (part2)
                return blocked.Values.Sum();
            return visited.Count();


            void Line(int x1, int y1, int x2, int y2, int dx, int dy, int type = -1)
            {
                Dot();
                while (x1 != x2 || y1 != y2)
                {
                    x1 += dx;
                    y1 += dy;
                    Dot();
                }

                void Dot()
                {
                    if (type >= 0)
                        Block(x1, y1, type);
                    else
                        visited.Add((x1, y1));
                }
            }

            int FindEnd(int x, int y, int dx)
            {
                while (!Blocked(x + dx, y) && Blocked(x, y + 1))
                    x += dx;
                return x;
            }



            void Flood(int x, int y)
            {
                var (left, right) = (FindEnd(x, y, -1), FindEnd(x, y, +1));
                Line(left, y, right, y, 1, 0);
                if (!Blocked(left, y + 1) && !Visited(left, y + 1))
                    Drop(left, y);
                if (!Blocked(right, y + 1) && !Visited(right, y + 1))
                    Drop(right, y);
                if (Blocked(left, y + 1) && Blocked(right, y + 1))
                {
                    Line(left, y, right, y, 1, 0, WATER);
                    Flood(x, y - 1);
                }
            }

            void Drop(int x, int y)
            {
                while (!Blocked(x, y + 1) && y != yMax)
                    visited.Add((x, ++y));

                if (y != yMax) Flood(x, y);
            }

        }
    }
}