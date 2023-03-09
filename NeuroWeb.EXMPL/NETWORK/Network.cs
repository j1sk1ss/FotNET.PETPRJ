using System.Collections.Generic;
using System.Linq;
using NeuroWeb.EXMPL.NETWORK.ACTIVATION.INTERFACES;
using NeuroWeb.EXMPL.NETWORK.LAYERS.INTERFACES;
using NeuroWeb.EXMPL.NETWORK.MATH;
using NeuroWeb.EXMPL.NETWORK.OBJECTS;

namespace NeuroWeb.EXMPL.NETWORK
{
    public class Network
    {
        public Network(List<ILayer> layers, IFunction lossFunction)
        {
            Layers = layers;
            MainFunction = lossFunction;
        }

        public List<ILayer> Layers { get; }
        public IFunction MainFunction { get; }

        public int ForwardFeed(Tensor data)
        {
            data = Layers.Aggregate(data, (current, layer) => layer.GetNextLayer(current));
            return Vector.GetMaxIndex(data.Flatten());
        }

        public void BackPropagation(double expectedAnswer)
        {
            var errorTensor = LossFunction.GetLoss(Layers[^1].GetValues(), (int)expectedAnswer, MainFunction);
            for (var i = Layers.Count - 2; i >= 0; i--)
                errorTensor = Layers[i].BackPropagate(errorTensor);
        }
    }
}