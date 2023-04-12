using System.Drawing;

using FotNET.DATA.IMAGE;
using FotNET.NETWORK;
using FotNET.NETWORK.MATH.LOSS_FUNCTION.ONE_BY_ONE;
using FotNET.NETWORK.OBJECTS.MATH_OBJECTS;

namespace FotNET.SCRIPTS.GENERATIVE_ADVERSARIAL_NETWORK;

public class GaNetwork {
    public GaNetwork(Network generator, Network discriminator, int inputTensorXSize, int inputTensorYSize, int inputTensorDepth) {
        Generator = generator;
        Discriminator = discriminator;

        InputTensorXSize = inputTensorXSize;
        InputTensorYSize = inputTensorYSize;
        InputTensorDepth = inputTensorDepth;
    }

    private Network Generator { get; }
    private Network Discriminator { get; }
    
    private int InputTensorXSize { get; }
    private int InputTensorYSize { get; }
    private int InputTensorDepth { get; }

    public List<Tensor> GenerateFake(int count) {
        var fake = new List<Tensor>();
        for (var i = 0; i < count; i++) 
            fake.Add(Generator.ForwardFeed(Vector.GenerateGaussianNoise(InputTensorXSize * InputTensorYSize * InputTensorDepth)
                .AsTensor(InputTensorXSize,InputTensorYSize,InputTensorDepth)));
        
        return fake;
    }

    public static List<Tensor> LoadReal(string directoryPath, int resizeX, int resizeY) {
        var files = Directory.GetFiles(directoryPath);
        return files.Select(file => Parser.ImageToTensor
            (new Bitmap((Bitmap)Image.FromFile(file), new Size(resizeX, resizeY)))).ToList();
    }
    
    public void DiscriminatorFitting(int epochs, List<Tensor> realDataSet, List<Tensor> fakeDataSet, double learningRate) {
        for (var j = 0; j < epochs; j++)
            for (var i = 0; i < Math.Min(realDataSet.Count, fakeDataSet.Count); i++) 
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

    public void GeneratorFitting(int epochs, double learningRate) {
        for (var i = 0; i < epochs; i++) {
            var answer = Discriminator.ForwardFeed(Generator.ForwardFeed(Vector
                .GenerateGaussianNoise(InputTensorXSize * InputTensorYSize * InputTensorDepth)
                .AsTensor(InputTensorXSize, InputTensorYSize, InputTensorDepth)), AnswerType.Class);
            if (Math.Abs(answer - 1) > .1) 
                Generator.BackPropagation(Discriminator.BackPropagation(1,1, 
                    new OneByOne(), learningRate, false), learningRate, true);
        }
    }

    public Bitmap GenerateBitmap() =>
        Parser.TensorToImage(Generator.ForwardFeed(Vector
            .GenerateGaussianNoise(InputTensorXSize * InputTensorYSize * InputTensorDepth)
            .AsTensor(InputTensorXSize, InputTensorYSize, InputTensorDepth)));

    public Tensor GenerateTensor() {
        return Generator.ForwardFeed(Vector
            .GenerateGaussianNoise(InputTensorXSize * InputTensorYSize * InputTensorDepth)
            .AsTensor(InputTensorXSize, InputTensorYSize, InputTensorDepth));
    }
}