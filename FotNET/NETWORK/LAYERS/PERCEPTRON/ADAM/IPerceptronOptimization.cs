using FotNET.NETWORK.MATH.OBJECTS;

namespace FotNET.NETWORK.LAYERS.PERCEPTRON.ADAM;

public interface IPerceptronOptimization {
    public Tensor BackPropagate(Tensor error, double learningRate, bool backPropagate,
        bool isEndLayer, Matrix weights, Vector neurons, Vector bias, double beta1 = 0.9, double beta2 = 0.999,
        double epsilon = 1e-8);
}