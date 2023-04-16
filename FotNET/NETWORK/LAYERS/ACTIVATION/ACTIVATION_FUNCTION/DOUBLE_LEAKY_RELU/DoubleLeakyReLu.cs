namespace FotNET.NETWORK.LAYERS.ACTIVATION.ACTIVATION_FUNCTION.DOUBLE_LEAKY_RELU {
    public class DoubleLeakyReLu : Function {
        protected override double Activate(double value) => value switch {
            < 0 => value * .01d,
            > 1 => 1d + .01d * (value - 1d),
            _   => value
        };

        protected override double Derivation(double value, double referenceValue) => 
            value * referenceValue is < 0 or > 1 ? .01d : referenceValue;
    }
}