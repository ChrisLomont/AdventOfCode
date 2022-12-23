namespace Lomont.AdventOfCode._2019
{
    internal class Day17 : AdventOfCode
    {
    //2019 Day 17 part 1: 8928 in 28473.8 us
    //2019 Day 17 part 2: 880360 in 45074 us

        public override object Run(bool part2)
        {
            var input = new Queue<long>();
            var output = new Queue<long>();
            var prog = Numbers64(ReadLines()[0]);
            var comp = new Day02.IntCode(prog, input.Dequeue, output.Enqueue);

            while (!comp.Step())
            {

            }

            var d = output.ToList();
            var w = d.IndexOf(10); // char return
            var h = d.Count / (w + 1);
            var g = new char[w, h];
            Apply(g, (i, j, v) =>
            {
                return (char)d[i+j*(w+1)];
            });
            //Dump(g,noComma:true);

            var sum = 0;
            vec3 start = new();
            char sc = ' ';
            Apply(g, (i, j, v) =>
            {
                var vec = new vec3(i,j);
                var d0 = Get(g, i, j, ' ') == '#';
                var d1 = Get(g, i+1, j, ' ') == '#';
                var d2 = Get(g, i-1, j, ' ') == '#';
                var d3 = Get(g, i, j+1, ' ') == '#';
                var d4 = Get(g, i, j-1, ' ') == '#';
                if ("<^>v".Contains(v))
                {
                    start = new vec3(i,j);
                    sc = v;
                }

                if (d0&&d1 && d2 && d3 && d4)
                {
                    sum += i * j;
                    return v;
                    //return 'O';
                }
                else
                    return v;
            });
            //Dump(g,true);
            if (!part2)
                return sum;

            // get moves - go straight as much as possible
            // then get large sub patterns, see if fits

            // by hand (could do map search...)
            // R incr dir, L decr dir
            var path =  "L , 6,R ,12,L , 6,"
                       +"R ,12,L ,10,L , 4,"
                       +"L , 6,L , 6,R ,12,"
                       +"L , 6,R ,12,L ,10,"
                       +"L , 4,L , 6,L , 6,"
                       +"R ,12,L , 6,L ,10,"
                       +"L ,10,L , 4,L , 6,"
                       +"R ,12,L ,10,L , 4,"
                       +"L , 6,L ,10,L ,10,"
                       +"L , 4,L , 6,L , 6,"
                       +"R ,12,L , 6,L ,10,"
                       +"L ,10,L , 4,L , 6,";

            if (false)
            {// this section allowed finding the path above by drawing it
                var moves = path.Split(',', StringSplitOptions.RemoveEmptyEntries);
                var dir = 0; // up
                var dirs = new[]
                {
                    -vec3.YAxis,
                    vec3.XAxis,
                    vec3.YAxis,
                    -vec3.XAxis,
                };
                var md = "^>v<";
                var pos = start;
                foreach (var m1 in moves)
                {
                    var m = m1.Trim();
                    if (m == "L")
                        dir = (dir - 1 + 4) % 4;
                    else if (m == "R")
                        dir = (dir + 1) % 4;
                    else
                    {
                        var n = int.Parse(m);
                        g[pos.x, pos.y] = md[dir];
                        while (n-- > 0)
                        {
                            pos += dirs[dir];
                            g[pos.x, pos.y] = md[dir];
                        }
                    }
                }

                Dump(g, true);
                return -111;
            }
            
            var (progABC, prefs) = GetProgs(path);

            // run comp
            input.Clear();
            output.Clear();
            prog[0] = 2;
            comp = new Day02.IntCode(prog, input.Dequeue, output.Enqueue);

            bool show = false;

            Add(progABC);
            Add(prefs[0]);
            Add(prefs[1]);
            Add(prefs[2]);
            Add(show?"y":"n"); // video feed y or n

            void Add(string str)
            {
                str = str.Replace(" ", "");
                str = str.Trim(',');
                Trace.Assert(str.Length <= 20);
                str += '\n';
                foreach (var c in Encoding.ASCII.GetBytes(str))
                    input.Enqueue(c);
            }

            while (!comp.Step())
            {
                if (show)
                {
                    var sz = (w + 1) * h + 1;
                    if (output.Count >= sz)
                    {
                        Console.SetCursorPosition(0, 0);
                        for (var i = 0; i < sz; ++i)
                            Console.Write((char)output.Dequeue());
                        Console.WriteLine();
                        Console.WriteLine();
                    }
                }
            }

            long last = 0;
            //Console.WriteLine($"Amt {output.Count} screen size {(w+1)*h}");
            while (output.Any())
            {
                last = output.Dequeue();
                if (last < 255 && show)
                    Console.Write((char)last);
            }

            return last;

            static (string progABC, string[] funcs) GetProgs(string path)
            {
                string[] prefs = new string[3];
                string progABC = ""; // the ABC program
                // for each prefix, assign it to A, split rest. Recurse on pieces for B, C until all until all pieces done
                List<string> pieces = new() { path };
                if (FindABC(pieces, 0, prefs))
                {
                    //Console.WriteLine("SOLN!");

                    var p2 = path;
                    while (p2.Length > 0)
                    {
                        for (var i = 0; i < prefs.Length; ++i)
                            if (p2.StartsWith(prefs[i]))
                            {
                                progABC += (char)('A' + i) + ",";
                                p2 = p2[prefs[i].Length..];
                            }
                    }
                }

                return (progABC, prefs);
            }



            // find the ABC breakdown recursively
            static bool FindABC(List<string> p, int depth, string[] prefs)
            {
                if (depth == 3 && !p.Any())
                {
                    return true;
                }

                if (depth >= 3 || !p.Any())
                    return false;

                var fst = p.First();
                foreach (var pref in Prefixes(fst))
                {
                    if (pref.Replace(" ", "").Length > 20) continue;
                    prefs[depth] = pref;
                    var pieces2 = new List<string>();
                    foreach (var q1 in p)
                    foreach (var q2 in q1.Split(pref, StringSplitOptions.RemoveEmptyEntries))
                        pieces2.Add(q2);
                    // crashes
                    //var pieces2 = p.SelectMany(q => q.Split(pref))
                    //    .Where(s=>!String.IsNullOrEmpty(s))
                    //    .ToList();
                    if (FindABC(pieces2, depth + 1, prefs))
                        return true;
                }
                return false;
            }

            // get all prefixes of string separated by commas
            static IEnumerable<string> Prefixes(string p)
            {
                for (var i = 0; i < p.Length; ++i)
                {
                    if (p[i] == ',')
                    {
                        var pref = p[..(i + 1)];
                        yield return pref;
                    }

                }
            }

            return -1234;

        }
    }
}

/*
L,6,
R,12,
L,6,
R,12,
L,10,
L,4,
L,6,
L,6,
R,12,
L,6,
R,12,
L,10,
L,4,
L,6,
L,6,
R,12,
L,6,
L,10,
L,10,
L,4,
L,6,
R,12,
L,10,
L,4,
L,6,
L,10,
L,10,
L,4,
L,6,
L,6,
R,12,
L,6,
L,10,
L,10,
L,4,
L,6

 */