namespace Lomont.AdventOfCode._2021
{
    internal class Day03 : AdventOfCode
    {
    //2021 Day 3a: 845186 in 2402.4 us
    //2021 Day 3b: 4636702 in 3517.2 us
        public override object Run(bool part2)
        {
            if (part2)
            {
                return Part2();
            }
            else
            {
                int gam = 0, eps = 0;
                var (w, h, g) = CharGrid();
                for (var bit = 0; bit < w; ++bit)
                {
                    var bc = 0;
                    for (var j = 0; j < h; ++j)
                        bc += g[bit, j] == '0' ? -1 : 1;
                    // bc < 0 if -1 most common, > 0 if 1 most common

                    var s = w - bit - 1; // amt to shift
                    gam |= bc > 0 ? (1 << s) : 0;
                    eps |= bc > 0 ? 0 : 1 << s;
                }

                return gam * eps;
            }
        }

        object Part2()
        {
            var linesO2 = ReadLines();
            var i = 0;
            while (linesO2.Count>1)
            {
                linesO2 = Filter(linesO2, i);
                ++i;
            }

            var linesCO2 = ReadLines();
            i = 0;
            while (linesCO2.Count > 1)
            {
                linesCO2 = Filter2(linesCO2, i);
                ++i;
            }

            long o2 = BinaryToInteger(linesO2[0]);
            long co2 = BinaryToInteger(linesCO2[0]);
            
            return o2 * co2;



            List<string> Filter(List<string> l, int i)
            {
                if (MostBit1(l, i))
                    return l.Where(m => m[i]=='1').ToList();
                return l.Where(m => m[i] == '0').ToList();
            }
            List<string> Filter2(List<string> l, int i)
            {
                if (LeastBit1(l, i))
                    return l.Where(m => m[i] == '1').ToList();
                return l.Where(m => m[i] == '0').ToList();
            }


            bool MostBit1(List<string> lines, int i)
            {
                int b0 = 0;
                foreach (var l in lines)
                    b0 += l[i] == '0' ? -1 : 1;
                return b0>=0; // tie to 1
            }
            bool LeastBit1(List<string> lines, int i)
            {
                int b0 = 0;
                foreach (var l in lines)
                    b0 += l[i] == '0' ? -1 : 1;
                return b0 < 0; // tie to 0
            }

        }
    }
}