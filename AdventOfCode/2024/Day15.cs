

using System.Runtime.Serialization;

namespace Lomont.AdventOfCode._2024
{
    internal class Day15 : AdventOfCode
    {
        /*
         2024 Day 15 part 1: 1485257 in 2814.5 us
         2024 Day 15 part 2: 1475512 in 136368.5 us
         */

        //public override string TestFileSuffix() => "-test1";
        public override string TestFileSuffix() => "";

        public override object Run(bool part2)
        {
            List<string> gl = new List<string>();
            int phase = 0;
            string seq = "";
            var boxs = part2 ? "[]" : "O";

            foreach (var line in ReadLines())
            {
                if (line.Length == 0)
                    phase++;


                if (phase == 0)
                {
                    var line2 = line;
                    if (part2)
                    {
                        line2=line2.Replace("#", "##")
                            .Replace("O", "[]")
                            .Replace(".", "..")
                            .Replace("@", "@.");
                    }
                    gl.Add(line2);
                }
                else if (phase == 1)
                    seq += line;
            }

            // draw board, get start pos
            var h = gl.Count;
            var w = gl[0].Length;
            var g = new char[w, h];
            var pos = new vec2();
            Apply(g, (i, j, c) =>
            {
                c = gl[j][i];
                if (c == '@')
                    pos = new vec2(i,j);
                return c;
            });

            // moves
            Dictionary<char, vec2> dirs = new Dictionary<char, vec2> {
                ['^'] = new vec2(0, -1),
                ['>'] = new vec2(+1, 0),
                ['v'] = new vec2(0, +1),
                ['<'] = new vec2(-1, 0), };
            foreach (var m in seq)
                Move(dirs[m]);

            // scoring
            int answer = 0;
            Apply(g, (i, j, c) =>
            {
                if (c == boxs[0])
                {
                    answer += 100 * j + i;
                }
                return c;
            });
            return answer;

            //Dump(g,noComma:true);
            void Move(vec2 dir)
            {
                if (part2) Move2(dir);
                else Move1(dir);
            }

            char Get(vec2 v) => g[v.x,v.y];
            void Set(vec2 v, char c) => g[v.x,v.y] = c;

            void Move1(vec2 dir)
            {
                // Dump(g,noComma:true);
                //  Console.WriteLine();
                var xy = pos + dir;
                var ch = Get(xy);
                if (ch == '#') return;
                if (ch == '.')
                {
                    Set(pos,'.');
                    pos = xy;
                    Set(pos,'@');
                }

                if (boxs.Contains(ch))
                {
                    // try push
                    var txy = xy;
                    while (boxs.Contains(Get(txy)))
                        txy += dir;

                    if (Get(txy) != '#')
                    {
                        Set(txy,'O');
                        Set(xy, '.');

                        Set(pos, '.');
                        pos = xy;
                        Set(pos,'@');
                    }
                }
            }

            void Move2(vec2 del)
            {
                //      Dump(g,noComma:true);
                ////      Console.WriteLine();
                var xy = pos + del;
                var ch = Get(xy);
                if (ch == '#') return;
                if (ch == '.')
                {
                    Set(pos, '.');
                    pos = xy;
                    Set(pos, '@');
                }

                if (ch == '[' || ch == ']')
                {
                    var (x, y) = xy;
                    var (dx, dy) = del;

                    // try push
                    var txy = new vec2(xy.x, xy.y);
                    if (dx != 0)
                    {
                        while ("[]".Contains(Get(txy)))
                            txy += del;

                        if (Get(txy) != '#')
                        {
                            // shift
                            while (txy != xy)
                            {
                                Set(txy, Get(txy - del));
                                txy -= del;
                            }

                            // last one
                            Set(xy, '.');

                            Set(pos, '.');
                            pos = xy;
                            Set(pos, '@');
                        }
                    }
                    else
                    {
                        HashSet<(int x, int y)> moved = new();
                        if (CanPush(xy,del,/*x, y, dx, dy, */false, moved))
                        {
                            CanPush(xy,del,/*x, y, dx, dy, */true, moved);
                            Set(pos, '.');
                            pos = xy;
                            Set(pos, '@');
                        }
                    }
                }
            }

            // given sq with box piece bx,by, and move dx,dy, return true if can push it
            bool CanPush(vec2 bxy, vec2 dir, /*int bx, int by, int dx, int dy, */bool doMove, HashSet<(int, int)> moved)
            {
                var (bx, by) = bxy;
                var (dx, dy) = dir;

                var (bx2, by2) = (bx, by);
                var c = g[bx, by];
                if (c == '#') return false;
                Trace.Assert("[]".Contains(c));
                char c2 = c;
                if (c == '[')
                {
                    bx2 = bx + 1;
                    c2 = ']';
                }
                else
                {
                    bx2 = bx - 1;
                    c2 = '[';
                }
                Trace.Assert(c2 == g[bx2, by2]);

                var bxy2 = new vec2(bx2,by2);
                var m1 = g[bx + dx, by + dy] == '.' || CanPush(/*bx + dx, by + dy, dx, dy, */bxy+dir,dir,doMove, moved);
                var m2 = g[bx2 + dx, by2 + dy] == '.' || CanPush(/*bx2 + dx, by2 + dy, dx, dy,*/bxy2+dir,dir, doMove, moved);


                if (doMove)
                {
                    // after moving children, move bx, bx2 from by to by + dy
                    var p1 = (bx, by);
                    var p2 = (bx2, by2);
                    if (!moved.Contains(p1) && !moved.Contains(p2))
                    {
                        moved.Add(p1);
                        moved.Add(p2);

                        g[bx + dx, by + dy] = g[bx, by];
                        g[bx2 + dx, by2 + dy] = g[bx2, by2];
                        g[bx, by] = '.';
                        g[bx2, by2] = '.';

                    }

                }

                return (m1 && m2);
            }

        }
        

        



    }
}