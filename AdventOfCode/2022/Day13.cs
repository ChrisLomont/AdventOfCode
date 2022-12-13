
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
    
    // todo - nice to have n-tree builder from text cleanly

    internal class Day13 : AdventOfCode
    {
        class Node
        {
            public List<Node> children = new();
            public string txt;
            public bool isNum;
            public int val;
            public override string ToString()
            {
                if (isNum) return $"N: {val}";
                return $"Ch: {children.Count}";
            }
        }

        public override object Run(bool part2)
        {
            long score = 0;
            var lines = ReadLines();
            var packets = new List<Node>();
            for (var index = 0; index < lines.Count; index+=3)
            {
                var left = Parse(lines[index]);
                var right = Parse(lines[index + 1]);
                packets.Add(right);
                packets.Add(left);

                if (Compare(left, right) < 0)
                    score += (index / 3)+1; // part 1 score: sum of 1 based packet pair indices
            }

            if (part2)
            {
                var t2 = "[[2]]";
                var t6 = "[[6]]";
                packets.Add(Parse(t2));
                packets.Add(Parse(t6));

                packets.Sort(Compare);

                var prod = 1;
                for (var i = 0; i < packets.Count; i++)
                    if (packets[i].txt == "[[2]]" ||  packets[i].txt == "[[6]]")
                        prod *= (i + 1); // part 2: product of 1 based special indices
                return prod;
            }


            return score; // not 5750

        }
        static int Compare(Node lp, Node rp)
        {
            if (lp.isNum && rp.isNum)
            {
                if (lp.val < rp.val)
                    return -1; // left smaller
                else if (lp.val > rp.val)
                    return +1; // right smaller
                return 0;
            }
            if (!lp.isNum && !rp.isNum)
            {
                int i = 0;
                while (true)
                {
                    if (i >= lp.children.Count && i < rp.children.Count)
                        return -1;
                    if (i >= rp.children.Count && i < lp.children.Count)
                        return +1;
                    if (i == rp.children.Count && i == lp.children.Count)
                        return 0;
                    var c = Compare(lp.children[i], rp.children[i]);
                    if (c != 0) return c;
                    ++i;
                }
            }

            if (lp.isNum && !rp.isNum)
            {
                var c = Compare(NumToList(lp), rp);
                if (c != 0) return c;
                return 0;
            }
            if (!lp.isNum && rp.isNum)
            {
                var c = Compare(lp, NumToList(rp));
                if (c != 0) return c;
                return 0;
            }

            return 0;

            Node NumToList(Node n)
            {
                var n1 = new Node();
                n1.children.Add(n);
                return n1;
            }
        } // compare

        static Node Parse(string line)
        {
            int ind = 0;
            var p = Recurse(line, ref ind);
            p.txt = line;
            return p;
            
            static Node Recurse(string line, ref int i)
            {
                var p = new Node();
                if (i == 0)
                {
                    Trace.Assert(line[i] == '[');
                    ++i;
                }

                while (true)
                {
                    var c = line[i++];
                    if (c == '[')
                        Add(Recurse(line, ref i));
                    else if (c == ']')
                        return p;
                    else if (char.IsDigit(c))
                    {
                        var val = c - '0';
                        while (char.IsDigit(line[i]))
                            val = 10 * val + (line[i++] - '0');
                        Add(new Node { isNum = true, val = val });
                    }
                    else if (c == ',')
                    {
                    }
                    else throw new Exception();
                }

                void Add(Node n)
                {
                    p.children.Add(n);
                }

            }
        }


    }
}