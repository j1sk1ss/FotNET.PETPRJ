using FotNET.NETWORK.MATH.OBJECTS;

namespace FotNET.NETWORK.MATH.LOSS_FUNCTION;

public abstract class LossFunction {
    protected static double Loss(double sum, int count) => 1d / count * sum;
    
    public abstract double GetLoss(Tensor outputTensor, Tensor expectedTensor);

    public abstract Tensor GetErrorTensor(Tensor outputTensor, Tensor expectedTensor);
}