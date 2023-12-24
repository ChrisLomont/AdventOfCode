namespace Lomont.AdventOfCode._2023
{
    internal class Day25 : AdventOfCode
    {
        object Run2()
        {
            long answer = 0;
            // var (w,h,g) = DigitGrid();
            // var (w,h,g) = CharGrid();

            foreach (var line in ReadLines())
            {
                var nums = Numbers64(line);
            }

            return answer;
        }

        object Run1()
        {
            long answer = 0;
            // var (w,h,g) = DigitGrid();
            // var (w,h,g) = CharGrid();

            foreach (var line in ReadLines())
            {
                var nums = Numbers64(line);
            }

            return answer;
        }

        public override object Run(bool part2)
        {
            return part2 ? Run2() : Run1();
        }

    }
}