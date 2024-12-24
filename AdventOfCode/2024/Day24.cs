
namespace Lomont.AdventOfCode._2024

{
    /*
    //  24   00:10:55    422      0   02:35:09   1329      0
2024 Day 24 part 1: 55544677167336 in 13302.5 us
2024 Day 24 part 2: gsd,kth,qnf,tbt,vpm,z12,z26,z32 in 14184.9 us    
    */

    internal class Day24 : AdventOfCode
    {

        //public override string TestFileSuffix() => "-test1";
        public override string TestFileSuffix() => "";

        object Run1()
        {
            Dictionary<string, int> wires = new();
            List<(string in1, string gate, string in2, string outName)> gates =
                new List<(string, string, string, string)>();
            foreach (var line in ReadLines())
            {
                if (line.Contains(':'))
                {
                    var w = line.Split(':');
                    wires.Add(w[0], Int32.Parse(w[1].Trim()));

                }
                else if (line.Contains("->"))
                {
                    var w = line.Split(" ");
                    var n1 = w[0];
                    var g = w[1];
                    var n2 = w[2];
                    var ot = w[4];
                    gates.Add((n1, g, n2, ot));
                }
            }



            while (true)
            {
                var seen = false;
                foreach (var g in gates)
                {
                    if (wires.ContainsKey(g.outName)) continue;
                    if (wires.ContainsKey(g.in1) && wires.ContainsKey(g.in2))
                    {
                        var a = wires[g.in1];
                        var b = wires[g.in2];
                        var c = g.gate switch
                        {
                            "AND" => a & b,
                            "OR" => a | b,
                            "XOR" => a ^ b,
                            _ => throw new NotFiniteNumberException()
                        };
                        wires.Add(g.outName, c);
                        seen = true;
                    }
                }

                if (!seen) break;
            }

            var zz = wires.Where(w => w.Key.StartsWith("z")).ToList();
            zz = zz.OrderBy(p => p.Key).ToList();
            var vals = zz.Select(p => p.Value).ToList();
            vals.Reverse();
            long val = 0;
            foreach (var v in vals)
            {
                val = 2 * val + v;
            }

            return val;

        }


        enum Op
        {
            AND,
            OR,
            XOR
        }

        record Gate(string in1, Op op, string in2, string output, int index);

        List<Gate> gates = new();
        List<string> carries = new(); // track names of carry pins
        List<string> swapNames = new(); // names of gates to swap
        bool isDebug = true; // log gates touched
        List<Gate> debug = new(); // loggged gates

        object Run2()
        {
            Dictionary<string, int> wires = new();
            foreach (var line in ReadLines())
            {
                if (line.Contains(':'))
                {
                    var w = line.Split(':');
                    wires.Add(w[0], Int32.Parse(w[1].Trim()));

                }
                else if (line.Contains("->"))
                {
                    var w = line.Split(" ");
                    var n1 = w[0];
                    Enum.TryParse<Op>(w[1], out var op);
                    var n2 = w[2];
                    var ot = w[4];
                    gates.Add(new(n1, op, n2, ot, gates.Count));
                }
            }

            bool dump = false;

            // idea: walk the circuit, checking the first half adder and rest are adders
            // when something is not right
            //   - log all the touched gates and wires
            //   - swap them in pairs and check again


            if (!CheckHalfAdder())
            {
                if (dump)
                    Console.WriteLine("Error in x0,y0,z0");
                // todo - handle this fix, leverage the debug code
                return "ERROR";
            }

            // check full adders
            int bitIndex = 1;
            var bitMax = gates.Where(g => g.output.StartsWith("z")).Count();
            while (bitIndex < bitMax)
            {
                var err = CheckAdder(bitIndex);
                if (err != "")
                {
                    if (dump)
                        Console.WriteLine($"Error index {bitIndex}: {err}");
                    if (!Debug(bitIndex, dump))
                        return "FAILED";
                    if (swapNames.Count == 8)
                    {
                        swapNames.Sort();
                        var ans = swapNames.Aggregate("", (a, b) => a + "," + b);
                        return ans[1..];
                    }
                }

                bitIndex++;
            }


            return "END";
        }


        bool Debug(int bitIndex, bool dump)
        {
            debug.Clear();
            isDebug = true;
            CheckAdder(bitIndex);
            isDebug = false;

            debug.DistinctBy(g => g.index);
            debug = debug.OrderBy(g => g.index).ToList();

            foreach (var g in debug)
            {
                if (dump)

                    Console.WriteLine($" - {g}");
            }

            // get wire names
            var names = debug.Select(g => g.in1).ToList();
            names.AddRange(debug.Select(g => g.in2));
            names.AddRange(debug.Select(g => g.output));
            names = names.Distinct().ToList();

            // try each combo swap
            for (int i = 0; i < names.Count; i++)
            for (int j = i + 1; j < names.Count; j++)
            {
                var (n1, n2) = (names[i], names[j]);
                Swap(n1, n2);
                if (CheckAdder(bitIndex) == "")
                {
                    if (dump)
                        Console.WriteLine($"Fixed {n1},{n2}");
                    swapNames.Add(n1);
                    swapNames.Add(n2);
                    return true;
                }

                Swap(n1, n2); // restore
            }

            return false;
        }

