using Lomont.AdventOfCode.Utils;

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
        // 1500 people by 12:32

        public override object Run(bool part2)
        {
            int len = part2?10:2;
            var rope = new vec3[len];
            for (var i = 0; i < len; ++i)
                rope[i] = new vec3(0,0);
            
            // where visited
            var visited = new HashSet<(int, int)>();

            foreach (var line in ReadLines())
            {
                var dir = line[0] switch
                {
                    'R' => new vec3(1, 0),
                    'U' => new vec3(0, -1),
                    'D' => new vec3(0, 1),
                    'L' => new vec3(-1, 0),
                    _ => throw new Exception()
                };
                var n = GetNumbers(line)[0];
                for (var k = 0; k < n; ++k)
                {
                    rope[0] += dir;
                    for (var i = 1; i < len; ++i)
                        rope[i] = Move(rope[i], rope[i - 1]);
                    visited.Add(new(rope[len-1].x, rope[len-1].y));
                }
            }
            return visited.Count;

            vec3 Move(vec3 follower, vec3 leader)
            {
                var dir = leader - follower;
                if (dir.LengthSquared > 2)
                    follower += dir.Sign();
                return follower;

            }

        }
    }
}