using Lomont.AdventOfCode.Utils;

namespace Lomont.AdventOfCode._2021
{
    internal class Day01 : AdventOfCode
    {
        public override object Run(bool part2)
        {
            int incr = 0;
            var lines = ReadLines();
            for (var i =0; i < lines.Count; ++i)
            {
                if (!part2)
                {
                    if (i > 0 && Num(lines[i]) > Num(lines[i - 1]))
                        ++incr;
                }
                else
                {
                    if (i < 3) continue;
                    var v1 = RangeLen(lines, i - 3, 3).Nums().Sum();
                    var v2 = RangeLen(lines, i - 2, 3).Nums().Sum();
                    if (v1 < v2) ++incr;
                }
            }
            return incr;
        }
    }
}