namespace Lomont.AdventOfCode._2022
{
    internal class Day04 : AdventOfCode
    {
        public override object Run(bool part2)
        {
            long score = 0;
            foreach (var line in ReadLines())
            {
                var n = GetNumbers64(line);
                var (a1,a2,b1,b2) = (n[0], n[1], n[2], n[3]);



                if (!part2)
                {
                    var in1 = (a1 <= b1 && b2 <= a2); // b in a
                    var in2 = (b1 <= a1 && a2 <= b2); // a in b
                    if (in1 || in2) score++;
                }

                if (part2)
                {
                    var disjoint = b2 < a1 || a2 < b1;
                    if (!disjoint) score++;
                }
            }

            return score;

        }
    }
}
