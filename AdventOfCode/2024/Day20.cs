namespace Lomont.AdventOfCode._2024
{
    internal class Day20 : AdventOfCode
    {
        /*
         *
2024 Day 20 part 1: 1426 in 19536.2 us
2024 Day 20 part 2: 1000697 in 64737.7 us
        */

        //public override string TestFileSuffix() => "-test1";
        public override string TestFileSuffix() => "";

        IEnumerable<(int i, int j, char c)> Walk(char[,] g) {
            for (int j = 0; j < g.GetLength(1); j++)
            for (int i = 0; i < g.GetLength(0); i++)
                yield return (i, j, g[i, j]); }

        public override object Run(bool part2) {
            (int dx, int dy)[] dirs = { (1, 0), (-1, 0), (0, 1), (0, -1) };
            var (w, h, g) = CharGrid();
            (int x, int y) s = (0, 0);
            (int x, int y) e = (0, 0);
            foreach (var (i, j, c) in Walk(g)) {
                if (c == 'S') s = (i, j);
                if (c == 'E') e = (i, j); }
            g[s.x, s.y] = '.';
            g[e.x, e.y] = '.';
            var (se, es) = (DistMap(s), DistMap(e));
            var cost = se[e.x, e.y];
            int[] counts = new int[cost + 1];
            var (maxSteps, minSavings) = part2 ? (20, 100) : (2, 100);
            //(maxSteps, minSavings) = (2, 2); // test
            foreach (var (i,j,c) in Walk(g)) {
                if (c != '.') continue; // todo - can only check things on optimal path, faster?
                (int x, int y) a = (i, j);
                var costA = se[i, j];
                // walk manhattan dist
                for (var di = -maxSteps; di <= maxSteps; ++di)
                for (var dj = Math.Abs(di)-maxSteps; dj <= maxSteps-Math.Abs(di); ++dj) {
                    (int x, int y) b = (a.x + di, a.y + dj);
                    if (InBounds(b) && g[b.x, b.y] == '.') {
                        var costB = es[b.x, b.y];
                        var cheatCost = costA + costB + Math.Abs(di) + Math.Abs(dj);
                        var savings = cost - cheatCost;
                        if (savings >= minSavings)
                            counts[savings]++; } } }
            //for (int i = 0; i < counts.Length; ++i) 
            //    if (counts[i] > 0) Console.WriteLine($"{counts[i]} cheats save {i}");
            return counts.Sum();

            int[,] DistMap((int x, int y) a) {
                int[,] dm = new int[w, h];
                Queue<((int x, int y), int d)> front = new() { };
                front.Enqueue((a, 0));
                HashSet<(int, int)> seen = new();
                while (front.Any()) {
                    var (q, d) = front.Dequeue();
                    if (seen.Contains(q)) continue;
                    seen.Add(q);
                    dm[q.x, q.y] = d;
                    foreach (var dir in dirs) {
                        (int x, int y) n = (q.x + dir.dx, q.y + dir.dy);
                        if (InBounds(n) && g[n.x, n.y] == '.') front.Enqueue((n, d + 1)); }}
                return dm; }

            bool InBounds((int x, int y) q) => 0 <= q.x && 0 <= q.y && q.x < w && q.y < h;
        }
    }
}
