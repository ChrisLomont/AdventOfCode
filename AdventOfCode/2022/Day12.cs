using System.Collections;


namespace Lomont.AdventOfCode._2022
{
    internal class Day12 : AdventOfCode
    {
        // 2:30 am, 7698 done, I finished then

        //12   02:31:31    8312      0   02:31:38    7678      0
        //11   00:27:50    1417      0   00:33:35     800      0

        // 481
        // 480

        //2022 Day 12 part 1: 481 in 2801.4 us
        //2022 Day 12 part 2: 480 in 1978.1 us

        // todo - move AStar, BFS into tools, make generic
        // todo AStar still wrong answer - see Draw stuff to visualize. Off by 4 at end

        public override object Run(bool part2)
        {
            //var p1 = AStarPath();
            //return p1.Count == 481;
            return BFSSoln(part2);
            //return FloodFillSoln(part2); // draws pretty stuff
        }

        long BFSSoln(bool part2)
        {
            var (w, h, g) = CharGrid(); // width, height, char grid
            vec3 start = new(), end = new();
            var dirs = new[] {new vec3(-1, 0), new vec3(0, -1), new vec3(1, 0), new vec3(0, 1)};
            vec3 zero = new(), bound = new(w - 1, h - 1);

            // init all grid things, replace S,E with a,z
            for (var i = 0; i < w; ++i)
            for (var j = 0; j < h; ++j)
            {
                if (g[i, j] == 'S')
                {
                    start = new vec3(i, j);
                    g[i, j] = 'a';
                }

                if (g[i, j] == 'E')
                {
                    end = new vec3(i, j);
                    g[i, j] = 'z';
                }
            }


            var visited = new HashSet<vec3>();
            var q = new Queue<(int, vec3)>();

            Func<vec3, bool> isDone;
            Func<vec3, vec3, bool> isLegal;

            Func<vec3,bool> isBdd = v => zero <= v && v <= bound;

            if (part2)
            {
                visited.Add(end);
                q.Enqueue((0, end));
                isDone = v => g[v.x, v.y] == 'a'; // first a hit is it
                isLegal = (nxt, v) => isBdd(nxt) &&  !(g[nxt.x, nxt.y] - g[v.x, v.y] < -1);
            }
            else
            {
                visited.Add(start);
                q.Enqueue((0, start));
                isDone = v => v == end; // first to reach end
                isLegal = (nxt, v) => isBdd(nxt) && g[nxt.x, nxt.y] - g[v.x, v.y] <= 1;
            }

            Func<vec3, List<vec3>> nbrs = cur =>
            {
                List<vec3> nbrs = new();
                foreach (var dir in dirs)
                {
                    var nxt = dir + cur;
                    if (isLegal(nxt, cur))
                        nbrs.Add(nxt);
                }
                return nbrs;
            };

            var p = Search.BreadthFirstSearch(
                nbrs,
                q.Select(v => v.Item2),
                visited,
                isDone
                );
            
            return p.Count-1; // # of items one more than number of steps to get there
            
        }

