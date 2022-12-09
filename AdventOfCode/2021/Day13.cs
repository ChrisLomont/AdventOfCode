namespace Lomont.AdventOfCode._2021
{
    internal class Day13 : AdventOfCode
    {
        public override object Run(bool part2)
        {
            // 850
            // AHGCPGAU
            var pts = new List<(int i,int j)>();
            var lines = ReadLines();
            foreach (var line in lines)
            {
                if (!line.Contains("fold") && line.Contains(','))
                {
                    var n = GetNumbers(line, false);
                    pts.Add(new(n[0], n[1]));
                }
            }

            var w = pts.Max(p => p.i) + 1;
            var h = pts.Max(p => p.j) + 1;
            var g = new int[w,h];
            foreach (var p in pts)
            {
                g[p.i,p.j] = 1;
            }

            foreach (var line in lines)
            {
                if (line.Contains("fold"))
                {
                    var v = GetNumbers(line, false)[0];
                    var ydir = line.Contains("y=");
                    if (ydir)
                    {
                        // below of y = y0 gets mapped up
                        for (var x = 0; x < w; ++x)
                        for (var y = v; y < h; ++y)
                        {
                            g[x,2*v-y] += g[x,y];
                        }
                        // resize:
                        h = v;
                        if (!part2)
                            break;
                    }
                    else
                    {
                        // right of x = x0 gets mapped left
                        for (var x = v; x < w; ++x)
                        for (var y = 0; y < h; ++y)
                        {
                            g[2*v-x, y] += g[x, y];
                        }
                        // resize:
                        w = v;
                        if (!part2)
                            break;
                    }
                }
            }

            if (part2)
            {
                for (var y = 0; y < h; ++y)
                {
                    for (var x = 0; x < w; ++x)
                    {
                        Console.Write(g[x, y] > 0 ? '#' : ' ');

                    }

                    Console.WriteLine();
                }

            }


            var c = 0;
            for (var x = 0; x < w; ++x)
            for (var y = 0; y < h; ++y)
                c += g[x,y] > 0?1:0;
            return c;
        }
    }
}