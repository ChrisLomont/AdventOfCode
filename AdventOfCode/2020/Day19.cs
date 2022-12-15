namespace Lomont.AdventOfCode._2020
{
    internal class Day19 : AdventOfCode
    {
        // 2020 Day 19 part 1: 233 in 373781.5 us
        // 2020 Day 19 part 2: 396 in 472377.1 us
        public override object Run(bool part2)
        {
            int score = 0;

            Dictionary<string, List<List<string>>> rules = new();
            Regex reg = new Regex("");

            foreach (var line1 in ReadLines())
            {
                if (line1.Contains(":"))
                {
                    var line = line1;
                    if (part2)
                        line = line.Replace("8: 42", "8: 42 | 42 8").Replace("11: 42 31", "11: 42 31 | 42 11 31");
                    var index = line.IndexOf(':');

                    var num = line[0..index];

                    var rule =
                        // split into ORed rules 
                        line[(index + 1)..].Split('|', StringSplitOptions.RemoveEmptyEntries)
                            // split each into list  of tokens
                            .Select(rule => rule.Split(' ', StringSplitOptions.RemoveEmptyEntries).ToList())
                            //all into a list
                            .ToList();

                    rules.Add(num, rule);
                }
                else if (String.IsNullOrEmpty(line1))
                {
                    var t = MakeRegex();
                    Console.WriteLine("Reg: "+t);
                    reg = new Regex("^"+t+"$"); // match start to end
                }
                else 
                {
                    if (reg.IsMatch(line1))
                    {
                        score++;
                        //Console.WriteLine($"Match {line1}");
                    }
                    else
                    {
                        //Console.WriteLine($"No match {line1}");
                    }
                }
            }

            return score;

            string MakeRegex(string r = "0", int depth=0)
            {
                if (depth == 20) return ""; // depth - works here, more complex needs higher cap
                if (rules[r][0][0].StartsWith('\"')) // single char
                    return rules[r][0][0].Trim('\"');

                var text = 
                        rules[r].Aggregate("", // start with '('
                        (a,subrule)=> // for each subrule, do...
                        (a==""?"":(a+'|') )
                        +
                        subrule.Aggregate("",(a,b)=>a+MakeRegex(b,depth+1)));
                return "(" + text + ")";
            }
        }
    }
}
