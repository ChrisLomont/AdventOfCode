


namespace Lomont.AdventOfCode._2018
{
    internal class Day24 : AdventOfCode
    {
        /*
648 units each with 10733 hit points (immune to radiation, fire, slashing) with an attack that does 143 fire damage at initiative 9
949 units each with 3117 hit points with an attack that does 29 fire damage at initiative 10
5776 units each with 5102 hit points (weak to cold; immune to slashing) with an attack that does 8 radiation damage at initiative 15
1265 units each with 4218 hit points (immune to radiation) with an attack that does 24 radiation damage at initiative 16
3088 units each with 10066 hit points (weak to slashing) with an attack that does 28 slashing damage at initiative 1
498 units each with 1599 hit points (immune to bludgeoning; weak to radiation) with an attack that does 28 bludgeoning damage at initiative 11
3705 units each with 10764 hit points with an attack that does 23 cold damage at initiative 7
3431 units each with 3666 hit points (weak to slashing; immune to bludgeoning) with an attack that does 8 bludgeoning damage at initiative 8

Infection:
2835 units each with 33751 hit points (weak to cold) with an attack that does 21 bludgeoning damage at initiative 13
4808 units each with 32371 hit points (weak to radiation; immune to bludgeoning) with an attack that does 11 cold damage at initiative 14
659 units each with 30577 hit points (weak to fire; immune to radiation) with an attack that does 88 slashing damage at initiative 12
5193 units each with 40730 hit points (immune to radiation, fire, bludgeoning; weak to slashing) with an attack that does 
         */

        /*
2018 Day 24 part 1: 26868 in 22653.7 us
2018 Day 24 part 2: 434 in 1020697 us         
         */

        // my code was a mess, this an online example (which was also wrong, needed not to use binary search)

        (bool immuneSystem, int units) Fight(string input, int b)
        {
            var army = Parse(input);
            foreach (var g in army)
            {
                if (g.immuneSystem)
                {
                    g.damage += b;
                }
            }

            var attack = true;
            while (attack)
            {
                attack = false;
                var remainingTarget = new HashSet<Group2>(army);
                var targets = new Dictionary<Group2, Group2>();
                foreach (var g in army.OrderByDescending(g => (g.effectivePower, g.initiative)))
                {
                    var maxDamage = remainingTarget.Select(t => g.DamageGivenTo(t)).Max();
                    if (maxDamage > 0)
                    {
                        var possibleTargets = remainingTarget.Where(t => g.DamageGivenTo(t) == maxDamage);
                        targets[g] = possibleTargets.OrderByDescending(t => (t.effectivePower, t.initiative)).First();
                        remainingTarget.Remove(targets[g]);
                    }
                }

                foreach (var g in targets.Keys.OrderByDescending(g => g.initiative))
                {
                    if (g.units > 0)
                    {
                        var target = targets[g];
                        var damage = g.DamageGivenTo(target);
                        if (damage > 0 && target.units > 0)
                        {
                            var dies = damage / target.hp;
                            target.units = Math.Max(0, target.units - dies);
                            if (dies > 0)
                            {
                                attack = true;
                            }
                        }
                    }
                }

                army = army.Where(g => g.units > 0).ToList();
            }

            return (army.All(x => x.immuneSystem), army.Select(x => x.units).Sum());
        }

        public override object Run(bool part2)
        {
            var inp = ReadText();
            if (!part2)
            {
                var f = Fight(inp,0);
                return f.units;
            }
            return PartTwo(inp);

            int PartTwo(string input)
            {
                int min = Int32.MaxValue;
                // cannot binary search, outcomes not smooth/sorted
                for (int boost = 1; boost < 100; ++boost)
                {
                    var f = Fight(input, boost);
                    if (f.immuneSystem)
                    {
                        if (f.units < min)
                            Console.WriteLine($"Boost {boost} -> {f.units}");
                        min = Math.Min(min, f.units);
                    }
                }

                return min;
            }
        }

        List<Group2> Parse(string input)
        {
            var lines = input.Split("\n");
            var immuneSystem = false;
            var res = new List<Group2>();
            foreach (var line in ReadLines())
                if (line == "Immune System:")
                {
                    immuneSystem = true;
                }
                else if (line == "Infection:")
                {
                    immuneSystem = false;
                }
                else if (line != "")
                {
                    //643 units each with 9928 hit points (immune to fire; weak to slashing, bludgeoning) with an attack that does 149 fire damage at initiative 14
                    var rx =
                        @"(\d+) units each with (\d+) hit points(.*)with an attack that does (\d+)(.*)damage at initiative (\d+)";
                    var m = Regex.Match(line, rx);
                    if (m.Success)
                    {
                        Group2 g = new Group2();
                        g.immuneSystem = immuneSystem;
                        g.units = int.Parse(m.Groups[1].Value);
                        g.hp = int.Parse(m.Groups[2].Value);
                        g.damage = int.Parse(m.Groups[4].Value);
                        g.attackType = m.Groups[5].Value.Trim();
                        g.initiative = int.Parse(m.Groups[6].Value);
                        var st = m.Groups[3].Value.Trim();
                        if (st != "")
                        {
                            st = st.Substring(1, st.Length - 2);
                            foreach (var part in st.Split(";"))
                            {
                                var k = part.Split(" to ");
                                var set = new HashSet<string>(k[1].Split(", "));
                                var w = k[0].Trim();
                                if (w == "immune")
                                {
                                    g.immuneTo = set;
                                }
                                else if (w == "weak")
                                {
                                    g.weakTo = set;
                                }
                                else
                                {
                                    throw new Exception();
                                }
                            }
                        }

                        res.Add(g);
                    }
                    else
                    {
                        throw new Exception();
                    }

                }

            return res;
        }


        class Group2
        {
            public bool immuneSystem;
            public int units;
            public int hp;
            public int damage;
            public int initiative;
            public string attackType;
            public HashSet<string> immuneTo = new HashSet<string>();
            public HashSet<string> weakTo = new HashSet<string>();

            public int effectivePower
            {
                get { return units * damage; }
            }

            public int DamageGivenTo(Group2 target)
            {
                if (target.immuneSystem == immuneSystem)
                {
                    return 0;
                }
                else if (target.immuneTo.Contains(attackType))
                {
                    return 0;
                }
                else if (target.weakTo.Contains(attackType))
                {
                    return effectivePower * 2;
                }
                else
                {
                    return effectivePower;
                }
            }
        }

    }
}