namespace Lomont.AdventOfCode.Utils
{
    internal static class Extensions
    {
        // generic overload
        public static IEnumerable<long> Nums(this IEnumerable<string> numbers)
            => numbers.Select(n => long.Parse(n));

        // string reverse
        public static string Reverse(this string text)
        {
            char[] charArray = text.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
        }

        // allow "foreach (var (item,index) in collection.WithIndex())"
        public static IEnumerable<(T item, int index)> WithIndex<T>(this IEnumerable<T> source)
        {
            return source.Select((item, index) => (item, index));
        }

    }
}
