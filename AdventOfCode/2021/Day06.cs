namespace Lomont.AdventOfCode._2021
{
    internal class Day06 : AdventOfCode
    { // 374994
        // 1686252324092
        public override object Run(bool part2)
        {
            // track # in each state
            long[] timeToBirth = new long[10];

            var lines = ReadLines();
            var nums = GetNumbers(lines[0], false);
            foreach (var n in nums)
                timeToBirth[n]++;
            var steps = part2 ? 256 : 80;
            for (var step = 0; step < steps; ++step)
            {
                var atZero = timeToBirth[0];
                for (var i = 0; i < timeToBirth.Length-1; ++i)
                    timeToBirth[i] = timeToBirth[i + 1];
                timeToBirth[8] += atZero; // give birth here
                timeToBirth[6] += atZero; // and return here
            }


            return timeToBirth.Sum();
        }
    }
}