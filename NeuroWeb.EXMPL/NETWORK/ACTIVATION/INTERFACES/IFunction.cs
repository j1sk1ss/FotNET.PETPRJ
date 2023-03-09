using NeuroWeb.EXMPL.NETWORK.OBJECTS;

namespace NeuroWeb.EXMPL.NETWORK.ACTIVATION.INTERFACES
{
    public interface IFunction
    {
        public Tensor Activate(Tensor tensor);
        public Tensor Derivation(Tensor tensor);
        public double Derivation(double value);
    }
}