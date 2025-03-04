using System;
using System.IO;

public class FileReader
{
    private int[,] tab;
    public int size;

    public int[,] LoadFromFileTxt(string path)
    {
        if (!File.Exists(path))
        {
            Console.WriteLine("Błędnie podana nazwa pliku");
            return null;
        }

        string[] lines = File.ReadAllLines(path);
        if (lines.Length == 0)
        {
            Console.WriteLine("Plik jest pusty");
            return null;
        }

        size = int.Parse(lines[0]);
        Allocate(size);

        for (int i = 0; i < size; i++)
        {
            string[] values = lines[i + 1].Split();
            int counter = 0;
            for (int j = 0; j < values.Count(); j++)
            {
                if (values[j] != "" && values[j] != null)
                {
                    tab[i, counter] = int.Parse(values[j]);
                    counter++;
                }
                   
            }
        }

        return tab;
    }

    public int[,] LoadFromFile(string path)
    {
        if (!File.Exists(path))
        {
            Console.WriteLine("Błędnie podana nazwa pliku");
            return null;
        }

        string[] lines = File.ReadAllLines(path);
        int N = 0;
        bool readingMatrix = false;
        int lineIndex = 0;

        foreach (var line in lines)
        {
            lineIndex++;
            if (line.StartsWith("DIMENSION:"))
            {
                N = int.Parse(line.Split(':')[1].Trim());
            }
            else if (line.StartsWith("EDGE_WEIGHT_SECTION"))
            {
                readingMatrix = true;
                continue;
            }

            if (readingMatrix && N > 0)
            {
                break;
            }
        }

        if (N == 0)
        {
            Console.WriteLine("Nie można odczytać wymiaru macierzy.");
            return null;
        }

        Allocate(N);
        int row = 0;
        int counter = 0;
        for (int i = lineIndex-1; i < lines.Length-1; i++)
        {
            string[] values = lines[i].Split();
            
            for (int j = 0; j < values.Length; j++)
            {
                if (values[j] != "")
                {
                    tab[row, counter] = int.Parse(values[j]);
                    counter++;
                }
                if (counter == size)
                {
                    row++;
                    counter = 0;
                }
            }
            if(counter == size)
            {
                row++;
                counter = 0;
            }
                
        }

        return tab;
    }

    private void Allocate(int N)
    {
        size = N;
        tab = new int[N, N];
    }

    public void ShowTab()
    {
        if (tab != null)
        {
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    Console.Write(tab[i, j] != -1 ? $"{tab[i, j],3} " : "    ");
                }
                Console.WriteLine();
            }
        }
    }

    public int[,] LoadRandomData(int N, int seed)
    {
        int[] source = new int[N * N - N];
        OrderTable(source, N * N - N);
        RandomShuttle(source, N * N - N, seed);
        Allocate(N);
        int k = 0;

        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                if (i == j)
                {
                    tab[i, j] = -1;
                    continue;
                }
                tab[i, j] = source[k++];
            }
        }
        return tab;
    }

    private void OrderTable(int[] randTab, int number)
    {
        for (int i = 0; i < number; i++)
        {
            randTab[i] = i + 1;
        }
    }

    private void RandomShuttle(int[] randTab, int number, int seed)
    {
        Random rand = new Random(seed);

        for (int i = 0; i < number; i++)
        {
            int randomIndex1 = rand.Next(number);
            int randomIndex2 = rand.Next(number);
            (randTab[randomIndex1], randTab[randomIndex2]) = (randTab[randomIndex2], randTab[randomIndex1]);
        }
    }
}
