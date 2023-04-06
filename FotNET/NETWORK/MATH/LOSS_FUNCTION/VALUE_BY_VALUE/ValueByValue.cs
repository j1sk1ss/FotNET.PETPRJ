namespace FotNET.NETWORK.MATH.LOSS_FUNCTION.VALUE_BY_VALUE;

public class ValueByValue : LossFunction {
    protected override double Derivation(double prediction, double expected) => prediction - expected;
}