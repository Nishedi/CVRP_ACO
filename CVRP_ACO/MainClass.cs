using CVRP_ACO;
using System;

class MainClass
{
    static double[,] instance;
    static int instanceSize;
    static int[] bestSolution;
    static double bestCost;
    static AntColony antColony = new AntColony();

    static void Main()
    {
        Random random = new Random();
        Menu();
    }

    static void Menu()
    {
        FileReader fileReader = new FileReader();
        General general = new General();

        int ants = instanceSize;
        double alpha = 0.9, beta = 3.0, rho = 0.7, q = 10.0;
        int maxIterations = instanceSize * instanceSize / 50;
        maxIterations = 700;
        int maxTimeACO = 10;

        while (true)
        {
            Console.WriteLine("\n----------------------------");
            Console.WriteLine("1. Wczytaj z pliku\n2. Wyświetl dane\n3. Algorytm mrówkowy");
            Console.WriteLine($"4. Liczba mrówek ({ants})\n5. Współczynnik parowania RHO ({rho})\n6. Stała feromonowa ({q})");
            Console.WriteLine($"7. Maksymalny czas algorytmu ({maxTimeACO} s)\n8. Porównaj z zachłannym\n0. Zakończ");
            int choice = int.Parse(Console.ReadLine());

            switch (choice)
            {
                case 2:
                    Console.WriteLine("Ostatnio wczytane dane:");
                    fileReader.ShowTab();
                    break;
                case 3:
                    
                     var cvrp = CVRPInstance.LoadFromFile("test2.txt");
                     cvrp.createDistanceMatrix(cvrp.Nodes);
                     Console.WriteLine("Algorytm mrówkowy");
                     antColony.AntColonyOptimization(cvrp, alpha, beta, rho, q, maxIterations, maxTimeACO, out bestSolution, out bestCost);
                    
                    break;
                case 4:
                    Console.Write("Podaj liczbę mrówek: ");
                    ants = int.Parse(Console.ReadLine());
                    break;
                case 5:
                    Console.Write("Podaj współczynnik parowania RHO: ");
                    rho = double.Parse(Console.ReadLine());
                    break;
                case 6:
                    Console.Write("Podaj stałą feromonową Q: ");
                    q = double.Parse(Console.ReadLine());
                    break;
                case 7:
                    Console.Write("Podaj maksymalny czas algorytmu: ");
                    maxTimeACO = int.Parse(Console.ReadLine());
                    break;
                case 8:
                    if (instance != null && instanceSize > 0)
                    {
                        Console.WriteLine("Algorytm zachłanny"+general.CalculateCost(general.Greedy2(instance, instanceSize),instance,instanceSize));
                       
                    }
                    else Console.WriteLine("Brak danych.");
                    break;
                /*case 9:
                    var cvrp = CVRPInstance.LoadFromFile("test.txt");
                    cvrp.createDistanceMatrix(cvrp.Nodes);
                    break;*/
                default:
                    Console.WriteLine("Nieprawidłowa opcja.");
                    break;
            }
        }
    }
}
