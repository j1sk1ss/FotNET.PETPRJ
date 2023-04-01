namespace FotNET.NETWORK.LAYERS.ACTIVATION.ACTIVATION_FUNCTION.RELU {
    public class ReLu : Function {
        protected override double Activate(double value) => value switch {
            < 0 => .01d,
            _   => value
        };

        protected override double Derivation(double value) => value * value < 0 ? .01d : 1;
    }
}
