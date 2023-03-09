using NeuroWeb.EXMPL.NETWORK.ACTIVATION.INTERFACES;
using NeuroWeb.EXMPL.NETWORK.LAYERS.INTERFACES;
using NeuroWeb.EXMPL.NETWORK.OBJECTS;

namespace NeuroWeb.EXMPL.NETWORK.LAYERS.ACTIVATION
{
    public class ActivationLayer : ILayer
    {
        public ActivationLayer(IFunction function) => Function = function;

        private IFunction Function { get; }

        public Tensor GetNextLayer(Tensor tensor) => Function.Activate(tensor);
        public Tensor BackPropagate(Tensor error) => Function.Derivation(error);

        public Tensor GetValues() => null;
        public string GetData() => "";
        public string LoadData(string data) => data;
    }
}