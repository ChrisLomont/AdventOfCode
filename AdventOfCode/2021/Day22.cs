
using Lomont.AdventOfCode.Utils;

namespace Lomont.AdventOfCode._2021
{
    //2021 Day 22 part 1: 615869 in 47912.2 us
    //2021 Day 22 part 2: 1323862415207825 in 2089874.2 us

    // new method - as expected, scales differently. Much nicer to make
    // 2021 Day 22 part 1: 615869 in 20748.7 us
    // 2021 Day 22 part 2: 1323862415207825 in 4684927.5 us


    internal class Day22 : AdventOfCode
    {
        long FastDay22(bool part2)
        {
            // parse all coords, use them as cell edges, each cell then size 1 until end
            var lines = ReadLines();

            var xH = new HashSet<long>();
            var yH = new HashSet<long>();
            var zH = new HashSet<long>();

            var boxes = new List<(vec3 min, vec3 max, bool on)>();

            long bound = part2?long.MaxValue/2:50; // part 1

            foreach (var line in lines)
            {
                var n = Numbers(line);
                if (n.Max()>bound || n.Min() < -bound) 
                    continue; // out of bounds this pass
                var (x1, x2, y1, y2, z1, z2) = (n[0], n[1], n[2], n[3], n[4], n[5]);
                x2++; // make max edges not part of items
                y2++;
                z2++;
                xH.Add(x1);
                xH.Add(x2);
                yH.Add(y1);
                yH.Add(y2);
                zH.Add(z1);
                zH.Add(z2);
                var on = line.Contains("on");
                boxes.Add((new vec3(x1, y1, z1), new vec3(x2, y2, z2), on));
            }

            // sort to get indices
            var xV = new List<long>( );
            xV.AddRange(xH);
            var yV = new List<long>();
            yV.AddRange(yH);
            var zV = new List<long>();
            zV.AddRange(zH);
            xV.Sort();
            yV.Sort();
            zV.Sort();

            var dx = xV.Count;
            var dy = yV.Count;
            var dz = zV.Count;
            //Console.WriteLine($"{dx*dy*dz} cells");

            // true if on, default to off
            bool[,,] grid = new bool[dx,dy,dz];

            // set cells on/off
            foreach (var (min, max, on) in boxes)
            {
                var (x1, x2) = (Get(xV, min.x), Get(xV, max.x));
                var (y1, y2) = (Get(yV, min.y), Get(yV, max.y));
                var (z1, z2) = (Get(zV, min.z), Get(zV, max.z));
                for (var i = x1; i < x2; ++i) // Note we use < and not <= here - it's correct
                for (var j = y1; j < y2; ++j)
                for (var k = z1; k < z2; ++k)
                    grid[i, j, k] = on;
            }

            // compute volume of all on cells
            long volume = 0;
            for (var i = 0; i < dx; ++i) // Note we use < and not <= here - it's correct
            for (var j = 0; j < dy; ++j)
            for (var k = 0; k < dz; ++k)
            {
                if (grid[i, j, k])
                {
                    var (x1, x2) = (xV[i], xV[i + 1]);
                    var (y1, y2) = (yV[j], yV[j + 1]);
                    var (z1, z2) = (zV[k], zV[k + 1]);
                    volume += (x2 - x1) * (y2 - y1) * (z2 - z1);
                }
            }

            return volume;



            // get index of coord
            long Get(List<long> coords, int coord)
            {
                var ind = coords.IndexOf(coord);
                Trace.Assert(0 <= ind && ind < coords.Count);
                return ind;
            }


        }

        class Box
        {
            public Box(vec3 min, vec3 max, int val)
            {
                Min = min; Max = max;
                Val = val;
                Orient();
            }

            public vec3 Min, Max;
            public int Val;
            // fix min, max
            public void Orient()
            {
                (Min, Max) = (vec3.Min(Min,Max), vec3.Max(Min,Max));
            }

            public ulong Volume()
            {
                var (x, y, z) = (Max - Min);
                var x1 = (ulong) x;
                var y1 = (ulong)y;
                var z1 = (ulong)z;
                return (x1+1) * (y1+1) * (z1+1);
            }
        }

        public object Run2(List<(vec3 v1, vec3 v2, bool on)> commands, bool part2)
        {
            var boxes = new List<Box>();
            var m = commands.Select(c => Math.Max(c.v1.AbsMax, c.v2.AbsMax)).Max();

            if (!part2) m = 50;
            //if (part2) return -1;

            var universe = new Box(new vec3(-m,-m,-m),new vec3(m,m,m),0);

            // one large off box
            boxes.Add(new Box(new vec3(-m, -m, -m), new vec3(m, m, m), 0));

            // each command can split any/all boxes
            foreach (var command in commands)
            {
                var cutter = new Box(command.v1, command.v2, command.on ? 1 : 0);

                // clip commands
                if (!Inside(cutter, universe))
                    continue;

                //Console.WriteLine($"{boxes.Count}, score {Score(boxes)}, cut {cutter.Min} {cutter.Max}");

                var b2 = new List<Box>();
                foreach (var b in boxes)
                {
                    var splits = Split(b, cutter);
                    b2.AddRange(splits);
                }

                boxes = b2;

                foreach (var b in boxes)
                {
                  //  Console.WriteLine($"   {b.Val} {b.Min} {b.Max} {b.Volume()}");
                }

                //return -1;

            }

            return Score(boxes);

