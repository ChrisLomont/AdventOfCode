namespace Lomont.AdventOfCode._2020
{
    internal class Day22 : AdventOfCode
    {
        public static uint FNVHash(IEnumerable<int> vals, uint hash = 0)
        {
            const uint fnv_prime = 0x811C9DC5;
            uint i = 0;
            foreach (var v in vals)
            {
                var t = v;
                hash *= fnv_prime;
                hash ^= ((byte)t);
                t >>= 8;
                while (t > 0)
                {
                    hash *= fnv_prime;
                    hash ^= ((byte)t);
                }
            }

            return hash;
        }

        object Run2()
        {
            List<int> player1 = new(), player2 = new();
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

            // player 1 wins
            bool PlayOne()
            {
                return true;
                HashSet<uint> seen = new();

                int round = 0;
                while (player1.Any() && player2.Any())
                {
                    ++round;
                    Console.Write("Round: " + round);

                    // new rule #1
                    // check seen
                    if (Seen()) return true; // player 1 wins

                    var p1 = player1[0];
                    var p2 = player2[0];
                    player1.RemoveAt(0);
                    player2.RemoveAt(0);

                    // new rule #2
                    if (p1 <= player1.Count && p2 <= player2.Count)
                    {
                        // recursive
                        //todo
                    }


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

                bool Seen()
                {
                    var h = FNVHash(player1);
                    h = FNVHash(player2, h);
                    if (seen.Contains(h)) return true;
                    seen.Add(h);
                    return false;
                }
            }

            //Console.WriteLine("Round " + round);

            var winner = player1.Any() ? player1 : player2;

            var score = 0L;
            for (var i = 0; i < winner.Count; ++i)
            {
                Console.WriteLine($"Score: {winner[i]} {winner.Count - i}");
                score += winner[i] * (winner.Count - i);
            }

            return score;
        }
        public override object Run(bool part2)
        {
            if (part2) return Run2();

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