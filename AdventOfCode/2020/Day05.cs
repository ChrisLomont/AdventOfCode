namespace Lomont.AdventOfCode._2020
{
    internal class Day05 : AdventOfCode
    {
    //2020 Day 5 part 1: 826 in 10783.4 us
    //2020 Day 5 part 2: 678 in 1187.4 us
        public override object Run(bool part2)
        {

            List<long> counts = new();
            foreach (var line in ReadLines())
            {
                var p = line[0..7];
                var q = line[7..];

                p = p.Replace('F', '0').Replace('B', '1');
                q = q.Replace('L', '0').Replace('R', '1');

                var row = BinaryToInteger(p);
                var col = BinaryToInteger(q);
                //Console.WriteLine(col);
                //Console.WriteLine(row);
                //Console.WriteLine();
                counts.Add(8*row + col);
            }
            counts.Sort();
            //Dump(counts);

            var min = counts.Min();
            var max = counts.Max();
            var miss = Enumerable.Range((int)min, (int)max).Where(e => !counts.Contains(e)).ToList();
            //Dump(miss);
            //Console.WriteLine(min);
            //Console.WriteLine(max);

            var mm = miss.Where(t => t > min + 100 && t < max - 100).ToList();
            if (part2)
            return mm[0];
            return counts.Max();


        }
    }
}