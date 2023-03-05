using System;
using System.Collections.Generic;
using System.Windows;
using NeuroWeb.EXMPL.INTERFACES;
using NeuroWeb.EXMPL.OBJECTS;
using NeuroWeb.EXMPL.OBJECTS.MATH;
using NeuroWeb.EXMPL.OBJECTS.NETWORK;
using NeuroWeb.EXMPL.SCRIPTS.MATH;
using Vector = NeuroWeb.EXMPL.OBJECTS.Vector;

namespace NeuroWeb.EXMPL.LAYERS.PERCEPTRON {
    public class PerceptronLayer : ILayer {
        
        public PerceptronLayer(int size, int nextSize, double learningRate) {
            _learningRate = learningRate;
            
            Neurons      = new double[size];
            NeuronsError = new double[size];
            Bias         = new double[size];

            Weights = new Matrix(nextSize, size);
            Weights.FillRandom();
            
            for (var i = 0; i < size; i++)
                Bias[i] = new Random().Next() % 50 * .06 / (Neurons[i] + 15);
        }
        
        public PerceptronLayer(int size, double learningRate) {
            _learningRate = learningRate;
            
            Neurons      = new double[size];
            NeuronsError = new double[size];
            Bias         = new double[size];
            
            Weights = new Matrix(size, size);
            for (var i = 0; i < size; i++) 
                Weights.Body[i, i] = 1;
        }

        private readonly double _learningRate;
        
        private double[] Neurons { get; set; }
        private double[] Bias { get; }
        private double[] NeuronsError { get; set; }
        private Matrix Weights { get; }

        public double[] GetNextLayer(double[] neurons) {
            var nextLayer = new Vector(Weights * neurons) + new Vector(Bias);
            return NeuronActivate.LeakyReLu(nextLayer);
        }

        public Tensor GetValues() {
            return new Vector(Neurons).AsTensor(1, Neurons.Length, 1);
        }
        
        public Tensor GetNextLayer(Tensor tensor) {
            var temp = tensor.Flatten();
            
            Neurons = temp.ToArray();
            var nextLayer = new Vector(Weights * Neurons) + new Vector(Bias);
            
            return new Vector(nextLayer).AsTensor(1, nextLayer.Length, 1);
        }

        public Tensor BackPropagate(Tensor error) {
            var temp = error.Flatten();
            NeuronsError       = Weights.GetTranspose() * temp.ToArray();
            SetWeights(_learningRate, temp.ToArray());
            return new Vector((new Vector(NeuronsError) * new Vector(NeuronActivate.GetDerivative(NeuronsError))).Body)
                .AsTensor(1, NeuronsError.Length, 1);
        }

        private void SetWeights(double learningRate, IReadOnlyList<double> previousErrors) {
            for (var j = 0; j < Weights.Body.GetLength(0); ++j)
                for (var k = 0; k < Weights.Body.GetLength(1); ++k)
                    Weights.Body[j, k] += Neurons[k] * previousErrors[j] * learningRate;

            for (var j = 0; j < Weights.Body.GetLength(1); j++)
                Bias[j] += NeuronsError[j] * learningRate;
        }
    }
}