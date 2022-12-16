
using System.Runtime.Intrinsics.X86;
using System.Text;

namespace Lomont.AdventOfCode._2022
{
    // rank1-100: 8:16 to 18:05 for both, 6:06 to 13:07 for first

    // had hard time getting ok parser - need better way
    // Day Time    Rank Score       Time Rank  Score
    // 11   00:27:50    1417      0   00:33:35     800      0

    // 56595
    // 15693274740

    // 1:13 am, 3500 solved both, 3558 solved one
    // 2:20am, 6745 both, 3883 one
    // 2:41am 7525 both, 3936 one
    // 9am 20139, 8131


    /*
// need quick way to parse:
     
Monkey 4:
  Starting items: 82
  Operation: new = old + 8
  Test: divisible by 11
    If true: throw to monkey 0
    If false: throw to monkey 2
     */


    internal class Day11 : AdventOfCode
    {
        class Monkey
        {
            public List<long> items = new();
            public Func<long, long> op;
            public Func<long, bool> test;
            public int testv;
            public int FalseDest = 0;
            public int TrueDest = 0;

            public int inspections = 0;

            public void Add(long item)
            {
                items.Add(item);
            }
        }

        public override object Run(bool part2)
        {
            //return Original(part2);
            return Cleaned(part2);
        }



        // nicer, using new parsing
        object Cleaned(bool part2)
        {
            var monkeys = ReadLines().Chunk(7).Select(block =>
            {
                var b = block.ToList();

                var ops = Split(b[2]);
                MatchWords(ops,(3,"old"),(4,"[+*]"),(5,"(\\d+)|old"));

                return new Monkey
                {
                    items = Numbers64(b[1], false).Select(t => (long)t).ToList(),
                    op = (ops[5], ops[4]) switch
                    {
                        ("old", "*") => t => t * t,
                        ("old", "+") => t => t + t,
                        (_, "*") => t => t * (int)Num(ops[5]),
                        (_, "+") => t => t + (int)Num(ops[5]),
                        _ => throw new Exception()
                    },
                    testv = Numbers(b[3])[0],
                    TrueDest = Numbers(b[4])[0],
                    FalseDest = Numbers(b[5])[0]
                };
            }).ToList();

            // 9699690
            var divProd = monkeys.Aggregate(1L, (v, m1) => m1.testv * v);

            var rounds = part2 ? 10000 : 20;

            for (var round = 0; round < rounds; ++round)
            {
                //Dump(monkeys, round);
                foreach (var m in monkeys)
                {
                    // remove items, in case monkey adds to self
                    var items = m.items.ToList(); // clone it
                    m.items.Clear(); // clear these
                    m.inspections += items.Count;
                    foreach (var item in items)
                    {
                        var worry = item;
                        worry = m.op(worry);
                        if (!part2)
                            worry /= 3;
                        else
                            worry %= divProd; // part need all remainders...
                        if (worry % m.testv == 0)
                            monkeys[m.TrueDest].Add(worry);
                        else
                            monkeys[m.FalseDest].Add(worry);
                    }
                }
            }

            var inspections = monkeys.Select(m => m.inspections).ToList();
            inspections.Sort();
            return (long)inspections[^1] * inspections[^2];
        }

        void Dump(IEnumerable<Monkey> monkeys, int round)
        {
            Console.WriteLine("Round" + round);
            foreach (var k in monkeys)
            {
                Console.Write($"Monkey: ");
                foreach (var i in k.items)
                    Console.Write($"{i},");
                Console.WriteLine($"");
                Console.WriteLine($"");
            }

            Console.WriteLine();
        }


        // my original soln
        object Original (bool part2)
        {
            // prevent overflow idea is:
            //  track all divisions we are interested in, take product
            // (if this were too big, we could use GCD or Chinese Remainder Theorem to
            // still deal, or even track multiple worry remainders mod different items)
            int divv = 1; 

            long score = 0;
            var lines = ReadLines();
            Monkey m = new();
            var monkeys = new List<Monkey>();
            for (var index = 0; index < lines.Count; ++index)
            {
                var line = lines[index];
                if (line.Contains("Monkey"))
                {
                    m = new Monkey();
                    monkeys.Add(m);
                }
                else if (line.Contains("Starting"))
                {
                    m.items = Numbers(line).Select(k=>(long)k).ToList();
                }
                else if (line.Contains("Test"))
                {
                    var n = (int)Numbers(line)[0];
                    divv *= n;
                    m.test = t => (t % n) == 0;
                }
                else if (line.Contains("Operation"))
                {
                    var r = new Regex
                        (@"= (?<a>(new|old|\d+)) (?<b>(\+|\*)) (?<c>(new|old|\d+))");
                    var match = r.Match(line);
                    var op = match.Groups["b"].Value;

                    var a = match.Groups["a"].Value;
                    Trace.Assert(a == "old");
                    var c = match.Groups["c"].Value;

                    op = line.Contains("*") ? "*" : "+";

                    if (c == "old")
                    {
                        if (op == "*")
                            m.op = t => t * t;
                        else if (op == "+")
                            m.op = t => t + t;
                    }
                    else
                    {
                        var n = int.Parse(c);
                        if (op == "*")
                            m.op = t => t * n;
                        else if (op == "+")
                            m.op = t => t + n;
                    }
                }
                else if (line.Contains("true"))
                {
                    m.TrueDest = Numbers(line)[0];
                    

                }
                else if (line.Contains("false"))
                {
                    m.FalseDest= Numbers(line)[0];
                }
            }

            var rounds = part2 ? 10000 : 20;

            checked
            {
                long worry = 0;
                for (var round = 0; round < rounds; ++round)
                {
                    foreach (var monkey in monkeys)
                    {
                        var it = new List<long>();
                        it.AddRange(monkey.items);
                        monkey.items.Clear();
                        monkey.inspections += (int)it.Count;
                        foreach (var item in it)
                        {
                            worry = item;
                            worry = monkey.op(worry);
                            if (!part2)
                                worry /= 3;
                            else
                            {
                                worry %= divv;
                            }
                            Trace.Assert(worry>=0);
                            if (monkey.test(worry))
                                monkeys[monkey.TrueDest].Add(worry);
                            else
                                monkeys[monkey.FalseDest].Add(worry);
                        }
                    }

                    //Console.WriteLine("Round" + round);
                    //foreach (var k in monkeys)
                    //{
                    //    Console.Write($"Monkey: ");
                    //    foreach (var i in k.items)
                    //        Console.Write($"{i},");
                    //    Console.WriteLine($"");
                    //    Console.WriteLine($"");
                    //}
                    //
                    //Console.WriteLine();

                }
            }

            var insp = monkeys.Select(m => m.inspections).ToList();
            insp.Sort();
            insp.Reverse();

            return insp[0] * insp[1];
        }
    }
}