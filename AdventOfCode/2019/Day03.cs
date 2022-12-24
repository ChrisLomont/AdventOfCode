namespace Lomont.AdventOfCode._2019
{
    internal class Day03 : AdventOfCode
    {
        public override object Run(bool part2)
        {
            var lines = ReadLines();
            var l1 = lines[0];
            var l2 = lines[1];

            var (dist1,path1) = Parse(l1);
            var (dist2,path2) = Parse(l2);

            var h2 = new HashSet<vec3>();
            h2.UnionWith(path2);

            var c = new List<vec3>();
            foreach (var a in path1)
            {
                if (h2.Contains(a) && a != vec3.Zero)
                {
                    c.Add(a);
                }
            }

            if (!part2)
            {
                return c.Min(c => c.ManhattanDistance);
            }

            var min = int.MaxValue;
            foreach (var k in c)
            {
                var i1 = path1.IndexOf(k);
                var i2 = path2.IndexOf(k);
                min = Math.Min(min, dist1[i1] + dist2[i2]);
            }

            return min;



            (List<int>,List<vec3>) Parse(string path)
            {
                List<vec3> p = new();
                var ds = new List<int>();
                var v1 = new vec3(0, 0, 0);
                var words = path.Split(',');
                int dist = 0;
                foreach (var w in words)
                {
                    var val = int.Parse(w[1..]);
                    var v2 = v1 + val * w[0] switch
                    {
                        'U' => -vec3.YAxis,
                        'D' => vec3.YAxis,
                        'R' => vec3.XAxis,
                        'L' => -vec3.XAxis,
                    };
                    foreach (var (x,y) in DDA.Dim2(v1.x,v1.y,v2.x,v2.y))
                    {
                        var xx = new vec3(x, y);
                        if (!p.Any() || p.Last() != xx)
                        {
                            p.Add(xx);
                            ds.Add(dist++);
                        }
                    }

                    v1 = v2;


                }

                return (ds,p);
            }
        }
    }
}