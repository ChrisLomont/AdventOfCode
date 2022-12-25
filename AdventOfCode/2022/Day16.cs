namespace Lomont.AdventOfCode._2022
{

    internal class Day16 : AdventOfCode
    {

        // original... rewrote with online ideas...
        //2022 Day 16 part 1: 2087 in 2922831.9 us
        //2022 Day 16 part 2: 2591 in 1007893415.1 us

        
        //2022 Day 16 part 1: 2087 in 669601.4 us
        //2022 Day 16 part 2: 2591 in 7515651.7 us


        public override object Run(bool part2)
        {

            // parse input into each valve, associated graph node, flow, and tunnels from there
            // turn that into a dict to lookup all that
            var valves = ReadLines()
                .Select(line=>line.Split(new []{' ',',',';'}, StringSplitOptions.RemoveEmptyEntries))
                .Select(words => (
                    Name: words[1],
                    Flow: Numbers(words[4])[0],
                    Tunnels: words[9..]))
                .ToDictionary(v=> v.Name, v=> v);

            Dictionary<(string src, string dst), int> pathLengths = new();
            var names = valves.Select(v=>v.Key).ToList();

            // all dists
            for (var i = 0; i < names.Count; ++i)
            for (var j = i + 1; j < names.Count; ++j)
            {
                // compute cost src to dest, symmetric, fill in both
                var src = names[i];
                var dst = names[j];

                var open = new Queue<(string name, int dist)>();
                var open2 = new HashSet<string>();
                open.Enqueue((src, 0));
                open2.Add(src);
                var closed = new HashSet<string>();
                var dist = -1;
                while (open.Any())
                {
                    var (n, d) = open.Dequeue();

                    if (n == dst)
                    {
                        dist = d;
                        break;
                    }

                    open2.Remove(n);
                    closed.Add(n);

                    foreach (var nbr in valves[n].Tunnels)
                    {
                        if (closed.Contains(nbr)) continue;
                        if (open2.Contains(nbr)) continue;
                        open.Enqueue((nbr, d + 1));
                        open2.Add(nbr);
                    }
                }

                Trace.Assert(dist != -1);

                pathLengths.Add((src, dst), dist);
                pathLengths.Add((dst, src), dist);
            }

            var highScore = 0;

            var startNode = "AA";
            var nonzeroFlows = valves.Values.Where(valve => valve.Flow > 0).Select(valve => valve.Name).ToList();

            Search("AA", nonzeroFlows, !part2 ? 30 : 26, 0, !part2);

            void Search(string src, IList<string> dsts, int timeEnd, int total, bool elephant)
            {
                highScore = Math.Max(highScore, total);
                foreach (var dst in dsts)
                {
                    var (newTimeLeft, newTotal) = DoMove(src, dst, timeEnd, total);
                    if (newTimeLeft >= 0)
                        Search(dst, dsts.Where(j => j != dst).ToList(), newTimeLeft, newTotal, elephant);
                    else if (!elephant && total >= highScore / 2)
                        Search(startNode, dsts, 26, total, true);
                }

                (int newTimeLeft, int newTotal) DoMove(string src, string dst, int timeLeft, int total)
                {
                    var newTimeLeft = timeLeft - pathLengths[(valves[src].Name, valves[dst].Name)] - 1;
                    var newTotal = total + newTimeLeft * valves[dst].Flow;
                    return (newTimeLeft, newTotal);
                }
            }

            return highScore;
        }
    }
}
