using System.Drawing;
using FotNET.DATA.IMAGE;
using FotNET.NETWORK;
using FotNET.NETWORK.LAYERS;
using FotNET.NETWORK.LAYERS.ACTIVATION;
using FotNET.NETWORK.LAYERS.ACTIVATION.ACTIVATION_FUNCTION.DOUBLE_LEAKY_RELU;
using FotNET.NETWORK.LAYERS.CONVOLUTION;
using FotNET.NETWORK.LAYERS.DATA;
using FotNET.NETWORK.LAYERS.DECONVOLUTION;
using FotNET.NETWORK.LAYERS.FLATTEN;
using FotNET.NETWORK.LAYERS.PERCEPTRON;
using FotNET.NETWORK.LAYERS.POOLING;
using FotNET.NETWORK.LAYERS.POOLING.SCRIPTS.MAX;
using FotNET.NETWORK.MATH.Initialization.HE;
using FotNET.NETWORK.MATH.Initialization.Xavier;
using FotNET.NETWORK.MATH.LOSS_FUNCTION.ONE_BY_ONE;
using FotNET.NETWORK.OBJECTS.MATH_OBJECTS;
using FotNET.NETWORK.ROUGHEN;

namespace FotNET.SCRIPTS.GENERATIVE_ADVERSARIAL_NETWORK;

public class Network {
    public NETWORK.Network Generator = new NETWORK.Network(new List<ILayer> {
        new RoughenLayer(4,4,9),
        new DeconvolutionLayer(6, 5, 5, 9, new XavierInitialization(), 2),
        new ActivationLayer(new DoubleLeakyReLu()),
        new DeconvolutionLayer(3, 8, 8, 6, new XavierInitialization(), 2),
        new ActivationLayer(new DoubleLeakyReLu()),
        new DataLayer(DataType.InputTensor)
    });

    public NETWORK.Network Discriminator = new NETWORK.Network(new List<ILayer> {
        new DataLayer(DataType.ErrorTensor),
        new ConvolutionLayer(3, 3, 3, 3, new HeInitialization(), 1),
        new ActivationLayer(new DoubleLeakyReLu()),
        new PoolingLayer(new MaxPooling(), 2),
        new ConvolutionLayer(6, 6, 6, 3,new HeInitialization(), 1),
        new ActivationLayer(new DoubleLeakyReLu()),
        new PoolingLayer(new MaxPooling(), 2),
        new FlattenLayer(),
        new PerceptronLayer(96, 48, new HeInitialization()),
        new ActivationLayer(new DoubleLeakyReLu()),
        new PerceptronLayer(48, 24, new HeInitialization()),
        new ActivationLayer(new DoubleLeakyReLu()),
        new PerceptronLayer(24, 12, new HeInitialization()),
        new ActivationLayer(new DoubleLeakyReLu()),
        new PerceptronLayer(12, 2, new HeInitialization()),
        new ActivationLayer(new DoubleLeakyReLu()),
        new PerceptronLayer(2)
    });

    public List<Tensor> GenerateFake(int count) {
        var fake = new List<Tensor>();
        for (var i = 0; i < count; i++) 
            fake.Add(Generator.ForwardFeed(Vector.GenerateGaussianNoise(144).AsTensor(4,4,9)));
        
        return fake;
    }

    public List<Tensor> LoadReal(string directoryPath) {
        var files = Directory.GetFiles(directoryPath);
        return files.Select(file => Parser.ImageToTensor
            (new Bitmap((Bitmap)Bitmap.FromFile(file), new Size(28, 28)))).ToList();
    }
    
    public void DiscriminatorFitting(List<Tensor> realDataSet, List<Tensor> fakeDataSet, double learningRate) {
        for (var i = 0; i < Math.Min(realDataSet.Count, fakeDataSet.Count); i++) {
            switch (new Random().Next() % 100 > 50) {
                case true: // load real 1
                    if (Math.Abs(Discriminator.ForwardFeed(realDataSet[i], AnswerType.Class) - 1) > .1) 
                        Discriminator.BackPropagation(1, 1, new OneByOne(), learningRate, true);
                    break;
                case false: // load fake 0
                    if (Discriminator.ForwardFeed(realDataSet[i], AnswerType.Class) != 0) 
                        Discriminator.BackPropagation(0, 1, new OneByOne(), learningRate, true);
                    break;
            }
        }
    }

    public void GeneratorFitting(int epochs, double learningRate) {
        for (var i = 0; i < epochs; i++) {
            var generatedData = Generator.ForwardFeed(Vector.GenerateGaussianNoise(256).AsTensor(4, 4, 16));
            Discriminator.ForwardFeed(generatedData);
            Discriminator.BackPropagation(1,1, new OneByOne(), learningRate, false);
            var error = Discriminator.GetLayers()[0].GetValues();
            Generator.BackPropagation(error, learningRate, true);
        }
    }

    public Bitmap GenerateTensor() => Parser.TensorToImage(Generator.ForwardFeed(Vector.GenerateGaussianNoise(256).AsTensor(4, 4, 16)));
}