using FotNET.NETWORK.LAYERS.PERCEPTRON.ADAM;
using FotNET.NETWORK.LAYERS.PERCEPTRON.ADAM.DEFAULT_PERCEPTRON;
using FotNET.NETWORK.MATH.Initialization;
using FotNET.NETWORK.MATH.OBJECTS;

namespace FotNET.NETWORK.LAYERS.PERCEPTRON {
    public class PerceptronLayer : ILayer {
        /// <summary> Perceptron or Fully-Connected layer. </summary>
        /// <param name="size"> Size of neurons on this layer. </param>
        /// <param name="nextSize"> Size of neurons on second layer. </param>
        /// <param name="weightsInitialization"> Type of weights initialization of filters on layer. </param>
        /// <param name="perceptronOptimization"> Optimization type </param>
        public PerceptronLayer(int size, int nextSize, IWeightsInitialization weightsInitialization, 
            IPerceptronOptimization perceptronOptimization) {
            PerceptronOptimization = perceptronOptimization;
            
            Neurons = new Vector(size);
            Bias    = new Vector(nextSize);
            Weights = weightsInitialization.Initialize(new Matrix(nextSize, size));

            for (var i = 0; i < nextSize; i++)
                Bias[i] = .001d;
            
            _isEndLayer = false;
        }

        /// <summary> Last layer of perceptron. </summary>
        /// <param name="size"> Size of neurons on this layer. </param>
        public PerceptronLayer(int size) {
            PerceptronOptimization = new NoPerceptronOptimization();
            
            Neurons = new Vector(size);
            Bias    = new Vector(size);

            Weights = new Matrix(size, size);
            for (var i = 0; i < size; i++)
                Weights.Body[i, i] = 1;

            _isEndLayer = true;
        }

        private readonly bool _isEndLayer;
        private Vector Neurons { get; set; }
        private Vector Bias { get; }
        private Matrix Weights { get; }
        private IPerceptronOptimization PerceptronOptimization { get; }

        public Tensor GetValues() => Neurons.AsTensor(1, Neurons.Size, 1);

        public Tensor GetNextLayer(Tensor tensor) {
            Neurons = new Vector(tensor.Flatten().ToArray());
            var nextLayer = Neurons * Weights + Bias;
            return nextLayer.AsTensor(1, nextLayer.Size, 1);
        }

        public Tensor BackPropagate(Tensor error, double learningRate, bool backPropagate) =>
            PerceptronOptimization.BackPropagate(error, learningRate, backPropagate, _isEndLayer, Weights, Neurons,
                Bias);

        public string GetData() => Weights.GetValues() + Bias.Print();

        public string LoadData(string data) {
            var position = 0;
            var dataNumbers = data.Split(" ", StringSplitOptions.RemoveEmptyEntries);

            for (var i = 0; i < Weights.Rows; i++)
                for (var j = 0; j < Weights.Columns; j++)
                    Weights.Body[i, j] = double.Parse(dataNumbers[position++]);

            for (var j = 0; j < Bias.Size; j++)
                Bias[j] = double.Parse(dataNumbers[position++]);

            return string.Join(" ", dataNumbers.Skip(position).Select(p => p.ToString()).ToArray());
        }
    }
}