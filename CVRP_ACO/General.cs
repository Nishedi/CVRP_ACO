using System;

public class General
{
    public int truckCapacity = 30;
    public int truckDimension = 30;
    public int cargoMass = 10;
    public int cargoDimension = 10;
    public void GenerateInitialSolution(int[] solution, int size)
    {
        for (int i = 0; i < size; i++)
        {
            solution[i] = i;
        }

        Random rand = new Random();
        for (int i = size - 1; i > 0; i--)
        {
            int j = rand.Next(i + 1);
            (solution[i], solution[j]) = (solution[j], solution[i]);
        }
    }

    public int[] Greedy2(double[,] costMatrix, int size)
    {
        int[] path = new int[size];
        bool[] visited = new bool[size];
        Random rand = new Random();

        path[0] = rand.Next(size);
        visited[path[0]] = true;

        for (int i = 1; i < size; i++)
        {
            double min = int.MaxValue;
            int minIndex = -1;

            for (int j = 0; j < size; j++)
            {
                if (!visited[j] && costMatrix[path[i - 1], j] < min)
                {
                    min = costMatrix[path[i - 1], j];
                    minIndex = j;
                }
            }

            path[i] = minIndex;
            visited[minIndex] = true;
        }

        return path;
    }

    public double CalculateCost(int[] path, double[,] costMatrix, int size)
    {
        double cost = 0;
        for (int i = 0; i < size - 1; i++)
        {
            cost += costMatrix[path[i], path[i + 1]];
        }
        cost += costMatrix[path[size - 1], path[0]];
        return cost;
    }

    public int[][] GetNeighbourhoodSwap(int[] currentSolution, int size, out int neighbourhoodSize)
    {
        neighbourhoodSize = size * (size - 1) / 2;
        int[][] neighbourhood = new int[neighbourhoodSize][];
        int index = 0;

        for (int i = 0; i < size; i++)
        {
            for (int j = i + 1; j < size; j++)
            {
                neighbourhood[index] = (int[])currentSolution.Clone();
                (neighbourhood[index][i], neighbourhood[index][j]) = (neighbourhood[index][j], neighbourhood[index][i]);
                index++;
            }
        }
        return neighbourhood;
    }

    public int[][] GetNeighbourhoodInsert(int[] currentSolution, int size, out int neighbourhoodSize)
    {
        neighbourhoodSize = size * (size - 1);
        int[][] neighbourhood = new int[neighbourhoodSize][];
        int index = 0;

        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                if (i == j) continue;

                neighbourhood[index] = (int[])currentSolution.Clone();
                int city = neighbourhood[index][i];
                Array.Copy(neighbourhood[index], i + 1, neighbourhood[index], i, size - i - 1);
                Array.Copy(neighbourhood[index], j, neighbourhood[index], j + 1, size - j - 1);
                neighbourhood[index][j] = city;
                index++;
            }
        }
        return neighbourhood;
    }

    public int[][] GetNeighbourhoodReverse(int[] currentSolution, int size, out int neighbourhoodSize)
    {
        neighbourhoodSize = size * (size - 1) / 2;
        int[][] neighbourhood = new int[neighbourhoodSize][];
        int index = 0;

        for (int i = 0; i < size - 1; i++)
        {
            for (int j = i + 1; j < size; j++)
            {
                neighbourhood[index] = (int[])currentSolution.Clone();
                Array.Reverse(neighbourhood[index], i, j - i + 1);
                index++;
            }
        }
        return neighbourhood;
    }

    public int[][] GetNeighbourhoodRandom(int[] currentSolution, int size, int noChange, out int neighbourhoodSize)
    {
        const int maxNoChange = 50;
        Random rand = new Random();

        if (noChange >= maxNoChange)
        {
            return rand.Next(2) == 0 ? GetNeighbourhoodSwap(currentSolution, size, out neighbourhoodSize) : GetNeighbourhoodReverse(currentSolution, size, out neighbourhoodSize);
        }
        else
        {
            return GetNeighbourhoodInsert(currentSolution, size, out neighbourhoodSize);
        }
    }
}
