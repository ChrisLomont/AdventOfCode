namespace Lomont.AdventOfCode._2023
{
    internal class Day05 : AdventOfCode
    {

        /*
              --------Part 1---------   --------Part 2--------
Day       Time    Rank  Score       Time   Rank  Score
  5   00:23:11    1843      0   01:43:15   3581      0
  4   00:09:08    2699      0   00:21:16   2451      0
  3   00:19:04    1331      0   00:29:57   1410      0
  2   00:06:03     329      0   00:09:23    411      0
  1   17:45:07  131162      0   18:25:46  92867      0

                First hundred users to get both stars on Day 5:

                1) Dec 05  00:08:38  5space
                2) Dec 05  00:11:07  Kroppeb(AoC++)
                    3) Dec 05  00:11:50  zacharybarbanell
                4) Dec 05  00:12:01  D.Salgado
                5) Dec 05  00:12:39  (anonymous user #1510407)
                6) Dec 05  00:13:11  bluepichu
                7) Dec 05  00:14:04  Magnus Hokland Hegdahl
                8) Dec 05  00:14:05  boboquack

                95) Dec 05  00:26:25  Ben
         96) Dec 05  00:26:27  teekaytai
         97) Dec 05  00:26:28  Robin Allen
         98) Dec 05  00:26:34  jakingy
         99) Dec 05  00:26:36  Luke-Sheltraw
        100) Dec 05  00:26:37  Victor Chen
        First hundred users to get the first star on Day 5:

          1) Dec 05  00:01:35  Jonas Haller
          2) Dec 05  00:01:53  (anonymous user #1508761)
          3) Dec 05  00:01:53  Nordine Lotfi
          4) Dec 05  00:02:21  Anango Prabhat
          5) Dec 05  00:02:31  shakespeare1564 (AoC++)

         95) Dec 05  00:08:09  rhendric
 96) Dec 05  00:08:09  Jimmy Ding
 97) Dec 05  00:08:12  dkalak
 98) Dec 05  00:08:13  Sam Harrison
 99) Dec 05  00:08:14  Sampriti Panda
100) Dec 05  00:08:15  David Sharick

        */

        // 199602917, 2254686

        public override object Run(bool part2)
        {
            Queue<(long start, long end)> seeds = new();
            Queue<(long start, long end)> remapped = new();
            foreach (var line in ReadLines())
            {
                var n = Numbers64(line);
                if (seeds.Count + remapped.Count == 0)
                    for (var j = 0; j < n.Count; j += part2 ? 2 : 1)
                        seeds.Enqueue((n[j], n[j] + (part2 ? n[j + 1] : 1)));
                else if (!n.Any()) // block end, push rest through
                    while (remapped.Any()) seeds.Enqueue(remapped.Dequeue());
                else // it's a mapping (dest,src,length)
                {
                    var shift = n[0] - n[1];
                    var num = seeds.Count; // must do each existing seed once
                    while (num-- > 0)
                    {
                        // split interval [s,e) with interval [n[1], n[1]+n[2])
                        var (s, e) = seeds.Dequeue(); // start, end
                        var os = Math.Max(s, n[1]);   // compute overlap 
                        var oe = Math.Min(e, n[1] + n[2]);
                        if (os < oe) // do we have overlap?
                        {
                            remapped.Enqueue((os + shift, oe + shift)); // mapped, shifted
                            if (os > s) seeds.Enqueue((s, os)); // send left part back to pool
                            if (e > oe) seeds.Enqueue((oe, e)); // send right part back to pool
                        }
                        else seeds.Enqueue((s,e)); // put back
                    }
                }
            }

            // final items
            while (remapped.Any()) seeds.Enqueue(remapped.Dequeue());

            return seeds.Min(s => s.start);
        }

#if false
        // split r1 by r2
        static void Split(Interval r1, Interval r2, out Interval? left, out Interval? mid, out Interval? right)
        {
            long a1 = r1.Start, b1 = r1.Start + r1.Length;
            long a2 = r2.Start, b2 = r2.Start + r2.Length;

            left = mid = right = null;

            if (b1 <= a2)
            {
                left = r1;
                return;
            }

            if (b2 <= a1)
            {
                right = r1;
                return;
            }

            // any on left?
            if (a1 < a2)
            {
                left = new Interval(a1, a2 - a1);
                a1 = a2;
            }

            // any on right?
            if (b2 < b1)
            {
                right = new Interval(b2, b1 - b2);
                b1 = b2;
            }

            // any in middle?
            if (b1 > a1)
            {
                mid = new Interval(a1, b2 - a1);
            }

        }


        struct Rec
        {
            public long dstStart; 
            public Interval start;
        }
        class Map
        {
            public string src, dst;
            public List<Rec> ranges = new();

            public List<Interval> MapIt(Interval val)
            {
                List<Interval> ans = new (){ val };

                    int count = ans.Count - 1;
                    while (count != ans.Count)
                    {
                        var ans2 = new List<Interval>();
                        count = ans.Count;
                        bool hitOne = false;
                        for (var ii1  = 0; ii1 < ans.Count && !hitOne; ++ii1)
                        {
                            var r1 = ans[ii1];
                            for (var ii2 = 0; ii2 < ranges.Count && !hitOne; ++ii2)
//                            {
//                            foreach (var r2 in ranges)
                            {
                                var r2 = ranges[ii2];
                                Interval? left, mid, right;

                                Split(r1, r2.start,
                                    out left, out mid, out right);

                                var count2 = 0;
                                if (left != null)
                                {
                                    ans2.Add(left);
                                    ++count2;
                                }

                                if (right!= null)
                                {
                                    ans2.Add(right);
                                    ++count2;
                                }

                            if (mid != null)
                            {
                                    ans2.Add(new Interval
                                    (
                                        mid.Start - r2.start.Start+r2.dstStart, 
                                        mid.Length
                                    ));
                                    ++count2;
                            }

                            Trace.Assert(count2>=1);

                            if (count2 > 1)
                            {
                                hitOne = true;
                            }

                            }

                        ans = ans2;
                        ans2 = new();
                        }

                    }

                    return ans;
            }
        }

        public override object Run(bool part2)
        {

            var lines = ReadLines();
            List<Interval> seeds = new();
            List<Map> maps = new();
            var rg = new Regex($"^(?<src>[a-z]+)-to-(?<dest>[a-z]+) map:");
            for (int i = 0; i < lines.Count; ++i)
            {
                var line = lines[i];
                if (line.StartsWith("seeds:"))
                {
                    var n = Numbers64(line);
                    if (part2)
                    {
                        for (var j = 0; j < n.Count; j += 2)
                            seeds.Add(new Interval(n[j], n[j + 1]));
                    }
                    else
                    {
                        for (var j = 0; j < n.Count; j++)
                            seeds.Add(new Interval (n[j], 1));
                    }
                }
                else if (rg.IsMatch(line))
                {
                    var m = rg.Match(line);
                    var src = m.Groups["src"].Value;
                    var dst = m.Groups["dest"].Value;
                    Parse(src, dst, ref i);
                }

            }
            // then map from seed to location via maps

            Console.WriteLine($"Maps {maps.Count}, Seeds {seeds.Count}");



List<Interval> solved = new();
foreach (var s in seeds)
{
    var m1 = SeedToLoc(s);
    Console.Write($"Seed {s.Start} -> ");
    foreach (var m in m1)
    {
        Console.Write($"{m.Start}, ");

    }
    Console.WriteLine();
    solved.AddRange(m1);

}

 ans1 = solved.Min(r => r.Start);
 return ans1;


            List<Interval> SeedToLoc(Interval seed)
            {
                var itemsTxt = "seed";
                var items = new List<Interval>(){seed};

                while (itemsTxt != "location")
                {
                    var map = maps.First(m => m.src == itemsTxt);

                    var next = new List<Interval>();
                    foreach (var r in items)
                    {
                        next.AddRange(map.MapIt(r));
                    }

                    Flatten(next);
                    items = next;

                    itemsTxt = map.dst;
                }

                return items;

                void Flatten(List<Interval> range)
                {
                 //   Console.Write($"Range count {range.Count} at {itemsTxt}");

                    bool done = false;
                    while (!done)
                    {
                        done = true;
                        for (int i = 0; i < range.Count && done; ++i)
                        for (int j = i+1; j < range.Count && done; ++j)
                        {
                            var ri = range[i];
                            var rj = range[j];
                            var m = Merge(ri,rj);
                            if (m != null)
                            {
                                range[i] = m;
                                range.RemoveAt(j);
                                done = false;
                            }

                        }
                    }

//                    Console.WriteLine($" to {range.Count}");
                }

                Interval? Merge(Interval r1, Interval r2)
                {
                    long a1 = r1.Start, b1 = r1.Start + r1.Length;
                    long a2 = r2.Start, b2 = r2.Start + r2.Length;
                    if (b1 < a2) return null;
                    if (b2 < a1) return null;
                    var ai = Math.Min(a1,a2);
                    var bi = Math.Max(b1, b2);
                   // Console.WriteLine($"Merge {a1}-{b1} + {a2}-{b2} to {ai}-{bi}");
                    return new Interval(ai, bi - ai);
                }


            }


            void Parse(string src, string dst, ref int i)
            {
                var map = new Map();
                map.src = src; map.dst = dst;
                i++;
                while (i < lines.Count && lines[i].Trim().Length>3)
                {
                    var n = Numbers64(lines[i]);
                    var rec = new Rec {dstStart = n[0], start = new Interval(n[1], n[2]) };
                    map.ranges.Add(rec);
                    ++i;
                }
                maps.Add(map);
            }

        
        }
#endif
        }
    }
