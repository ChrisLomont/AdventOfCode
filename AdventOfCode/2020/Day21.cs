namespace Lomont.AdventOfCode._2020
{
    internal class Day21 : AdventOfCode
    {
    //2020 Day 21 part 1: 2436 in 9001.3 us
    //2020 Day 21 part 2: dhfng,pgblcd,xhkdc,ghlzj,dstct,nqbnmzx,ntggc,znrzgs in 2466.3 us
        public override object Run(bool part2)
        {

            // allergens to possible ingreds
            Dictionary<string, HashSet<string>> map = new (); ;

            HashSet<string> allergens = new();
            HashSet<string> ingred = new();

            Dictionary<string, int> ingredCount = new();

            foreach (var line in ReadLines())
            {
                var split = line.IndexOf('(');
                var parts = line[..split].Split(' ',StringSplitOptions.RemoveEmptyEntries);
                var allerg = line[(split+1)..^1]
                    .Replace("contains","")
                    .Replace(" ",",")
                    .Split(',',StringSplitOptions.RemoveEmptyEntries);

                allergens.UnionWith(allergens);
                ingred.UnionWith(parts);
                foreach (var i in parts)
                {
                    if (!ingredCount.ContainsKey(i))
                        ingredCount.Add(i,0);
                    ingredCount[i]++;
                }

                foreach (var a in allerg)
                {
                    if (!map.ContainsKey(a))
                    {
                        map.Add(a, new());
                        foreach (var part in parts)
                            map[a].Add(part);
                    }
                    else
                    { // intersect
                        map[a].IntersectWith(parts);
                    }

                }

                //Console.WriteLine($"Parts"); 
                //Dump(parts);
                //Console.WriteLine($"allergen");
                //Dump(allerg);
                //Console.WriteLine("--");
            }

            var pairs = new List<(string part,string allerg)>();
            while (true)
            {
                // see if any items forced:
                var unique = new List<string>();
                foreach (var p in map)
                {
                    var (allerg, parts) = p;
                    //Console.Write($"allerg: {allerg} => ");
                    //foreach (var part in parts)
                    //    Console.Write($"{part}, ");
                    //Console.WriteLine();
                    if (parts.Count == 1)
                    {
                        var part = parts.First();
                      //  Console.WriteLine($"{allerg} maps to {part}");
                        unique.Add(part);
                        pairs.Add((part,allerg));
                    }
                }

                if (!unique.Any()) break;
                foreach (var part in unique)
                foreach (var p in map)
                    p.Value.Remove(part);
            }

            var usedIngred = pairs.Select(p => p.part).ToList();


            ingred.ExceptWith(usedIngred);

            if (part2)
            {
                pairs.Sort((a,b)=>a.allerg.CompareTo(b.allerg));
                var ans = pairs.Select(p=>p.part).Aggregate("", (a, b) => a + "," + b);

                return ans.Trim(',');
            }

            // count occurrences
            var count = 0;
            foreach (var unused in ingred)
                count += ingredCount[unused];



            return count;

        }
    }
}