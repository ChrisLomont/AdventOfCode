using System.Data.SqlTypes;
using System.Security.AccessControl;

namespace Lomont.AdventOfCode._2023
{
    internal class Day24 : AdventOfCode
    {
        // 17906
        // 571093786416929

        record v3(long x, long y, long z);
        record ray(v3 pos, v3 dir);


        /* Need some lossless 2D intersection, no overflows, in long
                       * This line/line intersection
                       * return if hits, and fractional num/den for parametric on each line
                       */
        static (bool hits, bool sameLine, long num1, long num2, long den) Hits2D(v3 pos1, v3 dir1, v3 pos2, v3 dir2)
        {
            checked
            {
                var (a, b, c, d) = (pos1.x, dir1.x, pos1.y, dir1.y);
                var (e, f, g, h) = (pos2.x, dir2.x, pos2.y, dir2.y);
                // solve parametric eqns with params (s,t), without overflows, exactly
                // (a+bs,c+ds) == (e+ft,g+ht)

                // soln s = num1/den, point then pos1 + s * dir1

                var num1 = -(c * f - f * g - a * h + e * h);
                var num2 = -(b * c - a * d + d * e - b * g);
                var den = d * f - b * h;

                if (den == 0)
                {
                    // collinear, same line iff (a,c) on (e+ft, g+ht)
                    // iff (a-e)/f == (c-g)/h
                    var same = (a - e) * h == (c - g) * f;

                    //todo;
                    //Assert(den != 0); // handle later - collinear
                    //   Console.WriteLine("ERROR =- handle collinear");
                    return (false, same, 0, 0, 0);
                }
                return (true, false, num1, num2, den);
            }
        }


        // shift ray velocity in 2d by (dx,dy), compute intersections
        // also checks parameter is >= 0 and that the intersection point is consistent
        static (bool hits, double x0, double y0) ShiftedHit(ray ray1, ray ray2, long dx, long dy)
        {
            checked
            {
                var d1 = ray1.dir;
                v3 dir1 = new(d1.x - dx, d1.y - dy, 0);
                var d2 = ray2.dir;
                v3 dir2 = new(d2.x - dx, d2.y - dy, 0);
                var h = Hits2D(ray1.pos, dir1, ray2.pos, dir2);
                if (!h.hits) 
                    return (false, 0, 0);
                
                var t0 = (double)(h.num1) / h.den;
                var x0 = ray1.pos.x + t0 * dir1.x;
                var y0 = ray1.pos.y + t0 * dir1.y;

                var t1 = (double)(h.num2) / h.den;
                var x1 = ray2.pos.x + t1 * dir2.x;
                var y1 = ray2.pos.y + t1 * dir2.y;

                if (t0 < 0 || t1 < 0) 
                    return (false, 0, 0);

                var (ddx, ddy) = (x0 - x1, y0 - y1);
                var err = ddx * ddx + ddy * ddy;
                return (err < 1e-8, x0, y0);
            }
        }

        // check rays intersection under shift in the given point
        static bool Same(ray ray1A, ray ray2A, double x0, double y0, long dx, long dy)
        {
            checked
            {
                var (hitA, x0A, y0A) = ShiftedHit(ray1A, ray2A, dx, dy);
                if (!hitA)
                    return false;
                var (errx, erry) = (x0 - x0A, y0 - y0A);
                return errx * errx + erry * erry < 1e-9;
            }
        }

        // search speeds in 2D, see where all rays hit
        // get unique, or exception
        static (long dx, long dy) SearchVelocities2D(List<ray> rays, long maxSpeed)
        {
            checked
            {
                List<(long dx, long dy)> deltasXY = new();

                // find all possible dx,dy part of dir st 2d cross with (dxi, dyi) not zero
                for (long dx = -maxSpeed; dx <= maxSpeed; ++dx)
                for (long dy = -maxSpeed; dy <= maxSpeed; ++dy)
                {
                    var (hit, x0, y0) = ShiftedHit(rays[0], rays[1], dx, dy);
                    if (!hit)
                        continue;

                    var allOk = true;
                    for (var i = 0; i < rays.Count && allOk; ++i)
                    for (var j = i+1; j < rays.Count && allOk; ++j)
                    {
                        if (i == 0 && j == 1) 
                            continue;  // already done
                        allOk &= Same(rays[i], rays[j], x0, y0, dx, dy);
                    }
                    if (allOk)
                        deltasXY.Add((dx, dy));
                }

                Assert(deltasXY.Count == 1); // unique!
                return deltasXY[0];
            }
        }


