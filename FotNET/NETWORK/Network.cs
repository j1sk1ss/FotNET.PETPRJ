using FotNET.NETWORK.FIT.DATA_OBJECTS;
using FotNET.NETWORK.LAYERS;
using FotNET.NETWORK.MATH;
using FotNET.NETWORK.OBJECTS;

namespace FotNET.NETWORK {
    public class Network {
        public Network(List<ILayer> layers) {
            Layers       = layers;
        }

        private List<ILayer> Layers { get; }

        public List<ILayer> GetLayers() => Layers;

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
            Layers.Aggregate(" ", (current, layer) => current + layer.GetData());

        public void LoadWeights(string weights) =>
            Layers.Aggregate(weights, (current, layer) => layer.LoadData(current));

        public Network Fit(List<IData> dataSet, int epochs) =>
            FIT.Fit.FitModel(this, dataSet, epochs);

        public double Test(List<IData> dataSet) =>
            FIT.Fit.TestModel(this, dataSet);
    }
}