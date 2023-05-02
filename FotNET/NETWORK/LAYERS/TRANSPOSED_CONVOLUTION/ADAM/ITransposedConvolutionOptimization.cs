using FotNET.NETWORK.MATH.OBJECTS;

namespace FotNET.NETWORK.LAYERS.TRANSPOSED_CONVOLUTION.ADAM;

public interface ITransposedConvolutionOptimization {
    public Tensor BackPropagate(Tensor error, double learningRate, bool backPropagate, Tensor input, Filter[] filters,
        int stride, double beta1 = 0.9, double beta2 = 0.999, double epsilon = 1e-8);
}