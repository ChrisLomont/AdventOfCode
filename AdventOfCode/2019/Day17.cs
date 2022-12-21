namespace Lomont.AdventOfCode._2019
{
    internal class Day17 : AdventOfCode
    {
        public override object Run(bool part2)
        {
            var input = new Queue<long>();
            var output = new Queue<long>();
            var prog = Numbers64(ReadLines()[0]);
            var comp = new Day02.IntCode(prog, input.Dequeue, output.Enqueue);

            while (!comp.Step())
            {

            }

            var d = output.ToList();
            var w = d.IndexOf(10); // char return
            var h = d.Count / (w + 1);
            var g = new char[w, h];
            Apply(g, (i, j, v) =>
            {
                return (char)d[i+j*(w+1)];
            });
            Dump(g,noComma:true);

            var sum = 0;
            vec3 start = new();
            char sc = ' ';
            Apply(g, (i, j, v) =>
            {
                var vec = new vec3(i,j);
                var d0 = Get(g, i, j, ' ') == '#';
                var d1 = Get(g, i+1, j, ' ') == '#';
                var d2 = Get(g, i-1, j, ' ') == '#';
                var d3 = Get(g, i, j+1, ' ') == '#';
                var d4 = Get(g, i, j-1, ' ') == '#';
                if ("".Contains(v))
                {
                    start = new vec3(i,j);
                    sc = v;
                }

                if (d0&&d1 && d2 && d3 && d4)
                {
                    sum += i * j;
                    return v;
                    //return 'O';
                }
                else
                    return v;
            });
            //Dump(g,true);
            if (!part2)
                return sum;

            input.Clear();
            output.Clear();
            prog[0] = 2;
            comp = new Day02.IntCode(prog, input.Dequeue, output.Enqueue);

            // get moves - go straight as much as possible
            // then get large sub patterns, see if fits
            
            // todo;




            return -1234;

        }
    }
}