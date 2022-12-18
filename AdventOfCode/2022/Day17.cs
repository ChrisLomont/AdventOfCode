namespace Lomont.AdventOfCode._2022
{
    internal class Day17 : AdventOfCode
    {
        // top 1-100 15:26 to 40:40 for both, 10:01 to 24:24 single

        // me
        // 17   00:45:52     799      0   14:46:42    6946      0

    //2022 Day 17 part 1: 3137 in 204737.7 us
    //2022 Day 17 part 2: 1564705882327 in 218499.1 us

    class CycleDetect
    {

            // cycle detect:
            List<long> heights = new(); // height at each rock drop
            // map state to the count of rock drop when seen
            Dictionary<string, long> rockCounts = new(); 

            // state hasher
            static string Hash(int inputIndex, int rockIndex, int height, int numLines, HashSet<vec3> obstacle)
            {
                var sb = new StringBuilder();
                for (var j = height - numLines; j <= height; ++j)
                    for (var i = 0; i < 7; ++i)
                    {
                        if (obstacle.Contains(new vec3(i, j)))
                        {
                            // top - j from top-(top-depth)=depth to top - (top) = 0
                            var jb = height - j;
                            Trace.Assert(0 <= jb && jb < 256);
                            sb.Append($"{i},{jb},");
                        }
                    }
                sb.Append($"{inputIndex},");
                sb.Append($"{rockIndex},");
                return sb.ToString();
            }

            public (bool done, long ans) Detect(
                long height, 
                int inputIndex, 
                long curRockCount, 
                long maxRockCount,
                HashSet<vec3> obstacles)
            {
                heights.Add(height); // track list of heights at each rock drop start

                // hash state (hope this handles collisions!)
                // 50 chosen somewhat arbitrarily - hope the inputs don't somehow form some
                // penrose tile like system that never repeats :)
                var key = Hash(inputIndex, (int)(curRockCount % 5), (int)(height + 1), 50, obstacles);

                if (rockCounts.ContainsKey(key))
                { // seen before, compute end expected endpoint

                    var startRockCount = rockCounts[key]; // count where key last seen

                    // maxRockCount - startCount is rocks till end
                    // curRockCount - startRockCount is period length

                    var periodsLeft = (maxRockCount - startRockCount) / (curRockCount - startRockCount);
                    var remainderLeft = (maxRockCount - startRockCount) % (curRockCount - startRockCount);

                    var heightInRemainderPlusPreCycles = heights[(int)(startRockCount + remainderLeft)];
                    var periodHeight = heights[(int)curRockCount] - heights[(int)startRockCount];

                    var finalHeight = periodsLeft * periodHeight + heightInRemainderPlusPreCycles;

                    return (true, finalHeight);
                }
                rockCounts.Add(key, curRockCount);
                return (false, 0);
            }
        }

        public override object Run(bool part2)
        {
            var input = ReadLines()[0];

            // 0,0 is lower left of rock image
            List<List<vec3>> rocks = new()
            {
                new List<vec3>
                {
                    new (0, 0),
                    new (1, 0),
                    new (2, 0),
                    new (3, 0)
                },
                new List<vec3>
                {
                    new (1, 2),
                    new (2, 1),
                    new (1, 1),
                    new (0, 1),
                    new (1, 0)
                },
                new List<vec3>
                {
                    new (2, 2),
                    new (2, 1),
                    new (2, 0),
                    new (1, 0),
                    new (0, 0)
                },
                new List<vec3>
                {
                    new (0, 3),
                    new (0, 2),
                    new (0, 1),
                    new (0, 0),
                },
                new List<vec3>
                {
                    new (1, 1),
                    new (0, 1),
                    new (1, 0),
                    new (0, 0),
                }
            };

            var maxRockCount = part2 ? 1000000000000L : 2022;

            // obstacles: 
            HashSet<vec3> obstacles = new();
            obstacles.UnionWith(Enumerable.Range(0, 7).Select(x => new vec3(x, 0)));

            var droppedRockCount = 0L;
            int inputIndex = 0;

            var cyc = new CycleDetect(); // abstract out cycle detection

            while (droppedRockCount < maxRockCount)
            {
                var height = obstacles.Max(v => v.y); // current height of tallest item

                // 3068 & 1514285714288
                // 3137 & 1564705882327 me
                var (found, answer) = cyc.Detect(height,inputIndex, droppedRockCount, maxRockCount, obstacles);
                if (found) return answer;

                var rock = rocks[(int)((droppedRockCount++) % 5)];
                
                var pos = new vec3(2, (int)(height + 4)); // delta to shape position

                while (true)
                {
                    if (input[inputIndex] == '>')
                        TryMove(rock, ref pos, new vec3(1, 0));
                    else
                        TryMove(rock, ref pos, new vec3(-1, 0));
                    inputIndex = (inputIndex + 1) % input.Length;

                    if (!TryMove(rock, ref pos, new vec3(0, -1)))
                    {
                        foreach (var p in rock)
                            obstacles.Add(p + pos);
                        break;
                    }
                }
            }

            return obstacles.Max(v=>v.y);

            bool TryMove(List<vec3> shape, ref vec3 pos, vec3 dir)
            {
                foreach (var p in shape)
                {
                    var t = p + pos + dir;
                    if (obstacles.Contains(t))
                        return false;
                    var (x1, _, _) = t;

                    if (x1 < 0 || 6 < x1)
                        return false;
                }
                pos += dir;
                return true;

            }

            // old crap....
#if false
// old obsolete matching tries

            (bool ok, long ans) Learn(long ht, int inLength, int inIndex)
            {
                checked
                {
                    // period: - repeats at best this often:
                    var period = inLength * rocks.Count;

                    history.Add(ht);
                    var cutoff = period * 2; // collect to here before checking
                    var len = history.Count;

                    if ((len%period) != 0)
                        return (false,0); // don't spend compute time too often

                    // these with +2 at end
                    //  7 1519285714286
                    //  8 1514285714290
                    //  9 1514285714288
                    // 10 1514285714293
                    // 11 1514285714291 
                    // 12 1514285714292
                    // 13 1514285714289
                    // 14 1514285714295
                    // 15 1514285714290
                    // 16 1514285714288

                    // 25 1514285714291
                    // 50 1514285714290

                    if ((len % period) == 0)
                    {
                        Console.WriteLine($"per {len/period} of {cutoff/period}");
                    }

                    // once last several IntLength has periodic
                    if (len > cutoff)
                    {
                        // detect period - why is it here?
                        var pMult = 1L;
                        bool done = false;
                        var p = new List<long>();
                        int i = 0;
                       while (true)
                       {
                           var low = len - 1 - (i + 1) * period;
                           if (low < 0) return (false,0);
                           var d1 = history[low];
                           var d2 = history[len - 1 - i * period];
                           p.Add(d2-d1);
                           (pMult, done) = DetectPeriod(p);
                           Console.WriteLine($"per {d2 - d1}");
                           if (done) break;
                           ++i;
                       }

                       Console.WriteLine($"P mult {pMult}");

                        period = (int)(pMult*period);

                        var numPeriodsLeft = (maxRockCount - len) / period;
                        var numInLastPortion = (maxRockCount - len) % period;

                        var htPerPeriod = history[len - 1] - history[len - 1 - period];
                        
                        var excessHtLeft = history[(int)(len - 1 - period + numInLastPortion)] - history[len - 1 - period];

                        var ans =
                            history[len - 1] // current ht
                            + htPerPeriod * numPeriodsLeft
                            +excessHtLeft;
                            ;
                            //ans += 2; // correction? why?


                        // 1514285714288
                        // 1514285714288
                        // 1000000030490
                        return (true, ans);


                        Console.WriteLine("YEAH");
                    }

                    return (false, 0);
                }

                (long per, bool found) DetectPeriod(List<long> p)
                {
                    if (p.Count < 10) return (0, false);

                    var len = p.Count;
                    var j = len / 4;
                    while (j < len/2)
                    {
                        if (p[j] != p[0])
                        {
                            ++j;
                            continue;
                        }
                        // see if periodic
                        if (IsPeriod(j))
                            return (j, true);
                        ++j;
                    }

                    return (0, false);

                    bool IsPeriod(int k)
                    {
                        var t = 0;
                        while (t + k < len)
                        {
                            if (p[t] != p[t+k])
                                return false;
                            ++t;
                        }
                        return true;
                    }

                }

            }

            void Draw()
            {
                var h = obstacle.Max(v => v.y);
                h = Math.Max(h,4);
                var ch = new char[7, h + 1];
                Apply(ch,(i,j,k)=>'.');
                foreach (var c in obstacle)
                {
                    ch[c.x, h-c.y] = '#';
                }
                Dump(ch,noComma:true);
            }
#endif
        }



    }
}