

using System.Diagnostics.Metrics;
using System.Runtime.InteropServices.Marshalling;
using System.Security.AccessControl;

namespace Lomont.AdventOfCode._2018
{
    /**
     * 2018 Day 12 part 1: 2349 in 2024.4 us
2018 Day 12 part 2: 2100000001168 in 20011.6 us
     */
    internal class Day12 : AdventOfCode
    {
        // hashlife:
        // can jump by ~64 steps using 64 bit integers
        // 
        object Run2()
        {
            int lineNumber = 0;
            var gens = 1000;
            int[] arr = new int[1];
            int size = 0, mid = 0;
            int[] rules = new int[32];

            // index i is score at gen i
            List<long> history = new();

            foreach (var line in ReadLines())
            {
                if (lineNumber == 0)
                {
                    var l2 = line.Replace("initial state: ", "");
                    size = (l2.Length + gens) * 3;
                    mid = size / 2;
                    arr = new int[size];
                    for (int i = 0; i < l2.Length; i++)
                        if (l2[i] == '#')
                        {
                            arr[i + mid] = 1;
                        }
                }
                else if (line != "")
                {
                    var rule = 0;
                    for (int i = 0; i < 5; i++)
                        rule = 2 * rule + (line[i] == '#' ? 1 : 0);
                    rules[rule] = line[9] == '#' ? 1 : 0;
                }

                lineNumber++;
            }

            // return start index
            long Dump(int gen)
            {
                Console.Write($"{gen:D4}: ");

                var fi = Array.FindIndex(arr, v=>v==1);
                var fe = Array.FindLastIndex(arr, v => v == 1);

                Console.Write($"{gen:D4} ({fi}) {Sum()}: ");

                for (int i = fi-3; i < fe+3; ++i)
                {
                    var c = arr[i];
                    if (i == mid)
                        Console.Write(c == 0 ? 'o' : 'O');
                    else
                        Console.Write(c == 0 ? '.' : '#');
                }

                Console.WriteLine();
                return fi;
            }
            for (int g = 0; g <= gens; ++g)
            {
                history.Add(Sum());
                //Dump(g);

                int[] nxt = new int[size];

                if (g == gens) break;


                for (var c = 2; c < size - 2; c++)
                {
                    var rule = 0;
                    for (int dc = -2; dc <= 2; ++dc)
                        rule = 2 * rule + arr[c + dc];
                    nxt[c] = rules[rule];
                }

                arr = nxt;
            }

            // jump per score from here on out
            var del = history[gens - 1] - history[gens - 2];
            long future = 1000;
            future = 50000000000L;
            var score = (future - gens+1) * del + history[gens - 1];
            return score;

            // score 1000: 43126


            var shift = Dump(gens);
            var sum = Sum();
            //long future = 50000000000 - gens;


            // too low: 50000009168
            // 2100000001126 too low

            long Sum()
            {
                var sum = 0;
                for (var i = 0; i < arr.Length; i++)
                {
                    if (arr[i] != 0)
                        sum += (i - mid);
                }

                return sum;
            }

            return (sum-gens)+400;
        }

        object Run1()
        {
            int lineNumber = 0;
            var gens = 20;
            int[] arr = new int[1];
            int size = 0, mid = 0;
            int [] rules = new int[32];
            foreach (var line in ReadLines())
            {
                if (lineNumber == 0)
                {
                    var l2 = line.Replace("initial state: ", "");
                    size = (l2.Length + gens) * 3;
                    mid = size / 2;
                    arr = new int[size];
                    for (int i =0 ; i < l2.Length ; i++)
                        if (l2[i] == '#')
                        {
                            arr[i + mid] = 1;
                        }
                }
                else if (line != "")
                {
                    var rule = 0;
                    for (int i = 0; i < 5; i++)
                        rule = 2 * rule + (line[i] == '#' ? 1 : 0);
                    rules[rule] = line[9] == '#' ? 1 : 0;
                }
                lineNumber++;
            }

            for (int g = 0; g <= gens; ++g)
            {
#if false
                Console.Write($"{g:D2}: ");
                for (int i = 0; i < size; ++i)
                {
                    var c = arr[i];
                    if (i == mid)
                        Console.Write(c==0?'o':'O');
                    else
                        Console.Write(c == 0 ? '.' : '#');
                }

                Console.WriteLine();
#endif

                int[] nxt = new int[size];
                
                if (g == gens) break;


                for (var c = 2; c < size - 2; c++)
                {
                    var rule = 0;
                    for (int dc = -2; dc <= 2; ++dc)
                        rule = 2 * rule + arr[c + dc];
                    nxt[c] = rules[rule];
                }
                arr = nxt;
            }

            var sum = 0;
            for (var i = 0; i < arr.Length; i++)
            {
                if (arr[i] != 0)
                    sum += (i-mid);
            }

            return sum;
        }

        public override object Run(bool part2)
        {
            return part2 ? Run2() : Run1();
        }
    }
}