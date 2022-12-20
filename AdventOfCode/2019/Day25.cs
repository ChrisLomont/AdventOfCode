
namespace Lomont.AdventOfCode._2019
{

    internal class Day25 : AdventOfCode
    {
        // 805307408

        // bot to play game
        class Bot
        {
            Random rand = new Random(1234); // same game

            Room? lastRoom = null;
            Room? curRoom = null;
            List<Room> rooms = new();

            string lastMove = "";
            HashSet<string> itemsTaken = new();

            int turn = 0;

            string preFinalRoomName = "Security Checkpoint";
            string finalRoomName = "Pressure Sensitive Floor";

            public void DrawMap()
            {
                var fg = Console.ForegroundColor;
                foreach (var room in rooms)
                {
                    Console.WriteLine($"Room: {room.name}");
                    foreach (var dir in room.dirs)
                    {
                        var key = (room.name, dir.Key);
                        var dst = toRoom.ContainsKey(key) ? toRoom[key].name : "?";
                        if (dst == "?") 
                            Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine($"   {dir.Key} -> {dst}");
                        Console.ForegroundColor = fg;
                    }
                }
            }

            // queue of moves, played from the top
            // play take moves first
            Queue<string> takeMoves = new();
            Queue<string> moves = new();

            class Room
            {
                public string name;
                public Dictionary<string, Room?> dirs = new();
                public List<string> items = new();
                public override string ToString() => name;
            }

            #region Mapping

            // learn map
            Dictionary<(string src, string dst), string> map = new();
            Dictionary<(string src, string dir), Room> toRoom = new();

            void Map(Room prev, Room nbr, string dir)
            {
                if ("northsoutheastwest".Contains(dir) && lastMove.Length >= 4)
                {
                    var key = (prev.name, nbr.name);
                    if (!map.ContainsKey(key))
                    {
                        map.Add(key, dir);
                        toRoom.Add((prev.name, dir), nbr);
                    }
                    else
                        Trace.Assert(map[key] == dir);
                }
            }

            // things left open at the moment
            List<(string name, string dir)> Unexplored()
            {
                var ans = new List<(string name, string dir)>();
                foreach (var r in rooms)
                {
                    foreach (var dir in r.dirs)
                    {
                        bool found = false;
                        foreach (var p in map)
                        {
                            found |= p.Key.src == r.name && p.Value == dir.Key;
                        }

                        if (!found)
                            ans.Add((r.name, dir.Key));
                    }
                }

                return ans;
            }

            #endregion

            // read room, gather info
            void ProcessRoom(List<string> lines)
            {
                Room r = null;
                var inDoors = false;
                var inItems = false;

                foreach (var line in lines)
                {
                    if (string.IsNullOrEmpty(line)) continue;
                    var placeMatch = Regex.Match(line, @"== (?<place>[a-zA-Z- ]+) ==");
                    if (placeMatch.Success)
                    {
                        var name = placeMatch.Groups["place"].Value;
                        if (name == finalRoomName)
                            Console.WriteLine("YAY!");
                        r = rooms.FirstOrDefault(q => q.name == name);
                        if (r == null)
                        {
                            r = new Room { name = name };
                            rooms.Add(r);
                        }
                    }
                    else if (line.Contains("=="))
                        Console.WriteLine("ERROR!");

                    var doorMatch = Regex.Match(line, @"Doors here lead");
                    if (doorMatch.Success)
                    {
                        inDoors = true;
                        inItems = false;
                        continue;
                    }

                    var itemsMatch = Regex.Match(line, @"Items here");
                    if (itemsMatch.Success)
                    {
                        inDoors = false;
                        inItems = true;
                        continue;
                    }

                    if (line.Contains("Command?"))
                        break;

                    if (line.StartsWith("-"))
                    {

                        var itemMatch = Regex.Match(line, @"\- (?<item>[a-zA-Z0-9 ]+)");
                        if (inItems && itemMatch.Success)
                        {
                            var item = itemMatch.Groups["item"].Value.Trim();
                            if (!r.items.Contains(item))
                            {
                                if (
                                    item != "infinite loop" // do not take this - it infinite loops
                                    && item != "giant electromagnet" // you cannot move
                                    && item != "molten lava"
                                    && item != "escape pod"
                                    &&
                                    item !=
                                    "photons" // You take the photons. It is suddenly completely dark! You are eaten by a Grue!
                                )
                                {
                                    r.items.Add(item);
                                    takeMoves.Enqueue("take " + item);
                                    itemsTaken.Add(item); // not quite true - will take soon
                                }
                            }

                            continue;
                        }

                        // 20 rooms, 8 items can take (5 cannot?), 2 unexplored, then go for win:

                        var dirMatch = Regex.Match(line, @"\- (?<dir>(north|south|east|west))");
                        if (inDoors && dirMatch.Success)
                        {
                            var dir = dirMatch.Groups["dir"].Value;
                            if (!r.dirs.ContainsKey(dir))
                                r.dirs.Add(dir, null);
                            continue;
                        }

                        Console.WriteLine("ERROR!");

                    } // line start
                }

                curRoom = r;
            }

