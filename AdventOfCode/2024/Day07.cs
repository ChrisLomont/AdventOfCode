

namespace Lomont.AdventOfCode._2024
{
    /*
     * 2024 Day 7 part 1: 5512534574980 in 140131.3 us
2024 Day 7 part 2: 328790210468594 in 7246685.4 us

2024 Day 7 part 1: 5512534574980 in 16367.5 us
2024 Day 7 part 2: 328790210468594 in 1039784.1 us    

2024 Day 7 part 1: 5512534574980 in 16854.6 us
2024 Day 7 part 2: 328790210468594 in 410719.1 us
    
     
     */

    internal class Day07 : AdventOfCode
    {
        // call Recurse(a,v,v[0],1)
        long ans;
        List<long> nums;
        bool threeop = false;
        bool noZeros = false;
        bool Recurse(long cur, int depth)
        {
            if (cur > ans && noZeros) return false; // cutoff - can only get bigger (or * 0?)
            checked // throw on overflow
            {
                if (depth >= nums.Count)
                    return ans == cur;
                return // short circuit search
                    Recurse(cur * nums[depth], depth + 1) ||
                    Recurse(cur + nums[depth], depth + 1) ||
                    (threeop && Recurse(Int64.Parse(cur.ToString() + nums[depth]), depth + 1));
            }
        }


        public override object Run(bool part2)
        {
            long answer = 0;
            threeop = part2;
            foreach (var line in ReadLines())
            {
                var w = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                ans = Int64.Parse(w[0][..^1]);
                nums = w.Skip(1).Select(t => Int64.Parse(t)).ToList();
                noZeros = !nums.Any(n=>n==0);
                if (Recurse(nums[0], 1))
                    answer += ans;
            }

            return answer;
        }
    }
}