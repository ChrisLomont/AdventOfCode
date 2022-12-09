namespace Lomont.AdventOfCode._2021
{
    internal class Day11 : AdventOfCode
    {
        public override object Run(bool part2)
        {
            var (w,h,g) = DigitGrid();
            bool[,] flashed = new bool[w,h];

            // energy += 1
            // any > 9 flash, incr all nbrs by 1 (8 nbrs) , recurse on 9
            // any that flashed go to 0

            long flashCount = 0;
            int step = 0;
            while (true)
            {
                if (!part2 && step >= 100) break;
                step++;

                // all up by one
                Apply(g, (i, j, v) => v+1);

                // do flashes until stops
                bool stepFlashed = false;
                do
                {
                    stepFlashed = false;
                    Apply(g, (i, j, v) =>
                    {
                        if (v < 10)
                            return v;
                        if (flashed[i, j])
                            return v; // don't double flash
                        Flash(i, j);
                        stepFlashed = true;
                        return v;
                    });
                } while (stepFlashed == true);

                // count and reset flashes
                var stepFlash = 0;
                Apply(flashed, (i, j, v) =>
                    {
                        if (v)
                        {
                            stepFlash++;
                            g[i, j] = 0;
                        }

                        return false;
                    }
                );
                flashCount += stepFlash;
                if (part2 && stepFlash == w * h)
                    break;
            }

            if (part2)
                return step;
            return flashCount;

            void Flash(int i, int j)
            {
                flashed[i, j] = true;
                // 8 nbrs
                for (var di = -1; di <=1 ; ++di)
                for (var dj = -1; dj <= 1; ++dj)
                {
                    if (di == 0 && dj == 0) continue;
                        Incr(i+di, j+dj);
                }
            }

            void Incr(int i, int j)
            {
                if (i<0 || j < 0 || w <= i || h <= j) return;
                g[i, j]++;
            }


        }
    }
}