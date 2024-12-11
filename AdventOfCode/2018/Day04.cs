

using System.Globalization;

namespace Lomont.AdventOfCode._2018
{
    internal class Day04 : AdventOfCode
    {
        /*
         * 2018 Day 4 part 1: 125444 in 78877 us
2018 Day 4 part 2: 18325 in 53525.2 us
         */

        // guard -1 = wake, -2 = sleep

        public override object Run(bool part2)
        {
            int wake = -1, sleep = -2;
            long answer = 0;
            // guard -1 = wake, -2 = sleep
            List < (DateTime date, int guard)> items = new List<(DateTime date, int)>();
            foreach (var line in ReadLines())
            {
                var m = Regex.Match(line, "^\\[(?<date>[0-9:\\- ]+)\\] ((Guard #(?<guard>[0-9]+) begins shift)|(?<wake>wakes up)|(?<sleep>falls asleep))$");
                var d = m.Groups["date"].Value;
                var g1 = m.Groups["guard"].Value;
                var w1 = m.Groups["wake"].Value;
                var s = m.Groups["sleep"].Value;
                Trace.Assert(m.Success);
                //DateTime d1 = new DateTime(1518, 11, 01, 0, 0, 0);
                var dd = DateTime.ParseExact(d,"yyyy-MM-dd HH:mm",CultureInfo.InvariantCulture);
                Console.WriteLine($"{dd}: {g1} {w1} {s}");
                //1518-11-01 00:00
                int guard = 0;
                if (g1.Length > 0) guard = Int32.Parse(g1);
                else if (w1.Length > 0) guard = wake;
                else if (s.Length > 0) guard = sleep;
                else throw new NotImplementedException();
                items.Add((dd,guard));
            }

            items.Sort((a,b)=>a.date.CompareTo(b.date));

            var min = items.Min(q => q.date);
            var max = items.Max(q => q.date);
            var minD = items.Min(q => q.date.Hour*60+q.date.Minute);
            var maxD = items.Max(q => q.date.Hour*60 + q.date.Minute);
            Console.WriteLine();
            Console.WriteLine($"{min} to {max}, {minD} to {maxD}");



            int w = 60, h = 5000;
            int left = 20;
            int top = 10;

            char[,] g = new char[w+left,h+top];
            Apply(g, c => '.');
            int cur = -3, sleepT = -1, wakeT = -1;
            int row = -1;
            int [] alseep = new int[h+top];
            int [] ids = new int[h+top];
            foreach (var q in items)
            {
                if (q.guard > 0)
                {
                    ++row;
                    cur = q.guard;
                    Trace.Assert(cur > 0);
                    g[0, row+top] = (char)(cur / 10+'0');
                    g[1, row+top] = (char)(cur %10 + '0');
                    g[2, row+top] = ':';
                    //Trace.Assert(0 <= cur && cur <= 255);
                    ids[row] = cur;
                }
                else if (q.guard == sleep)
                {
                    sleepT = q.date.Minute;
                }
                else if (q.guard == wake)
                {
                    wakeT = q.date.Minute;
                    for (int i = sleepT; i < wakeT; ++i)
                        g[i+left, row+top] = '#';
                    alseep[cur] += wakeT - sleepT;
                }
            }

           // Dump(g, noComma:  true);

            int maxSleep = alseep.Max();
            int sleepIndex = Array.IndexOf(alseep,maxSleep);
            Console.WriteLine($"max sleep {maxSleep} {sleepIndex}");

            int [] sleepMin = new int [60];
            for (int rowC = 0; rowC < h; ++rowC)
            {
                if (ids[rowC] == sleepIndex)
                {
                    for (int j = left; j < w + left; ++j)
                    {
                        if (g[j,rowC+top] == '#')
                            sleepMin[j-left]++;
                    }
                }
            }

            var maxm = sleepMin.Max();
            var maxmIndex = Array.IndexOf(sleepMin, maxm);
            Console.WriteLine($"{maxm}->{maxmIndex}");

            



            if (!part2)
                return maxmIndex* sleepIndex;

            // map guard index to list of minutes
            Dictionary<int, int[]> gmins = new();

            for (int rowC = 0; rowC < h; ++rowC)
            {
                var guard = ids[rowC];
                if (!gmins.ContainsKey(guard))
                    gmins.Add(guard, new int[60]);
                for (int j = left; j < w + left; ++j)
                {
                    if (g[j, rowC + top] == '#')
                        gmins[guard][j-left]++;
                }
            }

            // find max one
            int mx = 0;
            foreach (var p in gmins)
            {
                var mm = p.Value.Max();
                if (mm > mx)
                {
                    mx = mm;
                    var ind = Array.IndexOf(p.Value,mx);
                    answer = ind* p.Key;
                    Console.WriteLine($"Better guard {p.Key} sleeps {mx} on min {ind}");
                }
            }

            return answer;

        }
      
    }
}