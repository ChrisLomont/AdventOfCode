using System.Security.Cryptography;

namespace Lomont.AdventOfCode._2020
{
    // 2020 Day 22 part 1: 33694 in 6152.6 us
    // 2020 Day 22 part 2: 31835 in 6913033.2 us

    internal class Day22 : AdventOfCode
    {

        static string Hash(IEnumerable<int> d1, IEnumerable<int> d2)
        {
            using (SHA256 sh = SHA256.Create())
            {
                var b1 = d1.Select(t => (byte)t).ToList();
                var b2 = d2.Select(t => (byte)t).ToList();
                b1.AddRange(b2);
                var hh = sh.ComputeHash(b1.ToArray());
                var hash = String.Empty;
                // Convert the byte array to string format
                foreach (byte b in hh)
                {
                    hash += $"{b:X2}";
                }

                return hash;
            }
        }
#if false
        // FNV-1a (64-bit) non-cryptographic hash function.
        // Adapted from: http://github.com/jakedouglas/fnv-java

        const ulong Fnv64Offset = 14695981039346656037UL;
        const ulong Fnv64Prime = 0x100000001b3;
        public static ulong FNVHash(IEnumerable<int> vals, ulong hash = Fnv64Offset)
        {



            var len = 0;
            foreach (var v in vals)
            {
                var t = v;
                t >>= 8;
                do
                {
                    hash ^= ((byte)t);
                    hash *= Fnv64Prime;
                    t >>= 8;
                    ++len;
                } while (t > 0);

            }

            // lomont added:
            hash ^= ((byte)len);
            hash *= Fnv64Prime;


            return hash;
        }

        public static uint FNVHash32(IEnumerable<int> vals, uint hash = 0)
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
#endif
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

            bool show = false;

            Dictionary<string, (bool, List<int>, List<int>)> memo = new();
            int globalGame = 1;

            var (p1W, p1, p2) = PlayGame(player1, player2, ref globalGame, show, memo);

            if (show)
            {
                Console.WriteLine();
                Console.WriteLine("Post game results");
                Console.Write("Player 1's deck: ");
                Dump(p1, singleLine: true);
                Console.Write("Player 2's deck: ");
                Dump(p2, singleLine: true);
            }


            // player 1 wins
            static (bool player1Wins, List<int> player1, List<int> player2) 
                PlayGame(IList<int> player1In, IList<int> player2In, 
                    ref int globalGame, bool show,
                    Dictionary<string, (bool, List<int>, List<int>)> memo
                    )
            {

                var game = globalGame++;
                if (show)
                    Console.WriteLine($"=== Game {game} ===");
                var key = HashGame(player1In, player2In);
                bool useMemo = false;
                bool p1Wins = true;
                var player1 = new List<int>();
                var player2 = new List<int>();
                player1.AddRange(player1In);
                player2.AddRange(player2In);

                if (!useMemo || !memo.ContainsKey(key))
                {



                    HashSet<string> seen = new();
                    List<(List<int> p1, List<int> p2)> seen2 = new();

                    int round = 0;
                    while (player1.Any() && player2.Any())
                    {
                        ++round;
                        if (show)
                        {
                            Console.WriteLine();
                            Console.WriteLine($"-- Round {round} (Game {game}) -- ");
                            Console.Write("Player 1's deck: ");
                            Dump(player1, singleLine: true);
                            Console.Write("Player 2's deck: ");
                            Dump(player2, singleLine: true);
                        }

                        // new rule #1
                        // check seen
                        if (Seen())
                        {
                            p1Wins = true;
                            break;
                        }

                        var p1 = player1[0];
                        var p2 = player2[0];
                        player1.RemoveAt(0);
                        player2.RemoveAt(0);
                        if (show)
                        {
                            Console.WriteLine("Player 1 plays :" + p1);
                            Console.WriteLine("Player 2 plays :" + p2);
                        }

                        // new rule #2
                        if (p1 <= player1.Count && p2 <= player2.Count)
                        {
                            // recursive game
                            if (show)
                            {
                                Console.WriteLine("Playing a sub-game to determine the winner....");
                                Console.WriteLine();
                            }

                            (p1Wins, _, _) = PlayGame(player1.Take(p1).ToList(), player2.Take(p2).ToList(), ref globalGame, show, memo);
                            if (show)
                            {
                                Console.WriteLine();
                                Console.WriteLine($"....anyway, back to game {game}");
                            }
                        }
                        else
                        { // normal game
                            if (p1 > p2)
                                p1Wins = true;
                            else if (p1 < p2)
                            {
                                p1Wins = false;
                            }
                        }
                        if (p1Wins)
                        {
                            player1.Add(p1);
                            player1.Add(p2);
                            if (show)
                                Console.WriteLine($"Player 1 wins round {round} of game {game}!");
                        }
                        else
                        {
                            player2.Add(p2);
                            player2.Add(p1);
                            if (show)
                                Console.WriteLine($"Player 2 wins round {round} of game {game}!");
                        }
                    }

                    if (useMemo)
                        memo.Add(key,(p1Wins, player1, player2));

                    bool Seen()
                    {
#if false
// TODO - we are getting wrong hash collisions! Weird for SHA256
                        var h = HashGame(player1, player2);
                        if (seen.Contains(h)) return true;
                        seen.Add(h);
                        return false;
#else
                        foreach (var (p1,p2) in seen2)
                        {
                            if (Same(p1, player1) && Same(p2, player2))
                                return true;
                        }

                        List<int> c1 = new();
                        List<int> c2 = new();
                        c1.AddRange(player1);
                        c2.AddRange(player2);
                        seen2.Add((c1,c2));
                        return false;
                        
                        static bool Same(List<int> a, List<int> b)
                        {
                            if (a.Count != b.Count) return false;
                            for (var i =0; i < a.Count; ++i)
                                if (a[i] != b[i]) return false;
                            return true;


                        }
#endif
                    }
                }

                if (useMemo)
                    return memo[key];
                return (p1Wins, player1, player2);

                static string HashGame(IEnumerable<int> player1, IEnumerable<int> player2)
                {
                    return Hash(player1, player2);
                    //var h = FNVHash(player1);
                    //return FNVHash(player2, h);
                }
            }

            //Console.WriteLine("Round " + round);

            var winner = p1W ? p1 : p2;
            checked
            {
                var score = 0L;
                for (var i = 0; i < winner.Count; ++i)
                {
                    //Console.WriteLine($"Score: {winner[i]} {winner.Count - i}");
                    score += winner[i] * (winner.Count - i);
                }

                return score; // 31088 too low, 32050 too high
            }
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

                //Console.Write("Round: "+ round);

                var p1 = player1[0];
                var p2 = player2[0];
                player1.RemoveAt(0);
                player2.RemoveAt(0);
                if (p1 > p2)
                {
                    player1.Add(p1);
                    player1.Add(p2);
                    //Console.WriteLine($"player 1 wins {p1} {p2}");
                }
                else if (p1 < p2)
                {
                    player2.Add(p2);
                    player2.Add(p1);
                    //Console.WriteLine($"player 2 wins {p1} {p2}");
                }
            }

            //Console.WriteLine("Round "+ round);

            var winner = player1.Any() ? player1 : player2;

            var score = 0L;
            for (var i = 0; i < winner.Count; ++i)
            {
                //Console.WriteLine($"Score: {winner[i]} {winner.Count-i}");
                score += winner[i] * (winner.Count - i);
            }

            return score;


        }
    }
}