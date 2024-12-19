namespace Lomont.AdventOfCode.Utils
{
    internal class Search
    {


        // see 12,2022
        // 

        /// <summary>
        /// Breadth first search (BFS) over a type, given some functors
        /// NOTE: path length is 1 longer than number of steps! includes both ends!
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public static List<T> BreadthFirstSearch<T>(
            Func<T, IEnumerable<T>> getNeighbors,
            IEnumerable<T> itemsToEnqueue,
            IEnumerable<T>? itemsToMarkAsVisited = null,
            Func<T,bool>? visit = null // return true to return path here
            )  where T:notnull

        {
            var prev = new Dictionary<T, T>(); // tracks path
            var visited = new HashSet<T>();
            if (itemsToMarkAsVisited != null)
            {
                foreach (var item in itemsToMarkAsVisited )
                    visited.Add(item);
            }

            var q = new Queue<(int depth, T item)>();
            foreach (var item in itemsToEnqueue)
                q.Enqueue((0,item));

            while (q.Any())
            {
                var (d, cur) = q.Dequeue();
                foreach (var nxt in getNeighbors(cur))
                {
                    if (!visited.Contains(nxt))
                    {
                        prev.Add(nxt, cur); // how we got here
                        if (visit != null && visit(nxt))
                        {
                            List<T> path = new() { nxt };
                            cur = nxt;
                            while (true)
                            {
                                if (!prev.ContainsKey(cur))
                                {
                                    path.Reverse();
                                    return path;
                                }
                                var p = prev[cur];
                                path.Add(p);
                                cur = p;
                            }
                        }
                        visited.Add(nxt);
                        q.Enqueue((d + 1, nxt));
                    }
                }
            }

            return new();
        }

    }
}
