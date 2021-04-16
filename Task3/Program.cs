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

        public static Dictionary<string, double[]> TrainingInputVectors;
        public static Dictionary<string, IEnumerable<double[]>> TestingInputVectors;

        static void Main(string[] args)
        {
            TrainingInputVectors = GetInputs("./training"); // key = lang code, value => input vector (x)
            double alpha = .25;

            foreach (var lang in TrainingInputVectors.Keys)
            {
                Perceptrons.Add(lang, new Perceptron(NumberOfLetters, alpha));
            }

            var generations = 100000;
            var globalErrorRate = .0;
            for (var g = 0; g < generations; g++)
            {
                foreach (var lang in TrainingInputVectors)
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

            TestingInputVectors = GetTestingInputs("./testing"); // key = lang code, value => input vector (x)
            var total = 0;
            var correct = 0;
            foreach (var keyValuePair in TestingInputVectors)
            {
                foreach (var inputVector in keyValuePair.Value)
                {
                    string prediction = Predict(inputVector);
                    if (keyValuePair.Key != prediction)
                    {
                        Console.WriteLine($"Expected: {keyValuePair.Key}");
                        Console.WriteLine($"Actual: {prediction}");
                        Console.WriteLine();
                    }
                    else
                    {
                        correct++;
                    }

                    total++;
                }
            }
            
            Console.WriteLine($"Correctness: {correct} of {total} were predicted correctly");

            var customTextInput = NormalizeVector(GetInputByFile("./testing/test.txt"));
            
            Console.WriteLine();
            var result = Predict(customTextInput, true);
            //Console.WriteLine(String.Join("|", customTextInput));
            Console.WriteLine($"test.txt Prediction: {result}");
        }

        private static Dictionary<string, double[]> GetInputs(string dir)
        {
            var langsDirs = Directory.GetDirectories(dir);

            var langs = new Dictionary<string, double[]>(); // "en" => [ASCII => 0.6, ASCII => .8], pl => [...]
            foreach (string langDir in langsDirs)
            {
                var langFiles = Directory.GetFiles(langDir);

                if (langFiles.Length == 0)
                {
                    continue;
                }
                
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

        private static Dictionary<string, IEnumerable<double[]>> GetTestingInputs(string dir)
        {
            var langsDirs = Directory.GetDirectories(dir);

            var langs = new Dictionary<string, IEnumerable<double[]>>(); // "en" => [ASCII => 0.6, ASCII => .8], pl => [...]
            foreach (string langDir in langsDirs)
            {
                var langFiles = Directory.GetFiles(langDir);

                if (langFiles.Length == 0)
                {
                    continue;
                }
                
                var lang = langDir.Split("/").Last();
                var inputs = new List<double[]>();
                langs.Add(lang, inputs);

                foreach (var file in langFiles)
                {
                    var input =   NormalizeVector(GetInputByFile(file));
                    inputs.Add(input);
                }
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

            double maxConfidence = .0;
            foreach (var lang in TrainingInputVectors)
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