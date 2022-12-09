namespace Lomont.AdventOfCode._2021
{
    internal class Day18 : AdventOfCode
    {
        class Node
        {
            private static int i = 0;

            public Node()
            {
                Id = ++i;
            }

            public int Id;
            public override string ToString()
            {
                return "Node: "+Id;
            }
            public Pair? Parent;
        }

        class Leaf : Node
        {
            public Leaf(Pair? parent, int val)
            {
                value = val;
                Parent = parent;
            }

            public int value;
        }

        class Pair : Node
        {
            public Pair(Pair? parent, Node left, Node right)
            {
                Parent = parent;
                Left = left;
                Right = right;
                left.Parent = this;
                right.Parent = this;
            }

            public bool IsLeft(Node q)
            {
                Trace.Assert(q.Parent == this && q.Parent.Id == Id);
                return Left.Id == q.Id && Left == q;
            }

            public bool IsRight(Node q)
            {
                Trace.Assert(q.Parent == this && q.Parent.Id == Id);
                return Right.Id == q.Id && Right == q;
            }



            public Node Left, Right;
        }

        public object Run2()
        {
            var lines = ReadLines();
            var sums = new List<long>();
            for (var i = 0; i < lines.Count; ++i)
            {
                for (var j = 0; j < lines.Count; ++j)
                {
                    int index = 0;
                    var a = MakePair(null, lines[i], ref index);

                    index = 0;
                    var b = MakePair(null, lines[j], ref index);

                    var s = new Pair(null, a, b);
                    Reduce(s);
                    sums.Add(Magnitude(s));
                }
            }

            return sums.Max();
        }



        public override object Run(bool part2)

        {
            if (part2)
            {
                return Run2();
            }

            var lines = ReadLines();
            Pair? sum = null;
            foreach (var line in lines)
            {
                var index = 0;
                if (sum == null)
                    sum = MakePair(null, line, ref index);
                else
                {
                    var right = MakePair(null, line, ref index);
                    sum = new Pair(null, sum, right);
                    sum.Left.Parent = sum;
                    right.Parent = sum;
                }

                Check(sum,true);

                Reduce(sum);

               // DumpP(sum);
            }

           // Console.WriteLine("Final");
           // DumpP(sum!);

            return Magnitude(sum);



        }
        static long Magnitude(Node n)
        {
            if (n is Pair p)
                return 3 * Magnitude(p.Left) + 2 * Magnitude(p.Right);
            else if (n is Leaf q)
                return q.value;
            else throw new Exception();
        }
        static void Reduce(Pair root)
        {
            // pair:
            // reduce:
            // - if nested in 4 pairs, leftmost explodes
            //    [a,b] -> a adds to number to left, b adds to  number to right, [a,b] replaced with 0
            // - any number >= 10, leftmost such splits
            //      n-> [(n)/2,(n+1)/2]
            // 
            var done = false;
            while (!done)
            {
                done = true; // assume no changes this loop

                Check(root, true);

                // try explode
                while (Explode(root))
                { // repeat
                    done = false; // something changed, will continue
                    Check(root, true);
                }
                Check(root, true);

                // try split, if any, try explode after
                if (Split(root))
                {
                    done = false;
                    Check(root, true);
                    continue;
                }
                Check(root, true);

            }

        }

        // sanity check tree
        static void Check(Node n, bool isRoot, HashSet<int>? seen = null)
        {
            if (seen == null)
                seen = new HashSet<int>();
            Trace.Assert(!seen.Contains(n.Id));
            seen.Add(n.Id);
            if (n is Leaf q)
                Trace.Assert(q.Parent != null && (q.Parent.IsLeft(q) || q.Parent.IsRight(q)));

            else if (n is Pair p)
            {
                if (!isRoot)
                    Trace.Assert(p.Parent != null && (p.Parent.IsLeft(p) || p.Parent.IsRight(p)));

                Check(p.Left,false,seen);
                Check(p.Right,false,seen);
            }
        }

        // try explode, return if done
        static bool Explode(Node root)
        {
           // Console.WriteLine("Explode pre:");
          //  DumpP(root);
            var ans = Recurse(root, 0);
            if (ans)
            {
              //  Console.WriteLine("Explode post:");
              //  DumpP(root);
            }

            return ans;

            bool Recurse(Node n, int depth)
            {
                if (n is Leaf) return false; // leaf does not explode
                else if (n is Pair p)
                {
                    if (depth == 4)
                    {
                        // explodes
                        Trace.Assert(p.Left is Leaf);
                        Trace.Assert(p.Right is Leaf);
                        var lv = (p.Left as Leaf)!.value;
                        var rv = (p.Right as Leaf)!.value;

                        // left added to nbr on left
                        AddLeft(p, lv);
                        // right added to nbr on right
                        AddRight(p, rv);
                        // replace node n with 0 leaf

                        var isLeft = p.Parent.Left == p;
                        if (isLeft)
                            p.Parent.Left = new Leaf(p.Parent, 0);
                        else
                            p.Parent.Right = new Leaf(p.Parent, 0);
                        return true;

                        // add to leftmost value if exists
                        void AddLeft(Pair q, int val)
                        {
                            // go up till enters a parent from the right
                            while (true)
                            {
                                if (q.Parent == null) return; // no nbr
                                var fromRight = q.Parent.Right == q;
                                q = q.Parent;
                                if (fromRight) break;
                            }

                            // take one step to the left
                            var p = q.Left;

                            // walk right till leaf
                            while (p is Pair q2)
                                p = q2.Right;
                            // add here
                            (p as Leaf).value += val;
                        }

                        // add to rightmost value if exists
                        void AddRight(Pair q, int val)
                        {
                            // go up till enters a parent from the left
                            while (true)
                            {
                                if (q.Parent == null) return; // no nbr
                                var fromLeft = q.Parent.Left == q;
                                q = q.Parent;
                                if (fromLeft) break;
                            }

                            // take one step to the right
                            var p = q.Right;

                            // walk left till leaf
                            while (p is Pair q2)
                                p = q2.Left;
                            // add here
                            (p as Leaf).value += val;
                        }

                    }

                    if (Recurse(p.Left, depth + 1)) return true;
                    if (Recurse(p.Right, depth + 1)) return true;
                }

                return false;
            }
        }



        static void DumpP(Node n, bool start = true)
        {
            if (n is Pair p)
            {
                Console.Write('[');
                DumpP(p.Left, false);
                Console.Write(',');
                DumpP(p.Right, false);
                Console.Write(']');
            }
            else if (n is Leaf q)
            {
                Console.Write(q.value);
            }

            if (start)
                Console.WriteLine();
        }


        Pair MakePair(Pair? parent, string line, ref int i)
        {
            if (i == 0)
            {
                Trace.Assert(line[i] == '[');
                ++i; // open this pair
            }

            Node? left = null, right = null;

            while (i < line.Length)
            {
                var c = line[i++];
                if (Char.IsDigit(c))
                {
                    Set(new Leaf(parent, c - '0'));
                }
                else if (c == ',')
                {
                    Trace.Assert(left != null && right == null);
                }
                else if (c == '[')
                {
                    Set(MakePair(parent, line, ref i));
                }
                else if (c == ']')
                {
                    Trace.Assert(left != null);
                    Trace.Assert(right != null);
                    var p = new Pair(parent, left!, right!);
                    p.Left.Parent = p;
                    p.Right.Parent = p;
                    return p;
                }
            }

            void Set(Node n)
            {
                if (left == null) left = n;
                else if (right == null) right = n;
                else throw new InvalidOperationException();
            }

            throw new Exception();
        }

        // true if any splits
        static bool Split(Node n, int depth = 0)
        {
            if (n is Leaf q)
            {
                Check(n,depth == 0);
                if (q.value >= 10)
                {
                    var isLeft = q.Parent.IsLeft(q);
                    var isRight = q.Parent.IsRight(q);
                    Trace.Assert(isLeft ^ isRight); // one or the other

                    var v = q.value;
                    var left = new Leaf(null, v / 2); // round down
                    var right = new Leaf(null, (v + 1) / 2); // round up
                    var nq = new Pair(q.Parent, left, right);
                    Check(n, depth == 0);
                    if (isLeft) q.Parent.Left = nq;
                    else q.Parent.Right = nq;

                    //Check(nq, depth == 0);
                    return true;
                }
                Check(n, depth == 0);

                return false;
            }

            if (n is Pair p)
            {
                if (Split(p.Left,depth+1)) return true;
                if (Split(p.Right,depth+1)) return true;
            }
            return false;
        }

    }
}
