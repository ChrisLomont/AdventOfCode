namespace Lomont.AdventOfCode._2021
{
    internal class Day15 : AdventOfCode
    {
        public override object Run(bool part2)
        {

            var (w, h, g) = DigitGrid();

            if (part2)
            {
                // expand by 5
                var g2 = new int[w * 5, h * 5];
                Apply(g2, (i, j, v) =>
                {
                    var t = g[i % w,j % h]; // src tile value
                    var md = (i / w) + (j / h); // manhattan dist

                    var s1 = (t - 1); // zero index
                    var s2 = s1 + md; // count tiles away
                    var s3 = s2 % 9; // 0-8
                    var s4 = s3 + 1; // 1-9, wrapped
                    return s4;

                });
                (w, h, g) = (w * 5, h * 5, g2);

            }


            var min = new int[w, h];
            Apply(min, (i, j, v) => int.MaxValue / 2 + 2 * g[i,j]);

            // recurse fill
            min[0, 0] = 0;


            bool changed = false;
            do
            {
                changed = false; // assume this
                Apply(min, (i, j, v) =>
                    {
                        // corner set
                        if (i == 0 && j == 0) return 0;

                        var sc = g[i,j] + MinNbr(i,j);
                        if (sc < min[i, j])
                        {
                            changed = true;
                            min[i, j] = sc;
                        }
                        // each is min of nbrs + g[i,j]
                        return min[i,j];
                    }
                    );
            //Dump(min);
            } while (changed);


            return min[w-1,h-1];

            int MinNbr(int i, int j)
            {
                int def = Int32.MaxValue;
                var t4 = new[]
                {
                    Get(min, i - 1, j, def),
                    Get(min, i + 1, j, def),
                    Get(min, i, j - 1, def),
                    Get(min, i, j + 1, def)
                };
                return t4.Min();
            }

        }
    }
}