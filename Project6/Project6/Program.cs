using System;
using System.IO;
using System.Linq;

namespace Project6
{
    class Program
    {
        public static int[] Sizes;
        public static int[] Values;
        
        static void Main(string[] args)
        {
            var capacity = 40;
            var sizesInput = "1 1 6 10 3 4 8 9 3 3 4 8 7 10 6 1 8 6 6 10 1 8 7 3 4 5".Split(" ");
            var valuesInput = "18 18 5 10 11 9 11 17 12 13 3 1 7 7 2 6 2 6 1 7 4 15 18 18 14 8".Split(" ");
            int n = valuesInput.Length;
            
            Sizes = new int[n];
            Values = new int[n];
            for (int i = 0; i < n; i++)
            {
                Sizes[i] = int.Parse(sizesInput[i]);
                Values[i] = int.Parse(valuesInput[i]);
            }

            int vec = Knapsack(n, capacity);
            
            int totalSize = 0;
            int totalValue = 0;

            for (int i = 0; i < n; i++)
            {
                var isSelected = ((vec >> i) & 1);
                if (isSelected == 1)
                {
                    totalSize += Sizes[i];
                    totalValue += Values[i];
                }

                Console.Write(isSelected);
                Console.Write(" ");
            }
            Console.WriteLine();
            
            Console.WriteLine("Total Size:");
            Console.WriteLine(totalSize);
            
            Console.WriteLine("Total Value:");
            Console.WriteLine(totalValue);
            
        }
        
        static int Knapsack(int n, int capacity)
        {
            var vector = 0;
            int val = 0;
            int w = 0;

            for (int v = 1; v <	(1<<n); v++)
            {
                int value = GetValueSum(n, v);
                if (value <= val)
                {
                    continue;
                }
                
                int weight = GetWeightsSum(n, v);
                if (weight > capacity || weight < w)
                {
                    continue;
                }

                vector = v;
                val = value;
                w = weight;
                Console.WriteLine("New Best Weight:");
                Console.WriteLine(weight);
                Console.WriteLine("New Best Value:");
                Console.WriteLine(value);
            }
            
            Console.WriteLine("Total Iterations:");
            Console.WriteLine((1<<n));
            
            return vector;
        }
        
        
        public static int GetWeightsSum(int n, int v) 
        {
            int weight = 0;
            for (int i = 0; i < n; i++)
            {
                weight += Sizes[i] * ((v >> i) & 1);
            }

            return weight;
        }

        public static int GetValueSum(int n, int v) 
        {
            int value = 0;
            for (int i = 0; i < n; i++)
            {
                value += Values[i] * ((v >> i) & 1);
            }

            return value;
        }
    }
}