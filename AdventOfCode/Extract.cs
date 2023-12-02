#if false

// things from AOC code that I should extract to LomontLib
namespace Lomont.AdventOfCode.Extract
{
     // todo - vec3, vec4 have some nice integer ops;

     // tree parser that parses tree delimited with [], {}, (), etc.
     // Set class with nice stuff
     // lots of the search types: Dijkstra, UCS, frontier, A*, get all working and abstract
     // generic memoizer?



    class Output
    {
        /// <summary>
        /// Dump enumerable
        /// </summary>
        public static void Dump<T>(IEnumerable<T> items, bool singleLine = false, TextWriter? output = null)
        {
            output ??= Console.Out;
            foreach (var i in items)
            {
                if (singleLine)
                {
                    output.Write(i + ", ");
                }
                else
                    output.WriteLine(i);
            }
            if (singleLine) output.WriteLine();
        }

        /// <summary>
        /// Dump 2d grid
        /// </summary>
        protected static void Dump<T>(T[,] grid, bool noComma = false)
        {
            var m = grid.GetLength(0);
            Apply(grid, (i, j, v) =>
                {
                    Console.Write($"{grid[i, j]}");
                    if (!noComma)
                        Console.Write(',');
                    if (i == m - 1)
                        Console.WriteLine();
                    return v;
                }
            );
            Console.WriteLine();
        }
    }

    class Set
    {
        /// <summary>
        /// Intersect items
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list1"></param>
        /// <param name="list2"></param>
        /// <returns></returns>
        protected List<T> Intersect<T>(IEnumerable<T> list1, IEnumerable<T> list2) =>
            Enumerable.Intersect(list1, list2).ToList();

        /// <summary>
        /// Intersection of all lists
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="lists"></param>
        /// <returns></returns>
        protected List<T> Intersect<T>(params IEnumerable<T>[] lists)
        {
            if (lists.Length == 1)
                return lists[0].ToList();
            List<T> ans = Intersect(lists[0], lists[1]);
            for (var t = 2; t < lists.Length; ++t)
                ans = Intersect(ans, lists[t]);
            return ans;
        }

        /// <summary>
        /// Union of items
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list1"></param>
        /// <param name="list2"></param>
        /// <returns></returns>
        protected List<T> Union<T>(IEnumerable<T> list1, IEnumerable<T> list2) =>
            Enumerable.Union(list1, list2).ToList();

        /// <summary>
        /// Union of all lists
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="lists"></param>
        /// <returns></returns>
        protected List<T> Union<T>(params IEnumerable<T>[] lists)
        {
            if (lists.Length == 1)
                return lists[0].ToList();
            List<T> ans = Union(lists[0], lists[1]);
            for (var t = 2; t < lists.Length; ++t)
                ans = Union(ans, lists[t]);
            return ans;
        }
    }

    // 2d and 3d grid tools
    static class GridTools
    {
        /// <summary>
        /// Get grid item if in bounds, else default
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="grid"></param>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <param name="def"></param>
        /// <returns></returns>
        public static T Get<T>(T[,] grid, int i, int j, T def)
        {
            var w = grid.GetLength(0);
            var h = grid.GetLength(1);
            if (i < 0 || j < 0 || w <= i || h <= j) return def;
            return grid[i, j];
        }
        public static T Get<T>(T[,,] grid, int i, int j, int k, T def)
        {
            var w = grid.GetLength(0);
            var h = grid.GetLength(1);
            var d = grid.GetLength(2);

            if (i < 0 || j < 0 || k < 0 || w <= i || h <= j || d <= k) return def;
            return grid[i, j, k];
        }

        public static T Get<T>(T[,,,] grid, int i, int j, int k, int q, T def)
        {
            var w = grid.GetLength(0);
            var h = grid.GetLength(1);
            var d = grid.GetLength(2);
            var r = grid.GetLength(3);

            if (i < 0 || j < 0 || k < 0 || q < 0 || w <= i || h <= j || d <= k || r <= q) return def;
            return grid[i, j, k, q];
        }

