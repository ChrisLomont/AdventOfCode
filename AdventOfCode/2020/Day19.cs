namespace Lomont.AdventOfCode._2020
{
    internal class Day19 : AdventOfCode
    {
        public override object Run(bool part2)
        {
            // entry is rule index if >=0, else - of ASCII char
            // phrase is sequence of entries like 0 1 4 "a"
            // rule is 'or' of phrases
            //   seq (0+ = number, <0 = ascii)
            return 0;
        }
    }
}
#if false
            var rules = new List<List<List<int>>>();

            int score = 0;
            foreach (var line in ReadLines())
            {
                if (line.Contains(":"))
                {
                    var w = line.Split(' ',StringSplitOptions.RemoveEmptyEntries);
                    var num = Int32.Parse(w[0][..^1]); // rule number
                    var rule = new List<List<int>>();
                    var phrase = new List<int>(); // current phrase to scan
                    rule.Add(phrase);
                    rules.Add(rule);
                    foreach (var t in w.Skip(1))
                    {
                        if (char.IsDigit(t[0]))
                            phrase.Add(Int32.Parse(t));
                        else if (t == "|")
                        {
                            phrase = new List<int>();
                            rule.Add(phrase);
                        } else if ('a' <= t[0] && t[0] <= 'b' && t.Length == 1)
                        {
                            var v = (int)t[0];
                            phrase.Add(-v);
                        }
                        else throw new Exception();
                    }
                }
                else
                {
                    if (IsOk(line))
                        score++;
                }
            }

            return score;

            bool IsOk(string line)
            {
                var index = 0;
                var ans = Recurse(rules[0],0);
                return ans & index == line.Length;

                //todo - memoize?

                // given line and start parse index, and a rule (list of phrases to match, any ok)
                // return each match and chars used
                List<(bool match, int used)> Recurse(List<List<int>> rule, int startIndex)
                {
                    var ans = new List<(bool Match, int used)>();
                    foreach (var phrase in rule)
                    {
                        var res = MatchPhrase(phrase,startIndex);
                        if (res.match)
                            ans.Add(res);
                    }
                    return ans;
                }
                (bool Match, )

            }
        }
    }
}
#endif