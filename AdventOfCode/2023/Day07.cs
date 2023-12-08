using System.Security.Cryptography;

namespace Lomont.AdventOfCode._2023
{
    internal class Day07 : AdventOfCode
    {


        /*

        2023 Day 7 part 1: 249483956 in 800430.9 us
        2023 Day 7 part 2: 252137472 in 705299.1 us

             --------Part 1---------   --------Part 2--------
Day       Time    Rank  Score       Time   Rank  Score
  7   00:30:34    2592      0   00:38:43   1609      0
  6   00:05:05     515      0   00:14:05   2506      0
  5   00:23:11    1843      0   01:43:15   3581      0
  4   00:09:08    2699      0   00:21:16   2451      0
  3   00:19:04    1331      0   00:29:57   1410      0
  2   00:06:03     329      0   00:09:23    411      0
  1   17:45:07  131162      0   18:25:46  92867      0


        First hundred users to get both stars on Day 7:

          1) Dec 07  00:08:45  tckmn
          2) Dec 07  00:09:01  dan-simon
          3) Dec 07  00:09:05  Balint R
          4) Dec 07  00:09:27  Daniel Yang
          5) Dec 07  00:09:28  5space
          6) Dec 07  00:09:33  bluepichu
          7) Dec 07  00:09:37  mserrano

         95) Dec 07  00:15:36  D. Salgado
 96) Dec 07  00:15:38  Kevin Wang (AoC++)
 97) Dec 07  00:15:42  Jan Verbeek
 98) Dec 07  00:15:48  Noah Overcash (AoC++)
 99) Dec 07  00:15:55  Anthony Li
100) Dec 07  00:16:00  EIFY

        First hundred users to get the first star on Day 7:

  1) Dec 07  00:04:52  (anonymous user #1556836)
  2) Dec 07  00:05:17  dan-simon
  3) Dec 07  00:05:28  Nicky Skybytskyi
  4) Dec 07  00:05:50  jonathanpaulson (AoC++)
  5) Dec 07  00:05:56  piman51277
  6) Dec 07  00:06:01  mserrano
  7) Dec 07  00:06:16  Balint R

         96) Dec 07  00:09:50  Greg Zborovsky
 97) Dec 07  00:09:51  ilg
 98) Dec 07  00:09:53  Mathijs Vogelzang
 99) Dec 07  00:09:53  Bradon Zhang
100) Dec 07  00:09:57  zhangjunyan2580
         */

        record Hand(int Type, string Cards, int Bid);

        public override object Run(bool part2)
        {
            // 251516544
            checked
            {
                var cardOrder = part2? "AKQT98765432J" : "AKQJT98765432";

                List<Hand> hands = new();
                foreach (var line in ReadLines())
                {
                    var w = line.Split(' ');
                    var cards = w[0];
                    var bid = Numbers(w[1])[0];
                    hands.Add(new(Type(cards), cards, bid));
                }

                hands.Sort(CompareHands);

                return hands.Select((h, i) => h.Bid * (i + 1)).Sum();

                int Type(string cards)
                {
                    int[] counts = new int[cardOrder.Length];
                    foreach (var ch in cards)
                        counts[cardOrder.IndexOf(ch)]++;

                    var bestFixup = 0;
                    if (part2)
                    { // jokers into best slot
                        var ji = cardOrder.IndexOf("J");
                        bestFixup = counts[ji];
                        counts[ji] = 0;

                    }
                    counts = counts.OrderBy(x => -x).ToArray();
                    counts[0] += bestFixup;
                    return -(counts[0] * 10 + counts[1]); // top 2 orders fine
                }
         

                int CompareHands(Hand h1, Hand h2)
                {
                    var t1 = h1.Type;
                    var t2 = h2.Type;
                    if (t1 != t2) 
                        return Int32.Sign(t2 - t1);

                    var s1 = h1.Cards;
                    var s2 = h2.Cards;
                    if (s1 == s2) return 0;
                    for (int i = 0; i < s1.Length; ++i)
                    {
                        var diff = cardOrder.IndexOf(s2[i]) - cardOrder.IndexOf(s1[i]);
                        if (diff != 0) return diff;
                    }
                    return 0;
                }
            }

        }
    }
}