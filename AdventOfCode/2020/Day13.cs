using System.Numerics;
namespace Lomont.AdventOfCode._2020
{
    internal class Day13 : AdventOfCode
    {

        // inverse of a mod n, or -1 if no inverse
        BigInteger inverse(BigInteger a, BigInteger n)
        {
            var (t, newt) = (new BigInteger(), new BigInteger(1));
            var (r, newr) = (n, a);

            while (newr != 0)
            {
                var quotient = r / newr;
                (t, newt) = (newt, t - quotient * newt);
                (r, newr) = (newr, r - quotient * newr);
            }

            Trace.Assert(r<=1);
            if (r > 1)
                return -1; // no inverse
            if (t < 0)
                t = t + n;

            Trace.Assert(((a * t) % n) == 1);

            return t;
        }
        void extended_gcd(long a, long b)
{
    var (old_r, r) = (a, b);
    var (old_s, s) = (1L, 0L);
    var (old_t, t) = (0L, 1L);
    
    while (r != 0)
    {
        var quotient = old_r / r;
        (old_r, r) = (r, old_r - quotient * r);
        (old_s, s) = (s, old_s - quotient * s);
        (old_t, t) = (t, old_t - quotient * t);
    }
    
    // now a*old_s + b*old_t = gcd(a,b)

    // Bézout coefficients (old_s, old_t)
    // greatest common divisor, old_r
    // quotients by the gcd (t, s)
        }

        // 3966
        // 
        public override object Run(bool part2)
        {
            var lines = ReadLines();
            var min = Num(lines[0]);
            var w = Split(lines[1],',');
            var b = w.Where(x => x != "x").Select(Int32.Parse).ToList();

            if (part2)
            {
                // me
                //1006697
                //13,x,x,41,x,x,x,x,x,x,x,x,x,641,x,x,x,x,x,x,x,x,x,x,x,19,x,x,x,x,17,x,x,x,x,x,x,x,x,x,x,x,29,x,661,x,x,x,x,x,37,x,x,x,x,x,x,x,x,x,x,x,x,x,x,x,x,23

                // CRT: congruences solve x~a1 mod n1 and x~a2 mod n2, where gcd(n1,n2) = 1, is  
                // x = a1 n2 (n2^-1/n1) + a2 n1 (n1^-1/n2)
                // where (n1^-1/n2) is inverse of n1 mod n2

                //2020 Day 13 part 1: 3966 in 12114.6 us
                //2020 Day 13 part 2: 800177252346225 in 6080 us


                // get pairs (period,delay)
                var l2 = lines[1].Replace("x", "0");
                var pairs = Numbers64(l2,false).Select((v, i) => (v, (long)i)).Where(q => q.v != 0).ToList();
                //pairs.Reverse();
                // pairs are form (bus # prime, delta)

                checked
                {
                    BigInteger Rev(BigInteger ni, BigInteger ai)
                    {
                        var t = ni - ai;
                        while (t < 0) t += ni;
                        return t % ni;
                    }
                    //return Solve(5,7,8,15);

                    // solve first 2:

                    var n1 = new BigInteger(pairs[0].Item1);
                    var a1 = new BigInteger(pairs[0].Item2);
                    var n2 = new BigInteger(pairs[1].Item1);
                    var a2 = new BigInteger(pairs[1].Item2);

                    a1 = Rev(n1, a1);
                    a2 = Rev(n2, a2);
                    var t = Solve(a1, n1, a2, n2);
                    var m = n1 * n2; // composite modulus
                    for (var k = 2; k < pairs.Count; ++k)
                    {
                        var pair = pairs[k];
                        //Console.WriteLine(pair);
                        var ni = new BigInteger(pairs[k].Item1);
                        var ai = new BigInteger(pairs[k].Item2);

                        ai = Rev(ni, ai);

                        t = Solve(t, m, ai, ni); 
                        m *= ni;
                    }

                    // 2093560 1068781
                    t %= m;

                    // 1063618741115151 too big

                    BigInteger Solve(BigInteger a1, BigInteger n1, BigInteger a2, BigInteger n2)
                    {

                        // x = a1 n2 (n2^-1/n1) + a2 n1 (n1^-1/n2)
                        // where (n1^-1/n2) is inverse of n1 mod n2
                        var inv1 = inverse(n2,n1);
                        var inv2 = inverse(n1,n2);
                        return (a1 * n2 * inv1 + a2 * n1 * inv2) % (n1 * n2);
                    }
                    return t; // 1202161486 too low
                }


                // 

            }


            // 661 too low


            var (minId, minVal) = (-1, int.MaxValue);
            foreach (var busId in b)
            {
                var leftMinAgo = min % busId; // amount past last time
                

                var minTillNext = (busId - leftMinAgo) % busId;

              //  Console.WriteLine($"{min} % {busId} = {leftMinAgo} ago, {minTillNext} till next");

                if (minTillNext < minVal)
                {
                    (minId,minVal)=(busId,(int)minTillNext);
                }
            }

            return minId*minVal;

        }
    }
}