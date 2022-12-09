
namespace Lomont.AdventOfCode._2022
{
    // 1823
    // 211680

    /*
    // some days didn't compete

          --------Part 1---------   --------Part 2---------
    Day       Time    Rank  Score       Time    Rank  Score
      8   00:05:01     138      0   00:14:05     296      0
      7   00:15:48     347      0   00:20:11     357      0
      6   00:02:18     215      0   00:05:05    1208      0
      5   00:19:42    2673      0   00:22:17    2334      0
      4   00:03:24     498      0   00:05:27     555      0
      3   06:39:23   48009      0   06:44:04   42756      0
      2   11:24:55   87211      0   11:31:31   80993      0
      1       >24h  146240      0       >24h  141012      0
     */

    internal class Day08 : AdventOfCode
    {
        public override object Run(bool part2)
        {
            var (w,h,g) = DigitGrid();

            var visCount = 0;
            var bestScore = 0;
            for (var i = 0; i < w; ++i)
            for (var j = 0; j < h; ++j)
            {
                var (vis1, dst1) = Count(i, j, 1, 0);
                var (vis2, dst2) = Count(i, j, -1, 0);
                var (vis3, dst3) = Count(i, j, 0, 1);
                var (vis4, dst4) = Count(i, j, 0, -1);
                visCount += (vis1 | vis2 | vis3 | vis4);
                var score = dst1 * dst2 * dst3 * dst4;
                if (score > bestScore) bestScore = score;
            }

            if (part2)
                return bestScore;
            return visCount;

            (int isVisible,int sightDistance) Count(int i, int j, int di, int dj)
            {
                int isVisible;
                var sightDistance = 0;
                var ijHeight = g[i, j];
                while (true)
                {
                    i += di;
                    j += dj;
                    if (i < 0 || j < 0 || w <= i || h <= j)
                    {
                        isVisible = 1;
                        break;
                    }
                    // this is the tricky part -
                    // must increment between these
                    // two break conditions!
                    sightDistance++; 
                    if (g[i, j] >= ijHeight)
                    {
                        isVisible = 0;
                        break;
                    }

                }

                return (isVisible, sightDistance);
            }
            

        }
    }
}