namespace FotNET.NETWORK.ACTIVATION.TANGENSOID;

public class Tangensoid : Function {
    protected override double Activate(double value) => value switch {
        < 0 => .01d * (1 - Math.Pow(value, 2)),
        _   => 1 - Math.Pow(value, 2)
    };

    public override double Derivation(double value) => value switch {
        < 0 => .01 * (Math.Exp(value) - Math.Exp(-value)) / (Math.Exp(value) + Math.Exp(-value)),
        _   => (Math.Exp(value) - Math.Exp(-value)) / (Math.Exp(value) + Math.Exp(-value))
    };
}