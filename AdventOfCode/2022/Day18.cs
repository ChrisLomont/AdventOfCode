namespace Lomont.AdventOfCode._2022
{
    internal class Day18 : AdventOfCode
    {
        // top 100 4:24 to 12:29 both
        // 1:05 to 2:55 for part 2

        // me
        // 18   00:06:10     630      0   00:53:48    1833      0
        //
        // 1837 byte 12:54
        // 

        //2022 Day 18 part 1: 3496 in 17785.1 us
        //2022 Day 18 part 2: 2064 in 26309 us


        public override object Run(bool part2)
        {
            var lines = ReadLines();

            var ob = new HashSet<vec3>();
            foreach (var line in lines)
            {
                var n = Numbers(line);
                var v = new vec3(n[0], n[1], n[2]);
                ob.Add(v);
            }

            static int Area(HashSet<vec3> ob)
            {

                var faces = 6 * ob.Count;
                var covered = 0;
                foreach (var v in ob)
                {
                    for (var i = -1; i <= 1; i += 2)
                    {
                        if (ob.Contains(v + new vec3(i, 0, 0)))
                        {
                            ++covered;
                        }

                        if (ob.Contains(v + new vec3(0, i, 0)))
                        {
                            ++covered;
                        }

                        if (ob.Contains(v + new vec3(0, 0, i)))
                        {
                            ++covered;
                        }
                    }
                }

                var area = faces - covered;
                return area;
            }

            if (!part2) return Area(ob);


            var min = ob.Aggregate(
                new vec3(int.MaxValue, int.MaxValue, int.MaxValue),
                (a, b) => vec3.Min(a, b));
            var max = ob.Aggregate(
                new vec3(int.MinValue, int.MinValue, int.MinValue),
                (a, b) => vec3.Max(a, b));
            Console.WriteLine($"{min} {max}");

            min -= new vec3(1, 1, 1);
            max += new vec3(1, 1, 1);


            var (dx, dy, dz) = max - min;

            var outArea = (dx + 1) * (dy + 1) * 2 + (dx + 1) * (dz + 1) * 2 + (dy + 1) * (dz + 1) * 2;

            var outs = Fill2(min);
            var a1 = Area(outs);
            Console.WriteLine(a1);
            //return a1 - outArea; // not 2032, not 2048

            int faceArea = 0;
            foreach (var o in ob)
            {
                foreach (var d in Dirs())
                {
                    if (outs.Contains(o + d)) ++faceArea;
                }
            }

            return faceArea;


            // get all trapped, else 0
            HashSet<vec3> Fill2(vec3 start)
            {
                var f1 = new Queue<vec3>();
                f1.Enqueue(start);
                var inQueue = new HashSet<vec3>();

                var closed1 = new HashSet<vec3>();
                while (f1.Any())
                {
                    var cur = f1.Dequeue();
                    closed1.Add(cur);
                    inQueue.Remove(cur);
                    foreach (var d in Dirs())
                    {
                        var p = cur + d;
                        if (!ob.Contains(p) && !closed1.Contains(p)
                                            && min <= p && p <= max
                                            && !inQueue.Contains(p)
                                            )

                        {
                            f1.Enqueue(p);
                            inQueue.Add(p);
                        }
                    }
                }
                return closed1;
            }



            min += new vec3(-1, -1, -1);
            max -= new vec3(-1, -1, -1);




            var f = new Queue<vec3>();
            var closed = new HashSet<vec3>();

            var fil = new HashSet<vec3>();

            while (f.Any())
            {
                var cur = f.Dequeue();
                closed.Add(cur);

                for (var i = -1; i <= 1; i += 2)
                {
                    var d1 = cur + new vec3(i, 0, 0);
                    var d2 = cur + new vec3( 0, i,0);
                    var d3 = cur + new vec3(0, 0,i);

                    Do(d1);
                    Do(d2);
                    Do(d3);

                    void Do(vec3 v)
                    {
                        if (
                            min<=v && v <= max && 
                            !ob.Contains(v) && closed.Contains(v)
                            )
                        {
                            fil.Add(v);
                            f.Enqueue(v);
                        }
                    }
                }
            }

            //var filled = fil.Count;
            //var (vx, vy, vz) = max - min;
            //var vol = vx * vy * vz;

            // return vol - filled - ob.Count;

            IEnumerable<vec3> Dirs()
            {
                for (var i = -1; i <= 1; i += 2)
                {
                    yield return new vec3(i, 0, 0);
                    yield return new vec3(0, i, 0);
                    yield return new vec3(0, 0, i);
                }
            }

            var outside = new HashSet<vec3>();
            var filled = new HashSet<vec3>();
            var inArea = 0;
            var c = 0;
            foreach (var v in ob)
            {
                Console.WriteLine($"{c++} of {ob.Count}");
                foreach (var d in Dirs())
                    if (!ob.Contains(v + d) && !filled.Contains(v+d) && !outside.Contains(v+d))
                    {

                        var enclosed = Fill(v + d);
                        if (enclosed.Any())
                        {
                            Console.WriteLine($"Filled {enclosed.Count}, {enclosed.First()}, outside {outside.Count}");
                        }

                        foreach (var e in enclosed)
                        {
                            filled.Add(e);
                            foreach (var d1 in Dirs())
                            {
                                if (ob.Contains(d1 + e))
                                {
                                    ++inArea;
                                }
                            }
                        }
                    }
            }

            return -1234; // area-inArea;


            // get all trapped, else 0
            HashSet<vec3> Fill(vec3 start)
            {
                var f1 = new Queue<vec3>();
                f1.Enqueue(start);
                
                var closed1 = new HashSet<vec3>();
                while (f1.Any())
                {
                    var cur = f1.Dequeue();
                    closed1.Add(cur);
                    foreach (var d in Dirs())
                    {
                        var p = cur + d;
                        if (!ob.Contains(p) && !closed1.Contains(p))
                        {
                            f1.Enqueue(p);
                        }
                        if (filled.Contains(p) || p <= min || max <= p)
                        {
                            outside.UnionWith(closed1);
                            return new HashSet<vec3>(); // outside
                        }
                    }
                }
                return closed1;
            }



           



            
        }
    }
}