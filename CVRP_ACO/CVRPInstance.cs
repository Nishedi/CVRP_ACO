using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CVRP_ACO
{
    public class CVRPInstance
    {
        public string Name { get; set; }
        public int NumberOfTrucks { get; set; }
        public int OptimalValue { get; set; }
        public int Capacity { get; set; }
        public List<Node> Nodes { get; set; }
        public int DepotId { get; set; }
        public double[,] costMatrix { get; set; }

        public CVRPInstance()
        {
            Nodes = new List<Node>();
        }

        public static CVRPInstance LoadFromFile(string filePath)
        {
            var instance = new CVRPInstance();
            string[] lines = File.ReadAllLines(filePath);
            int section = 0;

            foreach (string line in lines)
            {
                if (line.StartsWith("NAME"))
                    instance.Name = line.Split(':')[1].Trim();
                else if (line.StartsWith("COMMENT"))
                {
                    instance.OptimalValue = int.Parse(line.Split(" : ")[1].Split(',')[2].Split(':')[1].Trim().Replace(")", ""));
                }
                    
                else if (line.StartsWith("TYPE"))
                    continue;
                else if (line.StartsWith("DIMENSION"))
                    continue;
                else if (line.StartsWith("EDGE_WEIGHT_TYPE"))
                    continue;
                else if (line.StartsWith("CAPACITY"))
                    instance.Capacity = int.Parse(line.Split(':')[1].Trim());
                else if (line.StartsWith("NODE_COORD_SECTION"))
                    section = 1;
                else if (line.StartsWith("DEMAND_SECTION"))
                    section = 2;
                else if (line.StartsWith("DEPOT_SECTION"))
                    section = 3;
                else if (line.StartsWith("EOF"))
                    break;
                else
                {
                    var parts = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    if (section == 1 && parts.Length == 3)
                    {
                        int id = int.Parse(parts[0]);
                        int x = int.Parse(parts[1]);
                        int y = int.Parse(parts[2]);
                        instance.Nodes.Add(new Node(id, x, y));
                    }
                    else if (section == 2 && parts.Length == 2)
                    {
                        int id = int.Parse(parts[0]);
                        int demand = int.Parse(parts[1]);
                        var node = instance.Nodes.Find(n => n.Id == id);
                        if (node != null)
                            node.Demand = demand;
                    }
                    else if (section == 3 && parts.Length == 1)
                    {
                        int depotId = int.Parse(parts[0]);
                        if (depotId != -1)
                            instance.DepotId = depotId;
                    }
                }
            }

            return instance;
        }

        public Double[,] createDistanceMatrix(List<Node> Nodes)
        {

            double[,] distanceMatrix = new double[Nodes.Count, Nodes.Count];
            for (int i = 0; i < Nodes.Count; i++)
            {
                for (int j = 0; j < Nodes.Count; j++)
                {
                    double deltaX = Nodes[j].X - Nodes[i].X;
                    double deltaY = Nodes[j].Y - Nodes[i].Y;
                    double distance = Math.Sqrt(deltaX * deltaX + deltaY * deltaY);
                    distanceMatrix[i, j] = distance;
                }
            }
            this.costMatrix = distanceMatrix;
            return distanceMatrix;
        }


    }

}
