

namespace Lomont.AdventOfCode._2018
{
    internal class Day08 : AdventOfCode
    {
        /*
         *
         * 2018 Day 8 part 1: 46578 in 14607.2 us
         * 2018 Day 8 part 2: 31251 in 17479.9 us
         */
        object Run2()
        {
            var nums = Numbers64(ReadText());
            int index = 0;
            var root = new Node(nums, ref index);

            return Add(root);

            long Add(Node n)
            {
                if (!n.child.Any())
                {
                    return n.md.Sum();
                }

                long s = 0;
                foreach (var m in n.md)
                {
                    if (m > 0 && m - 1 < n.child.Count)
                        s += Add(n.child[(int)(m-1)]);
                }

                return s;
            }
        }

        class Node
        {
            public Node(List<long> nums, ref int index)
            {
                var h = nums[index++];
                var m = nums[index++];
                for (int i = 0; i < h; ++i)
                    child.Add(new Node(nums,ref index));
                for (int i = 0; i < m; ++i)
                    md.Add(nums[index++]);
            }
            public List<Node> child = new();
            public List<long> md = new();
        }

        object Run1()
        {
                var nums = Numbers64(ReadText());
                int index = 0;
                var root = new Node(nums,ref index);

                return Add(root);
                
                long Add(Node n)
                {
                    var m = n.md.Sum();
                    var t = n.child.Sum(Add);
                    return m + t;
                }


        }

        public override object Run(bool part2)
        {
            return part2 ? Run2() : Run1();
        }
    }
}