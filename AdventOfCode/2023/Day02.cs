using System.Globalization;
using System.Security.Cryptography.X509Certificates;

namespace Lomont.AdventOfCode._2023
{
    internal class Day02 : AdventOfCode
    {
        /*
         // top both
  1) Dec 02  00:01:34  Corentin THOMASSET (AoC++)
  2) Dec 02  00:01:37  tung491
  3) Dec 02  00:02:52  xiaowuc1
  4) Dec 02  00:02:58  5space
  5) Dec 02  00:02:59  Dominick Han
 99) Dec 02  00:06:15  fish-and-bear
100) Dec 02  00:06:15  Cameron Montag (AoC++)
        // top 1st
  1) Dec 02  00:00:37  Dominick Han
  2) Dec 02  00:00:54  Corentin THOMASSET (AoC++)
  3) Dec 02  00:01:06  Carl Oscar Aaro (AoC++)
  4) Dec 02  00:01:06  tung491
  5) Dec 02  00:01:19  zach1502
 96) Dec 02  00:04:07  Михаил Минков
 97) Dec 02  00:04:07  Lucas M.
 98) Dec 02  00:04:07  Nikhil Menon
 99) Dec 02  00:04:08  carro
100) Dec 02  00:04:10  Adavya Goyal


      --------Part 1---------   --------Part 2--------
Day       Time    Rank  Score       Time   Rank  Score
  2   00:06:03     329      0   00:09:23    411      0
  1   17:45:07  131162      0   18:25:46  92867      0

        2023 Day 2 part 1: 2369 in 3951.7 us
        2023 Day 2 part 2: 66363 in 1391.7 us
        
         */


        public override object Run(bool part2)
        {
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
        }


        public object RunOld(bool part2)
        {
            long sum = 0;
            vec3 cap = new vec3(12,13,14);
            foreach (var line in ReadLines())
            {
                var gameAndResults = line.Split(':');
                var games = gameAndResults[1].Split(';');

                vec3 m = vec3.Zero;

                foreach (var draw in games)
                {
                    var pieces = draw.Split(",");
                    vec3 v = new vec3();
                    foreach (var p1 in pieces)
                    {
                        if (p1.Contains("red"))
                            v.x = Numbers(p1)[0];
                        else if (p1.Contains("green"))
                            v.y = Numbers(p1)[0];
                        else if (p1.Contains("blue"))
                            v.z = Numbers(p1)[0];
                    }

                    m = vec3.Max(m, v);
                }

                if (part2)
                {
                    var prod = m.x * m.y * m.z;
                    sum += prod;
                }
                else if (m <= cap)
                    sum += Numbers(gameAndResults[0])[0];
            }
            return sum;
        }
    }
}