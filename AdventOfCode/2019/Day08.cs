namespace Lomont.AdventOfCode._2019
{
    internal class Day08 : AdventOfCode
    {
        public override object Run(bool part2)
        {
            var (w, h) = (25, 6);
            var (i, j, L) = (0, 0, 0);
            List<char[,]> layers = new();
            char[,] layer = new char[w, h];
            foreach (var c in ReadLines()[0])
            {
                if (i == 0 && j == 0)
                {
                    //Dump(layer,true);
                    layer = new char[w, h];
                    layers.Add(layer);
                }
                layer[i, j] = c;
                i++;
                if (i >= w)
                {
                    i = 0;
                    j++;
                    if (j >= h)
                    {
                        j = 0;
                        L++;
                    }
                }
            }


            //Console.WriteLine($"{Count(layers[0],'0')}");

            var min0Count = layers.Min(b=>Count(b,'0'));
            var min0Layer = layers.First(b => Count(b, '0') == min0Count);

            var c1 = Count(min0Layer,'1');
            var c2 = Count(min0Layer, '2');

            if (!part2)

                return c1 * c2;

            var final = new char[w, h];
            Apply(final, (i, j, _) =>
            {
                for (var d = 0; d < layers.Count; ++d)
                {
                    if (layers[d][i, j] == '0')
                        return '.';
                    if (layers[d][i,j] == '1')
                        return 'O';
                }

                return '_';

            });
            Dump(final,true);
            return "HFYAK"; // read from Console




        }

        int Count(char[,] b, char ch)
        {
            var count = 0;
            Apply(b, (i, j, v) =>
            {
                if (v == ch) 
                    ++count;
                return v;
            });
            return count;
        }

    }
}