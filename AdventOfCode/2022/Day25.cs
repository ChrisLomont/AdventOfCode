
namespace Lomont.AdventOfCode._2022
{
    // top 100 both 2:51 to 8:30
    // first part: 2:47 to 7:54

    // --------Part 1---------   --------Part 2---------
    //     Day Time    Rank Score       Time Rank  Score
    // 25   00:22:44     920      0   00:22:51     780      0
    // 24   01:09:31    1282      0   01:13:06    1085      0

    //2022 Day 25 part 1: 2-=2==00-0==2=022=10 in 10378.6 us
    //2022 Day 25 part 2: 2-=2==00-0==2=022=10 in 346.7 us

    internal class Day25 : AdventOfCode
    {
        public override object Run(bool part2)
        {
            var sum = 0L;
            foreach (var line in ReadLines())
            {
                // 2,1,0,-,=
                long val = 0;
                foreach (var c in line)
                {
                    var digit = "=-012".IndexOf(c) - 2;
                    val = 5 * val + digit;
                }

                sum += val;
            }

            var s = "";
            while (sum > 0)
            {
                var d = sum % 5;
                s = "012=-"[(int)d]+s;
                if (d > 2)
                    d = d - 5;
                sum -= d;
                sum /= 5;
            }
            return s;
        }
    }
}