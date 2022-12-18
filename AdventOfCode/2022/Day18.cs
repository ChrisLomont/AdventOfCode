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

        //2022 Day 18 part 1: 3496 in 13052.1 us
        //2022 Day 18 part 2: 2064 in 15000 us

        public override object Run(bool part2)
        {
            var lines = ReadLines();

            var cubes = new HashSet<vec3>();
            foreach (var line in lines)
            {
                var n = Numbers(line);
                var v = new vec3(n[0], n[1], n[2]);
                cubes.Add(v);
            }

            // 6 directions
            static IEnumerable<vec3> Dirs()
            {
                for (var i = -1; i <= 1; i += 2)
                {
                    yield return new vec3(i, 0, 0);
                    yield return new vec3(0, i, 0);
                    yield return new vec3(0, 0, i);
                }
            }

            // compute area of shape by looking at all neighbors
            static int Area(HashSet<vec3> ob)
            {

                var faces = 6 * ob.Count; // total cube faces
                var covered = 0; // faces blocked
                foreach (var v in ob)
                    foreach (var d in Dirs())
                        if (ob.Contains(v + d))
                            covered++;
                var area = faces - covered;
                return area;
            }
            // first part
            if (!part2) return Area(cubes);

            // get bounding box (hope its not too big - mine ~20x20x20)
            var min = cubes.Aggregate(vec3.MaxValue, vec3.Min);
            var max = cubes.Aggregate(vec3.MinValue, vec3.Max);
            //Console.WriteLine($"{min} {max}");

            // expand box to ensure nothing trapped (there was!)
            vec3 one = new(1, 1, 1);
            min -= one;
            max += one;

            // flood fill the outside from a far corner
            var outs = FloodFill(min);
            
            // count area of cubes touching the flood fill
            int faceArea = 0;
            foreach (var o in cubes)
                foreach (var d in Dirs())
                    if (outs.Contains(o + d)) 
                        ++faceArea;

            return faceArea;


            // get all trapped, else 0
            HashSet<vec3> FloodFill(vec3 start)
            {
                var frontier = new Queue<vec3>();
                frontier.Enqueue(start);
                var inQueue = new HashSet<vec3>();

                var processed = new HashSet<vec3>();
                while (frontier.Any())
                {
                    var current = frontier.Dequeue();
                    processed.Add(current);
                    inQueue.Remove(current);
                    foreach (var dir in Dirs())
                    {
                        var next = current + dir;
                        if (!cubes.Contains(next) && 
                            !processed.Contains(next) &&
                            !inQueue.Contains(next) && 
                            min <= next && 
                            next <= max
                            )
                        {
                            frontier.Enqueue(next);
                            inQueue.Add(next);
                        }
                    }
                }
                return processed;
            }
        }
    }
}