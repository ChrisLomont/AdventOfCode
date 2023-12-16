
using System.Diagnostics.Contracts;
using System.Numerics;
using System.Runtime.InteropServices;

#if false
// todo - polish off, put in here
namespace Lomont.AdventOfCode.Utils
{
    public static class NumberTheory
    {
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
            long p;
            var sm = 0L;
            for (var i = 0; i < n.Length; i++)
            {
                p = prod / n[i];
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

            // Driver Code 
            public static void Main(string[] args)
            {
                int A = 3, M = 11;

                // Function call 
                modInverse(A, M);
            }
        }

    }
}
#endif