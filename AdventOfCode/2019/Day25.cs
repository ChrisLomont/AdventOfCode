using System.Data;
using System.Linq;
using System.Net.Mail;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Channels;
using System.Xml.Serialization;

namespace Lomont.AdventOfCode._2019
{
    // bot to play game
    class Bot
    {
        Random rand = new Random();

        Room? lastRoom = null;
        Room? curRoom = null;
        List<Room> rooms = new();

        string lastMove = "";
        HashSet<string> itemsTaken = new();

        int turn = 0;

        string preFinalRoomName = "Security Checkpoint";
        string finalRoomName = "Pressure Sensitive Floor";


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
        Dictionary<(string src, string dir), Room> toRoom = new ();
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

                    // 20 rooms, 8 items can take (5 cannot?)

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
#if true
        
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
#endif

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
                // todo - use MovesToNearestUnexplored

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

        string ProcessMoves()
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

        bool WinSequence()
        {
            if (
                curRoom != null &&
                itemsTaken.Count == 8 && 
                curRoom.name == preFinalRoomName && 
                !Unexplored().Any()
                )
            {
                
                takeMoves.Clear();
                moves.Clear();
                var finalDir = curRoom.dirs.Where(dir => toRoom[(curRoom.name, dir.Key)].name == finalRoomName)
                    .FirstOrDefault().Key;
                Trace.Assert(!string.IsNullOrEmpty(finalDir));
                for (var v = 0; v < 256; ++v)
                {
                    Drop(v, true);
                    moves.Enqueue(finalDir);
                    Drop(v, false);
                }

                return true;
            }

            return false;

            void Drop(int v, bool drop)
            {
                int item = 0;
                var items = itemsTaken.ToList();
                while (v > 0)
                {
                    if ((v & 1) != 0)
                    {
                        if (drop) 
                            moves.Enqueue($"drop {items[item]}");
                        else
                            moves.Enqueue($"take {items[item]}");
                    }

                    v >>= 1;
                }
            }
        }

