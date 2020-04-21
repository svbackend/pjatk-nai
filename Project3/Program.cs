using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Project3
{
    class Program
    {
        public const int NumberOfLetters = 26;
        public const int FirstASCII = 97;

        // <string> here is lang code (en/pl etc, depends on folder name in ./training)
        public static Dictionary<string, Perceptron> perceptrons = new Dictionary<string, Perceptron>();

        public static Dictionary<string, double[]> langs;

        static void Main(string[] args)
        {
            langs = GetInputs();
            double alpha = 1.5;

            foreach (var lang in langs)
            {
                perceptrons.Add(lang.Key, new Perceptron(NumberOfLetters, alpha));
            }

            for (var generation = 0; generation < 10; generation++)
            {
                foreach (var lang in langs)
                {
                    Train(lang.Value, lang.Key);
                }
            }

            Console.WriteLine();

            foreach (var lang in langs)
            {
                string prediction = Predict(lang.Value);
                if (lang.Key != prediction)
                {
                    Console.WriteLine($"Expected: {lang.Key}");
                    Console.WriteLine($"Actual: {prediction}");
                    Console.WriteLine();
                }
            }

            var customTextInput = GetInputByFile("./training/test.txt", true);
            Console.WriteLine();
            var result = Predict(customTextInput);
            //Console.WriteLine(String.Join("|", customTextInput));
            Console.WriteLine($"test.txt Prediction: {result}");
        }

        private static Dictionary<string, double[]> GetInputs()
        {
            var langsDirs = Directory.GetDirectories("./training");

            var langs = new Dictionary<string, double[]>(); // "en" => [ASCII => 0.6, ASCII => .8], pl => [...]
            foreach (string langDir in langsDirs)
            {
                var langFiles = Directory.GetFiles(langDir);
                var lang = langDir.Split("/").Last();
                langs.Add(lang, new double[NumberOfLetters]);
                
                for (int i = 0; i < langs[lang].Length; i++)
                    langs[lang][i] = 0;
                
                var totalChars = 0;
                foreach (var file in langFiles)
                {
                    var input = GetInputByFile(file, false);
                    for (int i = 0; i < input.Length; i++)
                    {
                        langs[lang][i] += input[i];
                        totalChars += (int)input[i];
                    }
                }

                for (var i = 0; i < langs[lang].Length; i++)
                {
                    langs[lang][i] /= totalChars;
                }
            }

            return langs;
        }

        public static double[] GetInputByFile(string file, bool divideByTotal)
        {
            var bytes = Encoding.ASCII.GetBytes(File.ReadAllText(file).ToLower());
            double[] result = new double[NumberOfLetters];
            var totalChars = 0;
            
            foreach (var ascii in bytes)
            {
                var key = ascii - FirstASCII;
                if (key >= 0 && key < NumberOfLetters)
                {
                    totalChars += 1;
                    result[key] += 1;
                }
            }

            if (divideByTotal)
            {
                for (int i = 0; i < result.Length; i++)
                {
                    result[i] /= totalChars;
                }
            }

            return result;
        }

        /**
     * Inputs = [ASCII CODE => HOW MANY TIMES THIS CHAR WAS USED DIVIDED BY TOTAL NUMBER OF CHARS]
     * Text: "hello world" (11 chars), let's assume that "l" represented as 0 in ASCII then input would looks like:
     * [0 => 3/11, ...other chars]
     */
        private static double Train(double[] inputs, string lang)
        {
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

        static string Predict(double[] inputs, bool printConfidence = false)
        {
            string prediction = "";

            double maxConfidence = 0;
            foreach (var lang in langs)
            {
                double value = perceptrons[lang.Key].Predict(inputs);
                if (value >= maxConfidence)
                {
                    maxConfidence = value;
                    prediction = lang.Key;
                }
            }

            if (printConfidence)
            {
                Console.WriteLine($"Confidence: {maxConfidence}");
            }
            
            return prediction;
        }
    }

    class Perceptron
    {
        public double[] weights;
        public double Alpha;

        public Perceptron(int numberOfLetters, double alpha)
        {
            weights = new double[numberOfLetters];

            for (int i = 0; i < weights.Length; i++)
            {
                weights[i] = .1;
            }

            this.Alpha = alpha;

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
                weights[i] += (d - y) * inputs[i] * Alpha;
            }

            NormalizeWeights();
        }

        public void NormalizeWeights()
        {
            // todo
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