namespace Lomont.AdventOfCode._2023
{
    internal class Day02 : AdventOfCode
    {
        public override object Run(bool part2)
        {
            long sum = 0;
            foreach (var line in ReadLines())
            {
                sum += line.Length;
            }

            return sum;
        }
    }
}