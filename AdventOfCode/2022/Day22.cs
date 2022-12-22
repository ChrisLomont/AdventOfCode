
namespace Lomont.AdventOfCode._2022
{
    internal class Day22 : AdventOfCode
    {

        // hardcoded to my puzzle - not sure all are same shape
        // top 1-100: 25:56 to 1:14 both
        // 7:46 to 19:04 for first

        // me
        // --------Part 1---------   --------Part 2---------
        //     Day Time    Rank Score       Time Rank  Score
        // 22   00:45:06     952      0   01:53:04     505      0
        // 21   00:07:47     455      0   00:40:26    1041      0

        // 1:55 am:  525 both, 2570 one

    //2022 Day 22 part 1: 197160 in 11146.5 us
    //2022 Day 22 part 2: 145065 in 4020.4 us
        static List<vec3> dirs = new()
        {
            vec3.XAxis,
            vec3.YAxis,
            -vec3.XAxis,
            -vec3.YAxis,
        };

        static vec3 Forward2(vec3 pos, int val, char[,] g, int w, int h, string dc, ref int dir)
        {
            while (val > 0)
            {
                var (nxt,dir2) = Next(pos,dir);
                var c = Get(g, nxt.x, nxt.y, ' ');
                if (c == '.' || dc.Contains(c))
                {

                    // ok
                    pos = nxt;
                    dir = dir2;
                    val--;
                }

                else if (c == '#') break;
                g[pos.x, pos.y] = dc[dir];
            }

            return pos;

            (vec3 nxt, int newDir) Next(vec3 cur, int dir)
            {
                var nxt = cur + dirs[dir];
                var (i, j, _) = nxt;
                if (Get(g, i, j, ' ') != ' ')
                    return (nxt, dir); // good spot, continue

                // off end, use lookup tables
                for (var ti = 0; ti < tbls.Length; ++ti)
                {
                    var (hit,newDir) = tbls[ti].Match(i, j, dir);
                    if (0 <= hit)
                    {
                        // pair
                        var tj = (ti & 1) == 1 ? ti - 1 : ti + 1;
                        var q = tbls[tj].Get(hit)+dirs[newDir];
                       // Console.WriteLine($"Match {(char)('A'+ti/2)}: {nxt},{dirs[dir]} -> {q},{dirs[newDir]}");

                        return (q,newDir);
                    }
                }

                Console.WriteLine($"Fatal: cannot place {nxt}");
                Next(cur, dir); // for debugging
                throw new Exception();




            }
        }

        record T(int startx, int starty, vec3 raydir, vec3 newdir, vec3 olddir)
        {
            // index of hit, or -1 if none
            public (int index, int newDir) Match(int i, int j, int dir)
            {
                if (dirs[dir] != olddir)
                    return (-1,-1); // no match

                var p = new vec3(startx, starty);
                var q = new vec3(i, j);
                for (var hit = 0; hit < dw; ++hit)
                {
                    if (p == q)
                        return (hit, dirs.IndexOf(newdir));
                    p += raydir;
                }

                return (-1, -1);
            }

            public vec3 Get(int hit)
            {
                var v = new vec3(startx, starty);
                while (hit-->0)
                {
                    v += raydir;
                }

                return v;
            }

        }


        static vec3 N = -vec3.YAxis;
        static vec3 S = vec3.YAxis;
        static vec3 E = vec3.XAxis;
        static vec3 W = -vec3.XAxis;


        // for my puzzle
        static int dw = 50, dh = 50;

        // tbls:
        // each is (x,y) start of a 1 cell wide line, a dir to extend it 
        // then a new dir to go if in it, then a dir moving from
        // come in pairs: even pairs with next odd
        static T[] tbls =
        {
            // A
            new(dw,-1,E,E,N),
            new(-1,3*dh,S,S,W),

            // B
            new(dw,3*dh,E,W,S),
            new(dw,3*dh,S,N,E),
            
            // C
            new(3*dw,0,S,W,E),
            new(2*dw,3*dh-1,N,W,E),

            // D
            new(2*dw,dh,E,W,S),
            new(2*dw,dh,S,N,E),

            // E
            new(dw-1,0,S,E,W),
            new(-1,3*dh-1,N,E,W),

            // F
            new(dw-1,dh,S,S,W),
            new(0,2*dh-1,E,E,N),

            // G
            new(2*dw,-1,E,N,N),
            new(0,4*dh,E,S,S),

        };



        public override object Run(bool part2)
        {
            long score = 0;
            var lines = ReadLines();

            var (w, h, g) = CharGrid(lines.Take(lines.Count-2).ToList());
            var moves = lines.Last().Trim();
            //Dump(g,true);

            //Console.WriteLine($"{w} {h}");


            var dir = 0; // facing right
            // left -1, right +1

            var dc = ">v<^";

            var pos = Move(new vec3(), new vec3(1, 0, 0), c => c == ' ');

            int i = 0;
            while (i < moves.Length)
            {
              // Console.WriteLine($"{i}/{moves.Length}");
                var c = moves[i];
                if (char.IsAsciiLetter(c))
                {
                  //  Console.WriteLine("move " + c);
                    ++i;
                    if (c == 'L')
                        dir = (dir - 1 + 4) % 4;
                    if (c == 'R')
                        dir = (dir + 1 + 4) % 4;
                    g[pos.x, pos.y] = dc[dir];
                }
                else
                {
                    var val = 0;
                    while (i < moves.Length && Char.IsDigit(moves[i]))
                    {
                        var c1 = moves[i];
                        val = 10*val + (c1-'0');
                        ++i;
                    }
                  //  Console.WriteLine("move F " + val);

                    if (part2)
                        pos = Forward2(pos, val,g,w,h, dc, ref dir);
                    else
                        pos = Forward1(pos, dirs[dir], val, g, w, h);

                }
                // Dump(g,true);
                // Console.WriteLine();
            }

            vec3 Forward1(vec3 pos, vec3 dir1, int val, char[,] g, int w, int h)
            {
                while (val > 0)
                {
                    var nxt = pos + dir1;
                    var c = Get(g, nxt.x, nxt.y, ' ');
                    if (c == '.' || dc.Contains(c))
                    {
                        // ok
                        pos = nxt;
                        val--;
                    }

                    else if (c == '#') break;
                    else if (c == ' ')
                    {
                        // wrap, may hit wall
                        var good = new List<vec3> { pos };

                        nxt = nxt - Math.Max(w, h) * dir1;

                        while (val > 0)
                        {
                            nxt += dir1;
                            c = Get(g, nxt.x, nxt.y, ' ');
                            if (c == '.' || dc.Contains(c))
                            {
                                good.Add(nxt);
                                val--;
                            }
                            else if (c == '#')
                            {
                                nxt = good.Last();
                                val = 0;
                                break;
                            }
                        }

                        pos = nxt;

                    }

                    g[pos.x, pos.y] = dc[dir];
                }

                return pos;
            }

            var (col, row,_) = pos;
            return 1000 * (row+1) + 4 * (col+1) + dir;

            vec3 Move(vec3 s, vec3 d, Func<char,bool> func)
            {
                var p = s; 
                while (func(g[p.x,p.y]))
                {
                    p += d;
                }
                return p;
            }

            return score;
        }
    }
}