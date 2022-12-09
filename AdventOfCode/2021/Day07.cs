namespace Lomont.AdventOfCode._2021
{
    internal class Day07 : AdventOfCode
    {
        public override object Run(bool part2)
        {
            var lines = ReadLines();
            var nums = GetNumbers(lines[0], false);
            var a = nums.Min();
            var b = nums.Max();

            var m = Int32.MaxValue;
            if (!part2)
            {
                for (var i = a; i <= b; ++i)
                    m = Math.Min(Sum(nums, i), m);
            }
            else
            {
                for (var i = a; i <= b; ++i)
                    m = Math.Min(Sum2(nums, i), m);
            }
            return m;

            int Sum(List<int> nums, int mean) => nums.Sum(v => Math.Abs(v - mean));
            int Sum2(List<int> nums, int mean)
            {
                
                return nums.Sum(v => Func(Math.Abs(v - mean)));

                int Func(int d) => (d + 1) * d / 2;
            }

        }
    }
}