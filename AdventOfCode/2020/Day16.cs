
namespace Lomont.AdventOfCode._2020
{
    internal class Day16 : AdventOfCode
    {

    //2020 Day 16 part 1: 24110 in 26919.9 us
    //2020 Day 16 part 2: 6766503490793 in 7569.7 us
        public override object Run(bool part2)
        {
            var lines = ReadLines();
            var index = 0;
            var dc = new Dictionary<string, (int a1, int b1, int a2, int b2)>();
            while (lines[index].Contains(':'))
            {
                var line = lines[index++];
                var t = line.Substring(0, line.IndexOf(':'));
                var n = Numbers(line,allowSigned:false);
                dc.Add(t, (n[0], n[1], n[2], n[3]));
            }

            while (!lines[index].StartsWith("your ticket"))
                index++;
            index++;
            var myNums = Numbers(lines[index++]); //my numbers
            int rate = 0;
            while (!lines[index].StartsWith("nearby tickets"))
                index++;
            index++;

            // fields: each is hashset, remove failures
            List<List<(string,(int, int, int, int))>> fields = new List<List<(string,(int, int, int, int))>>();
            var fld = dc.Count;
            for (var i = 0; i < fld; i++)
            {
                fields.Add(new List<(string,(int,int,int,int))>());
                foreach (var p in dc)
                {
                    fields[i].Add((p.Key, p.Value));
                }
            }
            

            while (index < lines.Count)
            {
                var line = lines[index++];
                //Console.WriteLine("Item:" + line);
                if (String.IsNullOrEmpty(line)) continue;
                var lineNums = Numbers(line);

                bool valid = true;
                foreach (var n in lineNums)
                {
                    var localValid = false;
                    foreach (var p in dc)
                    {
                        var (a1, b1, a2, b2) = p.Value;

                        if ((a1 <= n && n <= b1) || (a2 <= n && n <= b2))
                            localValid = true;
                    }

                    if (!localValid)
                    {
                        //Console.WriteLine("Invalid "+n);
                        rate += n;
                    }
                    valid &= localValid;
                }

                if (valid)
                {
                    Trace.Assert(fld == lineNums.Count);
                    // part2, clean fields
                    for (var i = 0; i < fld; ++i)
                    {
                        var n = lineNums[i];
                        var ruleList = fields[i];
                        var removeList = new List<(string, (int, int, int, int))>();
                        Trace.Assert(ruleList.Any());
                        foreach (var rule in ruleList)
                        {
                            var (a1, b1, a2, b2) = rule.Item2;
                            var ok = (a1 <= n && n <= b1) || (a2 <= n && n <= b2);
                            if (!ok) removeList.Add(rule);
                        }

                        foreach (var r in removeList)
                        {
                            //if (r.Item1.StartsWith("departure"))
                            //    Console.WriteLine($"Remove {i}: {r.Item1} {r.Item2} for {n}");
                            ruleList.Remove(r);
                        }
                        Trace.Assert(ruleList.Any());

                    }
                }

            }
            // associate string with index
            List<(string, int)> finalMap = new List<(string, int)>();
            // track how many times each term occurs, loop till all unique
            while (finalMap.Count < fields.Count)
            {
                var single = fields.FirstOrDefault(p => p.Count == 1);
                if (single == null) break;
                var sIndex = fields.IndexOf(single);
                var txt = single[0].Item1;
                finalMap.Add((txt,sIndex));
                // Console.WriteLine($"Removing {txt}");
                foreach (var f in fields)
                    f.RemoveAll(p => p.Item1 == txt);
            }

            var ans = finalMap.Where(c => c.Item1.StartsWith("departure")).Select(c=>c.Item2).ToList();
            var prod1 = ans.Select(v => myNums[v]).Aggregate(1L, (a, b) => a * b);


            if (part2) return prod1; 

            return rate; 



        }
    }
}