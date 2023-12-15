using System.Numerics;

namespace Lomont.AdventOfCode._2023
{
    internal class Day11 : AdventOfCode
    {
        /*
First hundred users to get both stars on Day 11:

  1) Dec 11  00:03:37  xiaowuc1
  2) Dec 11  00:04:06  Kevin Wang (AoC++)
  3) Dec 11  00:04:14  Oliver Ni (AoC++)
  4) Dec 11  00:04:16  bluepichu
  5) Dec 11  00:04:18  Antonio Molina (AoC++) (Sponsor)
  6) Dec 11  00:04:35  Lewin Gan

95) Dec 11  00:09:08  dllu
 96) Dec 11  00:09:08  Özgün Özusta
 97) Dec 11  00:09:08  Teferi
 98) Dec 11  00:09:13  Adalbert
 99) Dec 11  00:09:15  scanhex
100) Dec 11  00:09:18  Samuel Chen
First hundred users to get the first star on Day 11:

  1) Dec 11  00:02:00  superjarle
  2) Dec 11  00:02:16  dan-simon
  3) Dec 11  00:02:21  tckmn
  4) Dec 11  00:02:23  Adavya Goyal
  5) Dec 11  00:02:30  Kroppeb (AoC++)
  6) Dec 11  00:02:34  bluepichu
 97) Dec 11  00:06:06  HiggstonRainbird
 98) Dec 11  00:06:06  scanhex
 99) Dec 11  00:06:07  Noble Mushtak
100) Dec 11  00:06:07  Alexander Pfanne (AoC++)

      --------Part 1---------   --------Part 2--------
Day       Time    Rank  Score       Time   Rank  Score
 11   00:22:07    2485      0   00:34:57   2712      0
 10   01:24:17    5932      0   02:12:30   2882      0
  9   00:10:00    1239      0   00:13:20   1181      0
  8   00:06:36     922      0   00:22:04    905      0
  7   00:30:34    2592      0   00:38:43   1609      0

        2023 Day 11 part 1: 9648398 in 44559.8 us
        2023 Day 11 part 2: 618800410814 in 13955.9 us
        */

        public override object Run(bool part2)
        {
            // replace empty row or column with this many
            long ex = part2 ? 1_000_000 : 2;

            var (w, h, g) = CharGrid();

            var expandCols = Enumerable.Range(0, w).Where(i => !Col(g, i).Contains('#')).ToList();
            var expandRows = Enumerable.Range(0, h).Where(j => !Row(g, j).Contains('#')).ToList();

            var pts = Gather(g, c => c == '#').ToList();

            var answer = pts.SelectMany(_ => pts, (x, y) => (a:x, b:y)).Sum(p => Cost(p.a, p.b)) / 2;

            return answer;

            long Cost(vec2 a, vec2 b) => 
                Score(a.x, b.x, expandCols) + Score(a.y, b.y, expandRows);
            long Score(long a, long b, List<int> items) => 
                items.Count(c => Math.Min(a,b) <= c && c <= Math.Max(a,b)) * (ex - 1) + Math.Abs(a - b);
        }
    }

}