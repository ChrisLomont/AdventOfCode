namespace Lomont.AdventOfCode._2021
{
    internal class Day23 : AdventOfCode
    {
        /*
#############
#...........#
###B#C#B#D###
  #A#D#C#A#
  #########

        model as hallway and 4 columns
        columns work sort of like a stack and asnwer questions to make code easier

#############
#0123456789X#  hallway 0-10 index
###A#B#C#D###
  #A#B#C#D#
  #########

         */

        class Column
        {
            public int exit; // value when exits column
            public int desired; // 1,2,3,4 = A,B,C,D

            public Column(int size)
            {
                vals = new int[size];
                this.size = size;
            }
            public int[] vals;

            int size;

            // can enter if no wrong pieces in here
            // return steps to place piece
            public (bool success, int steps) CanEnter()
            {
                var ok = vals.All(p => p == 0 || p == desired);
                if (!ok) return (false, 0);

                var steps = 0;
                var i = size - 1;
                while (i >= 0)
                {
                    ++steps;
                    if (vals[i] == 0)
                        return (true, steps); 
                    i--;
                }
                throw new Exception();
            }

            // see if item can be taken, and if so, cost to exit to exit #
            public (bool success, int steps,int piece) CanPop()
            {
                var ok = vals.Any(p => p != desired && p != 0);
                if (!ok) return (false, 0,0);
                for (var i = 0; i < size; ++i)
                {
                    if (vals[i] != 0)
                        return (true, i + 1, vals[i]);
                }

                throw new Exception();
            }


            // get next item, 0 if none
            // moves is how many steps to move out
            public (int val, int moves) Take()
            {
                for (var i =0; i < size; ++i)
                    if (vals[i] != 0)
                    {
                        var v = vals[i];
                        vals[i] = 0;
                        return (v, i+1);
                    }

                throw new Exception(); // should not hit this
            }

            // fill from back
            public void Put(int val)
            {
                var i = size - 1;
                while (i >= 0)
                {
                    if (vals[i] == 0)
                    {
                        vals[i] = val;
                        return;
                    }
                    i--;
                }

                throw new Exception(); // should not hit
            }

            public bool Done => vals.All(v => v == desired);

            public override string ToString()
            {
                string s = "";
                foreach (var v in vals)
                    s += v==0?'.': (char)('A'+v-1);
                return s;
            }
        }

        class Board
        {
            public Board(int depth)
            {
                cols = new Column[4];

                cols[0] = new Column(depth) { exit = 2, desired = 1 };
                cols[1] = new Column(depth) { exit = 4, desired = 2 };
                cols[2] = new Column(depth) { exit = 6, desired = 3 };
                cols[3] = new Column(depth) { exit = 8, desired = 4 };
            }

            public Column[] cols;
            public int [] hall = new int[11];

            public bool Done => cols.All(c => c.Done);


            // 0-11 are hall, 
            // 20-23 is column 0-3
            public void DoMove(int src, int dst)
            {
                var p = Get(src);
                Set(dst, p);
            }

            // pull piece out
            int Get(int pos)
            {
                if (pos <= hall.Length)
                {
                    var v = hall[pos];
                    hall[pos] = 0;
                    return v;
                }

                var (piece,_) = cols[pos - 20].Take();
                return piece;
            }

            void Set(int pos, int val)
            {
                if (pos <= hall.Length)
                    hall[pos] = val;
                else
                    cols[pos - 20].Put(val);
            }

        }

        public override object Run(bool part2)
        {
            // memoize states, clear each run
            memos = new Dictionary<ulong, long>();


            int depth = part2?4:2;

            var board = new Board(depth);

            // mine CCAADBBD

            // pieces, in each column, left to right, each col bottom to top
            var p = "ABDCCBAD"; // example -> 11536
            p = "AABBCCDD"; // score 0
            p = "ABBACCDD"; // score 46
            p = "AABBCDDC"; // score 4600
            p = "BABACCDD"; // score 59
            p = "CCAADBBD"; // mine 11536
            if (part2)
                p = "CDDCABCADABBBCAD";//55136
            Set(p);

            // List<(int src, int dst, int cost)> solution = new();

            return MaxEnergy(board);

            void Set(string s)
            {
                var per = s.Length / board.cols.Length;
                for (var i = 0; i < s.Length; ++i)
                    board.cols[i/per].Put(s[i]-'A'+1);
            }
        }

        // memoize states
        Dictionary<ulong, long> memos = new Dictionary<ulong, long>();

        // compute max energy from current board state
        long MaxEnergy(Board board, int lastMove = -1, int depth = 0)
        {
            if (board.Done)
                return 0; // cost 0 from here

            var state = GetState(board);
            if (!memos.ContainsKey(state))
            {
                long bestScore = Int64.MaxValue / 2;
                var moves = GenMoves(board);
                //if (moves.Count == 0)
                //    moves = GenMoves(board); // todo - debug
                //Trace.Assert(moves.Count > 0);
                foreach (var m in moves)
                {
                    var mv = m.src * 100 + m.dst;
                    var inv = m.dst * 100 + m.src;
                    if (inv == lastMove) continue; // avoid back and forth moves

                    // do move
                    board.DoMove(m.src, m.dst);


                    var curCost = m.cost + MaxEnergy(board, mv, depth + 1);
                    if (curCost < bestScore)
                        bestScore = curCost;

                    // undo move
                    board.DoMove(m.dst, m.src);

                    //Console.WriteLine($"{depth}: {m.src}({(char)(board.Get(m.src)+'A'-1)}) -> {m.dst} {bestScore}");
                }


                memos[state] = bestScore;
            }

            return memos[state];

        }

        ulong GetState(Board board)
        {
            // each cell 0,1,2,3,4
            // 0-10 hall = 11 items
            // 4 cols, 2 or 4 deep each, is 8 or 16 more cells
            // 27 cells at most, 5 each, just fits in 64 bits
            var state = 0UL;
            foreach (var c in board.hall)
                state = 5 * state + (ulong)c;
            foreach (var col in board.cols)
            foreach (var c in col.vals)
                state = 5 * state + (ulong)c;
            return state;
        }


        int[] costs = new[] { 0, 1, 10, 100, 1000 };
        int[] dests = new []{ 0, 1, 3, 5, 7, 9, 10 };

        List<(int src, int dst, int cost)> GenMoves(Board board)
        {
            /*
             pods move.
             1. pods move from column (if it is not theirs or a wrong type is deeper in), to column exit square, to square in hall {0,1,3,5,7,9,10}
             2. items move from hall back into correct column, if no wrong types in it
             */
            List<(int, int, int)> moves = new();
            foreach (var d in dests)
            {
                var p = board.hall[d];
                if (p != 0)
                {
                    // move into column
                    var col = board.cols[p - 1];
                    var (success1, steps1) = col.CanEnter();
                    if (success1)
                    {
                        var (success2, steps2) = Clear(d, col.exit, true);
                        if (success2)
                            moves.Add(new(d, 20 + p - 1, costs[p] * (steps1 + steps2)));
                    }

                }
                else
                { // move each column out to p if possible
                    for (var i = 0; i < 4; ++i)
                    {
                        var col = board.cols[i];
                        var (success1,steps1,piece) = col.CanPop();
                        if (success1)
                        {
                            var (success2, steps2) = Clear(col.exit,d,false);
                            if (success2)
                                moves.Add(new(20 + i, d, costs[piece] * (steps1 + steps2)));
                        }
                    }
                    
                }
            }

            return moves;

            // are squares a to b clear (inclusive)
            (bool ok, int steps) Clear(int a, int b, bool excludeFirst)
            {
                if (excludeFirst)
                    a += Math.Sign(b - a);
                (a, b) = (Math.Min(a,b), Math.Max(a,b));
                for (var i = a; i <= b; ++i)
                    if (board.hall[i] != 0)
                        return (false, 0);
                return (true, b - a + (excludeFirst?1:0));
            }


        }

    }
}