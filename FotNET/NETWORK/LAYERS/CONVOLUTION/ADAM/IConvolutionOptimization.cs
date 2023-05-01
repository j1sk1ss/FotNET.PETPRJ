using FotNET.NETWORK.MATH.OBJECTS;

namespace FotNET.NETWORK.LAYERS.CONVOLUTION.ADAM;

public interface IConvolutionOptimization {
    public Tensor BackPropagate(Tensor error, double learningRate, bool backPropagate, Tensor input, Filter[] filters,
        bool update, int stride,
        double beta1 = 0.9, double beta2 = 0.999, double epsilon = 1e-8);
}