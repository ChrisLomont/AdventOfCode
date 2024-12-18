

namespace Lomont.AdventOfCode._2018
{
    internal class Day11 : AdventOfCode
    {
        /*
         * 2018 Day 11 part 1: 21,22 in 3826.7 us
2018 Day 11 part 2: 235,288,13 max 147 in 43689.5 us
         */
        object Run2()
        {
            long gridSerialNumber = 7511;//Numbers64(ReadText())[0];

            int size = 300; //
            long[,] g = new long[size + 1, size + 1];
            for (int x = 1; x <= size; ++x)
            for (int y = 1; y <= size; ++y)
            {
                long rackID = x + 10;
                long powerLevel = rackID * y;
                powerLevel += gridSerialNumber;
                powerLevel *= rackID;
                powerLevel = (powerLevel % 1000) / 100;
                powerLevel -= 5;
                g[x, y] = powerLevel;
            }

            // convert to area table
            for (int x = 1; x <= size; ++x)
            for (int y = 2; y <= size; ++y)
                g[x, y] += g[x, y-1];

            for (int y = 1; y <= size; ++y)
            for (int x = 2; x <= size; ++x)
                g[x, y] += g[x - 1, y];

            // now check all squares
            var (mx, my,ms) = (0L, 0L,0L);
            var maxPower = long.MinValue;
            for (int s = 1; s <= size; ++s)
            {
                for (int x = 1; x <= size - s; ++x)
                for (int y = 1; y <= size - s; ++y)
                {
                    var (a,b,c,d) =
                        (g[x,y],
                            g[x+s,y],
                            g[x,y+s],
                            g[x+s,y+s]
                            );
                    var sum = d - b - c + a;
                    if (sum > maxPower)
                    {
                        maxPower = sum;
                        (mx, my, ms) = (x+1, y+1, s);
                    }
                }
            }




            return $"{mx},{my},{ms} max {maxPower}";
        }

        object Run1()
        {
            long gridSerialNumber = 7511;//Numbers64(ReadText())[0];
            
            int size = 300; //
            long[,] g = new long[size + 1, size + 1];
            for (int x = 1; x <= size; ++x)
            for (int y = 1; y <= size; ++y)
            {
                long rackID = x + 10;
                long powerLevel = rackID * y;
                powerLevel += gridSerialNumber;
                powerLevel *= rackID;
                powerLevel = (powerLevel % 1000) / 100;
                powerLevel -= 5;
                    g[x,y] = powerLevel;
                }

            var (mx, my) = (0, 0);
            var maxPower = long.MinValue;
            for (int x = 1; x <= size-3; ++x)
            for (int y = 1; y <= size-3; ++y)
            {
                var sum = 0L;
                for (int dx = 0; dx < 3; ++dx)
                for (int dy = 0; dy < 3; ++dy)
                    sum += g[x+dx, y+dy];
                if (sum > maxPower)
                {
                    maxPower = sum;
                    (mx, my) = (x, y);
                }
            }


            return $"{mx},{my}";
        }

        public override object Run(bool part2)
        {
            return part2 ? Run2() : Run1();
        }
    }
}