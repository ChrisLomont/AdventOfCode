using System.ComponentModel.DataAnnotations;

namespace Lomont.AdventOfCode._2023
{
    internal class Day04 : AdventOfCode
    {
#if false
    --------Part 1---------   --------Part 2--------
    Day Time    Rank Score       Time Rank  Score
    4   00:09:08    2699      0   00:21:16   2451      0
    3   00:19:04    1331      0   00:29:57   1410      0
    2   00:06:03     329      0   00:09:23    411      0
    1   17:45:07  131162      0   18:25:46  92867      0

First hundred users to get both stars on Day 4:

  1) Dec 04  00:01:22  tung491
  2) Dec 04  00:03:03  Nordine Lotfi
  3) Dec 04  00:03:28  Shawn Hagler
  4) Dec 04  00:03:50  Neal Wu
  5) Dec 04  00:03:50  nthistle (AoC++) (Sponsor)
  6) Dec 04  00:03:53  eniraa
  7) Dec 04  00:03:53  Angelica Naguio
  8) Dec 04  00:03:55  D. Salgado


 96) Dec 04  00:07:07  rak1507
 97) Dec 04  00:07:07  Mercerenies
 98) Dec 04  00:07:08  glguy (AoC++)
 99) Dec 04  00:07:08  Jeffrey Fan
100) Dec 04  00:07:08  Parker Williams
First hundred users to get the first star on Day 4:

  1) Dec 04  00:00:43  tung491
  2) Dec 04  00:01:06  Nordine Lotfi
  3) Dec 04  00:01:22  Ryan O’Hara
  4) Dec 04  00:01:24  Oskar Haarklou Veileborg (AoC++)
  5) Dec 04  00:01:26  Angelica Naguio
  6) Dec 04  00:01:35  Carl Schildkraut

 97) Dec 04  00:02:48  Max Jäger (AoC++)
 98) Dec 04  00:02:50  carro
 99) Dec 04  00:02:50  mrkeldon
100) Dec 04  00:02:51  danzou56
        // 24733, 5422730
#endif

        public override object Run(bool part2)
        {
            var ans1 = 0;

            var lines = ReadLines();
            var counts = Enumerable.Repeat(1, lines.Count).ToList();

            foreach (var (line, i) in lines.WithIndex())
            {
                var w = line.Split(new []{'|',':'});

                var  matches = Intersect(Numbers(w[1]), Numbers(w[2])).Count;

                ans1 += matches == 0 ? 0 : 1 << (matches - 1);

                for (var j = 0; j < matches; ++j)
                    counts[i + 1 + j] += counts[i];
            }

            return part2 ? counts.Sum() : ans1;
        }
    }
}