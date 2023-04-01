namespace FotNET.NETWORK.LAYERS.ACTIVATION.ACTIVATION_FUNCTION.SIGMOID {
    public class Sigmoid : Function {
        protected override double Activate(double value) =>  1 / (1 + Math.Exp(-value));
        protected override double Derivation(double value) => value * (Activate(value) * (1 - Activate(value)));
    }
}