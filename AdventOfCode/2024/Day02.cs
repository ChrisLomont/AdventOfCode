

namespace Lomont.AdventOfCode._2024
{
    internal class Day02 : AdventOfCode
    {
        object Run2()
        {
            long answer = 0;
            // var (w,h,g) = DigitGrid();
            // var (w,h,g) = CharGrid();
            // ProcessAllLines(new() { ... regex->action list ... });
            foreach (var line in ReadLines())
            {
                var nums = Numbers64(line);
                bool good = false;
                for (var i = 0; i < nums.Count; ++i)
                {
                    var n2 = nums.ToList();
                    n2.RemoveAt(i);
                    good |= Good(n2);
                }

                if (good) answer++;
            }
            return answer;

        }

        bool Good(List<long> nums)
        {
            bool incr = true, decr = true, close = true;
            for (var i = 0; i < nums.Count - 1; ++i)
            {
                incr &= nums[i] < nums[i + 1];
                decr &= nums[i] > nums[i + 1];
                close &= Math.Abs(nums[i] - nums[i + 1]) <= 3;
            }

            bool good = (incr || decr) && close;
            return good;

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
                if (Good(nums)) answer++;
            }
            return answer;
        }

        public override object Run(bool part2)
        {
            return part2 ? Run2() : Run1();
        }
    }
}