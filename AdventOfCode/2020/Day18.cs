
namespace Lomont.AdventOfCode._2020
{

    internal class Day18 : AdventOfCode
    {
    //2020 Day 18 part 1: 464478013511 in 8620.6 us
    //2020 Day 18 part 2: 85660197232452 in 2396.5 us



        public override object Run(bool part2)
        {
            var sum = 0L;
            foreach (var line in ReadLines())
            {
                //Console.Write($"Line {line} = ");
                var v = part2?Eval2(line):Eval(line);
                sum += v;
                //Console.WriteLine($"{v}");
            }

            return sum; // too low

            long Eval(string t)
            {
                t = t.Replace("(", " ( ").Replace(")", " ) ");
                var w = t.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                var ops1 = new Stack<string>();
                var nums1 = new Stack<long>();
                var index = 0;

                bool verbose = false;

                string PopOp()
                {
                    var op = ops1.Pop();
                    if (verbose)
                        Console.WriteLine("pop op: " + op);
                    return op;

                }

                void PushOp(string op)
                {
                    if (verbose)
                        Console.WriteLine("push op: " + op);
                    ops1.Push(op);
                }

                long PopNum()
                {
                    var n = nums1.Pop();
                    if (verbose)
                        Console.WriteLine("pop num: " + n);
                    return n;

                }

                void PushNum(long num)
                {
                    if (verbose)
                        Console.WriteLine("push num: " + num);
                    nums1.Push(num);
                }

                foreach (var token in w)
                {
                    if (verbose)
                        Console.WriteLine("Token: " + token);
                    if (Char.IsDigit(token[0]))
                    {
                        var rhs = Int32.Parse(token);
                        if (nums1.Any() && ops1.Any())
                        {
                            var op = PopOp();
                            if (op == "(")
                            {
                                PushOp(op);
                                PushNum(rhs);
                            }
                            else
                            {
                                // number: pop stack and op and do, push answer
                                var lhs = PopNum();
                                PushNum(
                                    op switch
                                    {
                                        "+" => lhs + rhs,
                                        "*" => lhs * rhs,
                                        _ => throw new Exception()
                                    }
                                );
                            }
                        }
                        else
                            PushNum(rhs);
                    }
                    else if (token == "(")
                    {
                        PushOp(token);
                    }
                    else if (token == ")")
                    {
                        var op = PopOp();
                        Trace.Assert(op == "("); // match closing:

                        if (ops1.Any())
                        {
                            op = PopOp();
                            if (op == "(")
                                PushOp(op);
                            else
                            {
                                // collapse to prev
                                var rhs = PopNum();
                                var lhs = PopNum();
                                PushNum(
                                    op switch
                                    {
                                        "+" => lhs + rhs,
                                        "*" => lhs * rhs,
                                        _ => throw new Exception()
                                    }
                                );
                            }
                        }
                    }
                    else
                        PushOp(token);
                }
                Trace.Assert(ops1.Count == 0 && nums1.Count == 1);
                return PopNum();

                void DoOne()
                {

                }

            }

            long Eval2(string t)
            {
                t= t.Replace("(", " ( ").Replace(")"," ) ");
                var tokens = t.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                var ops = new Stack<string>();
                var RPNqueue = new Queue<string>();

                bool LeftGreaterPrec(string lhs, string rhs)
                {
                    return lhs == "+" && rhs == "*";
                }

                foreach (var token in tokens)
                {
                    if (Char.IsDigit(token[0]))
                        RPNqueue.Enqueue(token);
                    else if (token == "(")
                    {
                        ops.Push("(");
                    }
                    else if (token == ")")
                    {
                        while (ops.Peek() != "(")
                        {
                            RPNqueue.Enqueue(ops.Pop());
                        }
                        Trace.Assert(ops.Pop()=="(");
                    }
                    else // if (isop)
                    {
                        var op = token;
                        while (ops.Any() && LeftGreaterPrec(ops.Peek(),op))
                            RPNqueue.Enqueue(ops.Pop());
                        ops.Push(op);
                    }
                }
                while (ops.Any())
                    RPNqueue.Enqueue(ops.Pop());

                // now RPN, eval
                var nums = new Stack<long>();
                foreach (var token in RPNqueue)
                {
                    if (char.IsDigit(token[0]))
                        nums.Push(Int64.Parse(token));
                    else
                    {
                        var rhs = nums.Pop();
                        var lhs = nums.Pop();
                        var ans = token switch
                        {
                            "+" => lhs+rhs,
                            "*" => lhs*rhs,
                            _ => throw new Exception()
                        };
                        nums.Push(ans);
                    }
                }
                Trace.Assert(nums.Count == 1);
                return nums.Pop();
            }
        }
    }
}