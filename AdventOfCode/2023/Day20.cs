using System.Runtime.Serialization.Json;

namespace Lomont.AdventOfCode._2023
{
    internal class Day20 : AdventOfCode
    {
        // 253302889093151
        // 666795063

        public enum GT {
            FlipFlop,
            Conjunction,
            Broadcaster,
            Unknown
        }
        


        public override object Run(bool part2)
        {
            List<bool> flops = new();
            List<Dictionary<string, bool>> conjs = new();

            Dictionary<string, (GT type, int stateIndex, List<string> dests)> dict = new();

            foreach (var line in ReadLines())
            {
                var w = line.Split(' ');
                var name = w[0];

                var dests = line
                    .Split("->")[1]
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(n => n.Trim()).ToList();

                //    Console.WriteLine($"{line} -> [{name}] [{type}] [{dests.Count}]");
                var (gtype,stateIndex) = name[0] switch
                {
                    '%' => (GT.FlipFlop,flops.Count),
                    '&' => (GT.Conjunction,conjs.Count),
                    _=> (name == "broadcaster" ? GT.Broadcaster : GT.Unknown,-1)
                };
                if (gtype == GT.FlipFlop)
                    flops.Add(false);
                else if (gtype == GT.Conjunction)
                    conjs.Add(new ());

                dict.Add("%&".Contains(name[0]) ? name[1..]:name,
                    (gtype, stateIndex, dests)
                    );
            }

            // connect gates - ensures Conjunctions listen to all
            foreach (var name in dict.Keys)
            foreach (var dst in dict[name].dests.Where(dict.ContainsKey))
            {
                var gate = dict[dst];
                if (gate.type == GT.Conjunction && !conjs[gate.stateIndex].ContainsKey(name))
                    conjs[gate.stateIndex].Add(name, false);
            }

            // part 1 - count pulses
            long[] pulses = new long[2];

            // part 2 - find clicks to rx, which needs cycles of things triggering entry to rx
            // item pointing to rx
            var srcRxLst = dict.Keys.Where(name => dict[name].dests.Contains("rx")).ToList();
            Trace.Assert(srcRxLst.Count == 1);
            var srcRx = srcRxLst[0];
            // get those pointing to srcRx, for each, track button clicks when they toggle
            var watch = dict.Where(e => e.Value.dests.Contains(srcRx)).Select(d => d.Key).ToList();
            Dictionary<string, List<long>> cycles = new();
            foreach (var w in watch)
                cycles.Add(w, new());

            var buttonPresses = 0;
            while (buttonPresses < 1000 || part2)
            {
                buttonPresses++;
                var q = new Queue<(string sender, string name, int pulse)>();

                void Send(List<string> dests, string sender, int pulse)
                {
                    //  var h = pulse == 0 ? "low" : "high";
                    //  Console.Write($"{sender} -{h}-> ");
                    //  Dump(module.dests, true);
                    foreach (var nn in dests)
                        q.Enqueue((sender, nn, pulse));
                }

                q.Enqueue(("button", "broadcaster", 0));
                while (q.Any())
                {
                    var (sender, name, pulse) = q.Dequeue();
                    pulses[pulse]++;

                    if (part2 && cycles.ContainsKey(name) && pulse == 0)
                    {
                        //Console.WriteLine($"Ding {name}");
                        cycles[name].Add(buttonPresses);
                    }

                    if (!dict.ContainsKey(name))
                        continue;

                    var module = dict[name];
                    int nextPulse = -1;
                    if (module.type == GT.FlipFlop)
                    {
                        if (pulse == 0)
                        {
                            flops[module.stateIndex] = !flops[module.stateIndex];
                            nextPulse = flops[module.stateIndex] ? 1 : 0;
                        }
                        else nextPulse = -1; // no pulse
                    }
                    else if (module.type == GT.Conjunction)
                    {
                        var memory = conjs[module.stateIndex];
                        if (!memory.ContainsKey(sender))
                            memory.Add(sender, false);
                        memory[sender] = pulse != 0;
                        nextPulse = memory.Values.All(v => v) ? 0 : 1;
                    }
                    else if (module.type == GT.Broadcaster)
                        nextPulse = pulse;

                    if (nextPulse != -1)
                        Send(module.dests, name, nextPulse);
                }

                // if ((buttonPresses % 1000) == 0)
                //     Console.WriteLine($"Presses {buttonPresses}");

                if (part2)
                {
                    if (cycles.Values.All(v => v.Count > 1))
                        return NumberTheory.LCM(cycles.Values.Select(v => v[0]).ToList());
                }
            }

            //  Console.WriteLine($"low {pulses[0]} high {pulses[1]} ans {pulses[0] * pulses[1]}");
            return pulses[0] * pulses[1];
        }
    }
}