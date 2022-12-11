namespace Lomont.AdventOfCode._2020
{
    internal class Day08 : AdventOfCode
    {
    //2020 Day 8 part 1: 1939 in 818 us
    //2020 Day 8 part 2: 2212 in 19671.3 us
        public override object Run(bool part2)
        {
            var lines = ReadLines();
            if (!part2) 
                return RunOne(lines).val;

            // try each in turn
            int changed = 0;
            while (changed < lines.Count)
            {
                var line = lines[changed];
                if (line.StartsWith("nop"))
                {
                    lines[changed]="jmp"+line[4..];
                    var (inf,acc) = RunOne(lines);
                    if (!inf) return acc;
                    lines[changed] = line;
                }
                else if (line.StartsWith("jmp"))
                {
                    lines[changed] = "nop" + line[4..];
                    var (inf, acc) = RunOne(lines);
                    if (!inf) return acc;
                    lines[changed] = line;
                }

                ++changed;
            }

            throw new Exception();

            (bool inf, int val) RunOne(List<string> lines)
            {

                var pc = 0;
                var acc = 0;
                int[] count = new int[lines.Count];
                while (pc < lines.Count)
                {
                    var line = lines[pc];
                    if (count[pc] > 0)
                    {
                        return (true,acc);
                    }

                    count[pc]++;
                    pc++;
                    var op = line[0..3];
                    var n = Numbers(line)[0];
                    if (op == "acc")
                        acc += n;
                    else if (op == "nop")
                    {

                    }
                    else if (op == "jmp")
                    {
                        pc--;
                        pc += n;
                    }
                }
                return (false,acc);
            }
        }
    }
}