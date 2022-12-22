using System.Numerics;

namespace Lomont.AdventOfCode._2019
{
    internal class Day22 : AdventOfCode
    {
    
        //2019 Day 22 part 1: 1538 in 22114.4 us
        //2019 Day 22 part 2: 96196710942473 in 4292.5 us

        object Run2()
        {
            checked
            {
                // all ops behave nicely modulo other behavior, so track
                // only needed items

                // cards 119315717514047
                // 101741582076661 passes

                // 64 bit long not big enough

                BigInteger len = 119315717514047L; // # cards
                BigInteger passes = 101741582076661L; // # times to do operation

                 (BigInteger a, BigInteger b) = (1,0);

                 // compute what one pass does to the rotation and spacing
                foreach (var line in ReadLines())
                {
                    var w = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    if (line.Contains("cut"))
                    {
                        var n = BigInteger.Parse(w[1]);
                        b = (b - n + len) % len; // rotates start pos
                    }
                    else if (line.Contains("deal with increment"))
                    {
                        var n = BigInteger.Parse(w[3]);
                        (a, b) = ((a * n) % len, (b * n) % len); // new spacing 
                    }
                    else if (line.Contains("deal into new stack"))
                    {
                        (a, b) = ((len - a) % len, len-1-b); // reverses
                    }
                }

                var an = BigInteger.ModPow(a, passes, len);

                var qq = b * (an - 1 + len) * ModInverse(a - 1 + len , len);

                (a, b) = (an, qq % len);

                var t = (2020 - b + len);
                var ans = (t * ModInverse(a, len))%len;

                return ans; //96196710942473

                BigInteger ModInverse(BigInteger a, BigInteger m) => BigInteger.ModPow(a, m - 2, m);
            }
        }

        public override object Run(bool part2)
        {
            if (part2)
                return Run2();
            //return SolvePartOne();

            var len = 10007;
            //len = 10;
            var cards = Enumerable.Range(0, len).ToList();
            // top is index 0

            // new stack
            //cards.Reverse();

            // cut N - top N off top, move to bottom, -N moves other way

            // deal with incr k: M = # cards, M slots, top to 0, nxt to 0+k, then 0+2k, wrapping...
            // collect so leftmost is on top

            // cards 119315717514047
            // 101741582076661 passes

            int passes = 1;
            for (var pass = 0; pass < passes; ++pass)
            {
                foreach (var line in ReadLines())
                {
                    var w = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    if (line.Contains("cut"))
                    {
                        Cut(Int32.Parse(w[1]));
                    }
                    else if (line.Contains("deal with increment"))
                    {
                        DealIncr(Int32.Parse(w[3]));
                    }
                    else if (line.Contains("deal into new stack"))
                    {

                        DealInto();
                    }
                    else throw new Exception();
                }
            }
            
            var chk = new List<int>();
            chk.AddRange(cards);
            chk.Sort();
            for (var i =0; i < len; ++i)
                Trace.Assert(chk[i]==i);


            //if (len > 2020)
            {
                //Console.WriteLine($"{cards[2018]} {cards[2019]} {cards[2020]}");
                //Console.WriteLine($"{cards[2019]} {cards2.Cards[2019]}");
                if(!part2)
                    return cards.IndexOf(2019);
                
                    return cards.IndexOf(2020);
            }
            Dump(cards, true);
            return -1234;

            void Cut(int n)
            {
                if (n < 0) n += len;
#if false
                var (top, bot) =
                n>=0
                    ? (cards.Take(n).ToList(), cards.Skip(n).ToList())
                    : (cards.Skip(len-1-n).ToList(), cards.Take(len-1-n).ToList());
#else

                    //       n = ((n %len)+len)%len; // positive mod
                var top = cards.Take(n).ToList();
                var bot = cards.Skip(n).ToList();
#endif
                cards.Clear();
                cards.AddRange(bot);
                cards.AddRange(top);
            }

            void DealIncr(int n)
            {
                var dst = new int[len];
                var index = 0;
                for (var i = 0; i < len; ++i)
                {
                    dst[index] = cards[i];
                    index = (index + n)%len;
                }

                cards = dst.ToList();
            }

            void DealInto()
            {
                cards.Reverse();
            }

        }
    }
}