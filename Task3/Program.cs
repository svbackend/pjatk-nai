using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Task3
{
    class Program
    {
        public const int NumberOfLetters = 26;
        public const int FirstASCII = 97;

        // <string> here is lang code (en/pl etc, depends on folder name in ./training)
        public readonly static Dictionary<string, Perceptron> Perceptrons = new();

        public static Dictionary<string, double[]> InputVectors;

        static void Main(string[] args)
        {
            InputVectors = GetInputs(); // key = lang code, value => input vector (x)
            double alpha = .25;

            foreach (var lang in InputVectors.Keys)
            {
                Perceptrons.Add(lang, new Perceptron(NumberOfLetters, alpha));
            }

            var generations = 100000;
            var globalErrorRate = .0;
            for (var g = 0; g < generations; g++)
            {
                foreach (var lang in InputVectors)
                {
                    globalErrorRate += Train(lang.Value, lang.Key); // todo deal with error rate
                }

                if (globalErrorRate < 0.01)
                {
                    Console.WriteLine($"Error Rate < 0.01, end training on generation #{g}");
                    break;
                }
            }

            Console.WriteLine(globalErrorRate);

            Console.WriteLine();

            foreach (var lang in InputVectors)
            {
                string prediction = Predict(lang.Value);
                if (lang.Key != prediction)
                {
                    Console.WriteLine($"Expected: {lang.Key}");
                    Console.WriteLine($"Actual: {prediction}");
                    Console.WriteLine();
                }
            }

            var customTextInput = GetInputByFile("./training/test.txt");
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

                foreach (var file in langFiles)
                {
                    var input = GetInputByFile(file);
                    for (int i = 0; i < input.Length; i++)
                    {
                        langs[lang][i] += input[i];
                    }
                }

                langs[lang] = NormalizeVector(langs[lang]);
                
                Console.WriteLine();
            }

            return langs;
        }

        public static double[] NormalizeVector(double[] v)
        {
            double sum = 0;
            for (int i = 0; i < v.Length; i++)
            {
                sum += v[i] * v[i];
            }

            sum = Math.Sqrt(sum);
            for (int i = 0; i < v.Length; i++)
            {
                v[i] /= sum;
            }

            return v;
        }

        public static double[] GetInputByFile(string file)
        {
            var bytes = Encoding.ASCII.GetBytes(File.ReadAllText(file).ToLower());
            double[] result = new double[NumberOfLetters];
            
            foreach (var ascii in bytes)
            {
                var key = ascii - FirstASCII;
                if (key >= 0 && key < NumberOfLetters)
                {
                    result[key] += 1;
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
            double errorSignal = 0; // δ = (d−y)y(1−y)
            foreach (var perceptron in Perceptrons)
            {
                double d = perceptron.Key == lang ? 1 : 0;
                double y = perceptron.Value.Predict(inputs); // y
                errorSignal += (d - y) * y * (1 - y); // todo google | δ = (d−y)y(1−y)
                perceptron.Value.UpdateWeights(inputs, errorSignal);
            }

            return errorSignal;
        }

        static string Predict(double[] inputs, bool printConfidence = false)
        {
            string prediction = "";

            double maxConfidence = 0;
            foreach (var lang in InputVectors)
            {
                double value = Perceptrons[lang.Key].Predict(inputs);
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
}