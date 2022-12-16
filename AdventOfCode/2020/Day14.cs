namespace Lomont.AdventOfCode._2020
{
    internal class Day14 : AdventOfCode
    {
        // 2020 Day 14 part 1: 6559449933360 in 114879.2 us
        // 2020 Day 14 part 2: 3369767240513 in 11230749.2 us
        public override object Run(bool part2)
        {
            var mem = new Dictionary<long,long>();
            string mask = "";
            foreach (var line in ReadLines())
            {
                if (line.StartsWith("mask"))
                {
                    mask = line.Substring(7);
                    Trace.Assert(mask.Length == 36);
                }
                else
                {
                    var n = Numbers64(line, false);
                    var val = n[1];
                    var addr = n[0];
                    if (!part2)
                    {
                        val = Mask(val);
                        if (!mem.ContainsKey(addr))
                            mem.Add(addr, 0);
                        mem[addr] = val;
                    }
                    else
                    {
                        Write2(addr, val);
                    }

                }
            }

            return mem.Values.Sum();

            long Mask(long v)
            {
                var len = mask.Length - 1;
                long ans = 0;
                for (var i = 0; i <= len; ++i)
                {
                    var bit = v & 1;

                    if (mask[len-i] == '0') bit = 0;
                    if (mask[len-i] == '1') bit = 1;
                    v >>= 1;
                    ans += (bit << i);
                }

                return ans;
            }

            void Write2(long addr1, long val)
            {

                var d = new long[36];
                for (var i = 0; i < 36; ++i)
                {
                    d[i] = addr1 & 1;
                    addr1 >>= 1;
                }

                for (var i = 0; i < 36; ++i)
                {
                    if (mask[35 - i] == '1')
                        d[i] = 1;
                    if (mask[35 - i] == 'X')
                        d[i] = 2;
                }


                Recurse(0);

                void Recurse(int len)
                {
                    if (len == 36)
                    {
                        Trace.Assert(d.Max() < 2);
                        var addr2 = 0L;
                        for (var i = 35; i >= 0; --i)
                            addr2 = 2 * addr2 + d[i];
                        if (!mem.ContainsKey(addr2))
                            mem.Add(addr2, 0);
                        mem[addr2] = val;
                    }
                    else if (d[len] == 2)
                    {
                        d[len] = 0;
                        Recurse(len + 1);
                        d[len] = 1;
                        Recurse(len + 1);
                        d[len] = 2;
                    }
                    else
                        Recurse(len + 1);
                }
            }

        }
    }
}

