
namespace Lomont.AdventOfCode._2021
{
    internal class Day05 : AdventOfCode
    {
        public override object Run(bool part2)
        {
            var testLines = ReadLines();
            var numberLines = testLines.Select(b=>Numbers(b)).ToList();

            var max = numberLines.Max(l => l.Max());
            var g = new int[max+1, max+1];

            foreach (var line in numberLines)
            {
                var x0 = line[0];
                var y0 = line[1];
                var x1 = line[2];
                var y1 = line[3];

                if (x1 == x0)
                    Line(x0, y0, x1, y1, (i, j) => g[i,j]++);
                else if (y1==y0)
                    Line(x0, y0, x1, y1, (i, j) => g[i, j]++);
                else if (part2)
                    Line(x0, y0, x1, y1, (i, j) => g[i, j]++);
            }

            var sum = 0;
            Apply(g,v=>sum += v>1?1:0);
            return sum;
        }
    }
}