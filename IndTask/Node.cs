using System.Collections.Generic;

namespace IndTask
{
    class Node
    {
        internal List<Node> children;
        internal List<char> symbols;
        internal int counter;
        internal int startIndex;
        internal int endIndex;

        public Node()
        {
            this.symbols = new List<char>();
            this.children = new List<Node>();
            this.counter = 0;
        }
    }
}
