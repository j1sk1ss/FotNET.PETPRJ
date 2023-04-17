namespace FotNET.NETWORK.LAYERS.ACTIVATION.ACTIVATION_FUNCTION.PRELU;

public class PReLu : Function {
    public PReLu(double alpha) => Alpha = alpha;
    
    private double Alpha { get; }
    
    protected override double Activate(double value) => value switch {
        < 0 => Alpha,
        _   => value
    };

    protected override double Derivation(double value, double activatedValue) => value * (activatedValue < 0 ? Alpha : 1);
}