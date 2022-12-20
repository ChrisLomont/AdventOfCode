namespace Lomont.AdventOfCode._2019
{
    internal class Day04 : AdventOfCode
    {
        public override object Run(bool part2)
        {
            var v1 = 356261; // my puzzle
            var v2 = 846303;
            int count = 0;

            for (var v = v1; v <= v2; v++)
            {
                var t = v.ToString();
                var nondecr = true;
                var is2 = false;
                for (var k = 0; k < t.Length - 1; ++k)
                {
                    nondecr &= t[k] <= t[k+1];
                    is2 |= IsPair(t, k);
                }
                if (part2)
                is2 |= IsPair(t, 5);

                if (nondecr && is2)
                    ++count;
            }

            bool IsPair(string t, int k)
            {
                if (part2)
                {
                    var s = k;
                    var e = k;
                    while (0 <= s - 1 && t[s - 1] == t[k])
                        s--;
                    while (e +1 <= 5 && t[e+1] == t[k])
                        e++;
                    return e - s == 1;
                }
                else
                {
                    if (t[k] == t[k + 1]) return true;
                }

                return false;
            }

            return count;
        }
    }
}