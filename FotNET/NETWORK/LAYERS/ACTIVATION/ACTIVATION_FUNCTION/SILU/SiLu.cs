namespace FotNET.NETWORK.LAYERS.ACTIVATION.ACTIVATION_FUNCTION.SiLu;

public class SiLu : Function {
    protected override double Activate(double value) => value / (1 + Math.Exp(-value));

    protected override double Derivation(double value, double activatedValue) => 
        value * ((1 + Math.Exp(-activatedValue) + activatedValue * Math.Exp(-activatedValue))/Math.Pow(1 + Math.Exp(-activatedValue), 2));
}