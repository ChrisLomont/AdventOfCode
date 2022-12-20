using System.Collections.Concurrent;

namespace Lomont.AdventOfCode.Utils
{
    // todo - move to Lomont utils or somewhere
    // https://stackoverflow.com/questions/2852161/c-sharp-memoization-of-functions-with-arbitrary-number-of-arguments
    // https://stackoverflow.com/questions/20544641/how-to-perform-thread-safe-function-memoization-in-c
    // https://stackoverflow.com/questions/53285304/implementing-memoization-in-c-sharp
    // https://trenki2.github.io/blog/2018/12/31/memoization-in-csharp/
    // todo - give example here

    // todo - needs lots more work, thought....

    public static class Memoizer
    {
        /*
         example - todo - check, make test.
        Func<int, int, int> example2Func = (a, b) => a + b;
        var example2Memoized = example2Func.Memoize();
        var example2Result = example2Memoized(3, 4);
         */

        /// <summary>
        /// Threadsafe, requires func to be threadsafe
        /// </summary>
        /// <typeparam name="TArg1"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="f"></param>
        /// <returns></returns>
        public static Func<TArg1, TResult> Memoize<TArg1, TResult>(this Func<TArg1, TResult> f) where TArg1 : notnull
        {
            var cache = new ConcurrentDictionary<TArg1, TResult>();

            return arg1 => cache.GetOrAdd(arg1, f);
        }
        public static Func<TArg1, TArg2, TResult> Memoize<TArg1, TArg2, TResult>(this Func<TArg1, TArg2, TResult> f)
        {
            var cache = new ConcurrentDictionary<(TArg1, TArg2), TResult>();
            return (argument,arg2) => cache.GetOrAdd((argument, arg2),
                t=> f(t.Item1, t.Item2));
        }

        public static void Test()
        {
            Console.WriteLine($"fib(20) {fib1(20)}");

            Func<int, int> fib = null;
            fib = n => n > 1 ? fib(n - 1) + fib(n - 2) : n;

            var fibMemo = fib.Memoize();
            Console.WriteLine($"fib(20) {fibMemo(20)}");




        }
        static int fib1(int n)
        {
            if (n == 0 || n == 1) return n;
            return fib1(n - 1) + fib1(n - 2);
        }

    }
}
