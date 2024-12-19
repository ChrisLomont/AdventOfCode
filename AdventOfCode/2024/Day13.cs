

namespace Lomont.AdventOfCode._2024
{
    /*
    2024 Day 13 part 1: 29388 in 4482.5 us
    2024 Day 13 part 2: 99548032866004 in 1232.3 us
    */
    internal class Day13 : AdventOfCode
    {
        //public override string TestFileSuffix() => "-test1";
            public override string TestFileSuffix() => "";



            public override object Run(bool part2)
            {
                var lines = ReadLines();
                long ext = part2 ? 10000000000000 : 0;
                long tokens = 0;

                for (int i = 0; i < lines.Count; i += 4)
                {
                    var l1 = Numbers64(lines[i + 0]);
                    var l2 = Numbers64(lines[i + 1]);
                    var l3 = Numbers64(lines[i + 2]);
                    long ax = l1[0], ay = l1[1];
                    long bx = l2[0], by = l2[1];
                    long px = l3[0] + ext, py = l3[1] + ext;

                    //checked
                    {
                        // Cramer rule solve system of eqns
                        var det = ax * by - ay * bx;
                        var ad = px * by - py * bx;
                        var bd = ax * py - ay * px;
                        if ((ad % det) == 0 && (bd % det) == 0)
                            tokens += (3 * ad + bd) / det;
                    }
                }

                return tokens;
            } 
        

    }
}