            // try to find rooms, fill in moves
            bool MovesToNearestUnexplored(Room start)
            {
                HashSet<Room> closed = new();

                // parent[room] = where to come from
                Dictionary<Room, Room> parent = new();


                var frontier = new Queue<Room>();
                frontier.Enqueue(start);
                while (frontier.Any())
                {
                    var r = frontier.Dequeue();
                    closed.Add(r);

                    foreach (var dir in r.dirs)
                    {
                        var key = (r.name, dir.Key);
                        if (!toRoom.ContainsKey(key))
                        {
                            // unexplored - go here, take this dir
                            var ans = new List<string>();
                            ans.Add(dir.Key);

                            while (parent.ContainsKey(r))
                            {
                                var p = parent[r];
                                var d = map[(p.name, r.name)];
                                ans.Add(d);
                                r = p;
                            }

                            ans.Reverse();
                            foreach (var a in ans)
                                moves.Enqueue(a);
                            return true;

                        }
                        else
                        {
                            var child = toRoom[key];
                            if (!closed.Contains(child))
                            {
                                parent.Add(child, r);
                                frontier.Enqueue(child);
                            }
                        }
                    }
                }

                return false;
            }

            // search map, take things
            void Search()
            {
                if (moves.Any()) return; // nothing to do

                if (curRoom != null)
                {
                    // explore unexplored, unless it is the final output from 
                    // preFinalRoomName
                    var unexplored = Unexplored();
                    var lastOne = unexplored.Where(n => n.name == preFinalRoomName).Count() == 1;
                    if (unexplored.Any() && !lastOne)
                    {
                        if (MovesToNearestUnexplored(curRoom))
                            return;
                    }
                    // find something unexplored to do

                    // prefer unexplored
                    if (!curRoom.name.Contains(preFinalRoomName)) // if take unexplored, get into if loop
                        foreach (var dir in curRoom.dirs)
                            if (!toRoom.ContainsKey((curRoom.name, dir.Key)))
                            {
                                moves.Enqueue(dir.Key);
                                return;
                            }

                    var d = curRoom.dirs.Select(p => p.Key).ToList();
                    if (d.Any())
                        moves.Enqueue(d[rand.Next(d.Count)]);
                }
            }

            string GetNextMoveFromQueue()
            {

                string move = "";
                
                if (takeMoves.Any())
                    move = takeMoves.Dequeue();
                else if (moves.Any())
                {
                    move = moves.Dequeue();
                    lastRoom = curRoom;
                }
                else
                    move = "inv";

                lastMove = move;
                return move;
            }

            bool winPlayed = false;

            bool WinSequence()
            {
                if (winPlayed) return true; // don't repeat it

                // 20 rooms, 8 items, unexplored 2
                // only unexplored: Security CheckPoint -> east and Pressure-Sensitive Floor -> west

                if (rooms.Count != 20) return false;
                if (itemsTaken.Count != 8) return false;
                var un = Unexplored();
                if (un.Count != 2) return false;
                if (!Unexplored().Any(c => c.name == "Security Checkpoint" && c.dir == "east")) return false;
                if (!Unexplored().Any(c => c.name == "Pressure-Sensitive Floor" && c.dir == "west")) return false;

                winPlayed = true;
                moves.Clear();
                // let any take moves continue - pick up all items

                if (curRoom.name != "Security Checkpoint")
                    MovesToNearestUnexplored(curRoom);
                bool [] dropped = new bool[8];
                var finalDir = "east";
                for (var v = 0; v < 256; ++v)
                {
                    Drop(v, true);
                    moves.Enqueue(finalDir);
                    Drop(v, false);
                }

                return true;

                // drop or take item
                void Drop(int v, bool drop)
                {
                    int item = 0;
                    var items = itemsTaken.ToList();
                    while (v > 0)
                    {
                        if ((v & 1) != 0)
                        {
                            if (drop && !dropped[item])
                            {
                                moves.Enqueue($"drop {items[item]}");
                                dropped[item] = true;
                            }
                            else if (!drop && dropped[item])
                            {
                                moves.Enqueue($"take {items[item]}");
                                dropped[item] = false;
                            }
                        }

                        ++item;

                        v >>= 1;
                    }
                }
            }

