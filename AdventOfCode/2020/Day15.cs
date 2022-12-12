namespace Lomont.AdventOfCode._2020
{
    internal class Day15 : AdventOfCode
    {
        public override object Run(bool part2)
        {
            // map val to last two indices, -1
            var hist = new Dictionary<int, (int, int)>();

            var nums1 = Numbers(ReadLines()[0]);
            int turn = 1, last = 0;

            foreach (var d in nums1)
            {
                Add(d);
            }

            void Add(int val)
            {
                if (!hist.ContainsKey(val))
                    hist.Add(val,(turn,-1));
                else
                {
                    var (i1,i2) = hist[val];
                    hist[val] = (turn, i1);
                }
                last = val;
                turn++;
            }


            // 30000000th item :)

            //if (part2) return -13;

            var end = part2 ? 30_000_000  : 2020;

            while (turn <= end)
            {
                int next = - 1;
                //var last = nums[turn-1];
                var (i1, i2) = hist[last];
                //var i = nums.IndexOf(last);
                if (i2 == -1)
                {
                    // was a first
                    next = 0;
                }
                else
                { // repeat, get last two indices
                    //var s2 = nums.Take(turn-1).ToList().FindLastIndex(v=>v==last);
                    //
                    //next = (turn - 1) - s2;
                    next = (turn - 1) - i2;
                }

                //if (end-5<=turn)
                //    Console.WriteLine($"{turn} {next}");
                Add(next);
                //Console.WriteLine(next);
                //++turn;
            }

            return last;

        }
    }
}