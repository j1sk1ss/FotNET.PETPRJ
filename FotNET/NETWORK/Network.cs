using FotNET.DATA;
using FotNET.NETWORK.LAYERS;
using FotNET.NETWORK.MATH.LOSS_FUNCTION;
using FotNET.NETWORK.OBJECTS.DATA_OBJECTS;
using FotNET.NETWORK.OBJECTS.MATH_OBJECTS;

namespace FotNET.NETWORK {
    public class Network {
        public Network(List<ILayer> layers) => Layers = layers;
        
        private List<ILayer> Layers { get; set; }

        public List<ILayer> GetLayers() => Layers;

        public double ForwardFeed(Tensor data, AnswerType answerType) {
            var tensor = Layers.Aggregate(data, (current, layer) => layer.GetNextLayer(current));
            return answerType switch {
                AnswerType.Class => tensor.GetMaxIndex(),
                AnswerType.Value => tensor.Flatten()[tensor.GetMaxIndex()],
                _                => 0
            };
        }

        public void BackPropagation(double expectedAnswer, double expectedValue, LossFunction lossFunction, double learningRate) {
            var errorTensor = lossFunction.GetErrorTensor(Layers[^1].GetValues(), (int)expectedAnswer, expectedValue);
                for (var i = Layers.Count - 1; i >= 0; i--)
                    errorTensor = Layers[i].BackPropagate(errorTensor, learningRate);
        }
        
        public string GetWeights() =>
            Layers.Aggregate(" ", (current, layer) => current + layer.GetData());

        public void LoadWeights(string weights) =>
            Layers.Aggregate(weights, (current, layer) => layer.LoadData(current));

        public void Fit(IData.Type type, string path, Config config, int epochs, LossFunction lossFunction, double baseLearningRate) =>
             Layers = MODEL.Fit.FitModel(this, Parse(type, path, config), epochs, lossFunction, baseLearningRate).Layers;
        
        public double Test(IData.Type type, string path, Config config) =>
             MODEL.Test.TestModel(this, Parse(type, path, config));

        private static List<IData> Parse(IData.Type type, string path, Config config) => type switch {
            IData.Type.Array => DATA.CSV.Parser.CsvToArrays(path, config),
            IData.Type.Image => DATA.IMAGE.Parser.FilesToImages(path, config.LabelPath),
            _                => null!
        };
    }
}