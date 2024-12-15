

using System.Runtime.Serialization;

namespace Lomont.AdventOfCode._2024
{
    internal class Day15 : AdventOfCode
    {
        /*
         2024 Day 15 part 1: 1485257 in 2814.5 us
         2024 Day 15 part 2: 1475512 in 136368.5 us
         */

        object Run3(bool part2)
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

            // start pos
            var h = gl.Count;
            var w = gl[0].Length;
            var g = new char[w, h];
            var pos = new vec2();
            //int sx = -1, sy = -1;
            Apply(g, (i, j, c) =>
            {
                c = gl[j][i];
                if (c == '@')
                    pos = new vec2(i,j);
                //{
                //    sx = i;
                //    sy = j;
                //}
                return c;
            });

            // moves
            foreach (var m in seq)
            {
                //       Console.WriteLine(m);
                switch (m)
                {
                    case 'v':
                        Move(new vec2(0, 1));
                        break;
                    case '>':
                        Move(new vec2(1, 0));
                        break;
                    case '<':
                        Move(new vec2(-1, 0));
                        break;
                    case '^':
                        Move(new vec2(0, -1));
                        break;
                }
            }


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
                //int x = sx + dx, y = sy + dy;
                var xy = pos + dir;
                var ch = Get(xy);//g[x, y]);
                if (ch == '#') return;
                if (ch == '.')
                {
                    Set(pos,'.');
                    //g[sx, sy] = '.';
                    pos = xy;
                    //sx = x;
                    //sy = y;
                    Set(pos,'@');
                    //g[sx, sy] = '@';
                }

                if (ch == 'O')
                {
                    // try push
                    //int tx = x, ty = y;
                    var txy = xy;
                    while (Get(txy)/*g[tx, ty]*/ == 'O')
                    {
                        txy += dir;
                        //tx += dx;
                        //ty += dy;
                    }

                    if (Get(txy)/*g[tx, ty]*/ != '#')
                    {
                        Set(txy,'O');

                        //g[tx, ty] = 'O';
                        //g[x, y] = '.';
                        Set(xy, '.');

                        //g[sx, sy] = '.';
                        Set(pos, '.');
                        //sx = x;
                        //sy = y;
                        pos = xy;
                        Set(pos,'@');
                        //g[sx, sy] = '@';
                    }
                }
            }

            void Move2(vec2 del)
            {
                //      Dump(g,noComma:true);
                ////      Console.WriteLine();
             //   int x = sx + dx, y = sy + dy;
             var xy = pos + del;
             var ch = Get(xy);//g[x, y]);
                if (ch == '#') return;
                if (ch == '.')
                {
                  //  g[sx, sy] = '.';
                  //  sx = x;
                  //  sy = y;
                  //  g[sx, sy] = '@';

                    Set(pos, '.');
                    //g[sx, sy] = '.';
                    pos = xy;
                    //sx = x;
                    //sy = y;
                    Set(pos, '@');
                    //g[sx, sy] = '@';

                }

                if (ch == '[' || ch == ']')
                {
                    var (x, y) = xy;
                    var (dx, dy) = del;

                    // try push
                    int tx = x, ty = y;
                    int ax = x, ay = y; // start x,y tile
                    if (dx != 0)
                    {
                        while ("[]".Contains(g[tx, ty]))
                        {
                            tx += dx;
                            ty += dy;
                        }

                        if (g[tx, ty] != '#')
                        {
                            // shift
                            while (tx != ax || ty != ay)
                            {
                                g[tx, ty] = g[tx - dx, ty - dy];
                                tx -= dx;
                                ty -= dy;
                            }
                            // last one
                            g[x, y] = '.';

                            var (sx, sy) = pos;
                            g[sx, sy] = '.';
                            sx = x;
                            sy = y;
                            g[sx, sy] = '@';
                            pos = new vec2(sx,sy);
                        }
                    }
                    else
                    {
                        HashSet<(int x, int y)> moved = new();
                        if (CanPush(x, y, dx, dy, false, moved))
                        {
                            CanPush(x, y, dx, dy, true, moved);

                            var (sx, sy) = pos;

                            g[sx, sy] = '.';
                            sx = x;
                            sy = y;
                            g[sx, sy] = '@';
                            pos = new vec2(sx, sy);


                        }
                    }
                }
            }

