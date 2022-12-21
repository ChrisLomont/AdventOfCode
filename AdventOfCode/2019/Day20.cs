namespace Lomont.AdventOfCode._2019
{
    internal class Day20 : AdventOfCode
    {
        // this one was super painful to debug - vectors in the maps were getting corrupted, so I switched
        // to (i,j) in maps to force the level stuff from borking - another argument to make things immutable :)

        // 2019 Day 20 part 1: 684 in 29469.4 us
        // 2019 Day 20 part 2: 7758 in 5040425.4 us
        public override object Run(bool part2)
        {
            var (w, h, g) = CharGrid();
            string startName = "AA", endName = "ZZ";
            // start AA, end ZZ
            // 
            var closeChar = 'o';
            bool IsHallway(int i, int j)
            {
                var c = Get(g, i, j, ' ');
                return c == '.' || char.IsDigit(c) ||  c == closeChar;
            }

            // events to watch for for debugging
            var puzzSeqIndex = 0;
            var puzzSeq = new List<(string name, int fromLevel, int toLevel, int steps)>
            {
                ("XF",0,1,16),
                ("CK",1,2,10),
                ("ZH",2,3,14),
                ("WB",3,4,16),
                ("IC",4,5,16),
                ("RF",5,6,16),
                ("NM",6,7,16),
                ("LP",7,8,16),
                ("FD",8,9,16),
                ("XQ",9,10,16),
                ("WB",10,9,16),
                ("ZH",9,8,16),
                ("CK",8,7,16),
                ("XF",7,6,16),
                ("OA",6,5,16),
                ("CJ",5,4,16),
                ("RE",4,3,16),
                ("IC",3,4,16),
                ("RF",4,5,16),
                ("NM",5,6,16),
                ("LP",6,7,16),
                ("FD",7,8,16),
                ("XQ",8,9,16),
                ("WB",9,8,16),
                ("ZH",8,7,16),
                ("CK",7,6,16),
                ("XF",6,5,16),
                ("OA",5,4,16),
                ("CJ",4,3,16),
                ("RE",3,2,16),
                ("XQ",2,1,16),
                ("FD",1,0,16),
//                ("ZZ",0,1,16),
            };


            Dictionary<string, List< (int i, int j)>> doors = new ();
            Dictionary< (int i, int j), string> doors2 = new();


            // add door (with a letter at (i,j)(at sop) to system
            vec3 AddDoor(int i, int j)
            {
                var ch= Get(g, i , j, ' ').ToString();

                var rt = Get(g, i + 1, j, ' ');
                var v = new vec3(0,0);
                if (char.IsAsciiLetter(rt))
                {
                    if (IsHallway(i+2,j))
                        v = new vec3(i+2,j);
                    else
                        v = new vec3(i-1,j);
                    Add(v, ch + rt);
                }
                var dn = Get(g, i, j+1, ' ');
                if (char.IsAsciiLetter(dn))
                {
                    if (IsHallway(i,j+2))
                        v = new vec3(i, j+2);
                    else
                        v = new vec3(i , j-1);
                    Add(v, ch + dn);
                }

                void Add(vec3 v, string name)
                {
                    if (!doors.ContainsKey(name))
                        doors.Add(name,new());
                    doors[name].Add((v.x,v.y));

                    doors2.Add((v.x,v.y), name);

                }

                return v;

            }

            // search  out doors on map
            Apply(g, (i, j, v) =>
            {
                if (char.IsAsciiLetter(v))
                {
                 var p =  AddDoor(i, j);
                 //g[p.x, p.y] = '$'; // use to ensure doors correct
                }

                return v;
            });

            // other pair of a door if it exists
            // returns jump at level 0
            (bool jump, vec3 dest) Pair(vec3 ptIn)
            {
                var ptZero = new vec3(ptIn.x, ptIn.y, 0);
                var key = (ptIn.x, ptIn.y);
                if (!doors2.ContainsKey(key))
                    return (false, ptIn);

                var name = doors2[key];
                var pp = doors[name];
                var (p1, p2) = (pp[0], pp[1]);
                if (key == p1) return (true,new vec3(p2.i,p2.j));
                if (key == p2) return (true,new vec3(p1.i,p1.j));
                throw new Exception();
            }


            // NSWE directions
            var dirs = new[]
            {
                vec3.XAxis,
                -vec3.XAxis,
                vec3.YAxis,
                -vec3.YAxis,
            };

            // legal moves from given position
            // legal means there is a path, this does not check open/closed search lists...
            // points are x,y,level
            IEnumerable<vec3> LegalMoves(vec3 pos)
            {
                foreach (var dir in dirs)
                {
                    var nxt = pos + dir;
                    if (IsHallway(nxt.x,nxt.y))
                        yield return nxt;
                }

                var ( isJump, dest) = Pair(pos);
                if (!part2 && isJump)
                { // part 1
                    dest.z = pos.z; // keep level
                    yield return dest;
                }
                else if (part2 && isJump)
                { // part 2
                    // level 0: outer only has jumps AA and ZZ , inner goes one level deeper to outer
                    // level > 0: outer goes up a level to inner, inner goes one level deeper to outer

                    var b = 6; // border cells
                    var outer = pos.x < b || w-b < pos.x || pos.y < b || h-b < pos.y;
                    var level = pos.z;
                    // AA and ZZ removed from jump lists, so does not hit here
                    if (outer && level > 0)
                    {
                        dest.z = level-1; // go up
                        yield return dest;

                    }
                    else if (!outer)
                    { 
                        dest.z = level + 1; // go down
                        yield return dest;
                    }
                }
            }

            // positions are (x,y,level) where level = 0 in part1, is level of map in part2
            var start = ToVec(doors[startName][0]);
            var end  = ToVec(doors[endName][0]);

            vec3 ToVec((int i, int j) p) => new vec3(p.i, p.j);
            (int i, int j) FromVec(vec3 v) => (v.x, v.y);

            // remove start and end from maps to stop jumps
            doors2.Remove(doors[startName][0]);
            doors2.Remove(doors[endName][0]);
            doors.Remove(startName);
            doors.Remove(endName);

            // depth is search depth, pos is (x,y,level)
            var open = new Queue<(vec3 pos, int depth)>();
            var closed = new HashSet<string>();

            // i,j,lvl of parent for paths
            var parents = new List<vec3?[,]>();
            
            //var dd = "FD";
            //var ck = doors[dd];
            //Console.WriteLine($"start {start} end {end}, {dd} {ck[0]} & {ck[1]}");

            //Dump(LegalMoves(doors[dd][0]+new vec3(0,0,1)),true);
            //Dump(LegalMoves(doors[dd][1]),true);

            //return -1234;

            open.Enqueue((start,0));
            vec3 lastSet = new(-1,-1,-1);
            vec3 eraseNext = new();
            bool drawAll = true;

            while (open.Any())
            {

                var (p, d) = open.Dequeue();


                //var pz = new vec3(p.x, p.y, 0);
                //var name = doors2.ContainsKey(pz) ? $"{doors2[pz]}->{Pair(pz).dest}" : "";
                //Console.WriteLine($"Open {open.Count} {p} {d} {name}");

                if (p != start) Trace.Assert(parents[p.z][p.x,p.y] != null);

//                if (p.x == 13 && p.y == 2 && p.z == 0) //p == end)
                if (p == end)
                {
                   // Console.WriteLine("ZZ");
                    break; // hit z on correct level, done
                }


                var lvl = p.z;
                closed.Add(p.ToString());
                g[p.x, p.y] = (char)(((p.z)%10)+'0');// closeChar;
                lastSet = p;
              //  DumpLvl(p.z);


                // ensure space for path tracking
                if (parents.Count <= lvl+1)
                    parents.Add(new vec3?[w, h]);

                foreach (var nxt in LegalMoves(p))
                {
                    if (!closed.Contains(nxt.ToString()))
                    {
                        open.Enqueue((nxt, d + 1));
                        parents[nxt.z][nxt.x, nxt.y] = p;

                        //var puzzSeqIndex = 0;
                        //var puzzSeq = new List<(string name, int fromLevel, int toLevel, int steps)>
                        //{
                        //    ("XF",0,1,16),
                        //    ("CK",1,2,10),

                        var pz = new vec3(p.x,p.y,0);
                        var c1 = nxt.z != p.z;
                        var c2 = doors2.ContainsKey(FromVec(pz));
                        var c3 = c2&&doors2[FromVec(pz)] == puzzSeq[puzzSeqIndex].name;
                        var c4 = p.z == puzzSeq[puzzSeqIndex].fromLevel;
                        var c5 = nxt.z == puzzSeq[puzzSeqIndex].toLevel;
                        if (c1&&c2&&c3&&c4&&c5 && (puzzSeqIndex<puzzSeq.Count-1))
                        {
                            ++puzzSeqIndex;
                            var dir = (nxt.z > p.z) ? "Recurse into" : "Return to";
                          //  Console.WriteLine($"{dir} level {nxt.z} through {doors2[FromVec(pz)]}");
                          //  if (doors2[FromVec(pz)] == "FD")
                          //      Console.WriteLine("BREAK");

                        }

                        var name = doors2.ContainsKey(FromVec(pz)) ? $"{doors2[FromVec(pz)]}->{Pair(pz).dest}" : "                     ";
                       // Console.WriteLine($"Open {open.Count} depth {d} {p}->{nxt} {name}");
                    }
                }
            }

            void DumpLvl(int level)
            {
                var fg = Console.ForegroundColor;
                if (drawAll)
                {
                    Console.SetCursorPosition(0, 0);
                    for (var j = 0; j < h; ++j)
                    {
                        for (var i = 0; i < w; ++i)
                        {
                            if (i == lastSet.x && j == lastSet.y)
                                Console.ForegroundColor = ConsoleColor.Red;
                            Console.Write(g[i, j]);
                            Console.ForegroundColor = fg;
                        }

                        Console.WriteLine();
                    }

                    drawAll = false;
                }
                else
                {
                    if (eraseNext.x>=0)
                        Set(eraseNext);
                    Set(lastSet);
                    Console.SetCursorPosition(0,h+1);
                }

                eraseNext = lastSet;

                void Set(vec3 pos)
                {
                    Console.SetCursorPosition(pos.x, pos.y);
                    if (pos == lastSet)
                        Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write(g[pos.x, pos.y]);
                    Console.ForegroundColor = fg;
                }

            }


        // draw depth mod 10
        //            var c2 = new char[w, h];
        //            Apply(c2,(i,j,v)=>
        //            {
        //                if (g[i, j] != '.') return g[i, j];
        //                var d = depth[i, j];
        //                return (char)((d % 10) + '0');
        //            });
        //
        //            Dump(c2,true);

        List<vec3> path = new();
            vec3? cur = parents[0][end.x, end.y];
            while (cur != null)
            {
                path.Add(cur);
                cur = parents[cur.z][cur.x, cur.y];
            }

            path.Reverse();
            return path.Count;

            //var end = doors["ZZ"][0];
            //return depth[0][end.x, end.y];

            return -1234;

        }
    }
}