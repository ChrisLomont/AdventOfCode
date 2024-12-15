
using System.Diagnostics.Contracts;
using System.Numerics;
using System.Runtime.InteropServices;

// todo - polish off, put in here
namespace Lomont.AdventOfCode.Utils
{
    public static class NumberTheory
    {
        public static long PositiveMod(long a, long b) => ((a%b)+b)%b;

        public static long GCD(long a, long b)
        {
            while (b != 0)
                (a, b) = (b, a % b);
            return a;

        }
        public static long LCM(long a, long b)
        {
            var d = GCD(a, b);
            return (a / d) * b;
        }
        public static long LCM(List<long> vals)
        {
            var a = vals[0];
            var b = vals[1];
            var cur = LCM(a, b);
            for (int i = 2; i < vals.Count; ++i)
            {
                cur = LCM(cur, vals[i]);
            }

            return cur;
        }

        // given a,b, get am and bm so that a*am + b*bm = gcd(a,b)
        public static void ExtendedGCD(long a, long b, out long am, out long bm)
        {
            var (oldR, r) = (a, b);
            var (oldS, s) = (1L, 0L);
            var (oldT, t) = (0L, 1L);

            while (r != 0)
            {
                var quotient = oldR / r;
                (oldR, r) = (r, oldR - quotient * r);
                (oldS, s) = (s, oldS - quotient * s);
                (oldT, t) = (t, oldT - quotient * t);
            }

            am = oldS;
            bm = oldT;
            Trace.Assert(a * am + b * bm == GCD(a, b));
        }

#if false

        public static void Test()
        {
            int[] n = { 3, 5, 7 };
            int[] a = { 2, 3, 2 };

            int result = NumberTheory.ChineseRemainderTheorem(n, a);

            int counter = 0;
            int maxCount = n.Length - 1;
            while (counter <= maxCount)
            {
                Console.WriteLine($"{result} ≡ {a[counter]} (mod {n[counter]})");
                counter++;
            }
        }

        public static long ChineseRemainderTheorem(long[] n, long[] a)
        {
            var prod = n.Aggregate(1L, (i, j) => i * j);
            var sm = 0L;
            for (var i = 0; i < n.Length; i++)
            {
                long p = prod / n[i];
                sm += a[i] * ModularMultiplicativeInverse(p, n[i]) * p;
            }

            return sm % prod;
        }

        private static long ModularMultiplicativeInverse(long a, long mod)
        { 
            long b = a % mod;
            for (long x = 1; x < mod; x++)
            {
                if ((b * x) % mod == 1)
                    return x;
            }

            return 1;
        }

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

            Trace.Assert(r <= 1);
            if (r > 1)
                return -1; // no inverse
            if (t < 0)
                t = t + n;

            Trace.Assert(((a * t) % n) == 1);

            return t;
        }
        

        public class GFG
        {
           // public static int x, y;

            // Function for extended Euclidean Algorithm 
            static int gcdExtended(int a, int b)
            {

                // Base Case 
                if (a == 0)
                {
                    x = 0;
                    y = 1;
                    return b;
                }

                // To store results of recursive call 
                int gcd = gcdExtended(b % a, a);
                int x1 = x;
                int y1 = y;

                // Update x and y using results of recursive 
                // call 
                x = y1 - (b / a) * x1;
                y = x1;

                return gcd;
            }

            // Function to find modulo inverse of a 
            static void modInverse(int A, int M)
            {
                int g = gcdExtended(A, M);
                if (g != 1)
                    Console.Write("Inverse doesn't exist");
                else
                {

                    // M is added to handle negative x 
                    int res = (x % M + M) % M;
                    Console.Write(
                        "Modular multiplicative inverse is " + res);
                }
            }
        }
#endif
    }
}
