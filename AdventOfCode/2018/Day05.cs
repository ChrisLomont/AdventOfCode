

namespace Lomont.AdventOfCode._2018
{
    internal class Day05 : AdventOfCode
    {
    

        public override object Run(bool part2)
        {
            var line = ReadText();
            if (!part2) return Score(line);

            int min = Int32.MaxValue;
            for (int i = 0; i < 26; ++i)
            {

                var t = line;
                t=t.Replace(((char)(i+'a')).ToString(),"");
                t=t.Replace(((char)(i + 'A')).ToString(), "");
                var sc = Score(t);
                
                Console.WriteLine($"{i}->{sc}");

                min = Math.Min(min,sc);
            }

            return min;


        }

        int Score(string line)
        { while (true)
            {
                var ch = false;
                for (int i = 0; i < line.Length - 1; ++i)
                {
                    var c1 = line[i];
                    var c2 = line[i + 1];
                    if (Char.ToLower(c1) == Char.ToLower(c2) && c1 != c2)
                    {
                        //Console.WriteLine(line);
                        line = line[0..i] + line[(i+2)..];
                        //Console.WriteLine(line);
                        i -= 2;
                        if (i < 0) i = 0;
                        ch = true;
                    }
                }

                if (!ch) break;
            }

            return line.Length;


        }
    }
}