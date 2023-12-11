// Chris Lomont

using System.Linq.Expressions;
using Lomont.AdventOfCode;

// https://github.com/ChrisLomont/AdventOfCode

// create files for a year of stuff
//Utils.MakeYearFiles(2020, AdventOfCode.path);
//Utils.MakeYearFiles(2023, @"<paste in path to where the '2024' dir goes>"); 
//RunDays(1, 25, 2019);

//Memoizer.Test();
//return;

// call RunDays() to get all,
// RunDays(N) to get N on up, 
// RunDays(A,B) to get A through B
// RunDays(A,B,y1,y2) to get A through B on years y1 through y2

//RunDays(1, 25, 2019,2022);

// auto load needs access token, read this
// https://mmhaskell.com/blog/2023/1/30/advent-of-code-fetching-puzzle-input-using-the-api
// put session into text file names session.txt in exe directory

// todo - maybe at end of run, allow some keypress to add more commands, set next test, etc., then save that fo rnext run
// then don't have to change this every time (only if change crashes?)

RunDays(11,11);


void RunDays(int start = 0, int end = -1, int yearStart = -1, int yearEnd = -1)
{
        var bg = Console.BackgroundColor;
        var fg = Console.ForegroundColor;
        var types = Utils.GetDayTypes();
        if (end < 0) end = types.Count;
        if (start > 0) start--;
        end = Math.Min(end, types.Count);

        var thisYear = DateTime.Now.Year;
        if (yearEnd == -1 && yearStart == -1)
            yearStart = yearEnd = thisYear;
        else if (yearEnd == -1)
            yearEnd = yearStart;

        for (var year = yearStart; year <= yearEnd; year++)
        for (var i = start; i < end; ++i)
        {
            var day = i + 1;
            var type = GetDateType(year, day, types);
            if (type == null)
            {
                Console.WriteLine($"Cannot find type for year {year}, day {day}");
                continue;
            }

            var dayType = Activator.CreateInstance(type) as AdventOfCode;
            if (dayType == null) throw new Exception("Null day type!");

            var result1 = Time(dayType, false);
            var result2 = Time(dayType, true);
            Result(year, day, false, result1);
            Result(year, day, true, result2);

            Console.WriteLine();
        }


    void Result(int year, int day, bool part2, (object answer, TimeSpan elapsed) result)
    {
        if (result.answer is NotImplementedException ex)
        {
            result.answer = "EXCEPTION: " + ex.Message;
        }

        var dt = part2 ? "part 2" : "part 1";
        Console.Write($"{year} Day {day} {dt}: ");
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.Write($"{result.answer}");
        Console.ForegroundColor = fg;
        Console.Write(" in ");
        Console.ForegroundColor = ConsoleColor.Green;
        Console.Write(result.elapsed.TotalNanoseconds / 1000);
        Console.ForegroundColor = fg;
        Console.WriteLine(" us");
    }

    (object answer, TimeSpan elapsed) Time(AdventOfCode day, bool part2)
    {
        try
        {
            var sw = new Stopwatch();
            sw.Start();
            var score = day.Run(part2);
            sw.Stop();
            var elapsed = sw.Elapsed;
            return (score, elapsed);
        }
        catch (NotImplementedException ex)
        {
            return (ex, TimeSpan.Zero);
        }
    }
}

//Type or null
Type? GetDateType(int year, int day, IList<Type> types)
    => types.FirstOrDefault(t =>
        t.FullName.Contains("_" + year.ToString()) && 
        t.FullName.Contains($"Day{day:D2}")
    );
