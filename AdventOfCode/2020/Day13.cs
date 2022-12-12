namespace Lomont.AdventOfCode._2020
{
    internal class Day13 : AdventOfCode
    {
        public override object Run(bool part2)
        {
            var lines = ReadLines();
            var min = Num(lines[0]);
            var w = Split(lines[1],',');
            var b = w.Where(x => x != "x").Select(Int32.Parse).ToList();

            if (part2)
            {
                // get pairs (period,delay)
                var l2 = lines[1].Replace("x", "0");
                var pairs = Numbers(l2).Select((v, i) => (v, i)).Where(q => q.v != 0).ToList();
                pairs.Reverse();
                var t = 0;
                foreach (var pair in pairs)
                {
                    var (period, delta) = pair;
                    t = period * t + delta;
                    Console.WriteLine($"per {period} de {delta} t {t}");
                }

                return t;

                // 

            }


            // 661 too low

            var (minId, minVal) = (-1, int.MaxValue);
            foreach (var busId in b)
            {
                var leftMinAgo = min % busId; // amount past last time
                

                var minTillNext = (busId - leftMinAgo) % busId;

                Console.WriteLine($"{min} % {busId} = {leftMinAgo} ago, {minTillNext} till next");

                if (minTillNext < minVal)
                {
                    (minId,minVal)=(busId,(int)minTillNext);
                }
            }

            return minId*minVal;

        }
    }
}