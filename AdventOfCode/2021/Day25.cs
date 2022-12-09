namespace Lomont.AdventOfCode._2021
{
    internal class Day25 : AdventOfCode
    {
        // 389
        public override object Run(bool part2)
        {
            var (w, h, g) = CharGrid();

            var g2 = new char[w,h];
            int step = 0;
            while (true)
            {
                bool b1, b2;
                ++step;
                //Dump(g);
                //Console.WriteLine();
                (g, g2, b1) = Right(g, g2);
                (g, g2, b2) = Down(g, g2);
                //Dump(g);
                if (!b1 && !b2)
                break;
            }

            return step;

            (int,int) Wrap(int i, int j)
            {
                return (i % w, j % h);
            }


            (char[,], char[,], bool) Right(char[,] g, char[,] g2)
            {
                bool moved = false;
                Apply(g2, (i, j, v) => '.'); // erase
                Apply(g, (i, j, v) =>
                {
                    if (v == '>')
                    {
                        // set into g2
                        var (i2, j2) = Wrap(i+1, j);

                        if (g[i2, j2] == '.')
                        {
                            g2[i2, j2] = v;
                            moved = true;
                        }
                        else
                            g2[i, j] = v;
                    }

                    if (v == 'v') g2[i,j] = v;
                    return v;
                });
                return (g2, g, moved); // swap // g is current
            }
            (char[,], char[,], bool) Down(char[,] g, char[,] g2)
            {
                var moved = false;
                Apply(g2, (i, j, v) => '.'); // erase
                Apply(g, (i, j, v) =>
                {
                    if (v == 'v')
                    {
                        // set into g2
                        var (i2, j2) = Wrap(i,j+1);
                        if (g[i2, j2] == '.')
                        {
                            moved = true;
                            g2[i2, j2] = v;
                        
                        }
                        else
                            g2[i, j] = v;
                    }
                    if (v == '>') g2[i, j] = v;
                    return v;
                });
                return (g2, g, moved); // swap // g is current
            }

        }
    }
}