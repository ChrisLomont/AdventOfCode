namespace Lomont.AdventOfCode._2021
{
    internal class Day12 : AdventOfCode
    {
        class Node
        {
            public string name="";
            public List<Node> nbrs = new();
            public bool small => name == name.ToLower();
            public int visitCount = 0;
        }
        public override object Run(bool part2)
        {
            Dictionary<string,Node> dir = new Dictionary<string,Node>();
            foreach (var line in ReadLines())
            {
                var i = line.IndexOf('-');
                var src = line.Substring(0, i);
                var  dst = line.Substring(i + 1);

                var n1 = Get(src);
                var n2 = Get(dst);

                if (!n1.nbrs.Contains(n2))
                    n1.nbrs.Add(n2);
                if (!n2.nbrs.Contains(n1))
                    n2.nbrs.Add(n1);
            }

            var s = dir["start"];
            var e = dir["end"];
            s.visitCount = 2; // start here // mark as twice visited to prevent any more 

            // count paths
            long count = 0;
            bool doubleVisited = false;
            CountPaths(s, new List<string>(), ref doubleVisited);


            return count;

            Node Get(string name)
            {
                if (!dir.ContainsKey(name))
                {
                    var n = new Node {name = name};
                    dir.Add(name,n);
                }
                return dir[name];
            }


            void CountPaths(Node cur, List<string> path, ref bool doubleVisited)
            {
                path.Add(cur.name);
                if (cur == e)
                    ++count;
                else
                {

                    foreach (var n in cur.nbrs)
                    {
                        var dbl = doubleVisited;
                        if (n.visitCount != 0 && n.small)
                        {
                            // check restrictions
                            if (!part2)
                                continue;
                            if (n.visitCount > 1 || doubleVisited)
                                continue;
                            doubleVisited = true;
                        }

                        var v = n.visitCount;
                        //Console.WriteLine("Down "+n.name);
                        n.visitCount++;
                        CountPaths(n,path, ref doubleVisited);
                        //Console.WriteLine("Up " + n.name);
                        n.visitCount = v;
                        doubleVisited = dbl;
                    }
                }
                path.RemoveAt(path.Count-1);
            }

        }
    }
}