using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkArchitect
{
    public class Node
    {
        public String Value { get; set; }
        public Node(String v)
        {
            Value = v;
        }
        public override string ToString()
        {
            return Value;
        }
    }

}



 