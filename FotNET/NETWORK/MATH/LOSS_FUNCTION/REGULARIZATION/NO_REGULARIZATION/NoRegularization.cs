namespace FotNET.NETWORK.MATH.LOSS_FUNCTION.REGULARIZATION.NO_REGULARIZATION;

public class NoRegularization : Regularization {
    public override double GetRegularization() => 0d;
}