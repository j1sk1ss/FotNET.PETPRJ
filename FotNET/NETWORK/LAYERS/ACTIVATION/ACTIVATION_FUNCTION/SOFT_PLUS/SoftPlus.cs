namespace FotNET.NETWORK.LAYERS.ACTIVATION.ACTIVATION_FUNCTION.SOFT_PLUS;

public class SoftPlus : Function {
    protected override double Activate(double value) => Math.Log(1 + Math.Exp(value));

    protected override double Derivation(double value, double activatedValue) => value * (1 / (1 + Math.Exp(-activatedValue)));
}