        object Run2()
        {
            checked
            {
                List<ray> rays = new();
                foreach (var line in ReadLines())
                {
                    var n = Numbers64(line);
                    rays.Add(new(new(n[0], n[1], n[2]), new(n[3], n[4], n[5])));
                }

                long minBox = 200000000000000L, maxBox = 400000000000000;

                /* Idea 1: in frame of thrown rock p + t*d, rock is motionless, so intersection is all rays hitting same point
                 * at possibly different times. So in this frame, any two rays will hit here.
                 * This point is also then the coords of the starting point since in the new frame, the rock coords never move
                 *
                 * Idea 2: solve in 2D first, get a candidate (dx,dy) speed for rock. Then do same to get dz
                 */

                List<ray> skewRays = new();
                int startI = 0;
                while (skewRays.Count < 3)
                {
                    bool Diff(v3 a, v3 b) => a.x != b.x && a.y != b.y && a.z != b.z;

                    int selectI = startI;
                    while (selectI < rays.Count)
                    {
                        var noHits = true;
                        var rayI = rays[selectI];
                        foreach (var sk in skewRays)
                        {
                            if (!Diff(sk.dir, rayI.dir))
                            {
                                noHits = false;
                                break;
                            }
                        }

                        if (noHits)
                        {
                            startI = selectI + 1; // start here next time
                            skewRays.Add(rayI);
                            break;
                        }

                        ++selectI;
                    }
                }


                var maxV = 300; // make bigger till works

                // get unique xy velocity
                var(dx, dy) = SearchVelocities2D(skewRays, maxV);


                // find a dz from dx,dz as plane
                v3 SwapYZ(v3 v) => new v3(v.x, v.z, v.y);
                ray SwapYZr(ray r) => new(SwapYZ(r.pos), SwapYZ(r.dir));
                
                var dzRays = skewRays.Select(SwapYZr).ToList(); // swap y,z in each

                var (dx2, dz) = SearchVelocities2D(dzRays, maxV);

                Assert(dx == dx2);


                // the point to start is the hit point:
                var (hitXY, x0a, y0F) = ShiftedHit(skewRays[0], skewRays[1], dx, dy);
                var (hitXZ, x0b, z0F) = ShiftedHit(dzRays[0], dzRays[1], dx, dz);

                long finalx = (long)Math.Round(x0a);
                long finaly = (long)Math.Round(y0F);
                long finalz = (long)Math.Round(z0F);

                return finalx + finaly + finalz;
            }
        }


