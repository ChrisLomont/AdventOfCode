using Lomont.AdventOfCode.Utils;
using System.IO;

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
            return BFSSoln(part2);
            //return FloodFillSoln(part2);
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

            if (part2)
            {
                visited.Add(end);
                q.Enqueue((0, end));
                isDone = v => g[v.x, v.y] == 'a'; // first a hit is it
                isLegal = (nxt, v) => !(g[nxt.x, nxt.y] - g[v.x, v.y] < -1);
            }
            else
            {
                visited.Add(start);
                q.Enqueue((0, start));
                isDone = v => v == end; // first to reach end
                isLegal = (nxt, v) => g[nxt.x, nxt.y] - g[v.x, v.y] <= 1;
            }

            while (q.Any())
            {
                var (d, cur) = q.Dequeue();
                foreach (var dir in dirs)
                {
                    var nxt = dir + cur;
                    if (zero <= nxt && nxt <= bound &&
                        !visited.Contains(nxt) &&
                        isLegal(nxt, cur)
                       )
                    {
                        if (isDone(nxt))
                            return d + 1;
                        visited.Add(nxt);
                        q.Enqueue((d + 1, nxt));
                    }

                }
            }

            throw new Exception();
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

            Console.WriteLine($"Total visits {totalVisits}, {(double) totalVisits / (w * h):F1}x increase");

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
                Console.WriteLine("Visited " + visits); // got 3152, much overkill

                var solution1 = Solution(start);
                var solution2 = Solution(bestStart);
                Console.WriteLine("Shortest path and best start path");
                Draw(g, start, end, solution1, solution2);
                var solution3 = AStarPath(w, h, g, start, end);
                Console.WriteLine($"Shortest floodfill path {solution1.Count} and shortest A* path {solution3.Count}");
                Draw(g, start, end, solution1, solution3);

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


        List<vec3> AStarPath(int w, int h, char[,] g, vec3 start, vec3 end)
        {
            // this method fails to find shortest somehow...


            int Cost(vec3 v)
            {
                var (dx, dy, _) = v - end;
                //return Math.Max(Math.Abs(dx),Math.Abs(dy));
                // A heuristic is admissible if it never overestimates the distance between two nodes
                var d = Math.Abs(dx) + Math.Abs(dy) - 1; // manhattan
                return Math.Max(d, 0); // manhattan
            }

            var path = AStar(start, end,
                Cost,
                (a, b) => 1, // neighbor cost
                Neighbors // how to get neighbors
            );

            foreach (var p in path)
                Console.Write(g[p.x, p.y]);
            Console.WriteLine();
            Console.WriteLine($"{start} {path[0]} {end} {path.Last()}");

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
                var openSet = new Bob<T>();
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
                    if (current.Equals(goal)) // // current = the node in openSet having the lowest fScore[] value
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
        class Bob<T>
        {
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

        }

    }
}