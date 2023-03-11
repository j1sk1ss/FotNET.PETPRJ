namespace FotNET.NETWORK.ACTIVATION.RELU {
    public class ReLu : Function {
        protected override double Activate(double value) => value switch {
            < 0 => value * .01d,
            _   => value
        };
        
        public override double Derivation(double value) => value * value < 0 ? .01d : 1;
    }
}
