

namespace Lomont.AdventOfCode._2018
{
    internal class Day14 : AdventOfCode
    {
        /**
         * 2018 Day 14 part 1: 8610321414 in 14324.4 us
2018 Day 14 part 2: 20258123 in 830120.1 us
         */
        object Run2()
        {
            List<int> scores = new() { 3, 7 }; // start scores
            var (elf1, elf2) = (0, 1); // positions

            int pass = 0;
            int numRec = 51589; // 9
            numRec = 01245; // 5 - leading 0!
            numRec = 92510; // 18
            numRec = 59414; // 2018
            numRec = 607331;
            var digits = numRec.ToString().ToCharArray().Select(c => c - '0').ToList();

            while (true)
            {
            //    if (pass > 20) break;
            //    Draw();
                var s1 = scores[elf1];
                var s2 = scores[elf2];
                var ind = Add(s1 + s2);
                if (ind >= 0) return ind;
                elf1 = (elf1 + 1 + s1) % scores.Count;
                elf2 = (elf2 + 1 + s2) % scores.Count;

                ++pass;
            }

            // index, or -1
            int Add(int score)
            {
                int ans = -1;
                if (score < 10)
                {
                    scores.Add(score);
                    Check();
                }
                else
                {
                    scores.Add(score / 10);
                    Check();
                    scores.Add(score% 10);
                    Check();
                }

                void Check()
                {
                    var n = digits.Count;
                    if (ans != -1 || scores.Count < n) return;
                    for (int i = 0; i < n; ++i)
                    {
                        if (scores[scores.Count-n+i] != digits[i]) return;
                    }

                    ans = scores.Count - digits.Count;
                }

                return ans;

            }

            return -1;
            bool Hits(int s)
            {
                Trace.Assert(scores[s] == digits[0]);
                if (scores.Count < s + digits.Count) return false;
                for (int i = 0; i < digits.Count; ++i)
                {
                    
                    if (scores[s+i] != digits[i])
                        return false;
                }
                return true;

            }

            long ans = 0;
            for (int i = numRec; i < numRec + 10; ++i)
            {
                ans = 10 * ans + scores[i];
            }

            return ans;
            void Draw()
            {
                Console.Write($"{pass:D4}: ");
                for (int i = 0; i < scores.Count; i++)
                {

                    char c1 = ' ', c2 = ' ';
                    if (i == elf1) (c1, c2) = ('(', ')');
                    if (i == elf2) (c1, c2) = ('[', ']');
                    Console.Write($"{c1}{scores[i]}{c2}");
                }

                Console.WriteLine();
            }

            return -1;
        }

        object Run1()
        {
            List<int> scores = new() { 3, 7 }; // start scores
            var (elf1,elf2)=(0,1); // positions

            int pass = 0;
            int numRec = 607331; 
            while (scores.Count<=numRec+11)
            {
                //Draw();
                var s1 = scores[elf1];
                var s2 = scores[elf2];
                var ns = s1 + s2;
                if (ns < 10)
                    scores.Add(ns);
                else
                {
                    scores.Add(ns / 10);
                    scores.Add(ns % 10);
                }

                elf1 = (elf1+1 + s1) % scores.Count;
                elf2 = (elf2+1 + s2) % scores.Count;
                ++pass;
            }

            long ans = 0;
            for (int i = numRec; i < numRec + 10; ++i)
            {
                ans = 10 * ans + scores[i];
            }

            return ans;
            void Draw()
            {
                for (int i = 0; i < scores.Count; i++)
                {

                    char c1 = ' ', c2 = ' ';
                    if (i == elf1) (c1, c2) = ('(', ')');
                    if (i == elf2) (c1, c2) = ('[', ']');
                    Console.Write($"{c1}{scores[i]}{c2}");
                }

                Console.WriteLine();
            }

            return -1;

        }

        public override object Run(bool part2)
        {
            return part2 ? Run2() : Run1();
        }
    }
}