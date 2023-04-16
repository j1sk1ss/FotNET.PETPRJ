namespace FotNET.NETWORK.LAYERS.ACTIVATION.ACTIVATION_FUNCTION.LEAKY_RELU;

public class LeakyReLu : Function {
    protected override double Activate(double value) => value switch {
        < 0 => value * .01d,
        _   => value
    };

    protected override double Derivation(double value, double referenceValue) => value * referenceValue switch {
        < 0  => .01d,
        >= 0 => 1,
        _    => 0
    };
}