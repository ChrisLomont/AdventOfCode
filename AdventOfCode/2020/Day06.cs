namespace Lomont.AdventOfCode._2020
{
    internal class Day06 : AdventOfCode
    {
    // 2020 Day 6 part 1: 6335 in 7067.6 us
    // 2020 Day 6 part 2: 3392 in 7398.2 us

      

        // todo -= make a tally object
        // todo - move BitCount to AOC base

        public override object Run(bool part2)
        {
            var count = 0;
            foreach (var group in Group(ReadLines()))
            {

                if (!part2)
                {
                    var gg = group.Select(g => g.ToCharArray()).SelectMany(g => g);
                    var tally = Tally(gg);
                    count += tally.Count;
                }
                else
                {
                    // Idea: map each to number, bit set for char present
                    var f = group.Select( // for each group entry
                                s => s.ToCharArray() 
                                    .Select(cc => 1L << (cc - 'a'))
                                    .Sum(v => v))
                            .Aggregate(-1L, (a, b) => a & b) // then for each group, and all numbers together to see what all voted yes on
                        ;
                    // then counting bits gives # all agreed on
                    count += BitCount(f);
                }
            }

            return count;
        }
    }
}