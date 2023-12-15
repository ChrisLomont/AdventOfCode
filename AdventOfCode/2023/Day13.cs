using System.Data;
using System.Diagnostics.CodeAnalysis;

namespace Lomont.AdventOfCode._2023
{
    internal class Day13 : AdventOfCode
    {
        /*
      --------Part 1---------   --------Part 2--------
Day       Time    Rank  Score       Time   Rank  Score
 13   00:19:20    1004      0   00:57:07   2592      0
 12   00:12:11     274      0   01:42:51   1938      0
 11   00:22:07    2485      0   00:34:57   2712      0       
        

First hundred users to get both stars on Day 13:

  1) Dec 13  00:06:06  Anish Singhani (AoC++)
  2) Dec 13  00:06:11  mangoqwq
  3) Dec 13  00:06:39  boboquack
  4) Dec 13  00:06:53  jonathanpaulson (AoC++)
  5) Dec 13  00:07:05  nthistle (AoC++) (Sponsor)
  6) Dec 13  00:07:35  @AlexSkidanov

 96) Dec 13  00:13:39  wisewizardofthestars
 97) Dec 13  00:13:41  RWayne93
 98) Dec 13  00:13:42  nick-hiebl
 99) Dec 13  00:13:46  Satya Anshu
100) Dec 13  00:13:46  Iliya Yanev (AoC++)
First hundred users to get the first star on Day 13:

  1) Dec 13  00:03:51  Ian DeHaan
  2) Dec 13  00:04:15  Antonio Molina (AoC++) (Sponsor)
  3) Dec 13  00:04:26  Oskar Haarklou Veileborg (AoC++)
  4) Dec 13  00:04:55  Anish Singhani (AoC++)
  5) Dec 13  00:04:56  Xavier Rene Plourde

 98) Dec 13  00:08:55  Zack Lee
 99) Dec 13  00:08:57  Dmitry Ivanov
100) Dec 13  00:08:58  zachs18 (AoC++)

2023 Day 13 part 1: 34918 in 90073.6 us
2023 Day 13 part 2: 33054 in 101350 us

         */

        public override object Run(bool part2)
        {
            long answer = 0;

            var blocks =
                ReadText()
                    .Split("\n\n")
                    .Select(
                        b => b.Split('\n', StringSplitOptions.RemoveEmptyEntries)
                        );
            
            foreach (var block in blocks)
            {
                var (w, h, g) = CharGrid(block);

                for (var col = 0; col < w; ++col)
                {
                    var err = 0;
                    for (var (i1, i2) = (0, 2 * col - 1); i1 < col; ++i1, --i2)
                        if (i2 < w)
                            for (var j = 0; j < h; ++j)
                                err += g[i1, j] != g[i2, j] ? 1 : 0;

                    if (err == (part2 ? 1 : 0))
                        answer += col;
                }

                for (var row = 0; row < h; ++row)
                {
                    var err = 0;
                    for (var i = 0; i < w; ++i)
                    for (var (j1, j2) = (0, 2 * row - 1); j1 < row; ++j1, --j2)
                        if (j2 < h)
                            err += g[i, j1] != g[i, j2] ? 1 : 0;

                    if (err == (part2 ? 1 : 0))
                        answer += 100*row;
                }
            }

            return answer;


#if false


                var (vecsR,_1R,_2R) = DoRow(g, w, h);
                foreach (var v1 in vecsR)
                {
                    Console.WriteLine($"Toggle row! {v1}");
                    // this was it!
                    g[v1.x, v1.y] = g[v1.x, v1.y] == '.' ? '#' : '.';
                    var (v, sum, count) = DoRow(g, w, h);
                    g[v1.x, v1.y] = g[v1.x, v1.y] == '.' ? '#' : '.';
                    if (count == 1)
                    {
                        colsum += sum;
                        Console.WriteLine($"Woot! {sum}");
                    }
                }

                var (vecsC, _1C, _2C) = DoCol(g, w, h);
                foreach (var v1 in vecsC)
                {
                    Console.WriteLine($"Toggle col! {v1}");
                    // this was it!
                    g[v1.x, v1.y] = g[v1.x, v1.y] == '.' ? '#' : '.';
                    var (v, sum, count) = DoCol(g, w, h);
                    g[v1.x, v1.y] = g[v1.x, v1.y] == '.' ? '#' : '.';
                    if (count == 1)
                    {
                        rowsum += sum;

                        Console.WriteLine($"Woot! {sum}");
                    }
                }


                vec2 last = new vec2();
                bool hasFlip = false;
                /*
                // pass1, fix errors, 2nd, run counts
                for (int pass = 0; pass < 2; ++pass)
                {
                    for (int splitCol = 0; splitCol <= w; ++splitCol)
                    {
                        var k = 0;
                        var split = splitCol + k < w && 0 <= splitCol - k - 1;
                        var errs = 0;
                        while (split && splitCol + k < w && 0 <= splitCol - k - 1)
                        {
                            var r1 = Col(g, splitCol - k - 1).ToList();
                            var r2 = Col(g, splitCol + k).ToList();
                            var (success, errorCount, errorIndex) = Same(r1,r2);
                            errs += errorCount;
                            if (errorCount == 1)
                                last = new vec2(errorIndex, splitCol - k - 1);
                            split &= success;
                            ++k;
                        }

                        if (errs == 1 && pass == 0)
                        {
                            Console.WriteLine($"Toggle col! {last}");
                            // this was it!
                            g[last.x, last.y] = g[last.x, last.y] == '.' ? '#' : '.';
                            hasFlip = true;
                        }

                        if (split && pass == 1)
                        {
                            Console.WriteLine($"Col split {splitCol}");
                            colsum += splitCol; // # to left
                        }
                    }
                }
                */
                /*

                // pass1, fix errors, 2nd, run counts
                for (int pass = 0; pass < 2; ++pass)
                {
                    for (int splitCol = 0; splitCol <= h; ++splitCol)
                    {
                        var k = 0;
                        var split = splitCol + k < h && 0 <= splitCol - k - 1;
                        var errs = 0;
                        while (split && splitCol + k < h && 0 <= splitCol - k - 1)
                        {
                            var r1 = Row(g, splitCol - k - 1).ToList();
                            var r2 = Row(g, splitCol + k).ToList();
                            var (success, errorCount, errorIndex) = Same(r1, r2);
                            errs += errorCount;
                            if (errorCount == 1)
                                last = new vec2(errorIndex, splitCol - k - 1);
                            split &= success;
                            ++k;
                        }

                        if (errs == 1 && pass == 0 && !hasFlip)
                        {
                            Console.WriteLine($"Toggle row! {last}");
                            // this was it!
                         //   g[last.x, last.y] = g[last.x, last.y] == '.' ? '#' : '.';
                        }

                        if (split && pass == 1)
                        {
                            Console.WriteLine($"Row split {splitCol}");
                            rowsum += splitCol; // # to left
                        }
                    }
                }
                */
            }

