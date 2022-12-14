namespace Lomont.AdventOfCode._2020
{
    internal class Day22 : AdventOfCode
    {
        public override object Run(bool part2)
        {

            List<int> player1= new(), player2 = new();
            List<int> player = player1;

            foreach (var line in ReadLines())
            {
                if (line.Contains(':'))
                {
                    if (line.Contains('2'))
                        player = player2;
                }
                else if (!String.IsNullOrEmpty(line))
                {
                    player.Add(int.Parse(line));
                }
            }

            int round = 0;
            while (player1.Any() && player2.Any())
            {
                ++round;
                Console.Write("Round: "+ round);

                var p1 = player1[0];
                var p2 = player2[0];
                player1.RemoveAt(0);
                player2.RemoveAt(0);
                if (p1 > p2)
                {
                    player1.Add(p1);
                    player1.Add(p2);
                    Console.WriteLine($"player 1 wins {p1} {p2}");
                }
                else if (p1 < p2)
                {
                    player2.Add(p2);
                    player2.Add(p1);
                    Console.WriteLine($"player 2 wins {p1} {p2}");
                }
            }

            Console.WriteLine("Round "+ round);

            var winner = player1.Any() ? player1 : player2;

            var score = 0L;
            for (var i = 0; i < winner.Count; ++i)
            {
                Console.WriteLine($"Score: {winner[i]} {winner.Count-i}");
                score += winner[i] * (winner.Count - i);
            }

            return score;


        }
    }
}