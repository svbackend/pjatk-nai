using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Project3
{
    class Program
    {
        public const int NumberOfLetters = 26;
        
        // <string> here is lang code (en/pl etc, depends on folder name in ./training)
        public static Dictionary<string, Perceptron> perceptrons = new Dictionary<string, Perceptron>();

        static void Main(string[] args)
        {
            var langs = GetInputs();
            double a = .5;

            foreach (var lang in langs)
            {
                perceptrons.Add(lang.Key, new Perceptron(NumberOfLetters, a));
            }

            double totalErrorRate = 0;
            while (totalErrorRate < 10)
            {
                foreach (var lang in langs)
                {
                    totalErrorRate += Train(lang.Value, lang.Key);
                }
                
                Console.WriteLine(totalErrorRate.ToString());
            }
        }

        private static Dictionary<string, double[]> GetInputs()
        {
            var firstASCII = 97;
            var langsDirs = Directory.GetDirectories("./training");
            var langs = new Dictionary<string, double[]>(); // "en" => [ASCII => 0.6, ASCII => .8], pl => [...]

            foreach (string langDir in langsDirs)
            {
                var langFiles = Directory.GetFiles(langDir);
                langs.Add(langDir, new double[NumberOfLetters]);
                var totalChars = 0;
                
                foreach (var file in langFiles)
                {
                    var bytes = Encoding.ASCII.GetBytes(File.ReadAllText(file).ToLower());
                    totalChars += bytes.Length;
                    foreach (var ascii in bytes)
                    {
                        var key = ascii - firstASCII;
                        if (key >= 0 && key < langs[langDir].Length)
                        {
                            langs[langDir][key] += 1;
                        }
                    }
                }
                
                for (var i = 0; i < langs[langDir].Length; i++)
                {
                    langs[langDir][i] /= totalChars;
                }
            }

            return langs;
        }

        /**
         * Inputs = [ASCII CODE => HOW MANY TIMES THIS CHAR WAS USED DIVIDED BY TOTAL NUMBER OF CHARS]
         * Text: "hello world" (11 chars), let's assume that "l" represented as 0 in ASCII then input would looks like:
         * [0 => 3/11, ...other chars]
         */
        private static double Train(double[] inputs, string lang)
        {
            //1/2(sum(d-y)^2)
            double errorRate = 0;

            foreach (var perceptron in perceptrons)
            {
                double d = perceptron.Key == lang ? 1 : 0;
                double activation = perceptron.Value.Predict(inputs); // y
                perceptron.Value.UpdateWeights(inputs, d, activation);
                errorRate += (d - activation) * (d - activation);
            }

            return errorRate / 2;
        }
    }

    class Perceptron
    {
        private double[] weights;
        private double a;

        public Perceptron(int numberOfLetters, double a)
        {
            weights = new double[numberOfLetters];

            for (int i = 0; i < weights.Length; i++)
            {
                weights[i] = .1;
            }
            
            this.a = a;

            NormalizeWeights();
        }

        public double Predict(double[] inputs)
        {
            double x = 0;
            for (int i = 0; i < inputs.Length; i++)
            {
                x += inputs[i] * weights[i];
            }

            return 1 / (1 + Math.Exp(-x)); // sigmoid function 1 / ( 1 + e^(-x) )
        }

        public void UpdateWeights(double[] inputs, double d, double y)
        {
            for (int i = 0; i < inputs.Length; i++)
            {
                weights[i] += (d - y) * inputs[i] * a;
            }
            
            NormalizeWeights();
        }

        public void NormalizeWeights()
        {
            double sum = 0;
            for (int i = 0; i < weights.Length; i++)
            {
                sum += weights[i] * weights[i];   
            }

            sum = Math.Sqrt(sum);
            for (int i = 0; i < weights.Length; i++)
            {
                weights[i] /= sum;
            }
        }
    }
}