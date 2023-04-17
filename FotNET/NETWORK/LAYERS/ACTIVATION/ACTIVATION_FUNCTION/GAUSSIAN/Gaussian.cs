namespace FotNET.NETWORK.LAYERS.ACTIVATION.ACTIVATION_FUNCTION.GAUSSIAN;

public class Gaussian : Function {
    protected override double Activate(double value) => Math.Exp(Math.Pow(-value, 2));

    protected override double Derivation(double value, double activatedValue) => value * (-2 * activatedValue * Math.Exp(Math.Pow(-activatedValue, 2)));
}