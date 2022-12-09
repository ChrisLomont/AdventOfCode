namespace Lomont.AdventOfCode._2021
{ 
    // 27464148626406
    internal class Day21 : AdventOfCode
    {

        record State1(int pos, int score);

        object Run2(int pos1, int pos2)
        {
            // recursive solution, memoize subproblems
            Dictionary< (State1,State1), (long, long)> memo = new ();

            (long moveWins, long waitWins) CountWins(State1 toMove, State1 toWait)
            {
                if (toWait.score >= 21) return (0, 1); // last mover, check score
                var key = (toMove, toWait);
                if (!memo.ContainsKey(key))
                { // 27464148626406
                    var (moveWins, waitWins) = (0L, 0L);
                    for (var a = 1; a <= 3; ++a)
                    for (var b = 1; b <= 3; ++b)
                    for (var c = 1; c <= 3; ++c)
                    {
                        var steps = a + b + c;
                        var pos = ((toMove.pos - 1 + steps) % 10) + 1;
                        var score = toMove.score + pos;
                        // swap player
                        var (mw, ow) = CountWins(toWait, new(pos, score));
                        moveWins += ow; // swapped players => swap tallies
                        waitWins += mw;
                    }
                    memo[key] = (moveWins, waitWins);
                }
                return memo[key];
            }

            var (wins1, wins2) = CountWins(new (pos1,0), new (pos2,0));
            //Console.WriteLine("Keys " + memo.Count);
            return wins1;


        }


        private int rolls = 0;
        private int roll = 1;
        int Die()
        {
            var v = roll;
            roll++;
            if (roll > 100) roll = 1;
            ++rolls;
            return v;
        }

        (int p1, int p2) Play(int pos1, int pos2)
        {
            int score1 = 0, score2 = 0;
            while (true)
            {
                var m1 = Die() + Die() + Die();
                while (m1-- > 0)
                {
                    pos1 += 1;
                    if (pos1 > 10) pos1 = 1;
                }
                score1 += pos1;
                if (score1 >= 1000) break;

                var m2 = Die() + Die() + Die();
                while (m2-- > 0)
                {
                    pos2 += 1;
                    if (pos2 > 10) pos2 = 1;
                }
                score2 += pos2;
                if (score2 >= 1000) break;
            }

            return (score1, score2);

        }

        public override object Run(bool part2)
        {
            var lines = ReadLines();

            int pos1 = 2, pos2 = 1; // my state
            if (part2)
                return Run2(pos1, pos2);

            rolls = 0;
            roll = 1;
            var (p1, p2) = Play(pos1,pos2);

            return rolls*Math.Min(p1,p2);
        }
    }
}