using System;
using System.Collections.Generic;
using System.Linq;

namespace Huffman
{
    class Program
    {
        static void Main(string[] args)
        {
            var input = @"
A:10
E:15
I:12
S:3
T:4
P:13
\n:1
";
            var chars = input.Split("\n");

            var charFreqs = new SortedDictionary<string, int>();

            foreach (var ch in chars)
            {
                if (ch.Trim().Length >= 3)
                {
                    var kv = ch.Split(":");
                    charFreqs.Add(kv[0], int.Parse(kv[1]));
                }
            }

            var strategy = 0;
            // 0 = need to save current char for next iteration
            // 1 = need to create new subtree with current and prev chars
            Node prevNode = null;

            KeyValuePair<string, int>? prevCharFreq = null;
            var leafs = new List<Node>();
            var subtrees = new List<Node>();

            var charsFreqs = charFreqs.OrderBy(kv => kv.Value).ToList();

            for (int i = 0; i <= charsFreqs.Count; i++)
            {
                if (i < charsFreqs.Count)
                {
                    var charFreq = charsFreqs[i];

                    if (strategy == 0)
                    {
                        prevCharFreq = charFreq;
                        strategy = 1;
                        continue;
                    }

                    if (strategy == 1 && prevCharFreq != null)
                    {
                        // choose 2 children and create parent node for them
                        if (prevNode != null)
                        {
                            subtrees.Add(prevNode); // save before overwrite
                        }

                        prevNode = new Node
                        {
                            freq = charFreq.Value + prevCharFreq.Value.Value
                        };

                        var rightNode = new Node
                        {
                            freq = charFreq.Value,
                            prefix = "1",
                            parent = prevNode,
                            ch = charFreq.Key
                        };

                        var leftNode = new Node
                        {
                            freq = prevCharFreq.Value.Value,
                            prefix = "0",
                            parent = prevNode,
                            ch = prevCharFreq.Value.Key
                        };

                        leafs.Add(leftNode);
                        leafs.Add(rightNode);

                        strategy = 2;
                        continue;
                    }

                    if (strategy == 2)
                    {
                        if (prevNode != null && prevNode.freq <= charFreq.Value)
                        {
                            var newParentNode = new Node
                            {
                                freq = charFreq.Value + prevNode.freq,
                            };

                            prevNode.parent = newParentNode;
                            prevNode.prefix = "0";
                            prevNode = newParentNode;

                            var newNode = new Node
                            {
                                parent = newParentNode,
                                prefix = "1",
                                ch = charFreq.Key,
                                freq = charFreq.Value
                            };
                            leafs.Add(newNode);
                        }
                        else
                        {
                            prevCharFreq = charFreq;
                            strategy = 1;
                            continue;
                        }

                        continue;
                    }
                }
                else
                {
                    // last iteration
                    if (prevCharFreq != null && subtrees.Count > 0)
                    {
                        var subtree = subtrees[0];
                        var newParent = new Node
                        {
                            freq = subtree.freq + prevCharFreq.Value.Value
                        };
                        subtree.parent = newParent;
                        subtree.prefix = "1";

                        var leftChild = new Node
                        {
                            parent = newParent,
                            freq = prevCharFreq.Value.Value,
                            prefix = "0",
                            ch = prevCharFreq.Value.Key
                        };
                        leafs.Add(leftChild);
                        subtrees.Remove(subtree);
                        if (prevNode != null)
                        {
                            var root = new Node
                            {
                                freq = prevNode.freq + newParent.freq
                            };

                            Node l;
                            Node r;
                            if (prevNode.freq < newParent.freq)
                            {
                                l = prevNode;
                                r = newParent;
                            }
                            else
                            {
                                l = newParent;
                                r = prevNode;
                            }

                            l.parent = root;
                            l.prefix = "0";

                            r.parent = root;
                            r.prefix = "1";
                        }
                    }
                }
            }


            foreach (var leaf in leafs)
            {
                Console.WriteLine($"{leaf.ch} = {leaf.GetPrefixCode()} ({leaf.freq})");
            }
        }
    }

    class Node
    {
        public Node parent { get; set; }

        public string prefix { get; set; }

        public string ch { get; set; }

        public int freq { get; set; }

        public string GetPrefixCode()
        {
            string pc = "";

            if (parent != null)
            {
                pc += parent.GetPrefixCode();
            }

            return pc + prefix;
        }
    }
}