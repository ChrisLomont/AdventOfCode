
namespace Lomont.AdventOfCode._2021
{
    internal class Day14 : AdventOfCode
    {
        public override object Run(bool part2)
        {
            // track counts of pairs
            Dictionary<string, long> cells = new();

            List<(string src, (string p1,string p2))> rules = new ();
            var lines = ReadLines();
            var poly = lines[0];

            for (var i = 0; i < poly.Length-1; ++i)
            {
                var p2 = poly.Substring(i, 2);
                if (!cells.ContainsKey(p2))
                    cells.Add(p2, 0);
                cells[p2]++;
            }

            for (var i = 2; i < lines.Count; i++)
            {
                var ww = lines[i].Split(" -> ");
                rules.Add(new(ww[0],
                    (
                        ww[0][0] + ww[1],
                        ww[1] + ww[0][1]
                    )
                ));
            }

            var top = part2 ? 40 : 10;
            for (var i = 0; i < top; ++i)
            {
                var temp = new Dictionary<string,long>();
                foreach (var (pair, count) in cells)
                {
                    var r = rules.FirstOrDefault(r => r.src == pair);
                    if (r != default((string, (string,string))))
                    {
                        Check(temp, r.Item2.p1);
                        Check(temp, r.Item2.p2);
                        temp[r.Item2.p1] += count;
                        temp[r.Item2.p2] += count;
                    }
                    else
                    {
                        Check(temp,pair);
                        temp[pair] += count;
                    }

                }

                cells = temp;
            }
            void Check<T>(Dictionary<T, long> d, T p) where T : notnull
            {
                if (!d.ContainsKey(p))
                    d.Add(p, 0);
            }

            var counts = new Dictionary<char, long>();
            foreach (var (key,count) in cells)
            {
                Check(counts, key[0]);
                Check(counts, key[1]);
                counts[key[0]] += count;
                counts[key[1]] += count;
            }

            // all double counted except poly 0 and end
            counts[poly[0]]++;
            counts[poly.Last()]++;

            var v = counts.Values.ToList();
            return (v.Max() - v.Min())/2;
        }
    }
}