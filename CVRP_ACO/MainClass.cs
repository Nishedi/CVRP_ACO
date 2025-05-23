﻿using CVRP_ACO;
using System;
using System.Diagnostics;
using System.Reflection.Emit;
using System.Text;

class MainClass
{
    static double[,] instance;
    static int instanceSize;
    static int[] bestSolution;
    static double bestCost;
    static AntColony antColony = new AntColony();
    static KnnCVRP knn = new KnnCVRP();

    static void Main()
    {
        Random random = new Random();
        Menu();
    }

    static void CompareExecutionTime()
    {
        string[] filenames = new string[] { "A-n44-k6.txt" , "A-n55-k9.txt" , "A-n63-k10.txt", "A-n69-k9.txt" , "A-n80-k10.txt" };
        string outputfilename = "porownanie_czasow.tex";
        string caption = "{Porównanie czasów działania algorytmów}";
        string label = "{fig:czas-algorytmy}";
        double alpha = 0.9, beta = 3.0, rho = 0.7, q = 10.0;
        int maxTimeACO = 0, maxIterations = 10000;
        StringBuilder chart = new StringBuilder();
        StringBuilder x_coords = new StringBuilder("{");
        foreach (var filename in filenames)
        {
            string name = filename.Split('.')[0];
            name = name.Split("-")[1]+"-"+name.Split("-")[2];
            x_coords.Append( name + ", ");
        }
        x_coords.Length -= 2;
        x_coords.Append("}");
        chart.Append("\\begin{figure}[H]\r\n" +
            "\\centering\r\n    " +
            "\\begin{tikzpicture}\r\n        " +
            "\\begin{axis}[\r\n            " +
            "ybar,\r\n            " +
            "bar width=.7cm,\r\n            " +
            "width=12cm,\r\n            " +
            "height=7cm,\r\n            " +
            "ymin=0,\r\n            " +
            "ylabel={Czas [s]},\r\n            " +
            $"symbolic x coords={x_coords},\r\n            " +
            "xtick=data,\r\n            " +
            "nodes near coords,\r\n           " +
            "nodes near coords align={vertical},\r\n            " +
            "enlarge x limits=0.25,\r\n  legend pos=north west\r\n      ]\r\n        "
            );

        chart.Append("\\addplot coordinates {");
        foreach (var filename in filenames)
        {
            Console.WriteLine(filename);
            string name = filename.Split('.')[0];
            name = name.Split("-")[1] + "-" + name.Split("-")[2];
            chart.Append("(" + name + ",");
            var cvrp = CVRPInstance.LoadFromFile(filename);
            cvrp.createDistanceMatrix(cvrp.Nodes);
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            antColony.AntColonyOptimization(cvrp, alpha, beta, rho, q, maxIterations, maxTimeACO, out bestSolution, out bestCost);
            stopwatch.Stop();
            chart.Append($"{stopwatch.ElapsedMilliseconds})");
        }
        chart.Append("};\r\n" +
            "\\addlegendentry{Jednowątkowo}");
        chart.Append("\\addplot coordinates {");
        foreach (var filename in filenames)
        {
            Console.WriteLine(filename);
            string name = filename.Split('.')[0];
            name = name.Split("-")[1] + "-" + name.Split("-")[2];
            chart.Append("(" + name + ",");
            var cvrp = CVRPInstance.LoadFromFile(filename);
            cvrp.createDistanceMatrix(cvrp.Nodes);
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            antColony.AntColonyOptimizationPararrelv2(cvrp, alpha, beta, rho, q, maxIterations, maxTimeACO, out bestSolution, out bestCost);
            stopwatch.Stop();
            chart.Append($"{stopwatch.ElapsedMilliseconds})");
        }
        chart.Append("};\r\n" +
            "\\addlegendentry{Wielowątkowo}");
        chart.Append(
            "\\end{axis}\r\n    " +
            "\\end{tikzpicture}\r\n    " +
            $"\\caption{caption}\r\n    " +
            $"\\label{label}\r\n\\end"+"{figure}\r\n");
        Console.WriteLine(chart);
        File.WriteAllText(outputfilename, chart.ToString());
        File.WriteAllText("C:\\Users\\minec\\Desktop\\CVRP_ACO_Pempera_Zygadlo\\" + outputfilename, chart.ToString());
    }

