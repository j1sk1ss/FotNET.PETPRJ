namespace FotNET.NETWORK.LAYERS.ACTIVATION.ACTIVATION_FUNCTION.HARD_SIGMOID;

public class HardSigmoid : Function {
    protected override double Activate(double value) => Math.Max(0, Math.Min(1, (value + 1) / 2));

    protected override double Derivation(double value, double activatedValue) =>
        value * (activatedValue is < 0 or > 1 ? 0 : activatedValue);
}