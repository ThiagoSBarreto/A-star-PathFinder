using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A_start_PathFinder
{
    public class Node
    {
        public int x { get; set; }
        public int y { get; set; }
        public int f { get; set; }
        public int g { get; set; }
        public int g2 { get; set; }
        public int h { get; set; }
        public int c { get; set; }
        public bool impassable { get; set; }
        public Node p { get; set; }

        public List<Node> children = new List<Node>();

        public bool sNode { get; set; }
        public bool fNode { get; set; }

        public Node()
        {
        }
    }
}
