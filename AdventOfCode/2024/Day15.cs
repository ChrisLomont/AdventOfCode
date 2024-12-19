

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
                        line2 = line2.Replace("#", "##")
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
            Apply(g, (i, j, c) => {
                c = gl[j][i];
                if (c == '@')
                    pos = new vec2(i,j);
                return c; });

            // moves
            Dictionary<char, vec2> dirs = new (){ ['^'] = new vec2(0, -1), ['>'] = new vec2(+1, 0), ['v'] = new vec2(0, +1), ['<'] = new vec2(-1, 0) };
            foreach (var dir in seq.Select(m => dirs[m]))
                if (part2) Move2(dir);
                else Move1(dir);

            // scoring
            int answer = 0;
            Apply(g, (i, j, c) => {
                if (c == boxs[0])
                    answer += 100 * j + i;
                return c; });
            return answer;

            char Get(vec2 v) => g[v.x,v.y];
            void Set(vec2 v, char c) => g[v.x,v.y] = c;

            void Upd(vec2 xy)
            {
                Set(pos, '.');
                pos = xy;
                Set(pos, '@');
            }

            void Move1(vec2 dir)
            {
                // Dump(g,noComma:true);
                //  Console.WriteLine();
                var xy = pos + dir;
                var ch = Get(xy);
                if (ch == '#') return;
                if (ch == '.')
                    Upd(xy);

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
                        Upd(xy);
                    }
                }
            }

            void Move2(vec2 dir)
            {
                //      Dump(g,noComma:true);
                ////      Console.WriteLine();
                var xy = pos + dir;
                var ch = Get(xy);
                if (ch == '#') return;
                if (ch == '.')
                    Upd(xy);

                if (ch == '[' || ch == ']')
                {
                    // try push
                    var txy = new vec2(xy.x, xy.y);
                    if (dir.x != 0)
                    {
                        while ("[]".Contains(Get(txy)))
                            txy += dir;

                        if (Get(txy) != '#')
                        {
                            // shift
                            while (txy != xy)
                            {
                                Set(txy, Get(txy - dir));
                                txy -= dir;
                            }

                            // last one
                            Set(xy, '.');
                            Upd(xy);
                        }
                    }
                    else
                    {
                        if (CanPush(xy, dir, false))
                        {
                            CanPush(xy, dir, true);
                            Upd(xy);
                        }
                    }
                }
            }

            // given sq with box piece bxy, and move dir, return true if can push it
            bool CanPush(vec2 bxy, vec2 dir, bool doMove)
            {
                var c1 = Get(bxy);
                if (c1 == '#') return false;
                Trace.Assert("[]".Contains(c1));
                char c2 = c1;
                vec2 bxy2;
                if (c1 == '[')
                {
                    bxy2 = bxy + new vec2(1,0);
                    c2 = ']';
                }
                else
                {
                    bxy2 = bxy + new vec2(-1, 0);
                    c2 = '[';
                }

                Trace.Assert(c2 == Get(bxy2));

                var m1 = Get(bxy+dir) == '.' || CanPush(bxy+dir,dir,doMove);
                var m2 = Get(bxy2+dir) == '.' || CanPush(bxy2+dir,dir, doMove);

                //doMove = m1 && m2;
                if (doMove)
                {
                    // after moving children, move this
                    Set(bxy + dir, Get(bxy));
                    Set(bxy2 + dir, Get(bxy2));
                    Set(bxy, '.');
                    Set(bxy2, '.');
                }

                return m1 && m2;
            }

        }
        

        



    }
}