        long FloodFillSoln(bool part2)
        {
            var history = new List<vec3>(); // track filling, animate later

            var (w, h, g) = CharGrid(); // width, height, char grid

            var distToEnd = new int[w, h];
            var bestNbr = new vec3[w, h]; // best direction to move from each cell
            var visited = new int[w, h]; // how many times cell visited on search for fun
            vec3 start = new(-1, -1), end = new(-1, -1);
            var dirs = new vec3[] {new vec3(-1, 0), new vec3(0, -1), new vec3(1, 0), new vec3(0, 1)};
            vec3 zero = new vec3(), bound = new vec3(w - 1, h - 1);

            // init all grid things
            for (var i = 0; i < w; ++i)
            for (var j = 0; j < h; ++j)
            {
                distToEnd[i, j] = int.MaxValue;
                //bestNbr[i, j] = new vec3(int.MinValue, int.MinValue);
                if (g[i, j] == 'S')
                    start = new vec3(i, j);
                if (g[i, j] == 'E')
                    end = new vec3(i, j);
            }

            // set height at start and end to make searchable
            g[start.x, start.y] = 'a';
            g[end.x, end.y] = 'z';

            distToEnd[end.x, end.y] = 0; // set initial score to start search
            // compute all cell cost starting from back
            RecurseNeighbors(end);

            if (!part2)
                return distToEnd[start.x, start.y];

            // part 2, find best start location
            var bestStart = start;
            var mostVisits = 0;
            long totalVisits = 0;
            for (var i = 0; i < w; ++i)
            for (var j = 0; j < h; ++j)
            {
                mostVisits = Math.Max(mostVisits, visited[i, j]);
                totalVisits += visited[i, j];
                if (g[i, j] == 'a' && distToEnd[i, j] < distToEnd[bestStart.x, bestStart.y])
                    bestStart = new vec3(i, j);
            }

            //Console.WriteLine($"Total visits {totalVisits}, {(double) totalVisits / (w * h):F1}x increase");

            // save history:
            using (var f = File.CreateText("Prob11_History.txt"))
            {
                f.WriteLine($"{w},{h},");
                foreach (var v in history)
                    f.Write($"{v.x},{v.y},");
            }

            Testing(mostVisits);

            return distToEnd[bestStart.x, bestStart.y];

            void RecurseSelf(vec3 self, vec3 nbr)
            {
                var (selfI, selfJ, _) = self;
                var (nbrI, nbrJ, _) = nbr;
                visited[nbrI, nbrJ]++;
                if (g[selfI, selfJ] + 1 >= g[nbrI, nbrJ]) // can move to nbr?
                {
                    if (distToEnd[selfI, selfJ] > distToEnd[nbrI, nbrJ] + 1)
                    {
                        distToEnd[selfI, selfJ] = distToEnd[nbrI, nbrJ] + 1;
                        bestNbr[selfI, selfJ] = nbr;
                        RecurseNeighbors(self); // better to me - flood out to see if anyone else better
                    }
                }
            }

            void RecurseNeighbors(vec3 self)
            {
                history.Add(self);
                foreach (var v in dirs)
                    if (zero <= self + v && self + v <= bound)
                        RecurseSelf(self + v, self); // reverse self and neighbor
            }


            static void Draw(char[,] g, vec3 start, vec3 end, List<vec3> path, List<vec3>? altPath = null)
            {
                var (fg, bg) = (Console.ForegroundColor, Console.BackgroundColor);
                var (w, h) = Size(g);
                var path2 = altPath != null;
                for (var j = 0; j < h; ++j)
                {
                    for (var i = 0; i < w; ++i)
                    {
                        var c = g[i, j];
                        var p = new vec3(i, j);
                        var p1 = path.Contains(p);
                        var p2 = altPath != null && altPath.Contains(p);
                        if (p1)
                        {
                            if (p == start)
                            {
                                c = 'S';
                                Console.ForegroundColor = ConsoleColor.Green;
                                Console.BackgroundColor = ConsoleColor.White;
                            }
                            else if (p == end)
                            {
                                c = 'E';
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.BackgroundColor = ConsoleColor.White;
                            }
                            else
                            {
                                Console.ForegroundColor = ConsoleColor.White;
                                Console.BackgroundColor = ConsoleColor.Magenta;
                            }
                        }
                        else if (p2)
                        {
                            (Console.ForegroundColor, Console.BackgroundColor) =
                                (ConsoleColor.Cyan, ConsoleColor.Red);
                        }

                        Console.Write(c);
                        Console.ForegroundColor = fg;
                        Console.BackgroundColor = bg;
                    }

                    Console.WriteLine();
                }
            }


            void Testing(int visits)
            {
                //Console.WriteLine("Visited " + visits); // got 3152, much overkill

                var solution1 = Solution(start);
                var solution2 = Solution(bestStart);
                //Console.WriteLine("Shortest path and best start path");
                //Draw(g, start, end, solution1, solution2);
                var solution3 = AStarPath();
                //Console.WriteLine($"Shortest floodfill path {solution1.Count} and shortest A* path {solution3.Count}");
                //Draw(g, start, end, solution1, solution3);

                DetectError(solution1, solution3);

                void DetectError(List<vec3> s1, List<vec3> s2)
                {
                    // when they overlap, should be same dist to end
                    for (var i = 0; i < s1.Count; ++i)
                    {
                        var ind = s2.IndexOf(s1[i]);
                        if (ind >= 0)
                        {
                            var dist1 = s1.Count - i;
                            var dist2 = s2.Count - ind;
                            if (dist1 != dist2)
                            {
                                Console.WriteLine($"{s1[i]} in s1, index {i}, dist {dist1} to end");
                                Console.WriteLine($"{s2[ind]} in s2, index {ind}, dist {dist2} to end");
                            }
                        }
                    }

                }

                List<vec3> Solution(vec3? cur)
                {
                    var path = new List<vec3>();
                    while (cur != null)
                    {
                        path.Add(new vec3(cur.x, cur.y));
                        cur = bestNbr![cur.x, cur.y];
                    }

                    return path;
                }

            }

        }


