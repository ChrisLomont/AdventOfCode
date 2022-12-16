namespace Lomont.AdventOfCode._2019
{
    internal class Day09 : AdventOfCode
    {
        public override object Run(bool part2)
        {
            // 2682107844
            // 34738

            var lines = ReadLines();
            var prog = Numbers64(lines[0]);
            List<long> output = new();
            var mem = Day02.RunIntCode(prog, new List<int> { part2?2:1}, s=>output.Add(s));
            return output.Last();
        }
    }
}