using FotNET.NETWORK.MATH.Initialization;
using FotNET.NETWORK.OBJECTS.MATH_OBJECTS;

namespace FotNET.NETWORK.LAYERS.PERCEPTRON {
    public class PerceptronLayer : ILayer {
        /// <summary> Perceptron or Fully-Connected layer. </summary>
        /// <param name="size"> Size of neurons on this layer. </param>
        /// <param name="nextSize"> Size of neurons on second layer. </param>
        /// <param name="weightsInitialization"> Type of weights initialization of filters on layer. </param>
        public PerceptronLayer(int size, int nextSize, IWeightsInitialization weightsInitialization) {
            Neurons = new double[size];
            Bias    = new double[nextSize];
            Weights = weightsInitialization.Initialize(new Matrix(nextSize, size));

            for (var i = 0; i < nextSize; i++)
                Bias[i] = .001d;
            
            _isEndLayer = false;
        }

        /// <summary> Last layer of perceptron. </summary>
        /// <param name="size"> Size of neurons on this layer. </param>
        public PerceptronLayer(int size) {
            Neurons = new double[size];
            Bias    = new double[size];

            Weights = new Matrix(size, size);
            for (var i = 0; i < size; i++)
                Weights.Body[i, i] = 1;

            _isEndLayer = true;
        }

        private readonly bool _isEndLayer;
        private double[] Neurons { get; set; }
        private double[] Bias { get; }
        private Matrix Weights { get; }

        public Tensor GetValues() => new Vector(Neurons).AsTensor(1, Neurons.Length, 1);

        public Tensor GetNextLayer(Tensor tensor) {
            Neurons = tensor.Flatten().ToArray();
            var nextLayer = new Vector(Neurons * Weights) + new Vector(Bias);
            return new Vector(nextLayer).AsTensor(1, nextLayer.Length, 1);
        }

        public Tensor BackPropagate(Tensor error, double learningRate) {
            var previousError = error.Flatten().ToArray();
            if (_isEndLayer) return new Vector(previousError).AsTensor(1, previousError.Length, 1);
            
            for (var j = 0; j < Weights.Rows; ++j)
                for (var k = 0; k < Weights.Columns; ++k)
                    Weights.Body[j, k] -= Neurons[k] * previousError[j] * learningRate;

            for (var j = 0; j < Weights.Rows; j++)
                Bias[j] -= previousError[j] * learningRate;

            var neuronsError = previousError * Weights.Transpose();
            return new Vector(neuronsError).AsTensor(1, neuronsError.Length, 1);
        }

        public string GetData() {
            var temp = "";
            temp += Weights.GetValues();
            return Bias.Aggregate(temp, (current, bias) => current + bias + " ");
        }

        public string LoadData(string data) {
            var position = 0;
            var dataNumbers = data.Split(" ", StringSplitOptions.RemoveEmptyEntries);

            for (var i = 0; i < Weights.Rows; i++)
                for (var j = 0; j < Weights.Columns; j++)
                    Weights.Body[i, j] = double.Parse(dataNumbers[position++]);

            for (var j = 0; j < Bias.Length; j++)
                Bias[j] = double.Parse(dataNumbers[position++]);

            return string.Join(" ", dataNumbers.Skip(position).Select(p => p.ToString()).ToArray());
        }
    }
}