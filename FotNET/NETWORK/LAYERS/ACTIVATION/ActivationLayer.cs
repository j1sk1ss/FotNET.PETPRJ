using FotNET.NETWORK.LAYERS.ACTIVATION.ACTIVATION_FUNCTION;
using FotNET.NETWORK.OBJECTS.MATH_OBJECTS;

namespace FotNET.NETWORK.LAYERS.ACTIVATION {
    public class ActivationLayer : ILayer {
        /// <summary> Layer that perform tensor activation. </summary>
        /// <param name="function"> Activation function. </param>
        public ActivationLayer(Function function) => Function = function;

        private Function Function { get; }

        public Tensor GetNextLayer(Tensor tensor) => Function.Activate(tensor);
        
        public Tensor BackPropagate(Tensor error, double learningRate, bool backPropagate) => Function.Derivation(error);

        public Tensor GetValues() => null!;
        
        public string GetData() => "";
        
        public string LoadData(string data) => data;
    }
}