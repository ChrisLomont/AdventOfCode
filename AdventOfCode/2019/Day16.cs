using System.Security.Cryptography;
using System.Xml.Schema;

namespace Lomont.AdventOfCode._2019
{
    internal class Day16 : AdventOfCode
    {
        public override object Run(bool part2)
        {
            var nums = ReadLines()[0].Select(c => c - '0').ToArray();

            long startIndex = 0;
            if (part2)
            {
                startIndex = nums.Take(7).Aggregate(0, (a, b) => a * 10 + b);
                var tmp = new List<int>();
                for (var k =0; k < 10000; ++k)
                    tmp.AddRange(nums);
                nums= tmp.ToArray();
            }

            Console.WriteLine($"Num len {nums.Length}");

            var pp = new int[] {1,0,-1,0 };
            // length k, pattern i
            int Patt(int k, int i)
            {
                if (i < k) return 0;
                i -= k;
                i %= 4 * (k+1);
                return pp[i / (k+1)];
            }

            for (var phase =0 ; phase < 100; ++phase)
            {
                Console.WriteLine($"Phase {phase}");
                //Console.Write($"Phase {phase}: ");
                //Dump(nums,true);
                var nxt = new int[nums.Length];

                for (var dst = 0; dst < nxt.Length; dst++)
                {
                    var sum = 0;
                    for (var k = 0; k < nums.Length; ++k)
                     sum += nums[k] * Patt(dst,k);
                    nxt[dst] = Math.Abs(sum) % 10;
                }
                nums = nxt;
            }
            
            //Dump(nums,true);
//            if (!part2)
//                return nums.Take(8).Aggregate(0, (a, b) => a * 10 + b);

            return nums.Skip((int)startIndex).Take(8).Aggregate(0, (a, b) => a * 10 + b);

        }
    }
}