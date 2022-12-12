namespace Lomont.AdventOfCode._2020
{
    internal class Day05 : AdventOfCode
    {
    //2020 Day 5 part 1: 826 in 10783.4 us
    //2020 Day 5 part 2: 678 in 1187.4 us
        public override object Run(bool part2)
        {

            List<long> seatIds = new();
            foreach (var line in ReadLines())
            {
                var rowText = line[..7];
                var colText = line[7..];

                rowText = rowText.Replace('F', '0').Replace('B', '1');
                colText = colText.Replace('L', '0').Replace('R', '1');

                var row = BinaryToInteger(rowText);
                var col = BinaryToInteger(colText);
                //Console.WriteLine(col);
                //Console.WriteLine(row);
                //Console.WriteLine();
                seatIds.Add(8*row + col);
            }
            //seatIds.Sort();
            //Dump(counts);

            var min = seatIds.Min();
            var max = seatIds.Max();
            var miss = Enumerable.Range((int)min, (int)max).Where(e => !seatIds.Contains(e)).ToList();
            //Dump(miss);
            //Console.WriteLine(min);
            //Console.WriteLine(max);

            var missed = miss.Where(t => t > min + 100 && t < max - 100).ToList();
            if (part2)
            return missed[0];
            return seatIds.Max();


        }
    }
}