
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
    // example probs 2022: 13, 2021 18, 10, 

    // maybe make bottom up regex based?

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
        static int Compare(Node lhs, Node rhs)
        {
            if (lhs.isNum && rhs.isNum) 
                return Math.Sign(lhs.val - rhs.val);
            if (!lhs.isNum && !rhs.isNum)
            {
                var (lc, rc) = (lhs.children.Count, rhs.children.Count);
                for (var i = 0; i < Math.Min(lc, rc); ++i)
                {
                    var c = Compare(lhs.children[i], rhs.children[i]);
                    if (c != 0) return c;
                }

                return Math.Sign(lc - rc);
            }

            return Compare(
                    EnforceNumAsList(lhs), 
                    EnforceNumAsList(rhs));

            Node EnforceNumAsList(Node n)
            {
                if (!n.isNum) return n;
                var n1 = new Node();
                n1.children.Add(n);
                return n1;
            }
        } // compare

        static Node Parse(string line)
        {
            Trace.Assert(line[0] == '[');
            var i = 1; // start part opening '['
            var root = Recurse();
            root.txt = line;
            return root;

            Node Recurse()
            {
                var p = new Node();
                while (true)
                {
                    var c = line[i++];
                    if (c == '[')
                        p.children.Add(Recurse());
                    else if (c == ']')
                        return p;
                    else if (char.IsDigit(c))
                    {
                        var val = c - '0';
                        while (char.IsDigit(line[i]))
                            val = 10 * val + (line[i++] - '0');
                        p.children.Add(new Node { isNum = true, val = val });
                    }
                }
            }
        }

        // parsing experiment - bottom up
        static Node Parse2(string line)
        {

            //var leafReg = new Regex(@"([0-9]+,)*[0-9]+");
            var leafReg = new Regex(@"\[(((x\d+|\d+),)*((x\d+|\d+)))?\]");

            Console.WriteLine("Matching " + line);
            var nodeIndex = 0; // node index
            while (true)
            {
                var matches = leafReg.Matches(line);
                if (matches.Count == 0) break;
                foreach (Match m in matches)
                {
                    Console.Write($"<{m.Value}> ");
                    line = line.Replace(m.Value, $"x{nodeIndex}");
                    ++nodeIndex;
                }
                Console.WriteLine();
                Console.WriteLine("reduced " + line);
            }

            return null;

        }



    }
}