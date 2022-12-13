
namespace Lomont.AdventOfCode._2022
{
    // done 12:40am ~1840
    // rank1-100 both: 6:17 to 12:56
    // for prob 1: 3:28 to 8:16

    // me :|
    // 13   00:34:29    2304      0   00:39:48    1837      0
    // 12   02:31:31    8312      0   02:31:38    7678      0
    // 11   00:27:50    1417      0   00:33:35     800      0
    
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
                var ind = 0;
                var leftT = lines[index];
                var left = Parse(leftT,ref ind);
                ind = 0;
                var rightT = lines[index+1];
                var right = Parse(rightT,ref ind);
                left.txt = leftT;
                right.txt = rightT;
                packets.Add(right);
                packets.Add(left);

                var c = Compare(left, right);
                if (c < 0)
                {
                    //Console.WriteLine($"{c}: {leftT} {rightT} Correct");
                    score += (index / 3)+1;
                }
                else
                {
                    // Console.WriteLine($"{c}: {leftT} {rightT} Wrong");
                }



            }

            if (part2)
            {
                var ind = 0;
                var p2 = Parse("[[2]]", ref ind);
                p2.txt = "[[2]]";
                packets.Add(p2);
                ind = 0;
                var p6 = Parse("[[6]]", ref ind);
                p6.txt = "[[6]]";
                packets.Add(p6);

                packets.Sort((a, b) => Compare(a, b));
                //foreach (var p in packets)
                //    Console.WriteLine(p.txt);
                var prod = 1;
                for (var i = 0; i < packets.Count; i++)
                {
                    if (packets[i].txt == "[[2]]" ||
                        packets[i].txt == "[[6]]"
                       )
                        prod *= (i + 1);
                }

                return prod;


            }


            //Console.WriteLine();
            //Console.WriteLine();
            return score; // not 5750

            int Compare(Node lp, Node rp)
            {
                if (lp.isNum && rp.isNum)
                {
                    if (lp.val < rp.val)
                        return -1; // left smaller
                    else if (lp.val > rp.val)
                        return +1; // right smaller
                    return 0;
                }
                else if (!lp.isNum && !rp.isNum)
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

                else if (lp.isNum && !rp.isNum)
                {
                    var c = Compare(Pack(lp), rp);
                    if (c != 0) return c;
                    return 0;
                }
                else if (!lp.isNum && rp.isNum)
                {
                    var c = Compare(lp, Pack(rp));
                    if (c != 0) return c;
                    return 0;
                }

                return 0;

                Node Pack(Node n)
                {
                    var n1 = new Node();
                    n1.children.Add(n);
                    return n1;
                }
            } // compare

            Node Parse(string line, ref int i)
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
                    {
                        var n = Parse(line, ref i);
                        Set(n);
                    }
                    else if (c == ']')
                        return p;
                    else if (char.IsDigit(c))
                    {
                        var val = c - '0';
                        while (char.IsDigit(line[i]))
                        {
                            c = line[i++];
                            val = 10 * val + (c-'0');
                        }

                        Set(new Node { isNum = true, val = val });

                    }
                    else if (c == ',')
                    {
                    }
                }

                void Set(Node n)
                {
                    p.children.Add(n);
                }

            }
        }
    }
}