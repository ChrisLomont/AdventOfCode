
using Lomont.Formats;

namespace Lomont.AdventOfCode._2022
{
    // done 12:40am ~1840
    // rank1-100 both: 6:17 to 12:56
    // for prob 1: 3:28 to 8:16

    // me :|
    // 13   00:34:29    2304      0   00:39:48    1837      0
    // 12   02:31:31    8312      0   02:31:38    7678      0
    // 11   00:27:50    1417      0   00:33:35     800      0

    //2022 Day 13 part 1: 5825 in 7241.4 us
    //2022 Day 13 part 2: 24477 in 1823.9 us

    internal class Day13 : AdventOfCode
    {

        public override object Run(bool part2)
        {
            long score = 0;
            var lines = ReadLines();
            var packets = new List<Tree>();
            for (var index = 0; index < lines.Count; index += 3)
            {
                packets.Add(new Tree(lines[index]));
                packets.Add(new Tree(lines[index + 1]));
                if (Compare(packets[^2], packets[^1]) < 0)
                    score += (index / 3) + 1; // part 1 score: sum of 1 based packet pair indices
            }

            if (part2)
            {
                var t2 = "[[2]]";
                var t6 = "[[6]]";
                packets.Add(new Tree(t2));
                packets.Add(new Tree(t6));

                packets.Sort(Compare);

                var prod = 1;
                for (var i = 0; i < packets.Count; i++)
                    if (packets[i].Text == "[[2]]" || packets[i].Text == "[[6]]")
                        prod *= (i + 1); // part 2: product of 1 based special indices
                return prod; // 
            }

            return score; // 5825
        }


        static int Compare(Tree lhs, Tree rhs)
        {
            (bool isNum, int val) GetNum(Tree t)
            {

                var isNum = Int32.TryParse(t.Payload, out var v);
                return (isNum, v);
            }

            var (lnum, lval) = GetNum(lhs);
            var (rnum, rval) = GetNum(rhs);

            if (lnum && rnum)
                return Math.Sign(lval - rval);
            if (!lnum && !rnum)
            {
                var (lc, rc) = (lhs.Children.Count, rhs.Children.Count);
                for (var i = 0; i < Math.Min(lc, rc); ++i)
                {
                    var c = Compare(lhs.Children[i], rhs.Children[i]);
                    if (c != 0) return c;
                }

                return Math.Sign(lc - rc);
            }

            return Compare(
                EnforceNumAsList(lhs),
                EnforceNumAsList(rhs));

            Tree EnforceNumAsList(Tree n)
            {
                if (!GetNum(n).isNum) return n;
                var n1 = new Tree();
                n1.Children.Add(n);
                return n1;
            }
        } // compare
    }
}