using System;
using System.Runtime.InteropServices.ComTypes;

namespace Lomont.AdventOfCode._2023
{
    internal class Day06 : AdventOfCode
    {

        /*
              --------Part 1---------   --------Part 2--------
        Day       Time    Rank  Score       Time   Rank  Score
          6   00:05:05     515      0   00:14:05   2506      0
          5   00:23:11    1843      0   01:43:15   3581      0
          4   00:09:08    2699      0   00:21:16   2451      0
          3   00:19:04    1331      0   00:29:57   1410      0
          2   00:06:03     329      0   00:09:23    411      0
          1   17:45:07  131162      0   18:25:46  92867      0  



First hundred users to get both stars on Day 6:

  1) Dec 06  00:02:05  Nordine Lotfi
  2) Dec 06  00:02:25  Neal Wu
  3) Dec 06  00:02:28  Zack Chroman
  4) Dec 06  00:02:48  Zack Lee

 97) Dec 06  00:05:00  LegionMammal978
 98) Dec 06  00:05:01  Kunal Chattopadhyay
 99) Dec 06  00:05:01  Balint R
100) Dec 06  00:05:02  Alex Waese-Perlman


First hundred users to get the first star on Day 6:

  1) Dec 06  00:01:15  siraben
  2) Dec 06  00:01:29  Nordine Lotfi
  3) Dec 06  00:01:33  Zack Chroman
  4) Dec 06  00:01:39  Neal Wu
  5) Dec 06  00:01:45  James Satherley

 96) Dec 06  00:03:10  (anonymous user #975121)
 97) Dec 06  00:03:10  John Kesler
 98) Dec 06  00:03:10  PenguinEncounter
 99) Dec 06  00:03:11  bluepichu
100) Dec 06  00:03:11  Epiphane (AoC++)

2023 Day 6 part 1: 227850 in 3048.5 us
2023 Day 6 part 2: 42948149 in 122663.6 us
                */

        public override object Run(bool part2)
        {

            var lines = ReadLines();
            if (part2)
            {
                lines[0] = lines[0].Replace(" ", "");
                lines[1] = lines[1].Replace(" ", "");
            }
            
            var times = Numbers64(lines[0]);
            var dists = Numbers64(lines[1]);

             long prod = 1;
             for (var i = 0; i < times.Count; ++i)
             {
                 var (t,d) = (times[i],dists[i]);
                 
                 // both work 
#if true
                var count = 0;
                for (var c = 0L; c <= t; ++c)
                    if (t * c - c * c > d)
                        count++;
#else

                double disc  = (Double.Floor(Double.Sqrt(t*t-4*d)));
                double lower = Double.Ceiling((t-disc)/2);
                double upper = Double.Floor((t + disc) / 2);
                long count= (long)(upper - lower + 1);
                //long low1 = (t - disc) / 2;
                //long upp1 = (t + disc) / 2;
                //long count = upp1 - low1+1;
#endif

                prod *= count;
             }
             return prod;
        }
    }
}