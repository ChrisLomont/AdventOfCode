using System.Net;
using System.Numerics;

namespace Lomont.AdventOfCode._2023
{

    /*
      --------Part 1---------   --------Part 2--------
Day       Time    Rank  Score       Time   Rank  Score
 12   00:12:11     274      0   01:42:51   1938      0
 11   00:22:07    2485      0   00:34:57   2712      0
    
First hundred users to get both stars on Day 12:

  1) Dec 12  00:05:58  Nathan Fenner (AoC++)
  2) Dec 12  00:08:05  D. Salgado
  3) Dec 12  00:09:33  the-chenergy
  4) Dec 12  00:09:50  @AlexSkidanov

 96) Dec 12  00:22:41  (anonymous user #3813216)
 97) Dec 12  00:22:43  PoustouFlan
 98) Dec 12  00:22:48  (anonymous user #637942)
 99) Dec 12  00:22:54  Tomas Rokicki
100) Dec 12  00:22:57  TINA WANG - STUDENT
First hundred users to get the first star on Day 12:

  1) Dec 12  00:02:02  Dominick Han (AoC++)
  2) Dec 12  00:02:15  Nordine Lotfi
  3) Dec 12  00:03:11  xiaowuc1
  4) Dec 12  00:03:15  Cody Carlsen (AoC++)
  5) Dec 12  00:03:25  glguy (AoC++)
  6) Dec 12  00:03:25  hyper-neutrino
  7) Dec 12  00:03:36  Роман Курмаев

 98) Dec 12  00:08:10  denilblecodeur
 99) Dec 12  00:08:11  pvillano (AoC++)
100) Dec 12  00:08:12  Elmer Le

     */

