namespace Lomont.AdventOfCode._2023
{
    internal class Day21 : AdventOfCode
    {
        // 3733, 617729401414635

        // size is 131x131, primes
        // S at 66,?? - center?
        // steps = 26501365 = 5*11*481843
        // 5*11 = 55 steps means none get off original board
        // thus 55 starting positions * prime steps left, prime sized boards, some form of cycles, CRT?
        // border empty! (leverage? count to edge, then edge to edge stuff??)
        // center row, col empty!
        // growth is quadratic, so can be modeled by an order 2 delta system

        object Part2()
        {
            var (w, h, g) = CharGrid();
            var s = new vec2(-1,-1);

            var dirs = new List<vec2> { new(-1, 0), new(1, 0), new(0, -1), new(0, 1) };

            var open = new HashSet<vec2>();
            Apply(g, (i, j, v) =>
            {

                if (v == 'S')
                    s = new vec2(i, j);
                if (v != '#')
                    open.Add(new(i, j));
                return v;
            });

            var visited = new HashSet<vec2>{};
            var frontier = new HashSet<vec2>{s};

            // track perimeter size at each step, look at behavior, looks linear after a while
            // so total is quadratic after a while

            // these store a sliding window of values to define the order 2 difference eqn
            // walk till all delta2 are 0, then can add backwards
            var delta0 = new int[w];
            var delta1 = new int[w];
            var delta2 = new int[w];

            // running list of last 2 values (since odd/even toggles on grid), 
            long v1 = 0, v2 = 0;

            void Cycle(long nv)
            {
                v2 += nv;
                (v1, v2) = (v2, v1);
            }


            int P(int  a, int  n) => ((a%n)+n)% n; // positive mod
            vec2 mp(vec2 v) => new(P(v.x,w),P(v.y,h));

            GIFMaker? gif = null;

            int step = 0;
            // loop till enough steps, and no more changes in 2nd order - all info in the arrays
            while (step < 2 * w || delta2.Sum() !=0)
            {
                // new frontier, to get sizes
                // get points not yet seen each time. This is not quite the frontier, 
                // but is the "delta" of the frontier
                var possiblePts = frontier.SelectMany(p => dirs.Select(d => p + d));
                var filteredPts = possiblePts.Where(p => open.Contains(mp(p)) && visited.Add(p));
                frontier = filteredPts.ToHashSet();

                // track 2nd order difference eqn to find quadratic growth coeffs
                var newSize = frontier.Count;
                Cycle(newSize);

                // track first and higher order differences until top one zeros out
                var smod     = step % w; // board wraps
                var fDelta   = newSize - delta0[smod];
                delta2[smod] = fDelta - delta1[smod]; // 2nd order delta
                delta1[smod] = fDelta; // first order delta
                delta0[smod] = newSize;

                step++;

                AddMap(true,step);
            }
            AddMap(false,0);

            // todo - can solve 2nd order difference eqn directly? do it :)
            // but vals wiggle :| can it be closed form?
            for (int i = step, j = step % w; i < 26501365; i++, j = (j + 1) % w)
                Cycle(delta0[j] += delta1[j]);

            return v1;

            void AddMap(bool more, int step)
            {
                if (more == false)
                {
                    gif.Save("Day21_2023.gif");
                    return;
                }

                //if ((step % 5) != 0)
                //    return;

              //  Console.WriteLine("Gif frame");

                var c = 2; // size -c to c copies of grid
                var c2 = (2 * c + 1)*w;
                var g2 = new char[c2, c2];
                var pix = 3; // pixel size
                if (gif == null)
                    gif = new GIFMaker(c2 * pix, c2 * pix);

                byte[,] img = new byte[c2 * pix, c2 * pix];

                Apply(g2, (i, j, v) =>
                {
                    var c = g[i%w,j%h];
                    if (c == 'S')
                        c = '.';
                    if (visited.Contains(new vec2(i - (c2+1)/2+w/2+1, j - (c2+1)/2+w/2+1)))
                        c = 'O';
                    for (int di = 0; di < pix; ++di)
                    for (int dj = 0; dj < pix; ++dj)
                    {
                        var x = i * pix + di;
                        var y = j * pix + dj;
                        img[x, y] = (byte)(c switch
                        {
                            '.' => 0,
                            'O' => 121,
                            '#' => 39,
                            _ => 44
                        });
                        //Console.WriteLine($"{i} {j} {img[x, y]}");
                    }

                    return c;
                });

                var delay = 20;
                gif.AddFrame(img,delay);
            }

        }
    
