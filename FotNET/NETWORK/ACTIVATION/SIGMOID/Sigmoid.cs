namespace FotNET.NETWORK.ACTIVATION.SIGMOID {
    public class Sigmoid : Function {
        protected override double Activate(double value) => 1 / (1 + Math.Exp(-value));
        public override double Derivation(double value) => value * (Activate(value) * (1 - Activate(value)));
    }
}