

using System.Data;

namespace Lomont.AdventOfCode._2018
{
    internal class Day16 : AdventOfCode
    {
        /*
         2018 Day 16 part 1: 592 in 8917.3 us
         2018 Day 16 part 2: 557 in 10570.9 us
         */
        object Run2()
        {
            int answer = 0;
            var lines = ReadLines();
            int pos = 0;

            // op map entry j holds the op to send to CPU
            int[] opMap = new int[16];

            List<int[]> program = new List<int[]>();
            int state = 0;
            while (pos < lines.Count)
            {
                if (!lines[pos].StartsWith("Before"))
                    state = 1;
                if (state == 0)
                {
                    var bf = Numbers(lines[pos++]);
                    var ops = Numbers(lines[pos++]);
                    var fi = Numbers(lines[pos++]).ToArray();

                    List<int> valid = new();
                    for (int op = 0; op < 16; ++op)
                    {
                        var st = new St();
                        st.regs = bf.ToArray();
                        st.A = ops[1];
                        st.B = ops[2];
                        st.C = ops[3];
                        st.DoOp(op);
                        if (Same(st.regs, fi))
                        {
                            valid.Add(op);
                        }
                    }

                    // set missing bits
                    var opV = ops[0];
                    for (int i = 0; i < 16; i++)
                    {
                        if (!valid.Contains(i))
                            opMap[opV] |= 1 << i;
                    }

                    //if (valid.Count >= 3) answer++;
                    //if (valid.Count == 1)
                    //{
                    //    opMap[ops[0]] = valid[0];
                    //}

                    ++pos;
                }
                else
                {
                    var line = lines[pos++];
                    if (line.Length>1)
                    {
                        program.Add(Numbers(line).ToArray());
                    }
                }
            }


            // now invert ops, any with one bit set, remove from others, repeat
            for (int i = 0; i < 16; ++i)
                opMap[i] ^= 65535;
            
            //foreach (var c in opMap)
            //    Console.Write($"{c},");
            //Console.WriteLine();

            while (true)
            {
                bool change = false;
                for (int i = 0; i < 16; ++i)
                {
                    if (OneBit(opMap[i]))
                    {
                        int mask = opMap[i]^65535;
                        for (int j = 0; j < 16; ++j)
                        {
                            if (j != i)
                            {
                                var v = opMap[j];
                                opMap[j] &= mask;
                                change |= opMap[j] != v;
                            }
                        }
                    }
                }
             //   foreach (var c in opMap)
             //       Console.Write($"{c},");
             //   Console.WriteLine();
                if (!change) break;
            }

            // convert to bit index
            for (int i = 0; i < 16; ++i)
            {
                opMap[i] = BitIndex(opMap[i]);
            }

            int BitIndex(int v)
            {
                for (int t = 0; t < 16; ++t)
                    if ((1 << t) == v)
                        return t;
                throw new NoNullAllowedException();
            }

            bool OneBit(int v)
            {
                var b = 0;
                while (v > 0)
                {
                    b += (v & 1);
                    v >>= 1;
                }

                return b == 1;
            }

            // eval program
            St s = new();
            foreach (var p in program)
            {
                s.A = p[1];
                s.B = p[2];
                s.C = p[3];
                //Dump(p,singleLine:true);
                s.DoOp(opMap[p[0]]);
                //Dump(s.regs, singleLine: true);
            }

            return s.regs[0];

            bool Same(int[] a, int[] b)
            {
                for (int i = 0; i < a.Length; ++i)
                {
                    if (a[i] != b[i]) return false;
                }
                return true;
            }
        }

        class St
        {
            public int[] regs = new int[4];
            public int A, B, C;

            public void DoOp(int op)
            {
                if (op == 0) regs[C] = regs[A] + regs[B];
                else if (op == 1) regs[C] = regs[A] + B;
                else if (op == 2) regs[C] = regs[A] * regs[B];
                else if (op == 3) regs[C] = regs[A] * B;
                else if (op == 4) regs[C] = regs[A] & regs[B];
                else if (op == 5) regs[C] = regs[A] & B;
                else if (op == 6) regs[C] = regs[A] | regs[B];
                else if (op == 7) regs[C] = regs[A] | B;
                else if (op == 8) regs[C] = regs[A];
                else if (op == 9) regs[C] = A;

                else if (op == 10) regs[C] = A > regs[B] ? 1 : 0;
                else if (op == 11) regs[C] = regs[A] > B ? 1 : 0;
                else if (op == 12) regs[C] = regs[A] > regs[B] ? 1 : 0;

                else if (op == 13) regs[C] = A == regs[B] ? 1 : 0;
                else if (op == 14) regs[C] = regs[A] == B ? 1 : 0;
                else if (op == 15) regs[C] = regs[A] == regs[B] ? 1 : 0;
            }
        }

        object Run1()
        {
            int answer = 0;
            var lines = ReadLines();
            int pos = 0;
            while (pos < lines.Count)
            {
                if (!lines[pos].StartsWith("Before"))
                    break;
                var bf = Numbers(lines[pos++]);
                var ops = Numbers(lines[pos++]);
                var fi = Numbers(lines[pos++]).ToArray();

                List<int> valid = new();
                for (int op = 0; op < 16; ++op)
                {
                    var st = new St();
                    st.regs = bf.ToArray();
                    st.A = ops[1];
                    st.B = ops[2];
                    st.C = ops[3];
                    st.DoOp(op);
                    if (Same(st.regs,fi))
                    {
                        valid.Add(op);
                    }
                }

                if (valid.Count >= 3) answer++;

                ++pos;
            }

            return answer;

            bool Same(int[] a, int[]b)
            {
                for (int i = 0; i < a.Length; ++i)
                {
                    if (a[i] != b[i] ) return false;
                }
                return true;
            }
        }

        public override object Run(bool part2)
        {
            return part2 ? Run2() : Run1();
        }
    }
}