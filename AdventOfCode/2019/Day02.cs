namespace Lomont.AdventOfCode._2019
{
    internal class Day02 : AdventOfCode
    {
        // 7210630
        // 3892

        public static Dictionary<long, long> RunIntCode(List<long> prog, List<int>? input = null, Action<long>?output =null)
        {
            int inputIndex = 0;

            //Console.WriteLine("TODO: fix input");
            return RunIntCode(prog, () => 
            {
                //return 1;
                return input[inputIndex++];
            },
                output);
        }

        public static Dictionary<long,long> RunIntCode(List<long> prog, Func<int> getInput, Action<long>?output = null)
        {
            checked
            {
                output ??= Console.WriteLine;
                // nice python interpreter https://raw.githubusercontent.com/fogleman/AdventOfCode2019/master/09.py
                // fix 203 error https://www.reddit.com/r/adventofcode/comments/e8aw9j/2019_day_9_part_1_how_to_fix_203_error/

                // problem 25 added more complex input, list moved to calling function
                // changed all inputs to long

                // problem 25 changed memory to dictionary for large accesses
                Dictionary<long, long> memory = new();

                for (var addr = 0; addr < prog.Count; addr++)
                    memory.Add(addr, prog[addr]);

                //var extra = numIn.Count * 1000; // problem 25 changed * 4 to * 1000
                //var nums = new List<long>(); // problem 9 added big numbers
                //nums.AddRange(numIn);
                //while (nums.Count < extra)
                //    nums.Add(0);

                var comp = new Day25.Computer(prog);


                int pc = 0;

                long relativeBase = 0; // added problem #9

                long ReadLow(long addr)
                {
                    return memory.ContainsKey(addr) ? memory[addr] : 0;
                }

                void WriteLow(long addr, long val)
                {
                    if (!memory.ContainsKey(addr))
                        memory.Add(addr, 0);
                    memory[addr] = val;
                }

                long Read(long mode)
                {
                    var val = ReadLow(pc++);
                    if (mode == 0)
                        return ReadLow(val);
                    else if (mode == 1) // immediate mode 1 added problem 5
                        return val;
                    else if (mode == 2) // relative mode 2 added problem 9
                        return ReadLow(val + relativeBase);

                    throw new Exception();
                }

                void Write(long val, long mode)
                {
                    var addr = ReadLow(pc++);
                    if (mode == 0)
                        WriteLow(addr, val);
                    else if (mode == 2)
                        WriteLow(addr + relativeBase, val);
                    else
                        throw new Exception();
                }

                void Compare()
                {
                    //Console.WriteLine("PC: "+pc);
                    var c = comp;
                    Trace.Assert(pc == comp.Position);
                    
                    foreach (var op in comp.Program)
                        Trace.Assert(memory[op.Key]==op.Value);
                    foreach (var op in comp.Program)
                        Trace.Assert(memory[op.Key] == op.Value);

                }

                bool done = false;
                while (!done)
                {
                    Compare();
                    comp.Iterate();
                    var inst = ReadLow(pc++);

                    //memory[1000] = 1;


                    Debug.Write($"{pc-1}:{inst%100}, ");

                    // problem 5 added parameter Mode
                    var mode1 = (inst / 100) % 10;
                    var mode2 = (inst / 1000) % 10;
                    var mode3 = (inst / 10000) % 10;
                    inst %= 100;

                    long x, y;
                    switch (inst)
                    {
                        case 1: // Add
                            Write(Read(mode1) + Read(mode2), mode3);
                            break;
                        case 2: // Mul
                            Write(Read(mode1) * Read(mode2), mode3);
                            break;
                        case 3: // Input - added problem #5 
                            Write(getInput(), mode1);
                            break;
                        case 4: // Output - added problem #5 
                            output(Read(mode1));
                            break;
                        case 5: // jump true - added problem #5 
                            x = Read(mode1);
                            y = Read(mode2);
                            if (x != 0) pc = (int)y;
                            break;
                        case 6: // jump false - added problem #5 
                            x = Read(mode1);
                            y = Read(mode2);
                            if (x == 0) pc = (int)y;
                            break;
                        case 7: // less than - added problem #5 
                            Write(Read(mode1) < Read(mode2) ? 1 : 0, mode3);
                            break;
                        case 8: // equals - added problem #5 
                            Write(Read(mode1) == Read(mode2) ? 1 : 0, mode3);
                            break;
                        case 9: // relative base - added problem #9
                            relativeBase += Read(mode1);
                            break;

                        case 99: // halt
                            done = true;
                            break;
                        default:
                            throw new Exception($"IntCode unknown inst {inst}");
                    }
                }
                return memory;
            }
        }
        public override object Run(bool part2)
        {
            var lines = ReadLines();
            if (!part2)
            {
                var prog = Numbers64(lines[0]);
                prog[1] = 12;
                prog[2] = 2;
                var mem = RunIntCode(prog);
                return mem[0];
            }
            for (var noun =0; noun <= 99; ++noun)
            for (var verb = 0; verb <= 99; ++verb)
            {
                var prog = Numbers64(lines[0]);
                prog[1] = noun;
                prog[2] = verb;
                var mem = RunIntCode(prog);
                if (mem[0] == 19690720)
                {
                    return 100 * noun + verb; // 3892 
                }
            }

            throw new Exception();
        }
    }
}