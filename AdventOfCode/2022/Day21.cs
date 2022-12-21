namespace Lomont.AdventOfCode._2022
{
    internal class Day21 : AdventOfCode
    {
        // top 1-100  5:24 - 16:15 both parts
        //  part1 1:18 - 4:28

        // 2343, 2186 solved both, one by 1:16 am

        // 3219579395609 from mathematica :)

        //2022 Day 21 part 1: 291425799367130 in 15907.2 us
        //2022 Day 21 part 2: 3219579395609 in 8106.7 us

        // --------Part 1---------   --------Part 2---------
        //     Day Time    Rank Score       Time Rank  Score
        // 21   00:07:47     455      0   00:40:26    1041      0
        // 20   00:24:00     278      0   01:02:21    1195      0
        class Rat
        {
            public long num, den;

            long gcd(long a, long b)
            {
                while (b != 0)
                    (a, b) = (b, a % b);
                return a;
            }

            public Rat(long n=0, long d=1)
            {
                var g = gcd(n,d);
                num = n/g;
                den = d/g;
            }

            public override string ToString()
            {
                return $"[{num}/{den}]";
            }

            public static Rat operator +(Rat lhs, Rat rhs)
            {
                checked
                {
                    return new Rat(lhs.num * rhs.den + lhs.den * rhs.num, lhs.den * rhs.den);
                }
            }
            public static Rat operator -(Rat lhs, Rat rhs)
            {
                checked
                {
                    return new Rat(lhs.num * rhs.den - lhs.den * rhs.num, lhs.den * rhs.den);
                }
            }
            public static Rat operator *(Rat lhs, Rat rhs)
            {
                checked
                {
                    return new Rat(lhs.num * rhs.num, lhs.den * rhs.den);
                }
            }
            public static Rat operator /(Rat lhs, Rat rhs)
            {
                checked
                {
                    return new Rat(lhs.num * rhs.den, lhs.den * rhs.num);
                }
            }

        }

        class poly : List<Rat>
        {
            public override string ToString()
            {
                var s=new StringBuilder();
                for (var i = this.Count-1; i >= 0;--i)
                {
                    var v = base[i];
                    s.Append($"{v}, ");
                }

                return s.ToString();
            }

            void Size(int len)
            {
                while (this.Count <= len)
                    this.Add(new Rat());
            }
            public Rat this[int index]
            {
                get { 
                    Size(index);
                    return base[index];
                }
                set
                {
                    Size(index);
                    base[index] = value;
                }
            }
            public static poly operator +(poly lhs, poly rhs)
            {
                var p = new poly();
                for (var i = 0; i < Math.Max(lhs.Count,rhs.Count); ++i)
                    p[i] = lhs[i] + rhs[i];
                return p;
            }
            public static poly operator -(poly lhs, poly rhs)
            {
                var p = new poly();
                for (var i = 0; i < Math.Max(lhs.Count, rhs.Count); ++i)
                    p[i] = lhs[i] - rhs[i];
                return p;
            }
            public static poly operator *(poly lhs, poly rhs)
            {
                var p = new poly();
                for (var i = 0; i < lhs.Count; ++i)
                for (var j = 0; j < rhs.Count; ++j)
                {
                    p[i + j] += lhs[i] * rhs[j];
                }

                return p;
            }

            public static poly operator /(poly lhs, poly rhs)
            {
                Trace.Assert(rhs.Count == 1);
                var p = new poly();
                for (var i = 0; i < lhs.Count; ++i)
                {
                    //Trace.Assert((lhs[i] % rhs[0]) == 0);
                    p[i] = lhs[i] / rhs[0];
                }

                return p;
            }

        }
        class Node
        {
            public string name;
            public poly? poly = null;
            public string op;
            public Node Ch1, Ch2;
            public string ch1, ch2;
        }


        public object Run2()
        {
            long score = 0;
            var lines = ReadLines();
            var name = new Dictionary<string, Node>();
            var res = new Dictionary<string, long>();
            for (var pass = 0; pass < 2; ++pass)
            for (var index = 0; index < lines.Count; ++index)
            {
                var line = lines[index];
                var key = line[0..4];
                if (pass == 0)
                {
                    var n = new Node { name = key };
                    name.Add(key, n);
                }
                else
                {
                    var rest = line[5..].Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    if (rest.Length == 1)
                    {
                        var poly = new poly();
                        poly[0] = new Rat(Int64.Parse(rest[0]));
                        name[key].poly = poly;
                    }
                    else
                    {
                        var n = name[key];
                        n.ch1 = rest[0];
                        n.op = rest[1];
                        n.ch2 = rest[2];
                        n.Ch1 = name[rest[0]];
                        n.Ch2 = name[rest[2]];
                    }
                }
            }

            var hp = new poly();
            hp[1] = new Rat(1,1); // linear in variable human
            name["humn"].poly = hp;
            name["root"].op = "=";

            // reduce expression
            bool done = false;
            var red = new HashSet<string>();
            foreach (var n in name)
                if (n.Value.poly != null)
                    red.Add(n.Value.name);

            while (!done)
            {
                done = true;
                foreach (var p in name)
                {
                    var n = p.Value;
                    if (red.Contains(n.name)) continue;

                    if (red.Contains(n.ch1) && red.Contains(n.ch2))
                    {
                        red.Add(n.name);
                        done = false;
                       // Console.WriteLine($"RED2 {n.name}");
                        if (n.op == "=")
                        {
                            return Solve(n.Ch1.poly, n.Ch2.poly);
                            Dump1(n.Ch1);
                            Console.Write("==");
                            Dump1(n.Ch2);
                            Console.WriteLine();
                            return -1213;

                        }
                        n.poly = n.op switch
                        {
                            "+" => n.Ch1.poly +  n.Ch2.poly,
                            "-" => n.Ch1.poly -  n.Ch2.poly,
                            "/" => n.Ch1.poly /  n.Ch2.poly,
                            "*" => n.Ch1.poly *  n.Ch2.poly,
                            _ => throw new Exception(),
                        };
                    }
                }

            }

            long Solve(poly lhs, poly rhs)
            {
                //Console.WriteLine($"{lhs} == {rhs}");
                if (rhs.Count > lhs.Count)
                    (lhs, rhs) = (rhs, lhs);
                Trace.Assert(lhs.Count==2 && rhs.Count == 1); // linear eqn
                var con = new poly();
                con[0] = lhs[0];
                lhs -= con;
                rhs -= con;
                //Console.WriteLine($"{lhs} == {rhs}");

                var a1 = new poly();
                a1[0] = lhs[1];
                lhs /= a1;
                rhs /= a1;
                //Console.WriteLine($"{lhs} == {rhs}");

                var r0 = rhs[0];
                Trace.Assert(r0.den == 1);

                return r0.num;

            }


            Console.WriteLine($"{name.Count - red.Count} nodes left");

//            Dump1(name["root"]);

            void Dump1(Node nm)
            {
                if (red.Contains(nm.name))
                    Console.Write(nm.poly);
                else if (nm.name=="humn")
                    Console.Write(nm.name);
                else
                {
                    Console.Write("(");
                    Dump1(nm.Ch1);
                    Console.Write($"){nm.op}(");
                    Dump1(nm.Ch2);
                    Console.Write($")");
                }
            }


            return -1212;
        }

        public override object Run(bool part2)
        {
            if (part2) return Run2();
            long score = 0;
            var lines = ReadLines();
            var name = new Dictionary<string, List<string>>();
            var res = new Dictionary<string, long>();
            for (var index = 0; index < lines.Count; ++index)
            {
                var line = lines[index];
                var key = line[0..4];
                var rest = line[5..].Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (rest.Length == 1)
                    res.Add(key, Int64.Parse(rest[0]));
                else
                    name.Add(key,rest.ToList());
            }

            if (part2)
            {
                var t = name["root"];
                t[1] = "=";
            }

            var r2 = new Dictionary<string, long>();
            foreach (var p in res)
                r2.Add(p.Key, p.Value);

            var humanT = "humn";

            if (!part2) 
                return Sample(0);


            //for (var i =0; i <= 10_000; ++i)
            //    if (Sample(i) == 1)
            //        return i;
            Sample(1023);
            return -12345;

            long Sample(int human)
            {
                // restore
                res.Clear();
                foreach (var p in r2)
                    res.Add(p.Key, p.Value);

                if (part2)
                {
                    res["humn"] = human;

                    res.Remove(humanT);
                }

                var done = false; 
                while (!done)
                {
                    done = true;
                    foreach (var p in name)
                    {
                        if (part2 && p.Key == "humn")
                        {
                            done = false;
                            continue;
                        }
                        if (res.ContainsKey(p.Key))
                            continue;
                        var p1 = p.Value[0];
                        var op = p.Value[1];
                        var p2 = p.Value[2];
                        if (res.ContainsKey(p1) && res.ContainsKey(p2))
                        {
                           // Console.WriteLine($"Resolve {p.Key}, {res.Count}/{name.Count}");
                            done = false;

                            var val = op switch
                            {
                                "=" => res[p1] == res[p2] ? 1 : 0,
                                "+" => res[p1] + res[p2],
                                "-" => res[p1] - res[p2],
                                "/" => res[p1] / res[p2],
                                "*" => res[p1] * res[p2],
                                _ => throw new Exception(),
                            };
                            res.Add(p.Key, val);
                        }
                    }
                }
                return res["root"];
            }

            if (!part2)
                return res["root"];
            
            return res["root"];




            return score;
        }
    }
}