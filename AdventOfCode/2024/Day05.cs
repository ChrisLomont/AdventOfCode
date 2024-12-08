

namespace Lomont.AdventOfCode._2024
{
    // done 7:23 am
    // started 7:14

    /*
     * 2024 Day 5 part 1: 4569 in 83939.8 us
2024 Day 5 part 2: 6456 in 211432.7 us
     */

    internal class Day05 : AdventOfCode
    {

        object Run2()
        {
            long answer = 0;
            // var (w,h,g) = DigitGrid();
            // var (w,h,g) = CharGrid();
            // ProcessAllLines(new() { ... regex->action list ... });
            List<(int, int)> rules = new List<(int, int)>();
            foreach (var line in ReadLines())
            {
                if (line.Contains('|'))
                {
                    var w = line.Split('|', StringSplitOptions.RemoveEmptyEntries);
                    var n1 = Int32.Parse(w[0]);
                    var n2 = Int32.Parse(w[1]);
                    rules.Add((n1, n2));
                }
                else if (line.Contains(','))
                {
                    var nums = Numbers64(line);
                    if (!Good(nums, rules))
                    {
                        //Order(nums, rules);
                        nums.Sort((a,b)=>Compare(a,b,rules));
                        var n = nums.Count;
                        answer += nums[n / 2];
                    }
                }
            }

            return answer;
        }

        int Compare(long lhs, long rhs, List<(int, int)> rules)
        {
            if (lhs == rhs) return 0;
            foreach (var rule in rules)
            {
                if (rule.Item1 == lhs && rule.Item2 == rhs) return 1;
                if (rule.Item1 == rhs && rule.Item2 == lhs) return -1;
            }

            throw new NotImplementedException();

        }

        void Order(List<long> nums, List<(int, int)> rules)
        {
            while (!Fixup())
            {
            }
            // flip any errors
            bool Fixup()
            {
                bool error = false;
                for (int i = 0; i < nums.Count; i++)
                {
                    for (int j = i + 1; j < nums.Count; j++)
                    {
                        var lhs = nums[i];
                        var rhs = nums[j];
                        foreach (var r in rules)
                        {
                            if (r.Item1 == rhs && r.Item2 == lhs)
                            {
                                nums[i] = rhs;
                                nums[j] = lhs;
                                error = true;
                                //return false;
                            }
                        }
                    }
                }

                return !error;//true;
            }
        }

        object Run1()
        {
            long answer = 0;
            // var (w,h,g) = DigitGrid();
            // var (w,h,g) = CharGrid();
            // ProcessAllLines(new() { ... regex->action list ... });
            List<(int, int)> rules = new List<(int, int)>();
            foreach (var line in ReadLines())
            {
                if (line.Contains('|'))
                {
                    var w = line.Split('|',StringSplitOptions.RemoveEmptyEntries);
                    var n1 = Int32.Parse(w[0]);
                    var n2 = Int32.Parse(w[1]);
                    rules.Add((n1,n2));
                }
                else if (line.Contains(','))
                {
                    var nums = Numbers64(line);
                    if (Good(nums, rules))
                    {
                        var n = nums.Count;
                        answer += nums[n/2];
                    }
                }
            }

            return answer;
        }

        bool Good(List<long> nums, List<(int, int)> rules)
        {
            for (int i = 0; i < nums.Count; i++)
            {
                for (int j = i+1; j < nums.Count; j++)
                {
                    var lhs = nums[i];
                    var rhs = nums[j];
                    foreach (var r in rules)
                    {
                        if (r.Item1 == rhs && r.Item2 == lhs)
                            return false;
                    }
                }
            }

            return true;
        }

        public override object Run(bool part2)
        {
            return part2 ? Run2() : Run1();
        }
    }
}