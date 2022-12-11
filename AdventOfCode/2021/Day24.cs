namespace Lomont.AdventOfCode._2021
{
    // solve https://adventofcode.com/2021/day/24
    // Chris Lomont
    // Interpreter for code, must solve for input digits 
    // of 14 digit number, each in 1-9 (never 0).
    //
    // has 4 regs, w,x,y,z
    // instructions:
    // inp a : read into var a, reads most sig digit, then next, etc. till done
    // add a b : a = a + b
    // mul a b : a = a * b
    // div a b : a = a / b, round towards 0, throw on error
    // mod a b : a = a % b, throw if a < 0 or b <= 0
    // eql a b : if a = b then a = 1 else a = 0
    // w=x=y=z=0 start?


    // 79997391969649
    // 16931171414113
    internal class Day24 : AdventOfCode
    {
        (long low, long high) Fast(List<string> lines)
        {
            // c1 defines a push/pop compare sequence, pairing digits
            // 0,13  1,12  2,7  3,4  5,6  8,11,  9,10
            int[] c1 = {1, 1, 1, 1, 26, 1, 26, 26, 1, 1, 26, 26, 26, 26};
            // push/pop pair 
            int[] c2 = {14, 13, 13, 12, -12, 12, -2, -11, 13, 14, 0, -12, -13, -6};
            // what gets pushed is input digit + c3[i]
            int[] c3 = {8, 8, 3, 10, 8, 8, 8, 5, 9, 3, 4, 9, 2, 7};

            // parse constants from lines (or above to read them nicely)
            for (var i = 0; i < 14; ++i)
            {
                c1[i] = Numbers(lines[i * 18 + 4])[0];
                c2[i] = Numbers(lines[i * 18 + 5])[0];
                c3[i] = Numbers(lines[i * 18 + 15])[0];
            }

            // make push,pop pairs based on c1
            var pairs = new List<(int, int)>();
            var s = new Stack<int>();
            for (var i = 0; i < c1.Length; ++i)
            {
                if (c1[i] == 1) 
                    s.Push(i);
                else
                    pairs.Add(new(s.Pop(), i));
            }

            // create all combos for 2 digit pairs
            // for each, store (push,pop) index and list of pairs
            List<(int push, int pop, List<(int, int)>)> all = new ();
            long combos = 1;
            foreach (var (push,pop) in pairs)
            {
                var p2 = new List<(int, int)>();
                for (var d1 = 1; d1 <= 9; ++d1)
                for (var d2 = 1; d2 <= 9; ++d2)
                {
                    var pushed = d1 + c3[push];
                    var last = d2 - c2[pop];
                    if (last == pushed)
                        p2.Add(new (d1, d2));
                }
                all.Add(new(push,pop,p2));
                combos *= p2.Count;
            }

            // now generate all combos

            var ans = new List<long>();
            var space = new long[14];

            Recurse(0);

            // return if fails
            void Recurse(int depth)
            {
                if (depth == 14/2)
                {
                    //if (CheckFull(space))
                    {
                        ans.Add(space.Aggregate(0L,(n,d)=>n*10+d));
                    }

                }
                else
                {
                    var (push, pop, pairs) = all[depth];
                    foreach (var (d1, d2) in pairs)
                    {
                        space[push] = d1;
                        space[pop] = d2;
                        Recurse(depth + 1);
                    }
                }
            }

            //            SF2(79997391969649);
            //            Console.WriteLine("\nLow");
            //            SF2(16931171414113);

            // ~28,000 answers
            // 16931171414113, 79997391969649
            var low = ans.Min();
            var high = ans.Max();
            return (low, high);

        } // Fast

        void Simple1()
        {
            int[] c1 = { 1, 1, 1, 1, 26, 1, 26, 26, 1, 1, 26, 26, 26, 26 };
            int[] c2 = { 14, 13, 13, 12, -12, 12, -2, -11, 13, 14, 0, -12, -13, -6 };
            int[] c3 = { 8, 8, 3, 10, 8, 8, 8, 5, 9, 3, 4, 9, 2, 7 };
            Trace.Assert(c1.Length == 14);
            Trace.Assert(c2.Length == 14);
            Trace.Assert(c3.Length == 14);

            Console.WriteLine("High");
            Simple(79997391969649);
            Console.WriteLine("Low");
            Simple(16931171414113);
            Console.WriteLine("Other");
            Simple(12345678991234);


            void Simple(long val)
            {
                var fg = Console.ForegroundColor;
                // z works like a stack
                long x = 0, y = 0, z = 0;
                bool useZ = true; // z or stack version

                var text = val.ToString();
                var s = new Stack<long>();
                s.Push(0);
                for (var pass = 0; pass < text.Length; ++pass)
                {

                    int inp = text[pass] - '0';
                    
                    var last = useZ
                        ? (z % 26) + c2[pass] // last digit plus offset
                        : s.Peek() + c2[pass];

                    var push = inp != last;

                    if (c1[pass] == 26) 
                        s.Pop();
                    if (useZ)
                        z /= c1[pass]; // z popped in rounds 4,6,7,10,11,12,13 (rounds 0-13)


                    y = push ? (inp + c3[pass]) : 0; // push some val or push 0
                    if (useZ)
                    {
                        z *= push ? 26 : 1; // z spaced out
                        z += y;
                    }

                    if (push)
                        s.Push(y);

                    if (useZ)
                    {
                        Console.Write($"{pass}: read {inp}, z base 26: ");
                        // base 26:
                        var t = z;
                        while (t > 0)
                        {
                            Console.Write($"{t % 26} ");
                            t /= 26;
                        }

                        Console.WriteLine();
                    }
                    else
                        Console.WriteLine($"Stack depth {s.Count}");
                }

                Console.ForegroundColor = fg;
                var f = s.Pop();
                Console.WriteLine($"Stack depth {s.Count} val {f}"); // 0 on success

#if false
                    if (push)
                    {
                        Console.ForegroundColor = push ? ConsoleColor.Red : fg;
                        Console.WriteLine("Push");
                        Console.ForegroundColor = fg;
                    }
                    else
                    {
                        Console.ForegroundColor = !push ? ConsoleColor.Yellow : fg;
                        Console.WriteLine("Pop");
                        Console.ForegroundColor = fg;
                    }
#endif
            }



        }
        public override object Run(bool part2)
        {

            /*
            This problem took a bit to run. The code below solves it by executing the operations on
            sets of numbers, until it finds sets that work, then it reduces the set.

            Alternatively, looking at the block of code with the 'grid' below, you can see
            the code is 14 blocks of very similar code (one block for each digit). Fiddle by hand, 
            remove instructions that affect nothing (noise inserted), and you get a checksum method.
            Code that directly, and the solution becomes instantaneous

            Run Simple1 to see how it works: z is a stack, base 26, digits push or pop things

             */

            //var (low,high) = RunFast(ReadLines());
            //if (!part2) return low;
            //return high;
            //return -2;

            var (low,high) = Fast(ReadLines());
            return part2?low:high;

            //Simple1();
            //return -1;

            var insts = ReadLines();
            bool showWork = true; // set to true to see long version run

#if true
            Trace.Assert(insts.Count == 18*14);
            // note 18*14 grid of same instructions
            var fg = Console.ForegroundColor;
            for (var i = 0; i < 18; ++i)
            {
                var same = Enumerable.Range(0, 14)
                    .Select(j => insts[i + j * 18])
                    .All(m => m == insts[i]);
                Console.ForegroundColor = same?fg: ConsoleColor.Yellow;
                for (var j = 0; j < 14; ++j)
                {
                    Console.Write($"{insts[i + j * 18],-12}");
                    //Trace.Assert(insts[i+j*18][0..5] == insts[j][0..5]);
                }
                Console.WriteLine();
            }
            Console.WriteLine();
            Console.ForegroundColor = fg;

            return -1;
#endif
            if (part2)
            {
                // my largest was 79997391969649
                // my smallest was 16931171414113
                var smallest = SetReduce(111, 999, showWork, insts); // find smallest
                //Console.WriteLine("Smallest: " + smallest);
                //return smallest;
            }

            var largest = SetReduce(999, 111, showWork, insts); // find largest
            Console.WriteLine("Largest: " + largest);
            return largest;
        }







        string SetReduce(int first, int last, bool show, List<string> insts)
        {
            var done = false;
            var dir = Math.Sign(last - first);
            //for (var pref = 111; pref <= 999; ++pref) // smallest
            var pref = first;
            while (!done)
            {
                var pre1 = pref.ToString();
                if (!pre1.Contains('0'))
                {
                    Queue<string> prefixes = new();
                    prefixes.Enqueue(pre1);
                    while (prefixes.Count > 0 && !done)
                    {
                        var pre = prefixes.Dequeue();
                        if (PrefixReduce(pre, insts))
                        {
                            if (pre.Length == 14)
                                return pre;

                            for (var d = 1; d <= 9; ++d)
                                prefixes.Enqueue(pre + d);

                            if (show)
                                Console.WriteLine($"{pre} soln, queue size {prefixes.Count}");
                        }
                    }
                }

                // next
                if (pref == last) done = true;
                pref += dir;
            }

            throw new Exception();
        }

// does this prefix allow a solution?
        bool PrefixReduce(string prefix, List<string> insts)
        {
            var inputs = new Set[]
            {
                new Set {1, 2, 3, 4, 5, 6, 7, 8, 9},
                new Set {1, 2, 3, 4, 5, 6, 7, 8, 9},
                new Set {1, 2, 3, 4, 5, 6, 7, 8, 9},
                new Set {1, 2, 3, 4, 5, 6, 7, 8, 9},
                new Set {1, 2, 3, 4, 5, 6, 7, 8, 9},
                new Set {1, 2, 3, 4, 5, 6, 7, 8, 9},
                new Set {1, 2, 3, 4, 5, 6, 7, 8, 9},
                new Set {1, 2, 3, 4, 5, 6, 7, 8, 9},
                new Set {1, 2, 3, 4, 5, 6, 7, 8, 9},
                new Set {1, 2, 3, 4, 5, 6, 7, 8, 9},
                new Set {1, 2, 3, 4, 5, 6, 7, 8, 9},
                new Set {1, 2, 3, 4, 5, 6, 7, 8, 9},
                new Set {1, 2, 3, 4, 5, 6, 7, 8, 9},
                new Set {1, 2, 3, 4, 5, 6, 7, 8, 9},
            };

            // check a fixed start
            for (var i = 0; i < prefix.Length; ++i)
                inputs[i] = new Set {prefix[i] - '0'}; // fixed digit

            // registers w,x,y,z
            Set[] regs = new[] {new Set {0}, new() {0}, new() {0}, new() {0}};

            var nullReg = new Set();
            var inpCount = 0; // 1st digit
            foreach (var line in insts)
            {
                var words = line.Split(new char[] {' ', '\r'}, StringSplitOptions.RemoveEmptyEntries);

                Set Get(string reg) =>
                    regs[reg[0] - 'w'];

                var reg = Get(words[1]);
                var right =
                    (words.Length > 2)
                        ? (Number(words[2])
                            ? new Set { Int32.Parse(words[2]) }
                            : Get(words[2]))
                        : nullReg;

                //Console.WriteLine($"{regs[0].Count} {regs[1].Count} {regs[2].Count} {regs[3].Count}");

                switch (words[0])
                {
                    case "inp":
                        reg.SetIt(inputs[inpCount++]);
                        break;
                    case "add":
                        reg.ApplyOp((a, b) => a + b, right);
                        break;
                    case "mul":
                        reg.ApplyOp((a, b) => a * b, right);
                        break;
                    case "div":
                        reg.ApplyOp((a, b) => b == 0 ? Int32.MaxValue : a / b, right);
                        break;
                    case "mod":
                        reg.ApplyOp((a, b) => (a < 0 || b <= 0) ? Int32.MaxValue : a % b, right);
                        break;
                    case "eql":
                        reg.ApplyOp((a, b) => a == b ? 1 : 0, right);
                        break;
                    default:
                        throw new NotImplementedException(words[0]);
                }
            }

            return regs[3].Contains(0); // z?


            bool Number(string txt) => (txt.Length > 0 && (char.IsDigit(txt[0]) || txt[0] == '-'));
        }

        class Set : List<int>
        {
            public override string ToString()
            {
                var sb = new StringBuilder();
                foreach (var v in this)
                    sb.Append($"{v}, ");
                return sb.ToString();
            }

            public void SetIt(Set a)
            {
                this.Clear();
                this.AddRange(a);

            }


            // modify this set by applying op(a,b) to all a in here, b in s
            // op set int to MaxInt for fails (those are ignored)
            // result normalized and unioned
            public void ApplyOp(Func<int, int, int> op, Set s)
            {
                var h = new HashSet<int>();

                foreach (var a in this)
                foreach (var b in s)
                {
                    var c = op(a, b);
                    if (c != int.MaxValue)
                        h.Add(c);
                }

                this.Clear();
                this.AddRange(h);
            }
        }

    }

}

