using System.Runtime.ConstrainedExecution;
using System.Runtime.Serialization;

namespace Lomont.AdventOfCode._2023
{
    internal class Day08 : AdventOfCode
    {


        /*
        First hundred users to get both stars on Day 8:

          1) Dec 08  00:04:24  ruuddotorg
          2) Dec 08  00:04:25  boboquack
          3) Dec 08  00:04:33  Robert Xiao
          4) Dec 08  00:05:01  Oskar Haarklou Veileborg (AoC++)
          5) Dec 08  00:05:21  leijurv
          6) Dec 08  00:05:28  Joseph Durie
          7) Dec 08  00:05:41  mserrano

 96) Dec 08  00:10:12  Matt Gruskin (AoC++)
 97) Dec 08  00:10:13  ndimov
 98) Dec 08  00:10:15  rhendric
 99) Dec 08  00:10:16  terrance-li
100) Dec 08  00:10:16  Ari Wachtel
First hundred users to get the first star on Day 8:

  1) Dec 08  00:00:38  Hessel
  2) Dec 08  00:01:01  Nordine Lotfi
  3) Dec 08  00:01:53  shakespeare1564 (AoC++)
  4) Dec 08  00:01:57  (anonymous user #1508761)
  5) Dec 08  00:02:01  Davis Yoshida (AoC++)
  6) Dec 08  00:02:15  Noble Mushtak
  7) Dec 08  00:02:16  Joseph Malagon

 94) Dec 08  00:03:27  glguy (AoC++)
 95) Dec 08  00:03:28  WutPu
 96) Dec 08  00:03:28  TimHuisman1703 (AoC++)
 97) Dec 08  00:03:28  PoustouFlan
 98) Dec 08  00:03:29  Aaron (AoC++)
 99) Dec 08  00:03:29  drysle
100) Dec 08  00:03:30  matthewyu01

      --------Part 1---------   --------Part 2--------
Day       Time    Rank  Score       Time   Rank  Score
  8   00:06:36     922      0   00:22:04    905      0
  7   00:30:34    2592      0   00:38:43   1609      0
  6   00:05:05     515      0   00:14:05   2506      0
  5   00:23:11    1843      0   01:43:15   3581      0
  4   00:09:08    2699      0   00:21:16   2451      0
  3   00:19:04    1331      0   00:29:57   1410      0
  2   00:06:03     329      0   00:09:23    411      0
  1   17:45:07  131162      0   18:25:46  92867      0

                2023 Day 8 part 1: 19241 in 3010 us
                2023 Day 8 part 2: 9606140307013 in 10010.2 us

        */


        public override object Run(bool part2)
        {
            var lines = ReadLines();
            var moves = lines[0];
            var map = new Dictionary<string, (string left, string right)>();
            for (var i = 2; i < lines.Count; ++i)
            {
                var w = lines[i].Split();
                map.Add(
                    w[0],
                    (
                        w[2].Substring(1, 3),
                        w[3].Substring(0, 3)
                        ));
            }

            //Console.WriteLine($"maps {map.Count} moves {moves.Length}");

            if (!part2)
                return Runner("AAA", c => c == "ZZZ");

            Func<string, bool> endsA = s => s.EndsWith("A");
            Func<string, bool> endsZ = s => s.EndsWith("Z");

            var vals = map.Keys
                .Where(endsA)
                .Select(s => Runner(s, endsZ))
                .ToList();

            //foreach (var v in vals)
            //{
            //    Console.WriteLine($"{v} -> {v%moves.Length} {v/moves.Length}");
            //}

            //foreach (var n in map.Keys)
            //{
            //    if (endsA(n))
            //        Looper(n, endsZ);
            //}


            long lcm = 1;
            foreach (var v in vals)
                lcm = LCM(v, lcm);
            return lcm;

            long GCD(long a, long b)
            {
                while (b != 0)
                    (a, b) = (b, a % b);
                return a;
            }

            long LCM(long a, long b) => (a / GCD(a, b)) * b;

            long Runner(string cur, Func<string,bool> done)
            {
                long count = 0;
                while (!done(cur))
                    cur = moves[(int)(count++ % moves.Length)] == 'L' ? map[cur].left : map[cur].right;
                return count;
            }

            void Looper(string cur, Func<string, bool> isEnd)
            {
                Console.WriteLine($"Mapping: {cur}");

                Dictionary<(string node, int move),List<long>> seen = new ();
                long count = 0;
                int movePos = 0;
                bool done = false;
                while (!done)
                {   // track endpoints
                    if (isEnd(cur))
                    {
                        var key = (cur, movePos);

                        if (seen.ContainsKey(key))
                        { // we have state loop
                            seen[key].Add(count);
                            done = true;
                        }
                        else
                        {
                            seen.Add(key, new List<long> {count});
                        }
                    }


                    cur = moves[movePos] == 'L' ? map[cur].left : map[cur].right;
                    ++count;
                    movePos = (int)(count % moves.Length);
                }

                foreach (var k in seen.Keys)
                {
                    var cts = seen[k];
                    Console.Write($"  {k.node} {k.move} => ");
                    foreach (var v in cts)
                        Console.Write($"{v}({v-cts[0]}), ");
                    Console.WriteLine();
                }
            }

        }
    }
}