    static void CompareAlgorithmQuality(string[] filenames, int[] timesOfExecution, string outputfilename, string caption, string label)
    {
       
        double alpha = 0.9, beta = 3.0, rho = 0.7, q = 10.0;
        StringBuilder chart = new StringBuilder();
        StringBuilder x_coords = new StringBuilder("{");
        foreach (var filename in filenames)
        {
            string name = filename.Split('.')[0];
            name = name.Split("-")[1] + "-" + name.Split("-")[2];
            x_coords.Append(name + ", ");
        }
        x_coords.Length -= 2;
        x_coords.Append("}");
        chart.Append("\\begin{figure}[H]\r\n" +
            "\\centering\r\n    " +
            "\\begin{tikzpicture}\r\n        " +
            "\\begin{axis}[\r\n            " +
            "ybar,\r\n            " +
            "bar width=.5cm,\r\n            " +
            "width=12cm,\r\n            " +
            "height=7cm,\r\n            " +
            "ymin=0,\r\n            " +
            "ylabel={Błąd [\\%]},\r\n            " +
            $"symbolic x coords={x_coords},\r\n            " +
            "xtick=data,\r\n            " +
            "nodes near coords,\r\n           " +
            "nodes near coords align={vertical},\r\n            " +
            "enlarge x limits=0.25,\r\n            legend pos=north west,\r\n" +
            "legend style={at={(0.5,-0.15)}, anchor=north, legend columns=-1}" +
            "]   "
            ); 
        for (int j = 1; j < 5; j++)
        {
            chart.Append("\\addplot coordinates {");
            for (int i = 0; i < filenames.Length; i++)
            {
                string name = filenames[i].Split('.')[0];
                name = name.Split("-")[1] + "-" + name.Split("-")[2];
                chart.Append("(" + name + ",");
                int maxTimeACO = timesOfExecution[i], maxIterations = 10000;
                var cvrp = CVRPInstance.LoadFromFile(filenames[i]);
                cvrp.createDistanceMatrix(cvrp.Nodes);
                if (j == 2)
                {
                    double x = 0;
                    for (int k = 0; k < 10; k++)
                    {
                        x+= antColony.AntColonyOptimization(cvrp, alpha, beta, rho, q, maxIterations, maxTimeACO, out bestSolution, out bestCost);
                    }
                    x = x/10;
                    Console.WriteLine(filenames[i] + ":" + Math.Round(x, 0) + "/" + cvrp.OptimalValue + "(" + Math.Round((x - cvrp.OptimalValue) / cvrp.OptimalValue, 2) + ")");
                    chart.Append($"{Math.Round(((x - cvrp.OptimalValue) / cvrp.OptimalValue)*100, 0)})");
                }
                else if (j == 1)
                {
                    double x = 0;
                    for (int k = 0; k < 10; k++)
                    {
                         x+= antColony.AntColonyOptimizationPararrelv2(cvrp, alpha, beta, rho, q, maxIterations, maxTimeACO, out bestSolution, out bestCost);
                    }
                    x = x/10;
                    Console.WriteLine(filenames[i] + ":" + Math.Round(x, 0) + "/" + cvrp.OptimalValue + "(" + Math.Round((x - cvrp.OptimalValue) / cvrp.OptimalValue, 2) + ")");
                    chart.Append($"{Math.Round(((x - cvrp.OptimalValue) / cvrp.OptimalValue) * 100, 0)})");
                }
                else if (j == 4)
                {
                    double x = 0;
                    for(int k = 0; k < 10; k++)
                    {
                        x+= knn.KnnSolve(cvrp);
                        
                    }
                    x = x/10;
                    Console.WriteLine(filenames[i] + ":" + Math.Round(x, 0) + "/" + cvrp.OptimalValue + "(" + Math.Round((x - cvrp.OptimalValue) / cvrp.OptimalValue, 2) + ")");
                    chart.Append($"{Math.Round(((x - cvrp.OptimalValue) / cvrp.OptimalValue) * 100, 0)})");
                }
                else if (j == 3)
                {
                    double x = 0;
                    for (int k = 0; k < 10; k++)
                    {
                        x += knn.knn2OptSolve(cvrp);
                        
                    }
                    x = x/10;
                    Console.WriteLine(filenames[i] + ":" + Math.Round(x, 0) + "/" + cvrp.OptimalValue + "(" + Math.Round((x - cvrp.OptimalValue) / cvrp.OptimalValue, 2) + ")");
                    chart.Append($"{Math.Round(((x - cvrp.OptimalValue) / cvrp.OptimalValue) * 100, 0)})");
                }
                else if (j == 0)
                {
                    chart.Append($"{cvrp.OptimalValue})");
                }
                }
            Console.WriteLine("--------------------------------------------------");
            if (j == 2) {
                chart.Append("};\r\n" +
                    "\\addlegendentry{Jednowątkowo}\r\n");
            }
            else if (j == 1)
            {
                chart.Append("};\r\n" +
                    "\\addlegendentry{Wielowątkowo}\r\n");
            }
            else if (j == 4)
            {
                chart.Append("};\r\n" +
                    "\\addlegendentry{KNN}");
            }
            else if (j == 3)
            {
                chart.Append("};\r\n" +
                    "\\addlegendentry{KNN 2 OPT}\r\n");
            }
            else if (j == 0)
            {
                chart.Append("};\r\n" +
                    "\\addlegendentry{Optymalnie}\r\n");
            }
        }
        
        chart.Append(
            "\\end{axis}\r\n    " +
            "\\end{tikzpicture}\r\n    " +
            $"\\caption{caption}\r\n    " +
            $"\\label{label}\r\n\\end" + "{figure}\r\n");
        File.WriteAllText(outputfilename, chart.ToString());
        File.WriteAllText("C:\\Users\\KONRAD PEMPERA\\Desktop\\CVRP_ACO_Pempera_Zygadlo\\" + outputfilename, chart.ToString());
    }

