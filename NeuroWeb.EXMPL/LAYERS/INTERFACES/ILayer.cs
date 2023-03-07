using NeuroWeb.EXMPL.OBJECTS;
using NeuroWeb.EXMPL.OBJECTS.NETWORK;

namespace NeuroWeb.EXMPL.LAYERS.INTERFACES {
    public interface ILayer {
        public Tensor GetNextLayer(Tensor tensor);
        public Tensor BackPropagate(Tensor error);
        public Tensor GetValues();
        public string GetData();
        public string LoadData(string data);
    }
}