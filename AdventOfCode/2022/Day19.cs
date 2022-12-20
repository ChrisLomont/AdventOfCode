
namespace Lomont.AdventOfCode._2022
{

    // top 100 both: 24:29 - 57:45
    // top 100 first part 19:29 - 48:27

    //me 19   01:02:55     309      0   01:18:41     332      0
    
    //2022 Day 19 part 1: 1023 in 3644776.6 us
    //2022 Day 19 part 2: 13520 in 19995536.4 us

    internal class Day19 : AdventOfCode
    {
       


        public override object Run(bool part2)
        {
            long score = part2 ? 1 : 0;
            var lines = ReadLines();
            var botsAdded = new[]
            {
                new vec4(0, 0, 0, 0), // no bots added
                new vec4(1, 0, 0, 0), // one of each type, costs below
                new vec4(0, 1, 0, 0),
                new vec4(0, 0, 1, 0),
                new vec4(0, 0, 0, 1),
            };
            for (var index = 0; index < lines.Count; ++index)
            {
                var line = lines[index];

                var n = Numbers(line);

                // bot construction costs
                // w,x,y,z = ore, clay, obsidian, geode

                var costs = new[]
                {
                    new vec4(0, 0, 0, 0), // no cost, buy nothing
                    new vec4(n[1]), // ore robot costs this many ore
                    new vec4(n[2]), // one clay robot costs uses this ore
                    new vec4(n[3], n[4]), // obsidian robot uses this ore and clay
                    new vec4(n[5], 0, n[6]) // geode robot uses this much ore and obsidian
                };

                // max useful to grab per turn
                var maxResourceNeeded = vec4.Max(costs);

                // need to set allowed geodes to large number to allow geode gathering
                // not set to max since will be mult by time later
                maxResourceNeeded.z = Int32.MaxValue / 1000;

                if (part2)
                    score *= (index > 2) ? 1 : FindBest(32, costs, botsAdded, maxResourceNeeded);
                else
                    score += n[0] * FindBest(24, costs, botsAdded, maxResourceNeeded);
            }

            return score;
        }


        static long FindBest(int timeLeft, vec4[] costs, vec4[] extra, vec4 maxResourceNeeded)
        {
            var zero = new vec4(0, 0, 0, 0);
            long mostGeodes = 0;
            var open = new Queue<(vec4 resources, vec4 bots, int time)>();
            open.Enqueue((new vec4(0, 0, 0, 0), new vec4(1, 0, 0, 0), timeLeft));
            var closed = new HashSet<(vec4 resources, vec4 bots, int time)>();
            while (open.Any())
            {
                // ideas:
                // skip till next robot build, ignore in between steps
                // if have enough resources per turn to build any robot, only build geode robots

                var state = open.Dequeue();
                var (resources, bots, t) = state;

                mostGeodes = Math.Max(mostGeodes, resources.z);

                if (resources.z + 0 < mostGeodes) // tweaked until worked, 0 works for me, ho about anyone?
                {
                    closed.Add(state);
                    continue;
                }


                if (t == 0)
                {
                    closed.Add(state);
                    continue;
                }


                // magic is these two lines - maybe A* or something else better?
                // clamp bots - don't need more than this for full production
                bots = vec4.Clamp(bots, zero, maxResourceNeeded);
                // clamp resources - don't need more of them than this to max production
                resources = vec4.Clamp(resources, zero, t * maxResourceNeeded - (t - 1) * bots);

                state = (resources, bots, t);
                if (closed.Contains(state))
                    continue;
                closed.Add(state);

                //if (costs[4] <= resources || costs[4] <= bots)
                //{ // always build geode robot if can, and once one can be built per turn, build nothing else
                //    open.Enqueue((resources + bots - costs[4], bots + extra[4], t - 1));
                //}
                //else
                for (var k = 0; k < costs.Length; ++k)
                    if (costs[k] <= resources)
                        open.Enqueue((resources + bots - costs[k], bots + extra[k], t - 1));
            }

            Console.WriteLine($"States {closed.Count}");
            return mostGeodes;
        }


#if false
// old stuff

                var memo = new Dictionary<string, long>();

                // num of each: ore
                // returns num geodes obtained
                long Sim(
                    //int ore, int clay, int obs,
                    vec4 resources,
                    //int oreBots, int clayBots, int obsBots,
                    vec4 bots,
                    int timeLeft)
                {
                    var geodes = resources.geo;
                    var key = $"{resources},{bots},{timeLeft}";

                    if (!memo.ContainsKey(key))
                    {
                        var best = 0L;
                        if (timeLeft > 0)
                        {
                            for (var k = 0; k < costs.Length; ++k)
                            {
                                var botCost = costs[k];
                                var botGain = extra[k];
                                if (LT(botCost, resources))
                                {
                                    //Console.WriteLine($"time {timeLeft} builds {botGain} resources {resources}");
                                    var newResources = Add(Sub(resources,botCost),bots);
                                    var newBots = Add(bots, botGain);
                                    // build ore bot
                                    best = Math.Max(best, Sim(newResources, newBots, timeLeft - 1));
                                }
                            }
                        }

                        memo.Add(key, best + geodes);
                        if ((memo.Count%1000000)==0)
                            Console.WriteLine($"memo {memo.Count}");
                    }

                    return memo[key];
                }

                // num of each: ore
                // returns num geodes obtained
                long Sim2(
                    //int ore, int clay, int obs,
                    vec4 resources,
                    //int oreBots, int clayBots, int obsBots,
                    vec4 bots,
                    int timeLeft)
                {
                    var geodes = resources.geo;
                    var key = $"{resources},{bots},{timeLeft}";

                    if (!memo.ContainsKey(key))
                    {
                        var best = 0L;
                        if (timeLeft > 0)
                        {
                            if (LT(oreCost,resources))
                            {
                                // build ore bot
                                best = Math.Max(best,
                                Sim(
                                    Add(Sub(resources,oreCost),bots),
                                    Add(bots, new vec4(1, 0, 0, 0)),
                                    timeLeft-1
                                ));
                            }
                            if (LT(clayCost, resources))
                            {
                                // build ore bot
                                best = Math.Max(best,
                                    Sim(
                                        Add(Sub(resources, clayCost), bots),
                                        Add(bots, new vec4(0, 1, 0, 0)),
                                        timeLeft - 1
                                    ));
                            }
                            if (LT(obCost, resources))
                            {
                                // build ore bot
                                best = Math.Max(best,
                                    Sim(
                                        Add(Sub(resources, obCost), bots),
                                        Add(bots, new vec4(0, 0, 1, 0)),
                                        timeLeft - 1
                                    ));
                            }
                            if (LT(geoCost, resources))
                            {
                                // build ore bot
                                best = Math.Max(best,
                                    Sim(
                                        Add(Sub(resources, geoCost), bots),
                                        Add(bots, new vec4(0, 0, 0, 1)),
                                        timeLeft - 1
                                    ));
                            }

                            // do nothing
                            // build ore bot
                            best = Math.Max(best,
                                Sim(
                                    Add(resources, bots),
                                    bots,
                                    timeLeft - 1
                                ));
                        }

                        memo.Add(key, best+geodes);
                    }

                    return memo[key];
                }

            }
#endif
    }
}