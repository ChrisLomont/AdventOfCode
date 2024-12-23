

using static Lomont.AdventOfCode._2018.Day19;

namespace Lomont.AdventOfCode._2018
{
    internal class Day21 : AdventOfCode
    {
        /*
2018 Day 21 part 1: 1797184 in 4872.8 us
2018 Day 21 part 2: 11011493 in 24245275.7 us
        */

        object Run2()
        {
            bool part2 = false;
            var cpu = new Cpu();
            var prog = new List<(string inst, int A, int B, int C)>();
            foreach (var line in ReadLines())
            {
                var nums = Numbers(line);
                var opc = line.Split(' ')[0];
                if (line.StartsWith("#ip"))
                    cpu.ipReg = nums[0];
                else
                    prog.Add((opc, nums[0], nums[1], nums[2]));
            }

            cpu.regs[0] = part2 ? 1 : 0;

            // r0 accessed only time via "eqrr 1 0 5"
            // compare to r1
            // intercept and set r0 = r1

            HashSet<string> seen = new();

            long ts = 0;
            long lastr1 = 0;
            while (cpu.ip < prog.Count)
            {
                var m = prog[cpu.ip];
                if (m.inst == "eqrr")
                {
                    //var hash = cpu.regs.Aggregate("", (a, b) => a + "," + b.ToString());
                    var hash = cpu.regs[1].ToString();
                    if (seen.Contains(hash))
                    {
//                        Console.WriteLine($"{ts}: {cpu.regs[1]} {lastr1}");
                        return lastr1;
                    }

                    lastr1 = cpu.regs[1];
                    seen.Add(hash);
                }

                cpu.DoOp(m.inst, m.A, m.B, m.C);
                ++ts;
            }

            return "ERR";
        }

        object Run1()
        {
            bool part2 = false;
            var cpu = new Cpu();
            var prog = new List<(string inst, int A, int B, int C)>();
            foreach (var line in ReadLines())
            {
                var nums = Numbers(line);
                var opc = line.Split(' ')[0];
                if (line.StartsWith("#ip"))
                    cpu.ipReg = nums[0];
                else
                    prog.Add((opc, nums[0], nums[1], nums[2]));
            }

            cpu.regs[0] = part2 ? 1 : 0;

            // r0 accessed only time via "eqrr 1 0 5"
            // compare to r1
            // intercept and set r0 = r1

            long ts = 0;
            while (cpu.ip < prog.Count)
            {
                var m = prog[cpu.ip];
                if (m.inst == "eqrr")
                    return cpu.regs[1];
                cpu.DoOp(m.inst, m.A, m.B, m.C);
                ++ts;
            }

            return "ERR";
        }

        public override object Run(bool part2)
        {
            return part2 ? Run2() : Run1();
        }
    }
}