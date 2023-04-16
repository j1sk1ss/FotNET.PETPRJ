namespace FotNET.NETWORK.LAYERS.ACTIVATION.ACTIVATION_FUNCTION.SiLu;

public class SiLu : Function {
    protected override double Activate(double value) => value / (1 + Math.Exp(-value));

    protected override double Derivation(double value, double referenceValue) => 
        value * ((1 + Math.Exp(-referenceValue) + referenceValue * Math.Exp(-referenceValue))/Math.Pow(1 + Math.Exp(-referenceValue), 2));
}