using System;
using System.ComponentModel.Design;
using System.Net;
using System.Text.Json.Serialization;
using System.Xml;

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


    
        // would be nice to have a BNF parser with regex stuff
        // for example, lines in this item 
        // Game 19: 2 blue; 1 blue, 4 green, 6 red; 7 green, 6 red, 2 blue; 2 blue, 5 red, 4 green; 1 green, 10 red
        // can be described as
        // "Game (#:game): (((#:red red|#:green green|#:blue blue)?,)+;)+
        // want parsed into struct that is
        // struct {
        //    int game;
        //    List<vec3> items;  // or maybe List<(int red,int green,int blue)>....
        // };
        // and this parsed into some nice structure to walk
        //
        // or even a regex tokenizer: give string, list of patterns and maybe discards, and it splits into tokens
        // see prob 2023 #2



    */
    internal abstract class AdventOfCode
    {
        // path from where exe runs to data file, when run in Visual Studio 2022
        public static string DataPath = "../../../";

        public AdventOfCode()
        {
            try
            {
                var filename = GetFileName();
                if (!File.Exists(filename) || (new FileInfo(filename)).Length == 0)
                {
                    var webContent = GetWebfile();
                    File.WriteAllText(filename, webContent);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: trying to read web file {ex}");

            }
        }

        /// <summary>
        /// Run a problem, part 2 if part2==true
        /// </summary>
        /// <param name="part2"></param>
        /// <returns></returns>
        public abstract object Run(bool part2);


        #region files
        /// <summary>
        /// Get input file from web
        /// </summary>
        /// <returns></returns>
        public string GetWebfile()
        {
            var day = Int32.Parse(this.GetType().Name.Substring(3));

            var t = this.GetType().FullName;
            Trace.Assert(t != null);
            var year = new Regex(@"\d{4}").Match(t!).Value;
            // https://adventofcode.com/2023/day/1/input
            var url = $"https://adventofcode.com/{year}/day/{day}/input";

            var webRequest = WebRequest.Create(url);

            void Add(WebRequest wr)
            {
                var ht = wr as HttpWebRequest;
                if (ht == null) 
                    return;
                if (ht.CookieContainer == null)
                    ht.CookieContainer = new CookieContainer();
                var sessionText = File.ReadAllText(Path.Join(DataPath,"session.txt"));
                ht.CookieContainer.Add(
                    new Cookie(
                        "session",
                        sessionText,
                        "",
                        "adventofcode.com"
                    ));
            }

            Add(webRequest);

            using (var response = webRequest.GetResponse())
            using (var content = response.GetResponseStream())
            using (var reader = new StreamReader(content))
            {
                return reader.ReadToEnd();
            }
        }

        public static double Wrap01(double value)
        {
            // todo - prove correct carefully
            return value - Math.Floor(value);
        }

        public static double Wrap(double value, double min, double max)
        {
            if (max == min) return min; // collapse to point
            // todo - prove correct carefully
            var del = max - min;
            var scaled = (value - min) / del; // shifted and scaled so min = 0, max = 1
            var wrapped = Wrap01(scaled);
            var ans = wrapped * del + min;
            Trace.Assert(min <= ans && ans < max);
            return ans;

        }

        /// <summary>
        /// Clamp value into [0,1]
        /// </summary>
        /// <param name="value"></param>
        /// <param name="tolerance"></param>
        /// <returns></returns>
        public static double Clamp01(double value, double tolerance = 0.00001)
        {
            if (0 <= value && value <= 1) return value;
            if (value < 0 && -tolerance < value) return 0;
            if (value > 1 && 1 + tolerance > value) return 1;

            Trace.TraceError("Clamp01 color parse error");
            throw new Exception("Value out of tolerance");
        }

        static (double red, double green, double blue) HSLVtoRGB(
           double hue, double saturation, double lightnessOrValue, bool useValue
       )
        {
            hue = Wrap(hue, 0, 360);
            var lv = lightnessOrValue;
            var s = saturation;

            double red, green, blue;
            const double tolerance = 0.0001; // values this close to 0 are treated as equal
            if (saturation <= tolerance)
            {
                // achromatic (grey)
                red = green = blue = lightnessOrValue;
            }
            else
            { // chromatic case
                double c, m;
                if (useValue)
                {
                    c = lv * s;
                    m = lv - c;
                }
                else
                {
                    c = (1 - Math.Abs(2 * lv - 1)) * s; // chroma
                    m = lv - c * 0.5;
                }

                var hp = hue / 60; // sector 0 to 5
                double ab = (hp / 2 - Math.Floor(hp / 2)) * 2; // hp mod 2
                double x = c * (1 - Math.Abs(ab - 1));
                red = green = blue = 0;
                if (hp < 1)
                {
                    red = c;
                    green = x;
                }
                else if (hp < 2)
                {
                    red = x;
                    green = c;
                }
                else if (hp < 3)
                {
                    green = c;
                    blue = x;
                }
                else if (hp < 4)
                {
                    green = x;
                    blue = c;
                }
                else if (hp < 5)
                {
                    blue = c;
                    red = x;
                }
                else if (hp < 6)
                {
                    blue = x;
                    red = c;
                }
                red += m;
                green += m;
                blue += m;
            }
            // sanity check
            double tolerance2 = 0.005;
            Trace.Assert(0 - tolerance2 <= red && red <= 1.0 + tolerance2);
            Trace.Assert(0 - tolerance2 <= green && green <= 1.0 + tolerance2);
            Trace.Assert(0 - tolerance2 <= blue && blue <= 1.0 + tolerance2);
            red = Clamp01(red);
            green = Clamp01(green);
            blue = Clamp01(blue);

            return (red, green, blue);
        }

        // uhue 0-360, others 0-1
        public static (double r, double g, double b) HSL2RGB(double h, double s, double l)
        {
            var (r,g,b) = HSLVtoRGB(h, s, l, false);
            return (r, g, b);
        }

        public string GetFileName()
        {
            var day = Int32.Parse(this.GetType().Name.Substring(3));

            var t = this.GetType().FullName;
            Trace.Assert(t != null);
            var year = new Regex(@"\d{4}").Match(t!).Value;
            return $"{DataPath}/{year}/Data/Day{day:D2}.txt";
        }

        /// <summary>
        /// Get all lines from the data file
        /// </summary>
        /// <returns></returns>
        protected List<string> ReadLines() => File.ReadAllLines(GetFileName()).ToList();

        /// <summary>
        /// Get all text
        /// </summary>
        /// <returns></returns>
        protected string ReadText() => File.ReadAllText(GetFileName());
    

        #endregion

        #region numbers
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
        protected static long BinaryToInteger(string txt)
        {
            long v = 0;
            foreach (var c in txt)
            {
                v = 2 * v + (c == '0' ? 0 : 1);
            }
            return v;
        }
        #endregion

        #region grid
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

        // iterate down row
        public static IEnumerable<T> Row<T>(T[,] g, int row)
        {
            var (w, _) = Size(g);
            for (var col = 0; col < w; ++col)
                yield return g[col, row];
        }
        // iterate down col
        public static IEnumerable<T> Col<T>(T[,] g, int col)
        {
            var (_, h) = Size(g);
            for (var row = 0; row < h; ++row)
                yield return g[col, row];
        }

        // enumerate (i,j) of things that match predicate
        public static IEnumerable<vec2> Gather<T>(T[,] g, Func<T, bool> predicate)
        {
            var (w, h) = Size(g);
            for (var i = 0; i < w; ++i)
            for (var j = 0; j < h; ++j)
            {
                if (predicate(g[i, j]))
                    yield return new vec2(i, j);
            }

        }
        public static IEnumerable<vec2> Gather<T>(T[,] g, Func<T, int,int, bool> predicate)
        {
            var (w, h) = Size(g);
            for (var i =0; i < w; ++i)
            for (var j = 0; j < h; ++j)
            {
                if (predicate(g[i,j],i,j))
                    yield return new vec2(i, j);
            }

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
        public static T Get<T>(T[,] grid, vec2 v, T def)
        {
            return Get(grid, v.x, v.y, def);
        }
        public static T Get<T>(T[,,] grid, int i, int j, int k, T def)
        {
            var w = grid.GetLength(0);
            var h = grid.GetLength(1);
            var d = grid.GetLength(2);

            if (i < 0 || j < 0 || k < 0 || w <= i || h <= j || d <= k) return def;
            return grid[i, j, k];
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
        protected (int width, int height, char[,] grid) CharGrid(IEnumerable<string>? lines1 = null, char filler = ' ')
        {
            List<string> lines;

            if (lines1 == null)
                lines = ReadLines();
            else
                lines = lines1.ToList();
            var w = lines.Max(b => b.Length);
            var h = lines.Count;
            char[,] g = new char[w,h];
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

        protected static void Apply<T>(T[,] grid, Func<T, T> func)
        {
            for (var i = 0; i < grid.GetLength(0); ++i)
            for (var j = 0; j < grid.GetLength(1); ++j)
                grid[i, j] = func(grid[i, j]);
        }
        protected static void Apply<T>(T[,,] grid, Func<T, T> func)
        {
            for (var i = 0; i < grid.GetLength(0); ++i)
            for (var j = 0; j < grid.GetLength(1); ++j)
            for (var k = 0; k < grid.GetLength(2); ++k)
                grid[i, j, k] = func(grid[i, j, k]);
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
        protected static int Count<T>(T[,] grid, Func<T, bool> func)
        {
            var count = 0;
            Apply(grid, v => count += func(v) ? 1 : 0);
            return count;
        }

        // i,j,grid value to new grid value
        protected static void Apply<T>(T[,] grid, Func<int, int, T, T> func)
        {
            for (var j = 0; j < grid.GetLength(1); ++j)
            for (var i = 0; i < grid.GetLength(0); ++i)
                grid[i, j] = func(i, j, grid[i, j]);
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

        #endregion

        #region parsers
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

        /// <summary>
        /// string to number
        /// </summary>
        protected long Num(string n) => long.Parse(n);

        static Regex numberRegex = new(@"\d+");
        static Regex signedNumberRegex = new(@"(\+|-)?\d+");



        // read all numbers out of string, ignore other stuff
        protected static List<long> Numbers64(string line, bool allowSigned = true)
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


        protected class ActionList : List<(Regex regex, string type, int index)>
        {
            Action<string> endAction = s => { };

            List<Action<string>> strActions = new();
            List<Action<string,int>> strIntActions = new();

            public void EndAction()
            {
                if (endIndex != -1)
                    DoOne(endType, endIndex, "");
            }

            int endIndex = -1;
            string endType = "";

            void AddOne(string type, string pattern, int index)
            {
                if (String.IsNullOrEmpty(pattern))
                {
                    endIndex = index;
                    endType = type;
                }
                else
                {
                    var reg = new Regex("^" + pattern);
                    Add(new(reg, type, index));
                }
            }

            public void DoOne(string type, int index, string match)
            {
                switch (type)
                {
                    case "str+int":
                        int n = Numbers(match)[0];
                        strIntActions[index](match, n);
                        break;
                    case "str":
                        strActions[index](match);
                        break;
                    default:
                        Console.WriteLine($"ERROR: unknown parse stype {type}");
                        break;
                }
            }

            public void Add(string pattern, Action<string,int> action)
            {
                strIntActions.Add(action);
                AddOne("str+int",pattern,strIntActions.Count-1);
            }

            public void Add(string pattern, Action<string> action)
            {
                strActions.Add(action);
                AddOne("str", pattern, strActions.Count - 1);
            }
        }

        /// <summary>
        /// Given line, and patterns to walk it, and actions, do it
        /// See 2023, Day2, for example
        /// 
        /// </summary>
        protected static void PatternActions(string line, ActionList patterns)
        {
            var n = patterns.Count;
            while (line.Length > 0)
            {
                int i = 0;
                while (i < n)
                {
                    var (reg, type,actIndex) = patterns[i];
                
                    var m = reg.Match(line);
                    if (m.Success)
                    {
                        var matchText = m.Groups[0].Value;
                        patterns.DoOne(type,actIndex,matchText);
                        line = line.Substring(matchText.Length);
                        break;
                    }
                    ++i;
                }
                if (i >= n)
                {
                    Console.WriteLine($"Error: parser match ended at {line}");
                    return;
                }
            }

            if (line.Length == 0)
                patterns.EndAction();

        }

        protected void ProcessAllLines(ActionList patterns)
        {
            foreach (var line in ReadLines())
                PatternActions(line,patterns);
        }





        #endregion

        #region set intersect, union, etc

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


        protected List<T> Union<T>(IEnumerable<T> list1) =>
            Enumerable.Union(list1, list1).ToList();
        #endregion

        #region ranges
        // from items, take range [first, first+length)
        protected List<T> RangeLen<T>(IEnumerable<T> list1, int first, int length) =>
            list1.Skip(first).Take(length).ToList();

        /// <summary>
        /// Take rane from first to last
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list1"></param>
        /// <param name="first">Inclusive</param>
        /// <param name="last">Exclusive</param>
        /// <returns></returns>
        protected List<T> RangeTo<T>(IEnumerable<T> list1, int first, int last) =>
            list1.Skip(first).Take(last - first).ToList();

        #endregion

        #region output

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


        // map (item,i,j) to (is colored, foreground hex color)
        // background color can be appended with ;#rrggbb
        public class DumpColors<T> : List<Func<T, int,int, (bool match, string)>>
        {

        }
        /// <summary>
        /// Dump 2d grid
        /// </summary>
        protected static void Dump<T>  (
            T[,] grid,
            bool noComma = false,
            DumpColors<T> ? colors = null
        ) 
        {
            var m = grid.GetLength(0);
            var (back, fore) = (Console.BackgroundColor, Console.ForegroundColor);

            (int r, int g, int b) FromHexColor(string s)
            {
                s = s.ToUpper();
                int Hex(string s2)
                {
                    var hs = "0123456789ABCDEF";
                    return hs.IndexOf(s2[0]) * 15 + hs.IndexOf(s2[1]);
                }

                if (s.StartsWith('#')) s= s.Substring(1);
                var r = Hex(s.Substring(0,2));
                var g = Hex(s.Substring(2, 2));
                var b = Hex(s.Substring(4, 2));
                return (r, g, b);
            }

            void Restore()
            {
                Console.ForegroundColor = fore;
                Console.BackgroundColor = back;
            }

            Apply(grid, (i, j, v) =>
                {
                    var ch = grid[i, j];
                    var colored = false;
                    if (colors != null)
                    {
                        foreach (var s in colors)
                        {
                            var (match, color) = s(ch,i,j);
                            if (match)
                            { //Console.WriteLine("\x1b[36mTEST\x1b[0m");

                                // ESC[38;2;⟨r⟩;⟨g⟩;⟨b⟩m Select RGB foreground color
                                // ESC[48; 2;⟨r⟩;⟨g⟩;⟨b⟩m Select RGB background color
                                var cc = color.Split(';');
                                var (r, g, b) = FromHexColor(cc[0]);
                                Console.Write($"\x1b[38;2;{r};{g};{b}m");
                                if (cc.Length > 1)
                                {
                                    (r, g, b) = FromHexColor(cc[1]);
                                     Console.Write($"\x1b[48;2;{r};{g};{b}m");
                                }
                                //Console.ForegroundColor = color;
                                colored = true;
                                break;
                            }
                        }
                    }

                    Console.Write($"{ch}");
                    if (colored)
                    {
                        Console.Write("\x1b[0m");
                    }
                    if (colors != null)
                    {
                        Restore();
                    }

                    if (!noComma)
                        Console.Write(',');
                    if (i == m - 1)
                        Console.WriteLine();
                    return v;
                }
            );
            Console.WriteLine();
        }

        #endregion

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
            return d.Select(p => (p.Value, p.Key)).ToList();
        }

        /// <summary>
        /// Mutate collection in place
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <param name="func"></param>
        public void Mutate<T>(IList<T> items, Func<int, T, T> func)
        {
            for (var i = 0; i < items.Count; ++i)
                items[i] = func(i, items[i]);
        }

        /// <summary>
        /// Mutate collection in place
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <param name="func"></param>
        // modify item in place, over increasing i, allowing telescoping rewrites
        public void Mutate<T>(IList<T> items, Func<int, T> func)
        {
            for (var i = 0; i < items.Count; ++i)
                items[i] = func(i);
        }

    

    }
}
