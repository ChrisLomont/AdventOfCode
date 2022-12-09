namespace Lomont.AdventOfCode._2021
{
    internal class Day17 : AdventOfCode
    {
        public override object Run(bool part2)
        {
            var lines = ReadLines();
            int x = 0, y = 0;
            int vx = 0, vy = 0;

            // target
            //int x0 = 20, x1 = 30;
            //int y0 = -10, y1 = -5;

            //target area: x = 96..125, y = -144..-98
            int x0 = 96, x1 = 125;
            int y0 = -144, y1 = -98;

            var maxHt = 0;
            var solns = 0;
            foreach (var line in lines)
            {

                (x0, x1) = (Math.Min(x0,x1), Math.Max(x0,x1));
                (y0, y1) = (Math.Min(y0, y1), Math.Max(y0, y1));
                //1149 too low
                for (var xs = 1; xs < 1000; ++xs)
                for (var ys = y0; ys < 1000; ++ys)
                {
                    x = 0;
                    y = 0;
                    vx = xs;
                    vy = ys;
                    //vx = 6;
                    //vy = 9;
                    //Console.WriteLine($"Start {vx},{vy}");
                    var maxHt2 = 0;
                    while (x <= x1 && y0 <= y)
                    {
                        maxHt2 = Math.Max(maxHt2, y);
                        if (x0 <= x && x <= x1 && y0 <= y && y <= y1)
                        {
                            //Console.WriteLine($"{xs},{ys} -> {maxHt2}");
                            maxHt = Math.Max(maxHt, maxHt2);
                            ++solns;
                            break;
                        }
                        SimStep();
                    }
                }
            }

            if (part2) return solns;
            return maxHt;

            void SimStep()
            {
                x += vx;
                y += vy;
                if (vx > 0) vx--;
                else if (vx < 0) vx++;
                vy -= 1;
                //Console.WriteLine($"  {x},{y} ");

            }

        }
    }
}