using CVRP_ACO;
using System;
using System.Collections.Generic;
using System.Linq;

public class KnnCVRP
{
    /// <summary>
    /// Solves the CVRP instance using a nearest neighbor heuristic.
    /// </summary>
    /// <param name="cvrp">The CVRP instance containing the nodes, capacity and cost matrix.</param>
    public double KnnSolve(CVRPInstance cvrp)
    {
        int size = cvrp.costMatrix.GetLength(0);
        // Initialize visited array; assume depot (index 0) is always visited.
        bool[] visited = new bool[size];
        visited[0] = true;

        // List to hold all routes (each route is a list of city indices)
        List<List<int>> routes = new List<List<int>>();
        double totalCost = 0.0;

        // Continue while there is at least one unvisited customer (skip depot)
        while (visited.Skip(1).Any(v => !v))
        {
            List<int> route = new List<int>();
            int currentCity = 0;
            int currentLoad = 0;
            route.Add(currentCity);  // Start route at the depot

            // Keep extending the route until no further customer can be added
            while (true)
            {
                int nextCity = -1;
                double bestDistance = double.MaxValue;

                // Check all potential customers (skip depot)
                for (int i = 1; i < size; i++)
                {
                    // Select only those not yet visited and whose demand can be met
                    if (!visited[i] && (currentLoad + cvrp.Nodes[i].Demand <= cvrp.Capacity))
                    {
                        double d = cvrp.costMatrix[currentCity, i];
                        if (d < bestDistance)
                        {
                            bestDistance = d;
                            nextCity = i;
                        }
                    }
                }

                // If no candidate found, finish the current route by returning to the depot.
                if (nextCity == -1)
                {
                    route.Add(0);
                    totalCost += cvrp.costMatrix[currentCity, 0];
                    break;
                }
                else
                {
                    // Extend the route to the chosen next city.
                    route.Add(nextCity);
                    totalCost += bestDistance;
                    visited[nextCity] = true;
                    currentLoad += cvrp.Nodes[nextCity].Demand;
                    currentCity = nextCity;
                }
            }
            routes.Add(route);
        }

        return totalCost;
    }

    /// <summary>
    /// Solves the CVRP instance using a nearest neighbor heuristic combined with a 2-opt local search to improve each route.
    /// </summary>
    /// <param name="cvrp">The CVRP instance containing the nodes, capacity, and cost matrix.</param>
    public double knn2OptSolve(CVRPInstance cvrp)
    {
        int size = cvrp.costMatrix.GetLength(0);
        if (size == 0)
        {
            throw new InvalidOperationException("The cost matrix is empty. Please check your CVRPInstance initialization.");
        }
        bool[] visited = new bool[size];
        visited[0] = true;

        List<List<int>> routes = new List<List<int>>();
        double totalCost = 0.0;

        // Continue until all customers (non-depot nodes) are visited.
        while (visited.Skip(1).Any(v => !v))
        {
            List<int> route = new List<int>();
            int currentCity = 0;
            int currentLoad = 0;
            route.Add(currentCity); // Start at the depot

            // Build the route using the nearest neighbor criterion.
            while (true)
            {
                int nextCity = -1;
                double bestDistance = double.MaxValue;

                // Look for the closest unvisited customer that can be served.
                for (int i = 1; i < size; i++)
                {
                    if (!visited[i] && (currentLoad + cvrp.Nodes[i].Demand <= cvrp.Capacity))
                    {
                        double d = cvrp.costMatrix[currentCity, i];
                        if (d < bestDistance)
                        {
                            bestDistance = d;
                            nextCity = i;
                        }
                    }
                }

                // If no candidate found, complete the route by returning to the depot.
                if (nextCity == -1)
                {
                    route.Add(0);
                    totalCost += cvrp.costMatrix[currentCity, 0];
                    break;
                }
                else
                {
                    route.Add(nextCity);
                    totalCost += bestDistance;
                    visited[nextCity] = true;
                    currentLoad += cvrp.Nodes[nextCity].Demand;
                    currentCity = nextCity;
                }
            }

            // Only try to improve the route if it has a meaningful length.
            if (route.Count >= 4)
            {
                List<int> improvedRoute = TwoOpt(route, cvrp.costMatrix);
                // Adjust total cost: subtract the cost of the original route and add the improved route cost.
                totalCost -= CalculateRouteCost(route, cvrp.costMatrix);
                double improvedCost = CalculateRouteCost(improvedRoute, cvrp.costMatrix);
                totalCost += improvedCost;
                routes.Add(improvedRoute);
            }
            else
            {
                routes.Add(route);
            }
        }
        return totalCost;
    }

    /// <summary>
    /// Applies a 2‑opt local search to improve the given route.
    /// Assumes the route starts and ends with the depot.
    /// </summary>
    private List<int> TwoOpt(List<int> route, double[,] costMatrix)
    {
        // If the route is too short, return it unchanged.
        if (route.Count < 4)
            return new List<int>(route);

        bool improvement = true;
        List<int> bestRoute = new List<int>(route);
        double bestCost = CalculateRouteCost(bestRoute, costMatrix);

        // Continue until no further improvements are found.
        while (improvement)
        {
            improvement = false;
            // i starts at 1 to keep the depot at the beginning fixed,
            // and goes until Count - 3 so that j and j+1 are valid.
            for (int i = 1; i < bestRoute.Count - 2; i++)
            {
                // j goes from i+1 up to Count - 2 (so that j+1 is within bounds)
                for (int j = i + 1; j < bestRoute.Count - 1; j++)
                {
                    List<int> newRoute = TwoOptSwap(bestRoute, i, j);
                    double newCost = CalculateRouteCost(newRoute, costMatrix);
                    if (newCost < bestCost)
                    {
                        bestRoute = new List<int>(newRoute);
                        bestCost = newCost;
                        improvement = true;
                    }
                }
            }
        }
        return bestRoute;
    }

    /// <summary>
    /// Reverses the segment of the route between indices i and j (inclusive).
    /// </summary>
    private List<int> TwoOptSwap(List<int> route, int i, int j)
    {
        List<int> newRoute = new List<int>();

        // Copy route from start to i-1.
        for (int k = 0; k < i; k++)
        {
            newRoute.Add(route[k]);
        }
        // Reverse the segment from i to j.
        for (int k = j; k >= i; k--)
        {
            newRoute.Add(route[k]);
        }
        // Copy the remainder of the route from j+1 to end.
        for (int k = j + 1; k < route.Count; k++)
        {
            newRoute.Add(route[k]);
        }
        return newRoute;
    }

    /// <summary>
    /// Calculates the total cost of a given route.
    /// </summary>
    private double CalculateRouteCost(List<int> route, double[,] costMatrix)
    {
        double cost = 0.0;
        for (int i = 0; i < route.Count - 1; i++)
        {
            cost += costMatrix[route[i], route[i + 1]];
        }
        return cost;
    }

}