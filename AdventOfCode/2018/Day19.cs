

namespace Lomont.AdventOfCode._2018
{
    internal class Day19 : AdventOfCode
    {
        public class Cpu
        {
            public int[] regs = new int[6];
            public int ip => regs[ipReg];
            public int ipReg = 0; // reg index

            public void DoOp(string inst, int A, int B, int C)
            {
                if (inst == "addr") regs[C] = regs[A] + regs[B];
                else if (inst == "addi") regs[C] = regs[A] + B;
                else if (inst == "mulr") regs[C] = regs[A] * regs[B];
                else if (inst == "muli") regs[C] = regs[A] * B;
                else if (inst == "banr") regs[C] = regs[A] & regs[B];
                else if (inst == "bani") regs[C] = regs[A] & B;
                else if (inst == "borr") regs[C] = regs[A] | regs[B];
                else if (inst == "bori") regs[C] = regs[A] | B;
                else if (inst == "setr") regs[C] = regs[A];
                else if (inst == "seti") regs[C] = A;
                else if (inst == "gtir") regs[C] = A > regs[B] ? 1 : 0;
                else if (inst == "gtri") regs[C] = regs[A] > B ? 1 : 0;
                else if (inst == "gtrr") regs[C] = regs[A] > regs[B] ? 1 : 0;
                else if (inst == "eqir") regs[C] = A == regs[B] ? 1 : 0;
                else if (inst == "eqri") regs[C] = regs[A] == B ? 1 : 0;
                else if (inst == "eqrr") regs[C] = regs[A] == regs[B] ? 1 : 0;

                regs[ipReg]++;
            }
        }


        public override object Run(bool part2)
        {
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

            long ts = 0;
            while (cpu.ip < prog.Count)
            {
                var m = prog[cpu.ip];
                cpu.DoOp(m.inst, m.A, m.B, m.C);
                ++ts;
                if (part2 && ts > 1000)
                {
                    // sum of divisors of reg 2 (mine is 10551305)
                    // ans (mine) 13406472 
                    long n = cpu.regs[2];
                    long sum = 0;
                    for (int d = 1; d <= (long)Math.Sqrt(n); ++d)
                    {
                        if ((n % d) == 0)
                        {
                            var d2 = n / d;
                            sum += d;
                            if (d != d2)
                                sum += d2;
                        }
                    }

                    return sum;
                }
            }

            return cpu.regs[0];
        }
    }
}