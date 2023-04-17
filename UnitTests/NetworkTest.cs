using System.Drawing;
using System.Drawing.Imaging;
using FotNET.DATA.IMAGE;
using FotNET.MODELS.IMAGE_CLASSIFICATION;
using FotNET.NETWORK;
using FotNET.NETWORK.LAYERS;
using FotNET.NETWORK.LAYERS.ACTIVATION;
using FotNET.NETWORK.LAYERS.ACTIVATION.ACTIVATION_FUNCTION.DOUBLE_LEAKY_RELU;
using FotNET.NETWORK.LAYERS.ACTIVATION.ACTIVATION_FUNCTION.LEAKY_RELU;
using FotNET.NETWORK.LAYERS.ACTIVATION.ACTIVATION_FUNCTION.RELU;
using FotNET.NETWORK.LAYERS.CONVOLUTION;
using FotNET.NETWORK.LAYERS.CONVOLUTION.SCRIPTS.PADDING.VALID;
using FotNET.NETWORK.LAYERS.DATA;
using FotNET.NETWORK.LAYERS.DECONVOLUTION;
using FotNET.NETWORK.LAYERS.FLATTEN;
using FotNET.NETWORK.LAYERS.NOISE;
using FotNET.NETWORK.LAYERS.NORMALIZATION;
using FotNET.NETWORK.LAYERS.NORMALIZATION.NORMALIZATION_TYPE.ABS;
using FotNET.NETWORK.LAYERS.NORMALIZATION.NORMALIZATION_TYPE.MIN_MAX;
using FotNET.NETWORK.LAYERS.PERCEPTRON;
using FotNET.NETWORK.LAYERS.POOLING;
using FotNET.NETWORK.LAYERS.POOLING.SCRIPTS.MAX;
using FotNET.NETWORK.LAYERS.ROUGHEN;
using FotNET.NETWORK.LAYERS.SOFT_MAX;
using FotNET.NETWORK.MATH.Initialization.HE;
using FotNET.NETWORK.MATH.Initialization.Xavier;
using FotNET.NETWORK.MATH.LOSS_FUNCTION.RATING.MAE;
using FotNET.NETWORK.MATH.LOSS_FUNCTION.RATING.MSE;
using FotNET.NETWORK.MATH.OBJECTS;

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
            new ConvolutionLayer(8, 9,9,3, new HeInitialization(), 1, new ValidPadding()),
            new ActivationLayer(new DoubleLeakyReLu()),
            new PoolingLayer(new MaxPooling(), 4),
            new ConvolutionLayer(16, 9, 9, 8, new HeInitialization(), 1, new ValidPadding()),
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
            model.BackPropagation(1,1,new Mse(), 1, true);
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
        model.BackPropagation(errorTensor, new Mse(), .1, true);
    }

    [Test]
    public void GanTest() {
        const string path = @"C://Users//j1sk1ss//Desktop//RCNN_TEST//";

        var generator = new Network(new List<ILayer> {
            new NoiseLayer(1089),
            new RoughenLayer(11,11,9),
            new DeconvolutionLayer(6,10,10,9, new XavierInitialization(), 2),
            new ActivationLayer(new LeakyReLu()),
            new DeconvolutionLayer(3,6,6,6, new XavierInitialization(), 2),
            new NormalizationLayer(new Abs()),
            new NormalizationLayer(new MinMax(255d))
        });
        
        var discriminator = new Network(new List<ILayer> {
            new ConvolutionLayer(3, 9,9,3, new HeInitialization(), 1, new ValidPadding()),
            new ActivationLayer(new DoubleLeakyReLu()),
            new PoolingLayer(new MaxPooling(), 4),
            new ConvolutionLayer(16, 9, 9, 3, new HeInitialization(), 1, new ValidPadding()),
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
        
        //discriminator.LoadWeights(File.ReadAllText(@$"C://Users//j1sk1ss//Desktop//RCNN_TEST//answers//ForTest.txt"));
        //var a = discriminator.ForwardFeed(Parser.ImageToTensor(
          //@$"C://Users//j1sk1ss//Desktop//RCNN_TEST//faces//41d3e9385e34ebc0e3ba.jpeg"), AnswerType.Value);
        //var b = discriminator.ForwardFeed(generator.ForwardFeed(null), AnswerType.Value);
        //Console.WriteLine(a);
        //Console.WriteLine(b);
        
        
        /*
        var network = new GaNetwork(generator, discriminator, 11, 11, 9);
        network.DiscriminatorFitting(10, GaNetwork.LoadReal(path + "faces", 64, 64), .005d);
        //
        File.WriteAllText(@$"C://Users//j1sk1ss//Desktop//RCNN_TEST//answers//{Guid.NewGuid()}.txt", network.GetDiscriminator().GetWeights());
        */
        
        //var network = new GaNetwork(generator, discriminator);
       // network.GeneratorFitting(100, .00005d);
        
        //for (var i = 0; i < 1; i++)
        //network.GenerateBitmap().Save(@$"C://Users//j1sk1ss//Desktop//RCNN_TEST//answers//{Guid.NewGuid()}.png", ImageFormat.Png);
        //Console.WriteLine(network.GenerateTensor().Channels[0].Print());
        //Console.WriteLine(Normalize(network.GenerateFake(1, 11, 11, 9)[0]).Channels[0].Print());
    }

    [Test]
    public void GeneratorBackPropTest() {
        const string path = @"C://Users//j1sk1ss//Desktop//RCNN_TEST//";

        var generator = new Network(new List<ILayer> {
            //new NoiseLayer(225),
            new RoughenLayer(5,5,9),
            new DeconvolutionLayer(6,10,10,9, new XavierInitialization(), 2),
            new ActivationLayer(new DoubleLeakyReLu()),
            new DeconvolutionLayer(3,6,6,6, new XavierInitialization(), 2),
            new ActivationLayer(new DoubleLeakyReLu()),
            new NormalizationLayer(new Abs()),
            new NormalizationLayer(new MinMax(255d)),
            new DataLayer(DataType.InputTensor)
        });

        var a = Vector.GenerateGaussianNoise(225).AsTensor(1, 225, 1);
        
        for (var i = 0; i < 100; i++) {
            var answer = generator.ForwardFeed(a);
            if (i % 10 == 0)
                Console.WriteLine(new Mae().GetLoss(answer, 
                    Parser.ImageToTensor(new Bitmap((Bitmap)Bitmap.FromFile(@"C://Users//j1sk1ss//Desktop//RCNN_TEST//faces//41d3e9385e34ebc0e3ba.jpeg")))));
            if (i % 10 == 0)
                Parser.TensorToImage(answer).Save(@$"C://Users//j1sk1ss//Desktop//RCNN_TEST//answers//{Guid.NewGuid()}.png", ImageFormat.Png);
            generator.BackPropagation(
                Parser.ImageToTensor(new Bitmap((Bitmap)Bitmap.FromFile(@"C://Users//j1sk1ss//Desktop//RCNN_TEST//faces//41d3e9385e34ebc0e3ba.jpeg"), new Size(40,40))), 
                new Mse(), -.0005, true);
        }
    }
}