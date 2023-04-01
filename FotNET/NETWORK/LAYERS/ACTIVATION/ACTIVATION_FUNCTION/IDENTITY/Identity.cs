namespace FotNET.NETWORK.LAYERS.ACTIVATION.ACTIVATION_FUNCTION.IDENTITY;

public class Identity : Function {
    protected override double Activate(double value) => value;

    protected override double Derivation(double value) => value;
}