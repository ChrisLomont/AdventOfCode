
namespace Lomont.AdventOfCode._2022
{
    internal class Day02 :AdventOfCode
    {
        public override object Run(bool part2)
        {
            var score = 0;
            foreach (var line in ReadLines())
            {
                var p1 = Check(line[0] - 'A');
                var p2 = Check(line[2] - 'X');
                score += part2 ? Score2(p1, p2) : Score(p1, p2);
            }
            return score;

            int Check(int p)
            {
                Trace.Assert(0 <= p && p <= 2);
                return p;
            }


            // A,B,C = Rock, Paper, Scissors
            // X,Y,Z = ...
            int Score(int p1, int p2)
            {
                Check(p1);
                Check(p2);

                if (p1 == (p2 + 1) % 3) return (p2 + 1) + 0; // loss
                if (p1 == p2) return (p2 + 1) + 3; // draw
                if ((p1 + 1) % 3 == p2) return (p2 + 1) + 6; // win
                throw new Exception();
            }

            // A,B,C = Rock, Paper, Scissors
            // X,Y,Z = me lose, me draw, me win
            int Score2(int p1, int p2)
            {
                // find my move m and score it
                var m = p2 switch
                {
                    0 => (p1 + 2) % 3, // I lose
                    1 => p1, // i draw
                    2 => (p1 + 1) % 3, // i win
                    _ => throw new Exception()
                };
                return Score(p1, m);
            }
        }

    }
}