            // "Oh, hello! You should be able to get in by typing 805307408 on the keypad at the main airlock."


            // update, return move
            public string Update(List<string> lines)
            {
                ++turn;

                // read info
                if (!winPlayed)
                    ProcessRoom(lines);

                // mapping
                if ("northsoutheastwest".Contains(lastMove) && lastMove.Length > 3)
                    if (curRoom != null && lastRoom != null && lastRoom != curRoom)
                        Map(lastRoom, curRoom, lastMove);

                // enqueue any moves
                if (!WinSequence())
                    Search();

                // show some things
                var show = true;
                if (show)
                {
                    Console.WriteLine($"TURN {turn}");
                    Console.WriteLine(
                        $"Rooms seen {rooms.Count}, items {rooms.Sum(r => r.items.Count)}, map {map.Count}, unexplored {Unexplored().Count}");
                    foreach (var r1 in rooms)
                        Console.Write(r1.name + ", ");
                    Console.WriteLine();
                    foreach (var item in itemsTaken)
                        Console.Write(item + ", ");
                    Console.WriteLine();
                }

                // play out any enqueued moves
                return GetNextMoveFromQueue();
            }

            /*

        Handle case:

    == Pressure-Sensitive Floor ==
    Analyzing...
    Doors here lead:
    - west
    A loud, robotic voice says "Alert! Droids on this ship are lighter than the detected value!" and you are ejected back to the checkpoint.
    == Security Checkpoint ==
    In the next room, a pressure-sensitive floor will verify your identity.
    Doors here lead:
    - north
    - east
    Command?
             */


            /* string forms:

                "== <name place> =="
            "Doors here lead:"
            "- north"
            "- east"..

                "Items here:
            "- weather machine"
            "Command?"

             */
        }


        Day02.IntCode? comp1;

        Queue<long> inputQueue = new();

        long GetInput()
        {
            return inputQueue.Dequeue();
        }

        string curText = "";

        void WriteOutput(long ch)
        {
            curText += (char)ch;

            if (curText.Contains("Command?"))
            {
                var inp = GetNextCommand(curText);
                curText = "";

                inputQueue.Clear();
                inp += '\n';

                foreach (var s in inp)
                    inputQueue.Enqueue(s);
            }
        }
        
        Bot bot = new Bot();

        string GetNextCommand(string text)
        {

            //Console.WriteLine("Input:");
            //return Console.ReadLine();

            Console.WriteLine(curText); // - todo - make pretty

            var fg = Console.ForegroundColor;
            var rep = new[]
            {
                "s", "south",
                "n", "north",
                "e", "east",
                "w", "west",
            };

            // let bot process
            var lines = curText.Split('\n', StringSplitOptions.RemoveEmptyEntries).ToList();

            var winner = lines.FirstOrDefault(c => c.StartsWith("Oh, hello! You should be able to get in by typing"));
            if (!string.IsNullOrEmpty(winner))
            {
                answer = Numbers(winner)[0];
                throw new Exception($"Win {answer}");
            }

            var inpBot = bot.Update(lines);

            var inp = Console.ReadLine().Trim();
            // command replacements
            for (var i = 0; i < rep.Length; i += 2)
                inp = inp == rep[i] ? rep[i + 1] : inp;

            if (inp == "map")
            {
                bot.DrawMap();
                inp = inpBot;
            }

            if (string.IsNullOrEmpty(inp))
                inp = inpBot;
            
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write(inp);
            Console.ForegroundColor = fg;

            return inp;
        }

        long answer = -1;

        public override object Run(bool part2)
        {
           
            var lines = ReadLines();
            var input = Numbers64(lines[0]);
            comp1 = new Day02.IntCode(input, GetInput, WriteOutput);

            try
            {
                while (!comp1.Step())
                {
                }
            }
            catch { }

            return answer;

        }
    }
}