        /// <summary>
            /// Size of a grid
            /// </summary>
            public static (int w, int h) Size<T>(T[,] g)
        {
            return (g.GetLength(0), g.GetLength(1));
        }
        /// <summary>
        /// Size of a grid
        /// </summary>
        public static (int w, int h, int d) Size<T>(T[,,] g)
        {
            return (g.GetLength(0), g.GetLength(1), g.GetLength(2));
        }
        /// <summary>
        /// 27 3d neighbors
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="gg"></param>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <param name="k"></param>
        /// <param name="def"></param>
        /// <returns></returns>
        public static IEnumerable<T> Nbrs3<T>(T[,,] gg, int i, int j, int k, T def)
        {
            for (var di = -1; di <= 1; ++di)
            for (var dj = -1; dj <= 1; ++dj)
            for (var dk = -1; dk <= 1; ++dk)
            {
                if (di == 0 && dj == 0 && dk == 0) continue;
                yield return Get(gg, i + di, j + dj, k + dk, def);
            }
        }
        /// <summary>
        /// 8 2d neighbors
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="gg"></param>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <param name="def"></param>
        /// <returns></returns>
        public static IEnumerable<T> Nbrs2<T>(T[,] gg, int i, int j, T def)
        {
            for (var di = -1; di <= 1; ++di)
            for (var dj = -1; dj <= 1; ++dj)
            {
                if (di == 0 && dj == 0) continue;
                yield return Get(gg, i + di, j + dj, def);
            }
        }
    }

    // things for line parsing, splitting
    static class LineTools
    {
        /// <summary>
        /// Apply action over grid
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="grid"></param>
        /// <param name="func"></param>
        public static void Apply<T>(T[,] grid, Action<T> func)
        {
            for (var i = 0; i < grid.GetLength(0); ++i)
            for (var j = 0; j < grid.GetLength(1); ++j)
                func(grid[i, j]);
        }

        // i,j,k grid value to new grid value
        public static void Apply<T>(T[,,] grid, Func<int, int, int, T, T> func)
        {
            for (var k = 0; k < grid.GetLength(2); ++k)
            for (var j = 0; j < grid.GetLength(1); ++j)
            for (var i = 0; i < grid.GetLength(0); ++i)
                grid[i, j, k] = func(i, j, k, grid[i, j, k]);
        }
        public static void Apply<T>(T[,,,] grid, Func<int, int, int, int, T, T> func)
        {
            for (var l = 0; l < grid.GetLength(3); ++l)
            for (var k = 0; k < grid.GetLength(2); ++k)
            for (var j = 0; j < grid.GetLength(1); ++j)
            for (var i = 0; i < grid.GetLength(0); ++i)
                grid[i, j, k, l] = func(i, j, k, l, grid[i, j, k, l]);
        }

        // i,j,grid value to new grid value
        public static int Count<T>(T[,] grid, Func<T, bool> func)
        {
            var count = 0;
            Apply(grid, v => count += func(v) ? 1 : 0);
            return count;
        }

        // i,j,grid value to new grid value
        public static void Apply<T>(T[,] grid, Func<int, int, T, T> func)
        {
            for (var j = 0; j < grid.GetLength(1); ++j)
            for (var i = 0; i < grid.GetLength(0); ++i)
                grid[i, j] = func(i, j, grid[i, j]);
        }

        static Regex numberRegex = new Regex(@"\d+");
        static Regex signedNumberRegex = new Regex(@"(\+|-)?\d+");

        // read all numbers out of string, ignore other stuff
        public static List<long> Numbers64(string line, bool allowSigned = true)

        {
            if (allowSigned)
                return signedNumberRegex.Matches(line).Select(m => long.Parse(m.Value)).ToList();
            return numberRegex.Matches(line).Select(m => long.Parse(m.Value)).ToList();
        }

