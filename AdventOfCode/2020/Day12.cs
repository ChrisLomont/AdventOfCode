using System.Security.AccessControl;

namespace Lomont.AdventOfCode._2020
{
    internal class Day12 : AdventOfCode
    {
    //2020 Day 12 part 1: 1007 in 739.5 us
    //2020 Day 12 part 2: 41212 in 161.6 us
        public override object Run(bool part2)
        {
            var dirs = new[] {(1,0),(0,1),(-1,0),(0,-1) };
            var dir = 0L; // facing on xy plane, i.e., east

            var (x, y) = (0L, 0L); // pos in part 1, waypoint part 2
            var (sx, sy) = (0L, 0L);
            if (part2) (x, y) = (10, 1); // waypoint

            foreach (var line in ReadLines())
            {
                var n = Num(line[1..]);
                switch (line[0])
                {
                    case 'N':
                        y+=n;
                        break;
                    case 'S':
                        y -= n;
                        break;
                    case 'E':
                        x+= n;
                        break;
                    case 'W':
                        x -= n;
                        break;
                    case 'F':
                        if (part2)
                        {
                            sx += x * n;
                            sy += y * n;
                        }
                        else
                        {
                            var (dx, dy) = dirs[dir / 90];
                            x += dx * n;
                            y += dy * n;
                        }

                        break;
                    case 'R':
                        if (part2)
                        {
                            while (n > 0)
                            {
                                (x, y) = (y, -x);
                                n -= 90;
                            }

                        }
                        else
                        {
                            Trace.Assert(n % 90 == 0);
                            dir = (dir - n + 360) % 360;
                        }

                        break;
                    case 'L':
                        if (part2)
                        {
                            while (n > 0)
                            {
                                (x, y) = (-y, x);
                                n -= 90;
                            }

                        }
                        else
                        {
                            Trace.Assert(n % 90 == 0);
                            dir = (dir + n + 360) % 360;
                        }
                        break;
                }
            }

            if (part2)
                return Math.Abs(sx) + Math.Abs(sy);

            return Math.Abs(x) + Math.Abs(y);
        }
    }
}