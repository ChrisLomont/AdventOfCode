namespace Lomont.AdventOfCode._2024
{
    internal class Day12 : AdventOfCode
    {
        /*
         *  2024 Day 12 part 1: 1387004 in  321304.5 us
         *  2024 Day 12 part 2:  844198 in 1114475.5 us
         */

        //public override string TestFileSuffix() => "-test1";
        public override string TestFileSuffix() => "";

        public override object Run(bool part2)
        {
            List<List<vec2>> regions = new();
            char cleared = '.';
            var (w, h, g) = CharGrid();
            Apply(g, (i, j, c) => {
                if (g[i, j] != cleared)
                    regions.Add(FloodFill(i, j, c, cleared, g, w, h));
                return cleared; });


            long answer = 0;
            foreach (var region in regions)
            {
                var sides = Perimeter(region);
                if (part2) Collapse(sides);
                answer += sides.Count * region.Count;
            }

            return answer;

            List<(vec2, vec2)> Perimeter(List<vec2> region)
            {
                List<(vec2, vec2)> sides = new List<(vec2, vec2)>();

                foreach (var sq in region)
                {
                    var (x, y) = sq;
                    AddIf(sq, 1, 0, x + 1, y);
                    AddIf(sq, -1, 0, x, y + 1);
                    AddIf(sq, 0, 1, x + 1, y + 1);
                    AddIf(sq, 0, -1, x, y);
                }
                return sides;

                void AddIf(vec2 sq, int dx, int dy, int xx, int yy)
                {
                    if (!region.Contains(new vec2(sq.x + dx, sq.y + dy)))
                        sides.Add((new(xx, yy), new(xx -dy, yy +dx)));
                }
            }

            // collapse sides int straight segments
            static void Collapse(List<(vec2 a, vec2 b)> sides)
            {
                while (true)
                {
                    int ct = sides.Count;
                    for (int i = 0; i < sides.Count; ++i)
                    {
                        for (int j = 0; j < sides.Count; ++j)
                        {
                            if (i == j) continue;
                            var (a1,a2) = sides[i];
                            var (b1,b2) = sides[j % sides.Count];
                            if (a2 == b1 && vec2.Cross(a2 - a1, b2 - b1) == 0)
                            {
                                sides[i] = (a1, b2);
                                sides.RemoveAt(j);
                                if (j < i) i--;
                                j--;
                            }
                        }
                    }
                    if (ct == sides.Count) break;
                }
            }

            // flood fill region, erase it, return list of blocks
            static List<vec2> FloodFill(int x, int y, char ch, char cleared, char [,] g, int w, int h)
            {
                var r = new List<vec2>();
                if (x < 0 || y < 0 || w <= x || h <= y || g[x, y] != ch) return r;
                r.Add(new(x, y));
                g[x, y] = cleared;
                r.AddRange(FloodFill(x + 1, y, ch, cleared, g, w, h));
                r.AddRange(FloodFill(x - 1, y, ch, cleared, g, w, h));
                r.AddRange(FloodFill(x, y + 1, ch, cleared, g, w, h));
                r.AddRange(FloodFill(x, y - 1, ch, cleared, g, w, h));
                return r;
            }
        }

   }
}