        List<vec3> AStarPath()
        {
            // this method fails to find shortest somehow...
            var (w, h, g) = CharGrid(); // width, height, char grid
            vec3 start = new(), end = new();

            // init all grid things, replace S,E with a,z
            for (var i = 0; i < w; ++i)
            for (var j = 0; j < h; ++j)
            {
                if (g[i, j] == 'S')
                {
                    start = new vec3(i, j);
                    g[i, j] = 'a';
                }

                if (g[i, j] == 'E')
                {
                    end = new vec3(i, j);
                    g[i, j] = 'z';
                }
            }

            int Cost(vec3 v)
            {
                var (dx, dy, _) = v - end;
                //return Math.Max(Math.Abs(dx),Math.Abs(dy));
                // A heuristic is admissible if it never overestimates the distance between two nodes
                var d = Math.Abs(dx) + Math.Abs(dy) - 1; // manhattan
                return Math.Max(d, 0); // manhattan
            }

            // AStar and AStar2 not working... 
            var path = AStar3(start, end,
                Cost,
                (a, b) => 1, // neighbor cost
                Neighbors // how to get neighbors
            );

            //foreach (var p in path)
            //    Console.Write(g[p.x, p.y]);
            //Console.WriteLine();
            //Console.WriteLine($"{start} {path[0]} {end} {path.Last()}");

            for (var i = 0; i < path.Count - 1; ++i)
            {
                var p1 = path[i];
                var p2 = path[i + 1];
                var c1 = g[p1.x, p1.y];
                var c2 = g[p2.x, p2.y];
                Trace.Assert(c2 - c1 <= 1);
            }


            return path; // not 485, 486

            IEnumerable<vec3> Neighbors(vec3 current)
            {
                var (i, j, _) = current;
                // list legal moves
                var cur = g[i, j];
                var m = new List<vec3>();
                Try(i + 1, j);
                Try(i - 1, j);
                Try(i, j + 1);
                Try(i, j - 1);
                //Console.WriteLine($"nbrs {i},{j} = {m.Count}");
                return m;

                void Try(int x, int y)
                {
                    if (!InBounds(new vec3(x, y)))
                        return;
                    var d = g[x, y];
                    if (d <= cur + 1)
                        m.Add(new vec3(x, y));
                }
            }

            bool InBounds(vec3 v)
            {
                var (x, y, _) = v;
                return 0 <= x && 0 <= y && x < w && y < h;
            }

            static List<T> AStar3<T>
            (
                T start,
                T destination,
                Func<T, int> costEstimate,
                Func<T, T, int> neighborCost,
                Func<T, IEnumerable<T>> neighbors 
                )
            {
                //----------------------
                var closed = new HashSet<T>();
                var queue = new PriorityQueue<Path<T>,int>();
                queue.Enqueue(new Path<T>(start),0);

                while (queue.Count>0)
                {
                    var path = queue.Dequeue();
                    if (closed.Contains(path.LastStep))
                        continue;
                    if (path.LastStep.Equals(destination))
                    {
                        var ans = new List<T>();
                        ans.AddRange(path);
                        ans.Reverse();
                        //Console.WriteLine($"Solution length {ans.Count - 1}");
                        return ans;
                    }

                    closed.Add(path.LastStep);
                    foreach (T n in neighbors(path.LastStep))
                    {
                        var d = neighborCost(path.LastStep, n);
                        if (n!.Equals(destination))
                            d = 0;
                        var newPath = path.AddStep(n, d);
                        queue.Enqueue(newPath, newPath.TotalCost + costEstimate(n));
                    }
                }
                return null;
            }

            static List<T> AStar2<T>(
                T start,
                T goal,
                Func<T, int> costEstimate, // heuristic: estimate cost from n to goal
                Func<T, T, int> neighborCost, // cost from src->dst
                Func<T, IEnumerable<T>> neighbors // get neighbors of given node
            ) where T : IEquatable<T>
            {
                // track best parent to reconstruct path
                Dictionary<T, T> cameFrom = new();

                //let the openList equal empty list of nodes
                var openList = new List<(T node, int score)>();
                //let the closedList equal empty list of nodes
                var closedList = new HashSet<T>();
                // Add the start node
                // put the startNode on the openList (leave it's f at zero)
                openList.Add((start,0));

                // scores:
                Dictionary<T, int> f = new();
                Dictionary<T, int> g = new();
                Dictionary<T, int> h = new();

                int pass = 0;
                // Loop until you find the end
                while (openList.Any())
                {
                    pass++;
                    if ((pass % 100) == 0) 
                        Console.WriteLine($"{pass}: {openList.Count} {closedList.Count}");
                    // Get the current node
                    // let the currentNode equal the node with the least f value
                    var min = openList.Min(p=>p.score);
                    var current = openList.First(p => p.score == min);
                    
                    // remove the currentNode from the openList
                    openList.Remove(current);
                    // add the currentNode to the closedList
                    closedList.Add(current.node);


                    // Found the goal
                    //    if currentNode is the goal
                    //        Congratz! You've found the end! Backtrack to get path
                    if (current.node.Equals(goal))
                    {
                        return ReconstructPath(goal);
                    }
                    // Generate children, loop over them
                    foreach (var child in neighbors(current.node))
                    { // https://medium.com/@nicholas.w.swift/easy-a-star-pathfinding-7e6689c7f7b2
                        // Child is on the closedList
                        if (closedList.Contains(child))
                            continue;
                        // Create the f, g, and h values
                        //        child.g = currentNode.g + distance between child and current
                        //        child.h = distance from child to end
                        //        child.f = child.g + child.h
                        if (!g.ContainsKey(child)) g[child] = 0;
                        if (!h.ContainsKey(child)) h[child] = 0;
                        if (!f.ContainsKey(child)) f[child] = 0;
                        //if (!g.ContainsKey(current.node)) g[current.node] = int.MaxValue/2; // inf
                        g[child] = current.score/* g[current.node] */+ neighborCost(current.node, child);
                        h[child] = costEstimate(child);
                        f[child] = g[child] + h[child];

                        // Child is already in openList
                        var exists = openList.Any(c=>c.node.Equals(child));
                        //        if child.position is in the openList's nodes positions
                        //            if the child.g is higher than the openList node's g
                        //                continue to beginning of for loop
                        if (exists && g[child] > openList.First(c=>c.node.Equals(child)).score)
                            continue;

                        cameFrom[child] = current.node;

                        // Add the child to the openList
                        openList.Add((child, f[child]));
                    }
                }

                    throw new Exception();

                    List<T> ReconstructPath(T current)
                    {
                        var path = new List<T> { current };
                        while (cameFrom.ContainsKey(current))
                        {
                            current = cameFrom[current];
                            path.Add(current);
                        }

                        path.Reverse();
                        return path;
                    }

            }


            static List<T> AStar<T>(
                T start,
                T goal,
                Func<T, int> costEstimate, // heuristic: estimate cost from n to goal
                Func<T, T, int> neighborCost, // cost from src->dst
                Func<T, IEnumerable<T>> neighbors // get neighbors of given node
            ) where T : IEquatable<T>
            {

                // A* finds a path from start to goal.
                // h is the heuristic function. h(n) estimates the cost to reach goal from node n.
                var h = costEstimate;
                var d = neighborCost;

                // The set of discovered nodes that may need to be (re-)expanded.
                // Initially, only the start node is known.
                // This is usually implemented as a min-heap or priority queue rather than a hash-set.
                var openSet = new Priority<T>();
                openSet.Enqueue(start, h(start));

                // For node n, cameFrom[n] is the node immediately preceding it on the cheapest path from start
                // to n currently known.
                Dictionary<T, T> cameFrom = new();

                // For node n, gScore[n] is the cost of the cheapest path from start to n currently known.
                // default to infinity score
                Dictionary<T, int> gScore = new() {{start, 0}}; // cost to start = 0

                // For node n, fScore[n] := gScore[n] + h(n). fScore[n] represents our current best guess as to
                // how cheap a path could be from start to finish if it goes through n.
                var fScore = new Dictionary<T, int> {{start, h(start)}}; // estimate cost to end

                while (openSet.Count > 0)
                {
                    // This operation can occur in O(Log(N)) time if openSet is a min-heap or a priority queue
                    var current = openSet.Dequeue();
                    if (current.Equals(goal)) // current = the node in openSet having the lowest fScore[] value
                    {
                        Console.WriteLine($"Solved! {fScore[goal]}");
                        return ReconstructPath(current);
                    }

                    foreach (var neighbor in neighbors(current))
                    {
                        // d(current,neighbor) is the weight of the edge from current to neighbor
                        // tentative_gScore is the distance from start to the neighbor through current
                        var tentativeGScore = gScore[current] + d(current, neighbor);
                        if (!gScore.ContainsKey(neighbor) || tentativeGScore < gScore[neighbor])
                        {
                            // This path to neighbor is better than any previous one. Record it!
                            cameFrom[neighbor] = current;
                            gScore[neighbor] = tentativeGScore;
                            fScore[neighbor] = tentativeGScore + h(neighbor);
                            //if (neighbor.Equals(goal)) fScore[neighbor] = 0; // Lomont added
                            if (!openSet.Contains(neighbor))
                                openSet.Enqueue(neighbor, h(neighbor));
                        }
                    }
                }

                // Open set is empty but goal was never reached
                throw new Exception("No path found");

                List<T> ReconstructPath(T current)
                {
                    var path = new List<T> {current};
                    while (cameFrom.ContainsKey(current))
                    {
                        current = cameFrom[current];
                        path.Add(current);
                    }

                    path.Reverse();
                    return path;
                }
            }

        }

