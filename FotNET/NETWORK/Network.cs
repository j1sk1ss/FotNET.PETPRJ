using FotNET.NETWORK.LAYERS;
using FotNET.NETWORK.MATH;
using FotNET.NETWORK.OBJECTS;

namespace FotNET.NETWORK {
    public class Network {
        public Network(List<ILayer> layers) {
            Layers       = layers;
        }

        public List<ILayer> Layers { get; }

        public int ForwardFeed(Tensor data) {
            try {
                data = Layers.Aggregate(data, (current, layer) => layer.GetNextLayer(current));
                return data.GetMaxIndex();
            }
            catch (Exception) {
                Console.WriteLine("Код ошибки: 1n");
                return 0;
            }
        }

        public void BackPropagation(double expectedAnswer) {
            try {
                var errorTensor = LossFunction.GetErrorTensor(Layers[^1].GetValues(), (int)expectedAnswer);
                for (var i = Layers.Count - 1; i >= 0; i--)
                    errorTensor = Layers[i].BackPropagate(errorTensor);
            }
            catch (Exception) {
                Console.WriteLine("Код ошибки: 2n");
            }
        }
        
        public string GetWeights() =>
            Layers.Aggregate("", (current, layer) => current + layer.GetData());

        public void LoadWeights(string Weights) {
            foreach (var layer in Layers) {
                Weights = layer.LoadData(Weights);
            }
        }
    }
}