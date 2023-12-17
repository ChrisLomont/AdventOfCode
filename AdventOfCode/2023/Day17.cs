
namespace Lomont.AdventOfCode._2023
{
    internal class Day17 : AdventOfCode
    {
        //1008, 1210

        record State(vec2 pos, vec2 dir, int runlength);

        public override object Run(bool part2)
        {
            var (w, h, g) = DigitGrid();

            bool Legal(vec2 p) => 0 <= p.x && 0 <= p.y && p.x < w && p.y < h;
            var maxRun = part2 ? 10 : 3;
            var minRun = part2 ? 4 : 1;
            var dirs = new List<vec2> { new(0, 1), new(1, 0), new(0, -1), new(-1, 0), };
            var end = new vec2(w - 1, h - 1);

            var seen = new HashSet<State>();
            var frontier = new PriorityQueue<(int cost, State state), int>();
            frontier.Enqueue((0,new State(new (),new (),0)),0);

            while (frontier.Count > 0)
            {
                var element = frontier.Dequeue();
                var (cost, state) = element;
                var (pos, dir, n) = state;

                var c2 = !part2 || n >= minRun;
                if (pos == end && c2)
                    return cost;

                if (seen.Contains(state))
                    continue;
                seen.Add(state);

                if (n < maxRun && dir != vec2.Zero)
                    AddIf(new(pos + dir, dir, n + 1));

                var minOk = n >= minRun || dir == vec2.Zero;

                if (!part2 || (part2 && minOk))
                    foreach (var dir2 in dirs)
                        if (dir2 != dir && dir2 != -dir)
                            AddIf(new(pos + dir2, dir2, 1));
                
                void AddIf(State s)
                {
                    if (Legal(s.pos))
                    {
                        var newCost = cost + g[s.pos.x, s.pos.y];
                        frontier.Enqueue((newCost, s), newCost);
                    }
                }
            }

            return 0;
        }
    }
}