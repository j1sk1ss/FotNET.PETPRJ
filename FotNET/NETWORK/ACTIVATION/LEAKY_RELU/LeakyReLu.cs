namespace FotNET.NETWORK.ACTIVATION.LEAKY_RELU {
    public class LeakyReLu : Function {
        protected override double Activate(double value) => value switch {
            < 0 => value * .01d,
            > 1 => 1d + .01d * (value - 1d),
            _   => value
        };

        protected override double Derivation(double value) => value * value is < 0 or > 1 ? .01d : value;
    }
}