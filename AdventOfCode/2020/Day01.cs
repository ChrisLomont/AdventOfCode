namespace Lomont.AdventOfCode._2020
{
    internal class Day01 : AdventOfCode
    {
        // 514579
        // 241861950
        public override object Run(bool part2)
        {
            var l = ReadLines().Select(int.Parse).ToList();
            l.Sort();
            if (part2)
            {
                for (var i = 0; i < l.Count; i++)
                for (var j = 0; j < l.Count; j++)
                for (var k = 0; k < l.Count; k++)
                {
                    if (l[i] + l[j] + l[k] == 2020)
                        return l[i] * l[j] * l[k];

                }
            }

            foreach (var i in l)
            {
                if (l.Contains(2020 - i))
                    return i * (2020 - i);
            }

            return -1;
        }
    }
}