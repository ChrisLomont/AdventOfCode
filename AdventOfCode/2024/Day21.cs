
namespace Lomont.AdventOfCode._2024
{
    internal class Day21 : AdventOfCode
    {
        /*
         2024 Day 21 part 1: 176650 in 5529.3 us
         2024 Day 21 part 2: 217698355426872 in 1482.9 us
         */

        record State(vec2 pos, string presses);

        public override object Run(bool part2)
        {
            var DOWN = new vec2(1, 0);
            var RIGHT = new vec2(0, 1);
            var out1 = new vec2(0, 0);
            var out2 = new vec2(3, 0);
            int maxDepth = part2?25:2;
            Dictionary<string, long> memo = new(); // gets 56 and 493 entries

            return ReadLines().Sum(code => ScoreGoal(code, maxDepth) * Int64.Parse(code.Replace("A", "")));

            // sum minimal moves to given target string
            long ScoreGoal(string target, int depth)
            {
                if (depth == -1) return target.Length;
                var (padText, cur) = depth == maxDepth 
                        ? ("789456123X0A", new vec2(3, 2)) 
                        : ("X^A<v>", new vec2(0, 2));

                long result = 0;

                foreach (var ch in target)
                {
                    var k = padText.IndexOf(ch);
                    var nxt = new vec2(k / 3, k % 3); // 3=padwidth
                    result += Minimal(cur, nxt, depth);
                    cur = nxt;
                }

                return result;
            }
            
            // compute minimal path length from cur to dst at given depth
            long Minimal(vec2 cur, vec2 dst, int depth)
            {
                var key = $"{cur}:{dst}:{depth}";
                if (memo.ContainsKey(key))
                    return memo[key];

                long answer = long.MaxValue;
                Queue<State> q = new();
                q.Enqueue(new(cur, ""));
                while (q.Any())
                {
                    var (pos,presses) = q.Dequeue();
                    if (pos == dst)
                    {   // got to desired location, press it! keep best
                        answer = Math.Min(answer, ScoreGoal(presses + "A", depth - 1));
                        continue;
                    }

                    // out of bounds
                    if ((depth== maxDepth && pos == out2)|| (depth != maxDepth && pos == out1))
                        continue;

                    // move closer
                    if (pos.x < dst.x) Add(DOWN, "v");
                    else if (pos.x > dst.x) Add(-DOWN, "^");

                    if (pos.y < dst.y) Add(RIGHT, ">");
                    else if (pos.y > dst.y) Add(-RIGHT, "<");

                    void Add(vec2 dir, string nxt) => q.Enqueue(new(pos + dir, presses + nxt));
                }

                memo[key] = answer;

                return answer;
            }
        }
        


    }
}