namespace FotNET.NETWORK.LAYERS.ACTIVATION.ACTIVATION_FUNCTION.SELU;

public class SeLu : Function {
    private const double Alpha  = 1.67326324235437728481d;
    private const double Lambda = 1.05070098735548049341d;

    protected override double Activate(double value) => Lambda * value switch {
        > 0  => value,
        <= 0 => Alpha * Math.Exp(value) - Alpha,
        _    => 0
    };

    protected override double Derivation(double value, double activatedValue) => value * (Lambda * activatedValue switch {
        < 0  => Alpha * Math.Exp(activatedValue),
        >= 0 => 1,
        _    => 0
    });
}