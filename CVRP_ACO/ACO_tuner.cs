using CVRP_ACO;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

public class ACO_Tuner
{
    public void AntColonyOptimizationWithTuning(CVRPInstance cvrp,
        int tuningIterations, int acoIterations, double temperature, double learningRate)
    {
        var parameterRewardLog = new List<string>();
        parameterRewardLog.Add("Alpha,Beta,Rho,Q,Reward");


        double[] candidateAlpha = Enumerable.Range(1, 25).Select(i => i * 0.1).ToArray();
        double[] candidateBeta = Enumerable.Range(7, 25).Select(i => i * 0.1).ToArray();
        double[] candidateRho = Enumerable.Range(1, 20).Select(i => i * 0.05).ToArray();
        double[] candidateQ = { 1, 10, 20, 30, 40, 50, 100, 200, 300, 400, 500 };

        double[] qualityAlpha = new double[candidateAlpha.Length];
        double[] qualityBeta = new double[candidateBeta.Length];
        double[] qualityRho = new double[candidateRho.Length];
        double[] qualityQ = new double[candidateQ.Length];

        double bestOverallCost = double.MaxValue;
        double[] bestParameters = new double[4];

        // Logowanie danych
        var rewardHistory = new List<double>();
        var bestCostHistory = new List<double>();
        var qualityAlphaHistory = new List<double[]>();

        int progressBarWidth = 50;

        for (int t = 0; t < tuningIterations; t++)
        {
            // Pasek postępu
            double progress = (double)t / tuningIterations;
            int filledBars = (int)(progress * progressBarWidth);
            string progressBar = "[" + new string('#', filledBars) + new string('-', progressBarWidth - filledBars) + "]";
            Console.Write($"\rProgress: {progress:P0} {progressBar}");

            int idxAlpha = SoftmaxSelect(qualityAlpha, temperature);
            int idxBeta = SoftmaxSelect(qualityBeta, temperature);
            int idxRho = SoftmaxSelect(qualityRho, temperature);
            int idxQ = SoftmaxSelect(qualityQ, temperature);

            double selectedAlpha = candidateAlpha[idxAlpha];
            double selectedBeta = candidateBeta[idxBeta];
            double selectedRho = candidateRho[idxRho];
            double selectedQ = candidateQ[idxQ];

            int[] bestPath;
            double bestCost;
            AntColony aco = new AntColony();
            aco.AntColonyOptimizationPararrelv2(cvrp, selectedAlpha, selectedBeta, selectedRho, selectedQ,
                                    acoIterations, maxTimeACO: acoIterations, out bestPath, out bestCost);

            double reward = cvrp.OptimalValue / bestCost;

            parameterRewardLog.Add($"{selectedAlpha.ToString(CultureInfo.InvariantCulture)}," +
                       $"{selectedBeta.ToString(CultureInfo.InvariantCulture)}," +
                       $"{selectedRho.ToString(CultureInfo.InvariantCulture)}," +
                       $"{selectedQ.ToString(CultureInfo.InvariantCulture)}," +
                       $"{reward.ToString(CultureInfo.InvariantCulture)}");


            // Aktualizacja jakości
            qualityAlpha[idxAlpha] += learningRate * (reward - qualityAlpha[idxAlpha]);
            qualityBeta[idxBeta] += learningRate * (reward - qualityBeta[idxBeta]);
            qualityRho[idxRho] += learningRate * (reward - qualityRho[idxRho]);
            qualityQ[idxQ] += learningRate * (reward - qualityQ[idxQ]);

            // Logowanie danych
            rewardHistory.Add(reward);
            bestCostHistory.Add(bestCost);
            qualityAlphaHistory.Add((double[])qualityAlpha.Clone());

            if (bestCost < bestOverallCost)
            {
                bestOverallCost = bestCost;
                bestParameters[0] = selectedAlpha;
                bestParameters[1] = selectedBeta;
                bestParameters[2] = selectedRho;
                bestParameters[3] = selectedQ;
            }
        }

        // Pasek postępu końcowy
        Console.WriteLine("\rProgress: 100% [" + new string('#', progressBarWidth) + "]");

        Console.WriteLine("\nBest Overall Parameters Found:");
        Console.WriteLine($"Alpha: {bestParameters[0]}, Beta: {bestParameters[1]}, Rho: {bestParameters[2]}, Q: {bestParameters[3]}");
        Console.WriteLine("Najlepszy koszt: " + bestOverallCost + "/" + cvrp.OptimalValue);
        Console.WriteLine("x" + (bestOverallCost - cvrp.OptimalValue) / cvrp.OptimalValue);

        // Zapis CSV
        File.WriteAllLines("rewardHistory.csv", rewardHistory.Select(r => r.ToString(CultureInfo.InvariantCulture)));
        File.WriteAllLines("bestCostHistory.csv", bestCostHistory.Select(c => c.ToString(CultureInfo.InvariantCulture)));
        File.WriteAllLines("parameter_reward_log.csv", parameterRewardLog);

        using (var writer = new StreamWriter("qualityAlphaHistory.csv"))
        {
            foreach (var row in qualityAlphaHistory)
            {
                string line = string.Join(",", row.Select(v => v.ToString(CultureInfo.InvariantCulture)));
                writer.WriteLine(line);
            }
        }
    }

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

    private double RandomDouble(double min, double max)
    {
        Random rand = new Random();
        return min + (rand.NextDouble() * (max - min));
    }

    
}
