

namespace Lomont.AdventOfCode._2024
{
    internal class Day22 : AdventOfCode
    {
        /*
2024 Day 22 part 1: 13584398738 in 130515.7 us
2024 Day 22 part 2: 1612 in 843775 us
        */

        //public override string TestFileSuffix() => "-test1";
        public override string TestFileSuffix() => "";



        object Run2()
        {
            Dictionary<(int, int, int, int), long> sums = new Dictionary<(int, int, int, int), long>();

            foreach (var l in ReadLines())
            {
                var (a1, a2, a3, a4) = (0, 0, 0, 0);
                var secret = ulong.Parse(l);

                HashSet<(int, int, int, int)> seen = new();
                var prv = (int)(secret % 10);
                for (int k = 0; k <= 1999; ++k)
                {
                    secret = (secret ^ (secret * 64)) % 16777216;
                    secret = (secret ^ (secret / 32)) % 16777216;
                    secret = (secret ^ (secret * 2048)) % 16777216;
                    var nxt = (int)(secret % 10);
                    (a1, a2, a3, a4) = (a2, a3, a4, nxt - prv);
                    prv = nxt;

                    if (k >= 3)
                    {
                        var key = (a1, a2, a3, a4);
                        if (!seen.Contains(key))
                        {
                            if (!sums.ContainsKey(key))
                                sums.Add(key, nxt);
                            else
                                sums[key] += nxt;
                        }

                        seen.Add((a1, a2, a3, a4));
                    }
                }
            }

            return sums.Values.Max();
        }



        object Run1()
        {
            ulong answer = 0;
            foreach (var l in ReadLines())
            {
                ulong secret = ulong.Parse(l);
                ;
                for (int k = 0; k <= 1999; ++k)
                {
                    secret = (secret ^ (secret * 64)) % 16777216;
                    secret = (secret ^ (secret / 32)) % 16777216;
                    secret = (secret ^ (secret * 2048)) % 16777216;
                }

                answer += secret;
            }


            return answer;
        }


        public override object Run(bool part2)
        {
            return part2 ? Run2() : Run1();
        }
    }
}