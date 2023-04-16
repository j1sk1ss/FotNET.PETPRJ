namespace FotNET.NETWORK.LAYERS.ACTIVATION.ACTIVATION_FUNCTION.HYPERBOLIC_TANGENT {
    public class HyperbolicTangent : Function {
        protected override double Activate(double value) =>
            (Math.Exp(value) - Math.Exp(-value)) / (Math.Exp(value) + Math.Exp(-value));

        protected override double Derivation(double value, double referenceValue) => value * (1 - Math.Pow(Activate(referenceValue), 2));
    }    
}