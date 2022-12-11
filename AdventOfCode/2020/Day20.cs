namespace Lomont.AdventOfCode._2020
{
    internal class Day20 : AdventOfCode
    {
        class Tile
        {
            // orient:
            public int id;
            public char[,] grid;
            public List<int> hashes; // edge hashes
            public bool placed;
        }

        enum Dir
        {
            N,
            S,
            E,
            W
        };

        // 2020 Day 17 part 1: 319 in 524265.3 us
        // 2020 Day 17 part 2: 2324 in 23238779 us
        public override object Run(bool part2)
        {
            int index = 0;
            var lines = ReadLines();
            List<Tile> tiles = new();
            while (index < lines.Count)
            {
                if (lines[index].Contains("Tile"))
                {
                    var tile = Numbers(lines[index])[0];
                    var s = lines[index + 1].Length;
                    var (w, h, g) = CharGrid(lines.Skip(index+1).Take(s).ToList());
                    tiles.Add(new Tile{id = tile, grid = g, hashes = Hashes(g).ToList() });
                    index += s + 1;
                }
                else if (String.IsNullOrEmpty(lines[index]))
                    ++index;
                else
                    throw new ArgumentException();
            }

            // idea - create edge hashes (binary 0/1) for each orientation, find unique tiles, expand
            Dictionary<int, List<Tile>> edgeCounts = new();
            foreach (var tile in tiles)
            {
                foreach (var hash in Hashes(tile.grid))
                {
                    if (!edgeCounts.ContainsKey(hash))
                        edgeCounts.Add(hash, new());
                    edgeCounts[hash].Add(tile);
                }
            }

            // split tiles by # edge matches

            // get tile ids with 2 unique edges
            var cornerTiles = tiles
                .Where(
                    t => Hashes(t.grid).Count(hash => edgeCounts[hash].Count == 1) == 4
                ).ToList();
            var edgeTiles = tiles
                .Where(
                    t => Hashes(t.grid).Count(hash => edgeCounts[hash].Count == 1)==2
                    ).ToList();
            var interiorTiles = tiles
                .Where(
                    t => Hashes(t.grid).All(hash => edgeCounts[hash].Count == 2)
                ).ToList();

            // 40 edges, 4 corners, 100 interior
            Console.WriteLine($"{edgeTiles.Count} edges, {cornerTiles.Count} corner,  {interiorTiles.Count} interior");

            //Dump(cor2);
            //return -2;
            if (!part2)
            {
                // 64802175715999
                return cornerTiles.Select(t => (ulong)t.id).Aggregate(1UL, (a, b) => a * b);
            }

            // must build image:
            var sideLen = (int)Math.Sqrt(tiles.Count);
            var (w1,h1) = Size(tiles[0].grid);
            Tile[,] tileImg = new Tile[sideLen, sideLen];
            
            char[,] charImg= new char[sideLen * (w1-1)+1+10, sideLen * (h1-1)+1+10];

            for (var j = 0; j < sideLen; ++j)
            for (var i = 0; i < sideLen; ++i)
            {
                    if (i == 0 && j == 0)
                {
                    // place 0,0 corner
                    while (true)
                    {
                        var c1 = edgeCounts[Hash(cornerTiles[0], Dir.W)].Count;
                        var c2 = edgeCounts[Hash(cornerTiles[0], Dir.N)].Count;
                        if (c1 == 1 && c2 == 1) 
                            break;
                        cornerTiles[0].grid = Rotate90(cornerTiles[0].grid);
                    }

                    PlaceTile(cornerTiles[0],0,0);
                }
                    else if (i == sideLen-1 && j == 0)
                    {
                        // match to left from corners
                        var hash = Hash(tileImg[i - 1, j], Dir.E);
                        var pair = Find(cornerTiles, hash, Dir.W);
                        PlaceTile(pair, i, j);

                    }
                    else if ((i==0 && j == sideLen-1) || (i == sideLen-1 && j == sideLen-1))
                {
                    // match to above from corners
                    var hash = Hash(tileImg[i, j - 1], Dir.S);
                    var pair = Find(cornerTiles, hash, Dir.N);
                    PlaceTile(pair, i, j);
                }
                else if (i == 0)
                { // match to above from edges
                    var hash = Hash(tileImg[i, j - 1], Dir.S);
                    var pair = Find(edgeTiles, hash, Dir.N);
                    PlaceTile(pair, i, j);
                }
                else if (j == 0 || i == sideLen-1 || j == sideLen-1)
                {
                    // match to left from edges
                    var hash = Hash(tileImg[i - 1, j], Dir.E);
                    var pair = Find(edgeTiles, hash, Dir.W);
                    PlaceTile(pair, i, j);
                }
                else
                {
                    // match to left from interior
                    var hash = Hash(tileImg[i - 1, j], Dir.E);
                    var pair = Find(interiorTiles, hash, Dir.W);
                    PlaceTile(pair, i, j);
                }
            }

            Apply(tileImg, (i, j, v) =>
            {
                Console.Write(v.id+ " ");
                return v;
            });
            Console.WriteLine();
        

            //charImg = Rotate90(charImg);
            //charImg = Rotate90(charImg);
            //charImg = Rotate90(charImg);
            Dump(charImg,noComma:true);
            return -1;

            // now do same on large image, match pattern

            var pattern = CharGrid(
                new List<string>
                {
                    //"# #",
                    //" # ",
                    //"# #",
                    "                  # ",
                    "#    ##    ##    ###",
                    " #  #  #  #  #  #   ",
                });

            long count = 0;
            for (var r = 0; r < 8; ++r)
            {
                count += Matches(charImg);
                charImg = Rotate90(charImg);
                if (r == 3)
                    charImg = Flip(charImg);
            }

            return count;

/*




 */

            long Matches(char[,] g)
            {
                var (pw, ph,pg) = pattern;
                var count = 0;
                Apply(g, (i, j, v) =>
                {
                    var ok = true;
                    Apply(pg, (i1, j1, v1) =>
                        {
                            var gset = Get(g, i + i1, j + j1, ' ') == '#';
                            var pset = Get(pg, i1, j1, ' ') == '#';
                            if (pset && !gset) 
                                ok = false;
                            return v1;
                        }
                        );
                    if (ok) 
                        count++;
                    return v;
                });
                return count;

            }

            void PlaceTile(Tile t, int i, int j)
            {
                Console.WriteLine($"Placing {i},{j}: N{Hash(t,Dir.N)} S{Hash(t, Dir.S)} E{Hash(t, Dir.E)} W{Hash(t, Dir.W)} ");

                t.placed = true;
                Trace.Assert(tileImg[i,j] == null);
                tileImg[i, j] = t;

                // copy in chars
                Apply(t.grid, (i1, j1, v) =>
                {
                    charImg[i*(w1+50)+i1,j*(h1+2)+j1] = v;
                    return v;
                });

            }

            Tile Find(List<Tile> tiles, int hash, Dir dir)
            {
                HashSet<int> hashes = new();
                foreach (var t in tiles)
                {
                    hashes.Clear();
                    if (t.placed == true) continue;
                    var t1 = Hash(t, Dir.N);
                    for (var r = 0; r < 8; ++r)
                    {
                        var h = Hash(t, dir);
                        hashes.Add(h);
                        if (h == hash)
                            return t;
                        t.grid = Rotate90(t.grid);
                        if (r == 3)
                            t.grid = Flip(t.grid);
                    }
                    t.grid = Flip(t.grid); // restore
                    var t2 = Hash(t, Dir.N);
                    Trace.Assert(t1 == t2);
                    if (hashes.Count != 8)
                    {
                        Console.WriteLine("bad hashing");
                        Dump(t.grid);
                        Console.WriteLine("Saw");
                        Dump(t.hashes);
                        Console.WriteLine("hashed");
                        Dump(hashes);
                        Trace.Assert(hashes.Count == 8);
                    }
                }




                throw new Exception(); // should find one
            }

            char[,] Flip(char[,] g)
            {
                var (w, h) = Size(g);
                var g2 = new char[w, h];
                Apply(g, (i, j, v) =>
                {
                    g2[w-i-1,j] = v;
                    return v;
                });
                return g2;
            }


            char[,] Rotate90(char[,] g)
            {// Note: can do in place: transpose (swap i,j with ji)
                // then reverse each row (or column)
                var (w, h) = Size(g);
                var g2 = new char[w, h];
                Apply(g, (i, j, v) =>
                    {
                        g2[w-1-j,i] = v;
                        return v;
                    }
                );
                return g2;
            }

            int Hash(Tile tile, Dir dir)
            {
                var g = tile.grid;
                var (w,h) = Size(g);
                // hash to match:

                // E and W go top to bottom
                // N S go left to right
                return dir switch
                {
                    Dir.E => HashDir(g,0,0,0,1),
                    Dir.W => HashDir(g,w-1, 0, 0, 1),
                    Dir.N => HashDir(g, 0, 0, 1, 0),
                    Dir.S => HashDir(g, 0, h-1, 1, 0),
                    _ => throw new Exception()
                };
            }



            IEnumerable<int> Hashes(char[,] g)
            {
                var (w, h) = (g.GetLength(0), g.GetLength(1));

                yield return HashDir(g,0, 0, 1, 0);
                yield return HashDir(g,w - 1, 0, 0, 1);
                yield return HashDir(g,w - 1, h-1, -1, 0);
                yield return HashDir(g,0, h - 1, 0, -1);

                yield return HashDir(g, 0, 0, 0, 1);
                yield return HashDir(g, 0, h - 1, 1,0);
                yield return HashDir(g, w - 1, h - 1, 0, -1);
                yield return HashDir(g, w - 1, 0, -1, 0);
            }
            int HashDir(char[,] g, int i, int j, int di, int dj)
            {
                int val = 0;
                for (var k = 0; k < g.GetLength(0); ++k)
                {
                    val = 2 * val + (g[i, j] == '#' ? 1 : 0);
                    i += di;
                    j += dj;
                }
                return val;
            }



            return -1;
        }
    }
}

