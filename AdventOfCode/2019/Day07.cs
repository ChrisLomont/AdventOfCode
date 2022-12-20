namespace Lomont.AdventOfCode._2019
{
    internal class Day07 : AdventOfCode
    {
    //2019 Day 7 part 1: 398674 in 13251.4 us
    //2019 Day 7 part 2: 39431233 in 30600.5 us
        public override object Run(bool part2)
        {
            checked
            {

                var prog = Numbers64(ReadLines()[0]);

                var score = 0L;
                if (!part2)
                {
                    for (var a = 0; a <= 4; ++a)
                    for (var b = 0; b <= 4; ++b)
                    for (var c = 0; c <= 4; ++c)
                    for (var d = 0; d <= 4; ++d)
                    for (var e = 0; e <= 4; ++e)
                    {
                        if (a == b || a == c || a == d || a == e) continue;
                        if (b == c || b == d || b == e) continue;
                        if (c == d || c == e) continue;
                        if (d == e) continue;

                        var a1 = RunOne(0, a);
                        var b1 = RunOne(a1, b);
                        var c1 = RunOne(b1, c);
                        var d1 = RunOne(c1, d);
                        var e1 = RunOne(d1, e);
                        score = Math.Max(score, e1);
                    }

                    return score; // 315076920 too high



                    int RunOne(int input, int phase)
                    {
                        long ans = 0;
                        var inp = new List<int> { phase, input };
                        int inpIndex = 0;
                        var a = new Day02.IntCode(prog, () => inp[inpIndex++], v => ans = v);
                        while (!a.Step())
                        {
                        }

                        return (int)ans;
                    }
                }


                for (var a = 5; a <= 9; ++a)
                for (var b = 5; b <= 9; ++b)
                for (var c = 5; c <= 9; ++c)
                for (var d = 5; d <= 9; ++d)
                for (var e = 5; e <= 9; ++e)
                {
                    if (a == b || a == c || a == d || a == e) continue;
                    if (b == c || b == d || b == e) continue;
                    if (c == d || c == e) continue;
                    if (d == e) continue;

                    //(a,b,c,d,e) = (9, 7, 8, 5, 6);

                    var input = new Queue<long>();
                    var output = new Queue<long>();

                    // 5 computers

                    var ac = new Day02.IntCode(prog, input.Dequeue, output.Enqueue);
                    var bc = new Day02.IntCode(prog, input.Dequeue, output.Enqueue);
                    var cc = new Day02.IntCode(prog, input.Dequeue, output.Enqueue);
                    var dc = new Day02.IntCode(prog, input.Dequeue, output.Enqueue);
                    var ec = new Day02.IntCode(prog, input.Dequeue, output.Enqueue);
                    bool done = false;
                    long val = 0, eval = 0;

                    int pass = 0;
                    while (!done)
                    {
                        if (pass == 0)
                            input.Enqueue(a); // phase
                        input.Enqueue(val); // input
                        (done, val) = RunTillOutput(ac);

                        if (pass == 0)
                            input.Enqueue(b); // phase
                        input.Enqueue(val); // input
                        (done, val) = RunTillOutput(bc);

                        if (pass == 0)
                            input.Enqueue(c); // phase
                        input.Enqueue(val); // input
                        (done, val) = RunTillOutput(cc);

                        if (pass == 0)
                            input.Enqueue(d); // phase
                        input.Enqueue(val); // input
                        (done, val) = RunTillOutput(dc);

                        if (pass == 0)
                            input.Enqueue(e); // phase
                        input.Enqueue(val); // input
                        (done, val) = RunTillOutput(ec);

                        if (!done) eval = val;

                        //Console.WriteLine($"Pass {pass} val {eval}");
                        ++pass;
                    }

                    score = Math.Max(score, eval);

                    (bool done, long val) RunTillOutput(Day02.IntCode comp)
                    {
                        bool done = false;
                        while (!done && !output.Any())
                        {
                            done = comp.Step();
                        }

                        long val = 0;
                        if (output.Any())
                            val = output.Dequeue();
                        return (done, val);
                    }
                }



                return score; // 315076920 too high
            }

        }
    }
}