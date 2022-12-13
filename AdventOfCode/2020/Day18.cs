using System.Collections;
using System.Runtime.CompilerServices;

namespace Lomont.AdventOfCode._2020
{
    internal class Day18 : AdventOfCode
    {
        public override object Run(bool part2)
        {
            foreach (var line in ReadLines())
            {
                var v = Eval(line);
                Console.WriteLine($"{line} = {v}");
            }

            return 0;

            int Eval(string t)
            {
                t= t.Replace("(", " ( ").Replace(")"," ) ");
                var w = t.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                var ops = new Stack<char>();
                var nums = new Stack<int>();
                var index = 0;
                while (index < w.Length)
                {
                    var token = w[index++];
                    if (Char.IsDigit(token[0]))
                    {
                        var rhs = Int32.Parse(token);
                        if (nums.Count == 0)
                        {
                            nums.Push(rhs); // starting
                            continue;
                        }

                        var op = ops.Pop();
                        if (op == '(')
                            nums.Push(rhs);
                        else
                        {
                            // number: pop stack and op and do, push answer
                            var lhs = nums.Pop();
                            nums.Push(
                                op switch
                                {
                                    '+' => lhs + rhs,
                                    '*' => lhs * rhs,
                                    _ => throw new Exception()
                                }
                            );
                        }
                    }
                    else if (token == "(")
                    {
                        ops.Push('(');
                    }
                    else if (token == ")")
                    {
                        var rhs = nums.Pop();
                        var op = ops.Pop();
                        var lhs = nums.Pop();
                        nums.Push(
                            op switch
                            {
                                '+' => lhs + rhs,
                                '*' => lhs * rhs,
                                _ => throw new Exception()
                            }
                        );

                    }
                    else
                        ops.Push(token[0]);
                }
                Trace.Assert(ops.Count == 0 && nums.Count == 1);
                return nums.Pop();

                void DoOne()
                {

                }

            }
        }
    }
}