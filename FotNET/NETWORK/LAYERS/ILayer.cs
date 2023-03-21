using FotNET.NETWORK.OBJECTS.MATH_OBJECTS;

namespace FotNET.NETWORK.LAYERS {
    public interface ILayer {
        public Tensor GetNextLayer(Tensor tensor);
        public Tensor BackPropagate(Tensor error, double learningRate);
        public Tensor GetValues();
        public string GetData();
        public string LoadData(string data);
    }
}