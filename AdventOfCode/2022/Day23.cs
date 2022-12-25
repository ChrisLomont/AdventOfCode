namespace Lomont.AdventOfCode._2022
{

    // top 100,both 10:03 - 24:43
    // part 1: 8:35 - 21:46

    // --------Part 1---------   --------Part 2---------
    //     Day Time    Rank Score       Time Rank  Score
    // 23   00:43:49     931      0   00:51:50     999      0
    // 22   00:45:06     952      0   01:53:04     505      0

    //2022 Day 23 part 1: 4091 in 1429674.3 us
    //2022 Day 23 part 2: 1036 in 146 039 594.5 us = 146 secs :(

    internal class Day23 : AdventOfCode
    {
        static vec3 NW = new (-1, -1);
        static vec3 N  = new (0, -1);
        static vec3 NE = new (1, -1);
        static vec3 E  = new (1, 0);
        static vec3 SE = new (1, 1);
        static vec3 S  = new (0, 1);
        static vec3 SW = new (-1, 1);
        static vec3 W  = new (-1, 0);

        static List<List<vec3>> CheckTable = new()
        {
            new(){N,NE,NW},
            new(){S,SE,SW},
            new(){W,NW,SW},
            new(){E,NE,SE},
        };


        public override object Run(bool part2)
        {
            var elves = new HashSet<vec3>();
            var lines = ReadLines();
            for (var i = 0; i < lines[0].Length; ++i)
            for (var j = 0; j < lines.Count; ++j)
            {
                if (lines[j][i] == '#')
                    elves.Add(new vec3(i, j));

                }

            // track requested moves
            Dictionary<vec3, vec3> moves = new();

            var nbrs = new List<vec3>();
            for (var j = -1; j <= 1; ++j)
            for (var i = -1; i <= 1; ++i)
                if (i != 0 || j != 0) nbrs.Add(new(i, j)); ;

            var round = 0;
            while (true)
            {
                if ((round % 25) == 0)
                {
                    //Console.WriteLine($"Round {round + 1}, area {Area(elves)}");
                }

                // part 1 of round - choose moves
                moves.Clear();
                foreach (var elf in elves)
                {
                    // default no move
                    moves.Add(elf, elf); 

                    if (nbrs.Any(d => elves.Contains(elf + d)))
                    { // check moves
                        for (var c = 0; c < 4; ++c)
                        {
                            var chk = CheckTable[(c + round) % 4];
                            if (!chk.Any(d => elves.Contains(elf + d)))
                            {
                                moves[elf] = elf + chk[0];
                                break;
                            }
                        }
                    }
                }

                // part 2 of round : do moves
                var elves2 = new HashSet<vec3>();
                foreach (var elf in elves)
                {
                    var dst = moves[elf];
                    if (moves.Values.Count(c => c == dst) > 1)
                        elves2.Add(elf); // no move
                    else
                        elves2.Add(dst);
                }

                // see if any moved
                if (part2 && elves.SetEquals(elves2))
                    return round + 1;
                // check area for part 1
                if (!part2 && round == 9)
                    return Area(elves2) - elves2.Count;

                // use update elves for next round
                elves = elves2;
                ++round;
            }

            static int Area(HashSet<vec3> h)
            {
                var min = h.Aggregate(vec3.MaxValue, vec3.Min);
                var max = h.Aggregate(vec3.MinValue, vec3.Max);
                var (dx, dy) = max - min + vec3.One;
                return dx*dy;
            }

        }
    }
}