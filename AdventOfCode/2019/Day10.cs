namespace Lomont.AdventOfCode._2019
{
    internal class Day10 : AdventOfCode
    {
    //2019 Day 10 part 1: 344 in 15874.7 us
    //2019 Day 10 part 2: 2732 in 31803 us
        int gcd(int a, int b)
        {
            while (b != 0)
                (a, b) = (b, a % b);
            return a;
        }

        public override object Run(bool part2)
        {
            var (w, h, g) = CharGrid();

            var best = 0;
            int bestI = 0, bestJ = 0;
            for (var i1 = 0 ; i1 < w; ++i1)
            for (var j1 = 0; j1 < h; ++j1)
            {
                if (g[i1, j1] != '#') continue;
                var viewable = 0; // from i1,j1

                for (var i2 = 0; i2 < w; ++i2)
                for (var j2 = 0; j2 < h; ++j2)
                {
                    if (g[i2, j2] != '#') continue;

                    int hits1 = 0;
                    if (i1 == i2 && j1 == j2)
                        continue;
                    var (dx, dy) = (i2 - i1, j2 - j1);
                    var (x, y) = (i1, j1);
                    if (dx == 0 || dy == 0)
                    {
                        var (sx, sy) = (Math.Sign(dx), Math.Sign(dy));
                        do
                        {
                            x += sx;
                            y += sy;
                            if (g[x, y] != '.')
                                ++hits1;
                        } while (x != i2 || y != j2);
                    }
                    else
                    {
                        var d = gcd(Math.Abs(dx), Math.Abs(dy));
                        do
                        {
                            x += dx / d;
                            y += dy / d;
                            if (g[x, y] != '.')
                            {
                                ++hits1;
                            }
                        } while (x != i2 || y != j2);
                    }


                    if (hits1 == 1)
                    {
                        ++viewable;
                    }

                }

                if (best < viewable)
                {
                    (bestI, bestJ) = (i1, j1);
                    best = Math.Max(best, viewable);
                    // Console.WriteLine($"Best {best} at {i1},{j1}");
                }
            }
            if (!part2)
                return best;


            // compute all ray directions from bestI, bestJ, order
            List<(int dx, int dy)> rays1 = new();
            for (var i1 = 0; i1 < w; ++i1)
            for (var j1 = 0; j1 < h; ++j1)
            {
                var (dx, dy) = (i1 - bestI, j1 - bestJ);
                if (dx == 0 && dy == 0) continue;
                if (dx == 0 || dy == 0)
                    (dx, dy) = (Math.Sign(dx), Math.Sign(dy));
                else
                {
                    var d = gcd(Math.Abs(dx),Math.Abs(dy));
                    (dx, dy) = (dx / d, dy / d);
                }

                if (!rays1.Contains((dx, dy)))
                    rays1.Add((dx, dy));
            }

           // Console.WriteLine($"best {bestI} {bestJ}");
            // sort clockwise: 
            var rays = rays1.Select(r => (dx:r.dx, dy:r.dy, a:Angle(r))).ToList();
            rays.Sort((a,b)=>a.a.CompareTo(b.a));

            // now shoot
            g[bestI, bestJ] = 'X';
          //  Dump(g,true);
            var hits2 = 0;
            var raysFired = 0;
            while (true)
            {
                var ray = rays[raysFired];
                raysFired = (raysFired+1)%rays.Count;
                var (hit,hx,hy) = Hits(bestI, bestJ, ray);
                if (hit)
                {
                    ++hits2;
                  //  Console.WriteLine($"Vapor {hits2} {hx} {hy}");
                }

                if (hits2 == 200)
                {
                  //  Dump(g, true);
                    return 100*hx+ hy;
                }
            }

            (bool hits, int xh, int yh) Hits(int x, int y, (int dx,int dy,double ) ray)
            {
                while (true)
                {
                    x += ray.dx;
                    y += ray.dy;
                    if (x < 0 || y < 0 || w <= x || h <= y)
                    {
                        return (false, -1, 1);
                    }

                    if (g[x, y] == '#')
                    {
                        g[x, y] = '.';
                        return (true,x,y);
                    }
                } 
            }


            double Angle((int dx, int dy) p)
            {
                //var a = Math.Atan2(-p.dy, p.dx); // -y gives clockwise
                var a = Math.Atan2(p.dx, -p.dy); // up = 0, clockwise
                if (a < 0) a += Math.PI * 2;
                Trace.Assert(0<=a && a<2*Math.PI);
                return a; // clockwise
            }


        }
    }
}