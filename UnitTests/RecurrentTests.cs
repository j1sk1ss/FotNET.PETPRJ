using FotNET.NETWORK;
using FotNET.NETWORK.LAYERS;
using FotNET.NETWORK.LAYERS.ACTIVATION.ACTIVATION_FUNCTION.DOUBLE_LEAKY_RELU;
using FotNET.NETWORK.LAYERS.FLATTEN;
using FotNET.NETWORK.LAYERS.RECURRENT;
using FotNET.NETWORK.LAYERS.RECURRENT.RECURRENCY_TYPE.ManyToMany;
using FotNET.NETWORK.LAYERS.RECURRENT.RECURRENCY_TYPE.ManyToOne;
using FotNET.NETWORK.LAYERS.RECURRENT.RECURRENCY_TYPE.OneToMany;
using FotNET.NETWORK.LAYERS.SOFT_MAX;
using FotNET.NETWORK.MATH.Initialization.HE;
using FotNET.NETWORK.OBJECTS.MATH_OBJECTS;
using NUnit.Framework;

namespace UnitTests;

public class RecurrentTests {
    [Test]
    public void ForwardFeed_MTM() {
        var testTensorData = new Tensor(new Matrix(new double[] { 0, 0, 0, 1, 1, 1 }));
        var model = new Network(new List<ILayer> {
            new FlattenLayer(),
            new RecurrentLayer(new DoubleLeakyReLu(), new ManyToMany(), 10, new HeInitialization()),
            new SoftMaxLayer()
        });
        model.ForwardFeed(testTensorData, AnswerType.Class);

        var layers = model.GetLayers();
        Console.WriteLine($"Layer {layers.Count}:\nInput Tensor on layer:\n{layers[^1].GetValues().Channels[0].Print()}\n");
    }

    [Test]
    public void ForwardFeed_MTO() {
        var testTensorData = new Tensor(new Matrix(new double[] { .9, .1, .1, .1, 1, 1 }));
        var model = new Network(new List<ILayer> {
            new FlattenLayer(),
            new RecurrentLayer(new DoubleLeakyReLu(), new ManyToOne(), 10, new HeInitialization()),
            new SoftMaxLayer()
        });
        model.ForwardFeed(testTensorData, AnswerType.Class);

        var layers = model.GetLayers();
        Console.WriteLine($"Layer {layers.Count}:\nInput Tensor on layer:\n{layers[^1].GetValues().Channels[0].Print()}\n");
    }
    
    [Test]
    public void ForwardFeed_OTM() {
        var testTensorData = new Tensor(new Matrix(new double[] { .012d }));
        var model = new Network(new List<ILayer> {
            new FlattenLayer(),
            new RecurrentLayer(new DoubleLeakyReLu(), new OneToMany(), 10, new HeInitialization()),
            new SoftMaxLayer()
        });
        model.ForwardFeed(testTensorData, AnswerType.Class);

        var layers = model.GetLayers();
        Console.WriteLine($"Layer {layers.Count}:\nInput Tensor on layer:\n{layers[^1].GetValues().Channels[0].Print()}\n");
    }
    
    [Test]
    public void BackPropagation_MTM() {
        var testTensorData = new Tensor(new Matrix(new double[] { 0, 0, 0, 1, 1, 1 }));
        var model = new Network(new List<ILayer> {
            new FlattenLayer(),
            new RecurrentLayer(new DoubleLeakyReLu(), new ManyToMany(), 10, new HeInitialization()),
            new SoftMaxLayer()
        });
        model.ForwardFeed(testTensorData, AnswerType.Class);

        var layers = model.GetLayers();
        for (var layer = 0; layer < layers.Count; layer++) 
            Console.WriteLine($"(BEFORE) Layer {layer + 1}:\nInput Tensor on layer:\n{layers[layer].GetData()}\n");

        model.BackPropagation(0, 1, .15d);
        
        for (var layer = 0; layer < layers.Count; layer++) 
            Console.WriteLine($"(BEFORE) Layer {layer + 1}:\nInput Tensor on layer:\n{layers[layer].GetData()}\n");
    }
    
    [Test]
    public void BackPropagation_MTO() {
        var testTensorData = new Tensor(new Matrix(new double[] { 0, 0, 0, 1, 1, 1 }));
        var model = new Network(new List<ILayer> {
            new FlattenLayer(),
            new RecurrentLayer(new DoubleLeakyReLu(), new ManyToOne(), 10, new HeInitialization()),
            new SoftMaxLayer()
        });
        model.ForwardFeed(testTensorData, AnswerType.Value);

        var layers = model.GetLayers();
        for (var layer = 0; layer < layers.Count; layer++) 
            Console.WriteLine($"(BEFORE) Layer {layer + 1}:\nInput Tensor on layer:\n{layers[layer].GetData()}\n");

        model.BackPropagation(0, 1, .15d);
        
        for (var layer = 0; layer < layers.Count; layer++) 
            Console.WriteLine($"(BEFORE) Layer {layer + 1}:\nInput Tensor on layer:\n{layers[layer].GetData()}\n");
    }
    
    [Test]
    public void BackPropagation_OTM() {
        var testTensorData = new Tensor(new Matrix(new double[] { .12d }));
        var model = new Network(new List<ILayer> {
            new FlattenLayer(),
            new RecurrentLayer(new DoubleLeakyReLu(), new OneToMany(), 10, new HeInitialization()),
            new SoftMaxLayer()
        });
        model.ForwardFeed(testTensorData, AnswerType.Class);

        var layers = model.GetLayers();
        for (var layer = 0; layer < layers.Count; layer++) 
            Console.WriteLine($"(BEFORE) Layer {layer + 1}:\nInput Tensor on layer:\n{layers[layer].GetData()}\n");

        model.BackPropagation(0, 1, .15d);
        
        for (var layer = 0; layer < layers.Count; layer++) 
            Console.WriteLine($"(BEFORE) Layer {layer + 1}:\nInput Tensor on layer:\n{layers[layer].GetData()}\n");
    }
}