            return answer;

            return 100 * rowsum + colsum;

            static (List<vec2> vecs,long sum, int count) DoRow(char[,]g, int w, int h)
            {
                // pairs of possible toggles
                List<vec2> vecs = new();
                long sum = 0;
                int count = 0;

                vec2 v1 = new(), v2 = new(); 
                for (int splitCol = 0; splitCol <= w; ++splitCol)
                {
                    var k = 0;
                    var split = splitCol + k < w && 0 <= splitCol - k - 1;
                    var errs = 0;
                    while (split && splitCol + k < w && 0 <= splitCol - k - 1)
                    {
                        var r1 = Col(g, splitCol - k - 1).ToList();
                        var r2 = Col(g, splitCol + k).ToList();
                        var (success, errorCount, errorIndex) = Same(r1, r2);
                        errs += errorCount;
                        if (errorCount == 1)
                        {
                            v1 = new vec2(errorIndex, splitCol - k - 1);
                            v2 = new vec2(errorIndex, splitCol + k);
                        }

                        split &= success;
                        ++k;
                    }

                    if (errs == 1)
                    {
                        vecs.Add(v1);
                        vecs.Add(v2);
                    }

                    if (split)
                    {
                        sum += splitCol; // # to left
                        ++count;
                    }
                }

                return (vecs,sum,count);
            }

            static (List<vec2> vecs, long sum, int count) DoCol(char[,] g, int w, int h)
            {
                // pairs of possible toggles
                List<vec2> vecs = new();
                long sum = 0;
                int count = 0;

                vec2 v1 = new(), v2 = new();
                for (int splitCol = 0; splitCol <= w; ++splitCol)
                {
                    var k = 0;
                    var split = splitCol + k < h && 0 <= splitCol - k - 1;
                    var errs = 0;
                    while (split && splitCol + k < h && 0 <= splitCol - k - 1)
                    {
                        var r1 = Row(g, splitCol - k - 1).ToList();
                        var r2 = Row(g, splitCol + k).ToList();
                        var (success, errorCount, errorIndex) = Same(r1, r2);
                        errs += errorCount;
                        if (errorCount == 1)
                        {
                            v1 = new vec2(errorIndex, splitCol - k - 1);
                            v2 = new vec2(errorIndex, splitCol + k);
                        }

                        split &= success;
                        ++k;
                    }

                    if (errs == 1)
                    {
                        vecs.Add(v1);
                        vecs.Add(v2);
                    }

                    if (split)
                    {
                        sum += splitCol; // # to left
                        ++count;
                    }
                }

                return (vecs, sum, count);
            }

            // match, if not, error count, error index of last error

            static (bool success,int errorCount, int errorIndex) Same(List<char> r1, List<char> r2)

            {
                if (r1.Count != r2.Count)
                {
                    throw new NoNullAllowedException("VOOV");
                }

                var success = true;
                int errorCount = 0;
                int errorIndex = -1;
                for (var i = 0; i < r1.Count; ++i)
                {
                    if (r1[i] != r2[i])
                    {
                        success = false;
                        errorCount++;
                        errorIndex = i;
                    }
                }

                return (success,errorCount,errorIndex);
            }

#endif

        }
    }
}