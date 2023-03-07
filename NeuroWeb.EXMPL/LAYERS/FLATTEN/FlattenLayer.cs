using NeuroWeb.EXMPL.LAYERS.INTERFACES;
using NeuroWeb.EXMPL.OBJECTS;
using NeuroWeb.EXMPL.OBJECTS.MATH;
using NeuroWeb.EXMPL.OBJECTS.NETWORK;

namespace NeuroWeb.EXMPL.LAYERS.FLATTEN {
    public class FlattenLayer : ILayer {
        private Tensor _inputTensor;
        
        public Tensor GetValues() => _inputTensor;
                
        public Tensor GetNextLayer(Tensor tensor) {
            _inputTensor = tensor;
            return new Vector(tensor.Flatten().ToArray()).AsTensor(1, tensor.Flatten().Count, 1);
        }

        public Tensor BackPropagate(Tensor error) =>
             new Vector(error.Flatten().ToArray()).AsTensor(_inputTensor.Channels[0].Body.GetLength(0),
                _inputTensor.Channels[0].Body.GetLength(1), _inputTensor.Channels.Count);
        
        public string GetData() => "";
        public string LoadData(string data) => data;
    }
}