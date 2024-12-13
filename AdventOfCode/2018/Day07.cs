

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Security.Cryptography;

namespace Lomont.AdventOfCode._2018
{
    internal class Day07 : AdventOfCode
    {
        object Run2()
        {
            // for each letter, point to predecessors
            Dictionary<char, string> parents = new();


            // index by 'A'+, each is string of parents
            foreach (var line in ReadLines())
            {
                // Step F must be finished before step E can begin.
                var w = line.Split(' ');
                var (src, dst) = (w[1][0], w[7][0]); // src->dst, save parents
                Ensure(src);
                Ensure(dst);
                if (!parents[dst].Contains(src))
                    parents[dst] += src;
                // Console.WriteLine($"{a}->{b} {pairs.Last()}");
            }

            void Ensure(char t)
            {
                if (!parents.ContainsKey(t))
                    parents.Add(t, "");
            }

            // CABDFE 
            // part 1 GKRVWBESYAMZDPTIUCFXQJLHNO
            // part2 1107 too high

            int n = 1+4;   // or 2,5
            int del = 60; // or 0,60
            (int timeLeft, char token) [] workers = new(int,char)[n];
            for (var i = 0; i < workers.Length; i++)
                workers[i] = new(0,'#');

            string finished = "";
            string inFlight = "";
            int time = 0;
            while (true)
            {
                Console.Write($"{time} : ");


                // remove a second from each
                for (int i = 0; i < n; ++i)
                {
                    {
                        if (workers[i].timeLeft > 0)
                        {
                            workers[i].timeLeft--;
                            if (workers[i].timeLeft == 0)
                            {
                                var t = workers[i].token.ToString();
                                finished += t;
                                inFlight = inFlight.Replace(t, "");
                                Console.Write($"worker {i} finishes {t}, ");
                            }
                        }
                    }
                }

                var workerFree = workers.Any(w => w.timeLeft == 0);
                if (workerFree && finished.Length + inFlight.Length < parents.Count)
                {
                    // get those with no parents left that do not appear in ans
                    var free = "";
                    var notInPlay = finished + inFlight; 
                    foreach (var p in parents)
                    {
                        if (!notInPlay.Contains(p.Key) && Used(p.Value, finished))
                            free += p.Key;
                    }

                    Console.Write($"Free {free}, ");

                    // start all free workers
                    for (int i = 0; i < n; ++i)
                    {
                        if (free == "") break;
                        if (workers[i].timeLeft == 0)
                        {
                            var t = free[0]-'A'+1 + del; 
                            workers[i] = (t, free[0]);
                            inFlight += free[0];
                            Console.Write($"worker {i} takes {free[0]}, ");
                            free = free.Remove(0);
                        }
                    }
                }


                Console.WriteLine($" inflight {inFlight}  done {finished}");

                if (finished.Length == parents.Count) break;

                time++;


            }

            bool Used(string parents, string free)
            {
                return parents.All(c => free.Contains(c));
            }

            return time;
        }

        object Run1()
        {
            // for each letter, point to predecessors
            Dictionary<char, string> parents = new();


            // index by 'A'+, each is string of parents
            foreach (var line in ReadLines())
            {
                // Step F must be finished before step E can begin.
                var w = line.Split(' ');
                var (src, dst) = (w[1][0], w[7][0]); // src->dst, save parents
                Ensure(src);
                Ensure(dst);
                if (!parents[dst].Contains(src))
                    parents[dst] += src;
                // Console.WriteLine($"{a}->{b} {pairs.Last()}");
            }

            void Ensure(char t)
            {
                if (!parents.ContainsKey(t))
                    parents.Add(t, "");
            }

            // CABDFE 
            // part 1 GKRVWBESYAMZDPTIUCFXQJLHNO
            string order = "";

            while (true)
            {
                // get those with no parents left that do not appear in ans
                var free = "";
                foreach (var p in parents)
                {
                    if (!order.Contains(p.Key) && Used(p.Value, order))
                        free += p.Key;
                }

               // Console.WriteLine($"Free {free}");

                if (free.Length == 0) break;
                order += free.Min(c => c);

                
            }

            bool Used(string parents, string free)
            {
                return parents.All(c => free.Contains(c));
            }

                return order;

        }



        public override object Run(bool part2)
        {
            return part2 ? Run2() : Run1();
        }
    }
}