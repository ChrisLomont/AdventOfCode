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
                        } else if (t=="\"a\"" || t == "\"b\"")
                        {
                            var v = (int)t[1];
                            phrase.Add(-v);
                        }
                        else throw new Exception();
                    }
                }
                else if (!String.IsNullOrEmpty(line))
                {
                    var ok = IsOk(line);
                    if (ok)
                    {
                        score++;
                        Console.WriteLine($"Match {line}");
                    }
                    else
                    {
                        Console.WriteLine($"No match {line}");
                    }
                }
            }

            return score;

            bool IsOk(string line)
            {
                Dictionary<(int ruleIndex, int startIndex), HashSet<int>> memo = new();

                var index = 0;
                var ans = Recurse(0,0);
                return ans.Any(t => t == line.Length);

                //todo - memoize?


#if true
                // stack stores where we are:
                // op = 0 => in Recurse, 1=> in MatchPhrase, 2=>MatchSingle
                // pos = next index to try
                // IList<int> are the items to do...
                Stack<
                    (
                    (int ruleIndex, int phraseIndex, int itemIndex), 
                    HashSet<int> indices
                    )
                > state = new();



                // given line and start parse index, and a rule (list of phrases to match, any ok)
                // return list of matches and the new indices
                HashSet<int> Recurse(int ruleIndex, int startIndex)
                {
                    var key = (ruleIndex, startIndex);
                    if (!memo.ContainsKey(key))
                    {
                        var ruleMatchingIndices = new HashSet<int>();

                        for (var phraseIndex = 0; phraseIndex < rules[ruleIndex].Count; ++phraseIndex)
                        {
                            var phrase = rules[ruleIndex][phraseIndex];
                            
                            List<int> phraseIndices = new() { startIndex }; // each pass through loop updates the set of valid indices

                            for (var itemIndex = 0; itemIndex < phrase.Count; ++itemIndex)
                            {
                                var item = phrase[itemIndex];

                                if (item < 0)
                                {
                                    // char
                                    var ch = (char)(-item);
                                    var transformed = phraseIndices.Where(k => line[k] == ch).Select(k => k + 1)
                                        .ToList();
                                    var h = new HashSet<int>();
                                    foreach (var t in transformed)
                                        h.Add(t);
                                    phraseIndices = h.ToList();
                                }
                                else
                                {
                                    // match rule
                                    var h = new HashSet<int>();
                                    for (var subIndex= 0; subIndex < phraseIndices.Count;++subIndex)
                                    {
                                        var t = phraseIndices[subIndex];

                                        // items to save:
                                        //  ruleIndex, newIndices, phraseIndex, phraseIndices,itemIndex,

                                        var a = Recurse(item, t);
                                        foreach (var k in a)
                                            h.Add(k);
                                    }

                                    phraseIndices = h.ToList();
                                }

                                if (!phraseIndices.Any())
                                    break; // no solution
                            }

                            foreach (var t in phraseIndices)
                                ruleMatchingIndices.Add(t);
                        }

                        memo.Add(key, ruleMatchingIndices);
                    }

                    return memo[key];
                }
#else
                // given line and start parse index, and a rule (list of phrases to match, any ok)
                // return list of matches and the new indices
                List<int> Recurse(List<List<int>> rule, int startIndex)
                {
                    var newIndices = new List<int>();
                    foreach (var phrase in rule)
                        newIndices.AddRange(MatchPhrase(phrase,startIndex));
                    return newIndices;
                }

                List<int> MatchPhrase(List<int> phrase, int startIndex)
                {
                    HashSet<int> curIndices = new(){ startIndex };
                    foreach (var item in phrase)
                    {
                        curIndices = MatchSingle(item, curIndices);
                        if (!curIndices.Any()) return new List<int>(); // no solution
                    }
                    return curIndices.ToList(); // 
                }

                HashSet<int> MatchSingle(int item, HashSet<int> curIndices)
                {
                    if (item < 0)
                    { // char
                        var ch = (char)(-item);
                        var transformed = curIndices.Where(k => line[k] == ch).Select(k => k + 1).ToList();
                        var h = new HashSet<int>();
                        foreach (var t in transformed)
                            h.Add(t);
                        return h;
                    }
                    else
                    { // match rule
                        var h = new HashSet<int>();
                        foreach (var t in curIndices)
                        {
                            var a = Recurse(rules[item], t);
                            foreach (var k in a)
                                h.Add(k);
                        }
                        return h;
                    }
                }
#endif


            }
        }
    }
}
