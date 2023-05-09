using FotNET.NETWORK.LAYERS.POOLING.SCRIPTS;
using FotNET.NETWORK.MATH.OBJECTS;

namespace FotNET.NETWORK.LAYERS.POOLING {
    public class PoolingLayer : ILayer {
        /// <summary> Layer that perform pooling of tensor. </summary>
        /// <param name="pooling"> Type of pooling. </param>
        /// <param name="poolSize"> Pool size. </param>
        public PoolingLayer(Pooling pooling, int poolSize) {
            Pooling      = pooling;
            _poolSize    = poolSize;
            _inputTensor = new Tensor(new Matrix(0, 0));
        }

        private Pooling Pooling { get; }
        
        private readonly int _poolSize;
        private Tensor _inputTensor;

        public Tensor GetValues() => _inputTensor;

        public Tensor GetNextLayer(Tensor tensor) {
            _inputTensor = tensor.Copy();
            return Pooling.Pool(tensor.Copy(), _poolSize);
        }

        public Tensor BackPropagate(Tensor error, double learningRate, bool backPropagate) =>
             Pooling.BackPool(error.GetSameChannels(_inputTensor), _inputTensor, _poolSize);
        
        public string GetData() => "";
        
        public string LoadData(string data) => data;
    }
}