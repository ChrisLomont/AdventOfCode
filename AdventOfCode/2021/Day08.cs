namespace Lomont.AdventOfCode._2021
{
    internal class Day08 : AdventOfCode
    {
        // chara a,b,c,d,e,f,g
        int ToInt(string s)
        {
            int val = 0;
            foreach (var c in s)
                val |= 1 << (c - 'a');
            return val;
        }

        int BitSet(int v)
        {
            int b = 0;
            while (v > 1)
            {
                v >>= 1;
                b++;
            }
            return b;
        }

        char ChBck(int v) => (char) ('a' + BitSet(v));


        public override object Run(bool part2)
        {
            // digits 0-9 segments
            List<string> digs = new()
            {
                "abcefg",
                "cf","acdeg","acdfg","bcdf","abdfg",
                "abdefg", // 6

                "acf","abcdefg","abcdfg","abcefg"
            };

            var count = 0;
            foreach (var line in ReadLines())
            {
                var left = line.Substring(0,line.IndexOf('|')-1);
                var right = line.Substring(line.IndexOf('|') + 1);

                var wr = right.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                var wl = left.Split(' ', StringSplitOptions.RemoveEmptyEntries);

                if (part2)
                {
                    var cf = ToInt(wl.First(t => t.Length == 2)); // c,f set
                    var acf  = ToInt(wl.First(t => t.Length == 3)); 
                    var bcdf = ToInt(wl.First(t => t.Length == 4));
                    var abcdefg = ToInt(wl.First(t => t.Length == 7));

                    var a = acf & (~cf);

                    // size 5s, intersected
                    var size5 = wl.Where(t => t.Length == 5).Select(ToInt).ToList();
                    var adg = size5[0] & size5[1] & size5[2];
                    // size 6s, intersected
                    var size6 = wl.Where(t => t.Length == 6).Select(ToInt).ToList();
                    var abfg = size6[0] & size6[1] & size6[2];

                    var bd = bcdf & ~(acf);

                    var d = adg&bd;
                    var b = bd & ~d;
                    var f = abfg & cf;
                    var c = cf & ~f;
                    var ag = adg & abfg;
                    var g = ag & ~a;
                    var e = abcdefg&~(a|b|c|d|f|g);

                    var map = new Dictionary<char, char>();
                    map.Add(ChBck(a),'a');
                    map.Add(ChBck(b), 'b');
                    map.Add(ChBck(c), 'c');
                    map.Add(ChBck(d), 'd');
                    map.Add(ChBck(e), 'e');
                    map.Add(ChBck(f), 'f');
                    map.Add(ChBck(g), 'g');

                    var value = 0;
                    foreach (var w in wr)
                    {
                        var cc = w.Select(c => map[c]).ToList();
                        cc.Sort();
                        var ss = new string(cc.ToArray());
                        var digit = digs.IndexOf(ss);
                        Trace.Assert(0 <= digit && digit <= 9);
                        value = 10 * value + digit;
                    }

                    //Console.WriteLine("Value: " + value);

                    count += value;

                    // 61229 low
                    // high?
                    // 978171
                }
                else
                {


                    var n1 = wr.Count(t => t.Length == 2);
                    var n4 = wr.Count(t => t.Length == 4);
                    var n7 = wr.Count(t => t.Length == 3);
                    var n8 = wr.Count(t => t.Length == 7);
                    count += n1 + n4 + n7 + n8;
                }
            }
            return count;
        }

        string Diff(string s1, string s2)
        {
            var s3 = "";
            foreach (var c in s1)
                if (!s2.Contains(c))
                    s3 += c;
            return s3;
        }

    }
}