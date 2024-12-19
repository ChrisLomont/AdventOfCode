

namespace Lomont.AdventOfCode._2018
{
    internal class Day06 : AdventOfCode
    {
        object Run2()
        {

            long answer = 0;
            // var (w,h,g) = DigitGrid();
            // var (w,h,g) = CharGrid();
            // ProcessAllLines(new() { ... regex->action list ... });
            List<(long i, long j)> centers = new();
            foreach (var line in ReadLines())
            {
                var nums = Numbers64(line);
                centers.Add(new(nums[0], nums[1]));
            }

            var maxScore = 10000;
            var numCenters = centers.Count;
            // round up
            var maxSingle = (maxScore+numCenters-1)/numCenters;
            maxSingle = maxSingle; // helps
            Console.WriteLine($"Max single {maxSingle}");

            var minx = centers.Min(v => v.i);
            var maxx = centers.Max(v => v.i);
            var miny = centers.Min(v => v.j);
            var maxy = centers.Max(v => v.j);

            // grid overkill - can do walk of corners, picks theorem
            for (long i = minx - maxSingle; i <= maxx + maxSingle; ++i)
            for (long j = miny - maxSingle; j <= maxy + maxSingle; ++j)
            {
                long s = 0;
                foreach (var (x,y) in centers)
                {
                    long dx = x - i, dy = y-j;
                    s += Int64.Abs(dx) + Int64.Abs(dy);
                }

                if (s < maxScore) answer++;

            }



            return answer;
        }

        object Run1()
        {
            long answer = 0;
            // var (w,h,g) = DigitGrid();
            // var (w,h,g) = CharGrid();
            // ProcessAllLines(new() { ... regex->action list ... });
            List< (long i, long j)> centers = new();
            foreach (var line in ReadLines())
            {
                var nums = Numbers64(line);
                centers.Add(new(nums[0], nums[1]));
            }

            //centers.Clear();
            //centers.Add((5,5));

            var minx = centers.Min(v=>v.i);
            var maxx = centers.Max(v => v.i);
            var miny = centers.Min(v => v.j);
            var maxy = centers.Max(v => v.j);

            long w = maxx - minx + 1;
            long h = maxy - miny + 1;

            int sc = 2; // multiples
            w *= sc;
            h *= sc;
            long cx = w / 2, cy = h / 2;


            long[,] dists = new long[w,h];  // dist to owner
            long[,] owner = new long[w, h]; // owner
            Apply(dists, (i, j, v) => -1);  // no dist computed, -2 = tied
            Apply(owner, (i, j, v) => -1);  // no owner, -2 = tied

            // move centers
            centers = centers.Select(p=>(p.i-minx+cx,p.j-miny+cy)).ToList();

            //for (int i = 0; i < w; ++i)
            //for (int j = 0; j < h; ++j)
            for (int k = 0; k < centers.Count; ++k)
            {
                var (i, j) = centers[k];
                dists[i, j] = 0;
                owner[i, j] = k;
            }

            int fills = 0;
            // flood fill
            bool done = false;
            var dists2 = new long[w, h];
            var owner2 = new long[w, h];

            while (!done)
            {
                done = true; // assume

                Apply(dists2, (i, j, v) => dists[i,j]);
                Apply(owner2, (i, j, v) => owner[i, j]);

                for (int i = 0; i < w; ++i)
                for (int j = 0; j < h; ++j)
                {
                    var d = dists[i, j];
                    if (d != -1) 
                        continue; // done
                    var nbrs = Nbrs(i, j);

                    if (nbrs.Count > 0)
                    {
                        if (nbrs.Count == 1)
                        {
                            var (nx, ny) = nbrs[0];
                            dists2[i, j] = dists[nx, ny] + 1;
                            owner2[i, j] = owner[nx, ny];
                            ++fills;
                            done = false;
                        }
                        else if (nbrs.Count > 1)
                        {
                            // tied
                            ++fills;
                            dists2[i, j] = -2;
                            owner2[i, j] = -2;
                            done = false;
                        }
                    }

                }

                var t1 = owner;
                owner = owner2;
                owner2 = t1;
                var t2 = dists;
                dists = dists2;
                dists2 = t2;


              //  Draw();
              //  Console.WriteLine();
                   Console.WriteLine($"Fills {fills} / {w*h}");

            }

            Console.WriteLine("Filled");

            // find largest with no edge
            int n = centers.Count;
            bool [] hasEdge = new bool [n];
            long [] count = new long[n];
            Apply(owner, (i, j, owner) =>
            {
                if (owner < 0) return owner;

                var edge = i == 0 || j == 0 || i == w - 1 || j == h - 1;
                if (edge)
                    hasEdge[owner] = true;
                count[owner]++;
                return owner;
            });

            void Draw()
            {

                char[,] gg = new char[w, h];
                Apply(gg, (i, j, v) =>
                {
                    var s = owner[i, j];
                    if (s < 0) return '.';
                    return (char)(s + 'a');
                });

                for (int t = 0; t < centers.Count; ++t)
                {
                    var (i, j) = centers[t];
                    gg[i, j] = (char)(t + 'A');
                }

                Dump(gg, noComma: true);
            }

            long mx = 0;
            for (int i = 0; i < n; ++i)
                if (!hasEdge[i])
                {
                    Console.WriteLine($"Finite {i} has {count[i]}");
                    mx = Math.Max(mx, count[i]);
                }

            return mx;

            


            // locations with valid neighbor, and dist there
            // breaks ties, picks min in case of all from same owner
            List<(int i, int j)> Nbrs(int i, int j)
            {
                List<(int i, int j, long dist, long owner)> nbrs = new List<(int i, int j, long, long)>();
                if (Good(i + 1, j))
                    nbrs.Add((i + 1, j, dists[i + 1, j], owner[i + 1, j]));
                if (Good(i - 1, j))
                    nbrs.Add((i - 1, j, dists[i - 1, j], owner[i - 1, j]));
                if (Good(i, j + 1))
                    nbrs.Add((i, j + 1, dists[i , j+1], owner[i , j+1]));
                if (Good(i, j - 1))
                    nbrs.Add((i, j - 1, dists[i , j-1], owner[i , j-1]));

                // keep all with min dist
                if (nbrs.Count > 0)
                {
                    var md = nbrs.Min(p => p.dist);
                    nbrs = nbrs.Where(p => p.dist == md).ToList();
                }

                // if all same owner, reduce to 1 entry
                if (nbrs.Count > 0)
                {
                    var owner0 = nbrs[0].owner;
                    var allSame = nbrs.All(p => p.owner == owner0);
                    if (allSame)
                    {
                        var md = nbrs.Min(p => p.dist);
                        var best = nbrs.Where(n => n.dist == md).ToList()[0];
                        nbrs.Clear();
                        nbrs.Add(best);
                    }
                }


                return nbrs.Select(p=>(p.i,p.j)).ToList();
            }

            bool Good(int i, int j)
            {
                if (i < 0 || j < 0 || w <= i || h <= j) return false;
                if (dists[i, j] < 0) return false;
                if (owner [i, j] < 0) return false;
                return true;
            }





            // assume real board at most N times these


            return answer;
        }

        public override object Run(bool part2)
        {
            return part2 ? Run2() : Run1();
        }
    }
}