        object Part1()
        {
            long answer = 0;

            // var (w,h,g) = DigitGrid();
            
            var (w,h,g) = CharGrid();
            int x = -1, y = -1;
            
            Apply(g, (i, j, s) =>
            {
                if (g[i, j] == 'S')
                {
                    x = i;
                    y = j;
                }
                return s;
            });

            HashSet<(int i, int j)> seen = new();


            HashSet<(int i, int j, int s)> eval = new HashSet<(int i, int j, int s)>();

            void Recurse(int i, int j, int s)
            {
                var k = (i,j,s);
                if (eval.Contains(k))
                    return;
                eval.Add(k);

                if (s == 0)
                {
                    seen.Add((i, j));
                    return;
                }
                if (i > 0 && g[i-1, j] != '#')// && !seen.Contains((i - 1, j)))
                    Recurse(i - 1, j, s - 1);
                if (i < w-1 && g[i+1, j] != '#')// && !seen.Contains((i +1, j)))
                    Recurse(i + 1, j, s - 1);
                if (j > 0 && g[i , j-1] != '#')// && !seen.Contains((i , j-1)))
                    Recurse(i , j-1, s - 1);
                if (j < h - 1 && g[i, j+1] != '#')// && !seen.Contains((i , j+1)))
                    Recurse(i , j+1, s - 1);
            }
            Recurse(x,y,64);

            Apply(g, (i, j, s) =>
            {
                if (g[i,j] == '.' && seen.Contains((i, j)))
                    return 'O';
                return s;
            });
           // Dump(g,noComma:true);


            return seen.Count;
        }

        public override object Run(bool part2)
        {
            return part2 ? Part2() : Part1();
        }

        /*
    object Part2A()
    {
        long answer = 0;

        // var (w,h,g) = DigitGrid();

        var (w, h, g) = CharGrid();
        int x = -1, y = -1;

        Apply(g, (i, j, s) =>
        {
            if (g[i, j] == 'S')
            {
                x = i;
                y = j;
            }
            return s;
        });

        Console.WriteLine($"Size {w} {h}");

        HashSet<(int i, int j)> seen = new();


        HashSet<(int i, int j, int s)> eval = new HashSet<(int i, int j, int s)>();

        var dirs = new List<(int dx, int dy)>
        {
            (-1,0),(1,0),(0,-1),(0,1)
        };

        int P(int a, int n) => ((a % n) + n) % n; // pos mod

        void Recurse(int i, int j, int s)
        {
            var k = (i, j, s);
            if (eval.Contains(k))
                return;
            eval.Add(k);

            if (s == 0)
            {
                seen.Add((i, j));
                return;
            }

            foreach (var dir in dirs)
            {
                var i2 = i + dir.dx;
                var j2 = j + dir.dy;
                if (g[P(i2,w), P(j2,h)] != '#')
                    Recurse(i2,j2,s-1);
            }
        }

        // steps = 26501365
        Recurse(x, y, 55);
        bool Good(int i, int j) => 0<=i && 0 <= j && i < w && j < h;

        Console.WriteLine($"off {seen.Where(p=>!Good(p.i,p.j)).Count()}");

        Apply(g, (i, j, s) =>
        {
            if (g[i, j] == '.' && seen.Contains((i, j)))
                return 'O';
            return s;
        });
        // Dump(g,noComma:true);


        return seen.Count;
    }
    */

    }
}