using Lomont.GIFLib;
using System.Globalization;
using System.Numerics;
using System.Runtime.Intrinsics;

namespace Lomont.AdventOfCode._2023
{
    internal class Day22 : AdventOfCode
    {
        /*
2023 Day 22 part 1: 512 in 813962 us
2023 Day 22 part 2: 98167 in 10068920.7 us

      --------Part 1---------   --------Part 2--------
Day       Time    Rank  Score       Time   Rank  Score
 22   00:42:00     760      0   00:55:25    690      0
 21   00:11:18     949      0   01:19:54    107      0

First hundred users to get both stars on Day 22:

  1) Dec 22  00:12:50  etotheipi1
  2) Dec 22  00:12:55  nthistle (AoC++) (Sponsor)
  3) Dec 22  00:13:18  NotEvenJoking (AoC++)
  4) Dec 22  00:13:56  mrphlip (AoC++)
  5) Dec 22  00:15:29  Verulean
  6) Dec 22  00:16:35  Nathan Fenner (AoC++)
  7) Dec 22  00:16:38  scanhex

 95) Dec 22  00:29:22  Oliver Ni (AoC++)
 96) Dec 22  00:29:28  Vladyslav Burakov
 97) Dec 22  00:29:32  Zack Lee
 98) Dec 22  00:29:40  Sneed's Feeduck and Seeduck
 99) Dec 22  00:29:44  Christian Cederquist (AoC++)
100) Dec 22  00:29:48  Tomas Rokicki
First hundred users to get the first star on Day 22:

  1) Dec 22  00:10:04  mrphlip (AoC++)
  2) Dec 22  00:10:15  Nicky Skybytskyi
  3) Dec 22  00:10:23  Noble Mushtak
  4) Dec 22  00:10:31  nthistle (AoC++) (Sponsor)
  5) Dec 22  00:10:42  scanhex
  6) Dec 22  00:10:43  5space (AoC++)

 96) Dec 22  00:20:36  Vladyslav Burakov
 97) Dec 22  00:20:37  Adavya Goyal
 98) Dec 22  00:20:46  Tristan Sharma
 99) Dec 22  00:20:52  Sobhan Mohammadpour
100) Dec 22  00:21:00  ulrikdem
         */

        record cuboid(vec3 v1, vec3 v2, List<vec3> blocks, List<vec3> down);

        public override object Run(bool part2)
        {
            var blocks = new List<cuboid>();
            foreach (var line in ReadLines())
            {
                var w = line.Split('~');
                var n1 = Numbers(w[0]);
                var n2 = Numbers(w[1]);
                var v1 = new vec3(n1[0], n1[1], n1[2]);
                var v2 = new vec3(n2[0], n2[1], n2[2]);
                var (bk1, vertical) = Blocks(v1, v2);
                var bottomz = v1.z < v2.z ? v1 : v2;
                var bk2 = new List<vec3>();
                if (vertical)
                    bk2.Add(bottomz);
                else
                    bk2.AddRange(bk1);
                Assert(bk1.Contains(v1));
                Assert(bk1.Contains(v2));
                blocks.Add(new cuboid(v1, v2, bk1, bk2));
            }

            // get set of blocks, and if the stack is vertical (then footprint a single block)
            (List<vec3> blocks, bool vertical) Blocks(vec3 v1, vec3 v2)
            {
                List<vec3> b = new();
                var del = v2 - v1;
                var len = del.AbsMax;
                var div = len == 0 ? 1 : len;
                del = new vec3(del.x / div, del.y / div, del.z / div);
                for (int i = 0; i <= len; i++)
                    b.Add(v1+i*del);
                Assert(b.Contains(v1));
                Assert(b.Contains(v2));
                return (b, del.z != 0);
            }

            // gives minor speed factor
            blocks.Sort((a, b) => a.down[0].z.CompareTo(b.down[0].z));

            DropAll(blocks);

            static int DropAll(List<cuboid> blocks, int ignoreMe = -1, bool computeAll = true)
            {
                var down = new vec3(0, 0, -1);

                HashSet<vec3> filled = new();
                for (int i =0 ; i < blocks.Count; ++i)
                {
                    if (i == ignoreMe)
                        continue;
                    var block = blocks[i];
                    foreach (var b in block.blocks)
                    {
                        Assert(!filled.Contains(b));
                        filled.Add(b);
                    }
                }

                HashSet<int> whoFell = new();
                bool falling = true;
                do
                {
                    falling = false;
                    for (var i = 0; i < blocks.Count; ++i)
                    {
                        if (i == ignoreMe)
                            continue;
                        var b = blocks[i];
                        if (CanFall(b))
                        {
                            whoFell.Add(i);
                            // Console.WriteLine($"Drop {(char)('A'+i)}");
                            Drop1(ref b);
                            blocks[i] = b;
                            falling = true;
                            if (!computeAll)
                            {
                                return 1; // this many fell so far
                            }
                        }
                    }

                //    Console.WriteLine("Drop");
                } while (falling);

                return whoFell.Count;


                bool CanFall(cuboid block) => block.down.All(b => b.z > 1 && !filled.Contains(b + down));

                void Drop1(ref cuboid block)
                {
                    foreach (var b in block.blocks)
                        filled.Remove(b);

                    for (int i = 0; i < block.blocks.Count; i++)
                        block.blocks[i] += down;
                    for (int i = 0; i < block.down.Count; i++)
                        block.down[i] += down;

                    foreach (var b in block.blocks)
                        filled.Add(b);

                    block = block with { v1 = block.v1 + down, v2 = block.v2 + down };
                }

            }
            

            List<vec3> Clone(List<vec3> v) => v.Select(a => new vec3(a.x, a.y, a.z)).ToList();

            var ans1 = 0;
            var ans2 = 0;
            for (int i = 0; i < blocks.Count; i++)
            {
                //Console.WriteLine($"{i}/{blocks.Count}");
                var b2 = new List<cuboid>();
                foreach (var b in blocks)
                {
                    b2.Add(new cuboid(b.v1, b.v2, Clone(b.blocks),Clone(b.down))); // clone
                }

                var cnt = DropAll(b2, i, part2);
                if (cnt == 0) ans1++;
                ans2 += cnt;
            }


            if (!part2)
                return ans1;
            return ans2;

        }
   }
}