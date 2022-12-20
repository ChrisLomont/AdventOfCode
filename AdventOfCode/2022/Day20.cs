
using System.Runtime.ConstrainedExecution;
using System.Threading.Tasks.Sources;

namespace Lomont.AdventOfCode._2022
{
    internal class Day20 : AdventOfCode
    {
        // top 100 both: 4:53 to 21:14
        // one * only 3:46 to 15:41


        // huge bug to figure out: mod count-1 instead of mod count :( !

        //Day Time    Rank Score       Time Rank  Score
        //20   00:24:00     278      0   01:02:21    1195      0
        //19   01:02:55     309      0   01:18:41     332      0

        //2022 Day 20 part 1: 11073 in 110036.5 us
        //2022 Day 20 part 2: 11102539613040 in 990688.7 us

        class Node
        {
            public long val;
            public Node? next = null, prev = null;

        }

        public override object Run(bool part2)
        {
            long key = 811589153;
            long passes = 10;
            if (!part2) (key, passes) = (1, 1);

            List<Node> all = ReadLines().Select(
                line => new Node { val = int.Parse(line) * key }
            ).ToList();

            // link all
            var count = all.Count;
            for (var i = 0; i < count; ++i)
            {
                var p = all[i];
                var q = all[(i + 1) % count];
                p.next = q;
                q.prev = p;
            }

            // iterate
            for (var pass = 0; pass < passes; ++pass)
            for (int i = 0; i < count; ++i)
                Move(all[i]);

            void Move(Node q)
            {
                var v = q.val % (count - 1); // count -1 since loop has item itself removed

                if (v < 0)
                {
                    while (v++ < 0)
                    {
                        var n = q.next;
                        var p = q.prev;

                        // remove q
                        p.next = n;
                        n.prev = p;

                        q.prev = p.prev;
                        q.next = p;

                        q.prev.next = q;
                        q.next.prev = q;
                    }

                }
                else if (v > 0)
                {
                    while (v-- > 0)
                    {
                        var n = q.next;
                        var p = q.prev;

                        // remove q
                        p.next = n;
                        n.prev = p;

                        q.prev = n;
                        q.next = n.next;

                        q.next.prev = q;
                        q.prev.next = q;

                    }

                }
            }

            // start item with val 0, then 1000th, 2000th, 3000th
            var zero = all.First(v => v.val == 0);
            var qq = zero;
            int j = 1000;
            while (j-- > 0) qq = qq.next;
            var score = qq.val;
            j = 1000;
            while (j-- > 0) qq = qq.next;
            score += qq.val;
            j = 1000;
            while (j-- > 0) qq = qq.next;
            score += qq.val;

            return score; 
        }
    }
}