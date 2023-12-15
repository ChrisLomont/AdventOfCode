using System.Runtime.InteropServices.JavaScript;

namespace Lomont.AdventOfCode._2023
{
    internal class Day14 : AdventOfCode
    {
        /*

2023 Day 14 part 1: 110821 in 5001.1 us
2023 Day 14 part 2: 83516 in 1451318.9 us

              --------Part 1---------   --------Part 2--------
Day       Time    Rank  Score       Time   Rank  Score
 14   00:06:13     380      0   00:37:35   1066      0
 13   00:19:20    1004      0   00:57:07   2592      0
 12   00:12:11     274      0   01:42:51   1938      0

         First hundred users to get both stars on Day 14:

  1) Dec 14  00:05:46  xiaowuc1
  2) Dec 14  00:07:55  jebouin (AoC++)
  3) Dec 14  00:08:41  Carl Schildkraut
  4) Dec 14  00:09:06  Kirby703
  5) Dec 14  00:09:28  Pi-Hsun Shih

         95) Dec 14  00:17:07  Omkar Bhalerao
 96) Dec 14  00:17:09  Tim Black
 97) Dec 14  00:17:10  Defelo (AoC++)
 98) Dec 14  00:17:12  Robert P
 99) Dec 14  00:17:13  LyricLy
100) Dec 14  00:17:15  Epiphane (AoC++)
First hundred users to get the first star on Day 14:

  1) Dec 14  00:01:28  xiaowuc1
  2) Dec 14  00:01:54  Carl Schildkraut
  3) Dec 14  00:01:58  Ian DeHaan
  4) Dec 14  00:02:13  Laurentiu Ionele (Sponsor)
  5) Dec 14  00:02:23  tckmn
  6) Dec 14  00:02:26  Robert Xiao
         */

        //        110821, 83516

        /// <summary>
        /// Class to help project things that are periodic
        /// out to huge numbers of runs
        /// </summary>
        class PeriodProjector
        {
            readonly long periodRuns; // period checks in a row
            readonly long periodMax;  // guess

            readonly List<long> values; // the data to look for periods at end of list
            
            /// <summary>
            /// The period, -1 if none found yet
            /// </summary>
            public long Period { get; set; } = -1; // marks not found yet

            /// <summary>
            /// 
            /// </summary>
            /// <param name="values">A container that will hold values to search the end of</param>
            /// <param name="periodMax">periods of length 1 to this will be searched</param>
            /// <param name="periodRuns">the number of times a period must match to count</param>
            public PeriodProjector(
                List<long> values,
                long periodMax = 20, 
                long periodRuns = 5  
                )
            {
                this.values = values;
                this.periodMax = periodMax;
                this.periodRuns = periodRuns;
            }

            /// <summary>
            /// Is a period found? If not, add more examples
            /// </summary>
            /// <returns></returns>
            public bool Found()
            {
                // already found
                if (Period != -1)
                    return true;

                // period length
                var testPeriod = 0;

                while (testPeriod++ < periodMax)
                {
                    if (TestPeriod(testPeriod))
                    {
                        Period = testPeriod;
                        return true;
                    }
                }

                return false;
            }

            /// <summary>
            /// Project the data forward to the given position
            /// </summary>
            /// <param name="position">The position to project to</param>
            /// <returns>the projected value at that position, and the index of the matching data</returns>
            public (long valueIndex, long value) Project(long position)
            {
                // back off from the end of the data to allow room to match
                var checkStart = values.Count  - (periodRuns-2) * Period;

                // how many period from there?
                var periods = (position - checkStart) / Period;

                // the index of the data
                // why -1? seems weird? check carefully
                var index = position - periods * Period - 1;
                return (index, values[(int)index]);
            }

            /// <summary>
            /// See if the test period matches the tail of the data
            /// </summary>
            /// <param name="testPeriod"></param>
            /// <returns></returns>
            bool TestPeriod(int testPeriod)
            {
                var periodStart = (int)(values.Count - periodRuns * periodMax - 1);
                if (periodStart < 0) 
                    return false;
                for (var k = periodStart; k < periodStart + periodRuns * testPeriod; ++k)
                {
                    if (values[k] != values[k + testPeriod])
                        return false;
                }
                return true;
            }
        }

        public override object Run(bool part2)
        {
            var (w, h, g) = CharGrid();

            if (!part2)
            {
                Cycle(0,1);
                return Score();
            }

            long cycles = 1000000000;

            List<long> cycleScores = new List<long>();
            var periodHelper = new PeriodProjector(
                cycleScores,
                periodRuns:5,
                periodMax:20
                );

            while (!periodHelper.Found())
            {
                for (int i = 0; i < 100; ++i)
                {
                    Cycle();
                    cycleScores.Add(Score());
                }
            }

            var (scoreIndex,finalScore) = periodHelper.Project(cycles);
            // Console.WriteLine($"period {periodHelper.Period} guess index {scoreIndex} score {finalScore}");
            return finalScore;

            // cycle the moves N,W,S,E
            // dir 0=n,1=w,2=s,3=e
            void Cycle(int dirStart = 0, int dirEnd = 4)
            {
                for (var pass = dirStart; pass < dirEnd; ++pass)
                {
                    int dir = pass % 4;

                    // 0 -> (-1, 0)
                    // 1 -> ( 0,-1)
                    // 2 -> (+1, 0)
                    // 3 -> ( 0,+1)
                    // todo - 
                    var (di, dj) = dir switch
                    {
                        0 => (-1, +0), // N
                        1 => (+0, -1), // W
                        2 => (+1, +0), // S
                        3 => (+0, +1), // E
                        _ => throw new Exception("not impl")
                    };
                    //Console.WriteLine($"{di},{dj}");


                    // embed direction logic here
                    var usej = dj == 0;
                    var (x1, x2, dx) = (0, w, 1);
                    if (dj == 1) (x1,x2,dx) = (w-1,-1,-1);
                    var (y1, y2, dy) = (0, h, 1);
                    if (di == 1) (y1, y2, dy) = (h-1, -1, -1);
                    var dk = di + dj;
                    var k2 = -1;
                    if (dir > 1) k2 = (dir == 2) ? h : w;

                    ///   110821, 83516


                    for (var i = x1; i != x2; i += dx)
                    for (var j = y1; j != y2; j += dy)
                    {
                        if (g[i, j] == 'O')
                        {
                            g[i, j] = '.';
                            var k = usej ? j : i;
                            while (k != k2 && (usej ? g[i, k] : g[k, j]) == '.')
                                k += dk;
                            k -= dk;
                            if (usej)
                                g[i, k] = 'O';
                            else
                                g[k, j] = 'O';

                        }
                    }
                }
            }

 

            long Score()
            {
                long score = 0;
                for (var i = 0; i < w; ++i)
                for (var j = 0; j < h; ++j)
                    if (g[i, j] == 'O')
                        score += h - j;
                return score;
            }


        }

   }
}