            static ulong Score(List<Box> ss)
            {
                ulong sum = 0;
                foreach (var b in ss)
                {
                    if (b.Val == 0) continue;
                    sum += b.Volume();
                }
                return sum;
           }

        }

        // split b with cut, return cut up boxes
        List<Box> Split(Box b, Box cut)
        {
            var ans = new List<Box> {b};
            if (!Intersects(b, cut))
                return ans;
            if (Inside(b, cut))
            {
                b.Val = cut.Val;
                return ans;
            }

            // split x, cycle coords, 3 times
            ans = SplitX2(ans, cut);
            Rot(ans, cut); // x->y->z =>
            ans = SplitX2(ans, cut);
            Rot(ans, cut); // x->y->z->x
            ans = SplitX2(ans, cut);
            Rot(ans, cut); // x->y->z->x

            // set values  - each box should be inside or not intersect cut
            foreach (var b1 in ans)
            {
                if (Inside(b1, cut))
                    b1.Val = cut.Val;
                else Trace.Assert(!Intersects(b1, cut));
            }

            return ans;

        }

        // split boxes on x with cutter, up to two sides
        List<Box> SplitX2(List<Box> boxes, Box cut)
        {
            var (x3, x4) = (cut.Min.x, cut.Max.x);

            boxes = SplitX1(boxes, x3 - 0.5); // push past edge
            boxes = SplitX1(boxes, x4 + 0.5); // push past edge


            return boxes;
        }

        // split boxes on x value
        List<Box> SplitX1(List<Box> boxes, double cutX)
        {
            var ans = new List<Box>();
            var xLow = (int) Math.Floor(cutX);
            var xHigh = (int)Math.Ceiling(cutX);
            foreach (var b in boxes)
            {
                var (x1, y1, z1) = b.Min;
                var (x2, y2, z2) = b.Max;
                var v = b.Val;
                if (x1 < cutX && cutX < x2)
                {
                    
                    ans.Add(new Box(
                        new vec3(x1,y1,z1),
                        new vec3(xLow,y2,z2),
                        v));
                    ans.Add(new Box(
                        new vec3(xHigh, y1, z1),
                        new vec3(x2, y2, z2),
                        v)); 
                }
                else
                    ans.Add(b);
            }

            return ans;

        }




        // left inside right?
        bool Inside(Box lhs, Box rhs) => rhs.Min <= lhs.Min && lhs.Max <= rhs.Max;

        bool Intersects(Box p, Box q)
        {
            if (p.Max.x < q.Min.x) return false;
            if (p.Max.y < q.Min.y) return false;
            if (p.Max.z < q.Min.z) return false;
            if (p.Min.x > q.Max.x) return false;
            if (p.Min.y > q.Max.y) return false;
            if (p.Min.z > q.Max.z) return false;
            return true;
        }

        // cycle x,y,z
        void Rot(List<Box> bx, Box cut)
        {
            foreach (var b in bx)
            {
                b.Min = Rot1(b.Min);
                b.Max = Rot1(b.Max);
                b.Orient();
            }

            cut.Min = Rot1(cut.Min);
            cut.Max = Rot1(cut.Max);
            cut.Orient();

            vec3 Rot1(vec3 v) => new vec3(v.y, v.z, v.x);
        }


        public override object Run(bool part2)
        {
#if true
            return FastDay22(part2);
#else

            List<(vec3 v1, vec3 v2, bool on)> commands = new();
            foreach (var line in ReadLines())
            {
                var n = Numbers(line);
                var on = line.Contains("on");
                var (x1,x2) = (n[0], n[1]);
                var (y1, y2) = (n[2], n[3]);
                var (z1, z2) = (n[4], n[5]);
                commands.Add(new(new vec3(x1,y1,z1),new vec3(x2,y2,z2),on));
            }

            //if (part2)
                return Run2(commands,part2);

            var m = commands.Select(c => Math.Max(c.v1.AbsMax, c.v2.AbsMax)).Max();
            if (!part2)
                m = 50;

            var s = m;
            var (w, h) = (2 * s + 1, 2 * s + 1);
            var c = new int[2 * s + 1, 2 * s + 1, 2 * s + 1];

            foreach (var (v1,v2,on) in commands)
            {
                var (x1, y1, z1) = (v1.x, v1.y, v1.z);
                var (x2, y2, z2) = (v2.x, v2.y, v2.z);
                SetC(x1, x2, y1, y2, z1, z2, on ? 1 : 0);
            }

            long count = 0;
            for (var x = 0; x < w; ++x)
            for (var y = 0; y < w; ++y)
            for (var z = 0; z < w; ++z)
            {
                count += c[x, y, z];
            }
            
            return count;

            void SetC(int x1, int x2, int y1, int y2, int z1, int z2, int val)
            {
                (x1, x2) = (Math.Min(x1,x2), Math.Max(x1,x2));
                (y1, y2) = (Math.Min(y1,y2), Math.Max(y1,y2));
                (z1, z2) = (Math.Min(z1,z2), Math.Max(z1,z2));
                if (x2 < -s || y2 < -s || z2 < -s) return;
                if (s < x1 || s < y1 || s < z1) return;
                for (var x = x1; x <= x2; ++x)
                for (var y = y1; y <= y2; ++y)
                for (var z = z1; z <= z2; ++z)
                {
                    Set(x,y,z,val);
                }

            }
            void Set(int i, int j, int k, int val)
            {
                i += s;
                j += s;
                k += s;
                if (i<0 || j < 0 || k < 0 || w <= i || w <= j || w <= k)
                    return;
                c[i, j, k] = val;
            }
#endif
        }
    }
}