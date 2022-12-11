namespace Lomont.AdventOfCode._2020
{
    internal class Day02 : AdventOfCode
    {
        public override object Run(bool part2)
        {
            var count = 0;
            foreach (var line in ReadLines())
            {
                var w = Split(line);
                var n = Numbers(w[0],allowSigned:false);
                var c = w[1][0];
                if (part2)
                {
                    var b1 = w[2][n[0]-1] == c;
                    var b2 = w[2][n[1]-1] == c;
                    count += (b1^b2)?1:0;
                }
                else
                {
                    var count1 = w[2].Count(cc => cc == c);
                    count += (n[0] <= count1 && count1 <= n[1]) ? 1 : 0;
                }
            }

            return count;
        }
    }
}