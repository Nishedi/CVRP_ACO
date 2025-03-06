using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CVRP_ACO
{
    public class Node
    {
        public int Id { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Demand { get; set; }

        public Node(int id, int x, int y, int demand = 0)
        {
            Id = id;
            X = x;
            Y = y;
            Demand = demand;
        }
    }

}
