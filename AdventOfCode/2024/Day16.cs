
namespace Lomont.AdventOfCode._2024
{
    internal class Day16 : AdventOfCode
    {
        /*
2024 Day 16 part 1: 98484 in 75380.1 us
2024 Day 16 part 2: 531 in 92223.4 us

         */

        // N, E, S, W
        vec2[] dirs = { new(0, -1), new(1, 0), new(0, 1), new(-1, 0) };

        // min distance to each position and orientation
        Dictionary<(int i, int j, int d), int> Distances(char[,] g,
            PriorityQueue<(int score, int i, int j, int d), int> frontier,
            bool forward)
        {
            HashSet<(int i, int j, int d)> seen = new();
            var distances = new Dictionary<(int i, int j, int d), int>();

            while (frontier.Count > 0)
            {
                var (sc, i, j, d) = frontier.Dequeue();

                var key = (i, j, d);
                if (!distances.ContainsKey(key))
                    distances.Add(key, sc);

                if (seen.Contains(key))
                    continue;
                seen.Add(key);

                // forward or backward
                var (dx, dy) = forward ? dirs[d] : dirs[(d + 2) % 4];

                var (i2, j2) = (i + dx, j + dy);
                if (g[i2, j2] != '#')
                    frontier.Enqueue((sc + 1, i2, j2, d), sc + 1);

                frontier.Enqueue((sc + 1000, i, j, (d + 1) % 4), sc + 1000);
                frontier.Enqueue((sc + 1000, i, j, (d + 3) % 4), sc + 1000);
            }

            return distances;
        }

        public override object Run(bool part2)
        {
            var (w, h, g) = CharGrid();

            vec2 start = new(), end = new();
            Apply(g, (i, j, c) =>
            {
                if (c == 'S')
                    start = new vec2(i, j);
                if (c == 'E')
                    end = new vec2(i, j);
                return c;
            });


            var frontier = new PriorityQueue<(int score, int i, int j, int d), int>();
            frontier.Clear();
            frontier.Enqueue((0,start.x,start.y,1),0); // start East
            var dist1 = Distances(g, frontier, true);

            var best = dist1.Where(p => p.Key.i == end.x && p.Key.j == end.y).Min(p => p.Value);

            if (!part2)
                return best;

            // do it backwards
            frontier.Clear();
            for (int d = 0; d < 4; ++d)
                frontier.Enqueue((0, end.x, end.y, d), 0); // 4 dirs
            var dist2 = Distances(g, frontier, false);

            var ok = new HashSet<(int i, int j)>();
            for (var i = 0; i < w; ++i)
            for (var j = 0; j < h; ++j)
            for (var d = 0; d < 4; ++d)
            {
                // optimal path if cost from start to here to end is best
                var key = (i,j,d);
                if (
                    dist1.ContainsKey(key) &&
                    dist2.ContainsKey(key) &&
                    dist1[key] + dist2[key] == best
                )
                {
                    ok.Add((i, j));
                    g[i, j] = 'O';
                }
            }

            //Dump(g,noComma:true);

            return ok.Count;

   

        }
        //public override string TestFileSuffix() => "-test1";
        public override string TestFileSuffix() => "";

   }
}