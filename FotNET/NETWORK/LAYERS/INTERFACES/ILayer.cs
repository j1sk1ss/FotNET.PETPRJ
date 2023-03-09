using FotNET.NETWORK.OBJECTS;

namespace FotNET.NETWORK.LAYERS.INTERFACES
{
    public interface ILayer
    {
        public Tensor GetNextLayer(Tensor tensor);
        public Tensor BackPropagate(Tensor error);
        public Tensor GetValues();
        public string GetData();
        public string LoadData(string data);
    }
}