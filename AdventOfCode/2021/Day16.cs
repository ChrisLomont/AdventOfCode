namespace Lomont.AdventOfCode._2021
{
    // 889
    // 739303923668
    internal class Day16 : AdventOfCode
    {
        int bitIndex = 0;
        string line="";
        ulong Read(int bits)
        {
            ulong v = 0;
            for (var i = 0; i < bits; ++i)
                v = v * 2 + ReadBit();
            return v;
        }

        ulong ReadBit()
        {
            var chIndex = bitIndex / 4;
            var c = line[chIndex];
            var h = char.IsDigit(c) ? (c - '0') : (Char.ToLower(c) - 'a') + 10;
            var b = bitIndex % 4;
            var v = (h >> (3 - b))&1;
            bitIndex++;
            return (ulong)v;
        }

        bool EOF()
        {
            return bitIndex / 4 >= line.Length;
        }

        public override object Run(bool part2)
        {
            // MSB first
            // 3 bit version
            // 3 typeId
            // typeID
            //   4 are literal packed
            //   
            // length type id
            //  0 - next 15 bits are total length of subpackets in this one
            //  1 - next 11 bits numer of subpackets

            var lines = ReadLines();
            line = lines[0];
            bitIndex = 0;
            versionSum = 0;

            var vals = ReadPacket();
            if (!part2)
            return versionSum;
            return vals[0];
        }

        ulong versionSum = 0;
        void Show(string m)
        {
            //Console.Write(m);
        }

        List<long> ReadPacket(int maxBits = -1, int maxPackets = -1)
        {
            int bitStart = bitIndex, packetCount=0;
            var vals = new List<long>();
            
            while (!EOF())
            {
                Show("[");

                var version = Read(3);
                versionSum += version;

                var typeId = Read(3);
                Show($"v{version}t{typeId}");

                

                if (typeId == 4)
                {
                    Show("L");
                    // literal value
                    var start = bitIndex; // result uses multiple of 4
                    ulong v = 0;
                    while (true)
                    {
                        var more = ReadBit() == 1;
                        var v4 = Read(4); // read number
                        v = 16 * v + v4;
                        if (!more) break;
                    }
                    vals.Add((long)v);

                    //while ((bitIndex - start) % 4 != 0)
                    //    ReadBit(); // fill out to 4 multiple
                }
                else
                {
                    Show("O");
                    // operator = get sub packet values
                    var lengthType = ReadBit();
                    List<long> v2 = new();
                    if (lengthType == 0)
                    {
                        var bitLength = Read(15); // length of internal packets
                        v2 = ReadPacket((int)bitLength);
                    }
                    else
                    {
                        var packetCount1 = Read(11); // number of packets
                        v2 = ReadPacket(-1,(int)packetCount1);
                    }

                    if (typeId == 0)
                    {
                        // sum packets...
                        vals.Add(v2.Sum());
                    }
                    else if (typeId == 1)
                    {
                        // product packets...
                        vals.Add(v2.Aggregate(1L,(a,b)=>a*b));
                    }
                    else if (typeId == 2)
                    {
                        // min of packets...
                        vals.Add(v2.Aggregate(long.MaxValue, (a, b) => Math.Min(a ,b)));
                    }
                    else if (typeId == 3)
                    {
                        // max of packets...
                        vals.Add(v2.Aggregate(long.MinValue, (a, b) => Math.Max(a, b)));
                    }
                    else if (typeId == 5)
                    {
                        // >
                        vals.Add(v2[0] > v2[1] ? 1 : 0);
                    }
                    else if (typeId == 6)
                    {
                        // <
                        vals.Add(v2[0] < v2[1] ? 1 : 0);
                    }
                    else if (typeId == 7)
                    {
                        // ==
                        vals.Add(v2[0] == v2[1] ? 1 : 0);
                    }

                }

                packetCount++;

                Show("]");

                if (maxPackets != -1 && packetCount >= maxPackets)
                    return vals;
                var bitsUsed = bitIndex - bitStart;
                if (maxBits != -1 && bitsUsed >= maxBits)
                    return vals;
                if (maxBits == -1 && maxPackets == -1)
                {
                    return vals;
                }

            }

            return vals;
        }
    }
}