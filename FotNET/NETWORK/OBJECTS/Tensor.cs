namespace FotNET.NETWORK.OBJECTS {
    public class Tensor {
        public Tensor(Matrix matrix) => Channels = new List<Matrix> { matrix };

        public Tensor(List<Matrix> matrix) => Channels = matrix;

        public List<Matrix> Channels { get; protected init; }

        public List<double> Flatten() {
            try {
                var flatten = new List<double>();
                foreach (var matrix in Channels) flatten.AddRange(matrix.GetAsList());
                return flatten;
            }
            catch (Exception) {
                Console.WriteLine("Код ошибки: 1t");
                return null!;
            }
        }

        public Filter Flip() {
            try {
                foreach (var matrix in Channels)
                    matrix.Flip();

                return new Filter(Channels);
            }
            catch (Exception) {
                Console.WriteLine("Код ошибки: 2t");
                return null!;
            }
        }

        public Tensor GetSameChannels(Tensor reference) {
            try {
                if (Channels.Count != reference.Channels.Count) {
                    return Channels.Count < reference.Channels.Count
                        ? IncreaseChannels(reference.Channels.Count - Channels.Count)
                        : CropChannels(reference.Channels.Count);
                }

                return this;
            }
            catch (Exception) {
                Console.WriteLine("Код ошибки: 3t");
                return null!;
            }
        }

        private Tensor IncreaseChannels(int channels) {
            var tensor = new Tensor(Channels);

            for (var i = 0; i < channels; i++) tensor.Channels.Add(
                new Matrix(tensor.Channels[0].Body.GetLength(0), tensor.Channels[0].Body.GetLength(0)));

            return tensor;
        }

        private Tensor CropChannels(int channels) {
            var matrix = new List<Matrix>();

            for (var i = 0; i < channels * 2; i += 2) {
                matrix.Add(Channels[i]);
                
                for (var x = 0; x < Channels[i].Body.GetLength(0); x++) 
                    for (var y = 0; y < Channels[i].Body.GetLength(1); y++) 
                        matrix[^1].Body[x, y] = Math.Max(Channels[i].Body[x, y], Channels[i + 1].Body[x, y]);
            }

            return new Tensor(matrix);
        }

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
        
        public static Tensor operator +(Tensor tensor1, Tensor tensor2) {
            try {
                var endTensor = new Tensor(tensor1.Channels);

                for (var i = 0; i < endTensor.Channels.Count; i++)
                    endTensor.Channels[i] = tensor1.Channels[i] + tensor2.Channels[i];

                return endTensor;
            }
            catch (Exception) {
                Console.WriteLine("Код ошибки: 4t");
                return null!;
            }
        }

        public static Tensor operator -(Tensor tensor1, Tensor tensor2) {
            try {
                var endTensor = new Tensor(tensor1.Channels);

                for (var i = 0; i < endTensor.Channels.Count; i++)
                    endTensor.Channels[i] = tensor1.Channels[i] - tensor2.Channels[i];

                return endTensor;
            }
            catch (Exception) {
                Console.WriteLine("Код ошибки: 5t");
                return null!;
            }
        }

        public static Tensor operator *(Tensor tensor1, Tensor tensor2) {
            try {
                var endTensor = new Tensor(tensor1.Channels);

                for (var i = 0; i < endTensor.Channels.Count; i++)
                    endTensor.Channels[i] = tensor1.Channels[i] * tensor2.Channels[i];

                return endTensor;
            }
            catch (Exception) {
                Console.WriteLine("Код ошибки: 6t");
                return null!;
            }
        }

        public static Tensor operator -(Tensor tensor1, double value) {
            var endTensor = new Tensor(tensor1.Channels);

            for (var i = 0; i < tensor1.Channels.Count; i++)
                tensor1.Channels[i] -= value;

            return endTensor;
        }

        public static Tensor operator *(Tensor tensor1, double value) {
            var endTensor = new Tensor(tensor1.Channels);

            for (var i = 0; i < tensor1.Channels.Count; i++)
                tensor1.Channels[i] *= value;

            return endTensor;
        }

        public string GetInfo() => $"x: {Channels[0].Body.GetLength(0)}\n" +
                                   $"y: {Channels[0].Body.GetLength(1)}\n" +
                                   $"depth: {Channels.Count}";
        
        public Filter AsFilter() => new Filter(Channels);
    }

    public class Filter : Tensor {
        public Filter(List<Matrix> matrix) : base(matrix) {
            Bias = 0;
            Channels = matrix;
        }

        public double Bias { get; set; }

        public Tensor AsTensor() => new Tensor(Channels);
    }
}