

//  5   00:19:42    2673      0   00:22:17    2334      0
// Ans: CNSZFDVLJ
// Ans: QNDWLMGNS

namespace Lomont.AdventOfCode._2022
{
    internal class Day05 : AdventOfCode
    {
        public override object Run(bool part2)
        {
            List<List<char>> stacks = new();
            foreach (var line in ReadLines())
            {
                if (line.Contains("["))
                {
                    var cc = (line.Length + 1) / 4;
                    while (stacks.Count < cc)
                        stacks.Add(new());
                    for (var i = 0; i < cc; ++i)
                    {
                        var c = line[4 * i + 1];
                        if (c != ' ')
                            stacks[i].Insert(0, c);
                    }
                }
                else if (line.Contains("move"))
                {
                    var n = Numbers(line, false);
                    var cnt = n[0];
                    var s= stacks[n[1]-1];
                    var d= stacks[n[2]-1];

                    var cc = s.Count - cnt;
                    d.AddRange(part2 ? s.Skip(cc) : s.Skip(cc).Reverse());
                    s.RemoveRange(cc, cnt);
                }
            }

            return stacks.Select(s => s.Last()).Aggregate("", (a, b) => a + b);
        }
    }
}