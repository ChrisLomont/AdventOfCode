

using System.ComponentModel.DataAnnotations;
using Lomont.GIFLib;

namespace Lomont.AdventOfCode._2024
{
    internal class Day14 : AdventOfCode
    {

        /*
         * 2024 Day 14 part 1: 226548000 in 4964 us
2024 Day 14 part 2: 7753 in 261733.6 us
         */

        object Run2()
        {
            var (w, h) = (101, 103); // room size

            List<(long px, long py, long vx, long vy)> robots = new();

            foreach (var line in ReadLines())
            {
                var nums = Numbers64(line);
                long px = nums[0], py = nums[1], vx = nums[2], vy = nums[3];
                robots.Add(new(px,py,vx,vy));
            }

            // look for rows, columns with abnormal density
            long maxTime=0, maxVal = 0;

            // is w*h enough time?
            for (long s = 0; s < w * h; ++s)
            {
                long[] cx = new long[w];
                long[] cy = new long[h];
                foreach (var (px,py,vx,vy) in robots)
                {
                    var fx = NumberTheory.PositiveMod(px+vx*s,w);
                    var fy = NumberTheory.PositiveMod(py + vy * s, h);
                    cx[fx]++;
                    cy[fy]++;
                }
                var val = cx.Max()+cy.Max();
                if (val > maxVal)
                {
                    maxVal = val;
                    maxTime = s;
                }
            }

            return maxTime;


        }
        //public override string TestFileSuffix() => "-test1";
        public override string TestFileSuffix() => "";

        object Run1()
        {
            var (w,h)=(101,103); // room size
            long s = 100; // time

            long cx = w / 2, cy = h / 2; // center
            long[,] q = new long[2, 2];  // quarters

            foreach (var line in ReadLines())
            {
                var nums = Numbers64(line);
                long px = nums[0], py = nums[1], vx = nums[2], vy = nums[3];

                var fx = NumberTheory.PositiveMod( px + vx * s, w);
                var fy = NumberTheory.PositiveMod(py + vy * s,h);

                if (fx == cx || fy == cy) continue;

                q[fx/(cx+1),fy/(cy+1)]++;
            }
            return q[0, 0] * q[1, 0] * q[0, 1] * q[1, 1];
        }

        public override object Run(bool part2)
        {
            return part2 ? Run2() : Run1();
        }
    }
}