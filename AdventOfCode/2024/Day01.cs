

using System.Numerics;

namespace Lomont.AdventOfCode._2024
{
    internal class Day01 : AdventOfCode
    {
        object Run2()
        {
            long answer = 0;
            // var (w,h,g) = DigitGrid();
            // var (w,h,g) = CharGrid();
            // ProcessAllLines(new() { ... regex->action list ... });
            List<long> a = new(), b = new();
            foreach (var line in ReadLines())
            {
                var nums = Numbers64(line);
                a.Add(nums[0]);
                b.Add(nums[1]);
            }

            foreach (var n in a)
            {
                var x = b.Count(z => z == n);
                answer += x * n;
            }

            return answer;
        }

        object Run1()
        {
            long answer = 0;
            // var (w,h,g) = DigitGrid();
            // var (w,h,g) = CharGrid();
            // ProcessAllLines(new() { ... regex->action list ... });
            List<long> a = new(), b = new();
            foreach (var line in ReadLines())
            {
                var nums = Numbers64(line);
                a.Add(nums[0]);
                b.Add(nums[1]);
            }
            a.Sort();
            b.Sort();
            for (int i = 0; i < a.Count; ++i)
            {
                answer += Int64.Abs(a[i] - b[i]);
            }

            return answer;
        }

        public override object Run(bool part2)
        {
            return part2 ? Run2() : Run1();
        }
    }
}