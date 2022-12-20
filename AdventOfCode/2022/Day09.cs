namespace Lomont.AdventOfCode._2022
{
    internal class Day09 : AdventOfCode
    {
        //           1st star  both stars
        // rank1      2:41       6:00
        // rank100    7:32      14:08
        //
        // Day       Time    Rank  Score       Time    Rank  Score
        //  9   00:08:15     161      0   00:17:37     246      0
        //         
        // 5695
        // 2434
        //
        // 1500 people by 12:32 am, 5000 by 1 am, 25000 by 9 am


        public override object Run(bool part2)
        {
            var len = part2 ? 10 : 2; // # of links in rope

            var rope = new vec3[len];    // init rope
            for (var i = 0; i < len; ++i)
                rope[i] = new vec3();
            
            // track unique visited places
            var visited = new HashSet<(int, int)>();

            var dirs = new Dictionary<char, vec3>
            {
                ['R'] = new (1, 0),
                ['L'] = new (-1, 0),
                ['U'] = new (0, -1),
                ['D'] = new (0, 1)
            };


            foreach (var line in ReadLines())
            {
                var (dir,n) = (dirs[line[0]], Numbers(line)[0]);

                while (n-->0)
                {
                    Mutate(rope, i => 
                        i==0
                        ? rope[i]+dir
                        : Move(rope[i], rope[i - 1])
                        );
                    visited.Add(rope.Last().xy);
                }
            }
            return visited.Count;

            vec3 Move(vec3 follower, vec3 leader) => 
                (leader - follower).LengthSquared <= 2
                ? follower
                : follower + (leader - follower).Sign();

        }

    }
}