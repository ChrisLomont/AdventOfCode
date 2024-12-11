

namespace Lomont.AdventOfCode._2024
{
    internal class Day11 : AdventOfCode
    {
        /*
         * 2024 Day 11 part 1: 183620 in 32267.2 us
2024 Day 11 part 2: 220377651399268 in 37114.4 us
         */


        //public override string TestFileSuffix() => "-test1";
        public override string TestFileSuffix() => "";

        void Add(Dictionary<long, long> dict, long key, long count)
        {
            if (!dict.ContainsKey(key))
                dict.Add(key, 0);
            dict[key] += count;
        }


        public override object Run(bool part2)
        {

            // the stone number, and count of them
            Dictionary<long, long> counts = new();

            foreach (var w in ReadText().Split(' '))
                Add(counts,Int64.Parse(w),1);

            int steps = part2 ? 75 : 25;
            for (int i = 0; i < steps; ++i)
            {
                Dictionary<long, long> nextCounts = new();

                foreach (var p in counts)
                {
                    var n = p.Key;
                    var ns = n.ToString();
                    var nlen = ns.Length;
                    var count = p.Value;

                    if (n == 0)
                        Add(nextCounts, 1, count);
                    else if ((nlen & 1) == 0)
                    {
                        Add(nextCounts, Int64.Parse(ns.Substring(0, nlen / 2)), count);
                        Add(nextCounts, Int64.Parse(ns.Substring(nlen / 2)), count);
                    }
                    else
                        Add(nextCounts, n * 2024, count);
                }

                counts = nextCounts;
            }

            return counts.Sum(p => p.Value);
        }


    }
}