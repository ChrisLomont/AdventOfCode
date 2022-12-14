namespace Lomont.AdventOfCode._2020
{
    internal class Day24 : AdventOfCode
    {
        public override object Run(bool part2)
        {
            long blackCount;

            // flip hex tiles
            // nbrs: E, W, SE, SW, NE, NW
            // coords: row col with even rows to right
            HashSet< (int,int)> black = new();

            int even(int v) => (v & 1) == 0 ? 1 : 0;
            int odd(int v) => (v & 1) == 0 ? 0 : 1;

            (int, int) E((int x, int y) pos) => (pos.x + 1, pos.y);
            (int, int) W((int x, int y) pos) => (pos.x - 1, pos.y);
            (int, int) NE((int x, int y) pos) => (pos.x + odd(pos.y), pos.y - 1);
            (int, int) SE((int x, int y) pos) => (pos.x + odd(pos.y), pos.y + 1);
            (int, int) NW((int x, int y) pos) => (pos.x - even(pos.y), pos.y - 1);
            (int, int) SW((int x, int y) pos) => (pos.x - even(pos.y), pos.y + 1);

            foreach (var line in ReadLines())
            {
                var pos = (x:0,y:0);
                var i = 0;
                while (i < line.Length)
                {
                    switch (line[i++])
                    {
                        case 'e':
                            pos = E(pos);
                            break;
                        case 'w':
                            pos = W(pos);
                            break;
                        case 's':
                            if (line[i++] == 'e')
                                pos = SE(pos);
                            else
                                pos = SW(pos);
                            break;
                        case 'n':
                            if (line[i++] == 'e')
                                pos = NE(pos);
                            else
                                pos = NW(pos);
                            break;
                        default: throw new Exception();
                    }
                }

                if (black.Contains(pos)) black.Remove(pos);
                else black.Add(pos);
            }

            if (!part2)
            return black.Count;
            
            for (var day = 1; day <= 100; ++day)
            {
                // find things needing changed
                var toWhite = new List<(int,int)>();
                var toBlack = new HashSet<(int, int)>();
                foreach (var b in black)
                {
                    // black with 0 or >2 black dies
                    var c = CountBlackNbrs(b);
                    if (c == 0 || c > 2)
                        toWhite.Add(b);

                    // white with exactly 2 black flips
                    foreach (var w in Nbrs(b))
                    {
                        if (!black.Contains(w))
                        { // found a white cell with at least one black nbr
                            if (CountBlackNbrs(w) == 2)
                                toBlack.Add(w);
                        }
                    }

                }
                // do changes
                foreach (var b in toBlack)
                    black.Add(b);
                foreach (var w in toWhite)
                    black.Remove(w);
                //Console.WriteLine($"Day {day}: {black.Count}");
            }

            int CountBlackNbrs((int,int)p)
            {
                var c = 0;
                foreach (var n in Nbrs(p))
                    if (black.Contains(n))
                        ++c;
                return c;
            }

            List<(int, int)> Nbrs((int, int) p)
            {
                return new List<(int, int)> {E(p),W(p),NE(p),NW(p),SE(p),SW(p) };
            }



            return black.Count;

        }
    }
}