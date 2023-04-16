namespace FotNET.NETWORK.MATH.LOSS_FUNCTION.ERROR.ONE_BY_ONE;

public class OneByOne : ErrorFunction {
    protected override double Derivation(double prediction, double expected) => 
        prediction * (1 - prediction) * (expected - prediction);
}