

namespace Lomont.AdventOfCode._2024
{
    internal class Day03 : AdventOfCode
    {
        /*
         2024 Day 3 part 1: 196826776 in 2054 us
         2024 Day 3 part 2: 106780429 in 2575.9 us
         */
      

        public override object Run(bool part2)
        {
            var r = new Regex(@"(mul\((?<n1>\d+),(?<n2>\d+)\))|(do\(\))|(don't\(\))");
            long answer = 0;
            bool enable = true;
            foreach (var line in ReadLines())
            {
                var m = r.Match(line);
                while (m.Success)
                {
                    if (m.Value == "do()")
                    {
                        enable = true;
                    }
                    else if (m.Value == "don't()")
                    {
                        enable = false;
                    }
                    else
                    {
                        var d1 = m.Groups["n1"].Value;
                        var d2 = m.Groups["n2"].Value;
                        if (enable || !part2)
                        {
                            answer += long.Parse(d1) * long.Parse(d2);
                        }
                    }
                    m = m.NextMatch();
                }
            }

            return answer;
        }
    }
}