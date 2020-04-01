using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Project2
{
    class Program
    {
        const int Dim = 4;

        static void Main(string[] args)
        {
            var trainingFilePath = "iristrain.csv";
            var testFilePath = "iristest.csv";

            var trainingFileLines = File.ReadAllLines(trainingFilePath);
            var testFileLines = File.ReadAllLines(testFilePath);
            var totalTestLines = testFileLines.Length;

            double[] weights = new double[Dim];
            double lowerThreshold = .0;
            double upperThreshold = .0;
            double a = .5;

            for (int i = 0; i < Dim; i++)
                weights[i] = .1;

            // int generations = 5;
            // for (int g = 0; g < generations; g++)

            foreach (var row in trainingFileLines)
            {
                var trainingEntryData = row.Split(',');

                var col = 1;
                double[] inputs = new double[Dim];
                for (int i = 0; i < Dim; i++, col++)
                {
                    inputs[i] = Convert.ToDouble(trainingEntryData[col].ToString());
                }

                double y = CalcY(lowerThreshold, upperThreshold, weights, inputs);
                var species = trainingEntryData[col].ToString();
                var d = GetDBySpecies(species);

                weights = ProcessInputs(weights, inputs, d, y, a);
                upperThreshold -= (d - y) * a;
                lowerThreshold += (d - y) * a;
            }

            
            // for (int i = 0; i < Dim; i++)
                // System.Console.WriteLine(weights[i]);

            //System.Console.WriteLine(lowerThreshold);
            //System.Console.WriteLine(upperThreshold);

            var correct = 0;
            var all = 0; 
            foreach (var row in testFileLines)
            {
                all++;
                var testEntryData = row.Split(',');

                var col = 1;
                double[] inputs = new double[Dim];
                for (int i = 0; i < Dim; i++, col++)
                {
                    inputs[i] = Convert.ToDouble(testEntryData[col].ToString());
                }

                double prediction = CalcY(lowerThreshold, upperThreshold, weights, inputs);
                var species = testEntryData[col].ToString();

                if (prediction == GetDBySpecies(species))
                {
                    correct++;
                }
                else
                {
                    System.Console.WriteLine($"Incorrect prediction: {testEntryData[0]} == {species} == {prediction}");
                }
            }

            System.Console.WriteLine($"Correctness: {correct} of {all} were predicted correctly");
        }

        static double CalcY(double lowerThreshold, double upperThreshold, double[] weights, double[] inputs)
        {
            double sum = 0.0;

            for (int i = 0; i < Dim; i++)
            {
                sum += weights[i] * inputs[i];
            }

            if (sum >= upperThreshold)
            {
                return 1;
            }

            if (sum >= lowerThreshold)
            {
                return 0.5;
            }

            return 0;
        }

        static double[] ProcessInputs(double[] weights, double[] inputs, double d, double y, double a)
        {
            for (int i = 0; i < Dim; i++)
            {
                weights[i] += (d - y) * inputs[i] * a;
            }

            return weights;
        }

        static double GetDBySpecies(string species)
        {
            double d;
            switch (species)
            {
                case "\"setosa\"":
                    d = 0;
                    break;
                case "\"versicolor\"":
                    d = 0.5;
                    break;
                case "\"virginica\"":
                    d = 1;
                    break;
                default:
                    throw new InvalidDataException();
                    break;
            }

            return d;
        }
    }
}