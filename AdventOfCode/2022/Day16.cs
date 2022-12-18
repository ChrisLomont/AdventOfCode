
using System.ComponentModel.DataAnnotations;

namespace Lomont.AdventOfCode._2022
{
    internal class Day16 : AdventOfCode
    {


#if false
        object RunFast(bool part2)
        {
            var lines = ReadLines().Select(l => l.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries));
            var G = lines.ToDictionary(t => t[0], t => t[10..]);
            var F = lines.Where(t => t[5] != "0").ToDictionary(t => t[1], t => Int32.Parse(t[5]));
            var I = F.Keys.Select((f, i) => (f,1UL<<i)).ToDictionary(t=>t.Item1, t=>t.Item2);

            var T = G.Keys.ToDictionary(
                x => x, x => 
                    G.Keys.ToDictionary(
                        y=>y,
                        y => G[x].Contains(y)?1:Int32.MaxValue
                        )
            );

            foreach (var k in T.Keys)
            foreach (var i in T.Keys)
            foreach (var j in T.Keys)
                T[i][j] = Math.Min(T[i][j], T[i][k] + T[k][j]);

            Dictionary<ulong, int>  Visit(string v, int budget,ulong state, int flow, Dictionary<ulong, int> answer)
            {
                answer[state] = Math.Max(answer[state], flow);
                foreach (var u in F.Keys)
                {
                    var newbudget = budget - T[v][u] - 1;
                    if ((I[u] & state)!=0  || newbudget <= 0) continue;
                    Visit(u, newbudget, state | I[u], flow + newbudget * F[u], answer);
                }
                return answer;
            }

