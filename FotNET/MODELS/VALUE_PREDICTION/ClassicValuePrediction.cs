using FotNET.NETWORK;
using FotNET.NETWORK.LAYERS;
using FotNET.NETWORK.LAYERS.ACTIVATION;
using FotNET.NETWORK.LAYERS.ACTIVATION.ACTIVATION_FUNCTION.DOUBLE_LEAKY_RELU;
using FotNET.NETWORK.LAYERS.PERCEPTRON;
using FotNET.NETWORK.LAYERS.PERCEPTRON.ADAM.DEFAULT_PERCEPTRON;
using FotNET.NETWORK.MATH.Initialization.HE;

namespace FotNET.MODELS.VALUE_PREDICTION;

public static class ClassicValuePrediction {
    public static Network SimpleValuePrediction = new Network(new List<ILayer> {
        new PerceptronLayer(10, 128, new HeInitialization(), new NoPerceptronOptimization()),
        new ActivationLayer(new DoubleLeakyReLu()),
        new PerceptronLayer(128, 128, new HeInitialization(), new NoPerceptronOptimization()),
        new ActivationLayer(new DoubleLeakyReLu()),
        new PerceptronLayer(128, 1, new HeInitialization(), new NoPerceptronOptimization()),
        new PerceptronLayer(1)
    });
    
    public static Network DeepValuePrediction = new Network(new List<ILayer> {
        new PerceptronLayer(10, 128, new HeInitialization(), new NoPerceptronOptimization()),
        new ActivationLayer(new DoubleLeakyReLu()),
        new PerceptronLayer(128, 128, new HeInitialization(), new NoPerceptronOptimization()),
        new ActivationLayer(new DoubleLeakyReLu()),
        new PerceptronLayer(128, 128, new HeInitialization(), new NoPerceptronOptimization()),
        new ActivationLayer(new DoubleLeakyReLu()),
        new PerceptronLayer(128, 128, new HeInitialization(), new NoPerceptronOptimization()),
        new ActivationLayer(new DoubleLeakyReLu()),
        new PerceptronLayer(128, 128, new HeInitialization(), new NoPerceptronOptimization()),
        new ActivationLayer(new DoubleLeakyReLu()),
        new PerceptronLayer(128, 1, new HeInitialization(), new NoPerceptronOptimization()),
        new PerceptronLayer(1)
    });
}