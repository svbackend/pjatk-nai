using System;
using System.IO;
using System.Linq;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            var trainingFilePath = "iris.data";
            var testFilePath = "iris.test.data";

            var trainingFileLines = File.ReadAllLines(trainingFilePath);
            var testFileLines = File.ReadAllLines(testFilePath);

            var cols = trainingFileLines[0].Split(",").Length;
            var dim = cols - 1; // weight vector dimension
            var weights = Enumerable.Repeat(.1, dim).ToArray(); // vector of $dim dimension and 0.1
            
            var threshold = .0;
            var a = .5;
            var generations = 5;

            for (var g = 0; g < generations; g++)
            {
                foreach (var row in trainingFileLines)
                {
                    var trainingEntryData = row.Split(',');

                    var inputs = new double[dim];
                    for (var i = 0; i < dim; i++)
                    {
                        inputs[i] = Convert.ToDouble(trainingEntryData[i]);
                    }

                    var y = CalcY(threshold, weights, inputs); // y = activation function result. 1 or 0
                    var species = trainingEntryData[dim];
                    var d = GetDBySpecies(species); // d = desired result (expected) 1 if Setosa, 0 if Versicolor

                    weights = ProcessInputs(weights, inputs, d, y, a);
                    threshold -= (d - y) * a;
                }
                // iteration error - google
            }

            // for (int i = 0; i < dim; i++)
            //     Console.WriteLine(weights[i]);
            //
            // Console.WriteLine(threshold);

            var correct = 0;
            var all = testFileLines.Length;
            foreach (var row in testFileLines)
            {
                var testEntryData = row.Split(',');

                double[] inputs = new double[dim];
                for (int i = 0; i < dim; i++)
                {
                    inputs[i] = Convert.ToDouble(testEntryData[i]);
                }

                int prediction = CalcY(threshold, weights, inputs);
                var species = testEntryData[dim];

                if (GetSpeciesByD(prediction) == species)
                {
                    correct++;
                }
                else
                {
                    Console.WriteLine($"Incorrect! {row} - expected {species} but {GetSpeciesByD(prediction)} prediction given");
                }
            }

            Console.WriteLine($"Correctness: {correct} of {all} were predicted correctly");
            
            Console.WriteLine();
            Console.WriteLine("===USER ENTRY====");
            Console.WriteLine("Provide Entry (csv, use comma as separator, only input vector):");
            string userEntryLine = Console.ReadLine();
            var userEntryData = userEntryLine.Split(',');

            var userInput = new double[dim];
            for (int i = 0; i < dim; i++)
            {
                userInput[i] = Convert.ToDouble(userEntryData[i]);
            }
            
            var userEntryPrediction = CalcY(threshold, weights, userInput);
            Console.WriteLine("Prediction:");
            Console.WriteLine(GetSpeciesByD(userEntryPrediction));
        }

        static int CalcY(double threshold, double[] weights, double[] inputs)
        {
            double net = .0;

            for (int i = 0; i < weights.Length; i++)
            {
                net += weights[i] * inputs[i];
            }

            if (net >= threshold)
            {
                return 1;
            }

            return 0;
        }

        static double[] ProcessInputs(double[] weights, double[] inputs, int d, int y, double a)
        {
            for (int i = 0; i < weights.Length; i++)
            {
                weights[i] += (d - y) * inputs[i] * a;
            }

            return weights;
        }

        static int GetDBySpecies(string species)
        {
            return species == "Iris-setosa" ? 1 : 0;
        }

        static string GetSpeciesByD(int d)
        {
            return d == 1 ? "Iris-setosa" : "Iris-versicolor";
        }
    }
}