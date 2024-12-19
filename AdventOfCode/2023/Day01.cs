namespace Lomont.AdventOfCode._2023
{
    internal class Day01 : AdventOfCode
    {
        /*
2023 Day 1 part 1: 55386 in 9466.1 us
2023 Day 1 part 2: 54824 in 7516.1 us         
         */


        string[] nums = {
            "0", "1", "2", "3", "4", "5", "6", "7", "8", "9",
            "zero", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine" };

        int Value(string line, bool part2)
        {
            var digitIndices =
                nums.Take(part2 ? 20 : 10) // items to search
                    .Select((num, index) => (
                        forwardIndex : line.IndexOf(num), // first going forward
                        backwardIndex: line.Reverse().IndexOf(num.Reverse()), // first going backwards
                        digit        : index % 10 // digit 0-9 we look at
                    )).ToList();

            return 
                10 * digitIndices.Where(q => q.forwardIndex != -1).MinBy(q => q.forwardIndex).digit
                + digitIndices.Where(q => q.backwardIndex != -1).MinBy(q => q.backwardIndex).digit;
        }

        public override object Run(bool part2) =>
            ReadLines().Aggregate(0, (sum, line) => sum + Value(line, part2));
    }
}