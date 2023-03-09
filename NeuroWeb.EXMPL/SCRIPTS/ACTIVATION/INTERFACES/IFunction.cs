using NeuroWeb.EXMPL.OBJECTS;

namespace NeuroWeb.EXMPL.SCRIPTS.ACTIVATION.INTERFACES {
    public interface IFunction {
        public Tensor Activate(Tensor tensor);
        public Tensor Derivation(Tensor tensor);
        public double Derivation(double value);
    }
}