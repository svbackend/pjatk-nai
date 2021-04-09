using System;

namespace Task3
{
    public class Perceptron
    {
        public double[] Weights;
        public double Alpha;
        public double Bias = .1;

        public Perceptron(int numberOfLetters, double alpha)
        {
            Weights = new double[numberOfLetters];

            for (int i = 0; i < Weights.Length; i++)
            {
                Weights[i] = 0;
            }

            Alpha = alpha;
        }

        public double Predict(double[] inputs)
        {
            double net = 0;
            for (int i = 0; i < inputs.Length; i++)
            {
                net += inputs[i] * Weights[i] - Bias; // w*x−θ
            }

            return 1 / (1 + Math.Exp(-net)); // sigmoid unipolar function 1 / ( 1 + e^(-net) )
        }

        public void UpdateWeights(double[] inputs, double errorSignal)
        {
            for (int i = 0; i < inputs.Length; i++)
            {
                Weights[i] += Alpha * errorSignal * inputs[i]; // w′ = W+αδxT
            }

            Bias -= Alpha * errorSignal;  // θ'=θ−αδ
        }
    }
}