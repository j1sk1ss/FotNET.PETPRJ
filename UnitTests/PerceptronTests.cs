using FotNET.NETWORK;
using FotNET.NETWORK.LAYERS;
using FotNET.NETWORK.LAYERS.ACTIVATION;
using FotNET.NETWORK.LAYERS.ACTIVATION.ACTIVATION_FUNCTION.SIGMOID;
using FotNET.NETWORK.LAYERS.FLATTEN;
using FotNET.NETWORK.LAYERS.PERCEPTRON;
using FotNET.NETWORK.MATH.Initialization.Xavier;
using FotNET.NETWORK.OBJECTS.MATH_OBJECTS;

namespace UnitTests;

public class PerceptronTests {
    [Test]
    public void ForwardFeed() {
        var testTensorData = new Tensor(new Matrix(new double[] { 0, 0, 0, 1, 1, 1 }));
        var model = new Network(new List<ILayer> {
            new FlattenLayer(),
            new PerceptronLayer(6,5, new XavierInitialization()),
            new ActivationLayer(new Sigmoid()),
            new PerceptronLayer(5,4, new XavierInitialization()),
            new ActivationLayer(new Sigmoid()),
            new PerceptronLayer(4)
        });
        model.ForwardFeed(testTensorData, AnswerType.Class);

        var layers = model.GetLayers();
        for (var layer = 0; layer < layers.Count; layer++) {
            if (layers[layer].GetValues() == null) continue;
            Console.WriteLine($"Layer {layer + 1}:\nInput Tensor on layer:\n{layers[layer].GetValues().GetInfo()}\n");
        }
    }

    [Test]
    public void BackPropagation() {
        var testTensorData = new Tensor(new Matrix(new[] { 0d, 0d, 0d, 1d, 1d, 1d }));
        var model = new Network(new List<ILayer> {
            new FlattenLayer(),
            new PerceptronLayer(6,5, new XavierInitialization()),
            new ActivationLayer(new Sigmoid()),
            new PerceptronLayer(5,4, new XavierInitialization()),
            new ActivationLayer(new Sigmoid()),
            new PerceptronLayer(4)
        });
        
        var layers = model.GetLayers();
        for (var layer = 0; layer < layers.Count; layer++) 
            Console.WriteLine($"(BEFORE) Layer {layer + 1}:\nWeights:\n{layers[layer].GetData()}\n");

        model.ForwardFeed(testTensorData, AnswerType.Class);
        model.BackPropagation(0, 1, .015d);
        
        for (var layer = 0; layer < layers.Count; layer++) 
            Console.WriteLine($"(AFTER) Layer {layer + 1}:\nWeights:\n{layers[layer].GetData()}\n");
    }
}