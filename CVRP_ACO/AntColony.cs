/*using System;
using System.Linq;

public class AntColony
{
    private static Random random = new Random();

    public AntColony() { }

    public double RandomDouble(double min, double max)
    {
        return min + (max - min) * random.NextDouble();
    }

    public int SelectNextCity(int currentCity, bool[] visited, int size,
        double[,] pheromones, int[,] distanceMatrix, double ALPHA, double BETA)
    {
        double total = 0.0;
        double[] probabilities = new double[size];

        for (int i = 0; i < size; i++)
        {
            if (i != currentCity && !visited[i])
            {
                probabilities[i] = Math.Pow(pheromones[currentCity, i], ALPHA) *
                                   Math.Pow(1.0 / distanceMatrix[currentCity, i], BETA);
                total += probabilities[i];
            }
        }

        if (total == 0.0)
        {
            return Array.IndexOf(visited, false); // First unvisited city
        }

        double threshold = RandomDouble(0.0, total);
        double cumulative = 0.0;

        for (int i = 0; i < size; i++)
        {
            if (!visited[i])
            {
                cumulative += probabilities[i];
                if (cumulative >= threshold)
                {
                    return i;
                }
            }
        }

        return 0;
    }

    public void AntColonyOptimization(int[,] distanceMatrix, int size, int NUM_ANTS,
        double ALPHA, double BETA, double RHO, double Q, int maxIterations, int maxTimeACO,
        out int[] bestPath, out int bestCost)
    {
        bestPath = new int[size];
        bestCost = int.MaxValue;
        double[,] pheromones = new double[size, size];

        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                pheromones[i, j] = 1.0;
            }
        }

        int[][] paths = new int[NUM_ANTS][];
        int[] lengths = new int[NUM_ANTS];

        for (int i = 0; i < NUM_ANTS; i++)
        {
            paths[i] = new int[size];
        }

        for (int iteration = 0; iteration < maxIterations; iteration++)
        {
            Console.WriteLine(iteration);
            for (int ant = 0; ant < NUM_ANTS; ant++)
            {
                int currentMassLoad = 0;
                int currentCapacityLoad = 0;
                bool[] visited = new bool[size];
                int startCity = 0;
                paths[ant][0] = startCity;
                visited[startCity] = true;

                for (int step = 1; step < size; step++)
                {
                    int currentCity = paths[ant][step - 1];
                    int nextCity = SelectNextCity(currentCity, visited, size, pheromones, distanceMatrix, ALPHA, BETA);
                    paths[ant][step] = nextCity;
                    visited[nextCity] = true;
                }

                lengths[ant] = CalculateCost(paths[ant], distanceMatrix);
                if (lengths[ant] < bestCost)
                {
                    Console.WriteLine($"Iteracja: {iteration}, Mrówka: {ant} ({lengths[ant]})");
                    bestCost = lengths[ant];
                    Array.Copy(paths[ant], bestPath, size);
                }
            }

            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    pheromones[i, j] *= (1.0 - RHO);
                }
            }

            for (int ant = 0; ant < NUM_ANTS; ant++)
            {
                for (int step = 0; step < size - 1; step++)
                {
                    int from = paths[ant][step];
                    int to = paths[ant][step + 1];
                    pheromones[from, to] += Q / lengths[ant];
                    pheromones[to, from] += Q / lengths[ant];
                }

                int lastFrom = paths[ant][size - 1];
                int lastTo = paths[ant][0];
                pheromones[lastFrom, lastTo] += Q / lengths[ant];
                pheromones[lastTo, lastFrom] += Q / lengths[ant];
            }
        }

        Console.WriteLine("Najlepszy koszt: " + bestCost);
        Console.Write("Najlepsza sciezka: ");
        Console.WriteLine(string.Join(" ", bestPath) + " " + bestPath[0]);
    }

    private int CalculateCost(int[] path, int[,] distanceMatrix)
    {
        int cost = 0;
        for (int i = 0; i < path.Length - 1; i++)
        {
            cost += distanceMatrix[path[i], path[i + 1]];
        }
        cost += distanceMatrix[path[^1], path[0]];
        return cost;
    }
}*/
using System;
using System.Linq;

public class AntColony
{
    private static Random random = new Random();

    public AntColony() { }

    public double RandomDouble(double min, double max)
    {
        return min + (max - min) * random.NextDouble();
    }

