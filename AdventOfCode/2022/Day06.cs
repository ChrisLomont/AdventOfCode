
namespace Lomont.AdventOfCode._2022
{

    //
    // 
    //
    internal class Day06 : AdventOfCode
    {
        public override object Run(bool part2)
        {
            var line = ReadLines()[0];
            var len = part2 ? 14 : 4;
            for (var i = len-1; i < line.Length; ++i)
            {
                var dist = new int[26];
                for (var j = 0; j < len; ++j)
                {
                    var ch = line[i - j];
                    dist[ch - 'a']++;
                }

                var s = dist.Sum(v => v == 1 ? 1 : 0);
                if (s == len) return i+1;
            }


            return 0;
        }
    }
}