            // given sq with box piece bx,by, and move dx,dy, return true if can push it
            bool CanPush(int bx, int by, int dx, int dy, bool doMove, HashSet<(int, int)> moved)
            {
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

                var m1 = g[bx + dx, by + dy] == '.' || CanPush(bx + dx, by + dy, dx, dy, doMove, moved);
                var m2 = g[bx2 + dx, by2 + dy] == '.' || CanPush(bx2 + dx, by2 + dy, dx, dy, doMove, moved);


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

        object Run2()
        {
            List<string> gl = new List<string>();
            int phase = 0;
            string seq = "";
            foreach (var line1 in ReadLines())
            {
                if (line1.Length == 0)
                    phase++;


                if (phase == 0)
                {
                    var line2 = line1
                        .Replace("#", "##")
                        .Replace("O", "[]")
                        .Replace(".", "..")
                        .Replace("@", "@.");
                    gl.Add(line2);
                }
                else if (phase == 1)
                    seq += line1;
            }

            var h = gl.Count;
            var w = gl[0].Length;
            var g = new char[w, h];
            int sx = -1, sy = -1;
            Apply(g, (i, j, c) =>
            {
                c = gl[j][i];
                if (c == '@')
                {
                    sx = i;
                    sy = j;
                    //c = '.';
                }
                return c;
            });

            // moves
            foreach (var m in seq)
            {
          //       Console.WriteLine(m);
                switch (m)
                {
                    case 'v':
                        Move(0, 1);
                        break;
                    case '>':
                        Move(1, 0);
                        break;
                    case '<':
                        Move(-1, 0);
                        break;
                    case '^':
                        Move(0, -1);
                        break;
                }

             //  while (!Console.KeyAvailable)
             //   {
             //
             //   }
             //Console.ReadKey(true);

            }

            //Dump(g,noComma:true);

            void Move(int dx, int dy)
            {
           //      Dump(g,noComma:true);
            ////      Console.WriteLine();
                int x = sx + dx, y = sy + dy;
                var ch = g[x, y];
                if (ch == '#') return;
                if (ch == '.')
                {
                    g[sx, sy] = '.';
                    sx = x;
                    sy = y;
                    g[sx, sy] = '@';
                }

                if (ch == '[' || ch==']')
                {
                    // try push
                    int tx = x, ty = y;
                    int ax = x, ay = y; // start x,y tile
                    if (dx != 0)
                    {
                        while ("[]".Contains(g[tx, ty]))
                        {
                            tx += dx;
                            ty += dy;
                        }

                        if (g[tx, ty] != '#')
                        {
                            // shift
                            while (tx!=ax||ty!=ay)
                            {
                                g[tx, ty] = g[tx-dx,ty-dy];
                                tx -= dx;
                                ty -= dy;
                            }
                            // last one
                            g[x, y] = '.';

                            g[sx, sy] = '.';
                            sx = x;
                            sy = y;
                            g[sx, sy] = '@';
                        }
                    }
                    else
                    {
                        HashSet<(int x, int y)> moved = new();
                        if (CanPush(x, y, dx, dy, false, moved))
                        {
                            CanPush(x, y, dx, dy, true, moved);

                            g[sx, sy] = '.';
                            sx = x;
                            sy = y;
                            g[sx, sy] = '@';

                        }
                    }
                }
            }

            // given sq with box piece bx,by, and move dx,dy, return true if can push it
            bool CanPush(int bx, int by, int dx, int dy, bool doMove, HashSet<(int,int)> moved)
            {
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

                var m1 = g[bx + dx, by + dy] == '.' || CanPush(bx + dx, by + dy, dx, dy, doMove, moved);
                var m2 = g[bx2 + dx, by2 + dy] == '.' || CanPush(bx2 + dx, by2 + dy, dx, dy, doMove, moved);

                
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

            int answer = 0;
            Apply(g, (i, j, c) =>
            {
                if (c == '[')
                {
                     answer += 100 * j + i;
                }
                return c;
            });
            return answer;
        }

        //public override string TestFileSuffix() => "-test1";
        public override string TestFileSuffix() => "";

        object Run1()
        {
            List<string> gl = new List<string>();
            int phase = 0;
            string seq = "";
            foreach (var line in ReadLines())
            {
                if (line.Length == 0)
                    phase++;
                if (phase == 0)
                    gl.Add(line);
                else if (phase == 1)
                    seq += line;
            }

            var h = gl.Count;
            var w = gl[0].Length;
            var g = new char[w, h];
            int sx = -1, sy = -1;
            Apply(g, (i, j, c) =>
            {
                c = gl[j][i];
                if (c == '@')
                {
                    sx = i;
                    sy = j;
                    //c = '.';
                }
                return c;
            });

            // moves
            foreach (var m in seq)
            {
             //   Console.WriteLine(m);
                switch (m)
                {
                    case 'v':
                        Move(0, 1);
                        break;
                    case '>':
                        Move(1, 0);
                        break;
                    case '<':
                        Move(-1, 0);
                        break;
                    case '^':
                        Move(0, -1);
                        break;
                }
            }

            void Move(int dx, int dy)
            {
               // Dump(g,noComma:true);
              //  Console.WriteLine();
                int x = sx + dx, y = sy + dy;
                var ch = g[x,y];
                if (ch == '#') return;
                if (ch == '.')
                {
                    g[sx, sy] = '.';
                    sx = x;
                    sy = y;
                    g[sx, sy] = '@';
                }

                if (ch == 'O')
                {
                    // try push
                    int tx = x, ty = y;
                    while (g[tx,ty]=='O')
                    {
                        tx += dx;
                        ty += dy;
                    }

                    if (g[tx, ty] != '#')
                    {
                        g[tx, ty] = 'O';
                        g[x, y] = '.';

                        g[sx, sy] = '.';
                        sx = x;
                        sy = y;
                        g[sx, sy] = '@';
                    }
                }
            }

            int answer = 0;
            Apply(g, (i, j, c) =>
            {
                if (c == 'O')
                {
                    answer += 100*j + i;
                }
                return c;
            });
            return answer;
        }



        public override object Run(bool part2)
        {
            return Run3(part2);
            return part2 ? Run2() : Run1();
        }
    }
}