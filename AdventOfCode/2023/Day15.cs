namespace Lomont.AdventOfCode._2023
{
    internal class Day15 : AdventOfCode
    {
        /*
         


        First hundred users to get both stars on Day 15:

  1) Dec 15  00:01:26  Nordine Lotfi
  2) Dec 15  00:03:00  Karan Vijayakumar
  3) Dec 15  00:03:03  (anonymous user #1508761)
  4) Dec 15  00:03:48  yusent
  5) Dec 15  00:03:57  Coder Gautam
  6) Dec 15  00:03:58  eagely (AoC++)
  7) Dec 15  00:04:40  Роман Курмаев

 96) Dec 15  00:11:00  Magnus Hokland Hegdahl
 97) Dec 15  00:11:00  JYC (AoC++)
 98) Dec 15  00:11:00  Michael Broughton
 99) Dec 15  00:11:02  Paul Swingle
100) Dec 15  00:11:04  Strikeeaglechase
First hundred users to get the first star on Day 15:

  1) Dec 15  00:00:51  Nordine Lotfi
  2) Dec 15  00:00:53  Peter Kuhar
  3) Dec 15  00:01:02  xiaowuc1
  4) Dec 15  00:01:11  (anonymous user #1508761)

              --------Part 1---------   --------Part 2--------
Day       Time    Rank  Score       Time   Rank  Score
 15   00:05:45    1600      0   00:29:36   2090      0
 14   00:06:13     380      0   00:37:35   1066      0
 13   00:19:20    1004      0   00:57:07   2592      0

95) Dec 15  00:02:09  Tristan Sharma
 96) Dec 15  00:02:09  Ben
 97) Dec 15  00:02:10  JYC (AoC++)
 98) Dec 15  00:02:10  Vince7778
 99) Dec 15  00:02:10  Christian Cederquist (AoC++)
100) Dec 15  00:02:10  Jasper van Merle (AoC++)

        517315        247763

         */
        public override object Run(bool part2)
        {
            var boxes = new List<(string label, int lens)>[256];
            for (var i = 0; i < 256; ++i)
                boxes[i] = new();

            long answer1 = 0;

            static int Hash(string ht) =>
                Encoding.ASCII.GetBytes(ht).Aggregate(0, (val, c) => (17 * (val + c)) & 255);

            foreach (var line in ReadLines())
            {
                var commands = line.Split(',');
                foreach (var command in commands)
                {
                    answer1 += Hash(command);

                    if (part2)
                    {
                        var csplit = command.Split(new char[] { '-', '=' });
                        var label = csplit[0];
                        var val = Hash(label);
                        var index = boxes[val].FindIndex(b => b.label == label);

                        if (command.Contains('-'))
                        {
                            if (index>=0) // remove lens
                                boxes[val].RemoveAt(index);
                        }
                        else if (command.Contains('='))
                        {
                            var num = Int32.Parse(csplit[1]);
                            if (index<0) // add a new
                                boxes[val].Add((label, num));
                            else // replace
                                boxes[val][index] = (label, num);
                        }
                        else
                            throw new Exception("error!");


#if false
                    Console.WriteLine();

                    Console.WriteLine($"{w} {val}");

                    for (var i = 0; i < boxes.Length; i++)
                    {
                        if (boxes[i].Count > 0)
                        {
                            Console.Write($"Box {i}:");
                            foreach (var c in boxes[i])
                                Console.Write($"[{c}] ");
                            Console.WriteLine();
                        }
                    }
#endif
                    }
                }
            }

            if (part2)
            {
                // scoring
                long answer2 = 0;
                for (var i = 0; i < boxes.Length; ++i)
                    for (var j = 0; j < boxes[i].Count; ++j)
                        answer2 += (i+1) * (j+1) * boxes[i][j].lens;

                return answer2;
            }

            return answer1;
        }
    }
}