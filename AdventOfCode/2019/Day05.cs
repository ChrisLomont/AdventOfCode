namespace Lomont.AdventOfCode._2019
{
    internal class Day05 : AdventOfCode
    {
        public override object Run(bool part2)
        {
            // 5182797
            // 12077198

            var command = part2 ? 5 : 1;

            var lines = ReadLines();
            var prog = Numbers64(lines[0]);
            List<long> output = new();

            var mem = Day02.RunIntCode(prog, new List<long> { command }, output.Add);
            return output.Last();
        }
    }
}