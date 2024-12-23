

namespace Lomont.AdventOfCode._2024
{
    internal class Day23 : AdventOfCode
    {/*
      * 2024 Day 23 part 1: 1083 in 702661.7 us
2024 Day 23 part 2: as,bu,cp,dj,ez,fd,hu,it,kj,nx,pp,xh,yu in 347602.8 us
                    as,bu,cp,dj,ez,fd,hu,it,kj,nx,pp,xh,yu
        */
        object Run2()
        {
            var (nbrs, names) = Load();

            var allCliques = new List<HashSet<string>>();
            BronKerbosch(new(), names.ToHashSet(), new(), nbrs, allCliques);

            var maxNames = allCliques.MaxBy(c => c.Count).ToList();
            maxNames.Sort();

            return maxNames.Aggregate("", (a, b) => a+","+b)[1..];
        }

        // max cliques algo
        void BronKerbosch(
            HashSet<string> R, HashSet<string> P, HashSet<string> X,
            Dictionary<string, HashSet<string>> nbrs,
            List<HashSet<string>> cliques)
        {
            if (!(P.Any() || X.Any()))
            {
                cliques.Add(R);
                return;
            }

            while (P.Any())
            {
                var v = P.First();
                HashSet<string> newR = new(R) { v };
                HashSet<string> newP = new(P);
                newP.IntersectWith(nbrs[v]);
                HashSet<string> newX = new(X);
                newX.IntersectWith(nbrs[v]);
                BronKerbosch(newR, newP, newX, nbrs, cliques);
                P.Remove(v);
                X.Add(v);
            }
        }

        //public override string TestFileSuffix() => "-test1";
        public override string TestFileSuffix() => "";

        (Dictionary<string, HashSet<string>> nbrs, List<string> names) Load()
        {
            Dictionary<string, HashSet<string>> nbrs = new();

            foreach (var ln in ReadLines())
            {
                var n1 = ln[0..2];
                var n2 = ln[3..5];
                Add(n1, n2);
                Add(n2, n1);
            }
            return (nbrs,nbrs.Keys.ToList());
            void Add(string a, string b)
            {
                if (!nbrs.ContainsKey(a))
                    nbrs.Add(a, new());
                nbrs[a].Add(b);
            }
        }

        object Run1()
        {
            var (nbrs, names) = Load();

            long answer = 0;

            var n = nbrs.Count;
            for (int a = 0; a < n; ++a)
            for (int b = a + 1; b < n; ++b)
            for (int c = b + 1; c < n; ++c)
            {
                var n1 = names[a];
                var n2 = names[b];
                var n3 = names[c];

                if (nbrs[n1].Contains(n2) && nbrs[n1].Contains(n3) && nbrs[n2].Contains(n3))
                {
                    if (n1[0] == 't' || n2[0] == 't' || n3[0] == 't')
                        answer++;
                }
            }

            return answer;
        }

        public override object Run(bool part2)
        {
            return part2 ? Run2() : Run1();
        }
    }
}