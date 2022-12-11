using System.Dynamic;

namespace Lomont.AdventOfCode._2020
{
    internal class Day06 : AdventOfCode
    {
    // 2020 Day 6 part 1: 6335 in 7067.6 us
    // 2020 Day 6 part 2: 3392 in 7398.2 us

        // todo- make this general, use elsewhere
        List<List<string>> Group(IEnumerable<string> lines, string match)
        {
            var all = lines.Aggregate("", (a, b) => a + "\n" + b);
            var gps = all.Split(match, StringSplitOptions.RemoveEmptyEntries);
            return gps.Select(
                g => g.Split('\n', StringSplitOptions.RemoveEmptyEntries)
            ).Select(s => s.ToList()).ToList();


        }

        // todo -= make a tally object
        // todo - move BitCount to AOC base

        public override object Run(bool part2)
        {
            var count = 0;
            foreach (var group in Group(ReadLines(),"\n\n"))
            {
                int [] counts = new int[26];
                foreach (var g in group)
                foreach (var c in g)
                    counts[c-'a']++;

                if (!part2)
                    count += counts.Count(v=>v!=0);
                else
                {
                    var f = group.Select(
                        s => s
                            .ToCharArray()
                            .Select(cc => 1L << (cc - 'a'))
                            .Sum(v=>v)
                    )
                            .Aggregate(-1L,(a,b)=>a&b)
                        ;
                    count += BitCount(f);

                    int BitCount(long v)
                    {
                        int count = 0;
                        while (v > 0)
                        {
                            count += (int)(v & 1);
                            v >>= 1;
                        }

                        return count;
                    }

                }
            }

            return count;
        }
    }
}