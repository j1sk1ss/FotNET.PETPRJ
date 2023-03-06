using NeuroWeb.EXMPL.INTERFACES;
using NeuroWeb.EXMPL.OBJECTS.NETWORK;

namespace NeuroWeb.EXMPL.LAYERS.CHANNEL {
    public class ChannelNormalizer : ILayer {
        private Tensor _referenceTensor;
        
        public Tensor GetNextLayer(Tensor tensor) {
            _referenceTensor = tensor;
            return tensor;
        }

        public Tensor BackPropagate(Tensor error) {
            return error.GetSameChannels(_referenceTensor);
        }

        public Tensor GetValues() {
            throw new System.NotImplementedException();
        }

        public string GetData() => "";
        public string LoadData(string data) => data;
    }
}