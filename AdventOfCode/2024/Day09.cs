

using System.Runtime.InteropServices.JavaScript;
using System.Xml.Schema;

namespace Lomont.AdventOfCode._2024
{
    internal class Day09 : AdventOfCode
    {
        /*
    2024 Day 9 part 1: 6360094256423 in 3928.9 us
    2024 Day 9 part 2: 6379677752410 in 653816.9 us
        */


        /*
2024 Day 9 part 1: 6360094256423 in 4074.3 us
2024 Day 9 part 2: 6379677752410 in 406048.9 us
         */




        object Run1()
        {
            List<long> ids = new();
            int id = 0;
            var line = ReadText();

            for (int i = 0; i < line.Length; i++)
            {
                var len = line[i] - '0';
                for (int j = 0; j < len; ++j)
                    ids.Add((i & 1) == 0 ? id : -1);
                if ((i & 1) == 0)
                    id++;

            }

            int a = 0, b = ids.Count - 1;
            int n = ids.Count - 1;
            while (a < b)
            {
                while (a < n && ids[a] != -1) a++;
                while (b > 0 && ids[b] == -1) b--;
                if (a < b)
                {
                    ids[a] = ids[b];
                    ids[b] = -1;
                }
            }

            return ids.Select((id, pos) => (n: id, p: pos)).Sum(c => c.n == -1 ? 0 : c.p * c.n);
        }


        object Run2()
        {
            int pos = 0, id = 0; 
            List<(long id, long pos, long len)> files = new();
            List<(long pos, long len)> gaps = new();
            var line = ReadText();
            for (int i = 0; i < line.Length; i++)
            {
                var len = line[i] - '0';
                if ((i & 1) == 0)
                    files.Add(new(id++, pos, len));
                else
                    gaps.Add(new(pos, len));
                pos += len;
            }

            for (int i = files.Count-1; i>=0; i--)
            {
                var f = files[i];
                for (int j = 0; j < gaps.Count;++j)
                {
                    var g = gaps[j];
                    if (f.len <= g.len && f.pos > g.pos)
                    {
                        files[i] = new(f.id, g.pos, f.len);
                        gaps[j]  = new(g.pos + f.len, g.len - f.len);
                        break;
                    }
                }
            }

            files.Sort((a, b) => a.pos.CompareTo(b.pos));

            return files.Sum(
                f=> f.id * (f.pos * f.len + (f.len - 1) * f.len / 2)
                );
        }

        //public override string TestFileSuffix() => "-test1";
        public override string TestFileSuffix() => "";

        public override object Run(bool part2)
        {
            return part2 ? Run2() : Run1();
        }
    }
}