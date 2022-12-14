
namespace Lomont.AdventOfCode._2020
{
    internal class Day23 : AdventOfCode
    {
        // 36542897

#if false
        class Tbl
        {
            public Tbl(List<int> start)
            {

            }

            public int Min()
            {

            }

            public int Max()
            {

            }

            public int Add(int n)
            {

            }

            public int Count =>;

            public int this[int index]
            {
                todo;
            }

            public int IndexOf(int val)
            {

            }

            public void Remove(int firstIndex, int length)
            {
                todo;
            }

        }

#endif
        public override object Run(bool part2)
        {
            var order = ReadLines()[0];
            // clockwise

            //var arr = new Tbl(order.ToCharArray().Select(c => c - '0').ToList()));
            var arr = order.ToCharArray().Select(c => c - '0').ToList();

            part2 = false;
            var hi = arr.Max();
            var lo = arr.Min();

            if (part2)
            {
                var v = hi + 1;
                while (arr.Count < 1_000_000)
                    arr.Add(v++);
            }

            var len = arr.Count;

            var current = arr[0];// first cup is current

            var moves = part2 ? 10_000_000 : 100;
            for (var i = 0; i < moves; ++i)
            {
                if ((i%20000) == 0)
                    Console.WriteLine($"{i+1}/{moves}");
                var ind= arr.IndexOf(current);

                // take 3
                var c1 = arr[(ind + 1) % len];
                var c2 = arr[(ind + 2) % len];
                var c3 = arr[(ind + 3) % len];

                var dest = current - 1;
                if (dest < lo) dest = hi;
                while (dest ==c1 || dest==c2 || dest == c3)
                {
                    dest--;
                    if (dest < lo) dest = hi;
                }

#if true
                // remove 3 at ind+1, insert them at di
                var di = (arr.IndexOf(dest)+len-2)%len;
                var k = (ind + 1) % len;
                while (k != di)
                {
                    arr[k] = arr[(k + 3)%len];
                    k = (k + 1) % len;
                }

                arr[k] = c1;
                arr[(k+1)%len] = c2;
                arr[(k+2)%len] = c3;
#else

                // remove
                arr[(ind + 1) % len] = -1;
                arr[(ind + 2) % len] = -1;
                arr[(ind + 3) % len] = -1;
                arr.RemoveAll(c => c == -1);
                len -= 3;

                // insert
                var di = (arr.IndexOf(dest)+1)%len;
                arr.Insert(di++ % len,c1);
                len++;
                arr.Insert(di++ % len, c2);
                len++;
                arr.Insert(di++ % len, c3);
                len++;
#endif

                ind = arr.IndexOf(current);
                current = arr[(ind + 1) % len];

                //Console.WriteLine($"Cur {current}");
                //Dump(arr);
                //Console.WriteLine();
            }

            if (part2)
            {
                var ind2 = arr.IndexOf(1);
                long v1 = arr[(ind2+1)%len];
                long v2 = arr[(ind2 + 2) % len];
                return v1 * v2;
            }
            else
            {
                var ind2 = arr.IndexOf(1);
                var s = "";
                for (var i = 0; i < len; ++i)
                {
                    s += arr[ind2];
                    ind2 = (ind2 + 1) % len;
                }

                return s[1..];
            }
        }
    }
}