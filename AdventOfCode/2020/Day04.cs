namespace Lomont.AdventOfCode._2020
{
    internal class Day04 : AdventOfCode
    {
        //2020 Day 4 part 1: 237 in 8148.5 us
        //2020 Day 4 part 2: 172 in 6695.1 us
        // todo - may be some nicer parsing tricks to abstract out

        public override object Run(bool part2)
        {
            var count = 0;
            var ps = "";
            foreach (var line in ReadLines())
            {
                if (string.IsNullOrEmpty(line))
                {
                    //Console.Write(ps);
                    if (Valid(ps))
                    {
                        ++count;
                    //    Console.WriteLine(" true");
                    }
                    //else
                    //    Console.WriteLine(" false");

                    //Console.WriteLine();

                    ps = "";
                }
                ps += " " + line;
            }
            if (ps != "") 
                count += Valid(ps) ? 1 : 0;

            return count;

            bool Valid(string ps)
            {
                var w = Split(ps);
                w.Sort();
                var pre = w.Select(t => t[0..3]).ToList();
                var a = pre.Aggregate("", (a, b) => a + b);
                var ok = a == "byrcidecleyrhclhgtiyrpid" || a == "byrecleyrhclhgtiyrpid";
                if (part2 && ok)
                    ok &= Valid2(w);
                return ok;
            }

            bool Valid2(List<string> w)
            {
                var t= "byrcidecleyrhclhgtiyrpid";
                for (var i = 0; i < t.Length; i += 3)
                {
                    var b = t[i..(i + 3)];
                    var c = w.Count(t => t.StartsWith(b));
                    if (c > 1)
                        return false;
                }

                foreach (var pre in w)
                {
                    if (pre[3] != ':') 
                        return false;
                    var p = pre[0..3];
                    var i = t.IndexOf(p);
                    if (i < 0 || ((i%3)!=0))
                        return false;
                }


                var ok = true;
                ok &= Match("byr", s => int.TryParse(s, out var n1) && 1920 <= n1 && n1 <= 2002);
                ok &= Match("iyr", s => int.TryParse(s, out var n1) && 2010 <= n1 && n1 <= 2020);
                ok &= Match("eyr", s => int.TryParse(s, out var n1) && 2020 <= n1 && n1 <= 2030);
                ok &= Match("hgt", Ht);
                ok &= Match("hcl", s => Regex.IsMatch(s,@"^#[0-9a-f]{6}$"));
                ok &= Match("ecl", s => Regex.IsMatch(s,"^(amb|blu|brn|gry|grn|hzl|oth)$"));
                ok &= Match("pid", s => Regex.IsMatch(s, "^\\d{9}$"));

                return ok;

                bool Ht(string s)
                {
                    var unit = s[^2..];
                    var tx = s[0..^2];
                    if (int.TryParse(tx, out var ht))
                    {
                        if (unit == "in" && 59 <= ht && ht <= 76)
                            return true;
                        if (unit == "cm" && 150 <= ht && ht <= 193)
                            return true;
                    }

                    return false;

                }




                bool Match(string pref, Func<string,bool> f)
                {
                    var q = w.FirstOrDefault(t => t.StartsWith(pref));
                    if (String.IsNullOrEmpty(q))
                        return false;
                    var s = q[4..];

                    if (f(s)) 
                        return true;
                    return false;

                }
            }



        }
    }
}