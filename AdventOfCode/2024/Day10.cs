

namespace Lomont.AdventOfCode._2024
{
    internal class Day10 : AdventOfCode
    {
        object Run2()
        {
            long answer = 0;

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
            // ProcessAllLines(new() { ... regex->action list ... });
            foreach (var line in ReadLines())
            {
                var nums = Numbers64(line);
            }

            return answer;
        }
        //public override string TestFileSuffix() => "-test1";
        public override string TestFileSuffix() => "";

        public override object Run(bool part2)
        {
            throw new NotImplementedException("Year 2024, day 10 not implemented");
            return part2 ? Run2() : Run1();
        }
    }
}