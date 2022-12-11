namespace Lomont.AdventOfCode._2020
{
    internal class Day11 : AdventOfCode
    {
        public override object Run(bool part2)
        {
            var (w, h, g) = CharGrid();

            int step = 0;
            while (true)
            {
                //Dump(g, noComma: true);
                var g2 = part2 ? Map2(g) : Map1(g);
                (g, g2) = (g2, g);
                if (Same(g,g2)) 
                    break;
                step++;
            }

            return Count(g, v => v == '#');

            bool Same(char[,] g1, char[,] g2)
            {
                bool same = true;
                Apply(g1, (i, j, v) =>
                {
                    same &= g2[i, j] == v;
                    return v;
                });
                return same;
            }

            // todo - abstract, reuse
            // get hit, length, onsite or offsite, i,j hit, di,dj?
            static List<char> Rays(
                char [,] grid,
                Func<char,bool> stop,
                int i, int j, 
                char offedge)
            {
                var ans = new List<char>();
                var (w, h) = Size(grid);
                for (var di = -1; di <= 1; ++di)
                for (var dj = -1; dj <= 1; ++dj)
                {
                    if (di == 0 && dj == 0) 
                        continue;
                    
                    ans.Add(Ray(i, j, di, dj));
                }

                return ans;

                char Ray(int i, int j, int di, int dj)
                {
                    i += di;
                    j += dj;
                    while (true)
                    {
                        if (i < 0 || j < 0 || w <= i || h <= j)
                            return offedge;
                        if (stop(grid[i, j]))
                            return grid[i, j];
                        i += di;
                        j += dj;
                    }


                }
            }

            static char[,] Map2(char[,] g)
            {
                var (w, h) = Size(g);
                var g2 = new char[w, h];
                Func<char, bool> isSeat = d => d != '.';
                Apply(
                    g, (i, j, v) =>
                    {
                        g2[i, j] = v; // default
                        if (v == 'L')
                        {
                            var occ = Rays(g, isSeat, i, j, '.').Count(c => c == '#');
                            if (occ == 0) 
                                g2[i, j] = '#';
                        }

                        if (v == '#')
                        {

                            var rays = Rays(g, isSeat, i, j, '.');
                            var occ = Rays(g, isSeat, i, j, '.').Count(c => c == '#');
                            if (occ >= 5) 
                                g2[i, j] = 'L';
                        }

                        return v;
                    }
                );
                return g2;
            }


            static char[,] Map1(char[,] g)
            {
                var (w, h) = Size(g);
                var g2 = new char[w, h];
                Apply(
                    g, (i, j, v) =>
                    {
                        g2[i, j] = v; // default
                        if (v == 'L')
                        {
                            var occ = Nbrs2(g, i, j, '.').Count(c => c == '#');
                            if (occ == 0) 
                                g2[i,j] = '#';
                        }

                        if (v == '#')
                        {
                            var occ = Nbrs2(g, i, j, '.').Count(c => c == '#');
                            if (occ >= 4) 
                                g2[i, j] = 'L';
                        }

                        return v;
                    }
                );
                return g2;
            }
        }
    }
}