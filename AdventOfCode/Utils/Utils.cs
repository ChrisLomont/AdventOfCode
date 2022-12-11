using System.Net.NetworkInformation;

namespace Lomont.AdventOfCode.Utils
{
    internal static class Utils
    {
        /// <summary>
        /// Fill in files for given year
        /// Will not overwrite anything
        /// </summary>
        /// <param name="year"></param>
        public static void MakeYearFiles(int year, string path)
        {
            var yd = year.ToString() + "/";
            Dir(yd);
            for (var day = 1; day <= 25; ++day)
            {
                var code = codeTemplate
                    .Replace("<year>", year.ToString())
                    .Replace("<day>",day.ToString("D2"));
                MakeFile($"{yd}Day{day:D2}.cs", code);
            }

            var data = yd + "Data/";
            Dir(data);
            for (var day = 1; day <= 25; ++day)
                MakeFile($"{data}Day{day:D2}.txt", "");

            void MakeFile(string filename, string text)
            {
                filename = path + filename;
                if (File.Exists(filename))
                    Console.WriteLine($"ERROR: Filename {filename} exists");
                else
                {
                    Console.WriteLine($"   Creating file {filename}");
                    File.WriteAllText(filename, text);
                }
            }
            void Dir(string dirName)
            {
                dirName = path + dirName;
                if (Directory.Exists(dirName))
                    Console.WriteLine($"ERROR: directory {dirName} exists");
                else
                {
                    Console.WriteLine($"Creating dir {dirName}");
                    Directory.CreateDirectory(dirName);
                }
            }




        }


        // get all types for each day
        public static List<Type> GetDayTypes()
        {
            var ans = new List<Type>();
            foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var type in asm.GetTypes())
                {
                    if (type.BaseType == typeof(AdventOfCode))
                        ans.Add(type);
                }
            }

            return ans;
        }

        static string codeTemplate =
"""
namespace Lomont.AdventOfCode._<year>
{
    internal class Day<day> : AdventOfCode
    {
        public override object Run(bool part2)
        {
            throw new NotImplementedException("Year <year>, day <day> not implemented");
            return 0;
        }
    }
}
""";

    }
}
