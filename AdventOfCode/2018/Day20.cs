
namespace Lomont.AdventOfCode._2018
{
    internal class Day20 : AdventOfCode
    {
        // useful algo
        // https://en.wikipedia.org/wiki/Thompson%27s_construction

        /*
         * 2018 Day 20 part 1: 3672 in 30688.6 us
2018 Day 20 part 2: 8586 in 27714.3 us
         */

        public override object Run(bool part2)
        {
            var regText = ReadText();
            Trace.Assert(regText[0] == '^' && regText[^1]=='$');
            regText = regText.Substring(1, regText.Length - 2); // remove ends

            Dictionary<char, vec2> dirs = new()
            {
                ['N']=new vec2(-1,0),
                ['E'] = new vec2(0, 1),
                ['S'] = new vec2(1, 0),
                ['W'] = new vec2(0, -1),
            };

            Dictionary<vec2, int> map = new();
    
            var startPos = new vec2(0, 0);
            map.Add(startPos,0); // can be any value, say 0,0
            Recurse(regText, startPos);

            if (part2) return map.Values.Count(v => v >= 1000);
            return map.Values.Max();


      

            void Recurse(string s, vec2 pos)
            {
                foreach (var c in s)
                { // process prefix of NESW chars
                    if (c == '(') break;
                
                    var nextDepth = map[pos] + 1;
                    pos += dirs[c];

                    if (map.ContainsKey(pos))
                        map[pos] = Math.Min(map[pos], nextDepth);
                    else
                        map[pos] = nextDepth;
                }

                // any optional clauses?
                var start = s.IndexOf('(');
                if (start < 0) return;

                // starting a '(' clause, get all inside
                var end = start + 1; // skip '('
                var depth = 1; // in clauses
                while (depth != 0)
                {
                    if (s[end] == '(')
                        depth++;
                    else if (s[end] == ')')
                        depth--;
                    ++end;
                }

                var clauses = s.Substring(start + 1, end - start - 2);
                var left = s.Substring(end);

                while (true)
                {
                    // pop off a clause
                    end = 0;
                    depth = 0;
                    while (end < (int)clauses.Length)
                    {
                        if (clauses[end] == '(') depth++;
                        else if (clauses[end] == ')') depth--;
                        else if (depth == 0 && clauses[end] == '|') break;
                        ++end;
                    }

                    // parse clause
                    Recurse(clauses.Substring(0, end), pos);

                    if (end == clauses.Length) break;

                    // do any left
                    clauses = clauses.Substring(end + 1);
                }

                // do anything left
                Recurse(left, pos);
            }
        }
    }
}