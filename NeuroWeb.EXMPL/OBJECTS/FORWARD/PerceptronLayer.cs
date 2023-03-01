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
        
        public void SetWeights(double learningRange) {
            for (var j = 0; j < Weights.Body.GetLength(0); ++j)
                for (var k = 0; k < Weights.Body.GetLength(1); ++k)
                    Weights.Body[j, k] += Neurons[k] * NeuronsError[j] * learningRange;
            
            for (var j = 0; j < Weights.Body.GetLength(1); j++)
                Bias[j] += NeuronsError[j] * learningRange;
        }
    }
}