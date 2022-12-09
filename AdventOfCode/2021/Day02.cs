namespace Lomont.AdventOfCode._2021
{
    internal class Day02 : AdventOfCode
    {
        
        public override object Run(bool part2)
        {
            int h = 0, d = 0, aim = 0;
            foreach (var line in ReadLines())
            {
                if (!part2)
                {
                    if (line.Contains("forward"))
                        h += GetNumbers(line,false)[0];
                    if (line.Contains("down"))
                        d += GetNumbers(line, false)[0];
                    if (line.Contains("up"))
                        d -= GetNumbers(line, false)[0];
                }
                else
                {
                    if (line.Contains("forward"))
                    {
                        var v = GetNumbers(line, false)[0];
                        h += v;
                        d += aim*v;
                    }

                    if (line.Contains("down"))
                        aim += GetNumbers(line, false)[0];
                    if (line.Contains("up"))
                        aim -= GetNumbers(line, false)[0];
                }
            }
            return h*d;
        }
    }
}