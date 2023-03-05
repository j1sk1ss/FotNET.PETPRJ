using NeuroWeb.EXMPL.INTERFACES;
using NeuroWeb.EXMPL.OBJECTS.NETWORK;
using NeuroWeb.EXMPL.SCRIPTS.POOLING;

namespace NeuroWeb.EXMPL.LAYERS.POOLING {
    public class PoolingLayer : ILayer {
        public PoolingLayer(int poolSize) {
            _poolSize = poolSize;
        }
        
        private readonly int _poolSize;
        private Tensor _inputTensor;
        
        public Tensor GetValues() {
            return _inputTensor;
        }
        
        public Tensor GetNextLayer(Tensor tensor) {
            _inputTensor = tensor;
            return Pooling.MaxPool(tensor, _poolSize);
        }

        public Tensor BackPropagate(Tensor error) {
            return Pooling.BackMaxPool(error.GetSameChannels(_inputTensor), _inputTensor, _poolSize);
        }
    }
}