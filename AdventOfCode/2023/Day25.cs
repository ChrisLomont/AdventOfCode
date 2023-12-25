
using System.Runtime.InteropServices.JavaScript;
using System.Xml.Linq;

namespace Lomont.AdventOfCode._2023
{
    internal class Day25 : AdventOfCode
    {
        // used mathematica - use min cut
        // added C# later

        /*
2023 Day 25 part 1: 606062 in 4348525 us
2023 Day 25 part 2: 0 in 2857.3 us         
         */

        public override object Run(bool part2)
        {
            if (part2)
                return 0; // no part 2 on day 25

            var edges = new List<(string, string)>();
            var verts = new HashSet<string>();

            foreach (var line in ReadLines())
            {
                var w = line.Split(new[]{' ',':'},StringSplitOptions.RemoveEmptyEntries);

                foreach (var v in w)
                    verts.Add(v);

                edges.AddRange(w.Skip(1).Select(v2 => (w[0],v2)));
            }

            // Krager's algorithm, random contract till we get the cuts we want
            // https://en.wikipedia.org/wiki/Karger%27s_algorithm
            int pass = 0;
            var rand = new Random(12345); // makes repeatable

            while (true)
            {
                ++pass;
                //if ((pass%50)==0)
                //    Console.WriteLine($"Pass {pass}");
                var (vertSets, edgeCuts) = SmooshGraph(verts, edges, rand);
                if (edgeCuts.Count == 3)
                {
                  //  Console.WriteLine($"Passes {pass}");
                    return vertSets[0].Count * vertSets[1].Count;
                }
            }
        }


        // randomly contract edges to split graph in 2
        // return vertex sets and edges cut between those two sets
        (
            List<HashSet<string>> vertSets,
            List<(string v1, string v2)> edgeCuts
            ) 
            
            SmooshGraph(
                HashSet<string> originalVerts, 
                List<(string v1, string v2)> originalEdges, 
                Random rand
            )
        {

            // make copies so we don't lose originals
            var verts = new HashSet<string>(originalVerts);
            var edges = originalEdges.Select(e => (v1:e.v1, v2:e.v2)).ToList();

            // each merged vertex set starts with only itself
            var mergedVerts = verts.ToDictionary(node => node, node => new HashSet<string> { node });

            while (verts.Count > 2)
            {
                var cutEdge = edges[rand.Next(edges.Count)];
                var mergeNode = cutEdge.v1; // collapse v1 into v2

                mergedVerts[cutEdge.v2].UnionWith(mergedVerts[mergeNode]);
                mergedVerts.Remove(mergeNode);

                verts.Remove(mergeNode); // one less to process

                for (var i = edges.Count - 1; i >= 0; --i)
                {
                    var e = edges[i];
                    var (v1, v2) = e;
                    if (v1 != mergeNode && v2 != mergeNode)
                        continue;

                    var other = v2 == mergeNode ? v1 : v2;
                    if (other != cutEdge.v2)
                        edges.Add((cutEdge.v2, other));
                    edges.RemoveAt(i);
                }
            }

            var vertSets = mergedVerts.Values.ToList();
            var cutEdges = new List<(string v1, string v2)>();

            var edges1 = vertSets[0];
            var edges2 = vertSets[1];

            foreach (var (v1,v2) in originalEdges)
            {
                var cut1 = edges1.Contains(v1) && edges2.Contains(v2);
                var cut2 = edges1.Contains(v2) && edges2.Contains(v1);

                if (cut1 || cut2)
                    cutEdges.Add((v1,v2));
            }

            return (vertSets, cutEdges);
        }

#if false

        object Run2()
        {
            long answer = 0;
            // var (w,h,g) = DigitGrid();
            // var (w,h,g) = CharGrid();

            foreach (var line in ReadLines())
            {
                var nums = Numbers64(line);
            }

            return answer;
        }

        class Graph
        {
            public Dictionary<string, Node> nodes = new();
            public List<(string v1, string v2)> edges = new List<(string, string)>();

            bool SameEdge((string v1, string v2) e1, (string v1, string v2) e2)
            {
                return (e2.v1 == e1.v1 && e2.v2 == e1.v2) ||
                       (e2.v1 == e1.v2 && e2.v2 == e1.v1);
            }

