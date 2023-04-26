using System.Drawing;
using System.Drawing.Imaging;
using FotNET.DATA.IMAGE;
using FotNET.MODELS.IMAGE_CLASSIFICATION;
using FotNET.NETWORK;
using FotNET.NETWORK.LAYERS;
using FotNET.NETWORK.LAYERS.ACTIVATION;
using FotNET.NETWORK.LAYERS.ACTIVATION.ACTIVATION_FUNCTION.DOUBLE_LEAKY_RELU;
using FotNET.NETWORK.LAYERS.ACTIVATION.ACTIVATION_FUNCTION.PRELU;
using FotNET.NETWORK.LAYERS.ACTIVATION.ACTIVATION_FUNCTION.RELU;
using FotNET.NETWORK.LAYERS.ACTIVATION.ACTIVATION_FUNCTION.SIGMOID;
using FotNET.NETWORK.LAYERS.CONVOLUTION;
using FotNET.NETWORK.LAYERS.CONVOLUTION.SCRIPTS.PADDING.VALID;
using FotNET.NETWORK.LAYERS.DATA;
using FotNET.NETWORK.LAYERS.FLATTEN;
using FotNET.NETWORK.LAYERS.NOISE;
using FotNET.NETWORK.LAYERS.NOISE.SCRIPTS.GAUSSIAN;
using FotNET.NETWORK.LAYERS.NORMALIZATION;
using FotNET.NETWORK.LAYERS.NORMALIZATION.NORMALIZATION_TYPE.ABS;
using FotNET.NETWORK.LAYERS.NORMALIZATION.NORMALIZATION_TYPE.MIN_MAX;
using FotNET.NETWORK.LAYERS.PERCEPTRON;
using FotNET.NETWORK.LAYERS.POOLING;
using FotNET.NETWORK.LAYERS.POOLING.SCRIPTS.MAX;
using FotNET.NETWORK.LAYERS.ROUGHEN;
using FotNET.NETWORK.LAYERS.SOFT_MAX;
using FotNET.NETWORK.LAYERS.TRANSPOSED_CONVOLUTION;
using FotNET.NETWORK.LAYERS.UP_SAMPLING;
using FotNET.NETWORK.LAYERS.UP_SAMPLING.UP_SAMPLING_TYPE.NEAREST_NEIGHBOR;
using FotNET.NETWORK.MATH.Initialization.HE;
using FotNET.NETWORK.MATH.LOSS_FUNCTION.MSE;
using FotNET.NETWORK.MATH.OBJECTS;
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
            new ConvolutionLayer(8, 10, 10, 3, new HeInitialization(), 2, new ValidPadding()),
            new ActivationLayer(new DoubleLeakyReLu()),
            new PoolingLayer(new MaxPooling(), 2),
            new ConvolutionLayer(16, 3, 3, 8, new HeInitialization(), 2, new ValidPadding()),
            new ActivationLayer(new DoubleLeakyReLu()),
            new PoolingLayer(new MaxPooling(), 2),
            new FlattenLayer(),
            new PerceptronLayer(144, 100, new HeInitialization()),
            new ActivationLayer(new DoubleLeakyReLu()),
            new PerceptronLayer(100, 2, new HeInitialization()),
            new ActivationLayer(new DoubleLeakyReLu()),
            new PerceptronLayer(2),
            new SoftMaxLayer()
        });

        for (var i = 0; i < 1; i++) {
            model.ForwardFeed(new Tensor(new Matrix(64, 64)), AnswerType.Class);
            model.BackPropagation(1,1,new Mse(), 1, true);
        }
        
        //Console.WriteLine(model.ForwardFeed(new Tensor(new Matrix(64, 64)), AnswerType.Class));
    }

    [Test]
    public void GeneratorTest() {
        var model = new Network(new List<ILayer> {
            new RoughenLayer(3,3,32),
            new TransposedConvolutionLayer(16, 2,2,32, new HeInitialization(), 2),
            new ActivationLayer(new ReLu()),
            new TransposedConvolutionLayer(8, 6, 6, 16, new HeInitialization(), 2),
            new ActivationLayer(new ReLu()),
            new TransposedConvolutionLayer(4, 6, 6, 8, new HeInitialization(), 2),
            new ActivationLayer(new ReLu()),
            new TransposedConvolutionLayer(3, 6, 6, 4, new HeInitialization(), 2),
            new ActivationLayer(new ReLu()),
            new DataLayer(DataType.InputTensor)
        });
        
        var errorTensor = new Tensor(new List<Matrix> { new (76, 76), new (76, 76), new (76, 76) });
        model.BackPropagation(errorTensor, new Mse(), .1, true);
    }

    [Test]
    public void GanTest() {
        const string path = @"C://Users//j1sk1ss//Desktop//RCNN_TEST//";

        var generator = new Network(new List<ILayer> {
            new NoiseLayer(144, new GaussianNoise()),
            new RoughenLayer(4,4,9),
            new TransposedConvolutionLayer(6,4,4,9, new HeInitialization(), 1),
            new ActivationLayer(new PReLu(.2d)),
            new TransposedConvolutionLayer(3,12,12,6, new HeInitialization(), 1),
            new ActivationLayer(new PReLu(.2d)),
            new TransposedConvolutionLayer(3,23,23,6, new HeInitialization(), 1),
            new ActivationLayer(new Sigmoid()),
            new NormalizationLayer(new Abs()),
            new NormalizationLayer(new MinMax(1)),
            new DataLayer(DataType.InputTensor)
        });
        
        var discriminator = new Network(new List<ILayer> {
            new ConvolutionLayer(3, 9, 9, 3, new HeInitialization(), 1, new ValidPadding()),
            new ActivationLayer(new DoubleLeakyReLu()),
            new PoolingLayer(new MaxPooling(), 4),
            new ConvolutionLayer(16, 5, 5, 3, new HeInitialization(), 1, new ValidPadding()),
            new ActivationLayer(new DoubleLeakyReLu()),
            new PoolingLayer(new MaxPooling(), 2),
            new FlattenLayer(),
            new PerceptronLayer(64, 32, new HeInitialization()),
            new ActivationLayer(new DoubleLeakyReLu()),
            new PerceptronLayer(32, 2, new HeInitialization()),
            new ActivationLayer(new DoubleLeakyReLu()),
            new PerceptronLayer(2),
            new SoftMaxLayer()
        });
        
        discriminator.LoadWeights(File.ReadAllText(@$"C://Users//j1sk1ss//Desktop//RCNN_TEST//answers//ForTest.txt"));
        //var a = discriminator.ForwardFeed(Parser.ImageToTensor(
          //@$"C://Users//j1sk1ss//Desktop//RCNN_TEST//faces//41d3e9385e34ebc0e3ba.jpeg"), AnswerType.Value);
        //var b = discriminator.ForwardFeed(generator.ForwardFeed(null), AnswerType.Value);
        //Console.WriteLine(a);
        //Console.WriteLine(b);
        
        
        /*
        var network = new GaNetwork(generator, discriminator);
        network.DiscriminatorFitting(10, GaNetwork.LoadReal(path + "faces", 40, 40), .005d);
        //
        File.WriteAllText(@$"C://Users//j1sk1ss//Desktop//RCNN_TEST//answers//{Guid.NewGuid()}.txt", network.GetDiscriminator().GetWeights());
        */
        
        var network = new GaNetwork(generator, discriminator);
        network.GeneratorFitting(10000, .5d);
        
        for (var i = 0; i < 1; i++)
            network.GenerateBitmap().Save(@$"C://Users//j1sk1ss//Desktop//RCNN_TEST//answers//{Guid.NewGuid()}.png", ImageFormat.Png);
        //Console.WriteLine(network.GenerateTensor().Channels[0].Print());
        //Console.WriteLine(Normalize(network.GenerateFake(1, 11, 11, 9)[0]).Channels[0].Print());
        
    }

    [Test]
    public void GeneratorBackPropTest() {
        const string path = @"C://Users//j1sk1ss//Desktop//RCNN_TEST//";
        
        var generator = new Network(new List<ILayer> {
            new NoiseLayer(128, new GaussianNoise()),
            new PerceptronLayer(128, 324, new HeInitialization()),
            new ActivationLayer(new PReLu(.2d)),
            new RoughenLayer(6,6,9),
            new UpSamplingLayer(new NearestNeighbor(), 2),
            new FlattenLayer(),
            new PerceptronLayer(1296, 2100, new HeInitialization()),
            new ActivationLayer(new PReLu(.2d)),
            new PerceptronLayer(2100, 4800, new HeInitialization()),
            new ActivationLayer(new Sigmoid()),
            new RoughenLayer(40,40,3),
            new NormalizationLayer(new Abs()),
            new NormalizationLayer(new MinMax(1)),
            new DataLayer(DataType.InputTensor)
        });
        
        var generator1 = new Network(new List<ILayer> {
            new NoiseLayer(144, new GaussianNoise()),
            new RoughenLayer(4,4,9),
            new TransposedConvolutionLayer(6,4,4,9, new HeInitialization(), 1),
            new ActivationLayer(new PReLu(.2d)),
            new TransposedConvolutionLayer(3,12,12,6, new HeInitialization(), 1),
            new ActivationLayer(new PReLu(.2d)),
            new TransposedConvolutionLayer(3,23,23,6, new HeInitialization(), 1),
            new ActivationLayer(new Sigmoid()),
            new NormalizationLayer(new Abs()),
            new NormalizationLayer(new MinMax(1)),
            new DataLayer(DataType.InputTensor)
        });
        
        var generator2 = new Network(new List<ILayer> {
            new NoiseLayer(144, new GaussianNoise()),
            new RoughenLayer(4,4,9),
            new UpSamplingLayer(new NearestNeighbor(), 2), 
            new ConvolutionLayer(6, 3, 3, 9, new HeInitialization(), 1, new ValidPadding()), // 16
            new UpSamplingLayer(new NearestNeighbor(), 2),
            new ConvolutionLayer(3, 3, 3, 6, new HeInitialization(), 1, new ValidPadding()), // 16
            new UpSamplingLayer(new NearestNeighbor(), 2), 
            new NormalizationLayer(new Abs()), 
            new NormalizationLayer(new MinMax(1)),
            new DataLayer(DataType.InputTensor)
        });
        
        for (var i = 0; i < 1000; i++) {
            var answer = generator1.ForwardFeed(null!);

            if (i % 10 == 0)
                Parser.TensorToImage(answer).Save(@$"C://Users//j1sk1ss//Desktop//RCNN_TEST//answers//{Guid.NewGuid()}.jpg", ImageFormat.Png);
            generator1.BackPropagation(Parser.ImageToTensor(new Bitmap((Bitmap)Bitmap.FromFile(@"C://Users//j1sk1ss//Desktop//RCNN_TEST//answers//Untitled.png"), new Size(40,40))),
                new Mse(), .01, true);
        }
        
    }
}