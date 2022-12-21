namespace Lomont.AdventOfCode._2019
{
    internal class Day14 : AdventOfCode
    {
    //2019 Day 14 part 1: 870051 in 6905.5 us
    //2019 Day 14 part 2: 1863741 in 6050.3 us
        class Node
        {
            public long need = 0; // how many needed
            public string name;
            public long srcCount; // this many made per recipe
            public List<(Node, long dstCount)> Children = new();
            public List<Node> Parents = new();
        }

        public override object Run(bool part2)
        {
            checked
            {
                // make graph:
                Dictionary<string, Node> nodes = new();

                var ore = new Node {name = "ORE", srcCount = 1};
                nodes.Add(ore.name, ore);

                for (var pass = 0; pass < 2; ++pass)
                {
                    foreach (var line in ReadLines())
                    {
                        var w = line.Split("=>", StringSplitOptions.RemoveEmptyEntries);
                        var ing = w[0].Split(",", StringSplitOptions.RemoveEmptyEntries);

                        Node node;
                        var (item, itemCount) = It(w[1]);
                        if (pass == 0)
                        {
                            node = new Node {name = item, srcCount = itemCount};
                            nodes.Add(item, node);
                        }
                        else
                            node = nodes[item];


                        if (pass == 1)
                        {
                            var parent = node;
                            foreach (var p in ing)
                            {
                                var (ingName, ingCount) = It(p);
                                var child = nodes[ingName];
                                parent.Children.Add((child, ingCount));
                                child.Parents.Add(parent);
                            }

                        }

                        (string, long) It(string v)
                        {
                            var t = v.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                            return (t[1], long.Parse(t[0]));
                        }
                    }
                }

                long Score(long inputFuel)
                {

                    foreach (var n in nodes)
                        n.Value.need = 0; // reset
                    // now flow nodes forward

                    HashSet<string> closed = new();

                    Queue<string> opened = new();
                    HashSet<string> inQueue = new();

                    opened.Enqueue("FUEL");
                    inQueue.Add("FUEL");

                    nodes["FUEL"].need = inputFuel;

                    while (opened.Any())
                    {
                        var name = opened.Dequeue();
                        var parent = nodes[name];
                        if (parent.Parents.All(p => closed.Contains(p.name)))
                        {
                            // Console.WriteLine($"Closed {name}");
                            closed.Add(name);
                            inQueue.Remove(name);

                            // process
                            foreach (var (child, cost) in parent.Children)
                            {
                                if (inQueue.Contains(child.name))
                                {

                                    //  Console.WriteLine($"In queue {child.name}");
                                }
                                else
                                {
                                    //  Console.WriteLine($"Add {child.name}");
                                    opened.Enqueue(child.name);
                                    inQueue.Add(child.name);
                                }

                                var blockedNeeded = (parent.need + parent.srcCount - 1) / parent.srcCount; // round up
                                child.need += cost * blockedNeeded;
                            }
                        }
                        else
                            opened.Enqueue(name); // not ready yet
                    }

                    return ore.need;
                }

                if (!part2)
                    return Score(1);

                // part2: feed forward 1 trillion ore, how many fuel?
                // binary search
                long lo = 1L, hi = 2L, score = 0;
                long upper = 1_000_000_000_000L;
                do
                {
                    lo = hi;
                    hi *= 2;
                    score = Score(hi);
                } while (score < upper);

                while (hi > lo + 1)
                {
                    var mid = (lo + hi) / 2; // rounds down
                    var midScore = Score(mid);
                    if (midScore < upper)
                        lo = mid;
                    else if (upper < midScore)
                        hi = mid;

                    var hiScore = Score(hi);
                    var loScore = Score(lo);
                    //Console.WriteLine($"{lo}:{loScore} {hi}:{hiScore}");
                }

                // largest index with less than 1 trillion ore, get fuel
                //Trace.Assert(loScore <= upper && upper <= hiScore);
                //Score(lo);
                return lo;
            }
        }
    }
}