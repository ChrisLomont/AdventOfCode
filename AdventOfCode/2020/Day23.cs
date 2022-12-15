
namespace Lomont.AdventOfCode._2020
{
    internal class Day23 : AdventOfCode
    {
    //2020 Day 23 part 1: 36542897 in 7353 us
    //2020 Day 23 part 2: 562136730660 in 1560359.4 us

        class Cup
        {
            public Cup? Prev, Next;
            public int Value;
        }

        public override object Run(bool part2)
        {
            var order = ReadLines()[0];

            var startVals = order.ToCharArray().Select(c => c - '0').ToList();
            if (part2)
            {
                var next = startVals.Max() + 1;
                while (startVals.Count < 1_000_000)
                    startVals.Add(next++);
            }
            var size = startVals.Count;

            // array of cups, index [i] is cup with label i
            // order is determined by double linked list in the nodes
            var cups = new Cup[size+1];
            Cup? last = null;
            foreach (var v in startVals)
            {
                var q = new Cup { Prev = last, Value = v };
                if (last!=null)
                    last.Next = q;
                last = q;
                cups[v] = q;
            }

            // wrap last to first
            last!.Next = cups[startVals[0]];
            last.Next.Prev = last;

            var cupIndex = startVals[0];

            var moves = part2 ? 10_000_000 : 100;
            for (var move = 1; move <= moves; ++move)
            {
                var cup = cups[cupIndex];
                var c1 = cup.Next;
                var c2 = c1!.Next;
                var c3 = c2!.Next;
            
                // remove 3 items
                cup.Next = c3!.Next;
                cup.Next!.Prev = cup;

                // pick dest index
                var destIndex = cupIndex;
                do
                {
                    destIndex--;
                    if (destIndex < 1)
                        destIndex = size;
                } while (destIndex == c1.Value || destIndex == c2.Value || destIndex == c3.Value);


                // insert cups back right past destIndex
                var dest = cups[destIndex];
                c3.Next = dest.Next;
                c3.Next!.Prev = c3;
                dest.Next = c1;
                c1.Prev = dest;

                cupIndex = cups[cupIndex].Next!.Value;
            }

            if (!part2)
            {
                var s = "";
                var p = cups[1].Next;
                while (p != cups[1])
                {
                    s += p!.Value;
                    p = p.Next;
                }

                return s; // example 67384529, mine 36542897
            }

            var cup1 = cups[1];
            var n1 = cup1.Next;
            var n2 = n1!.Next;
            var (v1, v2) = ((long)n1.Value, (long)n2!.Value);
            return v1 * v2;
        }
    }
}