        // read all numbers out of string, ignore other stuff
        public static List<int> Numbers(string line, bool allowSigned = true)
        {
            if (allowSigned)
                return signedNumberRegex.Matches(line).Select(m => int.Parse(m.Value)).ToList();
            return numberRegex.Matches(line).Select(m => int.Parse(m.Value)).ToList();
        }
        // todo- make this general, use elsewhere
        // group lines by size of group or by regex match
        // if groupSize != -1, grouped by size
        // else lines appended with \n, then regex splits, then \n removed from each, 
        // default splits on \n\n (a blank line)
        public static List<List<string>> Group(IEnumerable<string> lines, int size = -1, string matchPattern = "\n\n")
        {
            if (size > 0)
                return lines.Chunk(size).Select(arr => arr.ToList()).ToList();

            var all = lines.Aggregate("", (a, b) => a + "\n" + b);
            var chunks = Regex.Split(all, matchPattern);
            //var gps = all.Split(match, StringSplitOptions.RemoveEmptyEntries);
            return chunks
                .Select(g => g.Split('\n', StringSplitOptions.RemoveEmptyEntries))
                .Select(s => s.ToList()).ToList();
        }

        /// <summary>
        /// pull numbers grid from file or lines
        /// </summary>
        /// <returns></returns>
        public static (int width, int height, long[,] grid) NumberGrid(int startLine = -1, int endLine = -1, List<string>? lines = null)
        {
            if (lines == null)
                lines = ReadLines();

            if (startLine < 0) startLine = -1;
            if (endLine < 0) endLine = lines.Count;

            lines = RangeTo(lines, startLine, endLine);

            var w = lines.Max(b => Numbers(b, false).Count);
            var h = lines.Count;

            var g = new long[w, h];
            for (var j = 0; j < h; ++j)
            {
                var n = Numbers(lines[j], false);
                for (var i = 0; i < w; ++i)
                    g[i, j] = n[i];
            }

            return (w, h, g);
        }

        /// <summary>
        /// Get file or lines into a digit grid.
        /// </summary>
        /// <returns></returns>
        public static (int width, int height, int[,] grid) DigitGrid(List<string>? lines = null)
        {
            var (w, h, cg) = CharGrid(lines);
            var g = new int[w, h];
            Apply(cg, (i, j, v) =>
                {
                    g[i, j] = v - '0';
                    return v;
                }
            );

            return (w, h, g);
        }
        /// <summary>
        /// Get file or lines into a char grid. Right pad if needed with char filler
        /// </summary>
        /// <returns></returns>
        public static (int width, int height, char[,] grid) CharGrid(List<string>? lines = null, char filler = ' ')
        {
            if (lines == null)
                lines = ReadLines();
            var w = lines.Max(b => b.Length);
            var h = lines.Count;
            char[,] g = new char[w, h];
            for (var i = 0; i < w; ++i)
            for (var j = 0; j < h; ++j)
            {
                if (i < lines[j].Length)
                    g[i, j] = lines[j][i];
                else
                    g[i, j] = filler;
            }

            return (w, h, g);
        }
    }

    static class MiscTools
    {
        /// <summary>
        /// Tally items, returning list of (count,item)
        /// </summary>
        public static List<(int count, T item)> Tally<T>(IEnumerable<T> items)
        {
            var d = new Dictionary<T, int>();
            foreach (var item in items)
            {
                if (!d.ContainsKey(item))
                    d.Add(item, 0);
                d[item]++;
            }
            return d.Select(p => (p.Value, p.Key)).ToList();
        }
        /// <summary>
        /// Count bits set
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public static int BitCount(long v)
        {
            int count = 0;
            while (v > 0)
            {
                count += (int)(v & 1);
                v >>= 1;
            }
            return count;
        }
        /// <summary>
        /// Create number from base b digits
        /// </summary>
        public static long FromDigits(List<int> digits, int b = 10)
        {
            long v = 0;
            digits.Reverse();
            foreach (var digit in digits)
            {
                v = b * v + digit;
            }
            return v;
        }

        /// <summary>
        /// Create digits base b from number
        /// </summary>
        public static List<int> ToDigits(long v, int b = 10)
        {
            var l = new List<int>();
            while (v > 0)
            {
                l.Add((int)(v % b));
                v /= b;
            }
            l.Reverse();
            return l;
        }
        /// <summary>
        /// Convert text of 0,1 to binary integer
        /// </summary>
        /// <param name="txt"></param>
        /// <returns></returns>
        public  static long BinaryToInteger(string txt)
        {
            long v = 0;
            foreach (var c in txt)
            {
                v = 2 * v + (c == '0' ? 0 : 1);
            }
            return v;
        }

    }

}
#endif