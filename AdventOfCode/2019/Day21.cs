namespace Lomont.AdventOfCode._2019
{
    internal class Day21 : AdventOfCode
    {
    //2019 Day 21 part 1: 19362259 in 21505.3 us
    //2019 Day 21 part 2: 1141066762 in 154368.8 us
        public override object Run(bool part2)
        {
            // 15 inst max
            // regs Temp, Jump
            // A = 1, B-D 2,3,4, true = ground
            
            // AND X Y  X&Y->Y
            // OR X Y X|Y->Y
            // NOT X Y -> Y=!X
            // Y = T,J
            // X = ABCDTJ

            var prog = Numbers64(ReadLines()[0]);

            return Succeeds(new List<string>(),prog, part2);

#if false
            // evolve prog:
            IEnumerable<string> Insts()
            {
                var ops = new[] { "AND", "OR", "NOT" };
                var src = "ABCDTJ";
                var dst = "TJ";
                foreach (var op in ops)
                    foreach (var s in src)
                    foreach (var d in dst)
                        yield return $"{op} {s} {d}";
            }
            foreach (var i in Insts())
                Console.WriteLine(i);
            Console.WriteLine(Insts().ToList().Count); // 36, too big to brute force
#endif



          
            return - 1234;

            static object Succeeds(List<string> ops, List<long> prog, bool part2)
            {
                var output = new Queue<long>();
                var input = new Queue<long>();
                var comp = new Day02.IntCode(prog, input.Dequeue, output.Enqueue);

                if (!part2)
                {
                    // jump is 3, need landing place
                    // if 3 long hole, jump at edge: A|B|C = 0 then jump
                    // if 1,2 hole, jump so can land: 

                    //foreach (var op in ops)
                    //{
                    //    Add(op);
                    //}

                    //                Add("OR A T");
#if false
            // if 3 hole, jump at A has hole
            Add("OR A T");
            Add("AND A T"); // T = A
            Add("OR B T");
            Add("OR C T");
            Add("NOT T T"); // 1 if hole at A,B,C

            Add("OR T J");
            Add("AND T J"); // J = T





            Add("OR A T");
            Add("AND A T"); // T = A
            Add("AND B T"); // T = A&B
            Add("NOT T T"); // T = !(A&B)
            Add("AND D T"); // T = !(A&B)|D
            
            Add("OR T J");
#endif
                    // jump immediately
                    //Add("OR T J");
                    //Add("NOT T T");
                    //Add("OR T J");

                    // Any hole A,B,C and D ground, jump
                    // (A&B&C) = 0 if any hole A,B,C
                    // !(A&B&C) & D -> J

                    Add("OR A T");
                    Add("AND A T"); // T = A
                    Add("AND B T"); // T = A&B
                    Add("AND C T"); // T = A&B&C
                    Add("NOT T T"); // T = !(A&B&C)
                    Add("AND D T"); // T = !(A&B&C) & D

                    Add("OR T J");
                    Add("AND T J"); // J = T



                }
                else
                { // part2: add dist regs EFGHI = 5,6,7,8,9, use "RUN"

                    // Test jump - same size
                    //Add("OR T J");
                    //Add("NOT T T");
                    //Add("OR T J");

                    // jump if hole B,C, can land at D and H, else don't
                    
                    Add("NOT B J");
                    Add("NOT C T");
                    Add("OR T J");
                    Add("AND D J");
                    Add("AND H J");

                    // if hole at A, must jump
                    Add("NOT A T");
                    Add("OR T J");



                }

                Add(part2 ? "RUN" : "WALK"); // start

                void Add(string line)
                {
                    line += "\n";
                    foreach (var c in Encoding.ASCII.GetBytes(line))
                        input.Enqueue(c);

                }

                bool show = false;
                long ans = 0;
                while (!comp.Step())
                {
                    if (output.Any())
                    {
                        var v = output.Dequeue();
                        if (v < 255)
                        {
                            if (show)
                                Console.Write((char)v);
                        }
                        else
                            ans = v;
                    }
                }

                return ans;

            }

        }
    }
}