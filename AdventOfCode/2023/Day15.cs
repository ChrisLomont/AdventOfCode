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
            var boxes = Enumerable.Range(0, 256).Select(i => new List<(string label, int lens)>()).ToArray();

            static int Hash(string ht) =>
                Encoding.ASCII.GetBytes(ht).Aggregate(0, (val, c) => (17 * (val + c)) & 255);

            if (!part2)
                return ReadLines().Select(line => line.Split(',').Select(Hash).Sum()).Sum();

            var rr = new Regex(@"(?<label>[a-zA-Z]+)(?<op>-|=)(?<num>\d+)?");

            foreach (var command in ReadLines().SelectMany(line => line.Split(',')))
            {
                var mm = rr.Match(command);
                var label = mm.Groups["label"].Value;
                var remove = mm.Groups["op"].Value[0] == '-';
                var box = boxes[Hash(label)];
                var entry = box.FirstOrDefault(b => b.label == label);

                if (remove)
                {
                    if (entry.label != null)
                        box.Remove(entry);
                }
                else
                {
                    var num = Int32.Parse(mm.Groups["num"].Value);
                    if (entry.label != null)
                        entry.lens = num;
                    else
                        box.Add((label, num));
                }
            }

            return boxes.Select((b, i) => b.Select((p, j) => (i + 1) * (j + 1) * p.lens).Sum()).Sum();
        }
    }
}