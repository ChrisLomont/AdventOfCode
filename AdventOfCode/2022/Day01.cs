namespace Lomont.AdventOfCode._2022
{
    internal class Day01 : AdventOfCode
    {
        public override object Run(bool part2)
        {
            List<int> elfTotals = new() { 0 };
            foreach (var line in ReadLines())
            {
                if (string.IsNullOrEmpty(line))
                {
                    // next elf
                    elfTotals.Add(0);
                }
                else
                {
                    var calories = Int32.Parse(line);
                    elfTotals[^1] += calories;
                }
            }
            if (!part2)
                return elfTotals.Max();
            else
            {
                elfTotals.Sort();
                var m = elfTotals.Skip(elfTotals.Count - 3).Sum();
                return m;
            }
        }

    }
}
