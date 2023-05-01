using FotNET.NETWORK;
using FotNET.NETWORK.LAYERS;
using FotNET.NETWORK.LAYERS.ACTIVATION;
using FotNET.NETWORK.LAYERS.ACTIVATION.ACTIVATION_FUNCTION.DOUBLE_LEAKY_RELU;
using FotNET.NETWORK.LAYERS.CONVOLUTION;
using FotNET.NETWORK.LAYERS.CONVOLUTION.ADAM.DEFAULT_CONVOLUTION;
using FotNET.NETWORK.LAYERS.CONVOLUTION.SCRIPTS.PADDING.VALID;
using FotNET.NETWORK.LAYERS.FLATTEN;
using FotNET.NETWORK.LAYERS.PERCEPTRON;
using FotNET.NETWORK.LAYERS.PERCEPTRON.ADAM.DEFAULT_PERCEPTRON;
using FotNET.NETWORK.LAYERS.POOLING;
using FotNET.NETWORK.LAYERS.POOLING.SCRIPTS.MAX;
using FotNET.NETWORK.MATH.Initialization.HE;

namespace FotNET.MODELS.IMAGE_CLASSIFICATION;

public static class CnnClassification {
    /// <summary>
    /// CNN model for MNIST data set. Takes 28x28 tensor.
    /// </summary>
    public static Network SimpleConvolutionNetwork = new Network(new List<ILayer> {
        new ConvolutionLayer(6, 5,5,3, new HeInitialization(), 1, new ValidPadding(), new NoConvolutionOptimization()),
        new ActivationLayer(new DoubleLeakyReLu()),
        new PoolingLayer(new MaxPooling(), 2),
        new ConvolutionLayer(16, 5, 5, 6, new HeInitialization(), 1, new ValidPadding(), new NoConvolutionOptimization()),
        new ActivationLayer(new DoubleLeakyReLu()),
        new PoolingLayer(new MaxPooling(), 2),
        new FlattenLayer(),
        new PerceptronLayer(256, 128, new HeInitialization(), new NoPerceptronOptimization()),
        new ActivationLayer(new DoubleLeakyReLu()),
        new PerceptronLayer(128, 10, new HeInitialization(), new NoPerceptronOptimization()),
        new ActivationLayer(new DoubleLeakyReLu()),
        new PerceptronLayer(10)
    });
    
    /// <summary>
    /// Big CNN model for MNIST data set. Takes 28x28 tensor.
    /// </summary>
    public static Network DeepConvolutionNetwork = new Network(new List<ILayer> {
        new ConvolutionLayer(16, 5,5,3, new HeInitialization(), 1, new ValidPadding(), new NoConvolutionOptimization()),
        new ActivationLayer(new DoubleLeakyReLu()),
        new ConvolutionLayer(32, 5, 5, 16, new HeInitialization(), 1, new ValidPadding(), new NoConvolutionOptimization()),
        new ActivationLayer(new DoubleLeakyReLu()),
        new FlattenLayer(),
        new PerceptronLayer(512, 256, new HeInitialization(), new NoPerceptronOptimization()),
        new ActivationLayer(new DoubleLeakyReLu()),
        new PerceptronLayer(256, 128, new HeInitialization(), new NoPerceptronOptimization()),
        new ActivationLayer(new DoubleLeakyReLu()),
        new PerceptronLayer(128, 10, new HeInitialization(), new NoPerceptronOptimization()),
        new ActivationLayer(new DoubleLeakyReLu()),
        new PerceptronLayer(10)
    });
}