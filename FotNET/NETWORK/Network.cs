using FotNET.NETWORK.DATA.CSV;
using FotNET.NETWORK.LAYERS;
using FotNET.NETWORK.MATH;
using FotNET.NETWORK.OBJECTS.DATA_OBJECTS;
using FotNET.NETWORK.OBJECTS.MATH_OBJECTS;

namespace FotNET.NETWORK {
    public class Network {
        public Network(List<ILayer> layers) {
            Layers       = layers;
        }

        private List<ILayer> Layers { get; set; }

        public List<ILayer> GetLayers() => Layers;

        public int ForwardFeed(Tensor data) {
            try {
                data = Layers.Aggregate(data, (current, layer) => layer.GetNextLayer(current));
                return data.GetMaxIndex();
            }
            catch (Exception ex) {
                Console.WriteLine("Код ошибки: 1n\n" + ex);
                return 0;
            }
        }

        public void BackPropagation(double expectedAnswer, double expectedValue, double learningRate) {
            try {
                var errorTensor = LossFunction.GetErrorTensor(Layers[^1].GetValues(), (int)expectedAnswer, expectedValue);
                for (var i = Layers.Count - 1; i >= 0; i--)
                    errorTensor = Layers[i].BackPropagate(errorTensor, learningRate);
            }
            catch (Exception ex) {
                Console.WriteLine("Код ошибки: 2n\n" + ex);
            }
        }
        
        public string GetWeights() =>
            Layers.Aggregate(" ", (current, layer) => current + layer.GetData());

        public void LoadWeights(string weights) =>
            Layers.Aggregate(weights, (current, layer) => layer.LoadData(current));

        public void Fit(IData.Type type, string path, DataConfig config, int epochs, double baseLearningRate) =>
             Layers = MODEL.Fit.FitModel(this, Parse(type, path, config), epochs, baseLearningRate).Layers;
        
        public double Test(IData.Type type, string path, DataConfig config) =>
             MODEL.Test.TestModel(this, Parse(type, path, config));

        private static List<IData> Parse(IData.Type type, string path, DataConfig config) => type switch {
            IData.Type.Array => Parser.CsvToArrays(path, config),
            IData.Type.Image => null!,
            _                => null!
        };
    }
}