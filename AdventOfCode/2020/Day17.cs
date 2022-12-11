namespace Lomont.AdventOfCode._2020
{
    internal class Day17 : AdventOfCode
    {
        // 2020 Day 17 part 1: 319 in 524265.3 us
        // 2020 Day 17 part 2: 2324 in 23238779 us
        public override object Run(bool part2)
        {
            if (part2) return Run4D();
            return Run3D();
        }

        object Run4D()
        {

            var (w, h, gc) = CharGrid();

            int days = 6;
            var s = days + Math.Max(w, h) + 2;

            var ds = 2 * s + 1;
            var g1 = new int[ds, ds, ds, ds];
            var g2 = new int[ds, ds, ds, ds];
            Apply(gc, (i, j, v) =>
            {
                g1[i + s, j + s, s, s] = v == '#' ? 1 : 0;
                return v;
            });

            //Console.WriteLine(Nbrs(g1,0,0,0,0).Count());

            // now apply N days
            for (var day = 0; day < days; ++day)
            {
                //Draw();
                Do4(g1,g2);
                (g1, g2) = (g2, g1); // swap

            }

            long count = 0;
            Apply(g1, (i, j, k, q, v) =>
            {
                if (v != 0) count++;
                return v;
            });


            return count;
        }
        IEnumerable<int> Nbrs(int[,,,] gg, int x, int y, int z, int w)
        {
            for (var dx = -1; dx <= 1; ++dx)
            for (var dy = -1; dy <= 1; ++dy)
            for (var dz = -1; dz <= 1; ++dz)
            for (var dw = -1; dw <= 1; ++dw)
            {
                if (dx == 0 && dy == 0 && dz == 0 && dw == 0) continue;
                yield return Get(gg, x + dx, y + dy, z + dz, w + dw, 0);
            }
        }

        void Do4(int [,,,] g1, int[,,,] g2)
        {
            // active & 2-3 => active, else inactive
            // inactive & 3 are active, become active, else inactive

            Apply(
                g1, (x, y, z, w, v) =>
                {
                    var s1 = Nbrs(g1, x, y, z, w).Sum();
                    if (v == 1)
                        g2[x, y, z, w] = (s1 == 2 || s1 == 3) ? 1 : 0;
                    if (v == 0)
                        g2[x, y, z, w] = (s1 == 3) ? 1 : 0;
                    return v;
                }
            );
        }

        object Run3D(){

        var (w,h,gc) = CharGrid();

            int days = 6;
            var s = days + Math.Max(w,h)+2;
            
            var ds = 2 * s + 1;
            var g1 = new int[ds, ds, ds];
            var g2 = new int[ds, ds, ds];
            Apply(gc, (i, j, v) =>
            {
                g1[i + s, j + s, s] = v == '#'?1:0;
                return v;
            });

            // now apply N days
            for (var day = 0; day < days; ++day)
            {
                //Draw();
                // active & 2-3 => active, else inactive
                // inactive & 3 are active, become active, else inactive
                Apply(
                    g1, (i, j, k, v) =>
                    {
                        var s = Nbrs3(g1, i, j, k,0).Sum();
                        if (v == 1)
                            g2[i, j, k] = (s==2||s==3)?1:0;
                        if (v == 0)
                            g2[i, j, k] = (s==3)?1:0;
                        return v;
                    }
                );
                (g1, g2) = (g2, g1); // swap

                void Draw()
                {
                    // draw
                    for (var i = 0; i < ds; ++i)
                    {
                        for (var j = 0; j < ds; ++j)
                        {
                            Console.Write(g1[i, j, s] == 1 ? '#' : '.');
                        }
                        Console.WriteLine();
                    }
                    Console.WriteLine();
                }


            }

            long count = 0;
            Apply(g1, (i,j,k ,v) =>
            {
                if (v != 0) count++;
                return v;
            });

    

            return count;
        }
    }
}