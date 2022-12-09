namespace Lomont.AdventOfCode._2021
{
    internal class Day10 : AdventOfCode
    {
        public override object Run(bool part2)
        {
            var cScores = new List<long>();
            // order ([{<
            int[] scores = new int[4];
            var open = "([{<";
            var clos = ")]}>";
            var stack = new Stack<char>();
            foreach (var line in ReadLines())
            {
                stack.Clear();
                var corrupted = false;
                foreach (var c in line)
                {
                    if (open.Contains(c))
                        stack.Push(c); // open
                    else
                    {
                        // check closes:
                        if (stack.Count == 0)
                        {
                            throw new Exception();
                            // todo continue; // incomplete
                        }
                        var p = stack.Pop();
                        var ind = clos.IndexOf(c);
                        if (open[ind] != p)
                        {
                            // corrupted
                            scores[ind]++;
                            corrupted = true;
                            break;
                        }

                    }
                }

                if (part2 && stack.Count > 0 && !corrupted)
                {
                    var comScore = 0L;
                    checked
                    {
                        while (stack.Count > 0)
                        {
                            comScore = 5 * comScore;
                            var c = stack.Pop();
                            comScore += open.IndexOf(c) + 1;
                        }
                    }
                    cScores.Add(comScore);
                }
            }

            if (part2)
            {
                cScores.Sort(); // 562935369 low
                return cScores[cScores.Count / 2];

            }

            return 
                scores[0]*3 +
                scores[1] * 57 +
                scores[2] * 1197+
                scores[3] * 25137
                ;
        }
    }
}