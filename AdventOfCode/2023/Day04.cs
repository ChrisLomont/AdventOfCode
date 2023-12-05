using System.ComponentModel.DataAnnotations;

namespace Lomont.AdventOfCode._2023
{
    internal class Day04 : AdventOfCode
    {
        /*
                    //foreach (var line in ReadLines()) { }
            // var (w,h,g) = DigitGrid();
            // var (w,h,g) = CharGrid();
            // Dump(g);
            // ProcessAllLines(new() { ... regex->action list ... });

         ********************************** day 2 2023 **********************************************
            var (sum, gameIndex) = (0, 0);
            var (max, temp, cap) = (new vec3(), new vec3(), new vec3(12, 13, 14));
            // Game 5: 3 red, 17 green, 10 blue; 9 blue, 5 green; 14 green, 9 blue, 11 red
            ProcessAllLines(new() {
                    { @"Game \d+:", (_, n) => {
                            gameIndex = n;
                            max = new vec3();
                            temp = new vec3(); } },
                    { @"\d+ red",   (_, n) => temp.x = n },
                    { @"\d+ green", (_, n) => temp.y = n },
                    { @"\d+ blue",  (_, n) => temp.z = n },
                    { ",",           _     => { } },
                    { " ",           _     => { } },
                    { ";",           _     => max = vec3.Max(max, temp) },
                    { "",            _     => { // end of line
                            max = vec3.Max(max, temp);
                            sum += part2 ? (max.x * max.y * max.z) : (max <= cap ? gameIndex : 0); }}
                    });
            return sum;        

        *******************************************************************
              var (w, h, g) = CharGrid();
         

            var (w,h,g) = DigitGrid();

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

         */

        /*
         * todo nicer start:
            throw new NotImplementedException("Year 2023, day 04 not implemented");
            int ans1 = 0, ans2 = 0;

            //foreach (var line in ReadLines()) { }
            // var (w,h,g) = DigitGrid();
            // var (w,h,g) = CharGrid();
            // ProcessAllLines(new() { ... regex->action list ... });

            return part2?ans2:ans1;
         */



        // 24733, 5422730

        public override object Run(bool part2)
        {
            var ans1 = 0;

            var lines = ReadLines();
            var counts = Enumerable.Repeat(1, lines.Count).ToList();

            foreach (var (line, i) in lines.WithIndex())
            {
                var w = line.Split(new []{'|',':'});

                var  matches = Intersect(Numbers(w[1]), Numbers(w[2])).Count;

                ans1 += matches == 0 ? 0 : 1 << (matches - 1);

                for (var j = 0; j < matches; ++j)
                    counts[i + 1 + j] += counts[i];
            }

            return part2 ? counts.Sum() : ans1;
        }
    }
}