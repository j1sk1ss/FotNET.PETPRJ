using FotNET.NETWORK.LAYERS.ACTIVATION.ACTIVATION_FUNCTION;
using FotNET.NETWORK.MATH.OBJECTS;

namespace FotNET.NETWORK.LAYERS.ACTIVATION {
    public class ActivationLayer : ILayer {
        /// <summary> Layer that perform tensor activation. </summary>
        /// <param name="function"> Activation function. </param>
        public ActivationLayer(Function function) {
            Function = function;
            Input    = new Tensor(new List<Matrix>());
        }

        private Function Function { get; }

        private Tensor Input { get; set; }

        public Tensor GetNextLayer(Tensor tensor) {
            Input = Function.Activate(tensor);
            return Input.Copy();
        }

        public Tensor BackPropagate(Tensor error, double learningRate, bool backPropagate) => 
            Function.Derivation(error, Input);
        
        public Tensor GetValues() => Input;
        
        public string GetData() => "";
        
        public string LoadData(string data) => data;
    }
}