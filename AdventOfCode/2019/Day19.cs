
namespace Lomont.AdventOfCode._2019
{
    internal class Day19 : AdventOfCode
    {
        // 2019 Day 19 part 1: 158 in 295326.8 us
        // 2019 Day 19 part 2: 6191165 in 3074801.4 us
        public override object Run(bool part2)
        {

            var prog = Numbers64(ReadLines()[0]);

            if (!part2)
            {
                var sum = 0L;
                for (var i = 0; i < 50; ++i)
                for (var j = 0; j < 50; ++j)
                {
                    sum += Sample(i, j);
                }

                return sum;
            }

            var sz = 100-1; // need this wide

            HashSet<vec3> beam = new();

            Dictionary<long,(vec3?,vec3?)> ray = new();


            for (var row = 10; row < 50; ++row)
            {
                var (left, right) = Test(row,0,50);
                if (left != null && right != null)
                {
                    //Console.WriteLine($"Row ok {row} {left} {right}");
                    ray.Add(row,(left,right));
                }
            }

            // sort of follow edge down

            // slopes
            var (sl, sr) = ray[49];
            var (dx1, dy1, _) = sl;
            var (dx2, dy2, _) = sr;

            var mrow = 100;
            int hrow=0, lrow=0;

            var stage = 0;

            bool Good(vec3?l1, vec3? r1)
            {

                if (l1 == null || r1 == null) return false;
                if (Sample(l1.x, l1.y) != 1 || Sample(l1.x - 1, l1.y) != 0) return false;
                if (Sample(r1.x, r1.y) != 1 || Sample(r1.x + 1, r1.y) != 0) return false;
                return true;
            }

            bool HasBox(vec3 pt)
            {
                var (x, y, _) = pt;
                return
                    Sample(x, y) == 1 &&
                    Sample(x + sz, y) == 1 &&
                    Sample(x, y - sz) == 1 &&
                    Sample(x + sz, y - sz) == 1
                    ;
            }

            (bool hasBox, vec3 upperLeft) CheckRow(int row, int c1, int c2)
            {
                var (l1, r1) = Test(mrow, c1, c2);
                Trace.Assert(Good(l1, r1));
                var has = HasBox(l1);

                return (has, l1+new vec3(0,-sz,0));
            }


            while (true)
            {

                if (stage == 0)
                {
                    mrow *= 2;
                    //Console.WriteLine($"row {mrow}");
                    var start = mrow * (dx1 - 5) / dy1;
                    var end = mrow * (dx2 + 5) / dy2;

                    var (hasBox, vec) = CheckRow(mrow,start,end);


                    if (hasBox)
                    {
                        stage = 1;
                        lrow = mrow / 2; // search
                        hrow = mrow;
                    }
                }
                else
                {
                    mrow = (hrow+lrow)/2;
                    var start = mrow * (dx1 - 5) / dy1;
                    var end = mrow * (dx2 + 5) / dy2;

                    var (hasBox, vec) = CheckRow(mrow, start, end);

                    //Console.WriteLine($"bsrch {lrow} {mrow} {hrow}");

                    if (lrow + 1 >= hrow)
                    {
                        // back up 100, search: other ideas were failing - solution likely somewhat jiggled
                        mrow -= 100;
                        do
                        {
                            mrow++;
                            start = mrow * (dx1 - 5) / dy1;
                            end = mrow * (dx2 + 5) / dy2;
                            (hasBox, vec) = CheckRow(mrow, start, end);
                        } while (!hasBox);
                      
                        return vec.x * 10_000 + (vec.y);
                    }

                    if (hasBox)
                    {
                        hrow = mrow;
                    }
                    else
                        lrow = mrow;
                }

             
            }

            (vec3? left,vec3?right) Test(int row, int startCol, int endCol)
            {
                while (true)
                {
                    vec3? first = null;
                    vec3? last = null;
                    for (var col = startCol; col < endCol; ++col)
                    {
                        if (Sample(col, row) == 1)
                        {
                            var v = new vec3(col, row);
                            if (first == null)
                                first = v;
                        }
                        else
                        {
                            if (first != null && last == null)
                            {
                                last = new vec3(col, row);
                            }
                        }
                    }

                    if (last != null)
                        last -= vec3.XAxis;
                    return (first, last);
                }
            }


            long Sample(int i, int j)
            {

                var input = new Queue<long>();

                    input.Enqueue(i);
                    input.Enqueue(j);

                var output = new Queue<long>();
                var c = new Day02.IntCode(prog, input.Dequeue, output.Enqueue);
                while (!c.Step())
                {
                }

                return output.Dequeue();

            }
        }
    }
}
