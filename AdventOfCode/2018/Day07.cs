

using System.Collections.Generic;
using System.Collections.Immutable;

namespace Lomont.AdventOfCode._2018
{
    internal class Day07 : AdventOfCode
    {
        object Run2()
        {
            long answer = 0;

            foreach (var line in ReadLines())
            {
                var nums = Numbers64(line);
            }

            return answer;
        }


        object Run1()
        {
            long answer = 0;
            // var (w,h,g) = DigitGrid();
            // var (w,h,g) = CharGrid();
            // ProcessAllLines(new() { ... regex->action list ... });
            var pairs = new List<(char a,char b)>();
            var names = new HashSet<char>();
            foreach (var line in ReadLines())
            {
                // Step F must be finished before step E can begin.
                var w = line.Split(' ');
                var (a, b) = (w[1], w[7]);
                Trace.Assert(a.Length == 1 && b.Length == 1);
                pairs.Add((a[0], b[0]));
                Console.WriteLine($"{a}->{b} {pairs.Last()}");
                names.Add(a[0]);
                names.Add(b[0]);
            }

            var letters = names.ToList();
            //letters.Sort();
            //letters.Reverse();


            // CABDFE
            //letters = "CABDFE".ToList();

            // fail ABCDGEFIMQJHKSLXNOPRUVZTWY
            //      ABCDGEFIMQJHKSLXNOPRUVZTWY
            // fail ABDWEFGRYHVIKMJLNOPQSTUCZX
            //      ABCDGEFIMQJHKSLXNOPRUVZTWY
            /*
            var n = letters.Count;
            bool done = false;
            while (!done)
            {
                done = true;
                for (int i = 0; i < n - 1; ++i)
                for (int j = i + 1; j < n; ++j)
                {
                    var a1 = letters[i];
                    var b1 = letters[j];
                    var reverse = pairs.Any(p => p.a == b1 && p.b == a1);
                    if (reverse)
                    {
                        letters[i] = b1;
                        letters[j] = a1;
                        done = false;
                        Console.WriteLine($"swap {a1} {b1}");
                    }
                    else
                    {
                        var required = pairs.Any(p => p.a == a1 && p.b == b1);
                        if (!required && b1 < a1)
                        {
                            letters[i] = b1;
                            letters[j] = a1;
                            done = false;
                            Console.WriteLine($"swap {a1} {b1}");
                        }
                    }
                }
            }
            */

            // CABDFE
            //letters.Sort();
            letters.Sort(
                (x, y) =>
                {
                    if (x == y) return 0;
                    foreach (var (fst, lst) in pairs)
                    {
                        if (fst == x && lst == y)
                            return 1;
                        if (fst == y && lst == x)
                            return -1;
                    }

                    return -x.CompareTo(y);
                }
            );

            letters.Reverse();

            return letters.Aggregate("", (a, b) => a + b);
        }

        public override object Run(bool part2)
        {
            return part2 ? Run2() : Run1();
        }
    }
}