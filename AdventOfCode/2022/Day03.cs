namespace Lomont.AdventOfCode._2022
{
    internal class Day03 :AdventOfCode
    {
        public override object Run(bool part2)
        {
            var score = 0;
            var index = 0;
            string[] gp = new string[3];
            foreach (var line in ReadLines())
            {
                // a-z = 1-26
                // A-Z = 27-52
                // find intersection
                // sum priorities
                if (!part2)
                {
                    var len = line.Length;
                    var half1 = line.Substring(0, len / 2);
                    var half2 = line.Substring(len / 2);
                    Trace.Assert(half1.Length == half2.Length);
                    var both = Intersect(half1, half2);
                    score += Score(both[0]);
                }
                else
                {
                    gp[(index % 3)] = line;
                    index++;
                    if ((index % 3) == 0)
                    {
                        var s2 = Intersect(gp[0], gp[1], gp[2]);
                        score += Score(s2[0]);
                    }
                }
            }

            int Score(char b)
            {
                if ('a' <= b && b <= 'z')
                    return b - 'a' + 1;
                if ('A' <= b && b <= 'Z')
                    return b - 'A' + 1 + 26;
                throw new Exception($"Unknown {b}");
            }

            return score;
        }

    }
}
