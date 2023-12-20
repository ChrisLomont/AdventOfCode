
namespace Lomont.AdventOfCode._2023
{
    internal class Day17 : AdventOfCode
    {
        //1008, 1210

        record State(vec2 pos, vec2 dir, int runlength);

        public override object Run(bool part2)
        {
            var (w, h, g) = DigitGrid();

            bool Legal(vec2 p) => 0 <= p.x && 0 <= p.y && p.x < w && p.y < h;
            var maxRun = part2 ? 10 : 3;
            var minRun = part2 ? 4 : 1;
            var dirs = new List<vec2> { new(0, 1), new(1, 0), new(0, -1), new(-1, 0), };
            var end = new vec2(w - 1, h - 1);

            var seen = new HashSet<State>();
            var frontier = new PriorityQueue<(int cost, State state), int>();
            frontier.Enqueue((0,new State(new (),new (),0)),0);

            Renderer? renderer = null;//new Renderer(w, h,8);
            renderer?.AddOne(frontier.Peek().state);

            while (frontier.Count > 0)
            {
                var element = frontier.Dequeue();
                var (cost, state) = element;
                var (pos, dir, n) = state;

                renderer?.StartFrame(state);

                var c2 = !part2 || n >= minRun;
                if (pos == end && c2)
                {
                    renderer?.Finish();
                    return cost;
                }

                if (seen.Contains(state))
                    continue;
                seen.Add(state);

                if (n < maxRun && dir != vec2.Zero)
                    AddIf(new(pos + dir, dir, n + 1));

                var minOk = n >= minRun || dir == vec2.Zero;

                if (!part2 || (part2 && minOk))
                    foreach (var dir2 in dirs)
                        if (dir2 != dir && dir2 != -dir)
                            AddIf(new(pos + dir2, dir2, 1));

                renderer?.EndFrame();

                void AddIf(State s)
                {
                    if (Legal(s.pos))
                    {
                        var newCost = cost + g[s.pos.x, s.pos.y];
                        renderer?.AddOne(s);
                        frontier.Enqueue((newCost, s), newCost);
                    }
                }
            }

            return 0;
        }

        class Renderer
        {
            GIFMaker gif;
            int w, h, px;
            public Renderer(int w, int h, int px)
            {
                this.w = w;
                this.h = h;
                this.px = px;
                gif = new(w*px, h*px);
                lastWritten = new int[w, h];
            }

            int frame = 0;
            int[,] lastWritten;

            HashSet<State> states = new();
            HashSet<State> done = new();

            State? inspectedThisFrame = null;
            List<State> AddedThisFrame = new();

            public void StartFrame(State state)
            {
                states.Remove(state);
                done.Add(state);
                AddedThisFrame.Clear();
                inspectedThisFrame = state;
            }

            void Color(State s, int t)
            {
                // t 0 = look at
                // t 1 = added
                var (x, y) = s.pos;
                if (t == 0)
                    lastWritten[x, y] = 1; // fixed val?
                else
                    lastWritten[x, y] = frame;
            }

            public void EndFrame()
            {
                if (inspectedThisFrame != null) Color(inspectedThisFrame, 0);
                foreach (var a in AddedThisFrame)
                    Color(a,1);


                var framesPerImage = 2000;
                if ((frame% framesPerImage) ==0)
                {
                    var image = new byte[w*px, h*px];
                    for (int j = 0; j < h; ++j)
                    for (int i = 0; i < w; ++i)
                    {
                        var lw = lastWritten[i, j];
                        lw /= framesPerImage;
                        var c = (byte)lw;
                        if (c == 0) c = 1;

                        for (int dj = 0; dj < px; ++dj)
                        for (int di = 0; di < px; ++di)
                        {
                            image[i*px+di, j*px+dj] = c;
                        }

                    }
                    gif.AddFrame(image, 20); // 100ths of sec
                }
                ++frame;

            }

            public void AddOne(State s)
            {
                states.Add(s);
                AddedThisFrame.Add(s);
            }

            public void Finish()
            {
                gif.Save("Day17.gif");
            }
        }

    }
}