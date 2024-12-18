

using System.Dynamic;

namespace Lomont.AdventOfCode._2018
{
    internal class Day13 : AdventOfCode
    {
        object Run2()
        {
            long answer = 0;

            foreach (var line in ReadLines())
            {
                var nums = Numbers64(line);
            }

            return answer;
        }

        object Run1()
        {
            long answer = 0;
            List<(int x, int y, int dir, int turn)> carts = new();

            // dir: NESW

            var (w, h, g) = CharGrid();
            Apply(g, (i, j, c) =>
            {
                var dir = "^>v<".IndexOf(c);
                if (dir >= 0)
                {
                    carts.Add((i, j, dir, 0));
                    c = "|-|-"[dir];
                }

                return c;
            });

            Console.WriteLine($"{carts.Count} carts");

            Dump(g, noComma: true);

            while (true)
            {
                var g2 = new char[w,h];
                for (int j = 0; j < h; j++)
                for (int i = 0; i < w; i++)
                {
                    var ind = carts.FindIndex(
                        c=>c.x==i&&c.y==j
                    );
                    if (ind >= 0)
                    {

//                        var (x, y, dir, turn) = carts[ind];
//                        todo;
//                        carts[k]
                    }
                }

                Dump(g, noComma: true);
                break;
            }


            return answer;
        }

        public override object Run(bool part2)
        {
            return part2 ? Run2() : Run1();
        }
    }
}