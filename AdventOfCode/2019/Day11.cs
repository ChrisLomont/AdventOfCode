namespace Lomont.AdventOfCode._2019
{
    internal class Day11 : AdventOfCode
    {
        // 2373
        // PCKRLPUK
        public override object Run(bool part2)
        {
            
            var dirs = new[]
            {
                new vec3(0,1,0), // + is turn right, up is start
                new vec3(1,0,0),
                new vec3(0,-1,0),
                new vec3(-1,0,0),
            };
            var dir = 0;

            var prog = Numbers64(ReadLines()[0]).ToList();
            var input  = new Queue<long>();
            var output = new Queue<long>();
            var comp = new Day02.IntCode(prog,input.Dequeue,output.Enqueue);

            var panels = new Dictionary<vec3, int>();
            var pos = new vec3();



            int Read(vec3 v)
            {
                if (!panels.ContainsKey(v))
                    return 0;
                return panels[v];
            }

            int uniqueWrites = 0;
            void Write(vec3 v, int val)
            {
                if (!panels.ContainsKey(v))
                {
                    panels.Add(v, val);
                    uniqueWrites++;
                }
                else
                    panels[v] = val;
            }

            if (part2)
                Write(new vec3(),1);
            bool done = false;
            while (!done)
            {
                input.Enqueue(Read(pos));
                while (!done && output.Count < 2)
                {
                    done = comp.Step();
                }

                if (done) break;
                Write(pos, (int)output.Dequeue());
                var turn = output.Dequeue() == 0 ? -1 : 1;
                dir = (dir + turn+4) % 4;
                pos += dirs[dir];
            }

            if (part2)
            {
                var set = panels.Where(p=>p.Value == 1).Select(p=>p.Key).ToList();
                var min = set.Aggregate(vec3.MaxValue, (a, b) => vec3.Min(a, b));
                var max  = set.Aggregate(vec3.MinValue, (a, b) => vec3.Max(a, b));
                var (dx, dy, _) = max - min;
                var g = new char[dx+1,dy+1];
                
                Apply(
                    g, (i, j, v) =>
                    {
                        var key = new vec3(i, dy-j, 0) + min;
                        if (panels.ContainsKey(key) && panels[key] == 1)
                            return '#';
                        return '.';
                        
                    }
                );

                Dump(g, true);
                return "PCKRLPUK"; // read from console
            }
            return uniqueWrites;

        }
    }
}