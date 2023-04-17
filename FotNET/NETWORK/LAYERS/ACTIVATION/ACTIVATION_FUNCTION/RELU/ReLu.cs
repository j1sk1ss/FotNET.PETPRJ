namespace FotNET.NETWORK.LAYERS.ACTIVATION.ACTIVATION_FUNCTION.RELU {
    public class ReLu : Function {
        protected override double Activate(double value) => Math.Max(0, value);

        protected override double Derivation(double value, double activatedValue) => value * (activatedValue <= 0 ? 0 : 1);
    }
}
