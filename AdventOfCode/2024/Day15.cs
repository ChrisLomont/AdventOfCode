

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
                        if (CanPush(xy,del,false))
                        {
                            CanPush(xy,del,true);
                            Set(pos, '.');
                            pos = xy;
                            Set(pos, '@');
                        }
                    }
                }
            }

            // given sq with box piece bx,by, and move dx,dy, return true if can push it
            bool CanPush(vec2 bxy, vec2 dir, bool doMove)
            {
                //var (bx, by) = bxy;
                //var (dx, dy) = dir;

                //var (bx2, by2) = (bx, by);
                var c = Get(bxy);//g[bx, by];
                if (c == '#') return false;
                Trace.Assert("[]".Contains(c));
                char c2 = c;
                vec2 bxy2;
                if (c == '[')
                {
                    bxy2 = bxy + new vec2(1,0);
                    //bx2 = bx + 1;
                    c2 = ']';
                }
                else
                {
                    bxy2 = bxy + new vec2(-1, 0);
                    //bx2 = bx - 1;
                    c2 = '[';
                }

                Trace.Assert(c2 == Get(bxy2));//g[bx2, by2]));

              //  var bxy2 = new vec2(bx2,by2);
                var m1 = Get(bxy+dir)/* g[bx + dx, by + dy]*/ == '.' || CanPush(bxy+dir,dir,doMove);
                var m2 = Get(bxy2+dir)/* g[bx2 + dx, by2 + dy] */== '.' || CanPush(bxy2+dir,dir, doMove);


                if (doMove)
                {
                    // after moving children, move bx, bx2 from by to by + dy
                    Set(bxy+dir,Get(bxy));
                    Set(bxy2 + dir, Get(bxy2));
                    Set(bxy, '.');
                    Set(bxy2,'.');

                    //g[bx + dx, by + dy] = g[bx, by];
                    //g[bx2 + dx, by2 + dy] = g[bx2, by2];
                        //g[bx, by] = '.';
                        //g[bx2, by2] = '.';

                }

                return m1 && m2;
            }

        }
        

        



    }
}