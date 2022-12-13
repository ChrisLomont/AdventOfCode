using Lomont.Formats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lomont.AdventOfCode.Utils
{
    class Tree
    {
        /*
         TODO:
          Depth first, breadth first, pre & in & post order, 
           iterable over: descendents, ancestors, siblings, children
           counts over same:..
           map, reduce, fold, 
           add/remove, remove all children, descendants, etc...

           i,j pos of tree in string?

                  *
        */

        public Tree()
        {
            Text = "";
            Payload = "";
        }

        public Tree? Parent { get; set; }

        /// <summary>
        ///  Tree children
        /// </summary>
        public List<Tree> Children { get; } = new();

        /// <summary>
        /// auto parse text for tree, detects brackets [],{}
        /// bottom text put in Payload
        /// string for each piece in Text
        /// </summary>
        /// <param name="text"></param>
        public Tree(string text)
        {
            var r = Parse1(text);
            Children.AddRange(r.Children);
            Text = r.Text;
            Payload = "";
        }

        /// <summary>
        /// Add child, set it's parent to this
        /// </summary>
        /// <param name="child"></param>
        /// <param name="index"></param>
        public void Add(Tree child, int index = -1)
        {
            if (index >= 0)
                Children.Insert(index, child);
            else
                Children.Add(child);
            child.Parent = this;
        }

        public void Dump(TextWriter? output = null)
        {
            if (output == null)
                output = Console.Out;
            TreeTextFormatter.Format(
                output,
                this,
                n => n.Children,
                n => $"Ch: {n.Children.Count}, {n.Text} => {n.Payload}"
            );
        }


        // tag of item
        public string Text { get; set; }

        // payload of this node
        public string Payload { get; set; }

        public override string ToString()
        {
            return Payload;
        }

        // compute most likely open and close chars
        // from [], (), {}, <>
        // and delimiter ',' or ' '
        static (char openChar, char closeChar, char delimeter) ComputeDelims(string line)
        {
            var cc = "[]{}<>()";
            var bestPair = (left: cc[0], right: cc[1]);
            int bestCount = 0;
            for (var i = 0; i < cc.Length; i += 2)
            {
                var (left, right) = (cc[i], cc[i + 1]);
                var c1 = line.Sum(c => c == left ? 1 : 0);
                var c2 = line.Sum(c => c == right ? 1 : 0);
                if (c1 == c2 && c1 > bestCount)
                {
                    bestCount = c1;
                    bestPair = (left, right);
                }

            }
            var delimiterChar = ',';
            return (bestPair.left, bestPair.right, delimiterChar);
        }

        static Tree Parse1(string line)
        {
            var (openChar, closeChar, delimiterChar) = ComputeDelims(line);
            Tree? root = null, cur = null;
            var i = 0;
            var stack = new Stack<(Tree t, int start)>();
            var ctrlTxt = openChar.ToString() + closeChar + delimiterChar;
            while (i < line.Length)
            {
                var c = line[i];
                if (c == openChar)
                {
                    var p = new Tree();
                    if (root == null)
                        cur = root = p;
                    else
                        cur.Add(p);
                    stack.Push((cur, i));
                    cur = p;

                }
                else if (c == closeChar)
                {
                    // todo - save substring?
                    int k;
                    (cur, k) = stack.Pop();
                    cur.Text = line[k..(i + 1)];
                }
                else if (c == delimiterChar)
                {
                    // do nothing
                }
                else
                {
                    // consume, set current node text
                    // todo - currently a child node?
                    var k = i;
                    while (!ctrlTxt.Contains(line[i]))
                        ++i;
                    //cur.Payload = line[k..i];
                    var ch = new Tree { Payload = line[k..i] };
                    cur.Add(ch);

                    i--; // one too many...
                    //Console.WriteLine($"Payload {cur.Payload}");
                }

                ++i; // next

            }

            return root;
        }
    }
}