    static void Menu()
    {
        FileReader fileReader = new FileReader();
        General general = new General();

        int ants = instanceSize;
        double alpha = 0.9, beta = 3.0, rho = 0.7, q = 10.0;
        // double alpha = 1.5, beta = 1.5, rho = 0.4, q = 1;
        int maxIterations = instanceSize * instanceSize / 50;
        maxIterations = 10000;
        int maxTimeACO =10;

        while (true)
        {
            Console.WriteLine("\n----------------------------");
            Console.WriteLine("1. Wczytaj z pliku\n2. Wyświetl dane\n3. Algorytm mrówkowy");
            Console.WriteLine($"4. Liczba mrówek ({ants})\n5. Współczynnik parowania RHO ({rho})\n6. Stała feromonowa ({q})");
            Console.WriteLine($"7. Maksymalny czas algorytmu ({maxTimeACO} s)\n8. Softmax\n9. KNN \n10.KNN 2 OPT " +
                $"\n11.Single Core Mrówkowy\n12.Algorytm mrówkowy z optymalizacją parametrów\n" +
                $"13. Porównanie czasu wykonania różnych podejść\n14. Porównanie jakości zaimplementowanych algorytmów\n0. Zakończ");
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
                case 8:
                    var filenames2 = new[] 
                    { 
                        new { Filename= "A-n44-k6.txt",Time= 50}, 
                        new { Filename= "A-n63-k10.txt", Time= 70 }, 
                        new {Filename = "A-n80-k10.txt", Time= 90} 
                    };
                    foreach (var filename in filenames2)
                    {
                        Console.WriteLine("\n" + filename);
                        var cvrp = CVRPInstance.LoadFromFile(filename.Filename);
                        cvrp.createDistanceMatrix(cvrp.Nodes);
                        Console.WriteLine("Algorytm mrówkowy");
                        Stopwatch stopwatch = new Stopwatch();
                        ACO_Tuner aco_tuner = new ACO_Tuner();
                        stopwatch.Start();
                        aco_tuner.AntColonyOptimizationWithTuning(cvrp, 1000, filename.Time, 0.5, 0.1);
                        stopwatch.Stop();
                    }
                    break;
                case 9:
                    string[] filenames3 = new string[] { "A-n44-k6.txt", "A-n55-k9.txt", "A-n63-k10.txt","A-n69-k9.txt", "A-n80-k10.txt"  };
                    foreach (var filename in filenames3)
                    {
                        Console.WriteLine("\n" + filename);
                        var cvrp = CVRPInstance.LoadFromFile(filename);
                        cvrp.createDistanceMatrix(cvrp.Nodes);
                        Console.WriteLine("KNN");
                        Stopwatch stopwatch = new Stopwatch();

                        stopwatch.Start();
                        knn.KnnSolve(cvrp);
                        stopwatch.Stop();
                        Console.WriteLine($"Czas wykonania algorytmu: {stopwatch.ElapsedMilliseconds} ms");
                    }
                    break;
                case 10:
                    string[] filenames4 = new string[] {"A-n44-k6.txt", "A-n55-k9.txt", "A-n63-k10.txt","A-n69-k9.txt", "A-n80-k10.txt"  };
                    foreach (var filename in filenames4)
                    {
                        Console.WriteLine("\n" + filename);
                        var cvrp = CVRPInstance.LoadFromFile(filename);
                        cvrp.createDistanceMatrix(cvrp.Nodes);
                        Console.WriteLine("KNN 2 OPT");
                        Stopwatch stopwatch = new Stopwatch();

                        stopwatch.Start();
                        knn.knn2OptSolve(cvrp);
                        stopwatch.Stop();
                        Console.WriteLine($"Czas wykonania algorytmu: {stopwatch.ElapsedMilliseconds} ms");
                    }
                    break;
                case 11:


                     string[] filenames5 = new string[] {"A-n44-k6.txt", "A-n55-k9.txt", "A-n63-k10.txt","A-n69-k9.txt", "A-n80-k10.txt"  };
                    foreach (var filename in filenames5)
                    {
                        Console.WriteLine("\n" + filename);
                        var cvrp = CVRPInstance.LoadFromFile(filename);
                        cvrp.createDistanceMatrix(cvrp.Nodes);
                        Console.WriteLine("Ant Colony Single Core");
                        Stopwatch stopwatch = new Stopwatch();

                        stopwatch.Start();
                        antColony.AntColonyOptimization(cvrp, alpha, beta, rho, q, maxIterations, maxTimeACO, out bestSolution, out bestCost);

                        stopwatch.Stop();
                        Console.WriteLine($"Czas wykonania algorytmu: {stopwatch.ElapsedMilliseconds} ms");
                    }
                    break;
                case 12:
                var testCases = new[]
                {
                    new { Filename = "A-n44-k6.txt", Alpha = 1.1, Beta = 1.7, Rho = 1.0, Q = 1.0 },
                    new { Filename = "A-n55-k9.txt", Alpha = 2.4, Beta = 2.6, Rho = 0.95, Q = 200.0 },
                    new { Filename = "A-n63-k10.txt", Alpha = 1.8, Beta = 1.2, Rho = 0.65, Q = 50.0 },
                    new { Filename = "A-n69-k9.txt", Alpha = 1.4, Beta = 1.9, Rho = 0.9, Q = 100.0 },
                    new { Filename = "A-n80-k10.txt", Alpha = 1.9, Beta = 1.5, Rho = 1.0, Q = 200.0 }
                };

                foreach (var testCase in testCases)
                {
                    Console.WriteLine("\n" + testCase.Filename);
                    var cvrp = CVRPInstance.LoadFromFile(testCase.Filename);
                    cvrp.createDistanceMatrix(cvrp.Nodes);

                    Console.WriteLine("Algorytm mrówkowy z optymalizacją parametrów");

                    Stopwatch stopwatch = new Stopwatch();

                    stopwatch.Start();
                    antColony.AntColonyOptimizationPararrelv2(cvrp, testCase.Alpha, testCase.Beta, testCase.Rho, testCase.Q, maxIterations, maxTimeACO, out bestSolution, out bestCost);
                    stopwatch.Stop();

                    Console.WriteLine($"\nCzas wykonania algorytmu wielowątkowego: {stopwatch.ElapsedMilliseconds} ms");
                }
                    break;
                case 13:
                    {
                        CompareExecutionTime();
                        break;
                    }
                case 14:
                    {
                        string [] filenamess = new string[] { "A-n44-k6.txt", "A-n55-k9.txt", "A-n63-k10.txt"};
                        string outputfilename = "porownanie_jakosciv1.tex";
                        string caption = "{Porównanie jakości wyników generowanych przez zaimplementowane algorytmy}";
                        string label = "{fig:jakosc-algorytmyv1}";
                        int[] timesOfExecution = new int[] { 50, 60,70 };
                        CompareAlgorithmQuality(filenamess, timesOfExecution,outputfilename, caption,label);
                        filenamess = new string[] {"A-n69-k9.txt", "A-n80-k10.txt" };
                        outputfilename = "porownanie_jakosciv2.tex";
                        caption = "{Porównanie jakości wyników generowanych przez zaimplementowane algorytmy}";
                        label = "{fig:jakosc-algorytmyv2}";
                        timesOfExecution = new int[] {80,90 };
                        CompareAlgorithmQuality(filenamess, timesOfExecution, outputfilename, caption, label);
                        break;
                    }   
                default:
                    Console.WriteLine("Nieprawidłowa opcja.");
                    break;
            }
        }
    }
}
