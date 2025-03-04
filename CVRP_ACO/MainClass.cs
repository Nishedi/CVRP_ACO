using System;

class MainClass
{
    static int[,] instance;
    static int instanceSize;
    static int[] bestSolution;
    static int bestCost;
    static AntColony antColony = new AntColony();

    static void Main()
    {
        Random random = new Random();
        Menu();
    }

    static void LoadFile(FileReader fileReader)
    {
        Console.WriteLine("Wybierz plik do wczytania:");
        Console.WriteLine("1. tsp_10a.txt\n2. tsp_10b.txt\n3. tsp_10c.txt\n4. tsp_14_db.txt");
        Console.WriteLine("5. br17.atsp\n6. ftv55.atsp\n7. ftv170.atsp\n8. rbg358.atsp\n9. Wpisz własną ścieżkę do pliku");

        int fileChoice = int.Parse(Console.ReadLine());
        string path = fileChoice switch
        {
            1 => "tsp_10a.txt",
            2 => "tsp_10b.txt",
            3 => "tsp_10c.txt",
            4 => "tsp_14_db.txt",
            5 => "br17.atsp",
            6 => "ftv55.atsp",
            7 => "ftv170.atsp",
            8 => "rbg358.atsp",
            9 => Console.ReadLine(),
            _ => ""
        };

        if (string.IsNullOrEmpty(path))
        {
            Console.WriteLine("Nieprawidłowy wybór.");
            return;
        }

        if (path.EndsWith(".atsp"))
        {
            instance = fileReader.LoadFromFile(path);
        }
        else
        {
            instance = fileReader.LoadFromFileTxt(path);
        }
        instanceSize = fileReader.size;
        Console.WriteLine($"Wczytano plik: {path}");
    }

    static void Menu()
    {
        FileReader fileReader = new FileReader();
        General general = new General();

        int ants = instanceSize;
        double alpha = 0.9, beta = 3.0, rho = 0.7, q = 10.0;
        int maxIterations = instanceSize * instanceSize / 50;
        maxIterations = 7;
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
                case 1:
                    LoadFile(fileReader);
                    ants = instanceSize;
                    break;
                case 2:
                    Console.WriteLine("Ostatnio wczytane dane:");
                    fileReader.ShowTab();
                    break;
                case 3:
                    if (instance != null && instanceSize > 0)
                    {
                        Console.WriteLine("Algorytm mrówkowy");
                        antColony.AntColonyOptimization(instance, instanceSize, ants, alpha, beta, rho, q, maxIterations, maxTimeACO, out bestSolution, out bestCost);
                    }
                    else Console.WriteLine("Brak danych.");
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
                default:
                    Console.WriteLine("Nieprawidłowa opcja.");
                    break;
            }
        }
    }
}
