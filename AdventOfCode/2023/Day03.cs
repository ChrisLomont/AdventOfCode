namespace Lomont.AdventOfCode._2023
{
    internal class Day03 : AdventOfCode
    {

        /*
2023 Day 3 part 1: 546312 in 309174.3 us
2023 Day 3 part 2: 87449461 in 212337.8 us

both stars
  1) Dec 03  00:05:09  xiaowuc1
  2) Dec 03  00:06:31  hyper-neutrino
  3) Dec 03  00:06:47  Joseph Durie
  4) Dec 03  00:07:02  ruuddotorg
  5) Dec 03  00:07:11  Ian DeHaan
  6) Dec 03  00:07:19  Ryan Hitchman (AoC++)
  7) Dec 03  00:07:27  Robert Xiao

 98) Dec 03  00:11:36  Dmytro Kozhevin
 99) Dec 03  00:11:37  Roman Elizarov (Sponsor)
100) Dec 03  00:11:37  Ryan Dewey

First hundred users to get the first star on Day 3:

  1) Dec 03  00:03:11  Nordine Lotfi
  2) Dec 03  00:03:39  xiaowuc1
  3) Dec 03  00:03:57  bluepichu
  4) Dec 03  00:04:04  Nikitosh
  5) Dec 03  00:04:09  Ian DeHaan
  6) Dec 03  00:04:12  hyper-neutrino

 97) Dec 03  00:07:06  Sam Le
 98) Dec 03  00:07:06  Verulean
 99) Dec 03  00:07:08  Jeff Zhang
100) Dec 03  00:07:09  Antonio Molina (Sponsor)

      --------Part 1---------   --------Part 2--------
Day       Time    Rank  Score       Time   Rank  Score
  3   00:19:04    1331      0   00:29:57   1410      0
  2   00:06:03     329      0   00:09:23    411      0
  1   17:45:07  131162      0   18:25:46  92867      0         
         */

        record class Pos(int col, int row, int length, int val);

        public override object Run(bool part2)
        {
            //var (w, h, g) = CharGrid();
            //DumpColors<char> colors = new()
            //{
            //    ch=>("0123456789".Contains(ch),ConsoleColor.Cyan),
            //    ch=>("*".Contains(ch),ConsoleColor.Red),
            //    ch=>(".".Contains(ch),ConsoleColor.DarkGray),
            //    ch=>(true,ConsoleColor.Yellow)
            //};
            //Dump(g,true, colors:colors);
            //return 0;

            var lines = ReadLines();
            var r = new Regex(@"\d+");

            List<Pos> nums = new();

            Dictionary<(int, int), char> symbols = new();

            for (int row = 0; row < lines.Count; ++row)
            {
                var line = lines[row];

                foreach (var v in r.EnumerateMatches(line))
                    nums.Add(new(v.Index, row, v.Length, Int32.Parse(line.Substring(v.Index, v.Length))));

                for (int col = 0; col < line.Length; ++col)
                {
                    if (!"0123456789.".Contains(line[col]))
                        symbols.Add((col, row), line[col]);
                }
            }

            var ans1 = 0;
            foreach (var n in nums)
            {
                var hasSymbol = false;
                for (int i = n.col - 1; i <= n.col + n.length; ++i)
                for (int j = n.row - 1; j <= n.row + 1; ++j)
                    hasSymbol |= symbols.ContainsKey((i, j));
                if (hasSymbol)
                    ans1 += n.val;
            }

            HashSet<(int, int)> Make(Pos n)
            {
                HashSet<(int, int)> h = new();
                for (int i = n.col - 1; i <= n.col + n.length; ++i)
                for (int j = n.row - 1; j <= n.row + 1; ++j)
                {
                    if (symbols.ContainsKey((i, j)) && symbols[(i, j)] == '*')
                        h.Add((i, j));
                }
                return h;
            }

            var ans2 = 0;
            for (var i = 0; i < nums.Count; ++i)
            {
                var hi = Make(nums[i]);
                for (var j = i + 1; j < nums.Count; ++j)
                {
                    var hj = Make(nums[j]);
                    hj.IntersectWith(hi);
                    if (hj.Count == 1)
                        ans2 += nums[i].val * nums[j].val;
                }
            }
            return part2 ? ans2 : ans1;
        }


        public object RunOLD(bool part2)
        {
            int answer = 0,ans2=0;
            var (w, h, g) = CharGrid();
            

            List< (int i, int j, int len, int num)> nums = new();
            int startI = 0;

            // walk all directions?
            bool inNum = false, hasNbr = false;
            int val = 0;
            for (var j = -1; j <= h; ++j) // must parse past ends?
            for (var i = -1; i <= w; ++i)
            {
                var ch = Get(g, i, j, '.');

                if (Char.IsAsciiDigit(ch))
                {
                    if (!inNum)
                        startI = i;
                    inNum = true;

                    val = 10 * val + ch - '0';
                    hasNbr |= HasNbr(i, j);
                }
                else if (inNum)
                {
                    // track positions, length, value
                    nums.Add((startI,j,i-startI,val));

                    if (hasNbr)
                        answer += val;

                    val = 0;
                    inNum = false;
                    hasNbr = false;
                }
            }

            bool HasNbr(int i, int j)
            {
                for (var di = -1; di <= 1; ++di)
                for (var dj = -1; dj <= 1; ++dj)
                    if (!"0123456789.".Contains(Get(g, di + i, dj + j, '.')))
                        return true;
                return false;
            }

            for (var j = -1; j <= h; ++j)
            for (var i = -1; i <= w; ++i)
            {
                var ch = Get(g, i, j, '.');

                if (ch == '*')
                {
                    int prod = 1, adj = 0;
                    foreach (var (numi, numj, len, num) in nums)
                    {
                        if (numi - 1 <= i && i <= numi + len && numj - 1 <= j && j <= numj + 1)
                        {
                            adj++;
                            prod *= num;
                        }
                    }

                    if (adj==2)
                        ans2 += prod;
                }
            }

            if (part2)
                return ans2;
            return answer;
        }
    }
}