using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkArchitect
{
    public class Edge
    {
        public Edge(Node f, Node s, int w)
        {
            this.FirstNode = f;
            this.SecondNode = s;
            this.weight = w;
        }
        public Node FirstNode { get; set; }

        public Node SecondNode { get; set; }

        public int weight { get; set; }

        public override string ToString()
        {
            return $"{FirstNode} - {SecondNode} - {weight}";
        }
    }
}
