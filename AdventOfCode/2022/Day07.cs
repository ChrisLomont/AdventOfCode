namespace Lomont.AdventOfCode._2022
{
    // 1886043
    // 3842121
    // 7   00:15:48     347      0   00:20:11     357      0

    // 1) Dec 07  00:05:08  betaveros(AoC++)
    // 2) Dec 07  00:07:02  nthistle
    // 3) Dec 07  00:07:05  Antonio Molina
    // 4) Dec 07  00:07:17  jonathanpaulson(AoC++)
    // 5) Dec 07  00:07:27  Jan Verbeek

    internal class Day07 : AdventOfCode
    {
        class FileEntry
        {
            public string name="";
            public long size;

        }
        class Dir
        {
            static Dir empty = new Dir();
            public List<Dir> Children = new();
            public Dir Parent=empty;
            public string Name="";
            public List<FileEntry> Files = new();

            public long Size;

            public void Add(Dir dir)
            {
                Children.Add(dir);
                dir.Parent = this;
            }

            public void AddFile(string name, long size)
            {
                Files.Add(new FileEntry{name = name, size = size});
            }
        }

        public override object Run(bool part2)
        {
            var root = new Dir();
            var curPath = root;

            var lines = ReadLines();
            var i = 0;
            while (i < lines.Count)
            {
                var line = lines[i++];
                if (line.Contains($"$ cd "))
                {
                    var pathText = line.Substring(5);
                    if (pathText == "/")
                        curPath = root;
                    else if (pathText == "..")
                        curPath = curPath!.Parent;
                    else
                    {
                        var t = curPath!.Children.FirstOrDefault(p => p.Name == pathText);
                        if (t == null)
                        {
                            t = new Dir { Name = pathText };
                            curPath.Add(t);
                        }

                        curPath = t;
                    }
                }
                else if (line.Contains("$ ls"))
                {
                    while (i < lines.Count && !lines[i].StartsWith('$'))
                    {
                        if (!lines[i].StartsWith("dir "))
                        {
                            var words = lines[i].Split(' ');
                            curPath.AddFile(words[1], Int64.Parse(words[0]));
                        }

                        ++i;

                    }
                }
            }

            // fill in sizes
            var ss = Sizes(root);
            if (!part2)
                return ss.Sum(s => s <= 100000 ? s : 0);

            // part 2
            long total = 70000000; // size of disk
            long needed = 30000000; // needed to do some work
            long used = root.Size; // total currently used
            long free = total - used; // how much is free
            long toClean = needed - free; // how much we need to clean off

            // find smallest that is >= toClean
            Dir best = root;

            Scan(root);

            return best.Size;

            // recurse scan
            void Scan(Dir n)
            {
                if (n.Size >= toClean && n.Size < best.Size)
                    best = n;
                foreach (var c in n.Children)
                    Scan(c);
            }
        }

        List<long> Sizes(Dir n)
        {
            var s = new List<long>();
            Recurse(n);
            return s;

            long Recurse(Dir n)
            {
                var sz = 0L;
                foreach (var c in n.Children)
                    sz += Recurse(c);
                foreach (var f in n.Files)
                    sz += f.size;
                n.Size = sz;
                s.Add(sz);
                return sz;
            }

        }
    }
}