    internal class Day12 : AdventOfCode
    {
        public override object Run(bool part2)
        {
            //7916,
            // 37366887898686

            checked
            {


                //part2 = false;
                long answer = 0;

                // var (w,h,g) = DigitGrid();
                // var (w,h,g) = CharGrid();

                //Dictionary<(string,string,bool),long> cache = new();

                var lines = ReadLines();
                Parallel.For(0, lines.Count, i =>
//                long count = 0;
//                foreach (var line in ReadLines())
                {
                    // Console.Write(line + " : ");
                    var line = lines[i];
                    var w = line.Split(' ');

                    if (part2)
                    {
                        w[0] = w[0] + '?' + w[0] + '?' + w[0] + '?' + w[0] + '?' + w[0];
                        w[1] = w[1] + ',' + w[1] + ',' + w[1] + ',' + w[1] + ',' + w[1];
                    }

                    // Console.WriteLine($"({w[0]} - {w[1]})");

                    var nums = Numbers64(w[1]);
                    var q = w[0].Sum(c => c == '?' ? 1 : 0);
                    var target = nums.Sum() - w[0].Sum(c => c == '#' ? 1 : 0);
                    //   Console.Write($"{q} & targ {target} ");
                    //Console.WriteLine();
                    //Console.WriteLine(w[0]);
                    var m = 1 << q;
                    //count = Recurse(w[0], 0, nums, 0);
                    //cache.Clear();
                    Dictionary<string, long> memo = new();

                    Interlocked.Add(ref answer, Recurse(w[0], nums, false, memo));

                 //   Console.WriteLine($"Memo {memo.Count}");

#if false
                    Parallel.For(0, m, i =>
                        //for (var i = 0; i < 1 << q; ++i)
                    {
                        //if (BitCount(i) == target)
                        {
                            var t = w[0];
                            t = Repl(t, i, q);
                            //Console.WriteLine($"{t}");
                            var c2 = t.Split('.', StringSplitOptions.RemoveEmptyEntries);
                            if (c2.Length == nums.Count)
                            {
                                var nums2 = c2.Select(c => (long)c.Length).ToList();
                                if (Same(nums2, nums))
                                {
                                    Interlocked.Increment(ref count);
                                }
                            }
                        }
                    });
#endif

                    //  answer += count;
                    //  Console.Write($"{count} ways");

                    //      Console.WriteLine();

                });
                // ProcessAllLines(new() { ... regex->action list ... });

              //  Console.WriteLine();
             //   Console.WriteLine($"Answer {answer}");
             //   Console.WriteLine("--------------------");
            //    Console.WriteLine();

                return answer;

                static long Recurse(string suffix, List<long> nums, bool forcedNum,  Dictionary<string, long> memo)
                {
                    long SingleStep()
                    {
                        if (nums.Sum() == 0)
                            return suffix.Contains('#') ? 0 : 1; // any '#' leftover?

                        if (suffix == "")
                            return 0; // nums had values

                        var (head, rest) = (suffix[0], suffix[1..]);
                        var end = nums[0] == 0;

                        if (head == '.')
                        {
                            if (forcedNum && !end)
                                return 0;
                            if (end)
                                nums.RemoveAt(0);
                            return Recurse(rest, nums, false, memo);
                        }

                        if (head == '#')
                        { // need a number
                            if (end) 
                                return 0;
                            nums[0]--;
                            return Recurse(rest, nums, true, memo);
                        }



                        if (forcedNum) // suffix starts with '?'
                        {
                            if (end)
                                nums.RemoveAt(0); // choose '.'
                            else
                                nums[0]--; // choose '#'
                            return Recurse(suffix[1..], nums, !end, memo);
                        }

                        // no choice made on '?', so sum both paths

                        var numCopy = new List<long>(nums); // copy
                        numCopy[0]--;
                        // both cases: '?' is chosen as . and as #
                        return Recurse(rest, nums, false, memo) + Recurse(rest, numCopy, true, memo);
                    }

                    // uniquely identify state
                    var key = $"{nums[0]},{nums.Count},{suffix},{forcedNum}";
                    if (!memo.ContainsKey(key))
                        memo.Add(key, SingleStep());

                    return memo[key];
                }

                /*
                Assert(false);

                bool Same(List<long> a, List<long> b)
                {
                    if (a.Count != b.Count) return false;
                    for (var i = 0; i < a.Count; ++i)
                        if (a[i] != b[i])
                            return false;
                    return true;
                }

                string Repl(string t, int val, int bits)
                {
                    var arr = t.ToCharArray().ToList();
                    var chIndex = 0;
                    for (long i = 0; i < bits; ++i)
                    {

                        var b = ((val & 1) == 1) ? '#' : '.';
                        val >>= 1;
                        while (arr[chIndex] != '?')
                        {
                            chIndex++;
                        }

                        arr[chIndex] = b;
                    }

                    var s = 
                    new string(arr.ToArray());
                    return s;
                }
                */



                /*
                long Recurse(string s, int si, List<long> nums, int ni)
                {
                    var key = (si, ni);
                  //  if (memo.ContainsKey(key))
                  //      return memo[key];

                  if (ni >= nums.Count)
                  {
                      // rest of s has no '#'
                      while (si < s.Length)
                          if (s[si++] == '#')
                              return 0;
                      return 1;
                  }

                  if (si >= s.Length)
                        return 0;

                    // must match next num, else backtrack

                    // skip any prefix
                    while (si < s.Length && s[si] == '.') si++;

                    long count = 0; // answers below this

                    // try places to start reading num in a row
                    var num = nums[ni];
                    bool hitFixed = false;
                    while (si <= s.Length-num && s[si] != '.')
                    {
                        var fits = true;
                        for (int k = 0; k < num; ++k)
                        {
                            fits &= s[si + k] != '.';
                            hitFixed |= (s[si + k] == '#');
                        }

                        if (!fits)
                        {
                            if (hitFixed) break;
                            ++si;
                            continue;
                        };

                        // read next blank, unless done
                        int blank = 0;
                        if (si+num < s.Length)
                        {
                            if (s[(int)(si + num)] == '#')
                            {
                                ++si;
                                continue;
                            }
                            blank = 1;
                        }
                         count += Recurse(s,(int)(si+num+blank),nums,ni+1);
                         si++; // next start
                    }
                  //  memo.Add(key,count);
                    return count;
                }
                */

            }

        }
    }
}