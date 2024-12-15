

namespace Lomont.AdventOfCode._2024
{
    internal class Day14 : AdventOfCode
    {
        object Run2()
        {
            long answer = 0;

            long w = 11, h = 7; // room size
            w = 101; h = 103;
           
            List<(long px, long py, long vx, long vy)> rob = new();

            foreach (var line in ReadLines())
            {
                var nums = Numbers64(line);
                long px = nums[0], py = nums[1], vx = nums[2], vy = nums[3];
                rob.Add(new(px,py,vx,vy));

              
            }

            long maxx = 0, maxy=0;
            long maxHas = 0, maxHasTime = 0;
            for (long s = 0; s < w * h; ++s)
            {

                char[,] g = new char[w, h];
                Apply(g, c => '.');
                long[] cx = new long[w];
                long[] cy = new long[h];
                foreach (var (px,py,vx,vy) in rob)
                {
                    var fx = PosMod(px+vx*s,w);
                    var fy = PosMod(py + vy * s, h);
                    g[fx, fy] = '*';
                    cx[fx]++;
                    cy[fy]++;
                }

                bool hasx = false, hasy = false;
                // look for lines
                var mx = cx.Max();
                if (mx >= maxx)
                {
                    maxx = mx;
                  //  Dump(g,noComma:true);
//                    Console.WriteLine($"X {s} {mx}");

                    hasx = true;
                }
                var my = cy.Max();
                if (my >= maxy)
                {
                    maxy = my;
                  //  Dump(g, noComma: true);
                  //  Console.WriteLine($"Y {s} {my}");
                    hasy = true;

                }

                if (hasx && hasy)
                {
                  //  Console.WriteLine($"Both {s}");
                   //  Dump(g, noComma: true);
                     if (maxx + maxy > maxHas)
                     {
                         maxHas = maxy + maxx;
                         maxHasTime = s;
                     }
                }

            }

            return maxHasTime;


        }
        //public override string TestFileSuffix() => "-test1";
        public override string TestFileSuffix() => "";

        object Run1()
        {
            long answer = 0;

            long w = 11, h = 7; // room size
            w = 101; h = 103;
            long s = 100; // time
            long[,] g = new long[w, h];

            foreach (var line in ReadLines())
            {
                var nums = Numbers64(line);
                long px = nums[0], py = nums[1], vx = nums[2], vy = nums[3];

                long fx = PosMod( px + vx * s, w);
                long fy = PosMod(py + vy * s,h);
                g[fx, fy]++;

            }

            long cx = w/2,cy= h/2;  
            long q1 = 0, q2 = 0, q3 = 0, q4 = 0;
            Apply(g, (i, j, v) =>
            {
                if (i < cx && j < cy)
                {
                    q1+=v;
                }
                if (i <cx && j > cy)
                {
                    q2 += v;
                }
                if (i > cx && j > cy)
                {
                    q3 += v;
                }
                if (i > cx && j <cy)
                {
                    q4 += v;
                }

                return v;
            });

         ////   Dump(g,noComma:true);

         //   Console.WriteLine($"{q1} {q2} {q3} {q4}");

            return q1*q2*q3*q4;
        }

        long PosMod(long a, long b)
        {
            return ((a % b) + b) % b;
        }

        public override object Run(bool part2)
        {
           
            return part2 ? Run2() : Run1();
        }
    }
}