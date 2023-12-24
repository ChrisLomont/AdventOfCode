using System.ComponentModel.DataAnnotations;

namespace Lomont.AdventOfCode._2023
{
    internal class Day23 : AdventOfCode
    {
        /*
         2402
6450

      --------Part 1---------   --------Part 2--------
Day       Time    Rank  Score       Time   Rank  Score
 23   00:14:23     326      0   01:07:17    530      0
 22   00:42:00     760      0   00:55:25    690      0
         */

        object Run2Graph()
        {
            var dirs = new List<vec2>
            {
                new vec2(1,0),
                new vec2(-1,0),
                new vec2(0,1),
                new vec2(0,-1)
            };
            var (w, h, g) = CharGrid();
            bool Legal(vec2 v) => 0 <= v.x && 0 <= v.y && v.x < w && v.y < h;

            List<vec2> nbrs(vec2 s) => dirs.Select(d => d + s).Where(q => Legal(q) && g[q.x, q.y] != '#').ToList();


            var start = new vec2();
            var end = new vec2();
            int top = 0, bot = 0;

            List<vec2> splits = new();



            // get start, end, and places where there's a split in the path
            Apply(g, (i, j, c) =>
            {
                Assert("#.>v".Contains(c));

                var v = new vec2(i, j);
                if (j == 0 && c == '.')
                    start = v;
                if (j == h - 1 && c == '.')
                    end = v;
                if (c != '#')
                {
                    var paths = nbrs(v).Count();
                    if (paths > 2)
                        splits.Add(v);
                }

                return c;
            });

          //  Console.WriteLine($"top {top} bot {bot} splits {splits.Count}");

            // add start, end to splits, so they are points of interest
            splits.Add(start);
            splits.Add(end);

            // for each split, compute dest splits and length from one to next
            // map split to list of dest,length
            // also compute path for image drawing
            Dictionary<vec2, List<(vec2 dest, int length, List<vec2> path)>> graph = new();


            foreach (var s in splits)
            {
                List<(vec2 dest, int length, List<vec2> path)> paths = new ();

                var starts = nbrs(s);
                foreach (var st in starts)
                {
                    List<vec2> seen = new() { s };
                    // run till next split
                    var cur = st;
                    while (!splits.Contains(cur))
                    {
                        seen.Add(cur);
                        var next = nbrs(cur).Where(v=>!seen.Contains(v)).ToList();
                        Assert(next.Count==1);
                        cur = next[0];
                    }
                    // now have paths from s to st and length
                    paths.Add((cur,seen.Count,seen));
                    Assert(splits.Contains(cur));
                }
                graph.Add(s,paths);
            }


            // depth first recurse
            int max = 0;
            long recurses = new();
            HashSet<vec2> seen1 = new() { start };
            //List<vec2> soln = new();


            //var pixsz = 10;
            //GIFMaker img = new(w * pixsz, h * pixsz);
            //Draw();


            Recurse(start, 0);

            //img.Save("Day_23_2023.gif");

            Console.WriteLine($"Recurses {recurses}");

            return max;

            void Recurse(vec2 pos, int depth)
            {
                //soln.Add(pos);
                ++recurses;
                if (pos == end)
                {
                    if (depth > max)
                    {
                        max = depth;
                      //  Console.WriteLine($"max {max}");
                        //Dump();
                        //Console.WriteLine();
                    }

                  //  Draw();
                }
                else
                {

                    //Console.Clear();
                    // Dump();
                    // Console.ReadKey(true);
                    var paths = graph[pos];

                    foreach (var path in paths)
                    {
                        var dest = path.dest;
                        if (!seen1.Contains(dest))
                        {
                            seen1.Add(dest);
                            Recurse(dest, depth + path.length);
                            seen1.Remove(dest);
                        }
                    }
                }
                //soln.RemoveAt(soln.Count - 1);
            }

            #if false
            void Draw()
            {
                byte[,] pix = new byte[w * pixsz, h * pixsz];
                void P(int i, int j, int c)
                {
                    for (int i1 = 0; i1 < pixsz; ++i1)
                    for (int j1 = 0; j1 < pixsz; ++j1)
                    {
                        pix[i * pixsz + i1, j * pixsz + j1] = (byte)c;
                    }

                }
                // draw
                // draw paths
                for (int j = 0; j < h; ++j)
                for (int i = 0; i < w; ++i)
                {
                    var c = g[i, j] == '#' ? 15 : 0;
                    if (splits.Contains(new vec2(i, j)))
                        c = 90; //6*16;
                    P(i,j,c);
                }

                // draw path
            //    Console.WriteLine($"Draw {soln.Count-1}");

                for (var i = 0; i < soln.Count - 1; ++i)
                {
                    var src = soln[i];
                    var dst = soln[i + 1];
                    var pt = graph[src].Where(p => p.dest == dst).ToList();
                    Assert(pt.Count == 1);
                    var path = pt[0].path;
                  //  Console.WriteLine($"  len {path.Count}");
                  // colors: 2 through 11, cycle, then * 16, -6
                    foreach (var p in path)
                    {
                        var c = (i % 10) + 2; // 2-11
                        c = c * 16 - 6; // 
                        P(p.x, p.y, c);
                    }
                }

                img.AddFrame(pix, 250);
            }
#endif
            
        }



