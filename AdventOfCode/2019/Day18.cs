namespace Lomont.AdventOfCode._2019
{
    internal class Day18 : AdventOfCode
    {

        // https://github.com/joelgrus/advent2019/blob/master/day18/day18.py

        static vec3[] dirs = new[]
        {
            vec3.XAxis,
            vec3.YAxis,
            -vec3.XAxis,
            -vec3.YAxis
        };


     
        string[] puzz1 =
        {
            "#########",
            "#b.A.@.a#",
            "#########"
        };

        // a, then b, then c. Choice between d & e - e closer, d better long run, then e
        string[] puzz2= {
            "########################",
            "#f.D.E.e.C.b.A.@.a.B.c.#",
            "######################.#",
            "#d.....................#",
            "########################"
        };
        
        string[] puzz3 = {
"########################",
"#...............b.C.D.f#",
"#.######################",
"#.....@.a.B.c.d.A.e.F.g#",
"########################"
        };
        string[] puzz4 = {
"#################",
"#i.G..c...e..H.p#",
"########.########",
"#j.A..b...f..D.o#",
"########@########",
"#k.E..a...g..B.n#",
"########.########",
"#l.F..d...h..C.m#",
"#################"
        };
        string[] puzz5 = {
"########################",
"#@..............ac.GI.b#",
"###d#e#f################",
"###A#B#C################",
"###g#h#i################",
"########################"
        };

        public override object Run(bool part2)
        {
            var walls = new HashSet<vec3>();
            var keys = new Dictionary<vec3, char>();
            var doors = new Dictionary<vec3, char>();
            List<vec3> starts = new();

            //Test("1",puzz1, 8,"ab");
            //Test("2",puzz2, 86,"abcdef");
            //Test("3", puzz3, 132,"bacdfeg");
            //Test("4", puzz4, 136,"");
            //Test("5", puzz5, 81,"");

            void Test(string p, IEnumerable<string> lines, int answer, string path)
            {
                Load(lines);
                var (ans,path1) = ShortestPath();
                if (ans != answer || path != path1)
                {
                    Console.WriteLine($"ERROR: puzzle {p} scored {ans}:{path1}, should be {answer}:{path}");
                }

            }

            var lines = ReadLines();
            if (part2)
            {
                var w = lines[0].Length;
                var h = lines.Count;
                var (cx, cy) = (w / 2, h / 2);
                Trace.Assert(lines[cy][cx]=='@');

                //Console.WriteLine(lines[cy-1]);
                //Console.WriteLine(lines[cy ]);
                //Console.WriteLine(lines[cy + 1]);

                lines[cy - 1] = lines[cy - 1][0..(cx-1)] + "@#@" + lines[cy - 1][(cx + 2)..];
                lines[cy + 0] = lines[cy + 0][0..(cx-1)] + "###" + lines[cy + 0][(cx + 2)..];
                lines[cy + 1] = lines[cy + 1][0..(cx-1)] + "@#@" + lines[cy + 1][(cx + 2)..];

                //Console.WriteLine(lines[cy - 1]);
                //Console.WriteLine(lines[cy]);
                //Console.WriteLine(lines[cy + 1]);


                // ...      @#@
                // .@.  =>  ###
                // ...      @#@


            }
            Load(lines);

            if (!part2)
                return ShortestPath();
            return ShortestPath2();


            void Load(IEnumerable<string> lines)
            {
                walls.Clear();
                keys.Clear();
                doors.Clear();

                var (i, j) = (0, 0);
                foreach (var line in lines)
                {
                    foreach (var c in line)
                    {
                        var v = new vec3(i, j);
                        if (c == '#')
                            walls.Add(v);
                        else if (char.IsAsciiLetterUpper(c))
                            doors.Add(v, c);
                        else if (char.IsAsciiLetterLower(c))
                            keys.Add(v, c);
                        else if (c == '@')
                            starts.Add(v);
                        ++i;
                    }

                    i = 0;
                    ++j;
                }
            }

            // given start pos, get path dist to each place and doors crossed
            static Dictionary<char, (int steps, string doors)> ShortestPathsFromSource(vec3 src, HashSet<vec3> walls, Dictionary<vec3, char> keys, Dictionary<vec3, char> doors1)
            {
                // key is key_name, value is pair (distance, doors opened)
                var results = new Dictionary<char, (int steps, string doors)>();
                var visited = new HashSet<vec3> { src };


                var frontier = new Queue<(int steps, vec3 pos, string doors)>();
                frontier.Enqueue((0, src, ""));

                while (frontier.Any())
                {
                    var (numSteps, pos, doorsT) = frontier.Dequeue();
                    foreach (var dir in dirs)
                    {
                        var newPos = pos + dir;

                        if (visited.Contains(newPos) || walls.Contains(newPos))
                            continue;

                        visited.Add(newPos);

                        if (keys.TryGetValue(newPos, out char key))
                        {
                            results.Add(key, (numSteps + 1, doorsT));
                            frontier.Enqueue((numSteps + 1, newPos, doorsT));
                        }
                        else if (doors1.TryGetValue(newPos, out char door))
                        {
                            var newDoorsT = doorsT + door; // track doors in order is fine
                            frontier.Enqueue((numSteps + 1, newPos, newDoorsT));
                        }
                        else
                            frontier.Enqueue((numSteps + 1, newPos, doorsT));
                    }

                }

                return results;
            }

            // get shortest path from each key to each other place.
            Dictionary<char, Dictionary<char, (int steps, string doors)>>
                AllShortestPaths()
            {
                var results = new Dictionary<char, Dictionary<char, (int steps, string doors)>>();
                foreach (var start in starts)
                    results.Add('@',ShortestPathsFromSource(start,walls,keys,doors));

                foreach (var (keyLoc, key) in keys)
                    results.Add(key, ShortestPathsFromSource(keyLoc, walls, keys, doors));
                return results;
            }

            (int pathLength, string keyOrder) ShortestPath2()
            {
                throw new Exception();
            }

            (int pathLength, string keyOrder) ShortestPath()
            {
                var allPaths = AllShortestPaths();
                var seenSignatures = new HashSet<string>();

                var numKeys = keys.Count;

                int best = int.MaxValue;

                // # maintain priority queue of num_steps, key at, keys had
                // prioritize on shortest path (the priority queue key, 2nd parameter )
                var pq = new PriorityQueue<(int numSteps, char keyAt, string keysHad), int>();
                pq.Enqueue((0, '@', ""), 0); // want others more and more negative key

                while (pq.Count > 0)
                {
                    var (numSteps, sourceKey, keysHad) = pq.Dequeue(); // lowest priority

                    var sig = Signature(keysHad, sourceKey);
                    if (seenSignatures.Contains(sig))
                        continue;
                    seenSignatures.Add(sig);

                    if (keysHad.Length == numKeys)
                    {
                        //Console.WriteLine($"Key order {keysHad}");
                        return (numSteps, keysHad); // cannot be any shorter path due to priority queue
                    }

                    foreach (var (destKey, (stepsToKey, doors1)) in allPaths[sourceKey])
                    {
                        if (keysHad.Contains(destKey))
                            continue;
                        // skip path to any door for which we don't have the key 
                        var dh = new HashSet<char>();
                        var dk = doors1.ToLower().ToCharArray().ToList();
                        dh.UnionWith(dk);
                        dh.ExceptWith(keysHad.ToCharArray());
                        if (dh.Any())
                            continue;

                        Trace.Assert(!keysHad.Contains(destKey));
                        var newKeys = keysHad + destKey;

                        pq.Enqueue((numSteps + stepsToKey, destKey, newKeys), (numSteps + stepsToKey));
                    }
                }

                throw new Exception();
            }
            static string Signature(string keysHad, char key)
            {
                var sortedDistinct = keysHad.ToCharArray().Distinct().Order().Aggregate("", (a, b) => a + b);
                return $"{key}-{sortedDistinct}";
            }

#if false

def signature(prev_keys: Set[str], loc: str) -> str:
    return f"{loc}:{''.join(sorted(prev_keys))}"


#endif
        }

#if false
        public override object Run(bool part2)
        {
            var (w, h, g) = CharGrid();
            var (entrance,floor,wall) = ('@','.','#');
            // doors uppercase, keys lowercase


            Dictionary<char, vec3> doors = new();
            Dictionary<char, vec3> keys = new();

            vec3 ePos = new();
            Apply(
                g, (i, j, v) =>
                {
                    if (v == entrance)
                        ePos = new vec3(i, j);
                    if (char.IsAsciiLetter(v))
                    {
                        if (char.IsUpper(v))
                            doors.Add(v,new vec3(i,j));
                        else
                            keys.Add(v,new vec3(i, j));
                    }
                    return v;
                });

            // make solid map
            for (var j = 0; j < h; ++j)
            {
                for (var i =0 ;  i < w; ++i)
                {
                    Console.Write(g[i,j]=='#'?"\u2588":' ');
                }
                Console.WriteLine();
            }


            Console.WriteLine($"{doors.Count} doors, {keys.Count} keys");
            //return -112;



            // check map has no loops.
            AssertNoLoops(w,h,g);

            static void AssertNoLoops(int w,int h, char [,] g)
            {
                var escapable = new HashSet<vec3>();
                for (var j = 0; j < h; ++j)
                {
                    Console.WriteLine($"{j+1}/{h}, escapable {escapable.Count}");
                    for (var i = 0; i < w; ++i)
                    {
                        if (g[i, j] == '#')
                            Trace.Assert(Escapes(new vec3(i, j)));
                    }
                }


                bool Escapes(vec3 pos)
                {
                    bool escaped = false;
                    Queue<vec3> open = new();
                    var closed = new HashSet<vec3>();
                    open.Enqueue(pos);
                    
                    while (open.Any() && !escaped)
                    {
                        var p = open.Dequeue();
                        if (escapable.Contains(p))
                            escaped = true;

                        escaped |= (p.x == 0 || p.y == 0 || p.x == w - 1 || p.y == h - 1);
                        closed.Add(p);
                        foreach (var d in dirs)
                        {
                            var nxt = p + d;
                            if (!closed.Contains(nxt))
                                open.Enqueue(nxt);
                        }
                    }

                    if (escaped) escapable.UnionWith(closed);

                    return escaped;

                }
            }

            
            // will store open doors and obtained keys as bitfields
            // 32 bits per, in a 64 bit number
            Trace.Assert(keys.Count < 32 && doors.Count < 32);

            var maxPath = Path(ePos,null,w,h,g);

            // all paths of interest: 
            // interesting points are door and key locations
            // all paths is path between each interesting point,
            // and all paths from start pos to each interesting point.
            var allPaths = AllPaths();

            Console.WriteLine("Start search");

            // depth is how many moves into sequence we are
            var opened = new Queue<(vec3 pos,long state, int depth)>();
            opened.Enqueue((ePos,0, 0)); // no items opened or obtained
            var closed = new HashSet<vec3>();
            var allKeys = (1L << keys.Count) - 1;
            var lastClosedCount = 0;
            
            while (opened.Any()) 
            {
                if (lastClosedCount > closed.Count)
                {
                    Console.WriteLine("???ERROR");
                }


                lastClosedCount = closed.Count;
                var (pos,state, depth) = opened.Dequeue();
                closed.Add(pos);


                var os = state;
                // obtain end item:
                var itemBits = Bit(g[pos.x, pos.y]);
                state |= itemBits;
                Trace.Assert(os!=state || (g[pos.x,pos.y] == '@')); // sanity check

                Console.WriteLine($"pos {pos}, depth {depth}, obtained {g[pos.x,pos.y]}, openend {opened.Count}, closed {closed.Count}, doors {(state & 0xFFFFFFFF):X4} keys {(state >> 32):X4}");

                if ((state>>32) == allKeys)
                {
                    Console.WriteLine("All keys");
                    break;
                }

                var ourKeys = state >> 32;
                var ourDoors = state & 0xFFFF_FFFF;

                foreach (var (path,pathItems) in allPaths[pos])
                {
                    // path not interesting if we have item at end
                    var end = path.Last();
                    var endItem = g[end.x,end.y];
                    var endItemBit = Bit(endItem);
                    if ((endItemBit & state) == endItemBit) continue; // already done

                    if (closed.Contains(end)) continue;
                    //Trace.Assert(!closed.Contains(end));

                    // if end is door, only go if we have key
                    if (char.IsUpper(endItem) && (endItemBit & ourKeys) != endItemBit)
                        continue;

                    // consider if all doors passable
                    // door passable if
                    //   1. already open in state
                    //   2. we have the key already - then we must open that door for next state
                    // we don't consider paths where key is on way, that will be found as a different path

                    var doorsOnPath = pathItems & 0xFFFF_FFFF;

                    //var passableDoors = ourKeys | ourDoors;
                    //var passableDoors = ourKeys | ourDoors;

                    if ((doorsOnPath & ourDoors) == doorsOnPath)
                    { // we have enough to do it
                        var newState = state | doorsOnPath; // we opened these
                        opened.Enqueue((end,newState, depth+1));

                    }
                }
            }

            Console.WriteLine("Done?");

            return -1435;

            // create map of all paths to interesting places from current interesting place
            Dictionary<vec3,List<(List<vec3> path,long pathItems)>> 
                AllPaths()
            {

                var ans = new Dictionary<vec3, List<(List<vec3> path, long pathItems)>>();
                var pts = new List<vec3>();
                pts.AddRange(doors.Values);
                pts.AddRange(keys.Values);

                // paths between all non epos items
                for (var i = 0; i < pts.Count; ++i)
                for (var j = 0; j < pts.Count; ++j)
                {
                    if (i == j) continue;
                    AddPath(pts[i], pts[j]);
                }

                // paths from epos to all other items
                foreach (var dst in pts)
                    AddPath(ePos,dst);

                return ans;

                void AddPath(vec3 src, vec3 dst)
                {
                    var (path, pathItems) = Path(src, dst, w, h, g);
                    if (!ans.ContainsKey(src))
                        ans.Add(src, new());
                    ans[src].Add((path, pathItems));
                }

            }

#if false


            // thus paths of interest are:
            // start  to any key (get it)
            // each key or door to each key or door
            // so may as well compute, for each interesting point, the path to each other point and 

            Console.WriteLine("Start paths");
            Dictionary<char, List<(vec3, char)>> startToKey = new();
            foreach (var k in keys)
                startToKey.Add(k.Key, Path(ePos, k.Value, w, h, g));
            //todo;

            var keyToKey   = Make(keys,keys);
            var doorToKey  = Make(doors, keys);
            var keyToDoor  = Make(keys, doors);
            var doorToDoor = Make(doors, doors);

            Console.WriteLine("End paths");

            var open = new Queue<(vec3 pos,long depth, ulong state)>();
            var closed = new HashSet<string>();
            while (open.Any())
            {
                var (pos, depth, state) = open.Dequeue(); 
                closed.Add($"{pos},{depth},{state}");

                if (pos == ePos)
                { // to key

                }
                else if (keys.ContainsValue(pos))
                {
                    var ch = g[pos.x, pos.y];
                    Trace.Assert(Char.IsLetter(ch));
                    // take key or open door, go to other keys and doors
                    //r newState = ??
                }
                else if (doors.ContainsValue(pos))
                {
                    // open door, go to other keys and doors
                }

            }

            // todo - path is vecs, and a ulong mask of all hit items


            Console.WriteLine($"maxd {maxPath.Count}");

            return -1;

            Dictionary<(char, char), List<(vec3, char)>> Make(Dictionary<char, vec3> dict1,
                Dictionary<char, vec3> dict2)
            {
                Console.WriteLine($"Making {dict1.Count}x{dict2.Count}");
                Dictionary<(char, char), List<(vec3, char)>> oneToTwo = new();
                foreach (var p in dict1)
                foreach (var q in dict2)
                {
                    if (p.Key == q.Key) continue;
                    var path = Path(p.Value, q.Value, w, h, g);
                    Trace.Assert(path[0].ch == p.Key);
                    Trace.Assert(path.Last().ch == q.Key);
                    oneToTwo.Add((p.Key, q.Key), path);
                }

                return oneToTwo;
            }

#endif
        }

        // compute a path from start to end, return each position and char underneath
        // if end null, finds deepest path in system
        // also returns bitfield for interior path items (not at endpoints)
        static (List<vec3> path, long pathItems) Path(vec3 start, vec3? end, int w, int h, char[,] g)
        {
            // a flood fill of depth from start
            Queue<(vec3, int)> open = new();

            HashSet<vec3> closed = new(); // should never visit "interesting place" as a dest more than once

            open.Enqueue((start, 0));

            vec3?[,] parent = new vec3[w, h];

            var maxDepth = 0;
            vec3? maxNode = null;

            bool found = false;
            while (open.Any() && !found)
            {
                //Console.WriteLine($"open {open.Count}, closed {closed.Count}");
                var (pos, depth) = open.Dequeue();
                closed.Add(pos);

                if (maxDepth <= depth)
                {
                    maxDepth = depth;
                    maxNode = pos;
                }

                foreach (var d in dirs)
                {
                    var nxt = pos + d;
                    var (x, y, _) = nxt;
                    if (x < 0 || y < 0 || w <= x || h <= y) continue;
                    if (g[x, y] == '#') continue;
                    if (closed.Contains(nxt)) continue;


                    parent[x, y] = pos;
                    if (nxt == end)
                    {
                        found = true;
                        break;
                    }
                    open.Enqueue((nxt, depth + 1));
                }
            }

            vec3? p = null;
            if (found)
                p = end;
            else
                p = maxNode;

            var ans = new List<(vec3 pos, char ch)>();
            while (p != null)
            {
                ans.Add((p, g[p.x, p.y]));
                p = parent[p.x, p.y];
            }

            ans.Reverse();

            long pathKey = ans.Skip(1).Take(ans.Count - 2)
                .Aggregate(0L, (a, b) => a | Bit(b.ch));
            var path = ans.Select(p => p.pos).ToList();

            return (path,pathKey);
        }

        static long Bit(char ch)
        {
            if (ch == '.') return 0;
            if (ch == '@') return 0;
            if ('A' <= ch && ch <= 'Z') return 1L<<(ch - 'A'); // doors low
            if ('a' <= ch && ch <= 'z') return 1L<<((ch - 'a') + 32); // keys high
            throw new Exception();
        }
#endif


    }
}