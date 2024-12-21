namespace Lomont.AdventOfCode._2018
{
    internal class Day15 : AdventOfCode
    {
        /*
         * 2018 Day 15 part 1: 216270 in 308888.1 us
2018 Day 15 part 2: 59339 in 772710.4 us
         */
        object Run2()
        {
            var ep = 4;
            int lastScore = 0;
            while (true)
            {
             //   Console.WriteLine($"EP: {ep}");
                var (r, h) = Sim(ep, false);
                lastScore = r * h;
                if (r * h != 0) break;
                ep *= 2;
            }

            while (true)
            {
               // Console.WriteLine($"EP: {ep}");
                var (r, h) = Sim(ep, false);
                if (r * h == 0) break;
                lastScore = r * h;
                ep--;
            }

            return lastScore;
        }

        class Actor
        {
            public Actor(int i, int j, int hp, char t)
            {
                this.i = i;
                this.j = j;
                this.hp = hp;
                this.t = t;
            }

            int _i, _j;

            public int i
            {
                get => _i;
                set
                {
                    _i = value;
                    Trace.Assert((value != 0));
                }

            }

            public int j
            {
                get => _j;
                set
                {
                    _j = value;
                    Trace.Assert((value != 0));
                }
            }

            public int hp;
            public char t;
            public bool moved = false;
        }

        object Run1()
        {
            var (r, h) = Sim();
            return r * h;
        }

        (int rounds, int hpLeft) Sim(int elfPower = 3, bool allowElfDeaths = true, bool show = false)
        {
            (int i, int j)[] dirs = new[]
            {
                (1, 0), (-1, 0), (0, 1), (0, -1)
            };

            int elvesDead = 0, goblinsDead = 0;

            var (w, h, g) = CharGrid();
            List<Actor> actors = new();
            Apply(g, (i, j, c) =>
            {
                if (c == 'G' || c == 'E')
                    actors.Add(new Actor(i, j, 200, c));
                return c;
            });

            bool done = false;
            int round = 0;
            while (!done)
            {
                if (show)
                    DumpR(g, round);
                foreach (var a in actors)
                    a.moved = false;

                var totalStartHp = actors.Sum(t => t.hp);

                bool moved = false;
                for (int j = 0; j < h; ++j)
                for (int i = 0; i < w; ++i)
                {
                    var ch = g[i, j];
                    if (ch == 'E' || ch == 'G')
                    {
                        Actor a = Get(i, j);
                        if (!a.moved)
                        {
                            DoTurn(i, j);
                            a.moved = true;
                            moved |= a.i != i || a.j != j;

                            if (allowElfDeaths == false && elvesDead > 0)
                                return (0, 0);
                        }
                    }
                }

                moved |= totalStartHp != actors.Sum(t => t.hp);
                done = !moved;
                if (moved)
                    round++;
            }

            var ac = actors.Sum(t => t.hp);
            //Console.WriteLine($"{round - 1} rounds, {ac} total hp");
            return (round - 1, ac);

            void DumpR(char[,] g, int round)
            {
                Console.WriteLine($"After {round} round(s)");
                for (int j = 0; j < h; ++j)
                {
                    for (int i = 0; i < w; ++i)
                    {
                        Console.Write(g[i, j]);
                    }

                    Console.Write("  ");
                    for (int i = 0; i < w; ++i)
                    {
                        if ("EG".Contains(g[i, j]))
                        {
                            var a = Get(i, j);
                            Console.Write($"{a.t}({a.hp}) ");
                        }
                    }

                    Console.WriteLine();
                }

                Console.WriteLine();
            }

            char Target(char t)
            {
                if (t == 'E') return 'G';
                if (t == 'G') return 'E';
                throw new NotImplementedException();
            }

            void Attack(Actor a)
            {
                var target = Target(a.t);
                List<Actor> targets = new();
                foreach (var d in dirs)
                {
                    var (i2, j2) = (a.i + d.i, a.j + d.j);
                    if (g[i2, j2] == target)
                        targets.Add(Get(i2, j2));
                }

                if (targets.Count == 0) return;
                var tf = targets.MinBy(t => t.hp);
                tf.hp -= (a.t == 'E') ? elfPower : 3; // hit
                if (tf.hp <= 0)
                {
                    // dead
                    actors.Remove(tf);
                    if (tf.t == 'E')
                        elvesDead++;
                    else
                        goblinsDead++;
                    g[tf.i, tf.j] = '.';
                }
            }


            bool DoTurn(int i, int j)
            {
                bool moved = false;
                var target = g[i, j] switch
                {
                    'E' => 'G',
                    'G' => 'E',
                    _ => throw new NotImplementedException()
                };


                var targets = actors.Where(a => a.t == target).ToList();
                if (targets.Count == 0) return false; // no one left

                Actor actor = Get(i, j);
                if (!HasNbr(i, j, target))
                {
                    // will try to move
                    HashSet<(int, int)> inRangeSquares = new();
                    Apply(g, (i1, j1, c) =>
                    {
                        if (c == '.' && HasNbr(i1, j1, target))
                            inRangeSquares.Add((i1, j1));
                        return c;
                    });

                    if (inRangeSquares.Count == 0) return false;

                    // do a move
                    Move(i, j, inRangeSquares.ToList());
                }

                // attack nbrs
                if (HasNbr(actor.i, actor.j, target))
                    Attack(actor);

                return moved;

                void Move(int i1, int j1, List<(int i, int j)> inRangeSquares)
                {
                    Actor actor = Get(i1, j1);
                    var dm = DepthMap(i1, j1);
                    //   todo here;
                    //var reachable = targets.Where(t => dm[t.i, t.j] < Int32.MaxValue)
                    //    .ToList();
                    //if (reachable.Count == 0) return;

                    var minDist = inRangeSquares.Min(sq => dm[sq.i, sq.j]);
                    var allMin =
                        inRangeSquares
                            .Where(t => dm[t.i, t.j] == minDist)
                            .ToList();
                    allMin.Sort((a, b) => ReadingOrder((a.i, a.j), (b.i, b.j)));

                    var finalTarget = allMin[0];
                    // score backwards
                    var dm2 = DepthMap(finalTarget.i, finalTarget.j);
                    var (i, j) = (i1, j1);
                    // 4 items in reading order
                    var s1 = dm2[i, j - 1];
                    var s2 = dm2[i - 1, j];
                    var s3 = dm2[i + 1, j];
                    var s4 = dm2[i, j + 1];
                    var ms = Math.Min(s1, Math.Min(s2, Math.Min(s3, s4)));
                    if (ms < Int32.MaxValue)
                    {
                        if (ms == s1) DoMove(i, j, 0, -1);
                        else if (ms == s2) DoMove(i, j, -1, 0);
                        else if (ms == s3) DoMove(i, j, 1, 0);
                        else DoMove(i, j, 0, 1);
                    }

                    return;
                }

                int[,] DepthMap(int i1, int j1)
                {
                    int[,] dm = new int[w, h];
                    for (int i = 0; i < w; ++i)
                    for (int j = 0; j < h; ++j)
                        dm[i, j] = Int32.MaxValue;
                    Queue<(int i, int j, int d)> frontier = new();
                    frontier.Enqueue((i1, j1, 0));
                    HashSet<(int, int)> scored = new();
                    while (frontier.Any())
                    {
                        var (x, y, d) = frontier.Dequeue();
                        if (scored.Contains((x, y))) continue;
                        scored.Add((x, y));
                        dm[x, y] = d;
                        foreach (var dir in dirs)
                        {
                            var (x1, y1) = (x + dir.i, y + dir.j);
                            if (g[x1, y1] == '.')
                                frontier.Enqueue((x1, y1, d + 1));
                        }

                    }

                    return dm;
                }
            }

            // one of the nbrs to this point is a target
            bool HasNbr(int i1, int j1, char target) => dirs.Any(d => g[i1 + d.i, j1 + d.j] == target);

            Actor Get(int i, int j, bool allowMoved = true)
            {
                return actors.First(t => t.i == i && t.j == j && (allowMoved || t.moved == false));
            }

            void DoMove(int i, int j, int di, int dj)
            {
                var a = Get(i, j);
                g[a.i, a.j] = '.';
                a.i += di;
                a.j += dj;
                Trace.Assert(a.i > 0 && a.j >= 0);
                g[a.i, a.j] = a.t;
            }


            int ReadingOrder((int x, int y) a, (int x, int y) b)
            {
                if (a.y != b.y) return a.y.CompareTo(b.y);
                return a.x.CompareTo(b.x);
            }
        }

        public override object Run(bool part2)
        {
            return part2 ? Run2() : Run1();
        }
    }
}