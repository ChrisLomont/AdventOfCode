using System.Data;

namespace Lomont.AdventOfCode._2023
{
    internal class Day16 : AdventOfCode
    {
        /*
2023 Day 16 part 1: 7415 in 23069.1 us
2023 Day 16 part 2: 7943 in 3445546.3 us        
        
Day       Time    Rank  Score       Time   Rank  Score
 16   00:27:12    1318      0   00:33:02   1167      0
 15   00:05:45    1600      0   00:29:36   2090      0

First hundred users to get both stars on Day 16:

  1) Dec 16  00:04:33  Роман Курмаев
  2) Dec 16  00:07:47  Antonio Molina (AoC++) (Sponsor)
  3) Dec 16  00:08:31  bluepichu
  4) Dec 16  00:08:35  Oliver Ni (AoC++)
  5) Dec 16  00:08:37  Neal Wu
  6) Dec 16  00:08:42  Lovemathboy H
  7) Dec 16  00:08:53  Nathan Fenner (AoC++)
  8) Dec 16  00:08:59  Balint R

 96) Dec 16  00:15:26  Mathijs Vogelzang
 97) Dec 16  00:15:26  SizableShrimp
 98) Dec 16  00:15:28  jebouin (AoC++)
 99) Dec 16  00:15:30  tenth
100) Dec 16  00:15:30  spampacc
First hundred users to get the first star on Day 16:

  1) Dec 16  00:02:03  Роман Курмаев
  2) Dec 16  00:05:34  Antonio Molina (AoC++) (Sponsor)
  3) Dec 16  00:06:05  Oliver Ni (AoC++)
  4) Dec 16  00:06:05  Ian DeHaan

 94) Dec 16  00:11:26  tom-fibo
 95) Dec 16  00:11:29  Also looking 4 an internship! 
 96) Dec 16  00:11:29  zhangjunyan2580
 97) Dec 16  00:11:31  ishmeals
 98) Dec 16  00:11:33  Nate Paymer
 99) Dec 16  00:11:33  Alex Liang
100) Dec 16  00:11:36  Liresol
         */

        public override object Run(bool part2)
        {
            var N = new vec2(0, -1);
            var E = new vec2(1, 0);
            var S = new vec2(0, 1);
            var W = new vec2(-1, 0);

            // get sizes
            var (w, h, g) = CharGrid();

            if (!part2)
                return Raycast((new vec2(1, 0), new vec2(0, 0)));

            // max edges
            var best = 0L;
            for (var i = 0; i < w; ++i)
            {
                best = Math.Max(Raycast((S, new vec2(i, 0))), best);
                best = Math.Max(Raycast((N, new vec2(i, h - 1))), best);
            }

            for (var j = 0; j < h; ++j)
            {
                best = Math.Max(Raycast((E, new vec2(0, j))), best);
                best = Math.Max(Raycast((W, new vec2(w - 1, j))), best);
            }

            return best;


            long Raycast((vec2 dir, vec2 pos) start)
            {
                Queue<(vec2 dir, vec2 pos)> beams = new();
                HashSet<(vec2 dir, vec2 pos)> processed = new();

                beams.Enqueue(start); // right, top-left corner
                while (beams.TryDequeue(out var beam))
                {
                    var (dir, pos) = beam;

                    if (processed.Contains(beam))
                        continue;
                    processed.Add(beam);

                    var ch = g[pos.x, pos.y];
                    if (ch == '/')
                        Add(new vec2(-dir.y, -dir.x));
                    else if (ch == '\\')
                        Add(new vec2(dir.y, dir.x));
                    else if (ch == '-' && (dir == N || dir == S))
                        Add(W, E);
                    else if (ch == '|' && (dir == W || dir == E))
                        Add(N, S);
                    else
                        Add(dir);

                    void Add(params vec2[] dirs)
                    {
                        foreach (var dir1 in dirs)
                        {
                            var p2 = pos + dir1;
                            if (p2.x < 0 || p2.y < 0 || w <= p2.x || h <= p2.y)
                                continue;
                            beams.Enqueue((dir1, p2));
                        }
                    }

                }

                return Union(processed.Select(beam => beam.pos)).Count;
            }
        }
    }
}