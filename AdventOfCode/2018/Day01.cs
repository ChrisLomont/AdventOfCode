

namespace Lomont.AdventOfCode._2018
{
    internal class Day01 : AdventOfCode
    {
        /*
         * 2018 Day 1 part 1: 556 in 14525.7 us
2018 Day 1 part 2: 448 in 21143.1 us
         */
        public override object Run(bool part2)
        {
            long answer = 0;
            HashSet<long> seen = new();
            var lines = ReadLines();
            do
            {
                foreach (var line in lines)
                {
                    if (line[0] == '+')
                    {
                        answer += Int64.Parse(line[1..]);

                    }
                    else if (line[0] == '-')
                    {
                        answer -= Int64.Parse(line[1..]);

                    }
                    else throw new Exception();

                    //Console.WriteLine(answer);
                    if (seen.Contains(answer) && part2)
                        return answer;
                    seen.Add(answer);
                }
            } while (part2);

            return answer;
        }

    }
}