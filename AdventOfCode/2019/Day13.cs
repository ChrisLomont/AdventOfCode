namespace Lomont.AdventOfCode._2019
{
    internal class Day13 : AdventOfCode
    {
        public override object Run(bool part2)
        {
            Dictionary<(long x, long y), long> board = new();

            long gameScore = 0;
            var prog = Numbers64(ReadLines()[0]);
            vec3 ball = new();
            vec3 paddle = new();


            Queue<long> output = new();
            

            if (part2) 
                prog[0] = 2; // set up to play

            var comp = new Day02.IntCode(prog, GetInput, output.Enqueue);



            while (!comp.Step())
            {
                if (output.Count == 3)
                {
                    var m  = output.ToList().Chunk(3).ToList()[0];
                    output.Clear();
                    var (x, y, v) = (m[0], m[1], m[2]);
                    if ((x, y) == (-1, 0))
                    {
                        gameScore = v;
                       // Console.WriteLine(gameScore);
                    }
                    else
                    {
                        if (!board.ContainsKey((x, y)))
                            board.Add((x, y), 0);
                        board[(x, y)] = v;
                        if (v == 4) ball = new vec3((int)x,(int)y);
                        if (v == 3) paddle = new vec3((int)x, (int)y);
                    }


                   // if (gameScore > 100 && !board.Any(p => p.Value == 2))
                   //     return gameScore;

                }
            }

            if (part2) return gameScore;
            return board.Sum(p=>p.Value==2?1:0);

            long GetInput()
            {
                //Show();
                var move = Math.Sign(-(paddle-ball).x);
                return move;
            }

            void Show()
            {
                Console.SetCursorPosition(0,0);
                Console.WriteLine($" Game score: {gameScore}");
                var minx = board.Min(p => p.Key.x);
                var maxx = board.Max(p => p.Key.x);
                var miny = board.Min(p => p.Key.y);
                var maxy = board.Max(p => p.Key.y);
                var (dx, dy) = (maxx - minx, maxy - miny);
                var g = new char[dx+1, dy+1];
                Apply(g, (i, j, v) =>
                {
                    if (board.ContainsKey((i, j)))
                    {
                        var tiles = ".#b_O";
                        return tiles[(int)board[(i, j)]];
                    }
                    return v;
                });
                Dump(g,true);
            }

        }
    }
}