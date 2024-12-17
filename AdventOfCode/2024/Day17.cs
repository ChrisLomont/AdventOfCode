

namespace Lomont.AdventOfCode._2024
{
    internal class Day17 : AdventOfCode
    {
        // 2,1,0,1,7,2,5,0,3

        /*
2024 Day 17 part 1: 2,1,0,1,7,2,5,0,3 in 6080.4 us
2024 Day 17 part 2: 267265166222235 in 1909384 us

         */



        object Run2()
        {
            /* dumped program is
                0: B=A&7
                2: B^=7
                4: C = A/(1<<B)
                6: A /= 1<<3
                8: B ^= C
                10: B^=7
                12: output B&7
                14: if (A!=0) goto 0         

            this is:
                
                // read off an octal number from A, invert
                B=(A&7)^7  
                A/=8

                // messy part, mixes values
                B ^= (A/2**B)&7  // everything octal in effect

                output B&7 

                if (A != 0) goto start // loop till A gone

            // so, loops over 7 bit digits of A, so idea is to 

            A = 0, n=1
            while !done
               A*=8; // next digit in A
               while last n digits dont match
                  A++;
               incr n



             */

            var lines = ReadLines();
            ulong A = (ulong)Numbers(lines[0])[0];
            ulong B = (ulong)Numbers(lines[1])[0];
            ulong C = (ulong)Numbers(lines[2])[0];
            var mem = Numbers(lines[4]).ToArray();

            //var target = mem.Aggregate("", (a, b) => a + (a != "" ? "," : "") + b);
            //Console.WriteLine($"Target {target}");

            ulong valA = 0;
            int pos = 0;

            var c = new comp { mem = mem };

            while (pos < 16)
            {
                valA *= 8;
                while (true)
                {
                    c.Run(valA,B,C);

                    // match end pos chars
                    if (MatchN(pos,c.output,mem))
                        break;
                    ++valA;
                }

                //Console.WriteLine($"{pos}:{-1} -> {valA} => {c.Ans()}");
                ++pos;
            }
            return valA;

            bool MatchN(int n, List<int> c, int[] t)
            {
                if (c.Count <= n) return false;
                for (int i = 0; i <= n; ++i)
                    if (c[c.Count-1-i] != t[t.Length-1-i]) return false;

                return true;
            }


        }
        //public override string TestFileSuffix() => "-test1";
        public override string TestFileSuffix() => "";

        object Run1()
        {
            var c = new comp();
            var lines = ReadLines();
            c.mem = Numbers(lines[4]).ToArray();
            c.Run(
                (ulong)Numbers(lines[0])[0],
                (ulong)Numbers(lines[1])[0],
                (ulong)Numbers(lines[2])[0]
            );

            return c.Ans();
        }

        class comp
        {
            public int ip = 0;
            public ulong A=0, B=0, C=0;
            public int [] mem;
            public List<int> output = new();

            public bool dump = false;

            void Dbg(string msg)
            {
                if (dump)
                Console.WriteLine($"{ip-2}: {msg}");
            }

            public string Ans()=> output.Aggregate("", (a, b) => a + (a != "" ? "," : "") + b);
            public void Run(ulong a, ulong b, ulong c)
            {
                output.Clear();
                A = a;
                B = b;
                C = c;
                ip = 0;
                while (exec())
                {
                }
            }

            public void Disasm()
            {
                dump = true;
                ip = 0;
                var (a,b,c)=(A,B,C);
                while (exec())
                {

                }

                (A, B, C) = (a, b, c);
                dump = false;
            }

            public bool exec()
            {
                if (mem.Length <= ip+1) return false;
                int opcode = mem[ip++];
                ulong operand = (ulong)mem[ip++];
               

                switch (opcode)
                {
                    case 0:  //adv
                        A = A/(1U<<(int)Combo());
                        Dbg($"A /= 1<<{ComboS()}");
                        break;
                    case 1:
                        B = B ^ operand;
                        Dbg($"B^={operand}");
                        break;
                    case 2:
                        B = Combo() & 7;
                        Dbg($"B={ComboS()}&7");
                        break;
                    case 3:
                        if (A != 0 && !dump) ip = (int)operand;
                        Dbg($"if (A!=0) goto {operand}");
                        break;
                    case 4:
                        B = B ^ C;
                        Dbg($"B ^= C");
                        break;
                    case 5:
                        Output(Combo()&7);
                        Dbg($"output {ComboS()}&7");
                        break;
                    case 6:
                        B = A / (1U << (int)Combo());
                        Dbg($"B = A/(1<<{ComboS()})");
                        break;
                    case 7:
                        C = A / (1U << (int)Combo());
                        Dbg($"C = A/(1<<{ComboS()})");
                        break;
                    default:
                        throw new NotImplementedException("Inst");
                }

                return true;

                string ComboS()
                {
                    var v = operand;
                    if (v < 4) return v.ToString();
                    if (v == 4) return "A";
                    if (v == 5) return "B";
                    if (v == 6) return "C";
                    throw new NotImplementedException("bad combo op");
                }
                ulong Combo()
                {
                    var v = operand;
                    if (v < 4) return v;
                    if (v == 4) return A;
                    if (v == 5) return B;
                    if (v == 6) return C;
                    throw new NotImplementedException("bad combo op");
                }
            }

            void Output(ulong v)
            {
                output.Add((int)v);
            }
        }


        public override object Run(bool part2)
        {
            return part2 ? Run2() : Run1();
        }
    }
}