        public override object Run(bool part2)
        {
            if (part2) 
             return Run2();
            
            checked
            {
                List<(v3 pos, v3 dir)> rays = new();
                List<long> coords = new();
                List<long> dirs   = new();
                foreach (var line in ReadLines())
                {
                    var n = Numbers64(line);
                    coords.AddRange(n.Take(3));
                    dirs.AddRange(n.Skip(3).Take(3));
                    rays.Add(new(new(n[0], n[1], n[2]), new(n[3], n[4], n[5])));
                }

                Console.WriteLine($"{rays.Count} rays, Coords max {coords.Select(Math.Abs).Max()} vel max {dirs.Select(Math.Abs).Max()}");

                long minBox = 200000000000000L, maxBox = 400000000000000;
                //minBox = 7; maxBox = 27;

                int hitCount = 0;
                for (int i = 0; i < rays.Count; ++i)
                for (int j = i + 1; j < rays.Count; ++j)
                {
                    var ri = rays[i];
                    var rj = rays[j];

                    var (pi, di) = (ri.pos, ri.dir);
                    var (pj, dj) = (rj.pos, rj.dir);

               //     Console.WriteLine($"A : {pi}@{di}");
               //     Console.WriteLine($"B : {pj}@{dj}");

                    var (hits, sameLine, num1, num2, den) = Hits2D(pi, di, pj, dj);


                 //   var mi = (double)num1 / den;
                 //   var mj = (double)num2 / den;
                 //   var (fxi, fyi) = (pi.x + mi * di.x, pi.y + mi * di.y);
                 //   var (fxj, fyj) = (pj.x + mj * dj.x, pj.y + mj * dj.y);
                 //
                 //   Console.WriteLine($"Intersects {fxi},{fyi} = {fxj},{fyj}");


                     //   var ins = false;
                     if (hits)
                     {
                         var p = ri.pos;
                         var d = ri.dir;


                         var xInBox = InBox(p.x, d.x, num1, den, minBox, maxBox);
                         var yInBox = InBox(p.y, d.y, num1, den, minBox, maxBox);
                         var ok1 = Nonneg(num1, den);
                         var ok2 = Nonneg(num2, den);
                         if (xInBox && yInBox && ok1 && ok2)
                         {
                             //     ins = true;
                           //  Console.WriteLine("   Inside");
                             hitCount++;
                         }
                         else
                         {
                             //Console.WriteLine($" {xInBox} {yInBox} ");
                         }
                     }

                     if (sameLine)
                        { // see if any pos direction in box for both
                            // todo;
                            hitCount++;

                        }

                      //  if (!ins)
                      //      Console.WriteLine("   Missed");
                      //  Console.WriteLine();
                }

                return hitCount;

                // param nonneg
                bool Nonneg(long n, long d)
                {
                    return Math.Sign(n) * Math.Sign(d) >= 0;
                }

                /* parametric point in range min,max
                 pt of form a + b * (n/d)
                 */
                bool InBox(long a, long b, long n, long d, long min, long max)
                {
                    // min - a <= b * (n/d) <= max-a
                    // cannot mult out n, too big
                    // hope doubles are enough!
                    // todo - find optimal
                    //var bnd = (b * n)/d;
                    //var (a1, a2) = (min - a, max - a);
                    //if (a1 <= bnd && bnd <= a2) return true;
                    //return false;

                    //var bn = b * n;
                    //return (min - a) * d <= bn && bn <= (max - a) * d;
                    var br = b*((double)n / d);
                    return min - a <= br && br <= max - a;
                }

              


            } // checked
        }




#if false
// prev crap
        object Run2()
        {
            long answer = 0;
            // var (w,h,g) = DigitGrid();
            // var (w,h,g) = CharGrid();

            foreach (var line in ReadLines())
            {
                var nums = Numbers64(line);
            }

            return answer;
        }

        record v3(long x, long y, long z);

