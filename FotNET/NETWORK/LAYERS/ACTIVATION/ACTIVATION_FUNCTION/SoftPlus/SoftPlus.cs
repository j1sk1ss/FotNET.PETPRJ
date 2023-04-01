namespace FotNET.NETWORK.LAYERS.ACTIVATION.ACTIVATION_FUNCTION.SoftPlus;

public class SoftPlus : Function {
    protected override double Activate(double value) => Math.Log(1 + Math.Exp(value));

    protected override double Derivation(double value) => value * (1 / (1 + Math.Exp(-value)));
}