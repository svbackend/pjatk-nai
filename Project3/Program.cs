using System;
using System.Collections.Generic;
using System.IO;

namespace Project3
{
    class Program
    {
        public const int NumberOfLetters = 26;
        
        static void Main(string[] args)
        {
            var langsDirs = Directory.GetDirectories("./training");
            var langs = new Dictionary<string, Dictionary<char, double>>(); // "en" => [a => 0.6, b => .8], pl => [...]

            foreach (string langDir in langsDirs)
            {
                langs.Add(langDir, new Dictionary<char, double>());
                var langFiles = Directory.GetFiles(langDir);
                foreach (var file in langFiles)
                {
                    var bytes = File.ReadAllBytes(file);
                    foreach (var ascii in bytes)
                    {
                        var c = Convert.ToChar(ascii);
                        if (langs[langDir].ContainsKey(c))
                        {
                            var currentVal = langs[langDir].GetValueOrDefault(c, 1);
                            langs[langDir].Remove(c);
                            langs[langDir].Add(c, currentVal + 1);
                        }
                        else
                        {
                            langs[langDir].Add(c, 1);
                        }
                        
                    }
                }
            }

            var numberOfLangs = 3;
            var numberOfDocs = 3;
            double threshold = .0;
            double a = .5;
            
            List<Perceptron> perceptrons = new List<Perceptron>();
            for (int i = 0; i < NumberOfLetters; i++)
            {
                perceptrons.Add(new Perceptron(NumberOfLetters, a));
            }
            
            // double totalErrorRate = 0;
            // while(totalErrorRate < 17.72)
            // {
            //     totalErrorRate = 0;
            //     for (int i = 0; i < numberOfLangs; i++)
            //     {
            //         for (int j = 0; j < numberOfDocs; j++)
            //         {
            //             totalErrorRate += 1;
            //         }
            //     }
            //     
            // }
        }
        
        static bool CalcY(double threshold, double[] weights, double[] inputs)
        {
            double sum = 0.0;

            for (int i = 0; i < NumberOfLetters; i++)
            {
                sum += weights[i] * inputs[i];
            }

            return sum >= threshold;
        }
    }

    class Perceptron
    {
        private double[] weights;
        
        public Perceptron(int numberOfLetters, double a)
        {
            double[] weights = new double[numberOfLetters];
            
            double length = 0;
            for (int i = 0; i < numberOfLetters; i++)
            {
                length += weights[i] * weights[i];
            }
                
            length = Math.Sqrt(length);

            for (int i = 0; i < numberOfLetters; i++)
            {
                weights[i] /= length;    
            }
        }
    }
}