namespace Lomont.AdventOfCode.Utils
{
    internal static class Utils
    {
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

    }
}
