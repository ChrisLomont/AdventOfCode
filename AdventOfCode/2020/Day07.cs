using System.Collections;
using System.IO.MemoryMappedFiles;
using System.Threading.Channels;

namespace Lomont.AdventOfCode._2020
{
    internal class Day07 : AdventOfCode
    {
        // todo - pull out directed graph use it
        // map tree over graph?

    // 2020 Day 7 part 1: 261 in 25492.4 us
    // 2020 Day 7 part 2: 3765 in 74465.3 us
        public override object Run(bool part2)
        {
            Dictionary<string, List<(int count, string name)>> map = new();

            foreach (var line in ReadLines())
            {
                var outer = Regex.Match(line,"(?<b1>[a-z ]+) bags contain ");
                var inner = Regex.Matches(line, @"(?<cnt>\d+) (?<name>[a-z ]+) bag[s]?");
                var key = outer.Groups["b1"].Value;
                if (!map.ContainsKey(key))
                    map.Add(key, new List<(int count, string name)>());
                //Console.WriteLine("Key " + key);
                foreach (Match m in inner)
                {
                    var (i, s) = (int.Parse(m.Groups["cnt"].Value), m.Groups["name"].Value);
                    //Console.WriteLine($"{i} {s}");
                    map[key].Add((i,s));
                }
            }

            var targ = "shiny gold";
            var q = new Queue<string>();
            var h = new HashSet<string>();
            foreach (var v in Higher(targ))
                q.Enqueue(v);

            while (q.Any())
            {
                var p = q.Dequeue();
                if (!h.Contains(p))
                {
                    //Console.WriteLine($"Higher: {p} = ");
                    //Dump(Higher(p).ToList());
                    //Console.WriteLine();

                    h.Add(p);
                    foreach (var v in Higher(p))
                        q.Enqueue(v);
                }
            }

            if (!part2)
                return h.Count;

            var count = 0L;
            var qq = new Queue<(long mul, string name)>();
            qq.Enqueue(new (1,targ));
            while (qq.Any())
            {
                var (mul,name1) = qq.Dequeue();
                var t = map[name1];
                //Console.WriteLine($"{name1} contains");
                foreach (var (cnt,name) in t)
                {
                   // Console.WriteLine($"  {cnt} of {name}");
                    count +=mul*cnt;
                    qq.Enqueue((mul*cnt,name));
                }
            }

            return count;



            IEnumerable<string> Higher(string targ)
            {
                foreach (var (key, val) in map)
                {
                    if (val.Any(v => v.name == targ))
                    {
                        yield return key;
                    }

                }
            }


            return -1;
        }
    }
}