            //if (!part2)
                return Visit("AA",30,0,0,new()).Max(v=>v.Value);

#if false
import sys, re
lines = [re.split('[\\s=;,]+', x) for x in sys.stdin.read().splitlines()]
G = {x[1]: set(x[10:]) for x in lines}
F = {x[1]: int(x[5]) for x in lines if int(x[5]) != 0}
I = {x: 1<<i for i, x in enumerate(F)}
T = {x: {y: 1 if y in G[x] else float('+inf') for y in G} for x in G}
for k in T:
    for i in T:
        for j in T:
            T[i][j] = min(T[i][j], T[i][k]+T[k][j])

def visit(v, budget, state, flow, answer):
    answer[state] = max(answer.get(state, 0), flow)
    for u in F:
        newbudget = budget - T[v][u] - 1
        if I[u] & state or newbudget <= 0: continue
        visit(u, newbudget, state | I[u], flow + newbudget * F[u], answer)
    return answer    

total1 = max(visit('AA', 30, 0, 0, {}).values())
visited2 = visit('AA', 26, 0, 0, {})
total2 = max(v1+v2 for k1, v1 in visited2.items() 
                   for k2, v2 in visited2.items() if not k1 & k2)
print(total1, total2)
#endif

        }
#endif
        //2022 Day 16 part 1: 2087 in 2922831.9 us
        //2022 Day 16 part 2: 2591 in 1007893415.1 us
#if true
        public override object Run(bool part2)
        {
         //   return RunFast(part2);
            List<string> names = new();
            Dictionary<string, List<string>> nbrs = new();
            Dictionary<string, int> flow = new();
            HashSet<string> opened = new();

            var lines = ReadLines();
            for (var index = 0; index < lines.Count; ++index)
            {
                var line = lines[index];
                var n = Numbers(line);

                var m = Regex.Match(line,
                    @"^Valve (?<name>[A-Z]{2}) has flow rate=(?<rate>\d+); tunnel(s)? lead(s)? to valve(s)? (?<valves>[A-Z, ]+)$");
                var name = m.Groups["name"].Value;
                var rate = Int32.Parse(m.Groups["rate"].Value);
                var valves = m.Groups["valves"].Value.Replace(" ", "")
                    .Split(',', StringSplitOptions.RemoveEmptyEntries);

                nbrs.Add(name, valves.ToList());
                flow.Add(name, rate);
                names.Add(name);

                //Console.WriteLine($"{name} has rate {rate} and nbrs {valves.Aggregate("",(a,b)=>a+","+b)}");

            }

            // final graph has lots of 0 rates, and the number of nodes makes the search huge, so crush them into all paths from nonzero rate items
            var start = "AA"; // must keep this in new graph
            Dictionary<string, List<(string nbr, int dist)>> nonzeroNbrs = new();
            foreach (var name in names)
            {
                if (flow[name] == 0 && name != start) continue;
                NonzeroDFS(name);
            }

            //Console.WriteLine($"{nonzeroNbrs.Count} nonzero nodes from {names.Count} total nodes");

            // memoize things
            Dictionary<long, int> memo = new();
            Dictionary<string, int> pathMemo = new();

            var runtime = part2 ? 26 : 30;

            // idea - get all paths where you open a valve each per step
            var paths = GenPaths("AA", runtime, new()).ToList();
            // GenPaths 85789 for run time 26
            // GenPaths 430864 for run time 30
            //Console.WriteLine($"GenPaths {paths.Count} for run time {runtime}");
            // sanitycheck:
            //foreach (var path in paths)
            //foreach (var c in path)
            //{
            //    Trace.Assert(path.Count(n => n == c) == 1);
            //}

            return -1;


            if (!part2)
            {
                // now recurse on nonzero graph
                //  return MaxFlow1("AA", new HashSet<string>(), runtime); // 2087


                //  test other method to check funcs work
                var paths1 = GenPaths("AA", runtime, new()).ToList();


                //Console.WriteLine($"{paths1.Count} paths");
                var max = paths1.Select(p => PathScore(p, 30)).Max();
                return max; // // 1651 example , 2087 me
            }

            // this fails
            //return MaxFlow2("AA", "AA", new HashSet<string>(), 26);


            // todo - memoize


            var best2 = 0;
            var tried = 0;
            for (var i = 0; i < paths.Count; ++i)
            {
                if ((i % 250) == 0) Console.WriteLine($"{i}/{paths.Count}");
                var path1 = paths[i];
                var val1 = PathScore(path1, 26);
                var avoid = new HashSet<string>();
                foreach (var p in path1)
                    avoid.Add(p);
                foreach (var path2 in GenPaths("AA", 26, avoid))
                {
                    ++tried;
                    var val2 = PathScore(path2, 26);
                    best2 = Math.Max(best2, val1 + val2);
                }
            }

            //Console.WriteLine($"Checked {tried}, best {best2}");
            return best2;

            int Dist(string src, string dst)
            {
                return nonzeroNbrs[src].First(c => c.nbr == dst).dist;

            }


            // value from a path
            int PathScore(List<string> path, int timeLeft)
            {
                var key = path.Aggregate("", (a, b) => a + b);
                if (!pathMemo.ContainsKey(key))
                {
                    // assumes flow rate AA = 0 ?
                    var total = 0;
                    for (var i = 0; i < path.Count - 1; ++i)
                    {
                        var (src, dst) = (path[i], path[i + 1]);
                        timeLeft -= Dist(src, dst) + 1; // remove cost to open and move
                        total += timeLeft * flow[dst];
                    }

                    Trace.Assert(timeLeft >= 0);
                    pathMemo.Add(key, total);
                }
                return pathMemo[key];
            }


            // all paths from 
            IEnumerable<List<string>> GenPaths(string node, int timeLeft, HashSet<string> visited)
            {
                if (timeLeft >= 1)
                    yield return new List<string> {node}; // length 0 path
                foreach (var (nbr, depth) in nonzeroNbrs[node])
                {
                    if (visited.Contains(nbr))
                        continue;
                    if (timeLeft >= depth + 2) // else not enough time to gain anything
                    {
                        visited.Add(node);
                        foreach (var p in GenPaths(nbr, timeLeft - depth - 1, visited))
                        {
                            var pref = new List<string> {node};
                            pref.AddRange(p);
                            yield return pref;
                        }
                        visited.Remove(node);
                    }
                }
            }


            // name hash 0-899
            static long NameHash(string name)
            {
                Trace.Assert(name.Length == 2);
                var d0 = name[0] - 'A';
                var d1 = name[1] - 'A';
                Trace.Assert(0 <= d0 && 0 <= d1 && d1 < 30 && d0 < 30);
                return d1 * 30 + d0;
            }

            // open hash 0 - (1<<# names)-1
            static long OpenedHash(HashSet<string> opened, IEnumerable<string> names)
            {
                checked
                {
                    long v = 0;
                    foreach (var n in names)
                        v = v * 2 + ((opened.Contains(n)) ? 1 : 0);
                    return v;
                }
            }
#if false
hard to do this way - cannot figure out time....
            int MaxFlow2(string node1, string node2, HashSet<string> opened, int timeLeft)
            {
                checked
                {
                    var key = (long)timeLeft;
                    key = 1000 * key + NameHash(node1);
                    key = 1000 * key + NameHash(node2);
                    key = key * (1<<(nbrs.Count+1)) + OpenedHash(opened,nonzeroNbrs.Keys);
                    var best = 0;

                    if (!memo.ContainsKey(key))
                    {
                        if (timeLeft > 0)
                        {

                            // can skip if prev node pair sent one to went to an already opened node. No need to come here, adds nothing
                            if (!opened.Contains(node1) && !opened.Contains(node2)) 
                            {
                                // opening each adds this much to overall end flow
                                var val1 = flow[node1] * (timeLeft - 1);
                                var val2 = flow[node2] * (timeLeft - 1);

                                foreach (var (nbr2, dist2) in nonzeroNbrs[node2])
                                foreach (var (nbr1, dist1) in nonzeroNbrs[node1])
                                {   
                                    // open and go to neighbor combos (check same node?)

                                    if (node1 != node2 && val1 != 0)
                                    {
                                        // open both independently case
                                        opened.Add(node1);
                                        opened.Add(node2);
                                        best =
 Math.Max(best, val1+val2 + MaxFlow1(nbr1, nbr2, opened, timeLeft - 1 - dist));
                                        opened.Remove(node2);
                                        opened.Remove(node1);
                                    }

                                        if (val != 0)
                                    {
                                        opened.Add(node);
                                        best = Math.Max(best, val + MaxFlow1(nbr, opened, timeLeft - 1 - dist));
                                        opened.Remove(node);
                                    }
                                    // go to neighbor
                                    best = Math.Max(best, MaxFlow1(nbr, opened, timeLeft - dist));
                                }
                            }
                        }

                        memo.Add(key, best);
                    }
                    return memo[key];
                }
            }
#endif

            // max flow added over given state
            int MaxFlow1(string node, HashSet<string> opened, int timeLeft)
            {
                checked
                {
                    var key = (long) timeLeft;
                    key = 1000 * key + NameHash(node);
                    key = key * (1 << (nbrs.Count + 1)) + OpenedHash(opened, nonzeroNbrs.Keys);
                    var best = 0;

                    if (!memo.ContainsKey(key))
                    {
                        if (timeLeft > 0)
                        {

                            // can skip if prev node went to an already opened node. No need to come here, adds nothing
                            if (!opened.Contains(node))
                            {
                                // opening adds this much to overall end flow
                                var val = flow[node] * (timeLeft - 1);

                                foreach (var (nbr, dist) in nonzeroNbrs[node])
                                {
                                    // open and go to neighbor
                                    if (val != 0)
                                    {
                                        opened.Add(node);
                                        best = Math.Max(best, val + MaxFlow1(nbr, opened, timeLeft - 1 - dist));
                                        opened.Remove(node);
                                    }

                                    // go to neighbor
                                    best = Math.Max(best, MaxFlow1(nbr, opened, timeLeft - dist));
                                }
                            }
                        }

                        memo.Add(key, best);
                    }

                    return memo[key];
                }
            }



            // depth first fill
            // track depths to all nonzero flow
            void NonzeroDFS(string sourceName)
            {
                var frontier = new Queue<(string name, int depth)>();
                var closed = new HashSet<string>();
                nonzeroNbrs.Add(sourceName, new());


                frontier.Enqueue((sourceName, 0));
                while (frontier.Any())
                {
                    var (name, depth) = frontier.Dequeue();
                    depth++; // all nbrs have this
                    closed.Add(name);
                    foreach (var nbr in nbrs[name])
                    {
                        if (closed.Contains(nbr)) continue; // already done
                        frontier.Enqueue((nbr, depth));
                        if (flow[nbr] > 0 || nbr == start)
                        {
                            // track
                            nonzeroNbrs[sourceName].Add((nbr, depth));
                            //Console.WriteLine($"Depth from {sourceName} to {nbr} is {depth}");
                        }
                    }
                }
            }

            return -123;
        }

#else
// old obsolete methods
        class Node
        {
            public string name;
            public int rate;
            public List<Node> nbrs = new();
            public List<string> nbrs1 = new();
            public bool open = false;
            public int visits = 0;
        }

#if false
print(maxflow("AA", (), 30))

@functools.lru_cache(maxsize = None)
def maxflow(cur, opened, min_left):
    if min_left <= 0:
        return 0
    best = 0
    if cur not in opened:
        val = (min_left - 1) * f[cur]
        cur_opened = tuple(sorted(opened + (cur,)))
        for adj in g[cur]:
            if val != 0:
                best = max(best,
                    val + maxflow(adj, cur_opened, min_left - 2))
            best = max(best,
                maxflow(adj, opened, min_left - 1))
    return best

print(maxflow("AA", (), 30))
#endif
    
