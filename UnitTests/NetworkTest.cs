using System.Drawing;
using System.Drawing.Imaging;
using FotNET.MODELS.IMAGE_CLASSIFICATION;
using FotNET.NETWORK;
using FotNET.NETWORK.LAYERS;
using FotNET.NETWORK.LAYERS.ACTIVATION;
using FotNET.NETWORK.LAYERS.ACTIVATION.ACTIVATION_FUNCTION.DOUBLE_LEAKY_RELU;
using FotNET.NETWORK.LAYERS.ACTIVATION.ACTIVATION_FUNCTION.RELU;
using FotNET.NETWORK.LAYERS.CONVOLUTION;
using FotNET.NETWORK.LAYERS.DATA;
using FotNET.NETWORK.LAYERS.DECONVOLUTION;
using FotNET.NETWORK.LAYERS.FLATTEN;
using FotNET.NETWORK.LAYERS.PERCEPTRON;
using FotNET.NETWORK.LAYERS.POOLING;
using FotNET.NETWORK.LAYERS.POOLING.SCRIPTS.MAX;
using FotNET.NETWORK.LAYERS.ROUGHEN;
using FotNET.NETWORK.LAYERS.SOFT_MAX;
using FotNET.NETWORK.MATH.Initialization.HE;
using FotNET.NETWORK.MATH.LOSS_FUNCTION.ONE_BY_ONE;
using FotNET.NETWORK.OBJECTS.MATH_OBJECTS;
using FotNET.SCRIPTS.GENERATIVE_ADVERSARIAL_NETWORK;
using FotNET.SCRIPTS.REGION_CONVOLUTION;

namespace UnitTests;

public class NetworkTest {
    [Test]
    public void RCnnTest() {
        var bitmap = (Bitmap)Image.FromFile(@"C://Users//j1sk1ss//Desktop//RCNN_TEST//test.jpg");
        RegionConvolution.ForwardFeed(bitmap, 50, 3, CnnClassification.DeepConvolutionNetwork, .2, 28, 28)
            .Save(@$"D:\загрузки\{Guid.NewGuid()}.png", ImageFormat.Png);
    }

    [Test]
    public void CnnTest() {
        var model = new Network(new List<ILayer> {
            new ConvolutionLayer(8, 9,9,3, new HeInitialization(), 1),
            new ActivationLayer(new DoubleLeakyReLu()),
            new PoolingLayer(new MaxPooling(), 4),
            new ConvolutionLayer(16, 9, 9, 8, new HeInitialization(), 1),
            new ActivationLayer(new DoubleLeakyReLu()),
            new PoolingLayer(new MaxPooling(), 2),
            new FlattenLayer(),
            new PerceptronLayer(144, 128, new HeInitialization()),
            new ActivationLayer(new DoubleLeakyReLu()),
            new PerceptronLayer(128, 2, new HeInitialization()),
            new ActivationLayer(new DoubleLeakyReLu()),
            new PerceptronLayer(2),
            new SoftMaxLayer()
        });

        for (var i = 0; i < 1; i++) {
            model.ForwardFeed(new Tensor(new Matrix(64, 64)), AnswerType.Class);
            model.BackPropagation(1,1,new OneByOne(), 1, true);
        }
        
        Console.WriteLine(model.ForwardFeed(new Tensor(new Matrix(64, 64)), AnswerType.Class));
    }

    [Test]
    public void GeneratorTest() {
        var model = new Network(new List<ILayer> {
            new RoughenLayer(3,3,32),
            new DeconvolutionLayer(16, 2,2,32, new HeInitialization(), 2),
            new ActivationLayer(new ReLu()),
            new DeconvolutionLayer(8, 6, 6, 16, new HeInitialization(), 2),
            new ActivationLayer(new ReLu()),
            new DeconvolutionLayer(4, 6, 6, 8, new HeInitialization(), 2),
            new ActivationLayer(new ReLu()),
            new DeconvolutionLayer(3, 6, 6, 4, new HeInitialization(), 2),
            new ActivationLayer(new ReLu()),
            new DataLayer(DataType.InputTensor)
        });
        
        Console.WriteLine(model.ForwardFeed(Vector.GenerateGaussianNoise(288).AsTensor(3,3,32)).GetInfo());
        var errorTensor = new Tensor(new List<Matrix> { new (76, 76), new (76, 76), new (76, 76) });
        model.BackPropagation(errorTensor, new OneByOne(), .1, true);
    }

    [Test]
    public void GanTest() {
        string Path = @"C://Users//j1sk1ss//Desktop//RCNN_TEST//";

        var network = new FotNET.SCRIPTS.GENERATIVE_ADVERSARIAL_NETWORK.GaNetwork(null, null);
        network.DiscriminatorFitting(GaNetwork.LoadReal(Path + "test_faces"), network.GenerateFake(50), .0015d);
        network.GeneratorFitting(1000, .15d);

        network.GenerateTensor().Save(@$"C://Users//j1sk1ss//Desktop//RCNN_TEST//answers//{Guid.NewGuid()}.png", ImageFormat.Png);
        //Parser.TensorToImage(network.GenerateFake(1)[0]).Save(@$"C://Users//j1sk1ss//Desktop//RCNN_TEST//answers//{Guid.NewGuid()}.png", ImageFormat.Png);
    }
}