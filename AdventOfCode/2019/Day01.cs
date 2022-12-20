namespace Lomont.AdventOfCode._2019
{
    internal class Day01 : AdventOfCode
    {
        public override object Run(bool part2)
        {
            if (!part2) 
            return ReadLines().Select(t => Int32.Parse(t) / 3 - 2).Sum();
            return ReadLines().Select(t=>comp(Int32.Parse(t))).Sum();

            int comp(int f)
            {
                var t = 0;
                while (f > 0)
                {
                    f = f / 3 - 2;
                    if (f > 0)
                        t += f;
                }

                return t;
            }

        }
    }
}