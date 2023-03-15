using FotNET.NETWORK.LAYERS.POOLING.SCRIPTS;
using FotNET.NETWORK.OBJECTS;

namespace FotNET.NETWORK.LAYERS.POOLING {
    public class PoolingLayer : ILayer {
        public PoolingLayer(Pooling pooling, int poolSize) {
            Pooling      = pooling;
            _poolSize    = poolSize;
            _inputTensor = new Tensor(new Matrix(0, 0));
        }

        private Pooling Pooling { get; set; }
        
        private readonly int _poolSize;
        private Tensor _inputTensor;

        public Tensor GetValues() => _inputTensor;

        public Tensor GetNextLayer(Tensor tensor) {
            _inputTensor = tensor;
            return Pooling.Pool(tensor, _poolSize);
        }

        public Tensor BackPropagate(Tensor error, double learningRate) =>
            Pooling.BackPool(error.GetSameChannels(_inputTensor), _inputTensor, _poolSize);

        public string GetData() => "";
        public string LoadData(string data) => data;
    }
}