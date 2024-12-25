

using Microsoft.VisualBasic.CompilerServices;

namespace Lomont.AdventOfCode._2018
{
    internal class Day23 : AdventOfCode
    {
        // 933
        // 70887840

        record v3(long x, long y, long z)
        {
            public static v3 operator +(v3 a, v3 b) => new v3(a.x + b.x, a.y + b.y, a.z + b.z);
            public static v3 operator -(v3 a, v3 b) => new v3(a.x - b.x, a.y - b.y, a.z - b.z);
            public static v3 operator *(long s, v3 b) => new v3(s * b.x, s * b.y, s * b.z);
            public long ManhattanDistance => Math.Abs(x) + Math.Abs(y) + Math.Abs(z);
        }

        record Nanobot(v3 p, long r);

        long dist(v3 a, v3 b) => (a - b).ManhattanDistance;
        
        public override object Run(bool part2)
        {
            checked
            {
                List<Nanobot> bots = new();
                foreach (var line in ReadLines())
                {
                    var nums = Numbers(line);
                    bots.Add(new(new v3(nums[0], nums[1], nums[2]), nums[3]));
                }

                if (!part2)
                {
                    var largestRadiusBot = bots.MaxBy(b => b.r);
                    return bots.Count(b => dist(b.p, largestRadiusBot.p) <= largestRadiusBot.r);
                }

                var bestPos = new v3(0, 0, 0);
                (long cnt, long dst) bs = (0, 0); // count in range, -dist to 0,0,0
                for (var range = (1L<<30); range > 0; range/=2)
                {
                    for (var x = -range; x <= range; x += range)
                    for (var y = -range; y <= range; y += range)
                    for (var z = -range; z <= range; z += range)
                    {
                        var pos = bestPos + new v3(x, y, z);

                        var (count, d0) = countAndDist(pos, bots);
                        if (count > bs.cnt || (count == bs.cnt && d0 > bs.dst))
                        {
                            bs = (count, d0);
                            bestPos = pos;
                        }
                    }
                }

                return bestPos.ManhattanDistance;

                (long inRange, long neg_dist_to_zero) countAndDist(v3 pos, List<Nanobot> nanobots)
                {
                    var dists = nanobots.Select(b => dist(b.p, pos)).ToList();

                    //if not ranges:
                    //    //if ranges is not set, calculate bot-to-pos ranges, else calculate pos-with-range-to-bots distance
                    var ranges = nanobots.Select(b => b.r).ToList();

                    var in_range = 0;
                    for (int i = 0; i < dists.Count; ++i)
                        in_range += dists[i] <= ranges[i] ? 1 : 0;

                    var dist_to_0 = dist(pos, new v3(0, 0, 0));

                    //as we try to maximize this function, the dist_to_0 (where we want a small one) should be negative
                    return (in_range, -dist_to_0);
                }
            }
        }

        
    }

}