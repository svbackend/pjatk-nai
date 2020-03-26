using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Project1
{
	class Entry
	{
		// "nr","Sepal.Length","Sepal.Width","Petal.Length","Petal.Width","Species"
		public double SepalLength { get; set; }
		public double SepalWidth { get; set; }
		public double PetalLength { get; set; }
		public double PetalWidth { get; set; }
		public string Species { get; set; }
		public string Prediction { get; set; }

		public double GetDistance(Entry e)
		{
			double distance = 0.0;
			
			distance += Math.Pow(this.SepalLength - e.SepalLength, 2);
			distance += Math.Pow(this.SepalWidth - e.SepalWidth, 2);
			distance += Math.Pow(this.PetalLength - e.PetalLength, 2);
			distance += Math.Pow(this.PetalWidth - e.PetalWidth, 2);
			
			return distance;
		}

		public override string ToString()
		{
			var str = $"{SepalLength}, {SepalWidth}, {PetalLength}, {PetalWidth}, {Species}";

			if (Prediction != null)
			{
				str += $" - our prediction={Prediction}";
			}
			
			return str;
		}
	}
	
    class Program
    {
	    static void Main(string[] args)
        {
	        var trainingFilePath = "../../../iristrain.csv";
	        var testFilePath = "../../../iristest.csv";
	        
	        var trainingFileLines = File.ReadAllLines(trainingFilePath);
	        var testFileLines = File.ReadAllLines(testFilePath);
	        var totalTestLines = testFileLines.Length;

	        //Console.WriteLine("Provide K:");
	        int k = 15; // Convert.ToInt32(Console.ReadLine());

	        var trainingEntries = new List<Entry>{};
	        foreach (var row in trainingFileLines)
	        {
		        var trainingEntryData = row.Split(',');
		        trainingEntries.Add(new Entry
		        {
			        SepalLength = Convert.ToDouble(trainingEntryData[1]),
			        SepalWidth = Convert.ToDouble(trainingEntryData[2]),
			        PetalLength = Convert.ToDouble(trainingEntryData[3]),
			        PetalWidth = Convert.ToDouble(trainingEntryData[4]),
			        Species = trainingEntryData[5]
		        });
	        }

	        var testEntries = new List<Entry>{};
	        foreach (var row in testFileLines)
	        {
		        var testEntryData = row.Split(',');
		        var e = new Entry
		        {
			        SepalLength = Convert.ToDouble(testEntryData[1]),
			        SepalWidth = Convert.ToDouble(testEntryData[2]),
			        PetalLength = Convert.ToDouble(testEntryData[3]),
			        PetalWidth = Convert.ToDouble(testEntryData[4]),
			        Species = testEntryData[5]
		        };
		        
		        MakePrediction(e, OrderByDistance(trainingEntries, e), k);
	        }

	        Console.WriteLine();
	        Console.WriteLine("===USER ENTRY====");
	        Console.WriteLine("Provide K:");
	        int userK = Convert.ToInt32(Console.ReadLine());
	        Console.WriteLine("Provide Entry (csv, use comma as separator):");
	        string userEntryLine = Console.ReadLine();
	        var userEntryData = userEntryLine.Split(',');
	        var userEntry = new Entry
	        {
		        SepalLength = Convert.ToDouble(userEntryData[0]),
		        SepalWidth = Convert.ToDouble(userEntryData[1]),
		        PetalLength = Convert.ToDouble(userEntryData[2]),
		        PetalWidth = Convert.ToDouble(userEntryData[3]),
	        };
	        
	        Console.WriteLine("Your Prediction");
	        MakePrediction(userEntry, OrderByDistance(trainingEntries, userEntry), userK, true);
        }

        static void MakePrediction(Entry e, List<Entry> neighbors, int k, bool full = false)
        {
	        Dictionary<string, int> counter = new Dictionary<string, int>();

	        
	        for (; k >= 0; k--)
	        {
		        var n = neighbors[k];
		        if (full)
		        {
			        Console.WriteLine("N: " + n + " / Distance: " + n.GetDistance(e));
		        }
		        
		        if (counter.ContainsKey(n.Species))
		        {
			        counter[neighbors[k].Species]++;
		        }
		        else
		        {
			        counter.Add(n.Species, 1);
		        }
	        }
	        
	        var prediction = counter.OrderByDescending(kv => kv.Value).First().Key;
	        e.Prediction = prediction;

	        if (full)
	        {
		        Console.WriteLine(prediction);
		        return;
	        }

	        if (e.Prediction == e.Species)
	        {
		        Console.Write(".");
	        }
	        else
	        {
		        Console.WriteLine();
		        Console.WriteLine("Incorrect prediction: " + e.ToString());
	        }
        }

        static List<Entry> OrderByDistance(List<Entry> list, Entry e)
        {
	        return list.OrderBy(le => le.GetDistance(e)).ToList();
        }

    }
}