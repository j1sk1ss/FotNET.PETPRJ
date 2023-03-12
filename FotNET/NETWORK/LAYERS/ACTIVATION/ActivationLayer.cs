using FotNET.NETWORK.ACTIVATION;
using FotNET.NETWORK.OBJECTS;

namespace FotNET.NETWORK.LAYERS.ACTIVATION {
    public class ActivationLayer : ILayer {
        public ActivationLayer(Function function) => Function = function;

        private Function Function { get; }

        public Tensor GetNextLayer(Tensor tensor) => Function.Activate(tensor);
        public Tensor BackPropagate(Tensor error) => Function.Derivation(error);

        public Tensor GetValues() => new Tensor(new Matrix(0,0));
        public string GetData() => "";
        public string LoadData(string data) => data;
    }
}