

namespace Lomont.AdventOfCode._2018
{
    internal class Day25 : AdventOfCode
    {
        /*
       2018 Day 25 part 1: 373 in 90533.6 us
2018 Day 25 part 2: 0 in 0.5 us
         */

        record v4(int x, int y, int z, int w);

        public override object Run(bool part2)
        {
            if (part2) return 0;
            var pts = new List<v4>();
            foreach (var line in ReadLines())
            {
                var nums = Numbers(line);
                pts.Add(new(nums[0], nums[1], nums[2], nums[3]));
            }

            var constellations = 0;
            var tested = new HashSet<int>();
            while (true)
            {
                var seen = new HashSet<int>();
                var frontier = new Queue<int>();
                int j = 0;
                while (!frontier.Any() && j < pts.Count)
                {
                    if (!tested.Contains(j))
                        frontier.Enqueue(j);
                    ++j;
                }

                while (frontier.Any())
                {
                    var nxt = frontier.Dequeue();
                    if (tested.Contains(nxt)) continue;

                    seen.Add(nxt);
                    tested.Add(nxt);

                    for (var i = 0; i < pts.Count(); i++)
                    {
                        if (!seen.Contains(i) && !tested.Contains(i))
                        {
                            if (Manhattan(pts[i], pts[nxt]) <= 3)
                                frontier.Enqueue(i);
                        }
                    }
                }

                if (!seen.Any())
                    break;

                constellations++;
            }

            return constellations;

            int Manhattan(v4 a, v4 b) =>
                Math.Abs(a.x - b.x) + Math.Abs(a.y - b.y) + Math.Abs(a.z - b.z) + Math.Abs(a.w - b.w);
        }

    }
}