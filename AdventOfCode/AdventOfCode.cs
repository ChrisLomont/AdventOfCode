﻿namespace Lomont.AdventOfCode
{
    // baseAOC tools
    /* TODO
    -  Add Set, Multiset, from numbers and string and char and IEnum...
       Union, Diff, Intersection, SymmDiff (Lomont code has it?)

    - add more generic Grid type here, at least 2d 3d 4d, functors, etc
    - generic rotate, flip match edges or overlap, align of grid things
    - Size(grid) return w,h .. for 2d, 3d, 4d
    - better parsing stuff for example Advent 2022 #11


    */
    internal abstract class AdventOfCode
    {
        // path from where exe runs to data file, when run in Visual Studio 2022
        string path = "../../../";

        public string GetFile(int day)
        {
            var t = this.GetType().FullName;
            Trace.Assert(t != null);
            var year = new Regex(@"\d{4}").Match(t!).Value;
            return $"{path}/{year}/Data/Day{day:D2}.txt";
        }


        /// <summary>
        /// Run a problem, part 2 if part2==true
        /// </summary>
        /// <param name="part2"></param>
        /// <returns></returns>
        public abstract object Run(bool part2);

        // enumerate over data file
        protected List<string> ReadLines()
        {
            var dayNumber = Int32.Parse(this.GetType().Name.Substring(3));
            return File.ReadAllLines(GetFile(dayNumber)).ToList();
        }

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

        // 01 string to integer
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

        public void Mutate<T>(IList<T> items, Func<int, T> func)
        {
            for (var i = 0; i < items.Count; ++i)
                items[i] = func(i);
        }

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

        // string to number
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
        /// Dump list
        /// </summary>
        public static void Dump<T>(IEnumerable<T> items)
        {
            foreach (var i in items)
                Console.WriteLine(i);
        }

        protected void Dump<T>(T[,] grid, bool noComma = false)
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

        Regex numberRegex = new Regex(@"\d+");
        Regex signedNumberRegex = new Regex(@"(\+|-)?\d+");

        // read all numbers out of string, ignore other stuff
        protected List<long> GetNumbers64(string line) => numberRegex.Matches(line).Select(m => long.Parse(m.Value)).ToList();

        // read all numbers out of string, ignore other stuff
        protected List<int> Numbers(string line, bool allowSigned = true)
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
        protected void Apply<T>(T[,] grid, Action<T> func)
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
        protected void Apply<T>(T[,] grid, Func<int,int,T,T> func)
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