        // update, return move
        public string Update(List<string> lines)
        {
            ++turn;
            Console.WriteLine($"TURN {turn}");

            ProcessRoom(lines);

            if ("northsoutheastwest".Contains(lastMove) && lastMove.Length>3)
            {
                if (curRoom != null && lastRoom != null && lastRoom != curRoom)
                {
                    Map(lastRoom, curRoom, lastMove);
                }
            }


            if (itemsTaken.Count == 8 && rooms.Count == 20)
            {
                // time to win, find room
            }

            
            if (!WinSequence())
                Search();

            var show = true;
            if (show)
            {
                Console.WriteLine(
                    $"Rooms seen {rooms.Count}, items {rooms.Sum(r => r.items.Count)}, map {map.Count}, unexplored {Unexplored().Count}");
                foreach (var r1 in rooms)
                    Console.Write(r1.name + ", ");
                Console.WriteLine();
                foreach (var item in itemsTaken)
                    Console.Write(item + ", ");
                Console.WriteLine();
            }


            return ProcessMoves();
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


    internal class Day25 : AdventOfCode
    {
        public class Computer
        {
            public Queue<long> InputQueue { get; set; }
            public Dictionary<long, long> Program { get; set; }
            public List<long> OutputList { get; set; }
            public bool Paused { get; set; }
            public bool Halted { get; set; }
            public Computer(List<long> input)
            {
                InputQueue = new Queue<long>();
                Program = new Dictionary<long, long>();
                var counter = 0;
                foreach (var i in input)
                {
                    Program.Add(counter, i);
                    counter++;
                }
                OutputList = new List<long>();
            }

            public void SendInput(long input)
            {
                InputQueue.Enqueue(input);
                Paused = false;
            }
            public void Output(long output)
            {
                OutputList.Add(output);
                Paused = true;
            }

            public long Position { get; set; }
            public long RelativeBase { get; set; }

            public void Run()
            {
                while (!Halted)
                {
                    Iterate();
                }
            }

            public string RunString(string input)
            {
                OutputList.Clear();
                //foreach(var i in input)
                //{
                //	int x = i;
                //	SendInput(x);
                //}
                while (!OutputList.Any() || OutputList.Last() != 10)
                {
                    Paused = false;
                    PauseRun();
                }
                return string.Concat(OutputList.Select(x => (char)x));
            }

            public void PauseRun()
            {
                while (!Halted && !Paused)
                {
                    Iterate();
                }
            }

            public void Iterate()
            {
                long param1;
                long param2;
                var op = new OpCode(ProgramRead(Position));
               // Debug.Write($"{Position}:{op.Op}, ");
                switch (op.Op)
                {
                    case 1:
                        param1 = GetValue(op.Mode1, Position + 1);
                        param2 = GetValue(op.Mode2, Position + 2);
                        SetValue(Position + 3, op.Mode3, param1 + param2);
                        Position += 4;
                        break;
                    case 2:
                        param1 = GetValue(op.Mode1, Position + 1);
                        param2 = GetValue(op.Mode2, Position + 2);
                        SetValue(Position + 3, op.Mode3, param1 * param2);
                        Position += 4;
                        break;
                    case 3:
                        SetValue(Position + 1, op.Mode1, InputQueue.Dequeue());
                        Position += 2;
                        break;
                    case 4:
                        param1 = GetValue(op.Mode1, Position + 1);
                        Output(param1);
                        Position += 2;
                        break;
                    case 5:
                        param1 = GetValue(op.Mode1, Position + 1);
                        if (param1 != 0)
                        {
                            Position = GetValue(op.Mode2, Position + 2);
                        }
                        else
                        {
                            Position += 3;
                        }
                        break;
                    case 6:
                        param1 = GetValue(op.Mode1, Position + 1);
                        if (param1 == 0)
                        {
                            Position = GetValue(op.Mode2, Position + 2);
                        }
                        else
                        {
                            Position += 3;
                        }
                        break;
                    case 7:
                        param1 = GetValue(op.Mode1, Position + 1);
                        param2 = GetValue(op.Mode2, Position + 2);
                        if (param1 < param2)
                        {
                            SetValue(Position + 3, op.Mode3, 1);
                        }
                        else
                        {
                            SetValue(Position + 3, op.Mode3, 0);
                        }
                        Position += 4;
                        break;
                    case 8:
                        param1 = GetValue(op.Mode1, Position + 1);
                        param2 = GetValue(op.Mode2, Position + 2);
                        SetValue(Position + 3, op.Mode3, param1 == param2 ? 1 : 0);
                        Position += 4;
                        break;
                    case 9:
                        param1 = GetValue(op.Mode1, Position + 1);
                        RelativeBase += param1;
                        Position += 2;
                        break;
                    case 99:
                        Halted = true;
                        break;
                    default:
                        throw new ArgumentException($"Unrecognised opcode {op.Op} at position {Position}");

                }
            }

            private long ProgramRead(long position)
            {
                return Program.ContainsKey(position) ? Program[position] : 0;
            }

            private void ProgramWrite(long position, long value)
            {
                if (!Program.ContainsKey(position))
                {
                    Program.Add(position, value);
                    return;
                }

                Program[position] = value;
            }

            internal void SetValue(long index, int mode, long value)
            {
                if (mode == 0)
                {
                    ProgramWrite(ProgramRead(index), value);
                    return;
                }
                if (mode == 2)
                {
                    ProgramWrite(RelativeBase + ProgramRead(index), value);
                    return;
                }
                throw new ArgumentException($"Error at index {index}: mode must be 0 or 2, not {mode}");
            }

            internal long GetValue(int mode, long index)
            {
                if (mode == 0)
                {
                    return ProgramRead(ProgramRead(index));
                }
                if (mode == 1)
                {
                    return ProgramRead(index);
                }
                if (mode == 2)
                {
                    return ProgramRead(ProgramRead(index) + RelativeBase);
                }
                throw new ArgumentException($"Error at index {index}: mode must be between 0 and 2, not {mode}");
            }
        }

        public class OpCode
        {
            public int Op { get; set; }
            public int Mode1 { get; set; }
            public int Mode2 { get; set; }
            public int Mode3 { get; set; }
            public OpCode(long input)
            {
                Op = (int)input % 100;
                Mode3 = (int)input / 10000;
                if (Mode3 > 2) throw new ArgumentException($"Mode 3 is {Mode3}, should be in range 0 to 2");
                var interim = input % 10000;
                Mode2 = (int)interim / 1000;
                if (Mode2 > 2) throw new ArgumentException($"Mode 2 is {Mode3}, should be in range 0 to 2");
                interim = input % 1000;
                Mode1 = (int)interim / 100;
                if (Mode1 > 2) throw new ArgumentException($"Mode 1 is {Mode3}, should be in range 0 to 2");
            }
        }

        List<string> commands = new(){
            "","north","north","north",
            "take semiconductor",
            "east",
            //"take prime number",
            "west","west",
            //"take monolith",
            "south", "north", "east", "south", "west", "north",
            "take jam",
            "south", "east", "south", "east", "east", "west", "west", "south", "west",
            "take mutex",
            "south", "south", "south",
            //"take polygon",
            "north", "east",
            //"take weather machine",
            "west", "north", "north", "east", "south",
            "take hologram",
            "west", "east", "north", "north", "north", "west", "north", "west", "north", "inv"
        };

    public object Run2(bool part2)
        {
            var lines = ReadLines();
            var input = Numbers64(lines[0]);

            var comp = new Computer(input);
       
            RunRooms(comp, commands);

            //into the checkpoint
            //var step = "north";
            //RunRoom(comp, step);

            return -100;
        }



    // Define other methods and classes here
        public void RunRooms(Computer comp, List<string> commands)
        {
            var fg = Console.ForegroundColor;
            var bot = new Bot();
            var rep = new[]
            {
                "s", "south",
                "n", "north",
                "e", "east",
                "w", "west",
            };

            var lines = new List<string>();
            while (true)
            {
                Console.Write("INPUT: ");
                var inp = Console.ReadLine().Trim();
                if (string.IsNullOrEmpty(inp))
                {
                    inp = bot.Update(lines);
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(inp);
                    Console.ForegroundColor = fg;
                }

                lines.Clear();
                Console.WriteLine(inp);

                for (var i = 0; i < rep.Length; i += 2)
                    inp = inp == rep[i] ? rep[i+1] : inp;

                comp.InputQueue.Clear();
                inp += '\n';

                foreach (var s in inp)
                    comp.SendInput((int)s);

                var next = "";
                while (!next.Contains("Command?"))
                {
                    next = comp.RunString("");
                    if (next != "\n")
                    {
                        lines.Add(next.Trim());
                        Console.WriteLine(next.Trim());
                    }

                    //next.Dump();
                    //str = "";
                }
            }
        }

        void Pack(List<int> input)
        {
            foreach (var c in commands)
            {
                var s = c.Trim();
                s += '\n';
                var b = Encoding.ASCII.GetBytes(s);
                input.AddRange(b.Select(cc=>(int)cc));
            }

        }

        public override object Run(bool part2)
        {
            Run2(part2);

            // commands - each ends with \n ASCII n
            // north, south, east, west
            // take <item>
            // drop <item>
            // inv to see all

            List<int> input = new();
            //Pack(input);
            int inputIndex = 0;
            Func<int> getInput = () =>
            {
                if (inputIndex >= input.Count)
                {
                    Console.Write("Input: ");
                    var txt = Console.ReadLine();
                    txt = txt.Replace("\n", "\n").Replace("\r", "\r");
                    txt += '\n';
                    var b = Encoding.ASCII.GetBytes(txt);
                    input.AddRange(b.Select(c=>(int)c));
                }
                return input[inputIndex++];
            };

            Action<long> handleOutput = v => { Console.WriteLine("Out: "+v); };

            var lines = ReadLines();
            var prog = Numbers64(lines[0], false);
            var mem = Day02.RunIntCode(prog, 
                //getInput,
                new List<int>{(int)'\n'},
                handleOutput
            );
            return mem[0];

        }

        static string good = "0:9, 2:2, 6:2, 10:6, 1424:9, 1426:2, 1430:2, 1434:1, 1438:6, 1234:9, 1236:2, 1240:1, 1244:2, 1248:6, 1174:9, 1176:1, 1180:1, 1184:2, 1188:1, 1192:8, 1196:5, 1199:1, 1203:1, 1207:2, 1211:2, 1215:2, 1219:5, 1256:9, 1258:1, 1262:1, 1266:4, 1268:9, 1270:6, 1222:1, 1226:5, 1192:8, 1196:5, 1199:1, 1203:1, 1207:2, 1211:2, 1215:2, 1219:5, 1256:9, 1258:1, 1262:1, 1266:4, 1268:9, 1270:6, 1222:1, 1226:5, 1192:8, 1196:5, 1199:1, 1203:1, 1207:2, 1211:2, 1215:2, 1219:5, 1256:9, 1258:1, 1262:1, 1266:4, 1268:9, 1270:6, 1222:1, 1226:5, 1192:8, 1196:5, 1199:1, 1203:1, 1207:2, 1211:2, 1215:2, 1219:5, 1256:9, 1258:1, 1262:1, 1266:4, 1268:9, 1270:6, 1222:1, 1226:5, 1192:8, 1196:5, 1199:1, 1203:1, 1207:2, 1211:2, 1215:2, 1219:5, 1256:9, 1258:1, 1262:1, 1266:4, 1268:9, 1270:6, 1222:1, 1226:5, 1192:8, 1196:5, 1199:1, 1203:1, 1207:2, 1211:2, 1215:2, 1219:5, 1256:9, 1258:1, 1262:1, 1266:4, 1268:9, 1270:6, 1222:1, 1226:5, 1192:8, 1196:5, 1229:9, 1231:6, 1251:9, 1253:5, 1441:1, 1445:1, 1449:2, 1453:5, 1234:9, 1236:2, 1240:1, 1244:2, 1248:6, 1174:9, 1176:1, 1180:1, 1184:2, 1188:1, 1192:8, 1196:5, 1199:1, 1203:1, 1207:2, 1211:2, 1215:2, 1219:5, 1256:9, 1258:1, 1262:1, 1266:4, 1268:9, 1270:6, 1222:1, 1226:5, 1192:8, 1196:5, 1199:1, 1203:1, 1207:2, 1211:2, 1215:2, 1219:5, 1256:9, 1258:1, 1262:1, 1266:4, 1268:9, 1270:6, 1222:1, 1226:5, 1192:8, 1196:5, 1199:1, 1203:1, 1207:2, 1211:2, 1215:2, 1219:5, 1256:9, 1258:1, 1262:1, 1266:4, 1268:9, 1270:6, 1222:1, 1226:5, 1192:8, 1196:5, 1199:1, 1203:1, 1207:2, 1211:2, 1215:2, 1219:5, 1256:9, 1258:1, 1262:1, 1266:4, 1268:9, 1270:6, 1222:1, 1226:5, 1192:8, 1196:5, 1199:1, 1203:1, 1207:2, 1211:2, 1215:2, 1219:5, 1256:9, 1258:1, 1262:1, 1266:4, 1268:9, 1270:6, 1222:1, 1226:5, 1192:8, 1196:5, 1199:1, 1203:1, 1207:2, 1211:2, 1215:2, 1219:5, 1256:9, 1258:1, 1262:1, 1266:4, 1268:9, 1270:6, 1222:1, 1226:5, 1192:8, 1196:5, 1199:1, 1203:1, 1207:2, 1211:2, 1215:2, 1219:5, 1256:9, 1258:1, 1262:1, 1266:4, 1268:9, 1270:6, 1222:1, 1226:5, 1192:8, 1196:5, 1199:1, 1203:1, 1207:2, 1211:2, 1215:2, 1219:5, 1256:9, 1258:1, 1262:1, 1266:4, 1268:9, 1270:6, 1222:1, 1226:5, 1192:8, 1196:5, 1199:1, 1203:1, 1207:2, 1211:2, 1215:2, 1219:5, 1256:9, 1258:1, 1262:1, 1266:4, 1268:9, 1270:6, 1222:1, 1226:5, 1192:8, 1196:5, 1199:1, 1203:1, 1207:2, 1211:2, 1215:2, 1219:5, 1256:9, 1258:1, 1262:1, 1266:4, 1268:9, 1270:6, 1222:1, 1226:5, 1192:8, 1196:5, 1199:1, 1203:1, 1207:2, 1211:2, 1215:2, 1219:5, 1256:9, 1258:1, 1262:1, 1266:4, 1268:9, 1270:6, 1222:1, 1226:5, 1192:8, 1196:5, 1229:9, 1231:6, 1251:9, 1253:5, 1456:2, 1460:1, 1464:5, 1234:9, 1236:2, 1240:1, 1244:2, 1248:6, 1174:9, 1176:1, 1180:1, 1184:2, 1188:1, 1192:8, 1196:5, 1199:1, 1203:1, 1207:2, 1211:2, 1215:2, 1219:5, 1256:9, 1258:1, 1262:1, 1266:4, 1268:9, 1270:6, 1222:1, 1226:5, 1192:8, 1196:5, 1199:1, 1203:1, 1207:2, 1211:2, 1215:2, 1219:5, 1256:9, 1258:1, 1262:1, 1266:4, 1268:9, 1270:6, 1222:1, 1226:5, 1192:8, 1196:5, 1199:1, 1203:1, 1207:2, 1211:2, 1215:2, 1219:5, 1256:9, 1258:1, 1262:1, 1266:4, 1268:9, 1270:6, 1222:1, 1226:5, 1192:8, 1196:5, 1199:1, 1203:1, 1207:2, 1211:2, 1215:2, 1219:5, 1256:9, 1258:1, 1262:1, 1266:4, 1268:9, 1270:6, 1222:1, 1226:5, 1192:8, 1196:5, 1229:9, 1231:6, 1251:9, 1253:5, 1467:1, 1471:2, 1475:1, 1479:5, 1234:9, 1236:2, 1240:1, 1244:2, 1248:6, 1174:9, 1176:1, 1180:1, 1184:2, 1188:1, 1192:8, 1196:5, 1199:1, 1203:1, 1207:2, 1211:2, 1215:2, 1219:5, 1256:9, 1258:1, 1262:1, 1266:4, 1268:9, 1270:6, 1222:1, 1226:5, 1192:8, 1196:5, 1199:1, 1203:1, 1207:2, 1211:2, 1215:2, 1219:5, 1256:9, 1258:1, 1262:1, 1266:4, 1268:9, 1270:6, 1222:1, 1226:5, 1192:8, 1196:5, 1199:1, 1203:1, 1207:2, 1211:2, 1215:2, 1219:5, 1256:9, 1258:1, 1262:1, 1266:4, 1268:9, 1270:6, 1222:1, 1226:5, 1192:8, 1196:5, 1199:1, 1203:1, 1207:2, 1211:2, 1215:2, 1219:5, 1256:9, 1258:1, 1262:1, 1266:4, 1268:9, 1270:6, 1222:1, 1226:5, 1192:8, 1196:5, 1199:1, 1203:1, 1207:2, 1211:2, 1215:2, 1219:5, 1256:9, 1258:1, 1262:1, 1266:4, 1268:9, 1270:6, 1222:1, 1226:5, 1192:8, 1196:5, 1199:1, 1203:1, 1207:2, 1211:2, 1215:2, 1219:5, 1256:9, 1258:1, 1262:1, 1266:4, 1268:9, 1270:6, 1222:1, 1226:5, 1192:8, 1196:5, 1199:1, 1203:1, 1207:2, 1211:2, 1215:2, 1219:5, 1256:9, 1258:1, 1262:1, 1266:4, 1268:9, 1270:6, 1222:1, 1226:5, 1192:8, 1196:5, 1199:1, 1203:1, 1207:2, 1211:2, 1215:2, 1219:5, 1256:9, 1258:1, 1262:1, 1266:4, 1268:9, 1270:6, 1222:1, 1226:5, 1192:8, 1196:5, 1199:1, 1203:1, 1207:2, 1211:2, 1215:2, 1219:5, 1256:9, 1258:1, 1262:1, 1266:4, 1268:9, 1270:6, 1222:1, 1226:5, 1192:8, 1196:5, 1199:1, 1203:1, 1207:2, 1211:2, 1215:2, 1219:5, 1256:9, 1258:1, 1262:1, 1266:4, 1268:9, 1270:6, 1222:1, 1226:5, 1192:8, 1196:5, 1199:1, 1203:1, 1207:2, 1211:2, 1215:2, 1219:5, 1256:9, 1258:1, 1262:1, 1266:4, 1268:9, 1270:6, 1222:1, 1226:5, 1192:8, 1196:5, 1199:1, 1203:1, 1207:2, 1211:2, 1215:2, 1219:5, 1256:9, 1258:1, 1262:1, 1266:4, 1268:9, 1270:6, 1222:1, 1226:5, 1192:8, 1196:5, 1199:1, 1203:1, 1207:2, 1211:2, 1215:2, 1219:5, 1256:9, 1258:1, 1262:1, 1266:4, 1268:9, 1270:6, 1222:1, 1226:5, 1192:8, 1196:5, 1199:1, 1203:1, 1207:2, 1211:2, 1215:2, 1219:5, 1256:9, 1258:1, 1262:1, 1266:4, 1268:9, 1270:6, 1222:1, 1226:5, 1192:8, 1196:5, 1199:1, 1203:1, 1207:2, 1211:2, 1215:2, 1219:5, 1256:9, 1258:1, 1262:1, 1266:4, 1268:9, 1270:6, 1222:1, 1226:5, 1192:8, 1196:5, 1199:1, 1203:1, 1207:2, 1211:2, 1215:2, 1219:5, 1256:9, 1258:1, 1262:1, 1266:4, 1268:9, 1270:6, 1222:1, 1226:5, 1192:8, 1196:5, 1199:1, 1203:1, 1207:2, 1211:2, 1215:2, 1219:5, 1256:9, 1258:1, 1262:1, 1266:4, 1268:9, 1270:6, 1222:1, 1226:5, 1192:8, 1196:5, 1199:1, 1203:1, 1207:2, 1211:2, 1215:2, 1219:5, 1256:9, 1258:1, 1262:1, 1266:4, 1268:9, 1270:6, 1222:1, 1226:5, 1192:8, 1196:5, 1199:1, 1203:1, 1207:2, 1211:2, 1215:2, 1219:5, 1256:9, 1258:1, 1262:1, 1266:4, 1268:9, 1270:6, 1222:1, 1226:5, 1192:8, 1196:5, 1199:1, 1203:1, 1207:2, 1211:2, 1215:2, 1219:5, 1256:9, 1258:1, 1262:1, 1266:4, 1268:9, 1270:6, 1222:1, 1226:5, 1192:8, 1196:5, 1199:1, 1203:1, 1207:2, 1211:2, 1215:2, 1219:5, 1256:9, 1258:1, 1262:1, 1266:4, 1268:9, 1270:6, 1222:1, 1226:5, 1192:8, 1196:5, 1199:1, 1203:1, 1207:2, 1211:2, 1215:2, 1219:5, 1256:9, 1258:1, 1262:1, 1266:4, 1268:9, 1270:6, 1222:1, 1226:5, 1192:8, 1196:5, 1199:1, 1203:1, 1207:2, 1211:2, 1215:2, 1219:5, 1256:9, 1258:1, 1262:1, 1266:4, 1268:9, 1270:6, 1222:1, 1226:5, 1192:8, 1196:5, 1199:1, 1203:1, 1207:2, 1211:2, 1215:2, 1219:5, 1256:9, 1258:1, 1262:1, 1266:4, 1268:9, 1270:6, 1222:1, 1226:5, 1192:8, 1196:5, 1199:1, 1203:1, 1207:2, 1211:2, 1215:2, 1219:5, 1256:9, 1258:1, 1262:1, 1266:4, 1268:9, 1270:6, 1222:1, 1226:5, 1192:8, 1196:5, 1199:1, 1203:1, 1207:2, 1211:2, 1215:2, 1219:5, 1256:9, 1258:1, 1262:1, 1266:4, 1268:9, 1270:6, 1222:1, 1226:5, 1192:8, 1196:5, 1199:1, 1203:1, 1207:2, 1211:2, 1215:2, 1219:5, 1256:9, 1258:1, 1262:1, 1266:4, 1268:9, 1270:6, 1222:1, 1226:5, 1192:8, 1196:5, 1199:1, 1203:1, 1207:2, 1211:2, 1215:2, 1219:5, 1256:9, 1258:1, 1262:1, 1266:4, 1268:9, 1270:6, 1222:1, 1226:5, 1192:8, 1196:5, 1199:1, 1203:1, 1207:2, 1211:2, 1215:2, 1219:5, 1256:9, 1258:1, 1262:1, 1266:4, 1268:9, 1270:6, 1222:1, 1226:5, 1192:8, 1196:5, 1199:1, 1203:1, 1207:2, 1211:2, 1215:2, 1219:5, 1256:9, 1258:1, 1262:1, 1266:4, 1268:9, 1270:6, 1222:1, 1226:5, 1192:8, 1196:5, 1199:1, 1203:1, 1207:2, 1211:2, 1215:2, 1219:5, 1256:9, 1258:1, 1262:1, 1266:4, 1268:9, 1270:6, 1222:1, 1226:5, 1192:8, 1196:5, 1199:1, 1203:1, 1207:2, 1211:2, 1215:2, 1219:5, 1256:9, 1258:1, 1262:1, 1266:4, 1268:9, 1270:6, 1222:1, 1226:5, 1192:8, 1196:5, 1199:1, 1203:1, 1207:2, 1211:2, 1215:2, 1219:5, 1256:9, 1258:1, 1262:1, 1266:4, 1268:9, 1270:6, 1222:1, 1226:5, 1192:8, 1196:5, 1199:1, 1203:1, 1207:2, 1211:2, 1215:2, 1219:5, 1256:9, 1258:1, 1262:1, 1266:4, 1268:9, 1270:6, 1222:1, 1226:5, 1192:8, 1196:5, 1199:1, 1203:1, 1207:2, 1211:2, 1215:2, 1219:5, 1256:9, 1258:1, 1262:1, 1266:4, 1268:9, 1270:6, 1222:1, 1226:5, 1192:8, 1196:5, 1199:1, 1203:1, 1207:2, 1211:2, 1215:2, 1219:5, 1256:9, 1258:1, 1262:1, 1266:4, 1268:9, 1270:6, 1222:1, 1226:5, 1192:8, 1196:5, 1199:1, 1203:1, 1207:2, 1211:2, 1215:2, 1219:5, 1256:9, 1258:1, 1262:1, 1266:4, 1268:9, 1270:6, 1222:1, 1226:5, 1192:8, 1196:5, 1199:1, 1203:1, 1207:2, 1211:2, 1215:2, 1219:5, 1256:9, 1258:1, 1262:1, 1266:4, 1268:9, 1270:6, 1222:1, 1226:5, 1192:8, 1196:5, 1199:1, 1203:1, 1207:2, 1211:2, 1215:2, 1219:5, 1256:9, 1258:1, 1262:1, 1266:4, 1268:9, 1270:6, 1222:1, 1226:5, 1192:8, 1196:5, 1199:1, 1203:1, 1207:2, 1211:2, 1215:2, 1219:5, 1256:9, 1258:1, 1262:1, 1266:4, 1268:9, 1270:6, 1222:1, 1226:5, 1192:8, 1196:5, 1199:1, 1203:1, 1207:2, 1211:2, 1215:2, 1219:5, 1256:9, 1258:1, 1262:1, 1266:4, 1268:9, 1270:6, 1222:1, 1226:5, 1192:8, 1196:5, 1199:1, 1203:1, 1207:2, 1211:2, 1215:2, 1219:5, 1256:9, 1258:1, 1262:1, 1266:4, 1268:9, 1270:6, 1222:1, 1226:5, 1192:8, 1196:5, 1199:1, 1203:1, 1207:2, 1211:2, 1215:2, 1219:5, 1256:9, 1258:1, 1262:1, 1266:4, 1268:9, 1270:6, 1222:1, 1226:5, 1192:8, 1196:5, 1199:1, 1203:1, 1207:2, 1211:2, 1215:2, 1219:5, 1256:9, 1258:1, 1262:1, 1266:4, 1268:9, 1270:6, 1222:1, 1226:5, 1192:8, 1196:5, 1199:1, 1203:1, 1207:2, 1211:2, 1215:2, 1219:5, 1256:9, 1258:1, 1262:1, 1266:4, 1268:9, 1270:6, 1222:1, 1226:5, 1192:8, 1196:5, 1199:1, 1203:1, 1207:2, 1211:2, 1215:2, 1219:5, 1256:9, 1258:1, 1262:1, 1266:4, 1268:9, 1270:6, 1222:1, 1226:5, 1192:8, 1196:5, 1199:1, 1203:1, 1207:2, 1211:2, 1215:2, 1219:5, 1256:9, 1258:1, 1262:1, 1266:4, 1268:9, 1270:6, 1222:1, 1226:5, 1192:8, 1196:5, 1199:1, 1203:1, 1207:2, 1211:2, 1215:2, 1219:5, 1256:9, 1258:1, 1262:1, 1266:4, 1268:9, 1270:6, 1222:1, 1226:5, 1192:8, 1196:5, 1199:1, 1203:1, 1207:2, 1211:2, 1215:2, 1219:5, 1256:9, 1258:1, 1262:1, 1266:4, 1268:9, 1270:6, 1222:1, 1226:5, 1192:8, 1196:5, 1199:1, 1203:1, 1207:2, 1211:2, 1215:2, 1219:5, 1256:9, 1258:1, 1262:1, 1266:4, 1268:9, 1270:6, 1222:1, 1226:5, 1192:8, 1196:5, 1199:1, 1203:1, 1207:2, 1211:2, 1215:2, 1219:5, 1256:9, 1258:1, 1262:1, 1266:4, 1268:9, 1270:6, 1222:1, 1226:5, 1192:8, 1196:5, 1199:1, 1203:1, 1207:2, 1211:2, 1215:2, 1219:5, 1256:9, 1258:1, 1262:1, 1266:4, 1268:9, 1270:6, 1222:1, 1226:5, 1192:8, 1196:5, 1199:1, 1203:1, 1207:2, 1211:2, 1215:2, 1219:5, 1256:9, 1258:1, 1262:1, 1266:4, 1268:9, 1270:6, 1222:1, 1226:5, 1192:8, 1196:5, 1199:1, 1203:1, 1207:2, 1211:2, 1215:2, 1219:5, 1256:9, 1258:1, 1262:1, 1266:4, 1268:9, 1270:6, 1222:1, 1226:5, 1192:8, 1196:5, 1199:1, 1203:1, 1207:2, 1211:2, 1215:2, 1219:5, 1256:9, 1258:1, 1262:1, 1266:4, 1268:9, 1270:6, 1222:1, 1226:5, 1192:8, 1196:5, 1199:1, 1203:1, 1207:2, 1211:2, 1215:2, 1219:5, 1256:9, 1258:1, 1262:1, 1266:4, 1268:9, 1270:6, 1222:1, 1226:5, 1192:8, 1196:5, 1199:1, 1203:1, 1207:2, 1211:2, 1215:2, 1219:5, 1256:9, 1258:1, 1262:1, 1266:4, 1268:9, 1270:6, 1222:1, 1226:5, 1192:8, 1196:5, 1199:1, 1203:1, 1207:2, 1211:2, 1215:2, 1219:5, 1256:9, 1258:1, 1262:1, 1266:4, 1268:9, 1270:6, 1222:1, 1226:5, 1192:8, 1196:5, 1199:1, 1203:1, 1207:2, 1211:2, 1215:2, 1219:5, 1256:9, 1258:1, 1262:1, 1266:4, 1268:9, 1270:6, 1222:1, 1226:5, 1192:8, 1196:5, 1199:1, 1203:1, 1207:2, 1211:2, 1215:2, 1219:5, 1256:9, 1258:1, 1262:1, 1266:4, 1268:9, 1270:6, 1222:1, 1226:5, 1192:8, 1196:5, 1199:1, 1203:1, 1207:2, 1211:2, 1215:2, 1219:5, 1256:9, 1258:1, 1262:1, 1266:4, 1268:9, 1270:6, 1222:1, 1226:5, 1192:8, 1196:5, 1199:1, 1203:1, 1207:2, 1211:2, 1215:2, 1219:5, 1256:9, 1258:1, 1262:1, 1266:4, 1268:9, 1270:6, 1222:1, 1226:5, 1192:8, 1196:5, 1199:1, 1203:1, 1207:2, 1211:2, 1215:2, 1219:5, 1256:9, 1258:1, 1262:1, 1266:4, 1268:9, 1270:6, 1222:1, 1226:5, 1192:8, 1196:5, 1199:1, 1203:1, 1207:2, 1211:2, 1215:2, 1219:5, 1256:9, 1258:1, 1262:1, 1266:4, 1268:9, 1270:6, 1222:1, 1226:5, 1192:8, 1196:5, 1199:1, 1203:1, 1207:2, 1211:2, 1215:2, 1219:5, 1256:9, 1258:1, 1262:1, 1266:4, 1268:9, 1270:6, 1222:1, 1226:5, 1192:8, 1196:5, 1199:1, 1203:1, 1207:2, 1211:2, 1215:2, 1219:5, 1256:9, 1258:1, 1262:1, 1266:4, 1268:9, 1270:6, 1222:1, 1226:5, 1192:8, 1196:5, 1199:1, 1203:1, 1207:2, 1211:2, 1215:2, 1219:5, 1256:9, 1258:1, 1262:1, 1266:4, 1268:9, 1270:6, 1222:1, 1226:5, 1192:8, 1196:5, 1199:1, 1203:1, 1207:2, 1211:2, 1215:2, 1219:5, 1256:9, 1258:1, 1262:1, 1266:4, 1268:9, 1270:6, 1222:1, 1226:5, 1192:8, 1196:5, 1199:1, 1203:1, 1207:2, 1211:2, 1215:2, 1219:5, 1256:9, 1258:1, 1262:1, 1266:4, 1268:9, 1270:6, 1222:1, 1226:5, 1192:8, 1196:5, 1199:1, 1203:1, 1207:2, 1211:2, 1215:2, 1219:5, 1256:9, 1258:1, 1262:1, 1266:4, 1268:9, 1270:6, 1222:1, 1226:5, 1192:8, 1196:5, 1199:1, 1203:1, 1207:2, 1211:2, 1215:2, 1219:5, 1256:9, 1258:1, 1262:1, 1266:4, 1268:9, 1270:6, 1222:1, 1226:5, 1192:8, 1196:5, 1199:1, 1203:1, 1207:2, 1211:2, 1215:2, 1219:5, 1256:9, 1258:1, 1262:1, 1266:4, 1268:9, 1270:6, 1222:1, 1226:5, 1192:8, 1196:5, 1199:1, 1203:1, 1207:2, 1211:2, 1215:2, 1219:5, 1256:9, 1258:1, 1262:1, 1266:4, 1268:9, 1270:6, 1222:1, 1226:5, 1192:8, 1196:5, 1199:1, 1203:1, 1207:2, 1211:2, 1215:2, 1219:5, 1256:9, 1258:1, 1262:1, 1266:4, 1268:9, 1270:6, 1222:1, 1226:5, 1192:8, 1196:5, 1199:1, 1203:1, 1207:2, 1211:2, 1215:2, 1219:5, 1256:9, 1258:1, 1262:1, 1266:4, 1268:9, 1270:6, 1222:1, 1226:5, 1192:8, 1196:5, 1199:1, 1203:1, 1207:2, 1211:2, 1215:2, 1219:5, 1256:9, 1258:1, 1262:1, 1266:4, 1268:9, 1270:6, 1222:1, 1226:5, 1192:8, 1196:5, 1199:1, 1203:1, 1207:2, 1211:2, 1215:2, 1219:5, 1256:9, 1258:1, 1262:1, 1266:4, 1268:9, 1270:6, 1222:1, 1226:5, 1192:8, 1196:5, 1199:1, 1203:1, 1207:2, 1211:2, 1215:2, 1219:5, 1256:9, 1258:1, 1262:1, 1266:4, 1268:9, 1270:6, 1222:1, 1226:5, 1192:8, 1196:5, 1199:1, 1203:1, 1207:2, 1211:2, 1215:2, 1219:5, 1256:9, 1258:1, 1262:1, 1266:4, 1268:9, 1270:6, 1222:1, 1226:5, 1192:8, 1196:5, 1199:1, 1203:1, 1207:2, 1211:2, 1215:2, 1219:5, 1256:9, 1258:1, 1262:1, 1266:4, 1268:9, 1270:6, 1222:1, 1226:5, 1192:8, 1196:5, 1199:1, 1203:1, 1207:2, 1211:2, 1215:2, 1219:5, 1256:9, 1258:1, 1262:1, 1266:4, 1268:9, 1270:6, 1222:1, 1226:5, 1192:8, 1196:5, 1199:1, 1203:1, 1207:2, 1211:2, 1215:2, 1219:5, 1256:9, 1258:1, 1262:1, 1266:4, 1268:9, 1270:6, 1222:1, 1226:5, 1192:8, 1196:5, 1199:1, 1203:1, 1207:2, 1211:2, 1215:2, 1219:5, 1256:9, 1258:1, 1262:1, 1266:4, 1268:9, 1270:6, 1222:1, 1226:5, 1192:8, 1196:5, 1199:1, 1203:1, 1207:2, 1211:2, 1215:2, 1219:5, 1256:9, 1258:1, 1262:1, 1266:4, 1268:9, 1270:6, 1222:1, 1226:5, 1192:8, 1196:5, 1199:1, 1203:1, 1207:2, 1211:2, 1215:2, 1219:5, 1256:9, 1258:1, 1262:1, 1266:4, 1268:9, 1270:6, 1222:1, 1226:5, 1192:8, 1196:5, 1199:1, 1203:1, 1207:2, 1211:2, 1215:2, 1219:5, 1256:9, 1258:1, 1262:1, 1266:4, 1268:9, 1270:6, 1222:1, 1226:5, 1192:8, 1196:5, 1199:1, 1203:1, 1207:2, 1211:2, 1215:2, 1219:5, 1256:9, 1258:1, 1262:1, 1266:4, 1268:9, 1270:6, 1222:1, 1226:5, 1192:8, 1196:5, 1199:1, 1203:1, 1207:2, 1211:2, 1215:2, 1219:5, 1256:9, 1258:1, 1262:1, 1266:4, 1268:9, 1270:6, 1222:1, 1226:5, 1192:8, 1196:5, 1199:1, 1203:1, 1207:2, 1211:2, 1215:2, 1219:5, 1256:9, 1258:1, 1262:1, 1266:4, 1268:9, 1270:6, 1222:1, 1226:5, 1192:8, 1196:5, 1199:1, 1203:1, 1207:2, 1211:2, 1215:2, 1219:5, 1256:9, 1258:1, 1262:1, 1266:4, 1268:9, 1270:6, 1222:1, 1226:5, 1192:8, 1196:5, 1199:1, 1203:1, 1207:2, 1211:2, 1215:2, 1219:5, 1256:9, 1258:1, 1262:1, 1266:4, 1268:9, 1270:6, 1222:1, 1226:5, 1192:8, 1196:5, 1199:1, 1203:1, 1207:2, 1211:2, 1215:2, 1219:5, 1256:9, 1258:1, 1262:1, 1266:4, 1268:9, 1270:6, 1222:1, 1226:5, 1192:8, 1196:5, 1199:1, 1203:1, 1207:2, 1211:2, 1215:2, 1219:5, 1256:9, 1258:1, 1262:1, 1266:4, 1268:9, 1270:6, 1222:1, 1226:5, 1192:8, 1196:5, 1199:1, 1203:1, 1207:2, 1211:2, 1215:2, 1219:5, 1256:9, 1258:1, 1262:1, 1266:4, 1268:9, 1270:6, 1222:1, 1226:5, 1192:8, 1196:5, 1199:1, 1203:1, 1207:2, 1211:2, 1215:2, 1219:5, 1256:9, 1258:1, 1262:1, 1266:4, 1268:9, 1270:6, 1222:1, 1226:5, 1192:8, 1196:5, 1199:1, 1203:1, 1207:2, 1211:2, 1215:2, 1219:5, 1256:9, 1258:1, 1262:1, 1266:4, 1268:9, 1270:6, 1222:1, 1226:5, 1192:8, 1196:5, 1199:1, 1203:1, 1207:2, 1211:2, 1215:2, 1219:5, 1256:9, 1258:1, 1262:1, 1266:4, 1268:9, 1270:6, 1222:1, 1226:5, 1192:8, 1196:5, 1199:1, 1203:1, 1207:2, 1211:2, 1215:2, 1219:5, 1256:9, 1258:1, 1262:1, 1266:4, 1268:9, 1270:6, 1222:1, 1226:5, 1192:8, 1196:5, 1199:1, 1203:1, 1207:2, 1211:2, 1215:2, 1219:5, 1256:9, 1258:1, 1262:1, 1266:4, 1268:9, 1270:6, 1222:1, 1226:5, 1192:8, 1196:5, 1199:1, 1203:1, 1207:2, 1211:2, 1215:2, 1219:5, 1256:9, 1258:1, 1262:1, 1266:4, 1268:9, 1270:6, 1222:1";
        static string bad1 = "0:9, 2:2, 6:2, 10:6, 1424:9, 1426:2, 1430:2, 1434:1, 1438:6, 1234:9, 1236:2, 1240:1, 1244:2, 1248:6, 1174:9, 1176:1, 1180:1, 1184:2, 1188:1, 1192:8, 1196:5, 1199:1, 1203:1, 1207:2, 1211:2, 1215:2, 1219:5, 0:9, 2:2, 6:2, 10:6, 1424:9, 1426:2, 1430:2, 1434:1, 1438:6, 1234:9, 1236:2, 1240:1, 1244:2, 1248:6, 1174:9, 1176:1, 1180:1, 1184:2, 1188:1, 1192:8, 1196:5, 1199:1, 1203:1, 1207:2, 1211:2, 1215:2, 1219:5, 0:9, 2:2, 6:2, 10:6, 1424:9, 1426:2, 1430:2, 1434:1, 1438:6, 1234:9, 1236:2, 1240:1, 1244:2, 1248:6, 1174:9, 1176:1, 1180:1, 1184:2, 1188:1, 1192:8, 1196:5, 1199:1, 1203:1, 1207:2, 1211:2, 1215:2, 1219:5, 0:9, 2:2, 6:2, 10:6, 1424:9, 1426:2, 1430:2, 1434:1, 1438:6, 1234:9, 1236:2, 1240:1, 1244:2, 1248:6, 1174:9, 1176:1, 1180:1, 1184:2, 1188:1, 1192:8, 1196:5, 1199:1, 1203:1, 1207:2, 1211:2, 1215:2, 1219:5, 0:9, 2:2, 6:2, 10:6, 1424:9, 1426:2, 1430:2, 1434:1, 1438:6, 1234:9, 1236:2, 1240:1, 1244:2, 1248:6, 1174:9, 1176:1, 1180:1, 1184:2, 1188:1, 1192:8, 1196:5, 1199:1, 1203:1, 1207:2, 1211:2, 1215:2, 1219:5, 0:9, 2:2, 6:2, 10:6, 1424:9, 1426:2, 1430:2, 1434:1, 1438:6, 1234:9, 1236:2, 1240:1, 1244:2, 1248:6, 1174:9, 1176:1, 1180:1, 1184:2, 1188:1, 1192:8, 1196:5, 1199:1, 1203:1, 1207:2, 1211:2, 1215:2, 1219:5, 0:9, 2:2, 6:2, 10:6, 1424:9, 1426:2, 1430:2, 1434:1, 1438:6, 1234:9, 1236:2, 1240:1, 1244:2, 1248:6, 1174:9, 1176:1, 1180:1, 1184:2, 1188:1, 1192:8, 1196:5, 1199:1, 1203:1, 1207:2, 1211:2, 1215:2, 1219:5, 0:9, 2:2, 6:2, 10:6, 1424:9, 1426:2, 1430:2, 1434:1, 1438:6, 1234:9, 1236:2, 1240:1, 1244:2, 1248:6, 1174:9, 1176:1, 1180:1, 1184:2, 1188:1, 1192:8, 1196:5, 1199:1, 1203:1, 1207:2, 1211:2, 1215:2, 1219:5, 0:9, 2:2, 6:2, 10:6, 1424:9, 1426:2, 1430:2, 1434:1, 1438:6, 1234:9, 1236:2, 1240:1, 1244:2, 1248:6, 1174:9, 1176:1, 1180:1, 1184:2, 1188:1, 1192:8, 1196:5, 1199:1, 1203:1, 1207:2, 1211:2, 1215:2, 1219:5, 0:9, 2:2, 6:2, 10:6, 1424:9, 1426:2, 1430:2, 1434:1, 1438:6, 1234:9, 1236:2, 1240:1, 1244:2, 1248:6, 1174:9, 1176:1, 1180:1, 1184:2, 1188:1, 1192:8, 1196:5, 1199:1, 1203:1, 1207:2, 1211:2, 1215:2, 1219:5, 0:9, 2:2, 6:2, 10:6, 1424:9, 1426:2, 1430:2, 1434:1, 1438:6, 1234:9, 1236:2, 1240:1, 1244:2, 1248:6, 1174:9, 1176:1, 1180:1, 1184:2, 1188:1, 1192:8, 1196:5, 1199:1, 1203:1, 1207:2, 1211:2, 1215:2, 1219:5, 0:9, 2:2, 6:2, 10:6, 1424:9, 1426:2, 1430:2, 1434:1, 1438:6, 1234:9, 1236:2, 1240:1, 1244:2, 1248:6, 1174:9, 1176:1, 1180:1, 1184:2, 1188:1, 1192:8, 1196:5, 1199:1, 1203:1, 1207:2, 1211:2, 1215:2, 1219:5, 0:9, 2:2, 6:2, 10:6, 1424:9, 1426:2, 1430:2, 1434:1, 1438:6, 1234:9, 1236:2, 1240:1, 1244:2, 1248:6, 1174:9, 1176:1, 1180:1, 1184:2, 1188:1, 1192:8, 1196:5, 1199:1, 1203:1, 1207:2, 1211:2, 1215:2, 1219:5, 0:9, 2:2, 6:2, 10:6, 1424:9, 1426:2, 1430:2, 1434:1, 1438:6, 1234:9, 1236:2, 1240:1, 1244:2, 1248:6, 1174:9, 1176:1, 1180:1, 1184:2, 1188:1, 1192:8, 1196:5, 1199:1, 1203:1, 1207:2, 1211:2, 1215:2, 1219:5, 0:9, 2:2, 6:2, 10:6, 1424:9, 1426:2, 1430:2, 1434:1, 1438:6, 1234:9, 1236:2, 1240:1, 1244:2, 1248:6, 1174:9, 1176:1, 1180:1, 1184:2, 1188:1, 1192:8, 1196:5, 1199:1, 1203:1, 1207:2, 1211:2, 1215:2, 1219:5, 0:9, 2:2, 6:2, 10:6, 1424:9, 1426:2, 1430:2, 1434:1, 1438:6, 1234:9, 1236:2, 1240:1, 1244:2, 1248:6, 1174:9, 1176:1, 1180:1, 1184:2, 1188:1, 1192:8, 1196:5, 1199:1, 1203:1, 1207:2, 1211:2, 1215:2, 1219:5, 0:9, 2:2, 6:2, 10:6, 1424:9, 1426:2, 1430:2, 1434:1, 1438:6, 1234:9, 1236:2, 1240:1, 1244:2, 1248:6, 1174:9, 1176:1, 1180:1, 1184:2, 1188:1, 1192:8, 1196:5, 1199:1, 1203:1, 1207:2, 1211:2, 1215:2, 1219:5, 0:9, 2:2, 6:2, 10:6, 1424:9, 1426:2, 1430:2, 1434:1, 1438:6, 1234:9, 1236:2, 1240:1, 1244:2, 1248:6, 1174:9, 1176:1, 1180:1, 1184:2, 1188:1, 1192:8, 1196:5, 1199:1, 1203:1, 1207:2, 1211:2, 1215:2, 1219:5, 0:9, 2:2, 6:2, 10:6, 1424:9, 1426:2, 1430:2, 1434:1, 1438:6, 1234:9, 1236:2, 1240:1, 1244:2, 1248:6, 1174:9, 1176:1, 1180:1, 1184:2, 1188:1, 1192:8, 1196:5, 1199:1, 1203:1, 1207:2, 1211:2, 1215:2, 1219:5, 0:9, 2:2, 6:2, 10:6, 1424:9, 1426:2, 1430:2, 1434:1, 1438:6, 1234:9, 1236:2, 1240:1, 1244:2, 1248:6, 1174:9, 1176:1, 1180:1, 1184:2, 1188:1, 1192:8, 1196:5, 1199:1, 1203:1, 1207:2, 1211:2, 1215:2, 1219:5, 0:9, 2:2, 6:2, 10:6, 1424:9, 1426:2, 1430:2, 1434:1, 1438:6, 1234:9, 1236:2, 1240:1, 1244:2, 1248:6, 1174:9, 1176:1, 1180:1, 1184:2, 1188:1, 1192:8, 1196:5, 1199:1, 1203:1, 1207:2, 1211:2, 1215:2, 1219:5, 0:9, 2:2, 6:2, 10:6, 1424:9, 1426:2, 1430:2, 1434:1, 1438:6, 1234:9, 1236:2, 1240:1, 1244:2, 1248:6, 1174:9, 1176:1, 1180:1, 1184:2, 1188:1, 1192:8, 1196:5, 1199:1, 1203:1, 1207:2, 1211:2, 1215:2, 1219:5, 0:9, 2:2, 6:2, 10:6, 1424:9, 1426:2, 1430:2, 1434:1, 1438:6, 1234:9, 1236:2, 1240:1, 1244:2, 1248:6, 1174:9, 1176:1, 1180:1, 1184:2, 1188:1, 1192:8, 1196:5, 1199:1, 1203:1, 1207:2, 1211:2, 1215:2, 1219:5, 0:9, 2:2, 6:2, 10:6, 1424:9, 1426:2, 1430:2, 1434:1, 1438:6, 1234:9, 1236:2, 1240:1, 1244:2, 1248:6, 1174:9, 1176:1, 1180:1, 1184:2, 1188:1, 1192:8, 1196:5, 1199:1, 1203:1, 1207:2, 1211:2, 1215:2, 1219:5, 0:9, 2:2, 6:2, 10:6, 1424:9, 1426:2, 1430:2, 1434:1, 1438:6, 1234:9, 1236:2, 1240:1, 1244:2, 1248:6, 1174:9, 1176:1, 1180:1, 1184:2, 1188:1, 1192:8, 1196:5, 1199:1, 1203:1, 1207:2, 1211:2, 1215:2, 1219:5, 0:9, 2:2, 6:2, 10:6, 1424:9, 1426:2, 1430:2, 1434:1, 1438:6, 1234:9, 1236:2, 1240:1, 1244:2, 1248:6, 1174:9, 1176:1, 1180:1, 1184:2, 1188:1, 1192:8, 1196:5, 1199:1, 1203:1, 1207:2, 1211:2, 1215:2, 1219:5, 0:9, 2:2, 6:2, 10:6, 1424:9, 1426:2, 1430:2, 1434:1, 1438:6, 1234:9, 1236:2, 1240:1, 1244:2, 1248:6, 1174:9, 1176:1, 1180:1, 1184:2, 1188:1, 1192:8, 1196:5, 1199:1, 1203:1, 1207:2, 1211:2, 1215:2, 1219:5, 0:9, 2:2, 6:2, 10:6, 1424:9, 1426:2, 1430:2, 1434:1, 1438:6, 1234:9, 1236:2, 1240:1, 1244:2, 1248:6, 1174:9, 1176:1, 1180:1, 1184:2, 1188:1, 1192:8, 1196:5, 1199:1, 1203:1, 1207:2, 1211:2, 1215:2, 1219:5, 0:9, 2:2, 6:2, 10:6, 1424:9, 1426:2, 1430:2, 1434:1, 1438:6, 1234:9, 1236:2, 1240:1, 1244:2, 1248:6, 1174:9, 1176:1, 1180:1, 1184:2, 1188:1, 1192:8, 1196:5, 1199:1, 1203:1, 1207:2, 1211:2, 1215:2, 1219:5, 0:9, 2:2, 6:2, 10:6, 1424:9, 1426:2, 1430:2, 1434:1, 1438:6, 1234:9, 1236:2, 1240:1, 1244:2, 1248:6, 1174:9, 1176:1, 1180:1, 1184:2, 1188:1, 1192:8, 1196:5, 1199:1, 1203:1, 1207:2, 1211:2, 1215:2, 1219:5, 0:9, 2:2, 6:2, 10:6, 1424:9, 1426:2, 1430:2, 1434:1, 1438:6, 1234:9, 1236:2, 1240:1, 1244:2, 1248:6, 1174:9, 1176:1, 1180:1, 1184:2, 1188:1, 1192:8, 1196:5, 1199:1, 1203:1, 1207:2, 1211:2, 1215:2, 1219:5, 0:9, 2:2, 6:2, 10:6, 1424:9, 1426:2, 1430:2, 1434:1, 1438:6, 1234:9, 1236:2, 1240:1, 1244:2, 1248:6, 1174:9, 1176:1, 1180:1, 1184:2, 1188:1, 1192:8, 1196:5, 1199:1, 1203:1, 1207:2, 1211:2, 1215:2, 1219:5, 0:9, 2:2, 6:2, 10:6, 1424:9, 1426:2, 1430:2, 1434:1, 1438:6, 1234:9, 1236:2, 1240:1, 1244:2, 1248:6, 1174:9, 1176:1, 1180:1, 1184:2, 1188:1, 1192:8, 1196:5, 1199:1, 1203:1, 1207:2, 1211:2, 1215:2, 1219:5, 0:9, 2:2, 6:2, 10:6, 1424:9, 1426:2, 1430:2, 1434:1, 1438:6, 1234:9, 1236:2, 1240:1, 1244:2, 1248:6, 1174:9, 1176:1, 1180:1, 1184:2, 1188:1, 1192:8, 1196:5, 1199:1, 1203:1, 1207:2, 1211:2, 1215:2, 1219:5, 0:9, 2:2, 6:2, 10:6, 1424:9, 1426:2, 1430:2, 1434:1, 1438:6, 1234:9, 1236:2, 1240:1, 1244:2, 1248:6, 1174:9, 1176:1, 1180:1, 1184:2, 1188:1, 1192:8, 1196:5, 1199:1, 1203:1, 1207:2, 1211:2, 1215:2, 1219:5, 0:9, 2:2, 6:2, 10:6, 1424:9, 1426:2, 1430:2, 1434:1, 1438:6, 1234:9, 1236:2, 1240:1, 1244:2, 1248:6, 1174:9, 1176:1, 1180:1, 1184:2, 1188:1, 1192:8, 1196:5, 1199:1, 1203:1, 1207:2, 1211:2, 1215:2, 1219:5, 0:9, 2:2, 6:2, 10:6, 1424:9, 1426:2, 1430:2, 1434:1, 1438:6, 1234:9, 1236:2, 1240:1, 1244:2, 1248:6, 1174:9, 1176:1, 1180:1, 1184:2, 1188:1, 1192:8, 1196:5, 1199:1, 1203:1, 1207:2, 1211:2, 1215:2, 1219:5, 0:9, 2:2, 6:2, 10:6, 1424:9, 1426:2, 1430:2, 1434:1, 1438:6, 1234:9, 1236:2, 1240:1, 1244:2, 1248:6, 1174:9, 1176:1, 1180:1, 1184:2, 1188:1, 1192:8, 1196:5, 1199:1, 1203:1, 1207:2, 1211:2, 1215:2, 1219:5, 0:9, 2:2, 6:2, 10:6, 1424:9, 1426:2, 1430:2, 1434:1, 1438:6, 1234:9, 1236:2, 1240:1, 1244:2, 1248:6, 1174:9, 1176:1, 1180:1, 1184:2, 1188:1, 1192:8, 1196:5, 1199:1, 1203:1, 1207:2, 1211:2, 1215:2, 1219:5, 0:9, 2:2, 6:2, 10:6, 1424:9, 1426:2, 1430:2, 1434:1, 1438:6, 1234:9, 1236:2, 1240:1, 1244:2, 1248:6, 1174:9, 1176:1, 1180:1, 1184:2, 1188:1, 1192:8, 1196:5, 1199:1, 1203:1, 1207:2, 1211:2, 1215:2, 1219:5, 0:9, 2:2, 6:2, 10:6, 1424:9, 1426:2, 1430:2, 1434:1, 1438:6, 1234:9, 1236:2, 1240:1, 1244:2, 1248:6, 1174:9, 1176:1, 1180:1, 1184:2, 1188:1, 1192:8, 1196:5, 1199:1, 1203:1, 1207:2, 1211:2, 1215:2, 1219:5, 0:9, 2:2, 6:2, 10:6, 1424:9, 1426:2, 1430:2, 1434:1, 1438:6, 1234:9, 1236:2, 1240:1, 1244:2, 1248:6, 1174:9, 1176:1, 1180:1, 1184:2, 1188:1, 1192:8, 1196:5, 1199:1, 1203:1, 1207:2, 1211:2, 1215:2, 1219:5, 0:9, 2:2, 6:2, 10:6, 1424:9, 1426:2, 1430:2, 1434:1, 1438:6, 1234:9, 1236:2, 1240:1, 1244:2, 1248:6, 1174:9, 1176:1, 1180:1, 1184:2, 1188:1, 1192:8, 1196:5, 1199:1, 1203:1, 1207:2, 1211:2, 1215:2, 1219:5, 0:9, 2:2, 6:2, 10:6, 1424:9, 1426:2, 1430:2, 1434:1, 1438:6, 1234:9, 1236:2, 1240:1, 1244:2, 1248:6, 1174:9, 1176:1, 1180:1, 1184:2, 1188:1, 1192:8, 1196:5, 1199:1, 1203:1, 1207:2, 1211:2, 1215:2, 1219:5, 0:9, 2:2, 6:2, 10:6, 1424:9, 1426:2, 1430:2, 1434:1, 1438:6, 1234:9, 1236:2, 1240:1, 1244:2, 1248:6, 1174:9, 1176:1, 1180:1, 1184:2, 1188:1, 1192:8, 1196:5, 1199:1, 1203:1, 1207:2, 1211:2, 1215:2, 1219:5, 0:9, 2:2, 6:2, 10:6, 1424:9, 1426:2, 1430:2, 1434:1, 1438:6, 1234:9, 1236:2, 1240:1, 1244:2, 1248:6, 1174:9, 1176:1, 1180:1, 1184:2, 1188:1, 1192:8, 1196:5, 1199:1, 1203:1, 1207:2, 1211:2, 1215:2, 1219:5, 0:9, 2:2, 6:2, 10:6, 1424:9, 1426:2, 1430:2, 1434:1, 1438:6, 1234:9, 1236:2, 1240:1, 1244:2, 1248:6, 1174:9, 1176:1, 1180:1, 1184:2, 1188:1, 1192:8, 1196:5, 1199:1, 1203:1, 1207:2, 1211:2, 1215:2, 1219:5, 0:9, 2:2, 6:2, 10:6, 1424:9, 1426:2, 1430:2, 1434:1, 1438:6, 1234:9, 1236:2, 1240:1, 1244:2, 1248:6, 1174:9, 1176:1, 1180:1, 1184:2, 1188:1, 1192:8, 1196:5, 1199:1, 1203:1, 1207:2, 1211:2, 1215:2, 1219:5, 0:9, 2:2, 6:2, 10:6, 1424:9, 1426:2, 1430:2, 1434:1, 1438:6, 1234:9, 1236:2, 1240:1, 1244:2, 1248:6, 1174:9, 1176:1, 1180:1, 1184:2, 1188:1, 1192:8, 1196:5, 1199:1, 1203:1, 1207:2, 1211:2, 1215:2, 1219:5, 0:9, 2:2, 6:2, 10:6, 1424:9, 1426:2, 1430:2, 1434:1, 1438:6, 1234:9, 1236:2, 1240:1, 1244:2, 1248:6, 1174:9, 1176:1, 1180:1, 1184:2, 1188:1, 1192:8, 1196:5, 1199:1, 1203:1, 1207:2, 1211:2, 1215:2, 1219:5, 0:9, 2:2, 6:2, 10:6, 1424:9, 1426:2, 1430:2, 1434:1, 1438:6, 1234:9, 1236:2, 1240:1, 1244:2, 1248:6, 1174:9, 1176:1, 1180:1, 1184:2, 1188:1, 1192:8, 1196:5, 1199:1, 1203:1, 1207:2, 1211:2, 1215:2, 1219:5, 0:9, 2:2, 6:2, 10:6, 1424:9, 1426:2, 1430:2, 1434:1, 1438:6, 1234:9, 1236:2, 1240:1, 1244:2, 1248:6, 1174:9, 1176:1, 1180:1, 1184:2, 1188:1, 1192:8, 1196:5, 1199:1, 1203:1, 1207:2, 1211:2, 1215:2, 1219:5, 0:9, 2:2, 6:2, 10:6, 1424:9, 1426:2, 1430:2, 1434:1, 1438:6, 1234:9, 1236:2, 1240:1, 1244:2, 1248:6, 1174:9, 1176:1, 1180:1, 1184:2, 1188:1, 1192:8, 1196:5, 1199:1, 1203:1, 1207:2, 1211:2, 1215:2, 1219:5, 0:9, 2:2, 6:2, 10:6, 1424:9, 1426:2, 1430:2, 1434:1, 1438:6, 1234:9, 1236:2, 1240:1, 1244:2, 1248:6, 1174:9, 1176:1, 1180:1, 1184:2, 1188:1, 1192:8, 1196:5, 1199:1, 1203:1, 1207:2, 1211:2, 1215:2, 1219:5, 0:9, 2:2, 6:2, 10:6, 1424:9, 1426:2, 1430:2, 1434:1, 1438:6, 1234:9, 1236:2, 1240:1, 1244:2, 1248:6, 1174:9, 1176:1, 1180:1, 1184:2, 1188:1, 1192:8, 1196:5, 1199:1, 1203:1, 1207:2, 1211:2, 1215:2, 1219:5, 0:9, 2:2, 6:2, 10:6, 1424:9, 1426:2, 1430:2, 1434:1, 1438:6, 1234:9, 1236:2, 1240:1, 1244:2, 1248:6, 1174:9, 1176:1, 1180:1, 1184:2, 1188:1, 1192:8, 1196:5, 1199:1, 1203:1, 1207:2, 1211:2, 1215:2, 1219:5, 0:9, 2:2, 6:2, 10:6, 1424:9, 1426:2, 1430:2, 1434:1, 1438:6, 1234:9, 1236:2, 1240:1, 1244:2, 1248:6, 1174:9, 1176:1, 1180:1, 1184:2, 1188:1, 1192:8, 1196:5, 1199:1, 1203:1, 1207:2, 1211:2, 1215:2, 1219:5, 0:9, 2:2, 6:2, 10:6, 1424:9, 1426:2, 1430:2, 1434:1, 1438:6, 1234:9, 1236:2, 1240:1, 1244:2, 1248:6, 1174:9, 1176:1, 1180:1, 1184:2, 1188:1, 1192:8, 1196:5, 1199:1, 1203:1, 1207:2, 1211:2, 1215:2, 1219:5, 0:9, 2:2, 6:2, 10:6, 1424:9, 1426:2, 1430:2, 1434:1, 1438:6, 1234:9, 1236:2, 1240:1, 1244:2, 1248:6, 1174:9, 1176:1, 1180:1, 1184:2, 1188:1, 1192:8, 1196:5, 1199:1, 1203:1, 1207:2, 1211:2, 1215:2, 1219:5, 0:9, 2:2, 6:2, 10:6, 1424:9, 1426:2, 1430:2, 1434:1, 1438:6, 1234:9, 1236:2, 1240:1, 1244:2, 1248:6, 1174:9, 1176:1, 1180:1, 1184:2, 1188:1, 1192:8, 1196:5, 1199:1, 1203:1, 1207:2, 1211:2, 1215:2, 1219:5, 0:9, 2:2, 6:2, 10:6, 1424:9, 1426:2, 1430:2, 1434:1, 1438:6, 1234:9, 1236:2, 1240:1, 1244:2, 1248:6, 1174:9, 1176:1, 1180:1, 1184:2, 1188:1, 1192:8, 1196:5, 1199:1, 1203:1, 1207:2, 1211:2, 1215:2, 1219:5, 0:9, 2:2, 6:2, 10:6, 1424:9, 1426:2, 1430:2, 1434:1, 1438:6, 1234:9, 1236:2, 1240:1, 1244:2, 1248:6, 1174:9, 1176:1, 1180:1, 1184:2, 1188:1, 1192:8, 1196:5, 1199:1, 1203:1, 1207:2, 1211:2, 1215:2, 1219:5, 0:9, 2:2, 6:2, 10:6, 1424:9, 1426:2, 1430:2, 1434:1, 1438:6, 1234:9, 1236:2, 1240:1, 1244:2, 1248:6, 1174:9, 1176:1, 1180:1, 1184:2, 1188:1, 1192:8, 1196:5, 1199:1, 1203:1, 1207:2, 1211:2, 1215:2, 1219:5, 0:9, 2:2, 6:2, 10:6, 1424:9, 1426:2, 1430:2, 1434:1, 1438:6, 1234:9, 1236:2, 1240:1, 1244:2, 1248:6, 1174:9, 1176:1, 1180:1, 1184:2, 1188:1, 1192:8, 1196:5, 1199:1, 1203:1, 1207:2, 1211:2, 1215:2, 1219:5, 0:9, 2:2, 6:2, 10:6, 1424:9, 1426:2, 1430:2, 1434:1, 1438:6, 1234:9, 1236:2, 1240:1, 1244:2, 1248:6, 1174:9, 1176:1, 1180:1, 1184:2, 1188:1, 1192:8, 1196:5, 1199:1, 1203:1, 1207:2, 1211:2, 1215:2, 1219:5, 0:9, 2:2, 6:2, 10:6, 1424:9, 1426:2, 1430:2, 1434:1, 1438:6, 1234:9, 1236:2, 1240:1, 1244:2, 1248:6, 1174:9, 1176:1, 1180:1, 1184:2, 1188:1, 1192:8, 1196:5, 1199:1, 1203:1, 1207:2, 1211:2, 1215:2, 1219:5, 0:9, 2:2, 6:2, 10:6, 1424:9, 1426:2, 1430:2, 1434:1, 1438:6, 1234:9, 1236:2, 1240:1, 1244:2, 1248:6, 1174:9, 1176:1, 1180:1, 1184:2, 1188:1, 1192:8, 1196:5, 1199:1, 1203:1, 1207:2, 1211:2, 1215:2, 1219:5, 0:9, 2:2, 6:2, 10:6, 1424:9, 1426:2, 1430:2, 1434:1, 1438:6, 1234:9, 1236:2, 1240:1, 1244:2, 1248:6, 1174:9, 1176:1, 1180:1, 1184:2, 1188:1, 1192:8, 1196:5, 1199:1, 1203:1, 1207:2, 1211:2, 1215:2, 1219:5, 0:9, 2:2, 6:2, 10:6, 1424:9, 1426:2, 1430:2, 1434:1, 1438:6, 1234:9, 1236:2, 1240:1, 1244:2, 1248:6, 1174:9, 1176:1, 1180:1, 1184:2, 1188:1, 1192:8, 1196:5, 1199:1, 1203:1, 1207:2, 1211:2, 1215:2, 1219:5, 0:9, 2:2, 6:2, 10:6, 1424:9, 1426:2, 1430:2, 1434:1, 1438:6, 1234:9, 1236:2, 1240:1, 1244:2, 1248:6, 1174:9, 1176:1, 1180:1, 1184:2, 1188:1, 1192:8, 1196:5, 1199:1, 1203:1, 1207:2, 1211:2, 1215:2, 1219:5, 0:9, 2:2, 6:2, 10:6, 1424:9, 1426:2, 1430:2, 1434:1, 1438:6, 1234:9, 1236:2, 1240:1, 1244:2, 1248:6, 1174:9, 1176:1, 1180:1, 1184:2, 1188:1, 1192:8, 1196:5, 1199:1, 1203:1, 1207:2, 1211:2, 1215:2, 1219:5, 0:9, 2:2, 6:2, 10:6, 1424:9, 1426:2, 1430:2, 1434:1, 1438:6, 1234:9, 1236:2, 1240:1, 1244:2, 1248:6, 1174:9, 1176:1, 1180:1, 1184:2, 1188:1, 1192:8, 1196:5, 1199:1, 1203:1, 1207:2, 1211:2, 1215:2, 1219:5, 0:9, 2:2, 6:2, 10:6, 1424:9, 1426:2, 1430:2, 1434:1, 1438:6, 1234:9, 1236:2, 1240:1, 1244:2, 1248:6, 1174:9, 1176:1, 1180:1, 1184:2, 1188:1, 1192:8, 1196:5, 1199:1, 1203:1, 1207:2, 1211:2, 1215:2, 1219:5, 0:9, 2:2, 6:2, 10:6, 1424:9, 1426:2, 1430:2, 1434:1, 1438:6, 1234:9, 1236:2, 1240:1, 1244:2, 1248:6, 1174:9, 1176:1, 1180:1, 1184:2, 1188:1, 1192:8, 1196:5, 1199:1, 1203:1, 1207:2, 1211:2, 1215:2, 1219:5, 0:9, 2:2, 6:2, 10:6, 1424:9, 1426:2, 1430:2, 1434:1, 1438:6, 1234:9, 1236:2, 1240:1, 1244:2, 1248:6, 1174:9, 1176:1, 1180:1, 1184:2, 1188:1, 1192:8, 1196:5, 1199:1, 1203:1, 1207:2, 1211:2, 1215:2, 1219:5, 0:9, 2:2, 6:2, 10:6, 1424:9, 1426:2, 1430:2, 1434:1, 1438:6, 1234:9, 1236:2, 1240:1, 1244:2, 1248:6, 1174:9, 1176:1, 1180:1, 1184:2, 1188:1, 1192:8, 1196:5, 1199:1, 1203:1, 1207:2, 1211:2, 1215:2, 1219:5, 0:9, 2:2, 6:2, 10:6, 1424:9, 1426:2, 1430:2, 1434:1, 1438:6, 1234:9, 1236:2, 1240:1, 1244:2, 1248:6, 1174:9, 1176:1, 1180:1, 1184:2, 1188:1, 1192:8, 1196:5, 1199:1, 1203:1, 1207:2, 1211:2, 1215:2, 1219:5, 0:9, 2:2, 6:2, 10:6, 1424:9, 1426:2, 1430:2, 1434:1, 1438:6, 1234:9, 1236:2, 1240:1, 1244:2, 1248:6, 1174:9, 1176:1, 1180:1, 1184:2, 1188:1, 1192:8, 1196:5, 1199:1, 1203:1, 1207:2, 1211:2, 1215:2, 1219:5, 0:9, 2:2, 6:2, 10:6, 1424:9, 1426:2, 1430:2, 1434:1, 1438:6, 1234:9, 1236:2, 1240:1, 1244:2, 1248:6, 1174:9, 1176:1, 1180:1, 1184:2, 1188:1, 1192:8, 1196:5, 1199:1, 1203:1, 1207:2, 1211:2, 1215:2, 1219:5, 0:9, 2:2, 6:2, 10:6, 1424:9, 1426:2, 1430:2, 1434:1, 1438:6, 1234:9, 1236:2, 1240:1, 1244:2, 1248:6, 1174:9, 1176:1, 1180:1, 1184:2, 1188:1, 1192:8, 1196:5, 1199:1, 1203:1, 1207:2, 1211:2, 1215:2, 1219:5, 0:9, 2:2, 6:2, 10:6, 1424:9, 1426:2, 1430:2, 1434:1, 1438:6, 1234:9, 1236:2, 1240:1, 1244:2, 1248:6, 1174:9, 1176:1, 1180:1, 1184:2, 1188:1, 1192:8, 1196:5, 1199:1, 1203:1, 1207:2, 1211:2, 1215:2, 1219:5, 0:9, 2:2, 6:2, 10:6, 1424:9, 1426:2, 1430:2, 1434:1, 1438:6, 1234:9, 1236:2, 1240:1, 1244:2, 1248:6, 1174:9, 1176:1, 1180:1, 1184:2, 1188:1, 1192:8, 1196:5, 1199:1, 1203:1, 1207:2, 1211:2, 1215:2, 1219:5, 0:9, 2:2, 6:2, 10:6, 1424:9, 1426:2, 1430:2, 1434:1, 1438:6, 1234:9, 1236:2, 1240:1, 1244:2, 1248:6, 1174:9, 1176:1, 1180:1, 1184:2, 1188:1, 1192:8, 1196:5, 1199:1, 1203:1, 1207:2, 1211:2, 1215:2, 1219:5, 0:9, 2:2, 6:2, 10:6, 1424:9, 1426:2, 1430:2, 1434:1, 1438:6, 1234:9, 1236:2, 1240:1, 1244:2, 1248:6, 1174:9, 1176:1, 1180:1, 1184:2, 1188:1, 1192:8, 1196:5, 1199:1, 1203:1, 1207:2, 1211:2, 1215:2, 1219:5, 0:9, 2:2, 6:2, 10:6, 1424:9, 1426:2, 1430:2, 1434:1, 1438:6, 1234:9, 1236:2, 1240:1, 1244:2, 1248:6, 1174:9, 1176:1, 1180:1, 1184:2, 1188:1, 1192:8, 1196:5, 1199:1, 1203:1, 1207:2, 1211:2, 1215:2, 1219:5, 0:9, 2:2, 6:2, The program '[19044] Lomont.AdventOfCode.exe' has exited with code 3221225786 (0xc000013a).\r\n";

    }
}