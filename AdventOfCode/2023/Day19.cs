namespace Lomont.AdventOfCode._2023
{
    internal class Day19 : AdventOfCode
    {
        // 487623,  113550238315130, 

        record Rule(int feature, bool alwaysTake, bool gt, int cutoff, string dest);

#if false
        // min, max inclusive
        record Subcube(string rule, vec4 min, vec4 max);

        object RunA()
        {
            var lines = ReadLines();
            int lp = 0;
            var works = Rules(lines, ref lp);


            var q = new Queue<Subcube>();
            q.Enqueue(new ("in",new vec4(1,1,1,1), new vec4(4000,4000,4000,4000)));
            long result = 0;
            while (q.Any())
            {
                var subcube = q.Dequeue();
                if (subcube.rule == "A")
                    result += product;
                else if (subcube.rule != "R")// works.ContainsKey(workflow_id))
                {
                    var rules = works[workflow_id];
                    foreach (var rule in rules)
                    {
                        if (rule.alwaysTake)
                        {
                            q.Enqueue(subcube with {rule = rule.dest});
                        }
                        else
                        {
                            var v = rule.cutoff;
                            var (a, b) = (subcube.min[rule.feature], subcube.max[rule.feature]);

                            // possible split: one gets mapped, one stays
                            var stay = (0L, 0L);
                            var go   = (0L, 0L);

                            if (rule.gt)
                            {
                                // those > v go elsewhere, else continue
                                stay = (a, v);
                                go   = (v + 1, b);
                            }
                            else
                            {

                            }
//                        rule.dest;
//                        rule.feature;
                    }
                }
            }

            return result;
        }
#endif

        public object Run2()
        {
            //return RunA();

            var lines = ReadLines();
            int lp = 0;
            var works = Rules(lines, ref lp);

            long CountVariations(long[] r, string name = "in")
            {
                if (name == "R")
                    return 0;
                if (name == "A")
                {
                    long prod = 1;
                    for (int k = 0; k < 8; k += 2)
                        prod *= r[k + 1] - r[k] + 1;
                    return prod;
                }

                var rules = works[name];
                long total = 0;

                for (var rc = 0; rc < rules.Count; rc++)
                {
                    var rule = rules[rc];
                    if (rule.alwaysTake)
                    {
                        total += CountVariations(r, rule.dest);
                        break;
                    }

                    var rIndex = rule.feature * 2;
                    var (v1, v2) = (r[rIndex], r[rIndex + 1]);
                    long cut = rule.cutoff;


                    var (r1, r2) = rule.gt
                        ? ((cut+1, v2), (v1, cut))
                        : ((v1, cut-1), (cut, v2));

                    if (r1.Item1 <= r1.Item2)
                    {
                        var copy = new long[8];
                        Array.Copy(r, copy, r.Length);
                        copy[rIndex] = r1.Item1;
                        copy[rIndex + 1] = r1.Item2;
                        total += CountVariations(copy, rule.dest);
                    }

                    if (r2.Item1 <= r2.Item2)
                    {
                        r[rIndex] = r2.Item1;
                        r[rIndex + 1] = r2.Item2;
                    }
                }
                return total;
            }

            var range1 = new long[] { 1, 4000, 1, 4000, 1, 4000, 1, 4000 };
            return CountVariations(range1);
        }


        public object Run1()
        {

            long answer = 0;

            var lines = ReadLines();
            int lp = 0;
            var works = Rules(lines, ref lp);

            while (lp < lines.Count)
            {
                var line = lines[lp++];
                var nums = Numbers(line); // in order x,m,a,s
                bool next = false;
                var cur = works["in"];

                while (!next)
                {
                    foreach (var r in cur)
                    {
                        var transition = false;
                        if (r.alwaysTake)
                            transition = true;
                        else
                        {
                            var v = nums[r.feature];
                            transition = r.gt && v > r.cutoff;
                            transition |= !r.gt && v < r.cutoff;
                        }

                        if (transition)
                        {
                            if (r.dest == "A")
                            {
                                answer += nums.Sum();
                                next = true;
                            }
                            else if (r.dest == "R")
                            {
                                next = true;
                            }
                            else 
                                cur = works[r.dest];

                            break;
                        }
                    }
                }
            }

            return answer;
        }

        public override object Run(bool part2)
        {
            return part2 ? Run2() : Run1();
        }


        Dictionary<string, List<Rule>> Rules(List<string> lines, ref int lp)
        {
            Dictionary<string, List<Rule>> works = new();

            while (lines[lp].Trim() != "")
            {
                var line = lines[lp++];
                var w1 = line.Split('{');
                var workflowName = w1[0];
                var ops = w1[1][0..^1].Split(',');
                List<Rule> rules = new();
                foreach (var op in ops)
                {
                    if (op.Contains(':'))
                    {
                        var feature = "xmas".IndexOf(op[0]);
                        var gt = op[1];
                        var dest = op.Split(':')[1];
                        var cutoff = Numbers(op)[0];
                        rules.Add(new(feature, false, gt == '>', cutoff, dest));
                    }
                    else
                    {
                        rules.Add(new(' ', true, true, 0, op));
                    }
                }
                works.Add(workflowName, rules);
            }

            lp++;
            return works;
        }

#if false


        // for each letter, where to split
        Dictionary<char, List<int>> splits = new();

            foreach (var rules in works.Values)
            {
                foreach (var r in rules)
                {
                    if (r.alwaysTake)
                        continue;
                    
                }
            }



            // get all possible ranges
            // each of x,m,a,s in 1-4000 inclusive

            // int x1, int x2, int m1, int m2, int a1, int a2, int s1, int s2
            Queue<int[]>
                todo = new ();

            //HashSet<int[]> splits= new();

            todo.Enqueue(new int[]{1, 4000, 1, 4000, 1, 4000, 1, 4000});

            List<int[]> acceptedRanges = new();


                while (todo.Any())
                {
                    var range = todo.Dequeue();
                    Console.Write("Range: "); Dump(range,singleLine:true);

                    bool accepted = false;
                    bool rejected = false;
                    bool split = false;
                    var cur = works["in"];

                   Console.WriteLine($"   in");

                    while (!accepted && !rejected && !split)
                    {
                        foreach (var r in cur)
                        {
                            var accept = false;
                            if (r.alwaysTake)
                            {
                                accept = true;
                            }
                            else
                            {

                                var chInd = "xmas".IndexOf(r.inp);
                                var (v1, v2) = (range[2 * chInd], range[2 * chInd + 1]);
                                Trace.Assert(v1 <= v2);

                                var v = r.num;

                            // gt: vals>v: [v1 v], (v v2] && both exist
                            split = r.gt && v >= v1 && v < v2;
                            // lt: vals<v: [v1 v), [v v2] && both exist
                            split |= !r.gt && v > v1 && v <= v2;

                            if (split)
                                { 
                                    var ch = "xmas"[chInd];
                                    Console.WriteLine($"Split {ch}: {v1},{v},{v2}");
                                    if (r.gt)
                                    {
                                        
                                            Add(range, chInd, v1, v );
                                            Add(range, chInd, v + 1, v2);
                                    }
                                    else
                                    {
                                        
                                            Add(range, chInd, v1, v - 1);
                                        
                                            Add(range, chInd, v , v2);
                                    }

                                }

                                // add inclusive ends
                                void Add(int[] range, int chInd, int minV, int maxV)
                                {
                                    if (maxV < minV) return;
                                    int[] r2 = new int[8];
                                    Array.Copy(range, r2, 8);
                                    r2[chInd * 2] = minV;
                                    r2[chInd * 2 + 1] = maxV;
                                    todo.Enqueue(r2);
                                    Console.Write("add: "); Dump(r2, singleLine: true);
                                }

                                // accept whole range?
                                accept = r.gt && r.num < v1;
                                accept |= !r.gt && v2 < r.num;
                            }

                            if (accept && !split)
                            {
                                if (r.dest == "A")
                                {
                                    acceptedRanges.Add(range);
                                    accepted = true;
                                }
                                else if (r.dest == "R")
                                {
                                    rejected = true;
                                }
                                else
                                {
                                    Console.WriteLine($"   {r.dest}");
                                cur = works[r.dest]; // next rule to try
                                }

                                break;
                            }
                        }
                    }
                } // do queue

               var len = Tally(acceptedRanges);
               Console.WriteLine();
            Console.WriteLine($"Tally: {len}");

               // Trace.Assert(len != 167409079868000L);

               // Console.WriteLine($"{more.Count} splits, prod {len}");


               // if (someSplit)
               // {
               //     foreach (var r in more)
               //         todo.Enqueue(r);
               //     more.Clear();
               // }

         //   } while (someSplit);

            long Tally(List<int[]> more1)
            {
                long sum = 0;
                foreach (var r in more1)
                {
                    var xx = r[1] - r[0] + 1L;
                    var mm = r[3] - r[2] + 1L;
                    var aa = r[5] - r[4] + 1L;
                    var ss = r[7] - r[6] + 1L;
                    var p = xx * mm * aa * ss;
                    Console.Write($"Tally {p} for range ");
                    Dump(r, true);
                    sum += p;
                }

                return sum;

            }

         


            return answer;
        }
#endif
    }
}