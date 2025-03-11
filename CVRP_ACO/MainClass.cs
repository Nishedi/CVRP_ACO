using CVRP_ACO;
using System;
using System.Diagnostics;

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
        maxIterations = 10000;
        int maxTimeACO =10;

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
                    Console.WriteLine("Wczytywanie danych: to do");
                    fileReader.ShowTab();
                    break;
                case 3:
                    string[] filenames = new string[] {/* "A-n44-k6.txt", "A-n55-k9.txt", "A-n63-k10.txt","A-n69-k9.txt",*/ "A-n80-k10.txt" };


                    foreach (var filename in filenames)
                    {
                        Console.WriteLine("\n" + filename);
                        var cvrp = CVRPInstance.LoadFromFile(filename);
                        cvrp.createDistanceMatrix(cvrp.Nodes);
                        Console.WriteLine("Algorytm mrówkowy");
                        Stopwatch stopwatch = new Stopwatch();

                        //wersja jednowątkowa
                        //stopwatch.Start(); 
                        //antColony.AntColonyOptimization(cvrp, alpha, beta, rho, q, maxIterations, maxTimeACO, out bestSolution, out bestCost);
                        //stopwatch.Stop();
                        //Console.WriteLine($"Czas wykonania algorytmu jednowątkowego: {stopwatch.ElapsedMilliseconds} ms");
                        //stopwatch.Reset();
                        /*stopwatch.Start();
                        antColony.AntColonyOptimizationPararrel(cvrp, alpha, beta, rho, q, maxIterations, maxTimeACO, out bestSolution, out bestCost);
                        stopwatch.Stop();
                        Console.WriteLine($"Czas wykonania algorytmu jednowątkowego: {stopwatch.ElapsedMilliseconds} ms");
                        stopwatch.Reset();*/
                        stopwatch.Start();
                        antColony.AntColonyOptimizationPararrelv2(cvrp, alpha, beta, rho, q, maxIterations, maxTimeACO, out bestSolution, out bestCost);
                        stopwatch.Stop();
                        Console.WriteLine($"Czas wykonania algorytmu jednowątkowego: {stopwatch.ElapsedMilliseconds} ms");
                    }
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
                case 0:
                    return;
                    break;
                default:
                    Console.WriteLine("Nieprawidłowa opcja.");
                    break;
            }
        }
    }
}