        public override object Run(bool part2)
        {
            List<Node> nodes = new();
            long score = 0;
            var lines = ReadLines();
            for (var index = 0; index < lines.Count; ++index)
            {
                var line = lines[index];
                var n = Numbers(line);

                var m = Regex.Match(line, @"^Valve (?<name>[A-Z]{2}) has flow rate=(?<rate>\d+); tunnel(s)? lead(s)? to valve(s)? (?<valves>[A-Z, ]+)$");
                var name = m.Groups["name"].Value;
                var rate = Int32.Parse(m.Groups["rate"].Value);
                var valves = m.Groups["valves"].Value.Replace(" ","").Split(',', StringSplitOptions.RemoveEmptyEntries);


                var node = new Node { name = name, rate = rate, nbrs1 = valves.ToList() };
                nodes.Add(node);
            }

            foreach (var n in nodes)
            foreach (var nbr in n.nbrs1)
            {
                n.nbrs.Add(nodes.Find(q=>q.name == nbr));
            }

            Console.WriteLine("----");
            foreach (var n in nodes)
            {
                Console.Write("Tok: "+n.name+": ");
                Dump(n.nbrs.Select(n=>n.name),singleLine:true);
            }

            // matehamtica
            Console.WriteLine("Mma");
            foreach (var n in nodes)
            {
                foreach (var q in n.nbrs)
                    Console.Write($"\"{n.name}\"<->\"{q.name}\",");
            }
            Console.WriteLine();


            var start = nodes.Find(n => n.name == "AA");
            Dictionary<long, long> memo2 = new();

            Console.WriteLine("Max flow: " + MaxFlow(start,30));
            return -1234;

            HashSet<(long,long)> memo = new();

            var max = 0L;

            int hits = 0;

            if (part2)
                Recurse2(start,start, 26, 0,0);
            //Recurse1(start, 30, 0, 0);
            
            

            // what more can be released from pos?
            void Recurse2(Node? loc1, Node? loc2, int time, long pressure, long total)
            {
                checked
                {
                    if (time < 0)
                    {
                        return;
                    }
                    if (total > max)
                    {
                        max = total;
                        Console.WriteLine($"Time {time} total {total} memo {memo.Count}");
                    }

                    var key1 = 0L;
                    foreach (var n in nodes)
                    {
                        key1 = 2 * key1 + (n.open ? 1 : 0);
                    }

                    //Console.WriteLine(key);
                    var key2 = 0L;
                    key2 = time;
                    key2 = 15000 * key2 + total;
                    Trace.Assert(15000 > total);
                    var name1 = loc1.name;
                    var name2 = loc2.name;
                    var hn1 = (name1[0] - 'A') * 30 + (name1[1] - 'A');
                    var hn2 = (name2[0] - 'A') * 30 + (name2[1] - 'A');
                    key2 = 900 * key2 + hn1;
                    key2 = 900 * key2 + hn2;
                    Trace.Assert(900 > hn1);
                    Trace.Assert(900 > hn2);
                    var key = (key1, key2);


                    //key = 1000 * key + pressure;
                    //Trace.Assert(100 > pressure);


                    if (!memo.Contains(key))
                    {
                        loc1.visits++;
                        loc2.visits++;
                        total += pressure; // always

                        var ok1 = !loc1.open && loc1.rate > 0;
                        var ok2 = !loc2.open && loc2.rate > 0;

                        if (ok1 && ok2 && loc1.name != loc2.name)
                        {
                            loc1.open = true;
                            loc2.open = true;
                            Recurse2(loc1, loc2, time - 1, pressure + loc1.rate+loc2.rate, total);
                            loc2.open = false;
                            loc1.open = false;
                        }
                        if (ok1 && !ok2)
                        {
                            loc1.open = true;
                            foreach (var n in loc2.nbrs.OrderBy(z => z.visits))
                                Recurse2(loc1, n, time - 1, pressure + loc1.rate, total);
                            loc1.open = false;
                        }
                        if (!ok1 && ok2)
                        {
                            loc2.open = true;
                            foreach (var n in loc1.nbrs.OrderBy(z => z.visits))
                                Recurse2(n, loc2, time - 1, pressure + loc2.rate, total);
                            loc2.open = false;
                        }

                        foreach (var n1 in loc1.nbrs.OrderBy(z => z.visits))
                        foreach (var n2 in loc2.nbrs.OrderBy(z => z.visits))
                        {
                            // Console.WriteLine($"Time {30 - time} go to {n.name}");
                            Recurse2(n1, n2, time - 1, pressure, total);
                            // Console.WriteLine($"Time {30 - time} back from {n.name}");
                        }

                        loc1.visits--;
                        loc2.visits--;

                        memo.Add(key);
                    }
                    // else 
                    //    ++hits;
                }
            }


            // what more can be released from pos?
            void Recurse1(Node? loc, int time, long pressure, long total)
            {
                checked
                {
                    if (time < 0)
                    {
                        return;
                    }
                    if (total > max)
                    {
                        max = total;
                        Console.WriteLine(total);
                    }

                    var key1 = 0L;
                    foreach (var n in nodes)
                    {
                        key1 = 2 * key1 + (n.open ? 1 : 0);
                    }

                    //Console.WriteLine(key);
                    var key2 = 0L;
                    key2=  time;
                    key2 = 10000 *key2+ total;
                    Trace.Assert(10000>total);
                    var name = loc.name;
                    var hn = (name[0] - 'A') * 30 + (name[1] - 'A');
                    key2 = 900 * key2 + hn;
                    Trace.Assert(900>hn);
                    var key = (key1,key2);


                    //key = 1000 * key + pressure;
                    //Trace.Assert(100 > pressure);


                    if (!memo.Contains(key))
                    {
                        loc.visits++;
                        total += pressure; // always

                        if (!loc.open && loc.rate > 0)
                        {
                            loc.open = true;
                          //  Console.WriteLine($"Time {30-time} open {loc.name}");
                            Recurse1(loc, time - 1, pressure + loc.rate, total);
                           // Console.WriteLine($"Time {30-time} close {loc.name}");
                            loc.open = false;
                        }

                        foreach (var n in loc.nbrs.OrderBy(z=>z.visits))
                        {
                           // Console.WriteLine($"Time {30 - time} go to {n.name}");
                            Recurse1(n, time - 1, pressure, total);
                           // Console.WriteLine($"Time {30 - time} back from {n.name}");
                        }

                        loc.visits--;

                       memo.Add(key);
                    }
                   // else 
                    //    ++hits;
                }
            }

            Console.WriteLine($"Hits: {hits}, memo {memo.Count}" );

            // todo - memoize:
            long MaxFlow(Node cur, int min_left)
            {
                var key1 = 0L;
                foreach (var n in nodes)
                {
                    key1 = 2 * key1 + (n.open ? 1 : 0);
                }

                key1 = 30 * key1 + min_left;
                var name = cur.name;
                var hn = (name[0] - 'A') * 30 + (name[1] - 'A');
                key1 = key1 * 900 + hn;
                if (!memo2.ContainsKey(key1))
                {

                    var best = 0L;

                    if (min_left > 0)
                    {


                        if (!cur.open)
                        {
                            var val = (min_left - 1) * cur.rate;

                            foreach (var adj in cur.nbrs)
                            {
                                if (val != 0)
                                {
                                    cur.open = true;
                                    best = Int64.Max(best, MaxFlow(adj, min_left - 2));
                                    cur.open = false;
                                }

                                best = Int64.Max(best, MaxFlow(adj, min_left - 1));
                            }
                        }
                    }
                    memo2[key1] = best;
                }
                return memo2[key1];
            }


            // 2087
            //part 2 1942 low
            return max;
        }
#endif
        }
    }