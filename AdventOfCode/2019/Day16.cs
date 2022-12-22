namespace Lomont.AdventOfCode._2019
{
    // 2019 Day 16 part 1: 40921727 in 478198.4 us
    // 2019 Day 16 part 2: 89950138 in 1491584.7 us

    internal class Day16 : AdventOfCode
    {
        object Run2(List<int> nums, long startIndex)
        {
            // we want answer from the back half (else could be a lot of work)
            Trace.Assert(nums.Count/2 < startIndex);

            // N = # nums
            // the pattern for the back half is 
            // N/2 0's, then a upper tri matrix of size N/2xN/2 where
            // lower tri all 0, upper tri all 1, so each pass is a suffix sum
            // can can be done in one pass

            var n1 = nums.ToArray();
            var n2 = new int[nums.Count];

            for (var phase = 0; phase < 100; ++phase)
            {
                var sum = 0;
                for (int k = nums.Count - 1; k >= nums.Count / 2; --k)
                {
                    sum += n1[k];
                    n2[k] = sum % 10;
                }
                (n1, n2) = (n2, n1);
            }

            return n1.Skip((int)startIndex).Take(8).Aggregate(0, (a, b) => a * 10 + b);
        }

        public override object Run(bool part2)
        {
            var nums = ReadLines()[0].Select(c => c - '0').ToList();
            long startIndex = 0;
            if (part2)
            {
                startIndex = nums.Take(7).Aggregate(0, (a, b) => a * 10 + b);
                var tmp = new List<int>();
                for (var k =0; k < 10000; ++k)
                    tmp.AddRange(nums);
                nums= tmp.ToList();
                return Run2(nums,startIndex);
            }

            Console.WriteLine($"Num len {nums.Count}, start Index {startIndex}");


            var pp = new int[] {1,0,-1,0 };
            // length k, pattern i
            int Patt(int k, int i)
            {
                if (i < k) return 0;
                i -= k;
                i %= 4 * (k+1);
                return pp[i / (k+1)];
            }

            //Console.SetCursorPosition(0, 0);
            for (var phase =0 ; phase < 100; ++phase)
            {
                //Console.WriteLine($"Phase {phase}");
                //Console.Write($"Phase {phase}: ");
                //Dump(nums,true);
                var nxt = new int[nums.Count];

                for (var dst = 0; dst < nxt.Length; dst++)
                {
                    //Console.SetCursorPosition(0,1);
                    //Console.Write($"{dst}/{nxt.Length}              ");
                    var sum = 0;
                    for (var k = 0; k < nums.Count; ++k)
                     sum += nums[k] * Patt(dst,k);
                    nxt[dst] = Math.Abs(sum) % 10;
                }
                nums.Clear();
                nums.AddRange(nxt);
            }

            //Dump(nums,true);
            //            if (!part2)
            //                return nums.Take(8).Aggregate(0, (a, b) => a * 10 + b);

            // 40921727
            return nums.Skip((int)startIndex).Take(8).Aggregate(0, (a, b) => a * 10 + b);

        }
    }
}