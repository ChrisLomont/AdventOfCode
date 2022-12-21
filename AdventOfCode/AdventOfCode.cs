namespace Lomont.AdventOfCode
{
    // baseAOC tools
    /* TODO
    -  Add Set, Multiset, from numbers and string and char and IEnum...
       Union, Diff, Intersection, SymmDiff (Lomont code has it?)

    - add more generic Grid type here, at least 2d 3d 4d, functors, etc
    - generic rotate, flip match edges or overlap, align of grid things
    - Size(grid) return w,h .. for 2d, 3d, 4d
    - better parsing stuff for example Advent 2022 #11
    - graph class, tree class (add to Lomont?) see 2020 day 4 and many others
    - generic memoizer?
    - grid diff locations?
    - gen rays in directions, find first hi item (see 2020 say 11)
    - nbrs on ordinal dirs, all dirs
    - list of dirs
    - perform dihedral actions on grid (see 2020 Day 20)
    - abstract patterns: see 2020 day 4
    - add manhattan distances, hamming distances

        // todo - nice to have n-tree builder from text cleanly
    // example probs 2022: 13, 2021 18, 10, 


    */
    internal abstract class AdventOfCode
    {
        // path from where exe runs to data file, when run in Visual Studio 2022
        public static string DataPath = "../../../";

        public string GetFile(int day)
        {
            var t = this.GetType().FullName;
            Trace.Assert(t != null);
            var year = new Regex(@"\d{4}").Match(t!).Value;
            return $"{DataPath}/{year}/Data/Day{day:D2}.txt";
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
        public static (int w, int h,int d) Size<T>(T[,,] g)
        {
            return (g.GetLength(0), g.GetLength(1),g.GetLength(2));
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
            return d.Select(p=>(p.Value,p.Key)).ToList();
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
        /// Run a problem, part 2 if part2==true
        /// </summary>
        /// <param name="part2"></param>
        /// <returns></returns>
        public abstract object Run(bool part2);

        /// <summary>
        /// Get all lines from the data file
        /// </summary>
        /// <returns></returns>
        protected List<string> ReadLines()
        {
            var dayNumber = Int32.Parse(this.GetType().Name.Substring(3));
            return File.ReadAllLines(GetFile(dayNumber)).ToList();
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
        public static List<int> ToDigits(long v, int b=10)
        {
            var l = new List<int>();
            while (v > 0)
            {
                l.Add((int)(v%b));
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
        protected static long BinaryToInteger(string txt)
        {
            long v = 0;
            foreach (var c in txt)
            {
                v = 2 * v + (c == '0' ? 0 : 1);
            }
            return v;
        }

        /// <summary>
        /// Get file or lines into a digit grid.
        /// </summary>
        /// <returns></returns>
        protected (int width, int height, int[,] grid) DigitGrid(List<string>? lines = null)
        {
            var (w, h, cg) = CharGrid(lines);
            var g = new int[w,h];
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
        protected (int width, int height, char[,] grid) CharGrid(List<string>? lines = null, char filler = ' ')
        {
            if (lines == null)
                lines = ReadLines();
            var w = lines.Max(b => b.Length);
            var h = lines.Count;
            char[,] g = new char[w,h];
            for (var i =0; i < w; ++i)
            for (var j = 0; j < h; ++j)
            {
                g[i, j] = lines[j][i];
            }

            return (w, h, g);
        }

        /// <summary>
        /// pull numbers grid from file or lines
        /// </summary>
        /// <returns></returns>
        protected (int width, int height, long[,] grid) NumberGrid(int startLine = -1, int endLine = -1, List<string>? lines = null)
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

        public void Mutate<T>(IList<T> items, Func<int, T, T> func)
        {
            for (var i = 0; i < items.Count; ++i)
                items[i] = func(i, items[i]);
        }

        // modify item in place, over increasing i, allowing telescroping rewrites
        public void Mutate<T>(IList<T> items, Func<int, T> func)
        {
            for (var i = 0; i < items.Count; ++i)
                items[i] = func(i);
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

        #region Utility

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
        protected List<T> Intersect<T>(params IEnumerable<T> [] lists)
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

        /// <summary>
        /// string to number
        /// </summary>
        protected long Num(string n) => long.Parse(n);

        protected List<T> Union<T>(IEnumerable<T> list1) =>
            Enumerable.Union(list1, list1).ToList();

        protected List<T> RangeLen<T>(IEnumerable<T> list1, int first, int length) =>
            list1.Skip(first).Take(length).ToList();
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list1"></param>
        /// <param name="first">Inclusive</param>
        /// <param name="last">Exclusive</param>
        /// <returns></returns>
        protected List<T> RangeTo<T>(IEnumerable<T> list1, int first, int last) =>
            list1.Skip(first).Take(last-first).ToList();

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
                    output.Write(i+", ");
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
            Apply(grid, (i, j,v) =>
                {
                    Console.Write($"{grid[i,j]}");
                    if (!noComma)
                        Console.Write(',');
                    if (i == m-1)
                        Console.WriteLine();
                    return v;
                }
                );
            Console.WriteLine();
        }

        static Regex numberRegex = new Regex(@"\d+");
        static Regex signedNumberRegex = new Regex(@"(\+|-)?\d+");

        // read all numbers out of string, ignore other stuff
        protected List<long> Numbers64(string line, bool allowSigned = true)

        {
            if (allowSigned)
                return signedNumberRegex.Matches(line).Select(m => long.Parse(m.Value)).ToList();
            return numberRegex.Matches(line).Select(m => long.Parse(m.Value)).ToList();
        }

        // read all numbers out of string, ignore other stuff
        protected static List<int> Numbers(string line, bool allowSigned = true)
        {
            if (allowSigned)
                return signedNumberRegex.Matches(line).Select(m => int.Parse(m.Value)).ToList();
            return numberRegex.Matches(line).Select(m => int.Parse(m.Value)).ToList();
        }

        /// <summary>
        /// Apply action over grid
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="grid"></param>
        /// <param name="func"></param>
        protected static void Apply<T>(T[,] grid, Action<T> func)
        {
            for (var i = 0; i < grid.GetLength(0); ++i)
            for (var j = 0; j < grid.GetLength(1); ++j)
                func(grid[i, j]);
        }

        // i,j,k grid value to new grid value
        protected void Apply<T>(T[,,] grid, Func<int, int, int, T, T> func)
        {
            for (var k = 0; k < grid.GetLength(2); ++k)
            for (var j = 0; j < grid.GetLength(1); ++j)
            for (var i = 0; i < grid.GetLength(0); ++i)
                grid[i, j, k] = func(i, j, k, grid[i, j, k]);
        }
        protected void Apply<T>(T[,,,] grid, Func<int, int, int, int, T, T> func)
        {
            for (var l = 0; l < grid.GetLength(3); ++l)
            for (var k = 0; k < grid.GetLength(2); ++k)
            for (var j = 0; j < grid.GetLength(1); ++j)
            for (var i = 0; i < grid.GetLength(0); ++i)
                grid[i, j, k, l] = func(i, j, k, l, grid[i, j, k, l]);
        }

        // i,j,grid value to new grid value
        protected static int Count<T>(T[,] grid, Func<T,bool> func)
        {
            var count = 0;
            Apply(grid,v=>count += func(v)?1:0);
            return count;
        }

        // i,j,grid value to new grid value
        protected static void Apply<T>(T[,] grid, Func<int,int,T,T> func)
        {
            for (var j = 0; j < grid.GetLength(1); ++j)
            for (var i = 0; i < grid.GetLength(0); ++i)
                grid[i,j] = func(i,j,grid[i, j]);
        }

        /// <summary>
        /// Call action on each (i,j) pair on line, inclusive
        /// </summary>
        /// <param name="x0"></param>
        /// <param name="y0"></param>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="func"></param>
        protected void Line(int x0, int y0, int x1, int y1, Action<int, int> func)
        {
            foreach (var (i, j) in DDA.Dim2(x0, y0, x1, y1))
                func(i, j);
        }

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

            if (i < 0 || j < 0 || k < 0 || w <= i || h <= j || d<=k) return def;
            return grid[i, j,k];
        }

        protected T Get<T>(T[,,,] grid, int i, int j, int k, int q, T def)
        {
            var w = grid.GetLength(0);
            var h = grid.GetLength(1);
            var d = grid.GetLength(2);
            var r = grid.GetLength(3);

            if (i < 0 || j < 0 || k < 0 || q < 0 || w <= i || h <= j || d <= k || r <= q) return def;
            return grid[i, j, k, q];
        }

        /// <summary>
        /// split line into words on char, to list, remove empty
        /// </summary>
        /// <param name="line"></param>
        /// <param name="sp"></param>
        /// <param name="opts"></param>
        /// <returns></returns>
        public static List<string> Split(string line, char sp = ' ', StringSplitOptions opts = StringSplitOptions.RemoveEmptyEntries)
        {
            var words = line.Split(sp, opts);
            return words.ToList();
        }

        public static void Assert(bool val)
        {
            Trace.Assert(val);
        }

        /// <summary>
        /// Given list of strings, and list of tuples (int,regex), assert they match
        /// For validation of assumptions
        /// </summary>
        /// <param name="items"></param>
        /// <param name="checks"></param>
        public static void MatchWords(IList<string> items, params (int, string)[] checks)
        {
            foreach (var (index, pattern) in checks)
            {
                if (!Regex.IsMatch(items[index], pattern))
                {
                    Console.WriteLine($"ERROR: '{items[index]}' does not match '{pattern}'");
                    Environment.Exit(-1);
                }
            }
        }


        #endregion

    }
}
