using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkArchitect
{
    public class Network
    {
        static List<List<Edge>> AllNetworks = new List<List<Edge>>();

        static List<Edge> Networks = new List<Edge>();
        static List<Node> AllNodes = new List<Node>();
        static List<Tuple<String, String>> UsedNodes = new List<Tuple<String, String>>();

        public static void FindConnections()
        {
            List<String> Visited = new List<String>();
            String tempValue = "";
            for (int i = 0; i < Networks.Count; i++)
            {
                if (i == 0)
                {
                    tempValue = Networks.ElementAt(0).FirstNode.Value;
                    Visited.Add(tempValue);
                }
                else
                {
                    if (Visited.Contains(Networks.ElementAt(i).FirstNode.Value) &&
                        !Visited.Contains(Networks.ElementAt(i).SecondNode.Value))
                    {

                        Visited.Add(Networks.ElementAt(i).SecondNode.Value);
                        tempValue = Networks.ElementAt(i).SecondNode.Value;

                    }
                    if (Visited.Contains(Networks.ElementAt(i).SecondNode.Value) &&
                        !Visited.Contains(Networks.ElementAt(i).FirstNode.Value))
                    {

                        Visited.Add(Networks.ElementAt(i).FirstNode.Value);
                        tempValue = Networks.ElementAt(i).FirstNode.Value;

                    }
                }
            }

            List<Edge> tempNetwork = new List<Edge>();
            for (int i = 0; i < Networks.Count; i++)
            {
                if (Visited.Contains(Networks.ElementAt(i).FirstNode.Value) || Visited.Contains(Networks.ElementAt(i).SecondNode.Value))
                {
                    if (!tempNetwork.Contains(Networks.ElementAt(i)))
                    {
                        tempNetwork.Add(Networks.ElementAt(i));
                    }
                }
            }
            AllNetworks.Add(tempNetwork);

            List<String> allVals = new List<String>();
            foreach (var c in tempNetwork)
            {
                allVals.Add(c.FirstNode.Value);
                allVals.Add(c.SecondNode.Value);
            }

            List<Edge> superTemp = new List<Edge>(Networks);
            for (int i = 0; i < Networks.Count; i++)
            {
                if (allVals.Contains(Networks.ElementAt(i).FirstNode.Value) || allVals.Contains(Networks.ElementAt(i).SecondNode.Value))
                {
                    superTemp.Remove(Networks.ElementAt(i));
                }
            }
            Networks = new List<Edge>(superTemp);
            if (Networks.Count != 0)
                FindConnections();
        }

        public static void GetHub(List<Edge> cnet)
        {
            int difference = -1,
                leftTotal,
                rightTotal,
                finalLeft = -1,
                finalRight = -1;
            Edge optimalLoc = null;
            List<Node> nodes = new List<Node>();

            foreach (var con in cnet)
            {
                if (!nodes.Contains(con.FirstNode))
                {
                    nodes.Add(con.FirstNode);
                }
                if (!nodes.Contains(con.SecondNode))
                {
                    nodes.Add(con.SecondNode);
                }
            }
            for (int i = 0; i < cnet.Count; i++)
            {
                if (i == 0)
                {
                    difference = TotalLength(cnet);
                }

                leftTotal = 0;
                rightTotal = 0;
                for (int j = 0; j < i; j++)
                {
                    leftTotal += cnet[j].weight;
                }
                for (int j = i; j < cnet.Count; j++)
                {
                    rightTotal += cnet[j].weight;
                }
                if (Math.Abs(leftTotal - rightTotal) < difference)
                {
                    difference = Math.Abs(leftTotal - rightTotal);
                    optimalLoc = cnet[i];
                    finalLeft = leftTotal;
                    finalRight = rightTotal;
                }
            }

            Console.WriteLine($"Optimal Hub Placement: {optimalLoc.FirstNode} ({finalLeft}:{finalRight})");
        }

        public static int TotalLength(List<Edge> cnet)
        {
            int result = 0;

            foreach (var c in cnet)
            {
                result += c.weight;
            }

            return result;
        }

        public static void Print(List<Edge> cnet)
        {
            List<String> allNodeNames = new List<String>();
            foreach (var c in cnet)
            {
                if (!allNodeNames.Contains(c.FirstNode.Value))
                {
                    allNodeNames.Add(c.FirstNode.Value);
                }
                if (!allNodeNames.Contains(c.SecondNode.Value))
                {
                    allNodeNames.Add(c.SecondNode.Value);
                }
            }

            String sSet = "Socket Set: ";
            foreach (var n in allNodeNames)
            {
                sSet += (n + ", ");
            }
            sSet = sSet.Substring(0, sSet.Length - 2);
            Console.WriteLine(sSet);
            Console.WriteLine("Cable needed: " + TotalLength(cnet) + " units");
        }

        public static void InitiateConsAndNodes(String[] lines)
        {
            for (int i = 0; i < lines.Length; i++)
            {
                if (i == 0)
                {
                    String[] newNodes = lines[i].Split(',');
                    foreach (var s in newNodes)
                    {
                        AllNodes.Add(new Node(s));
                    }
                }
                else
                {
                    String[] newCons = lines[i].Split(',');
                    for (int j = 1; j < newCons.Length; j++)
                    {
                        String[] temp = newCons[j].Split(':');
                        Node n1 = new Node(newCons[0]);
                        Node n2 = new Node(temp[0]);
                        if (HasCycle(n1.Value, n2.Value))
                        {
                            Networks.Add(new Edge(n1,
                                                       n2,
                                                       int.Parse(temp[1])
                                                       ));
                            UsedNodes.Add(new Tuple<string, string>(n1.Value, n2.Value));
                        }
                    }
                }
            }
        }

        public static List<Edge> Kruskal(List<Edge> cons)
        {
            var tempConnections = cons.OrderBy(c => c.weight);
            List<Edge> newCons = new List<Edge>();
            List<Node> nodes = new List<Node>();

            foreach (var con in tempConnections)
            {
                if (!nodes.Contains(con.FirstNode))
                {
                    nodes.Add(con.FirstNode);
                }
                if (!nodes.Contains(con.SecondNode))
                {
                    nodes.Add(con.SecondNode);
                }
            }
            foreach (var n in nodes)
            {
                var conToFirst = tempConnections.Where(c => c.FirstNode.Value == n.Value || c.SecondNode.Value == n.Value);

                conToFirst.OrderBy(c => c.weight);
                if (!newCons.Contains(conToFirst.First()))
                {
                    newCons.Add(conToFirst.First());
                }
            }
            return newCons;
        }

        private static bool HasCycle(String v1, String v2)
        {
            for (int i = 0; i < UsedNodes.Count; i++)
            {
                Tuple<String, String> t = UsedNodes[i];
                if ((t.Item1 == v1 && t.Item2 == v2) || (t.Item1 == v2 && t.Item2 == v1))
                {
                    return false;
                }
            }
            return true;
        }

        public static String ReadFile(String filepath)
        {
            return System.IO.File.ReadAllText(filepath);
        }

        static void Main(string[] args)
        {
            Console.Write("Enter the file: ");
            String filePath = Console.ReadLine();
            String allText = ReadFile(filePath);
            String[] lines = allText.Split(new String[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
            InitiateConsAndNodes(lines);

            FindConnections();
            int allNetworkLengths = 0;
            foreach (var net in AllNetworks)
            {
                Console.WriteLine("Network");
                Print(Kruskal(net));
                GetHub(Kruskal(net));
                allNetworkLengths += TotalLength(Kruskal(net));
                Console.WriteLine("");
            }
            Console.WriteLine("Total Length: " + allNetworkLengths + " units");
        }
    }
}









/*
 * MST 1:
 * Socket Set: AX1, AX4, AX2, AX3, AX5
 * Cable Needed: 24ft
 * Optimal Hub Placement: AX4 (9-15)
 * 
 * MST 2:
 * Socket Set: AX10, AX11, AX12, AX99, AX100
 * Cable Needed: 23ft
 * Optimal Hub Placement: AX99 (8-15)
 * 
 * Total Cable Required: 47ft
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * AX10,AX11:2,AX12:4
AX11,AX10:2,AX12:2
AX12,AX10:4,AX11:2,AX99:4
AX99,AX12:4,AX100:15
AX100,AX99:15
 */