        object Run1()
        {
            checked{
            long answer = 0;
            long LARGE = 1000000L;

            List<(v3 pos, v3 dir)> dots =
                new();

            foreach (var line in ReadLines())
            {
                var nums = Numbers64(line);

                dots.Add(
                    new
                    (
                        new(nums[0], nums[1], nums[2]),
                        new(nums[3], nums[4], nums[5])
                    ));
            }

            long min = 7, max = 27; // test area
            min = 200000000000000L;
            max = 400000000000000L;

            for (int i = 0; i < dots.Count; ++i)
            for (int j = i + 1; j < dots.Count; ++j)
            {
              //  Console.WriteLine($"A: {dots[i].pos}@{dots[i].dir}");
              //  Console.WriteLine($"B: {dots[j].pos}@{dots[j].dir}");

                    var (ok, x, y, den) = Cramer(
                    dots[i].pos, dots[i].dir,
                    dots[j].pos, dots[j].dir
                    );
                    var crosses = false;
                if (ok)
                {
                    if (min*den <= x && min * den <= y && x <= max * den && y <= max * den)
                    {
                        var (dir1,den1) = Dir(dots[i].pos, dots[i].dir, x, y);
                        var (dir2,den2) = Dir(dots[j].pos, dots[j].dir, x, y);
                        //Console.WriteLine($"Dirs {dir1} {dir2}");
                        if (dir1*den1 >= 0 && dir2*den2 >= 0)
                        {
                            crosses = true;
                            answer++;
                        }
                    }
                }

                if (crosses)
                {
                 //   Console.WriteLine($"Crosses {x},{y}");
                }
                else
                {
                 //   Console.WriteLine($"Misses {x},{y}");
                }

                // return num, den
                (BigInteger num, BigInteger den) Dir(v3 pos, v3 dir, BigInteger x, BigInteger y)
                {
                    if (dir.x != 0)
                    {
                        return (-(pos.x - x) , dir.x);
                    }
                    else
                    {
                        return (-(pos.y - y) , dir.y);
                    }
                }

                // get t,k with intersection at p + t*s / k
                //var p = dots[i].pos;
                //var s = dots[i].dir;
                //var (ok, (t,k)) = Int(p, s, dots[j].pos, dots[j].dir);
                //if (ok)
                //{
                //
                //    answer++;
                //
                //}

            }

            (bool ok, BigInteger x, BigInteger y, BigInteger den) Cramer(v3 p1, v3 d1, v3 p2, v3 d2)
            {
                checked
                {
                    BigInteger x1 = p1.x, y1 = p1.y;
                    BigInteger x2 = p1.x + d1.x, y2 = p1.y + d1.y;
                    BigInteger x3 = p2.x, y3 = p2.y;
                    BigInteger x4 = p2.x + d2.x, y4 = p2.y + d2.y;
                    var den = (x1 - x2) * (y3 - y4) - (y1 - y2) * (x3 - x4);
                    if (den == 0)
                    {
                        // collinear
                        var hits = d1.y * (p1.x - p2.x) == d1.x * (p1.y - p2.y);
                        if (hits)
                            return (true, p1.x + LARGE * d1.x, p1.y + LARGE * d1.y, den);
//                    todo;

                        //                  Console.WriteLine("Zero");
                        return (false, 0, 0, 1); // todo
                    }

                    var xn = (x1 * y2 - y1 * x2) * (x3 - x4) - (x1 - x2) * (x3 * y4 - x4 * y3);
                    var yn = (x1 * y2 - y1 * x2) * (y3 - y4) - (y1 - y2) * (x3 * y4 - x4 * y3);
                    return (true, xn, yn , den);
                }
            }



            return answer;

            long cross2(v3 a, v3 b)
            {
                return a.x * b.y - a.y * b.x;
            }
            v3 sub(v3 a, v3 b)
            {
                return new v3(a.x - b.x, a.y - b.y, a.z - b.z);
            }

            // return const k, t,u where meet at p1+td1 / k

            (bool, (long t, long k)) Int(v3 p1, v3 d1, v3 p2, v3 d2)
            {
                v3 p = p1, r = d1; // p+r
                v3 q = p2, s =d2; //q+s;

                var crs = cross2(r, s);
                var qpr = cross2(sub(q, p), r);

                // scaled by crs
                var t = cross2(sub(q,p), s); //  / (r × s)
                var u = cross2(sub(q, p), r); //  / (r × s)

                if (crs == 0 && qpr == 0)
                {
                    Assert(false);
                    // collinerar;
                    // todo;
                }
                else if (crs == 0 && qpr != 0)
                {
                    // Parallel, non intersect
                    return (false, (0,0));
                }
                else // if (crs != 0)
                {
                    // meet at point p+tr = q + rs
                    return (true, (t, crs));
                }



                // https://stackoverflow.com/questions/563198/how-do-you-detect-where-two-line-segments-intersect


                return (false, (0,0));
            }

        


    }}

        public override object Run(bool part2)
        {
            return part2 ? Run2() : Run1();
        }
#endif
    }
}