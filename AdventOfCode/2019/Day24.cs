using System.Runtime.InteropServices.JavaScript;

namespace Lomont.AdventOfCode._2019
{
    internal class Day24 : AdventOfCode
    {
        object Run2()
        {

            var (w, h, g) = CharGrid();


            // entry t is i+j*5
            // so upper right, along top, then next
            // each list is a list of nbrs in form (index, delta level)
            // 12 has none - it's the inner level
            var nbrsTbl = new List<List<int>>
            {
                //0
                new(){7,1, 1,0, 5,0, 11, 1},
                //1
                new(){7,1, 2,0, 6,0, 0, 0},
                //2
                new(){7,1, 3,0, 7,0, 1, 0},
                //3
                new(){7,1, 4,0, 8,0, 2, 0},
                //4
                new(){7,1, 13,1, 9,0, 3, 0},

                //5
                new(){0,0, 6,0, 10,0, 11, 1},
                //6
                new(){1,0, 7,0, 11,0, 5, 0},
                //7
                new(){2,0, 8,0, 6,0, 0,-1,1,-1,2,-1,3,-1,4,-1},
                //8
                new(){3,0, 9,0, 13,0, 7, 0},
                //9
                new(){4,0, 13,1, 14,0, 8, 0},

                //10
                new(){5,0, 11,0, 15,0, 11, 1},
                //11
                new(){6,0, 16,0, 10,0, 0,-1,5,-1,10,-1,15,-1,20,-1},
                //12
                new(){}, // center - do nothing here :)
                //13
                new(){8,0, 14,0, 18,0, 4,-1,9,-1,14,-1,19,-1,24,-1},
                //14
                new(){9,0, 13,1,  19,0, 13, 0},

                //15
                new(){10,0, 16,0, 20,0, 11, 1},
                //16
                new(){11,0, 17,0, 21,0, 15,0},
                //17
                new(){18,0,22,0,16,0, 20,-1,21,-1,22,-1,23,-1,24,-1},
                //18
                new(){13,0, 19,0, 23,0, 17,0},
                //19
                new(){14,0, 13,1, 24,0,  18,0},

                //20
                new(){15,0, 21,0, 17,1, 11, 1},
                //21
                new(){16,0, 22,0, 17,1, 20,0},
                //22
                new(){17,0, 23,0, 17,1, 21,0},
                //23
                new(){18,0, 24,0, 17,1, 22,0},
                //24
                new(){19,0, 13,1, 17,1, 23,0},
            };

            Dump(g, true);

            HashSet<(int pos,int lvl)> hasBug = new();
            Apply(g, (i, j, v) =>
            {
                if (v == '#')
                    hasBug.Add((i+j*5,0));
                return v;
            });

            for (var pass = 0; pass < 200; ++pass)
            {
                var dst = new HashSet<(int pos, int lvl)>();

                var minLevel = hasBug.Min(v => v.lvl) - 1;
                var maxLevel = hasBug.Max(v => v.lvl) + 1;

                for (var lvl = minLevel; lvl <= maxLevel; ++lvl)
                {
                    // loop over dest indices
                    for (var pos = 0; pos < 25; ++pos)
                    {
                        if (pos == 12) continue; // center
                        var nbrs = nbrsTbl[pos];
                        var cnt = 0;
                        for (var k = 0; k < nbrs.Count; k += 2)
                        {
                            var (nbrPos,dLevel) = (nbrs[k], nbrs[k+1]);
                            if (hasBug.Contains((nbrPos,lvl+dLevel)))
                                ++cnt;
                        }

                        var bug = hasBug.Contains((pos, lvl));
                        if (bug && cnt == 1)
                        {
                            dst.Add((pos, lvl));
                        }

                        if (!bug && (cnt == 1 || cnt == 2))
                        {
                            dst.Add((pos, lvl));
                        }
                    }
                }

                hasBug = dst;
            }

            //for (var lv = 6; lv >=-6; --lv)
            //    Dump1(lv);

            void Dump1(int level)
            {
                Console.WriteLine($"Depth {-level}:");
                for (var j = 0; j < 5; ++j)
                {
                    for (var i = 0; i < 5; ++i)
                        Console.Write(hasBug.Contains((i+j*5,level))?"#":'.');

                    Console.WriteLine();
                }
                Console.WriteLine();
            }


            return hasBug.Count;

        }

        public override object Run(bool part2)
        {
            if (part2) return Run2();

            var (w, h, g1) = CharGrid();
            var g2 = new char[w, h];

            var dirs = new[]
            {
                vec3.XAxis,
                vec3.YAxis,
                -vec3.XAxis,
                -vec3.YAxis,
            };

            Dump(g1, true);
            var seen = new HashSet<ulong>();
            while (true)
            {
                void F(char[,] g1, char[,] g2)
                {
                    Apply(g2, (i, j, v) =>
                    {
                        var q = new vec3(i, j);
                        var p = g1[i, j];
                        var cnt = 0;
                        foreach (var d in dirs)
                        {
                            var (i1, j1, _) = q + d;
                            var c = Get(g1, i1, j1, ' ');
                            if (c == '#') 
                                cnt++;
                        }

                        if (p == '#' && cnt != 1) return '.';
                        if (p == '.' && (cnt == 1 || cnt == 2)) return '#';
                        return g1[i, j];
                    });
                }

                F(g1,g2);
                (g1, g2) = (g2, g1);

                

                var score = Score(g1);
                if (seen.Contains(score))
                    return score;
                seen.Add(score);


                ulong Score(char[,] g)
                {
                    ulong s = 0;
                    ulong v = 1;
                    for(var j = 0; j < h; ++j)
                    for (var i = 0; i < w; ++i)
                    
                    {
                        if (g[i,j]=='#') s+=v;
                        v <<= 1;
                    }
                    return s;
                }

                //Dump(g1, true);
            }

            return -1244;


        }
    }
}