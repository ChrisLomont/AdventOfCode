namespace Lomont.AdventOfCode._2020
{
    internal class Day03 : AdventOfCode
    {
        public override object Run(bool part2)
        {
            var (w, h, g) = CharGrid();
            if (!part2) 
                return CountTrees(3, 1);

            var sl = new[] {(1,1),(3,1),(5,1),(7,1),(1, 2) }.Select(p => CountTrees(p.Item1,p.Item2)).ToList();
           // Dump(sl);
            return sl.Aggregate(1L,(a,b)=>a*b);

            int CountTrees(int dx, int dy)
            {
                int count = 0;
                var (x, y) = (0, 0);
                while (y < h)
                {
                    if (g[x, y] == '#') ++count;
                    x += dx;
                    if (x >= w) x -= w;
                    y += dy;
                }
                return count;
            }
        }
    }
}