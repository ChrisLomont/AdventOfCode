

namespace Lomont.AdventOfCode._2018
{
    internal class Day09 : AdventOfCode
    {
        /*
         * 2018 Day 9 part 1: 424639 in 73845.7 us
2018 Day 9 part 2: 3516007333 in 1166010 us
         */

        object Run2()
        {
            var nums = Numbers64(ReadText());
            var numPlayers = nums[0];
            var lastScore = nums[1];

            lastScore = lastScore * 100;

            var scores = new long[numPlayers];
            var circle = new LinkedList<long>();
            var current = circle.AddFirst(0);

            for (int i = 1; i < lastScore; i++)
            {
                if (i % 23 == 0)
                {
                    scores[i % numPlayers] += i;
                    for (int j = 0; j < 7; j++)
                    {
                        current = current.Previous ?? circle.Last;
                    }
                    scores[i % numPlayers] += current.Value;
                    var remove = current;
                    current = remove.Next;
                    circle.Remove(remove);
                }
                else
                {
                    current = circle.AddAfter(current.Next ?? circle.First, i);
                }
            }

            return scores.Max();
        }

        object Run1()
        {
            var nums = Numbers64(ReadText());
            var numPlayers = nums[0];
            var lastScore = nums[1];

            // tests
            //(numPlayers,lastScore)=(9,25); // -> 32
            //(numPlayers, lastScore) = (10, 1618); // -> 8317


            List<int> board = new();
            long[] scores= new long[numPlayers];

            // play first
            board.Add(0);
            int pl = 1; // person to play
            int mb = 1; // next marble to place
            int cur = 0; // current marble index


            while (mb<=lastScore)
            {
               // Dump();

                if (mb != 0 && (mb%23)==0)
                {
                    scores[pl] += mb;
                    //Console.WriteLine("Scores");
                    cur = (cur - 8 + board.Count) % board.Count;
                    scores[pl] += board[cur];
                    //Console.WriteLine($"Scores {mb} + {board[cur]}");
                    board.RemoveAt(cur);
                    cur = (cur + 1) % board.Count;
                }
                else
                {
                    board.Insert(cur+1, mb);
                    cur = (cur + 2) % board.Count;
                }
                mb++;
                pl = (pl + 1) % (int)(numPlayers);
            }
         //   Dump();

            void Dump()
            {
                Console.Write($"[{pl + 1}] ");
                foreach (var b in board)
                    Console.Write($"{b} ");
                Console.WriteLine();
            }

            //Console.WriteLine("Scores");
            //foreach (var sc in scores)
            //    Console.Write($"{sc} ");
            //Console.WriteLine();
            return scores.Max();
        }

        public override object Run(bool part2)
        {
            return part2 ? Run2() : Run1();
        }
    }
}