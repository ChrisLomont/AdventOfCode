namespace Lomont.AdventOfCode._2020
{
    //2020 Day 9 part 1: 776203571 in 3225.9 us
    //2020 Day 9 part 2: 104800569 in 397979.4 us

    // todo - switch to walking algo, finds quicker (linear instead of quadratic)

    internal class Day09 : AdventOfCode
    {
        public override object Run(bool part2)
        {
            var lines = ReadLines();
            int preamble = 25;
            var pre = lines.Take(preamble).Select(Int64.Parse).ToList();

            var ans = -1;
            for (var i = preamble; i<lines.Count && ans == -1; i++)
            {
                var n = int.Parse(lines[i]);
                if (!Sum2(n, pre,preamble))
                    ans = n;
                pre.Add(n);
            }
            if (!part2)
                return ans;

            var (a,b) = SubsetSum(ans);
            var cont = pre.Skip(a).Take(b - a).ToList();
            return cont.Min() + cont.Max();

            (int a, int b)
                SubsetSum(int total)
            {
                for (var i =0; i < pre.Count;++i)
                for (var b = i + 1; b < pre.Count; ++b)
                {
                    var s = pre.Skip(i).Take(b-i).Sum();
                    if (total == s && b-i>1) return (i, b);
                }

                throw new Exception();

            }

            return - 1;

            bool Sum2(int n, List<long> pre, int len)
            {
                var x = pre.Count;
                for (var i = 0; i < len; ++i)
                for (var j = 0; j < len; ++j)
                {
                    if (pre[x-1-i] + pre[x-1-j] == n) 
                        return true;

                }
                return false;
            }
        }
    }
}