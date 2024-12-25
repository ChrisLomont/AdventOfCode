
using System;
using System.Xml.Linq;

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


        enum Op { AND, OR, XOR }

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


            var err0 = CheckHalfAdder("x00", "y00", out var sum, out var carry);
            if (err0 !="" || sum != "z00")
            {
                if (dump)
                    Console.WriteLine($"Error in bit 0 : {err0}");
                // todo - handle this fix, leverage the debug code
                return "ERROR";
            }
            carries.Add(carry);

            // check full adders
            int bitIndex = 1;
            var bitMax = gates.Where(g => g.output.StartsWith("z")).Count();
            while (bitIndex < bitMax)
            {
                var xName = bitIndex < 10 ? $"x0{bitIndex}" : $"x{bitIndex}";
                var yName = bitIndex < 10 ? $"y0{bitIndex}" : $"y{bitIndex}";
                    var zName = bitIndex < 10 ? $"z0{bitIndex}" : $"z{bitIndex}";

                var err = CheckFullAdder(bitIndex, xName, yName, out sum, out carry);
                if (err != "" || sum != zName)
                {
                    if (dump)
                        Console.WriteLine($"Error index {bitIndex}: {err}");
                    if (!Debug(bitIndex, dump, xName, yName, zName))
                        return "FAILED";
                    if (swapNames.Count == 8)
                    {
                        swapNames.Sort();
                        var ans = swapNames.Aggregate("", (a, b) => a + "," + b);
                        return ans[1..];
                    }

                    // fixed names
                    CheckFullAdder(bitIndex, xName, yName, out sum, out carry);
                }

                carries.Add(carry);

                bitIndex++;
            }
            return "END";
        }


        bool Debug(int bitIndex, bool dump, string xName, string yName, string zName)
        {
            debug.Clear();
            isDebug = true;
            // gather registers
            CheckFullAdder(bitIndex, xName, yName, out var ss, out var cc);
            isDebug = false;

            debug = debug.OrderBy(g => g.index).ToList();

            if (dump)
            {
                foreach (var g in debug)
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
                if (CheckFullAdder(bitIndex, xName, yName, out ss, out cc) == ""  &&  ss == zName)
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

        // return error message
        string CheckHalfAdder(string inA, string inB, out string sum, out string carry)
        {
            /* half adder
            sum = A^B
            carry = A&B
            */

            sum = "";
            carry = "";
            var inputsA = All(inA, true);
            var inputsB = All(inB, true);
            if (   inputsA.Count != 2 
                || inputsB.Count != 2
                || inputsA[0].index != inputsB[0].index
                || inputsA[1].index != inputsB[1].index
               )
                return "mixed up inputs";

            for (var i = 0; i < 2; ++i)
            {
                var op = inputsA[i].op;
                if (op == Op.AND)
                    carry = inputsA[i].output;

                if (op == Op.XOR)
                    sum = inputsA[i].output;
            }

            if (sum=="" || carry=="")
                return "sum or carry bad";

            return "";
        }

        // return error msg
        string CheckFullAdder(int bitIndex, string xName, string yName, out string sum, out string carry)
        {
            sum = "";
            carry = "";

            var e1 = CheckHalfAdder(xName, yName, out var aXORb, out var c1);
            if (e1 != "") return e1;

            var carryM1 = carries[bitIndex - 1];
            var e2 = CheckHalfAdder(aXORb, carryM1, out sum, out var c2);
            if (e2 != "") return e2;

            var ci = gates.Where(g => Inputs(g, c1, c2) && g.op == Op.OR).ToList();
            if (isDebug)
                debug.AddRange(ci);

            if (ci.Count != 1)
                return "bad paired carry";

            carry = ci[0].output;
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