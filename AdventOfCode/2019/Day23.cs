namespace Lomont.AdventOfCode._2019
{
    internal class Day23 : AdventOfCode
    {
        // 2019 Day 23 part 1: 16250 in 69984.4 us
        // 2019 Day 23 part 2: 11046 in 938890.6 us
        public override object Run(bool part2)
        {
            var prog = Numbers64(ReadLines()[0]);

            int[] receiveCounts = new int[50];

            List<Day02.IntCode> comps = new();
            List<Queue<long>> inputs = new();
            List<Queue<long>> outputs = new();
            for (var i = 0; i < 50; ++i)
            {
                inputs.Add(new());
                outputs.Add(new());
                var i1 = i; // local copy for capture
                comps.Add(new(prog,()=>Get(i1),v=>Set(i1,v)));
                inputs.Last().Enqueue(i); // set NIC address
            }


            long Get(int addr)
            {
                var op = inputs[addr];
                if (op.Any())
                    return op.Dequeue();
                receiveCounts[addr]++;
                return -1;
            }

            void Set(long addr, long val)
            {
                receiveCounts[addr] = 0;
                outputs[(int)addr].Enqueue(val);
            }

            var nat = (x:-1L,y:-1L);
            var lastNatSent = (x:0L, y:0xDFEFL);
            bool hasNat = false;
            var natCount = 0;


            while (true)
            {
                foreach (var c in comps)
                    c.Step();
                
                // forward packets
                foreach (var q in outputs)
                {
                    if (q.Count >= 3)
                    {
                        var addr = (int)q.Dequeue();
                        var x = q.Dequeue();
                        var y = q.Dequeue();

                        if (addr == 255)
                        {
                            if (!part2)
                                return y;
                            //Console.WriteLine($"NAT read {x},{y}");
                            if (natCount>0 && y == nat.y)
                            { // twice in row
                             //   return y;
                            }
                            nat = (x, y);
                            hasNat = true;
                            natCount++;

                        }
                        else
                        {

                            inputs[addr].Enqueue(x);
                            inputs[addr].Enqueue(y);
                        }
                    }
                }

                // send NAT
                if (receiveCounts.Min() > 5 && hasNat)
                {
                    inputs[0].Enqueue(nat.x);
                    inputs[0].Enqueue(nat.y);
                    hasNat = false;
                    //Console.WriteLine($"NAT sent {nat.x},{nat.y}");

                    if (lastNatSent.y == nat.y)
                        return nat.y;
                    lastNatSent = nat;
                }
            }

        }
    }
}