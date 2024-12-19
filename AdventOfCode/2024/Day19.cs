

namespace Lomont.AdventOfCode._2024
{
    internal class Day19 : AdventOfCode
    {
        /*
         *
         2024 Day 19 part 1: 263 in 45846.1 us
         2024 Day 19 part 2: 723524534506343 in 205458.9 us
         */

        //public override string TestFileSuffix() => "-test1";
        public override string TestFileSuffix() => "";

        public override object Run(bool part2)
        {
            long answer = 0;
            List<string> pats = new();
            foreach (var line in ReadLines())
            {
                if (pats.Count == 0)
                {
                    pats = line.Split(',', StringSplitOptions.RemoveEmptyEntries)
                        .Select(p => p.Trim()).ToList();
                }
                else
                {
                    var cnt = Count(line, pats);
                    if (part2) answer += cnt;
                    else if (cnt > 0) answer++;
                }
            }

            return answer;
        }

        static long Count(string target, List<string> pats)
        {
            Dictionary<string, long> memo = new();
            return Rec(target);

            long Rec(string suffix)
            {
                if (memo.ContainsKey(suffix))
                    return memo[suffix];

                long count = 0;
                for (var i = 0; i < pats.Count; ++i)
                {
                    var p = pats[i];
                    if (p == suffix)
                        count++;
                    if (suffix.StartsWith(p))
                        count += Rec(suffix.Substring(p.Length));
                }
                memo.Add(suffix, count);
                return count;
            }
        }

    }
}