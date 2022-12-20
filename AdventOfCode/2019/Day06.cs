namespace Lomont.AdventOfCode._2019
{
    internal class Day06 : AdventOfCode
    {
        public override object Run(bool part2)
        {
            // map orbting item to center
            Dictionary<string, string> orbits = new();

            foreach (var line in ReadLines())
            {
                var w = line.Split(')');
                var (center, orbiting) = (w[0], w[1]);
                orbits.Add(orbiting,center);
            }

            if (!part2)
            {
                var c = 0;
                foreach (var p in orbits)
                {
                    var (orb, cen) = (p.Key, p.Value);
                    c++;
                    while (orbits.ContainsKey(cen))
                    {
                        ++c;
                        cen = orbits[cen];
                    }
                }

                return c;
            }

            var p1 = Pref("YOU");
            var p2 = Pref("SAN");
            //Dump(p1, true);
            //Dump(p2, true);

            foreach (var p in p1)
            {
                var ind2 = p2.IndexOf(p);
                if (ind2 >= 0)
                {
                    var ind1 = p1.IndexOf(p);


                    return ind1 + ind2-2;
                }
            }

            return -1234;

            List<string> Pref(string cen)
            {
                List<string> pref = new();
                pref.Add(cen);
                while (orbits.ContainsKey(cen))
                {
                    cen = orbits[cen];
                    pref.Add(cen);
                }
                return pref;
            }

        }
    }
}