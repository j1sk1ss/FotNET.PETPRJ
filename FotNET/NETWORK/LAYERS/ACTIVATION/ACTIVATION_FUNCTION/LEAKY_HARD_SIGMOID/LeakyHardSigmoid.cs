namespace FotNET.NETWORK.LAYERS.ACTIVATION.ACTIVATION_FUNCTION.LEAKY_HARD_SIGMOID;

public class LeakyHardSigmoid : Function {
    protected override double Activate(double value) => 
        Math.Max(value * .01d, Math.Min(1d + .01d * (value - 1d), (value + 1) / 2));

    protected override double Derivation(double value, double activatedValue) =>
        value * (activatedValue is < 0 or > 1 ? .01d : activatedValue);
}