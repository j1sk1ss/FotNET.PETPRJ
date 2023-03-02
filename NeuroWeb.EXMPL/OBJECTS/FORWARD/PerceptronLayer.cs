using System;
using NeuroWeb.EXMPL.SCRIPTS;

namespace NeuroWeb.EXMPL.OBJECTS.FORWARD {
    public class PerceptronLayer {
        
        public PerceptronLayer(int size, int nextSize) {
            Neurons      = new double[size];
            NeuronsError = new double[size];
            Bias         = new double[size];

            Weights = new Matrix(nextSize, size);
            Weights.FillRandom();
            
            for (var i = 0; i < size; i++)
                Bias[i] = new Random().Next() % 50 * .06 / (Neurons[i] + 15);
        }
        
        public PerceptronLayer(int size) {
            Neurons      = new double[size];
            NeuronsError = new double[size];
            Bias         = new double[size];
        }
        
        public double[] Neurons { get; set; }
        public double[] Bias { get; set; }
        public double[] NeuronsError { get; set; }
        public Matrix Weights { get; set; }

        public double[] GetNextLayer() {
            var nextLayer = new Vector(Weights * Neurons) + new Vector(Bias);
            return NeuronActivate.Activation(nextLayer);
        }
        
        public void SetWeights(double learningRange, PerceptronLayer nextLayer) {
            for (var j = 0; j < Weights.Body.GetLength(0); ++j) {
                for (var k = 0; k < Weights.Body.GetLength(1); ++k) {
                    var gradient = 0.0d;
                    for (var neuron = 0; neuron < nextLayer.NeuronsError.Length; neuron++) {
                        gradient += nextLayer.NeuronsError[neuron] * NeuronActivate.GetDerivative(Neurons[j]);
                    }
                    Weights.Body[j, k] += learningRange * gradient;
                }
            }

            for (var j = 0; j < Bias.Length; j++) {
                Bias[j] += learningRange * NeuronsError[j];
            }
        }
    }
}