

using System.Runtime.Intrinsics.Arm;

namespace Lomont.AdventOfCode._2018
{
    internal class Day02 : AdventOfCode
    {
        /*
         * 2018 Day 2 part 1: 6225 in 1530.4 us
2018 Day 2 part 2: revtaubfniyhsgxdoajwkqilp in 3252.8 us
         */
        object Run2()
        {
            long answer = 0;
            var lines = ReadLines();
            for (var i = 0; i < lines.Count; ++i)
            for (var j = i + 1; j < lines.Count; ++j)
            {
                var a = lines[i];
                var b = lines[j];
                if (a.Length != b.Length) continue;
                int mis = 0, ind = 0;
                for (int k = 0; k < a.Length; ++k)
                {
                    if (a[k] != b[k])
                    {
                        mis++;
                        ind = k;
                    }

                }

                if (mis == 1)
                {
                    //Console.WriteLine(a);
                    //Console.WriteLine(b);
                    a = a.Remove(ind,1);
                    b = b.Remove(ind,1);
                    //Console.WriteLine(a);
                    Assert(a==b);
                    return a;
                }


                }

            return 0;
            }

        object Run1()
        {
            long answer = 0;
            int c2 = 0, c3 = 0;
            foreach (var line in ReadLines())
            {
                int[] ct = new int[26];
                foreach (var c in line)
                    ct[c-'a']++;
                c2 += ct.Any(c => c == 2) ? 1 : 0;
                c3 += ct.Any(c => c == 3) ? 1 : 0;

            }

            return c2*c3;
        }

        public override object Run(bool part2)
        {
            return part2 ? Run2() : Run1();
        }
    }
}