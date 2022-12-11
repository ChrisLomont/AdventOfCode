
namespace Lomont.AdventOfCode._2022
{
    internal class Day10 : AdventOfCode
    {   // 15220
        // RFZEKBFA

        // leaders
        // rank1:  both 5:55 rank 100 12:17
        // rank1   single 2:09  rank 100 5:17

        //  10   00:08:26     609      0   00:14:05     201      0

        // 12:25, 1550 people done
        // 9am 29,476 done

        public override object Run(bool part2)
        {
            long score = 0;
            var screen = new char[40, 6];
            var (x,cycles) = (1,1); // state

            foreach (var line in ReadLines())
            {
                Tick(); // always, addx or noop

                if (line.Contains("addx"))
                {
                    Tick();
                    x += Numbers(line)[0];
                }
            }

            if (part2)
                Dump(screen,noComma :true);
            return score;

            void Tick()
            {
                if (cycles % 40 == 20)
                    score += x * cycles;
                var x1 = (cycles - 1) % 40;
                var y1 = (cycles - 1) / 40;
                screen[x1, y1 ] = Math.Abs(x - x1) < 2 ? '#' : '.';

                cycles++;
            }

        }
    }
}