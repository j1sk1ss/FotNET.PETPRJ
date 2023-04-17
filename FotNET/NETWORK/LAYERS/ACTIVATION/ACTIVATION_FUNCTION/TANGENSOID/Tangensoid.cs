namespace FotNET.NETWORK.LAYERS.ACTIVATION.ACTIVATION_FUNCTION.TANGENSOID;

public class Tangensoid : Function {
    protected override double Activate(double value) => 2d / (1d + Math.Exp(-2d * value)) - 1d;

    protected override double Derivation(double value, double activatedValue) => 
        value * (4 * Math.Exp(-2 * activatedValue) / Math.Pow(1 + Math.Exp(-2 * activatedValue), 2));
}