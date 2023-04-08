using FotNET.NETWORK;
using FotNET.NETWORK.LAYERS;
using FotNET.NETWORK.LAYERS.ACTIVATION.ACTIVATION_FUNCTION.DOUBLE_LEAKY_RELU;
using FotNET.NETWORK.LAYERS.ACTIVATION.ACTIVATION_FUNCTION.LEAKY_RELU;
using FotNET.NETWORK.LAYERS.ACTIVATION.ACTIVATION_FUNCTION.SIGMOID;
using FotNET.NETWORK.LAYERS.ACTIVATION.ACTIVATION_FUNCTION.TANGENSOID;
using FotNET.NETWORK.LAYERS.FLATTEN;
using FotNET.NETWORK.LAYERS.PERCEPTRON;
using FotNET.NETWORK.LAYERS.RECURRENT;
using FotNET.NETWORK.LAYERS.RECURRENT.RECURRENCY_TYPE.ManyToMany;
using FotNET.NETWORK.LAYERS.RECURRENT.RECURRENCY_TYPE.ManyToOne;
using FotNET.NETWORK.LAYERS.RECURRENT.RECURRENCY_TYPE.OneToMany;
using FotNET.NETWORK.LAYERS.SOFT_MAX;
using FotNET.NETWORK.MATH.Initialization.HE;
using FotNET.NETWORK.MATH.Initialization.Xavier;
using FotNET.NETWORK.MATH.LOSS_FUNCTION.ONE_BY_ONE;
using FotNET.NETWORK.MATH.LOSS_FUNCTION.VALUE_BY_VALUE;
using FotNET.NETWORK.OBJECTS.MATH_OBJECTS;

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
        var testTensorData = new Tensor(new Matrix(new double[] { 70, 10, 30, 21, 14, 77 }));
        var model = new Network(new List<ILayer> {
            new FlattenLayer(),
            new RecurrentLayer(new Tangensoid(), new ManyToMany(), 10, new XavierInitialization()),
            new SoftMaxLayer()
        });
        
        Console.WriteLine();
        Console.WriteLine(model.ForwardFeed(testTensorData, AnswerType.Value));
        Console.WriteLine(model.GetWeights());
        
        for (var i = 0; i < 10; i++) model.BackPropagation(0, 30000, new ValueByValue(), .015d);
        
        Console.WriteLine();
        Console.WriteLine(model.ForwardFeed(testTensorData, AnswerType.Value));
        Console.WriteLine(model.GetWeights());
    }
    
    [Test]
    public void BackPropagation_MTO() {
        var testTensorData = new Tensor(new Matrix(new[] { .60, .35, .11 }));
        var model = new Network(new List<ILayer> {
            new FlattenLayer(),
            new RecurrentLayer(new Tangensoid(), new ManyToOne(), 10, new HeInitialization()),
            new PerceptronLayer(1)
        });

        Console.WriteLine();
        Console.WriteLine(model.ForwardFeed(testTensorData, AnswerType.Value));
        Console.WriteLine(model.GetWeights());

        var answer = 0d;
        while (Math.Abs(answer - .52d) > .01d) {
            model.BackPropagation(0, .52, new OneByOne(), .0015d);
            answer = model.ForwardFeed(testTensorData, AnswerType.Value);
        }
        
        Console.WriteLine();
        Console.WriteLine(model.ForwardFeed(testTensorData, AnswerType.Value));
    }
    
    [Test]
    public void BackPropagation_OTM() {
        var testTensorData = new Tensor(new Matrix(new double[] { .12d }));
        var model = new Network(new List<ILayer> {
            new FlattenLayer(),
            new RecurrentLayer(new DoubleLeakyReLu(), new OneToMany(), 10, new HeInitialization()),
            new SoftMaxLayer()
        });
        
        Console.WriteLine();
        Console.WriteLine(model.ForwardFeed(testTensorData, AnswerType.Value));
        Console.WriteLine(model.GetWeights());
        
        for (var i = 0; i < 10; i++) model.BackPropagation(0, 30000, new ValueByValue(), .015d);
        
        Console.WriteLine();
        Console.WriteLine(model.ForwardFeed(testTensorData, AnswerType.Value));
        Console.WriteLine(model.GetWeights());
    }
}