    public int SelectNextCity(int currentCity, bool[] visited, int size,
        double[,] pheromones, int[,] distanceMatrix, double ALPHA, double BETA)
    {
        double total = 0.0;
        double[] probabilities = new double[size];

        for (int i = 0; i < size; i++)
        {
            if (i != currentCity && !visited[i])
            {
                probabilities[i] = Math.Pow(pheromones[currentCity, i], ALPHA) *
                                   Math.Pow(1.0 / distanceMatrix[currentCity, i], BETA);
                total += probabilities[i];
            }
        }

        if (total == 0.0)
        {
            return Array.IndexOf(visited, false); // First unvisited city
        }

        double threshold = RandomDouble(0.0, total);
        double cumulative = 0.0;

        for (int i = 0; i < size; i++)
        {
            if (!visited[i])
            {
                cumulative += probabilities[i];
                if (cumulative >= threshold)
                {
                    return i;
                }
            }
        }

        return 0;
    }

    public void AntColonyOptimization(int[,] distanceMatrix, int size, int NUM_ANTS,
        double ALPHA, double BETA, double RHO, double Q, int maxIterations, int maxTimeACO,
        out int[] bestPath, out int bestCost)
    {
        bestPath = new int[size];
        bestCost = int.MaxValue;
        double[,] pheromones = new double[size, size];

        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                pheromones[i, j] = 1.0;
            }
        }

        int[][] paths = new int[NUM_ANTS][];
        int[] lengths = new int[NUM_ANTS];

        for (int i = 0; i < NUM_ANTS; i++)
        {
            paths[i] = new int[size];
        }

        for (int iteration = 0; iteration < maxIterations; iteration++)
        {
            Console.WriteLine(iteration);
            for (int ant = 0; ant < NUM_ANTS; ant++)
            {
                General general = new General();
                int currentMassLoad = 0;
                int currentDimensionLoad = 0;
                bool[] visited = new bool[size];
                int startCity = 0;
                paths[ant][0] = startCity;
                visited[startCity] = true;

                for (int step = 1; step < size; step++)
                {
                    int currentCity = paths[ant][step - 1];
                    int nextCity = 0;
                    if (currentMassLoad+general.cargoMass<=general.truckCapacity && currentDimensionLoad + general.cargoDimension <= general.truckDimension)
                    {
                        nextCity = SelectNextCity(currentCity, visited, size, pheromones, distanceMatrix, ALPHA, BETA);
                        currentMassLoad = currentMassLoad + general.cargoMass;
                        currentDimensionLoad = currentDimensionLoad + general.cargoDimension;
                        
                    }
                    else
                    {
                        currentMassLoad = 0;
                        currentDimensionLoad = 0;
                    }
                    paths[ant][step] = nextCity;
                    
                    visited[nextCity] = true;
                }
                if (paths[ant][^1] == 0 && paths[ant][^2] == 0)
                {
                    var tempList = paths[ant].ToList(); // Konwersja do listy
                    tempList.RemoveAt(tempList.Count - 1); // Usunięcie ostatniego elementu
                    paths[ant] = tempList.ToArray(); // Konwersja z powrotem do tablicy
                }

                lengths[ant] = CalculateCost(paths[ant], distanceMatrix);
                if (lengths[ant] < bestCost)
                {
                    Console.WriteLine($"Iteracja: {iteration}, Mrówka: {ant} ({lengths[ant]})");
                    bestCost = lengths[ant];
                    Array.Copy(paths[ant], bestPath, size);
                }
            }

            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    pheromones[i, j] *= (1.0 - RHO);
                }
            }

            for (int ant = 0; ant < NUM_ANTS; ant++)
            {
                for (int step = 0; step < size - 1; step++)
                {
                    int from = paths[ant][step];
                    int to = paths[ant][step + 1];
                    pheromones[from, to] += Q / lengths[ant];
                    pheromones[to, from] += Q / lengths[ant];
                }

                int lastFrom = paths[ant][size - 1];
                int lastTo = paths[ant][0];
                pheromones[lastFrom, lastTo] += Q / lengths[ant];
                pheromones[lastTo, lastFrom] += Q / lengths[ant];
            }
        }

        Console.WriteLine("Najlepszy koszt: " + bestCost);
        CalculateCost(bestPath, distanceMatrix);
        Console.Write("Najlepsza sciezka: ");
        
        Console.Write(string.Join(" ", bestPath));
        if (bestPath[^1] != 0)
            Console.WriteLine(" "+ bestPath[0]);
    }

    private int CalculateCost(int[] path, int[,] distanceMatrix)
    {
        int cost = 0;
        for (int i = 0; i < path.Length - 1; i++)
        {
            if (path[i] != path[i+1])
                cost += distanceMatrix[path[i], path[i + 1]];
        }
        if (path[^1] != 0)
        {
            
            cost += distanceMatrix[path[^1], path[0]];
        }
            
        return cost;
    }
}