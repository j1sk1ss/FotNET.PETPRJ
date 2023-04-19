using FotNET.NETWORK;
using FotNET.NETWORK.LAYERS;
using FotNET.NETWORK.LAYERS.ACTIVATION.ACTIVATION_FUNCTION.DOUBLE_LEAKY_RELU;
using FotNET.NETWORK.LAYERS.ACTIVATION.ACTIVATION_FUNCTION.HYPERBOLIC_TANGENT;
using FotNET.NETWORK.LAYERS.DATA;
using FotNET.NETWORK.LAYERS.FLATTEN;
using FotNET.NETWORK.LAYERS.PERCEPTRON;
using FotNET.NETWORK.LAYERS.RECURRENT;
using FotNET.NETWORK.LAYERS.RECURRENT.RECURRENCY_TYPE.MANY_TO_ONE;
using FotNET.NETWORK.LAYERS.RECURRENT.RECURRENCY_TYPE.ONE_TO_MANY;
using FotNET.NETWORK.LAYERS.RECURRENT.RECURRENCY_TYPE.VALID_MANY_TO_MANY;
using FotNET.NETWORK.LAYERS.SOFT_MAX;
using FotNET.NETWORK.MATH.Initialization.HE;
using FotNET.NETWORK.MATH.Initialization.Xavier;
using FotNET.NETWORK.MATH.LOSS_FUNCTION.MSE;
using FotNET.NETWORK.MATH.OBJECTS;

namespace UnitTests;

public class RecurrentTests {
    [Test]
    public void ForwardFeed_MTM() {
        var testTensorData = new Tensor(new Matrix(new double[] { 0, 0, 0, 1, 1, 1 }));
        var model = new Network(new List<ILayer> {
            new FlattenLayer(),
            new RecurrentLayer(new DoubleLeakyReLu(), new ValidManyToMany(), 10, new HeInitialization()),
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
        var testTensorData = new Tensor(new Matrix(new[] { .7d, .1d, .3d, .21d, .14d, .77d }));
        var model = new Network(new List<ILayer> {
            new FlattenLayer(),
            new RecurrentLayer(new HyperbolicTangent(), new ValidManyToMany(), 10, new XavierInitialization()),
            new DataLayer(DataType.InputTensor)
        });
        
        var expected = new Tensor(new Matrix(new[] { .12d, .44d, .76d, .11d, .4d, .13d })); // Error cuz output is 1x1x10 instean 1x10x1 like we pass
        
        Console.WriteLine();
        //Console.WriteLine(new Vector(model.ForwardFeed(testTensorData).Flatten().ToArray()).Print());
        for (var i = 0; i < 200; i++) {
            model.BackPropagation(expected, new Mse(), -.0015d, true);
            model.ForwardFeed(testTensorData);
        }
        
        Console.WriteLine();
        //Console.WriteLine(new Vector(model.ForwardFeed(testTensorData).Flatten().ToArray()).Print());
    }
    
    [Test]
    public void BackPropagation_MTO() {
        var testTensorData = new Tensor(new Matrix(new[] { .60, .35, .11 }));
        var model = new Network(new List<ILayer> {
            new FlattenLayer(),
            new RecurrentLayer(new HyperbolicTangent(), new ManyToOne(), 10, new XavierInitialization()),
            new PerceptronLayer(1)
        });

        Console.WriteLine();
        Console.WriteLine(model.ForwardFeed(testTensorData, AnswerType.Value));
        Console.WriteLine(model.GetWeights());

        for (var i = 0; i < 200; i++) {
            model.BackPropagation(0, .52, new Mse(), -.015d, true);
            model.ForwardFeed(testTensorData, AnswerType.Value);
            //Console.WriteLine(answer);
        }
        
        Console.WriteLine();
        Console.WriteLine(model.ForwardFeed(testTensorData, AnswerType.Value));
    }
    
    [Test]
    public void BackPropagation_OTM() {
        var testTensorData = new Tensor(new Matrix(new[] { .12d }));
        var model = new Network(new List<ILayer> {
            new RecurrentLayer(new HyperbolicTangent(), new ValidManyToMany(), 5, new HeInitialization()),
            new FlattenLayer(),
            new DataLayer(DataType.InputTensor)
        });

        var expected = new Tensor(new Matrix(new[] { .12d, .44d, .76d, .11d, .4d }));
        
        Console.WriteLine();
        Console.WriteLine(new Vector(model.ForwardFeed(testTensorData).Flatten().ToArray()).Print());
        
        for (var i = 0; i < 50; i++) {
            model.ForwardFeed(testTensorData);
            model.BackPropagation(expected, new Mse(), -.5d, true);
        }
        
        Console.WriteLine();
        Console.WriteLine(new Vector(model.ForwardFeed(testTensorData).Flatten().ToArray()).Print());
    }
}