namespace Lomont.AdventOfCode.Utils
{
    internal static class Extensions
    {
        // generic overload
        public static IEnumerable<long> Nums(this IEnumerable<string> numbers)
            => numbers.Select(n => long.Parse(n));


    }
}