            public void AddEdge(string v1, string v2)
            {
                if (edges.Any(e => SameEdge(e, (v1, v2))))
                    return;

                edges.Add((v1, v2));

                if (!nodes.ContainsKey(v1))
                    nodes.Add(v1, new Node { name = v1 });
                if (!nodes.ContainsKey(v2))
                    nodes.Add(v2, new Node { name = v2 });

                var n1 = nodes[v1];
                var n2 = nodes[v2];
                if (!n1.neighbors.Contains(n2))
                    n1.neighbors.Add(n2);

                if (!n2.neighbors.Contains(n1))
                    n2.neighbors.Add(n1);
            }

            public Graph Clone()
            {
                var g = new Graph();
                g.edges = new List<(string, string)>();
                foreach (var p in nodes)
                {
                    var n = p.Value.name;
                    g.nodes.Add(n, new Node{name = n});
                }

                foreach (var e in edges)
                    g.AddEdge(e.v1, e.v2);
                return g;
            }

            
            public void RemoveEdge((string v1, string v2) e)
            {
                edges.RemoveAll(e2 => 
                    (e2.v1 == e.v1 && e2.v2 == e.v2) ||
                    (e2.v1 == e.v2 && e2.v2 == e.v1)
                    );
                if (nodes.ContainsKey(e.v1))
                    nodes[e.v1].neighbors.RemoveAll(v => v.name == e.v2);
                if (nodes.ContainsKey(e.v2))
                    nodes[e.v2].neighbors.RemoveAll(v => v.name == e.v1);
            }
        }

        class Node
        {
            public string name;
            public List<Node> neighbors = new();
            public int tag = 0;
        }

        object Run1()
        {
            //Dictionary<string, List<string>> links = new();
            var edges = new List<(string,string)>();
            var verts = new HashSet<string>();
            var g = new Graph();

            foreach (var line in ReadLines())
            {
                var line1 = line.Replace(":", "");
                var w = line1.Split(' ');


                foreach (var v in w)
                    verts.Add(v);

                var v1 = w[0];
                //links.Add(v1,w.Skip(1).ToList());
                foreach (var v2 in w.Skip(1))
                {
                    // order alpha
                    if (String.Compare(v1,v2)<0)
                        edges.Add((v1,v2));
                    else 
                        edges.Add((v2, v1));
                }
            }


            foreach (var v in verts)
            {
                var node = new Node{name=v};
                g.nodes.Add(v,node);
            }

            foreach (var e in edges)
            {
                var (v1, v2) = e;
                g.AddEdge(v1,v2);
            }

            foreach (var p in g.nodes)
            {
                Console.WriteLine($"{p.Key} -> {p.Value.neighbors.Count}");
            }

            var ec = g.edges.Count;
            var vc = g.nodes.Count;
            Console.WriteLine($" edges {ec} verts {vc} e-v+1 {ec-vc+1}");


            // find 3 edges to remove
            long answer = 0;
            var n = g.edges.Count;
            Console.WriteLine($"Edge count {n}");
            for (int i = 0; i < n; ++i)
            {
                for (int j = i+1; j < n; ++j)
                {
                    for (int k = i+2; k < n; ++k)
                    {
                        var score = Prod(i, j, k);
                        if (score > answer)
                        {
                            answer = score;
                            Console.WriteLine($"{g.edges[i]} {g.edges[j]} {g.edges[k]} -> {score}");
                        }
                    }
                }
            }


            return answer;

            long Prod(int i, int j, int k)
            {
                var g2 = g.Clone();


                g2.RemoveEdge(edges[i]);
                g2.RemoveEdge(edges[j]);
                g2.RemoveEdge(edges[k]);

                // count pieces
                HashSet<string> vLeft = new();
                foreach (var v in g2.nodes.Keys)
                    vLeft.Add(v);

                HashSet<string> visited = new();
                Dictionary<string, int> tags = new();
                int tag = 0;

                while (vLeft.Any())
                {
                    var v1 = vLeft.First();
                    Recurse(v1,tag);
                    ++tag;
                }

                if (tag == 2)
                {
                    var c1 = tags.Where(p => p.Value == 0).Count();
                    var c2 = tags.Where(p => p.Value == 1).Count();
                    return c1 * c2;
                }
                else
                {
                    return 0; // failed
                }


                void Recurse(string v1, int tag)
                {
                    vLeft.Remove(v1);
                    visited.Add(v1);
                    tags.Add(v1, tag);
                    foreach (var v2 in g2.nodes[v1].neighbors)
                    {
                        if (!visited.Contains(v2.name))
                            Recurse(v2.name, tag);
                    }

                }


            }

        }

        public override object Run(bool part2)
        {
            return part2 ? Run2() : Run1();
        }
#endif

    }
}