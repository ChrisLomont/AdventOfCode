

namespace Lomont.AdventOfCode._2018
{
    internal class Day03 : AdventOfCode
    {
        /*
         * 2018 Day 3 part 1: 112418 in 27483 us
2018 Day 3 part 2: 560 in 13407.9 us
         */
        object Run2()
        {
            long answer = 0;

            foreach (var line in ReadLines())
            {
                var nums = Numbers64(line);
            }

            return answer;
        }

        public override object Run(bool part2)
        {
            int sz = 1000; // big enough?
            int[,] grid = new int[sz, sz];

            // #1292 @ 415,21: 24x25
            var r = new Regex("^#(?<claim>[0-9]+) @ (?<dx>[0-9]+),(?<dy>[0-9]+): (?<w>[0-9]+)x(?<h>[0-9]+)$");

            foreach (var line in ReadLines())
            {
                var m = r.Match(line);
                var dx = Int32.Parse(m.Groups["dx"].Value);
                var dy = Int32.Parse(m.Groups["dy"].Value);
                var w = Int32.Parse(m.Groups["w"].Value);
                var h = Int32.Parse(m.Groups["h"].Value);

                for (int i = dx; i<dx+w; ++i)
                for (int j = dy; j < dy + h; ++j)
                    grid[i, j]++;

            }

            if (!part2)
                return Count(grid, v => v > 1);

            foreach (var line in ReadLines())
            {
                var m = r.Match(line);
                var dx = Int32.Parse(m.Groups["dx"].Value);
                var dy = Int32.Parse(m.Groups["dy"].Value);
                var w = Int32.Parse(m.Groups["w"].Value);
                var h = Int32.Parse(m.Groups["h"].Value);

                bool hits = false;
                for (int i = dx; i < dx + w; ++i)
                for (int j = dy; j < dy + h; ++j)
                    if (grid[i, j] > 1)
                        hits = true;
                
                if (!hits)
                    return m.Groups["claim"].Value;
            }


            return -1;

        }
    }
}