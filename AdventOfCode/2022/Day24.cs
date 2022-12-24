
namespace Lomont.AdventOfCode._2022
{
    internal class Day24 : AdventOfCode
    {
        //top 1-100, both parts 10:09 - 26:48
        // first part 7:15 - 21:08

        //--------Part 1---------   --------Part 2---------
        //    Day Time    Rank Score       Time Rank  Score
        //24   01:09:31    1282      0   01:13:06    1085      0
        //23   00:43:49     931      0   00:51:50     999      0

        // orig time
        //2022 Day 24 part 1: 253 in 10714756.5 us
        //2022 Day 24 part 2: 794 in 32733987.2 us

        // cleaned, release mode
        //2022 Day 24 part 1: 253 in 422853 us
        //2022 Day 24 part 2: 794 in 974674.6 us

        public override object Run(bool part2)
        {
            var (w, h, g) = CharGrid();

            var dirs = new []
            {
                vec3.XAxis,
                -vec3.XAxis,
                vec3.YAxis,
                -vec3.YAxis,
                vec3.Zero
            };
             


            // indexed by column or row
            List<List<(vec3 start, int ydelta)>> vdrifts = new(); // start and y delta (dx = 0)
            
            // index by y
            List<List<(vec3 start, int xdelta)>> hdrifts = new(); // start and x delta (dy = 0)

            //List<(vec3 start, int dir)> drifts = new ();
            var chd = "><v^";
            Apply(
                g, (i, j, v) =>
                {
                    var k = chd.IndexOf(v);
                    if (k >= 0)
                    {
                        var s = new vec3(i, j);
                        if (k < 2)
                            Add(hdrifts, s, s.y, dirs[k].x);
                        else
                            Add(vdrifts, s, s.x, dirs[k].y);
                       // drifts.Add((s, k));
                    }

                    return v;
                }
                );

            while (hdrifts.Count < h)
                hdrifts.Add(new());
            while (vdrifts.Count < w)
                vdrifts.Add(new());

            void Add(List<List<(vec3 start, int ydelta)>> d, vec3 start, int index, int delta)
            {
                while (d.Count <= index)
                    d.Add(new());
                d[index].Add((start,delta));
            }

            var start = new vec3(1, 0);
            var end = new vec3(w-2,h-1);

            //for (var t = 0; t < 10; ++t)
            //{
            //    //Dump(GetMap(t),true);
            //    //Console.WriteLine();
            //}

            var time1 = PathTime(0, start, end);
            if (!part2)
                return time1;
            var time2 = PathTime(time1, end, start); // note backwards direction
            var time3 = PathTime(time2, start, end);
            return time3;

            int PathTime(int startTime, vec3 startPos, vec3 endPos)
            {
                var queue = new Queue<(int time, vec3 pos)>();
                queue.Enqueue((startTime, startPos));
                var seen = new HashSet<string>();
                //int pass = 0;
                while (queue.Any())
                {
                    var (time, pos) = queue.Dequeue();
                   // if (pass++%100 == 0) Console.WriteLine($"{queue.Count} {seen.Count} time {time}");
                    if (pos == endPos)
                        return time;
                    foreach (var dir in dirs)
                    {
                        var nxt = pos + dir;
                        if (Get(g, nxt.x, nxt.y, '#') == '#' || seen.Contains(Hash(time + 1, nxt)) ||
                            Hits(nxt, time + 1)) continue;
                        seen.Add(Hash(time + 1, nxt));
                        queue.Enqueue((time + 1, nxt));
                    }
                }
                throw new Exception(); // no solution
                static string Hash(int time, vec3 pos) => $"{time} {pos}";

                bool Hits(vec3 pos, int t)
                {
                    foreach (var (pt, del) in hdrifts[pos.y])
                    {
                        var m = w - 2;
                        var p = pt.x - 1 + t * del;
                        p = ((p % m) + m) % m; // pos mod
                        if (1 + p == pos.x)
                            return true;
                    }
                    foreach (var (pt, del) in vdrifts[pos.x])
                    {
                        var m = h - 2;
                        var p = pt.y - 1 + t * del;
                        p = ((p % m) + m) % m; // pos mod
                        if (1 + p == pos.y)
                            return true;
                    }
                    return false;
                }
            }

#if false // did not work - todo- figure out what is wrong?
            Dictionary<int, char[,]> maps = new();
            //Queue<(vec3 pos, int time)> open = new();

            var pq = new PriorityQueue<(vec3 pos, int time), int>();
            pq.Enqueue((start,0),(end-start).ManhattanDistance);

            //open.Enqueue((start,0));
            var closed = new HashSet<string>();


            var soln = -1L;
            while (pq.Count>0)
            {
                //Console.WriteLine($"open {open.Count} closed {closed.Count}");
                
                var (pos,time) = pq.Dequeue();
                if (pos == end)
                {
                    soln = time;
                    break;
                }

                closed.Add(hash(pos,time));
                var map = GetMap(time + 1);
                //Dump(map,true);
                foreach (var move in dirs)
                {
                    var nxt = pos + move;
                    var ch = Get(map, nxt.x, nxt.y, '%');
                    if (ch != '.') 
                        continue;
                    if (closed.Contains(hash(nxt,time+1)))
                        continue;
                    pq.Enqueue((nxt,time+1),(end-nxt).ManhattanDistance);
                }
            }

            string hash(vec3 pos, int time)
            {
                return $"{pos}-{time}";

            }

            return soln; // not 396

            char[,] GetMap(int time)
            {
                if (!maps.ContainsKey(time))
                {
                    //Console.WriteLine($"Map time {time}");
                    var f = new char[w, h];
                    Apply(f, (i, j, v) =>
                    {
                        return '.';
                    });
                    for (var i = 0; i < w; ++i)
                        f[i, 0] = f[i, h - 1] = '#';
                    for (var j = 0; j < h; ++j)
                        f[0,j] = f[w-1,j] = '#';
                    f[end.x, end.y] = '.';
                    f[start.x, start.y] = '.';
                    foreach (var (pos,dir) in drifts)
                    {
                        var (dw, dh) = (w - 2, h - 2);

                        var (dx, dy) = dirs[dir] * time;
                        dx = ((dx % dw) + dw) % dw;
                        dy = ((dy % dh) + dh) % dh;
                        f[
                            ((pos.x + dx - 1 + dw) % dw) + 1,
                            ((pos.y + dy - 1 + dh) % dh)+1
                        ] = chd[dir];
                    }

                    maps.Add(time,f);
                }
                return maps[time];
            }
#endif


        }
    }
}