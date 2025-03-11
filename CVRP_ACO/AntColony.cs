using CVRP_ACO;
using System;
using System.Linq;
using System.Threading.Tasks;

public class AntColony
{
    private static Random random = new Random();

    public AntColony() { }

    public double RandomDouble(double min, double max)
    {
        return min + (max - min) * random.NextDouble();
    }

    public int SelectNextCity(int currentCity, bool[] visited, int size,
        double[,] pheromones, double[,] distanceMatrix, double ALPHA, double BETA, CVRPInstance cvrp, int truckLoad)
    {
        double total = 0.0;
        double[] probabilities = new double[size];



        for (int i = 0; i < size; i++)
        {
            if (i != currentCity && !visited[i] && cvrp.Nodes[i].Demand + truckLoad <= cvrp.Capacity)
            {
                probabilities[i] = Math.Pow(pheromones[currentCity, i], ALPHA) *
                                   Math.Pow(1.0 / distanceMatrix[currentCity, i], BETA);
                total += probabilities[i];
            }
        }


        if (total == 0.0 && cvrp.Nodes[Array.IndexOf(visited, false)].Demand + truckLoad > cvrp.Capacity)
        {

            return 0;
            //return Array.IndexOf(visited, false); // First unvisited city
        }
        else if (total == 0.0 && cvrp.Nodes[Array.IndexOf(visited, false)].Demand + truckLoad <= cvrp.Capacity)
        {
            return Array.IndexOf(visited, false);
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
        return Array.IndexOf(visited, false);
        return 0;
    }

    public void AntColonyOptimization(CVRPInstance cvrp,
        double ALPHA, double BETA, double RHO, double Q, int maxIterations, int maxTimeACO,
        out int[] bestPath, out double bestCost)
    {
        double[,] distanceMatrix = cvrp.costMatrix;
        int size = cvrp.costMatrix.GetLength(0);
        int NUM_ANTS = size;
        bestPath = new int[size * 2];
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
        double[] lengths = new double[NUM_ANTS];

        for (int i = 0; i < NUM_ANTS; i++)
        {
            paths[i] = new int[size * 2];
        }

        for (int iteration = 0; iteration < maxIterations; iteration++)
        {
            for (int ant = 0; ant < NUM_ANTS; ant++)
            {
                int capacity = 0;

                bool[] visited = new bool[size];
                int startCity = 0;
                paths[ant][0] = startCity;
                visited[startCity] = true;

                for (int step = 1; visited.Contains(false); step++)
                {
                    int currentCity = paths[ant][step - 1];
                    int nextCity = SelectNextCity(currentCity, visited, size, pheromones, distanceMatrix, ALPHA, BETA, cvrp, capacity);
                    if (nextCity != 0)
                        capacity = capacity + cvrp.Nodes[nextCity].Demand;
                    else
                    {
                        capacity = 0;
                    }
                    paths[ant][step] = nextCity;
                    visited[nextCity] = true;
                }
                /*if (paths[ant][^1] == 0 && paths[ant][^2] == 0)
                {
                    var tempList = paths[ant].ToList(); // Konwersja do listy
                    tempList.RemoveAt(tempList.Count - 1); // Usunięcie ostatniego elementu
                    paths[ant] = tempList.ToArray(); // Konwersja z powrotem do tablicy
                }*/

                lengths[ant] = CalculateCost(paths[ant], distanceMatrix);
                if (lengths[ant] < bestCost)
                {
                    Console.WriteLine($"Iteracja: {iteration}, Mrówka: {ant} ({lengths[ant]})");
                    bestCost = lengths[ant];
                    Array.Copy(paths[ant], bestPath, paths[ant].Length);
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
                for (int step = 0; step < size - 1; step++)// chyba do zmiany
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

        Console.WriteLine("Najlepszy koszt: " + bestCost + "/" + cvrp.OptimalValue);
        CalculateCost(bestPath, distanceMatrix);
        Console.Write("Najlepsza sciezka:\n ");

        Console.Write(string.Join(" ", bestPath));
        if (bestPath[^1] != 0)
            Console.WriteLine(" " + bestPath[0]);

    }




    public void AntColonyOptimizationPararrel(CVRPInstance cvrp,
        double ALPHA, double BETA, double RHO, double Q, int maxIterations, int maxTimeACO,
        out int[] bestPath, out double bestCost)
    {
        double[,] distanceMatrix = cvrp.costMatrix;
        int size = cvrp.costMatrix.GetLength(0);
        int NUM_ANTS = size;
        bestPath = new int[size * 2];
        bestCost = double.MaxValue;
        double[,] pheromones = new double[size, size];

        for (int i = 0; i < size; i++)
            for (int j = 0; j < size; j++)
                pheromones[i, j] = 1.0;

        int[][] paths = new int[NUM_ANTS][];
        double[] lengths = new double[NUM_ANTS];

        for (int i = 0; i < NUM_ANTS; i++)
            paths[i] = new int[size * 2];

        object lockObject = new object();

        for (int iteration = 0; iteration < maxIterations; iteration++)
        {
            // Track the best cost in the current iteration locally for each ant
            double localBestCost = double.MaxValue;
            int[] localBestPath = new int[size * 2];

            Parallel.For(0, NUM_ANTS, ant =>
            {
                int capacity = 0;
                bool[] visited = new bool[size];
                int startCity = 0;
                paths[ant][0] = startCity;
                visited[startCity] = true;

                for (int step = 1; visited.Contains(false); step++)
                {
                    int currentCity = paths[ant][step - 1];
                    int nextCity = SelectNextCity(currentCity, visited, size, pheromones, distanceMatrix, ALPHA, BETA, cvrp, capacity);
                    capacity = (nextCity != 0) ? capacity + cvrp.Nodes[nextCity].Demand : 0;
                    paths[ant][step] = nextCity;
                    visited[nextCity] = true;
                }

                lengths[ant] = CalculateCost(paths[ant], distanceMatrix);

                // Check if this ant's solution is the best so far in this iteration
                if (lengths[ant] < localBestCost)
                {
                    localBestCost = lengths[ant];
                    Array.Copy(paths[ant], localBestPath, paths[ant].Length);
                }
            });

            // After the parallel execution, update the global best cost and path
            lock (lockObject)
            {
                if (localBestCost < bestCost)
                {
                    Console.WriteLine($"Iteracja: {iteration}, Najlepszy koszt: {localBestCost}");
                    bestCost = localBestCost;
                    Array.Copy(localBestPath, bestPath, localBestPath.Length);
                }
            }

            // Update pheromones
            Parallel.For(0, size, i =>
            {
                for (int j = 0; j < size; j++)
                {
                    pheromones[i, j] *= (1.0 - RHO);
                }
            });

            Parallel.For(0, NUM_ANTS, ant =>
            {
                for (int step = 0; step < size - 1; step++)
                {
                    int from = paths[ant][step];
                    int to = paths[ant][step + 1];
                    lock (lockObject)
                    {
                        pheromones[from, to] += Q / lengths[ant];
                        pheromones[to, from] += Q / lengths[ant];
                    }
                }

                int lastFrom = paths[ant][size - 1];
                int lastTo = paths[ant][0];
                lock (lockObject)
                {
                    pheromones[lastFrom, lastTo] += Q / lengths[ant];
                    pheromones[lastTo, lastFrom] += Q / lengths[ant];
                }
            });
        }

        Console.WriteLine("Najlepszy koszt: " + bestCost + "/" + cvrp.OptimalValue);
        Console.WriteLine("x" + (bestCost - cvrp.OptimalValue) / cvrp.OptimalValue);

        CalculateCost(bestPath, distanceMatrix);
        Console.Write("Najlepsza ścieżka:\n ");
        Console.Write(string.Join(" ", bestPath));
        if (bestPath[^1] != 0)
            Console.WriteLine(" " + bestPath[0]);
    }



    public void AntColonyOptimizationPararrelv2(CVRPInstance cvrp,
        double ALPHA, double BETA, double RHO, double Q, int maxIterations, int maxTimeACO,
        out int[] bestPath, out double bestCost)
    {
        double[,] distanceMatrix = cvrp.costMatrix;
        int size = cvrp.costMatrix.GetLength(0);
        int NUM_ANTS = size;
        bestPath = new int[size * 2];
        bestCost = double.MaxValue;
        double[,] pheromones = new double[size, size];

        // Initialize pheromones
        for (int i = 0; i < size; i++)
            for (int j = 0; j < size; j++)
                pheromones[i, j] = 1.0;

        object lockObject = new object();

        for (int iteration = 0; iteration < maxIterations; iteration++)  // Sequential iterations
        {
            int[][] paths = new int[NUM_ANTS][];
            double[] lengths = new double[NUM_ANTS];
            double localBestCost = double.MaxValue;
            int[] localBestPath = new int[size * 2];

            for (int i = 0; i < NUM_ANTS; i++)
                paths[i] = new int[size * 2];

            Parallel.For(0, NUM_ANTS, ant =>
            {
                int capacity = 0;
                bool[] visited = new bool[size];
                int startCity = 0;
                paths[ant][0] = startCity;
                visited[startCity] = true;

                for (int step = 1; visited.Contains(false); step++)
                {
                    int currentCity = paths[ant][step - 1];
                    int nextCity = SelectNextCity(currentCity, visited, size, pheromones, distanceMatrix, ALPHA, BETA, cvrp, capacity);
                    capacity = (nextCity != 0) ? capacity + cvrp.Nodes[nextCity].Demand : 0;
                    paths[ant][step] = nextCity;
                    visited[nextCity] = true;
                }

                lengths[ant] = CalculateCost(paths[ant], distanceMatrix);

                // Update local best cost
                if (lengths[ant] < localBestCost)
                {
                    localBestCost = lengths[ant];
                    Array.Copy(paths[ant], localBestPath, paths[ant].Length);
                }
            });

            // Update global best cost outside parallel region
            lock (lockObject)
            {
                if (localBestCost < bestCost)
                {
                    // Console.WriteLine($"Iteracja: {iteration}, Najlepszy koszt: {localBestCost}");
                    bestCost = localBestCost;
                    Array.Copy(localBestPath, bestPath, localBestPath.Length);
                }
            }

            // Update pheromones sequentially
            for (int i = 0; i < size; i++)
                for (int j = 0; j < size; j++)
                    pheromones[i, j] *= (1.0 - RHO);  // Evaporation

            Parallel.For(0, NUM_ANTS, ant =>
            {
                for (int step = 0; step < size - 1; step++)
                {
                    int from = paths[ant][step];
                    int to = paths[ant][step + 1];
                    lock (lockObject)
                    {
                        pheromones[from, to] += Q / lengths[ant];
                        pheromones[to, from] += Q / lengths[ant];
                    }
                }

                int lastFrom = paths[ant][size - 1];
                int lastTo = paths[ant][0];
                lock (lockObject)
                {
                    pheromones[lastFrom, lastTo] += Q / lengths[ant];
                    pheromones[lastTo, lastFrom] += Q / lengths[ant];
                }
            });
        }

        Console.WriteLine("Najlepszy koszt: " + bestCost + "/" + cvrp.OptimalValue);
        CalculateCost(bestPath, distanceMatrix);
        Console.WriteLine("x" + (bestCost - cvrp.OptimalValue) / cvrp.OptimalValue);

        // Console.Write("Najlepsza ścieżka:\n ");
        // Console.Write(string.Join(" ", bestPath));
        // if (bestPath[^1] != 0)
        //     Console.WriteLine(" " + bestPath[0]);
    }




    private double CalculateCost(int[] path, double[,] distanceMatrix)
    {
        double cost = 0;
        for (int i = 0; i < path.Length - 1; i++)
        {
            if (path[i] != path[i + 1])
                cost += distanceMatrix[path[i], path[i + 1]];
        }
        if (path[^1] != 0)
        {

            cost += distanceMatrix[path[^1], path[0]];
        }

        return cost;
    }


    // -------------------------------
    // Softmax Tuning Algorithm Below
    // -------------------------------

    /// <summary>
    /// https://www.diva-portal.org/smash/get/diva2:1599698/FULLTEXT01.pdf
    /// Tunes the ACO parameters using a softmax selection strategy over candidate sets.
    /// For each tuning iteration, a candidate value for ALPHA, BETA, RHO, and Q is chosen
    /// via softmax (with temperature control), the ACO is executed, and the reward is used to
    /// update the quality estimates.
    /// </summary>
    /// <param name="cvrp">The CVRP instance.</param>
    /// <param name="tuningIterations">Number of tuning iterations.</param>
    /// <param name="acoIterations">Number of ACO iterations per tuning run.</param>
    /// <param name="temperature">Temperature parameter for softmax (controls exploration/exploitation).</param>
    /// <param name="learningRate">Learning rate for updating candidate quality estimates.</param>
    public void AntColonyOptimizationWithTuning(CVRPInstance cvrp,
        int tuningIterations, int acoIterations, double temperature, double learningRate)
    {
        // Define candidate sets for each parameter
        double[] candidateAlpha = {0,4,0.5,0.7, 1.0, 1.5, 2.0, 2.5 };
        double[] candidateBeta = {0.9, 1.0, 1.5, 2.0, 2.5 };
        double[] candidateRho = { 0.1, 0.2, 0.3, 0.4, 0.5, 0.6, 0.7, 0.8, 0.9 };
        double[] candidateQ = {1,10,20,30,40,50, 100, 200, 300, 400,500 };

        // Initialize quality (expected reward) estimates for each candidate
        double[] qualityAlpha = new double[candidateAlpha.Length];
        double[] qualityBeta = new double[candidateBeta.Length];
        double[] qualityRho = new double[candidateRho.Length];
        double[] qualityQ = new double[candidateQ.Length];
        for (int i = 0; i < candidateAlpha.Length; i++) qualityAlpha[i] = 0.0;
        for (int i = 0; i < candidateBeta.Length; i++) qualityBeta[i] = 0.0;
        for (int i = 0; i < candidateRho.Length; i++) qualityRho[i] = 0.0;
        for (int i = 0; i < candidateQ.Length; i++) qualityQ[i] = 0.0;

        double bestOverallCost = double.MaxValue;
        double[] bestParameters = new double[4]; // bestParameters[0]=ALPHA, [1]=BETA, [2]=RHO, [3]=Q

        for (int t = 0; t < tuningIterations; t++)
        {
            // Select candidate parameters using softmax selection
            int idxAlpha = SoftmaxSelect(qualityAlpha, temperature);
            int idxBeta = SoftmaxSelect(qualityBeta, temperature);
            int idxRho = SoftmaxSelect(qualityRho, temperature);
            int idxQ = SoftmaxSelect(qualityQ, temperature);

            double selectedAlpha = candidateAlpha[idxAlpha];
            double selectedBeta = candidateBeta[idxBeta];
            double selectedRho = candidateRho[idxRho];
            double selectedQ = candidateQ[idxQ];

            // Run the ACO algorithm using the selected parameters
            int[] bestPath;
            double bestCost;
            this.AntColonyOptimizationPararrelv2(cvrp, selectedAlpha, selectedBeta, selectedRho, selectedQ,
                                       acoIterations, maxTimeACO: 0, out bestPath, out bestCost);

            // Define a reward measure; here we use the ratio (OptimalValue / bestCost)
            // so that better (lower cost) solutions yield higher rewards.
            double reward = cvrp.OptimalValue / bestCost;

            // Update quality estimates using an exponential moving average
            qualityAlpha[idxAlpha] = qualityAlpha[idxAlpha] + learningRate * (reward - qualityAlpha[idxAlpha]);
            qualityBeta[idxBeta] = qualityBeta[idxBeta] + learningRate * (reward - qualityBeta[idxBeta]);
            qualityRho[idxRho] = qualityRho[idxRho] + learningRate * (reward - qualityRho[idxRho]);
            qualityQ[idxQ] = qualityQ[idxQ] + learningRate * (reward - qualityQ[idxQ]);

            // Record best overall parameters if this run improved the cost
            if (bestCost < bestOverallCost)
            {
                bestOverallCost = bestCost;
                bestParameters[0] = selectedAlpha;
                bestParameters[1] = selectedBeta;
                bestParameters[2] = selectedRho;
                bestParameters[3] = selectedQ;
            }

            // Console.WriteLine($"Tuning Iteration {t}: Selected (Alpha, Beta, Rho, Q)=({selectedAlpha}, {selectedBeta}, {selectedRho}, {selectedQ}) => Reward: {reward}, BestCost: {bestCost}");
        }

        Console.WriteLine("Best Overall Parameters Found:");
        Console.WriteLine($"Alpha: {bestParameters[0]}, Beta: {bestParameters[1]}, Rho: {bestParameters[2]}, Q: {bestParameters[3]} with cost: {bestOverallCost}");
    }

    /// <summary>
    /// Helper method that selects an index from the quality array using the softmax function.
    /// </summary>
    /// <param name="qualities">Array of quality (reward) estimates.</param>
    /// <param name="temperature">Temperature parameter for softmax.</param>
    /// <returns>Index of the selected candidate.</returns>
    private int SoftmaxSelect(double[] qualities, double temperature)
    {
        double sum = 0.0;
        double[] weights = new double[qualities.Length];
        for (int i = 0; i < qualities.Length; i++)
        {
            weights[i] = Math.Exp(qualities[i] / temperature);
            sum += weights[i];
        }
        double threshold = RandomDouble(0, sum);
        double cumulative = 0.0;
        for (int i = 0; i < weights.Length; i++)
        {
            cumulative += weights[i];
            if (cumulative >= threshold)
                return i;
        }
        return qualities.Length - 1;
    }



}