        class Path<T> :IEnumerable<T>
        {
            // https://gist.github.com/THeK3nger/7734169
            public T LastStep;

            public int TotalCost = 0;

            public Path(T node, Path<T>? prev, int cost)
            {
                LastStep = node;
                this.prev = prev;
                TotalCost = cost;

            }

            Path<T>? prev;

            public Path(T node) : this(node, null, 0)
            {
            }
            public Path<T> AddStep(T step, int stepCost)
            {
                return new Path<T>(step, this, TotalCost + stepCost);
            }

            /// <summary>
            /// Gets the enumerator.
            /// </summary>
            /// <returns>The enumerator.</returns>
            public IEnumerator<T> GetEnumerator()
            {
                for (Path<T> p = this; p != null; p = p.prev)
                    yield return p.LastStep;
            }

            /// <summary>
            /// Gets the enumerator.
            /// </summary>
            /// <returns>The enumerator.</returns>
            IEnumerator IEnumerable.GetEnumerator()
            {
                return this.GetEnumerator();
            }
        }

        class Priority<T>
        {
#if false
            List<(T, int)> openList = new();
#else

            PriorityQueue<T, int> openSet = new ();
            HashSet<T> hashSet = new HashSet<T>(); // track things in openSet

            public void Enqueue(T item, int priority)
            {
                openSet.Enqueue(item, priority);
                hashSet.Add(item);
            }

            public T Dequeue()
            {
                var item = openSet.Dequeue();
                hashSet.Remove(item);
                return item;
            }

            public bool Contains(T item)
            {
                return hashSet.Contains(item);
            }



            public int Count => openSet.Count;
#endif

        }

    }
}