        // global swap names
        void Swap(string n1, string n2)
        {
            var tn = "boogers";
            Replace(n1, tn);
            Replace(n2, n1);
            Replace(tn, n2);
        }

        // global replace name src with dst on outputs
        void Replace(string src, string dst)
        {
            for (int i = 0; i < gates.Count; ++i)
            {
                var g = gates[i];
                if (g.output == src)
                    gates[i] = g with { output = dst };

            }
        }

        // get all gates, order by gate index
        List<Gate> All(string name, bool input)
        {
            List<Gate> ans = new();
            if (!input) ans.AddRange(gates.Where(g => g.output == name));
            else
            {
                ans.AddRange(gates.Where(g => g.in1 == name));
                ans.AddRange(gates.Where(g => g.in2 == name));
            }

            ans = ans.OrderBy(g => g.index).ToList();
            if (isDebug)
                debug.AddRange(ans);

            return ans;
        }

        bool CheckHalfAdder()
        {
            /* half adder
           z0 = x0^y0
           c0 = x0&y0
          */
            var x0 = All("x00", true);
            var y0 = All("y00", true);
            var z0 = All("z00", false);
            if (x0.Count != 2 || y0.Count != 2 || z0.Count != 1
                || x0[0].index != y0[0].index
                || x0[1].index != y0[1].index
               )
            {
                Console.WriteLine("Error on x0,y0,z0");
                return false;
            }

            bool andOk = false, xorOk = false;
            for (int i = 0; i < 2; ++i)
            {
                var op = x0[i].op;
                if (op == Op.AND && x0[i].output != "z00")
                {
                    andOk = true;
                    carries.Add(x0[i].output);
                }

                if (op == Op.XOR && x0[i].output == "z00")
                {
                    xorOk = true;
                }
            }

            if (!andOk || !xorOk)
            {
                Console.WriteLine("ERROR x0,y0,z0");
                return false;
            }

            return true;
        }

        // return error msg
        string CheckAdder(int bitIndex)
        {
            /* full adder
             sum_i = xi ^ yi
             c1_i  = xi & yi
             c2_i = sum_i & c_(i-1)

             zi = c_(i-1) ^ sum_i

             c_i = c1_i ^ c2_i
             */


            /*
             check
             sum_i = xi ^ yi
             c1_i  = xi & yi
             */
            var cm1Name = carries[bitIndex - 1];
            var xName = bitIndex < 10 ? $"x0{bitIndex}" : $"x{bitIndex}";
            var yName = bitIndex < 10 ? $"y0{bitIndex}" : $"y{bitIndex}";
            var zName = bitIndex < 10 ? $"z0{bitIndex}" : $"z{bitIndex}";

            var xi = All(xName, true);
            var yi = All(yName, true);
            var zi = All(zName, false);
            var cIn = All(cm1Name, true);

            if (xi.Count != 2 || yi.Count != 2 || zi.Count != 1
                || xi[0].index != yi[0].index
                || xi[1].index != yi[1].index
                || cIn.Count != 2
               )
            {
                return "Error on xi,yi,zi";
            }

            Gate? sum_i = null, c1_i = null;
            for (int i = 0; i < 2; ++i)
            {
                var op = xi[i].op;
                if (op == Op.AND && xi[i].output != zName)
                {
                    c1_i = xi[i];
                }

                if (op == Op.XOR && xi[i].output != zName)
                {
                    sum_i = xi[i];
                }
            }

            if (sum_i == null || c1_i == null)
            {
                return "sum_i, c1_i";
            }

            /*
             check
             c2_i = sum_i & c_(i-1)
             zi = c_(i-1) ^ sum_i
             */

            Gate? z_i = null, c2_i = null;
            for (int i = 0; i < 2; ++i)
            {
                var g = cIn[i];
                var op = g.op;
                if (op == Op.AND && Inputs(g, sum_i.output, cm1Name))
                {
                    c2_i = g;
                }

                if (op == Op.XOR && Inputs(g, sum_i.output, cm1Name) && g.output == zName)
                {
                    z_i = g;
                }
            }

            if (z_i == null || c2_i == null)
            {
                return "z_i, c2_i";
            }

            /* check
              c_i = c1_i ^ c2_i
            */

            var ci = gates.Where(
                g => Inputs(g, c1_i.output, c2_i.output) && g.op == Op.OR
            ).ToList();
            if (isDebug)
                debug.AddRange(ci);

            if (ci.Count != 1)
            {
                return "ci";
            }

            carries.Add(ci[0].output);

            return "";
        }

        bool Inputs(Gate g, string n1, string n2)
        {
            if (g.in1 == n1 && g.in2 == n2) return true;
            if (g.in1 == n2 && g.in2 == n1) return true;
            return false;
        }


        public override object Run(bool part2)
        {
            return part2 ? Run2() : Run1();
        }
    }
}