namespace Lomont.AdventOfCode._2021
{
    internal class Day09 : AdventOfCode
    {
        public override object Run(bool part2)
        {
            var (w,h,g) = DigitGrid();

            // where to find basin:
            // is +- 1 in a dir to move
            // is +100,+100 for basin
            // is (0,0) for unknown 
            var dir = new (int, int)[w, h]; 

            int scale = 10_000;
            Dictionary<int,int> attractors = new ();

            Apply(dir, (i, j,v) =>
                {
                    return (0, 0);
                }
            );

            // get all attractors and sums
            var sum = 0;
            Apply(g, (i, j, v) =>
            {
                var sc = Ok1(i, j);
                sum += sc;
                if (sc > 0)
                {
                    dir[i, j] = new(100, 100);
                    attractors.Add(i * scale + j, 0);
                }

                return v;
            });
            if (part2)
            {
                Apply(g, (i, j, v) =>
                {
                    var (di, dj) = Dest(i, j); // where i,j ends up
                    if (di != -100)
                    {
                        var k = di * scale + dj;
                        attractors[k]++;
                    }

                    return v;
                });
            }

            if (!part2)
                return sum;
            else
            {
                var top = attractors
                    .Select(k => attractors[k.Key]).ToList();
                top.Sort();
                top.Reverse();
                return top[0] * top[1] * top[2];
            }
            

            (int di,int dj) Dest(int i, int j)
            {
                var ht = g[i, j];
                if (ht == 9)
                    return (-100, -100); // in no basin
                
                // go lower if can
                if (ht > Get(i, j + 1)) return Dest(i,j+1);
                if (ht > Get(i, j - 1)) return Dest(i, j - 1);
                if (ht > Get(i + 1, j)) return Dest(i+1, j );
                if (ht > Get(i - 1, j)) return Dest(i-1, j);

                return (i, j);  // must be a basin
            }

            int Ok1(int i, int j)
            {
                var ht = g[i, j];
                if (ht >= Get(i,j+1)) return 0;
                if (ht >= Get(i, j - 1)) return 0;
                if (ht >= Get(i+1, j )) return 0;
                if (ht >= Get(i-1, j )) return 0;
                return ht + 1;
            }

            int Get(int i, int j)
            {
                if (i < 0 || j < 0 || w <= i || h <= j)
                    return int.MaxValue;
                return g[i, j];
            }
        }
    }
}