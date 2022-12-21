namespace Lomont.AdventOfCode._2019
{
    internal class Day18 : AdventOfCode
    {
        static vec3[] dirs = new[]
        {
            vec3.XAxis,
            vec3.YAxis,
            -vec3.XAxis,
            -vec3.YAxis
        };

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


            var maxPath = Path(ePos,null,w,h,g);

            // search:
            // 1. From start, branch to each key
            // 2. from (ungotten key or unopenend door) branch to each (ungotten) key, unopenened door

            // thus paths of interest are:
            // start  toany key (get it)
            // each key or door to each key or door

            Console.WriteLine("Start paths");
            Dictionary<char, List<(vec3, char)>> startToKey = new();
            foreach (var k in keys)
                startToKey.Add(k.Key, Path(ePos, k.Value, w, h, g));
            //todo;

            var keyToKey   = Make(keys,keys);
            var doorToKey  = Make(doors, keys);
            var keyToDoor  = Make(keys, doors);
            var doorToDoor = Make(doors, doors);

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


            // compute a path
            static List<(vec3 pos,char ch)> Path(vec3 start, vec3? end, int w, int h, char[,] g)
            {
                // a flood fill of depth from start
                Queue<(vec3, int)> open = new();

                HashSet<vec3> closed = new();

                open.Enqueue((start, 0));

                vec3?[,] parent = new vec3[w, h];

                var maxDepth = 0;
                vec3? maxNode = null;

                bool found = false;
                while (open.Any() && !found)
                {
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

                var ans = new List<(vec3,char)>();
                while (p != null)
                {
                    ans.Add((p, g[p.x, p.y]));
                    p = parent[p.x, p.y];
                }

                ans.Reverse();
                return ans;
            }

            Console.WriteLine($"maxd {maxPath.Count}");

            return -1;
        }
    }
}