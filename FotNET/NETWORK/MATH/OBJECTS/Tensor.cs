namespace FotNET.NETWORK.MATH.OBJECTS {
    public class Tensor {
        /// <summary>
        /// Tensor or stack of matrices
        /// </summary>
        /// <param name="matrix"> Matrix will be converted to tensor </param>
        public Tensor(Matrix matrix) => Channels = new List<Matrix> { matrix };

        /// <summary>
        /// Tensor or stack of matrices
        /// </summary>
        /// <param name="matrix"> List of matrices </param>
        public Tensor(List<Matrix> matrix) => Channels = matrix;

        /// <summary>
        /// Tensor or stack of matrices
        /// </summary>
        /// <param name="x"> X size of tensor </param>
        /// <param name="y"> Y size of tensor </param>
        /// <param name="depth"> Depth of tensor </param>
        public Tensor(int x, int y, int depth) {
            Channels = new List<Matrix>();
            for (var i = 0; i < depth; i++)
                Channels.Add(new Matrix(x, y));
        }
        
        public List<Matrix> Channels { get; protected init; }

        public List<double> Flatten() {
            var flatten = new List<double>();
            foreach (var matrix in Channels) flatten.AddRange(matrix.GetAsList());
            return flatten;
        }

        public Tensor Flip() {
            Parallel.For(0, Channels.Count, channel => {
                Channels[channel].Flip();
            });
            
            return new Tensor(Channels);
        }

        /// <summary>
        /// Fit tensor size with reference
        /// </summary>
        /// <param name="reference"> Reference tensor </param>
        /// <returns> Tensor with same size with reference </returns>
        public Tensor GetSameChannels(Tensor reference) {
            if (Channels.Count != reference.Channels.Count) 
                return Channels.Count < reference.Channels.Count
                    ? IncreaseChannels(reference.Channels.Count - Channels.Count)
                    : CropChannels(reference.Channels.Count);
            
            return this;
        }

        private Tensor IncreaseChannels(int channels) {
            var tensor = new Tensor(Channels);

            for (var i = 0; i < channels; i++) tensor.Channels.Add(Channels[^1]);

            return tensor;
        }

        private Tensor CropChannels(int channels) {
            var matrix = new List<Matrix>();

            for (var i = 0; i < channels * 2; i += 2) {
                matrix.Add(Channels[i]);
                
                for (var x = 0; x < Channels[i].Rows; x++) 
                    for (var y = 0; y < Channels[i].Columns; y++) 
                        matrix[^1].Body[x, y] = Math.Max(Channels[i].Body[x, y], Channels[i + 1].Body[x, y]);
            }

            return new Tensor(matrix);
        }

        /// <summary>
        /// Find max element in tensor
        /// </summary>
        /// <returns> Index of max element </returns>
        public int GetMaxIndex() {
            var values = Flatten();
            var max = values[0];
            var index = 0;

            for (var i = 0; i < values.Count; i++)
                if (max < values[i]) {
                    max = values[i];
                    index = i;
                }

            return index;
        }

        /// <summary>
        /// Returns min value in tensor
        /// </summary>
        /// <returns> Min value in tensor </returns>
        public double Min() => Flatten().Min();
        
        /// <summary>
        /// Returns max value in tensor
        /// </summary>
        /// <returns> Max value in tensor </returns>
        public double Max() => Flatten().Max();
        
        public static Tensor operator +(Tensor tensor1, Tensor tensor2) {
            var endTensor = new Tensor(tensor1.Channels);

            for (var i = 0; i < endTensor.Channels.Count; i++)
                endTensor.Channels[i] = tensor1.Channels[i] + tensor2.Channels[i];

            return endTensor;
        }

        public static Tensor operator -(Tensor firstTensor, Tensor secondTensor) {
            var endTensor = new Tensor(firstTensor.Channels);

            for (var i = 0; i < endTensor.Channels.Count; i++)
                endTensor.Channels[i] = firstTensor.Channels[i] - secondTensor.Channels[i];

            return endTensor;
        }

        public static Tensor operator *(Tensor tensor, Tensor secondTensor) {
            var endTensor = new Tensor(tensor.Channels);

            for (var i = 0; i < endTensor.Channels.Count; i++)
                endTensor.Channels[i] = tensor.Channels[i] * secondTensor.Channels[i];

            return endTensor;
        }

        public static Tensor operator -(Tensor firstTensor, double value) {
            var endTensor = new Tensor(firstTensor.Channels);

            for (var i = 0; i < firstTensor.Channels.Count; i++)
                firstTensor.Channels[i] -= value;

            return endTensor;
        }

        public static Tensor operator *(Tensor tensor, double value) {
            var endTensor = new Tensor(tensor.Channels);

            for (var i = 0; i < tensor.Channels.Count; i++)
                tensor.Channels[i] *= value;

            return endTensor;
        }

        public static Tensor operator /(Tensor tensor, double value) {
            var endTensor = new Tensor(tensor.Channels);

            for (var i = 0; i < tensor.Channels.Count; i++)
                tensor.Channels[i] /= value;

            return endTensor;
        }
        
        public string GetInfo() => $"x: {Channels[0].Rows}\n" +
                                   $"y: {Channels[0].Columns}\n" +
                                   $"depth: {Channels.Count}";
        
        public Filter AsFilter() => new Filter(Channels);
    }

    public class Filter : Tensor {
        public Filter(List<Matrix> matrix) : base(matrix) {
            Bias     = 0;
            Channels = matrix;
        }

        public double Bias { get; set; }

        public Tensor AsTensor() => new Tensor(Channels);
    }
}