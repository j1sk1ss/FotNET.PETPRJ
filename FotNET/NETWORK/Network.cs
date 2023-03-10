using FotNET.NETWORK.ACTIVATION.INTERFACES;
using FotNET.NETWORK.LAYERS.INTERFACES;
using FotNET.NETWORK.MATH;
using FotNET.NETWORK.OBJECTS;

namespace FotNET.NETWORK {
    public class Network {
        public Network(List<ILayer> layers, IFunction lossFunction) {
            Layers       = layers;
            MainFunction = lossFunction;
        }

        private List<ILayer> Layers { get; }
        private IFunction MainFunction { get; }

        public Tensor GetLayerData(int layer) => Layers[layer].GetValues();
        
        public int ForwardFeed(Tensor data) {
            data = Layers.Aggregate(data, (current, layer) => layer.GetNextLayer(current));
            return data.GetMaxIndex();
        }

        public void BackPropagation(double expectedAnswer) {
            var errorTensor = LossFunction.GetErrorTensor(Layers[^1].GetValues(), (int)expectedAnswer, MainFunction);
            for (var i = Layers.Count - 1; i >= 0; i--)
                errorTensor = Layers[i].BackPropagate(errorTensor);
        }
    }
}