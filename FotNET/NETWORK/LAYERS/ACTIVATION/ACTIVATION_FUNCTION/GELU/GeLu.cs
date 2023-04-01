namespace FotNET.NETWORK.LAYERS.ACTIVATION.ACTIVATION_FUNCTION.GELU;

public class GeLu : Function {
    protected override double Activate(double value) {
        var cdf = 0.5 * (1.0 + Math.Tanh(Math.Sqrt(2.0 / Math.PI) * (value + 0.044715 * Math.Pow(value, 3)))); 
        var pdf = Math.Exp(-Math.Pow(value, 2) / 2.0) / Math.Sqrt(2.0 * Math.PI); 
        return 0.5 * (1.0 + Math.Tanh(Math.Sqrt(2.0 / Math.PI) * (value + 0.044715d * Math.Pow(value, 3)))) + value * cdf * pdf; 
    }

    protected override double Derivation(double value) => value * (0.5 * value * (1.0 + Math.Tanh(Math.Sqrt(2.0 / Math.PI) * (value + 0.044715 * Math.Pow(value, 3)))));
}