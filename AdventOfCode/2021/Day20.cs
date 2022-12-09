
namespace Lomont.AdventOfCode._2021;
//20210
// 5486

internal class Day20 : AdventOfCode
{
    public override object Run(bool part2)
    {
        var lines = ReadLines();
        var map = lines[0];
        Trace.Assert(map.Length == 512);
        var (w, h, g) = CharGrid(lines.Skip(2).ToList());

        int steps =2;
        if (part2) steps = 50; // 20170 too low

        // embed in center of huge image?!
        var (w2, h2) = (w * 3, h * 3);
        var del = w / 2;

        //(w2,h2)=(w+steps*10,h+steps*10);
        //del = steps*10;
        var g2 = new char[w2, h2];
        Apply(g2, (i, j, c) => '.');
        Apply(g, (i, j, c) =>
        {
            g2[i+del, j+del] = c;
            return c;
        });
        g = g2;
        (w, h) = (w2, h2);

        char defCase = '.';
        //Dump(g);
        for (var s = 0; s < steps; ++s)
        {
            g = One(g, w, h, map, defCase);
            if (defCase == '.')
                defCase = map[0];
            else
                defCase = map[511];
        //    Dump(g);
            //Console.WriteLine(s);
        }
        // DAMMIT! :)
        // upper left corner gets noise from map[0] = '#', so off by one...
        // adding def case stuff above fixes?

        var cnt = 0;
        Apply(g, v => cnt += v == '#' ? 1 : 0);
        return cnt; 
        
        // 84992 too high,  5487 too high  5510 wrong

        // 5279 wrong


        // https://github.com/sultan308/Advent-of-code-2021/blob/main/C%2B%2B/Day20/day20.cpp
        // other example code: 5954, failed (+ part2 24006)
    }

    char[,] One(char[,] g, int w, int h, string map, char defCase)
    {
        // add border for next pass!
        var g3 = new char[w, h];

        Apply(g, (i, j, c) =>
            {
                int val = Get1(g, i, j, defCase);
                g3[i, j] = map[val];

                return c;
            }
        );
        return g3;
    }

    int Get1(char[,] g, int i, int j, char defCase)
    {
        int val = 0;
        for (var j1 = -1; j1 <= 1; ++j1)
        for (var i1 = -1; i1 <= 1; ++i1)
        {
            var cc = Get(g, i + i1, j + j1, defCase);
            val = 2 * val + (cc == '#' ? 1 : 0);
        }

        return val;
    }

}


