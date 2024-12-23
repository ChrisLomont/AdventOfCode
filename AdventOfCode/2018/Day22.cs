

namespace Lomont.AdventOfCode._2018
{
    internal class Day22 : AdventOfCode
    {
        /*
            2018 Day 22 part 1: 8735 in 6388.3 us
            2018 Day 22 part 2: 984 in 2482980.9 us         
         */

        public override object Run(bool part2)
        {
            var mouth = new vec2(0, 0);
            var target = new vec2(10, 10);
            int depth = 510;

            // my puzzle
            depth = 11739;
            target = new vec2(11, 718);

            Dictionary<vec2, int> gm = new();

            // mod 3: 0 = rocky, 1 = wet, 2 = narrow
            int Erosion(vec2 p)
            {
                if (gm.ContainsKey(p)) return gm[p];

                int geo;
                if (p == mouth) geo = 0;
                else if (p == target) geo = 0;
                else if (p.y == 0) geo = p.x * 16807;
                else if (p.x == 0) geo = p.y * 48271;
                else
                {
                    var (x, y) = p;
                    geo = Erosion(new vec2(x - 1, y)) * Erosion(new vec2(x, y - 1));
                }

                var ero = (geo + depth) % 20183;
                gm[p] = ero;

                return ero;
            }

            if (!part2)
            {
                var risk = 0;
                for (int i = 0; i <= target.x; ++i)
                for (int j = 0; j <= target.y; ++j)
                    risk += Erosion(new vec2(i, j)) % 3;
                return risk;
            }


            // tool : 0 = none, 1 = torch, 2 = climbing gear
            var dirs = new[] { new vec2(1, 0), new vec2(-1, 0), new vec2(0, 1), new vec2(0, -1) };

            // minutes, x, y, cannot
            var front = new PriorityQueue<(vec2 pos, int tool), int>();

            front.Enqueue((mouth, 1), 0); // starts with torch time 0

            // x,y,cannot -> minutes
            Dictionary<(vec2 pos, int cannot), int> best = new();

            while (front.TryDequeue(out var key, out var minutes))
            {
                var (pos, cannot) = key;
                if (best.ContainsKey(key) && best[key] <= minutes)
                    continue;
                if (!best.ContainsKey(key))
                    best.Add(key, minutes);
                else
                    best[key] = minutes;


                if (pos == target && cannot == 1) return minutes;

                for (var i = 0; i < 3; ++i)
                    if (i != cannot && i != Erosion(pos) % 3)
                        front.Enqueue((pos, i), minutes + 7);

                // try all options
                foreach (var d in dirs)
                {
                    var n = pos + d;
                    if (n.x < 0 || n.y < 0) continue;
                    if (Erosion(n) % 3 == cannot) continue;
                    front.Enqueue((n, cannot), minutes + 1);
                }
            }

            return "ERROR";
        }
    }
}