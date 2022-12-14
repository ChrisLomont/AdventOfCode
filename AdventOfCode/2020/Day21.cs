namespace Lomont.AdventOfCode._2020
{
    internal class Day21 : AdventOfCode
    {
        public override object Run(bool part2)
        {
            Dictionary<string, HashSet<string>> ingredToAllerg = new();
            Dictionary<string, HashSet<string>> allerToIngred = new();

            foreach (var line in ReadLines())
            {
                var split = line.IndexOf('(');
                var parts = line[..split].Split(' ',StringSplitOptions.RemoveEmptyEntries);
                var allerg = line[(split+1)..^1]
                    .Replace("contains","")
                    .Replace(" ",",")
                    .Split(',',StringSplitOptions.RemoveEmptyEntries);

                foreach (var all in allerg)
                {
                    if (!allerToIngred.ContainsKey(all))
                        allerToIngred.Add(all, new());
                    foreach (var part in parts)
                        allerToIngred[all].Add(part);
                }
                foreach (var part in parts)
                {
                    if (!ingredToAllerg.ContainsKey(part))
                        ingredToAllerg.Add(part, new());
                    foreach (var all in allerg)
                        ingredToAllerg[part].Add(all);
                }

                Console.WriteLine($"Spl"); 
                Dump(parts);
                Dump(allerg);
                Console.WriteLine("--");
            }

            return 0;

        }
    }
}