
using System.ComponentModel.Design;
using System.Text;

namespace Lomont.AdventOfCode._2022
{

    // top 100,both 10:03 - 24:43
    // part 1: 8:35 - 21:46

    // --------Part 1---------   --------Part 2---------
    //     Day Time    Rank Score       Time Rank  Score
    // 23   00:43:49     931      0   00:51:50     999      0
    // 22   00:45:06     952      0   01:53:04     505      0

    //2022 Day 23 part 1: 4091 in 1429674.3 us
    //2022 Day 23 part 2: 1036 in 146039594.5 us

    internal class Day23 : AdventOfCode
    {
        static vec3 NW = new vec3(-1, -1);
        static vec3 N = new vec3(0, -1);
        static vec3 NE = new vec3(1, -1);

        static vec3 E = new vec3(1, 0);

        static vec3 SE = new vec3(1, 1);
        static vec3 S = new vec3(0, 1);
        static vec3 SW = new vec3(-1, 1);

        static vec3 W = new vec3(-1, 0);

        static Func<vec3, Dictionary<vec3, vec3>, HashSet<vec3>, bool>[] funcs = new Func<vec3, Dictionary<vec3, vec3>, HashSet<vec3>, bool>[]
        {
            (elf,moves, elves ) => Fun(N, NE, NW, elf, N, moves, elves),
            (elf,moves, elves ) => Fun(S, SE, SW, elf, S, moves, elves),
            (elf,moves, elves ) => Fun(W, NW, SW, elf, W, moves, elves),
            (elf,moves, elves) => Fun(E, NE, SE, elf, E, moves, elves),
        };

        static bool Fun(
            vec3 d1, vec3 d2, vec3 d3, vec3 elf, vec3 res,
            Dictionary<vec3, vec3> moves, HashSet<vec3> elves)
        {
            if (
                !elves.Contains(elf + d1) &&
                !elves.Contains(elf + d2) &&
                !elves.Contains(elf + d3)
            )
            {
                moves[elf] = elf + res;
                return true;
            }

            return false;
        }


        public override object Run(bool part2)
        {
            if (part2) return -1; 
            var (w,h,g) = CharGrid();

            var elves = new HashSet<vec3>();
            Apply(g, (i, j, v) =>
            {
                if (v == '#')
                elves.Add(new vec3(i, j));
                return v;
            });

            Dictionary<vec3, vec3> moves = new();
            //Draw();

            static void DoOne(vec3 elf, Dictionary<vec3, vec3> moves, HashSet<vec3> elves, int round)
            {
                for (var c = 0; c < 4; ++c)
                {
                    if (funcs[(c + round) % 4](elf,moves,elves)) break;
                }

            }



            int max = part2 ? int.MaxValue-10 : 10;
            for (var round = 0; round < max; ++round)
            {
                if ((round % 25) == 0)
                {
                    var (aminX, amaxX, aminY, amaxY) = Box(elves);
                    var aarea = (amaxX - aminX + 1) * (amaxY - aminY + 1);
                    Console.WriteLine($"Round {round + 1}, area {aarea}");
                }

                // part 1 - think of moves
                moves.Clear();
                foreach (var elf in elves)
                {
                    var cnt = 0;
                    for (var j = -1; j <= 1; ++j)
                    for (var i = -1; i <= 1; ++i)
                    {
                        if (i == 0 && j == 0) continue;
                        if (elves.Contains(elf+new vec3(i, j)))
                            cnt++;
                    }

                    moves.Add(elf, elf); // default stay still

                    if (cnt == 0) 
                        continue;

                    DoOne(elf, moves, elves, round);


                }


                // part2 = make moves
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
                var moved = false;
                foreach (var elf in elves)
                    if (!elves2.Contains(elf))
                    {
                        moved = true;
                        break;
                    }

                if (!moved)
                    return round+1;

                elves = elves2;
                //Console.WriteLine("Round "+(round+1));
                //Draw();
                //Console.WriteLine();

            }

            void Draw()
            {
                var (minX, maxX, minY, maxY) = Box(elves);
                for (var j = minY; j <= maxY; ++j)
                {
                    for (var i = minX; i <= maxX; ++i)
                    {
                        Console.Write(elves.Contains(new vec3(i,j))?"#":".");

                    }

                    Console.WriteLine();
                }

                Console.WriteLine();

            }

            static (int minX, int maxX, int minY, int maxY) Box(HashSet<vec3> h)
            {
                var minX = h.Min(v => v.x);
                var maxX = h.Max(v => v.x);
                var minY = h.Min(v => v.y);
                var maxY = h.Max(v => v.y);
                return (minX, maxX, minY, maxY);
            }

            var (minX, maxX, minY, maxY) = Box(elves);
            var area = (maxX - minX + 1) * (maxY - minY + 1);


            return area -elves.Count;
        }
    }
}