namespace Lomont.AdventOfCode._2019
{
    internal class Day15 : AdventOfCode
    {

    //2019 Day 15 part 1: 210 in 6402539.9 us
    //2019 Day 15 part 2: 290 in 6370215.9 us

        static vec3 [] dirs = new[]
        {
            new vec3(),
            -vec3.YAxis,
            vec3.YAxis,
            -vec3.XAxis,
            vec3.XAxis
        };

        public override object Run(bool part2)
        {
            var prog = Numbers64(ReadLines()[0]);

            Dictionary<vec3, char> map = new();

            void Write(vec3 p, char c)
            {
                if (!map.ContainsKey(p))
                    map.Add(p, c);
                else
                    Trace.Assert(map[p] == c);
            }

            // got to end of path
            (bool collides, bool found, vec3 end) Run(string path)
            {

                Queue<long> input = new();
                Queue<long> output = new();
                var c = new Day02.IntCode(prog, input.Dequeue, output.Enqueue);
                var pos = new vec3();

                // D droid, # wall, . mapped, 
                Write(pos, 'O');

                var moves = path.Select(c => c - '0').ToList();
                int moveIndex = 0;
                foreach (var m in moves)
                    input.Enqueue(m);

                var collides = false;
                var found = false;
                while (!c.Step())
                {
                    // 1,2,3,4 = NSWE
                    if (output.Any())
                    { // 0 hit wall, 1 ok, 2 ok and sensor

                        int lastMove = moves[moveIndex++];

                        var o = output.Dequeue();
                        if (o == 1)
                        {
                            pos += dirs[lastMove];
                            if (pos == new vec3())
                                Write(pos, 'O');
                            else
                                Write(pos, '.');
                        }
                        else if (o == 0)
                        {
                            collides = true;
                            Write(pos + dirs[lastMove], '#');
                        }
                        else if (o == 2)
                        {
                            pos += dirs[lastMove];
                            Write(pos, 'S');
                            found = true;
                            break;
                        }
                        
                        if (moveIndex == moves.Count)
                            return (collides, found, pos);

                        //DrawDictMap(map);
                        //Console.WriteLine(lastMove);

                    }
                }
                return (collides,found,pos);
            }

            // recurse till found
            long ans = -1;
            vec3 oxygen = new();
            var open = new Queue<string>();
            var seen = new HashSet<vec3>();
            open.Enqueue("1");
            open.Enqueue("2");
            open.Enqueue("3");
            open.Enqueue("4");
            
            while (open.Any())
            {
                var p = open.Dequeue();
                foreach (var c in "1234")
                {
                    var (collide, found, pos) = Run(p + c);
                    if (!seen.Contains(pos) && !collide)
                        open.Enqueue(p + c);
                    seen.Add(pos);
                    if (found && ans == -1)
                    {
                        ans = (p + c).Length;
                        oxygen = pos;
                    }
                }
            }

           // DrawDictMap(map);

            if (!part2)
                return ans;

            // recurse on fill
            return FillTime(map, oxygen);
        }
        // recurse on fill
        static long FillTime(Dictionary<vec3,char> map, vec3 start)
        {

            var q = new Queue<(vec3,int)>();
            q.Enqueue((start,0));
            Dictionary<vec3, int> depths = new();

            while (q.Any())
            {
                var (pos,depth)= q.Dequeue();
                depths.Add(pos,depth);
                foreach (var d in dirs)
                {
                    var nxt = pos + d;
                    if (map[nxt] != '#' && !depths.ContainsKey(nxt))
                        q.Enqueue((nxt,depth+1));
                }
            }

            return depths.Max(p => p.Value);

        }

        // abstract this out
        static void DrawDictMap(Dictionary<vec3, char> map)
        {
                Console.SetCursorPosition(0, 0);
                var minx = map.Min(p => p.Key.x);
                var maxx = map.Max(p => p.Key.x);
                var miny = map.Min(p => p.Key.y);
                var maxy = map.Max(p => p.Key.y);
                var (dx, dy) = (maxx - minx, maxy - miny);
                var g = new char[dx + 1, dy + 1];
                Apply(g, (i, j, v) =>
                {
                    var key = new vec3(i+minx,j+miny);
                    if (map.ContainsKey(key))
                    {
                        return map[key];
                    }
                    return '_';
                });
                
                Dump(g, true);
        }
    }
}