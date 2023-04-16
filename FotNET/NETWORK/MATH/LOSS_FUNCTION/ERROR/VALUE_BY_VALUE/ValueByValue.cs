namespace FotNET.NETWORK.MATH.LOSS_FUNCTION.ERROR.VALUE_BY_VALUE;

public class ValueByValue : ErrorFunction {
    protected override double Derivation(double prediction, double expected) => prediction - expected;
}