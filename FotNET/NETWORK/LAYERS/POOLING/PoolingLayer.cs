using FotNET.NETWORK.LAYERS.INTERFACES;
using FotNET.NETWORK.LAYERS.POOLING.SCRIPTS;
using FotNET.NETWORK.OBJECTS;

namespace NeuroWeb.EXMPL.NETWORK.LAYERS.POOLING
{
    public class PoolingLayer : ILayer
    {
        public PoolingLayer(int poolSize)
        {
            _poolSize = poolSize;
        }

        private readonly int _poolSize;
        private Tensor _inputTensor;

        public Tensor GetValues() => _inputTensor;

        public Tensor GetNextLayer(Tensor tensor)
        {
            _inputTensor = tensor;
            return Pooling.MaxPool(tensor, _poolSize);
        }

        public Tensor BackPropagate(Tensor error) =>
            Pooling.BackMaxPool(error.GetSameChannels(_inputTensor), _inputTensor, _poolSize);

        public string GetData() => "";
        public string LoadData(string data) => data;
    }
}