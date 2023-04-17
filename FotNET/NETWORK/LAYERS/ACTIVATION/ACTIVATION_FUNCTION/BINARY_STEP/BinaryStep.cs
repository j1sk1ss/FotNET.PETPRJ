namespace FotNET.NETWORK.LAYERS.ACTIVATION.ACTIVATION_FUNCTION.BINARY_STEP;

public class BinaryStep : Function {
    protected override double Activate(double value) => value switch {
        < 0 => 0,
        > 0 => 1,
        _   => 0
    };

    protected override double Derivation(double value, double activatedValue) => value * (1d / Math.Pow(Math.Cos(activatedValue), 2));
}   