        record State(int depth, vec2 cur, vec2 last, int nextDir);

        object Run2Stackless()
        {
            var (w, h, g) = CharGrid();

            int top = 0, bot = 0;
            Apply(g, (i, j, v) =>
            {
                if (j == 0 && v == '.')
                {
                    top = i;
                }
                if (j == h - 1 && v == '.')
                {
                    bot = i;
                }

                Assert("#.>v".Contains(v));

                return v;
            });

            Console.WriteLine($"top {top} bot {bot}");
            var pos = new vec2(top, 0);
            var end = new vec2(bot, h - 1);

            var dirs = new List<vec2>
            {
                new vec2(1,0),
                new vec2(-1,0),
                new vec2(0,1),
                new vec2(0,-1)
            };

            bool Legal(vec2 v) => 0 <= v.x && 0 <= v.y && v.x < w && v.y < h;

            var seen = new HashSet<vec2>();
            long max = 0;

            Stack<State> stack = new();
            var start = new State(0, pos, new vec2(-1, -1), 0);
            stack.Push(start);
            seen.Add(start.cur);
            seen.Add(start.last);

            R();

            void R()
            {
                while (stack.Any())
                {
                    var (depth, cur, last, nextDir) = stack.Pop();

                    if (cur == end)
                    {
                        if (depth > max)
                        {
                            max = depth;
                            Console.WriteLine($"max {max}");
                        }

                        seen.Remove(cur);
                    }
                    else
                    {


                        if (nextDir == 4)
                        {
                            seen.Remove(cur);
                        }
                        else
                        {

                            var d = dirs[nextDir];
                            nextDir++;
                            stack.Push(new(depth, cur, last, nextDir));

                            var n = d + cur;
                            if (Legal(n) && g[n.x, n.y] != '#' && !seen.Contains(n))
                            {
                                seen.Add(n);
                                stack.Push(new(depth + 1, n, cur, 0));
                            }
                        }
                    }

                }
            }
            return max;
        }


        object Run1()
        {
            var (w, h, g) = CharGrid();

            int top = 0, bot = 0;
            Apply(g,(i,j,v)=>
            {
                if (j == 0 && v == '.')
                {
                    top = i;
                }
                if (j == h-1 && v == '.')
                {
                    bot = i;
                }

                Assert("#.>v".Contains(v));

                return v;
            });

            Console.WriteLine($"top {top} bot {bot}");
            var pos = new vec2(top,0);
            var end = new vec2(bot, h - 1);

            var dirs = new List<vec2>
            {
                new vec2(1,0),
                new vec2(-1,0),
                new vec2(0,1),
                new vec2(0,-1)
            };

            bool Legal(vec2 v) => 0 <= v.x && 0 <= v.y && v.x < w && v.y < h;

            var seen = new HashSet<vec2>();
            long max = 0;

            seen.Add(pos);
            Recurse(pos,0);


            void Recurse(vec2 pos, long depth)
            {

                if (depth > max)
                {
                    max = depth;
                  //  Console.WriteLine($"max {max}");
                    //Dump();
                    //Console.WriteLine();
                }

                if (pos == end) 
                    return;

                //Console.Clear();
               // Dump();
               // Console.ReadKey(true);
                foreach (var d in dirs)
                {
                    var ch = g[pos.x, pos.y];
                    if (ch != '.')
                    {
                        // slope
                        if (ch == '>' && d != new vec2(1,0))
                            continue;
                        if (ch == 'v' && d != new vec2(0, 1))
                            continue;

                    }

                    var n = d + pos;
                    if (Legal(n) && g[n.x, n.y] != '#' && !seen.Contains(n))
                    {
                        seen.Add(n);
                        Recurse(n,depth+1);
                        seen.Remove(n);
                    }
                }
            }

            Recurse(pos, 0);

            return max;

            void Dump()
            {
                for (int j = 0; j < h; ++j)
                {
                    for (int i = 0; i < w; ++i)
                    {
                        var ch = g[i, j];
                        if (seen.Contains(new vec2(i, j)))
                            ch = 'O';
                        Console.Write(ch);
                    }

                    Console.WriteLine();
                }
            }
        }

 

        public override object Run(bool part2)
        {
            return part2 ? Run2Graph() : Run1();
        }
    }
}