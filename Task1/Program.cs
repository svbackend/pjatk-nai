using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Task1
{
    class Program
    {
        static void Main(string[] args)
        {
            const string trainingFilePath = "iris.data";
            const string testFilePath = "iris.test.data";

            var trainingFileLines = File.ReadAllLines(trainingFilePath);
            var testFileLines = File.ReadAllLines(testFilePath);

            Console.WriteLine("Provide K:");
            var k = Convert.ToInt32(Console.ReadLine());

            if (k % 2 == 0)
            {
                Console.WriteLine("K need to be an ODD number (e.g 3, 5, 7, 9...)");
                return;
            }

            var trainingEntries = new List<Entry>();
            foreach (var row in trainingFileLines)
            {
                trainingEntries.Add(CreateFromRow(row));
            }

            var total = testFileLines.Length;
            var correctPredictions = 0;
            foreach (var row in testFileLines)
            {
                var e = CreateFromRow(row);

                var isCorrectPrediction = MakePrediction(e, trainingEntries, k);

                if (isCorrectPrediction)
                {
                    Console.Write(".");
                    correctPredictions++;
                }
                else
                {
                    Console.WriteLine();
                    Console.WriteLine("Incorrect prediction: " + e);
                }
            }

            Console.WriteLine();
            Console.WriteLine($"For k = {k} - correct predictions = {correctPredictions} out of {total} total");

            //CalcSuccessRateBasedOnK(trainingEntries, testFileLines);
        }

        private static void CalcSuccessRateBasedOnK(List<Entry> trainingEntries, string[] testFileLines)
        {
            var result = "";
            var maxK = trainingEntries.Count;
            var total = testFileLines.Length;

            for (var k = 3; k < maxK; k += 2)
            {
                var correctPredictions = 0;
                foreach (var row in testFileLines)
                {
                    var e = CreateFromRow(row);

                    var isCorrectPrediction = MakePrediction(e, OrderByDistance(trainingEntries, e), k);
                    if (isCorrectPrediction)
                    {
                        correctPredictions++;
                    }
                }

                Decimal rate = Decimal.Multiply(100, Decimal.Divide(correctPredictions, total));
                result += $"{k},{correctPredictions},{rate:N2}\n";
            }

            Console.WriteLine();
            Console.WriteLine("CSV (k, number of correct predictions, success rate)");
            Console.WriteLine(result);
        }

        static bool MakePrediction(Entry e, List<Entry> neighbors, int k = 7)
        {
            Dictionary<string, int> counter = new Dictionary<string, int>();

            if (k > neighbors.Count)
            {
                k = neighbors.Count;
            }

            for (var i = 0; i < k; i++)
            {
                var n = neighbors[i];

                if (counter.ContainsKey(n.EntryClass))
                {
                    counter[n.EntryClass]++;
                }
                else
                {
                    counter.Add(n.EntryClass, 1);
                }
            }

            /*
             * e.g
             * Iris-setosa => 6,
             * Iris-versicolor => 3,
             */

            var prediction = counter.OrderByDescending(kv => kv.Value).First().Key;
            e.Prediction = prediction;

            return e.Prediction == e.EntryClass;
        }

        static List<Entry> OrderByDistance(List<Entry> list, Entry e)
        {
            return list.OrderBy(le => le.GetDistance(e)).ToList();
        }

        static Entry CreateFromRow(string row)
        {
            var trainingEntryData = row.Split(',');
            var lastIndex =
                trainingEntryData.Length - 1; // last index always a EntryClass (in other words name, e.g "iris-satosa")
            var entryParams = new double[lastIndex];

            for (var i = 0; i < lastIndex; i++)
            {
                entryParams[i] = Convert.ToDouble(trainingEntryData[i]);
            }

            return new Entry
            {
                EntryParams = entryParams,
                EntryClass = trainingEntryData[lastIndex]
            };
        }
    }
}