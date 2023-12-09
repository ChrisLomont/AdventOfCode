namespace Lomont.AdventOfCode._2023
{
    internal class Day09 : AdventOfCode
    {
        /*

2023 Day 9 part 1: 2098530125 in 8089.2 us
2023 Day 9 part 2: 1016 in 3492.1 us
        
First hundred users to get both stars on Day 9:

  1) Dec 09  00:01:48  Oliver Ni (AoC++)
  2) Dec 09  00:01:57  Cody Carlsen (AoC++)
  3) Dec 09  00:02:33  Louis Grennell

97) Dec 09  00:05:33  Thomas
 98) Dec 09  00:05:35  Patrick Hogg (AoC++) (Sponsor)
 99) Dec 09  00:05:35  Zeyu Chen
100) Dec 09  00:05:36  SimpleArt
First hundred users to get the first star on Day 9:

  1) Dec 09  00:01:16  Mihhail Novik
  2) Dec 09  00:01:21  Cody Carlsen (AoC++)
  3) Dec 09  00:01:28  Oliver Ni (AoC++)
  4) Dec 09  00:01:44  tenth
  5) Dec 09  00:01:48  5space
  6) Dec 09  00:01:51  Louis Grennell

         
 96) Dec 09  00:03:55  qwewqa
 97) Dec 09  00:03:56  Yiming Li
 98) Dec 09  00:03:59  Brett Math
 99) Dec 09  00:04:00  Maks Verver
100) Dec 09  00:04:02  Dan Roche         

      --------Part 1---------   --------Part 2--------
Day       Time    Rank  Score       Time   Rank  Score
  9   00:10:00    1239      0   00:13:20   1181      0
  8   00:06:36     922      0   00:22:04    905      0
  7   00:30:34    2592      0   00:38:43   1609      0
  6   00:05:05     515      0   00:14:05   2506      0
  5   00:23:11    1843      0   01:43:15   3581      0
  4   00:09:08    2699      0   00:21:16   2451      0
  3   00:19:04    1331      0   00:29:57   1410      0
  2   00:06:03     329      0   00:09:23    411      0
  1   17:45:07  131162      0   18:25:46  92867      0

         */

        public override object Run(bool part2)
        {
            long answer = 0;

            // get sequence derivative
            List<long> Der(List<long> seq) => seq.Skip(1).Select((k, i) => k - seq[i]).ToList();

            // get sequence derivatives until all zero
            List<List<long>> Ders(List<long> seq)
            {
                List<List<long>> h = new(){seq};
                while (seq.Any(c => c != 0))
                {
                    seq = Der(seq);
                    h.Add(seq);
                }
                return h;
            }

            // extend lists
            List<List<long>> Next(List<List<long>> h)
            {
                h.Last().Add(0);
                for (var k = h.Count - 2; k >= 0; --k)
                {
                    var c = h[k].Count-1;
                    h[k].Add(h[k + 1][c] + h[k][c]);
                }

                return h;
            }

            // lists prev value
            List<List<long>> Prev(List<List<long>> h)
            {
                h.Last().Insert(0, 0);
                for (var k = h.Count - 2; k >= 0; --k)
                    h[k].Insert(0, h[k][0] - h[k + 1][0]);
                return h;
            }

            foreach (var line in ReadLines())
            {
                var hist = Ders(Numbers64(line));
                answer += part2 
                    ? Prev(hist)[0][0] 
                    : Next(hist)[0].Last();
            }
            return answer;
        }
    }
}