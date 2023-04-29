using FotNET.NETWORK;
using FotNET.NETWORK.LAYERS;
using FotNET.NETWORK.LAYERS.ACTIVATION;
using FotNET.NETWORK.LAYERS.ACTIVATION.ACTIVATION_FUNCTION.DOUBLE_LEAKY_RELU;
using FotNET.NETWORK.LAYERS.PERCEPTRON;
using FotNET.NETWORK.MATH.Initialization.HE;

namespace FotNET.MODELS.IMAGE_CLASSIFICATION;

public static class ClassicClassification {
    /// <summary>
    /// Simple perceptron for MNIST data set. Takes 28x28 tensor.
    /// </summary>
    public static Network SimplePerceptron = new Network(new List<ILayer> {
        //new PerceptronLayer(784, 256, new HeInitialization()),
        new ActivationLayer(new DoubleLeakyReLu()),
        //new PerceptronLayer(256, 128, new HeInitialization()),
        new ActivationLayer(new DoubleLeakyReLu()),
        //new PerceptronLayer(128, 10, new HeInitialization()),
        new ActivationLayer(new DoubleLeakyReLu()),
        new PerceptronLayer(10)
    });

    /// <summary>
    /// Big perceptron for MNIST data set. Takes 28x28 tensor.
    /// </summary>
    public static Network DeepPerceptron = new Network(new List<ILayer> {
        //new PerceptronLayer(784, 256, new HeInitialization()),
        new ActivationLayer(new DoubleLeakyReLu()),
        //new PerceptronLayer(256, 256, new HeInitialization()),
        new ActivationLayer(new DoubleLeakyReLu()),
        //new PerceptronLayer(256, 256, new HeInitialization()),
        new ActivationLayer(new DoubleLeakyReLu()),
        //new PerceptronLayer(256, 128, new HeInitialization()),
        new ActivationLayer(new DoubleLeakyReLu()),
        //new PerceptronLayer(128, 10, new HeInitialization()),
        new ActivationLayer(new DoubleLeakyReLu()),
        new PerceptronLayer(10)
    });
}