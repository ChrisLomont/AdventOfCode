using System.Collections;

namespace Lomont.AdventOfCode._2020
{
    internal class Day10 : AdventOfCode
    {
    //2020 Day 10 part 1: 2070 in 1512.3 us
    //2020 Day 10 part 2: 24179327893504 in 2036.2 us
        public override object Run(bool part2)
        {
            var ad = ReadLines().Select(Int32.Parse).ToList();
            //ad.Sort();

            // outlet at 0
            // each input can take 1,2,3 lower than rating (output is rating)
            // device can eat 1,2,3 higher 
            
            var dev = ad.Max() + 3; // 

            if (!part2)
            {
                ad.Add(0); // start
                ad.Add(dev);
                ad.Sort();
                return Score(ad);
            }

            ad.Add(0); // start
            ad.Add(dev);
            ad.Sort();
            return Count1(ad,dev);


            return -1;


            var soln = new List<int>();
            soln.Add(0); // start at wall
            var ans2 = -1L;


            // todo - restore this  -too long... need shorter
            Recurse(0,dev,ad, soln);

            return ans2;
            static long Score(List<int> soln)
            {
                long c1 = 0, c3 = 0;
                for (var i = 0; i < soln.Count - 1; ++i)
                {
                    var d = Math.Abs(soln[i] - soln[i + 1]);
                    if (d == 1) c1++;
                    if (d == 3) c3++;
                }

                //Console.WriteLine($"Diffs {c1} {c3}");

                return c1 * c3;
            }

            // memoize
            static long Count1(List<int> vals, int targetVal)
            {
                var memo = new Dictionary<(int, int), long>();
                Trace.Assert(vals[0] == 0);
                return Help(0, 0, targetVal, vals);

                // curInd is last one consumed
                long Help(int curVal, int curInd, int targ, List<int> v)
                {
                    var key = (curVal, curInd);
                    if (!memo.ContainsKey(key))
                    {
                        long count = 0;
                        if (curInd >= v.Count-1)
                        {
                            return 1;
                        }
                        else
                        {
                            var nxtInd = curInd + 1;
                            while (nxtInd < v.Count && v[nxtInd] - v[curInd] <= 3)
                            {
                                count += Help(v[nxtInd], nxtInd, targ, v);
                                nxtInd++;
                            }
                            //else
                            //    Console.WriteLine("oops");

                            ++curInd;
                            memo.Add(key, count);
                            return count;
                        }
                    }

                    return memo[key];

                }
            }

            void Recurse(int currentVal, int targetVal, List<int> vals, List<int> soln)
            {
                if (vals.Count == 0)
                {
                    if (targetVal == currentVal + 3)
                    {
                        soln.Add(targetVal);
                        ans2 = Score(soln);
                        soln.RemoveAt(soln.Count - 1);
                        Console.WriteLine($"Solved! {ans2}");
                        //Dump(soln);
                    }
                    else
                        Console.WriteLine("Failed!");

                }
                else
                {
                    for (var del = 0; del <=3; ++del)
                    {
                        var nextVal = currentVal + del;
                        if (vals.Contains(nextVal))
                        {
                            vals.Remove(nextVal);
                            soln.Add(nextVal);
                            Recurse(nextVal, targetVal, vals, soln);
                            soln.RemoveAt(soln.Count-1);
                            vals.Add(nextVal);
                            vals.Sort(); // keep ordered
                        }
                    }
                }
            }




        }
    }
}