

using System.Formats.Asn1;
using System.Runtime.InteropServices.JavaScript;

namespace Lomont.AdventOfCode._2024
{
    internal class Day12 : AdventOfCode
    {
        object Run2()
        {
            List<List< vec2 >> regions = new();
            long answer = 0;
            var (w, h, g) = CharGrid();
            char em = '.';
            for (var j = 0; j < h; ++j)
            for (var i = 0; i < w; ++i)
            {
                if (g[i, j] != em)
                {
                    //Console.WriteLine();
                    //Dump(g, noComma: true);

                    var r = new List<vec2>();
                    Flood(i, j, r);
                    regions.Add(r);
                }
            }


            var vv = regions.Select(r => Score(r));


            foreach (var (s, a) in vv)
            {
                //Console.WriteLine($"area, sides {a} * {s.Count}");
                answer += s.Count * a;
            }

            (List<(vec2,vec2)> side, long area) Score(List<vec2> region)
            {
                var a = region.Count;
                List<(vec2, vec2)> sides = new List<(vec2, vec2)>();
                foreach (var r in region)
                {
                    var (x, y) = r;
                    // CCW
                    sides.Add((new(x, y), new(x + 1, y)));
                    sides.Add((new(x+1, y), new(x +1, y+1)));
                    sides.Add((new(x+1, y+1), new(x , y+1)));
                    sides.Add((new(x, y+1), new(x , y)));
                }


                foreach (var r in region)
                {
                    var (x, y) = r;

                    if (Nbr(r, 1, 0))
                        Remove(new vec2(x + 1, y), new vec2(x + 1, y + 1));
                    if (Nbr(r, -1, 0))
                        Remove(new vec2(x , y+1), new vec2(x,y));
                    if (Nbr(r, 0, 1))
                        Remove(new vec2(x + 1, y+1), new vec2(x , y + 1));
                    if (Nbr(r, 0, -1))
                        Remove(new vec2(x , y ), new vec2(x+1, y));

                }

                Collapse(sides);

                return (sides, a);

                void Remove(vec2 a, vec2 b)
                {
                    for (int i = 0; i < sides.Count; ++i)
                    {
                        if (sides[i].Item1 == a && sides[i].Item2 == b)
                        {
                            sides.RemoveAt(i);
                            return;
                        }

                    }
                    throw new NotImplementedException();

                }
                bool Nbr(vec2 sq, int dx, int dy)
                {
                    var n = new vec2(sq.x + dx, sq.y + dy);
                    return region.Contains(n);
                }

                void Collapse(List<(vec2,vec2)> sides)
                {
                    bool done = false;
                    while (!done)
                    {
                        done = true;
                        bool hit = false;
                        for (int i = 0; !hit && i < sides.Count; ++i)
                        {
                            for (int j = 0; !hit && j < sides.Count; ++j)
                            {
                                if (i == j) continue;
                                var a = sides[i];
                                var b = sides[j%sides.Count];
                                if (Extends(a,b))
                                {
                                    sides[i] = (a.Item1, b.Item2);
                                    sides.RemoveAt(j);
                                    done = false;
                                    hit = true;
                                }
                            }
                        }
                    }
                }

                bool Extends((vec2 s,vec2 e) a, (vec2 s,vec2 e) b)
                {
                    if (a.e != b.s) return false;
                    var d1 = a.e - a.s;
                    var d2 = b.e - b.s;
                    if (Math.Sign(d1.x) != Math.Sign(d2.x)) return false;
                    if (Math.Sign(d1.y) != Math.Sign(d2.y)) return false;
                    //Console.WriteLine("Extend");
                    return true;

                }


            }

            void Flood(int i, int j, List<vec2> r)
            {
                Rec(i, j, g[i, j]);

                void Rec(int x, int y, char ch)
                {
                    if (!Legal(x, y)) return;
                    if (g[x, y] != ch) return;
                    r.Add(new(x, y));
                    g[x, y] = em;
                    Rec(x + 1, y, ch);
                    Rec(x - 1, y, ch);
                    Rec(x, y + 1, ch);
                    Rec(x, y - 1, ch);
                }

            }

            bool Legal(int i, int j) => 0 <= i && 0 <= j && i < w && j < h;


            return answer;
        }
        //public override string TestFileSuffix() => "-test1";
        public override string TestFileSuffix() => "";

        object Run1()
        {
            List<List<vec2>> regions = new();
            long answer = 0;
            var (w,h,g) = CharGrid();
            char em = '.';
            for (var i = 0; i < w; ++i)
            for (var j = 0; j < h; ++j)
            {
                if (g[i, j] != em)
                {
                    //Console.WriteLine();
                    //Dump(g, noComma: true);

                        var r = new List<vec2>();
                    Flood(i, j, r);
                    regions.Add(r);
                }
            }

            var vv = regions.Select(r => Score(r));
            foreach (var (p,a) in vv)
                answer += p*a;
            
            (long perim, long area) Score(List<vec2> region)
            {
                var a = region.Count;
                var p = 4 * a;
                foreach (var r in region)
                {
                    if (Nbr(r, 1, 0))
                        p -= 1;
                    if (Nbr(r, -1, 0))
                        p -= 1;
                    if (Nbr(r, 0, 1))
                        p -= 1;
                    if (Nbr(r, 0, -1))
                        p -= 1;

                }
                return (p,a);

                bool Nbr(vec2 sq, int dx, int dy)
                {
                    var n = new vec2(sq.x + dx, sq.y + dy);
                    return region.Contains(n);
                }

            }

            void Flood(int i, int j, List<vec2> r)
            {
                Rec(i, j, g[i,j]);

                void Rec(int x, int y, char ch)
                {
                    if (!Legal(x, y)) return;
                    if (g[x, y] != ch) return;
                    r.Add(new (x,y));
                    g[x, y] = em;
                    Rec(x+1,y,ch);
                    Rec(x - 1, y,ch);
                    Rec(x , y+1,ch);
                    Rec(x , y-1,ch);
                }

            }

            bool Legal(int i, int j) => 0 <= i && 0 <= j && i < w && j < h;


            return answer;
        }

        public override object Run(bool part2)
        {
            return part2 ? Run2() : Run1();
        }
    }
}