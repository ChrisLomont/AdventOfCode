namespace Lomont.AdventOfCode._2020
{
    internal class Day25 : AdventOfCode
    {

        public override object Run(bool part2)
        {
            // map val to power
            var inv = new Dictionary<long, long>();

            long prime = 20201227;
            var tbl7 = new List<long>();
            // make tbl:
            tbl7.Add(1); // 7^i mod p
            for (var i = 1; i <= prime; ++i)
            {
                var v = (tbl7[i - 1] * 7) % prime;
                tbl7.Add(v);
                if (!inv.ContainsKey(v))
                    inv.Add(v,i);
            }

            long subjectNumber = 0;

            var cardPublic = 5764801;
            var doorPublic = 17807724;
            (cardPublic, doorPublic) = (3248366, 4738476);

            //Console.WriteLine($"TT {Transform(8,7)} == 5764801");

            var doorSecret = Crack(doorPublic);
            //Console.WriteLine("Door secret "+ doorSecret);
            var cardSecret = Crack(cardPublic);
            //Console.WriteLine("Card secret " + cardSecret);

            var ans = Transform(cardSecret, doorPublic);
            //Console.WriteLine("CC "+ans);

            return ans; // 18293391


            int Crack(int publicVal)
            {
                return (int)inv[publicVal % prime];
                int secret = 1;
                while (true)
                {
                    var chk = Transform(secret, 7);
                    if (chk == publicVal) break;
                    ++secret;
                }
                return secret;
            }



            int Transform(int loopSize, int subjectNumber)
            {
                long v = 1;
                for (var i = 0; i < loopSize; ++i)
                {
                    v = (v * subjectNumber) % prime;
                }
                return (int)v;
            }

            return -1;

        }
    }
}