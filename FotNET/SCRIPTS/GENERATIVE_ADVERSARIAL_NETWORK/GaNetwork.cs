using System.Drawing;

using FotNET.DATA.IMAGE;
using FotNET.NETWORK;
using FotNET.NETWORK.MATH.LOSS_FUNCTION.ONE_BY_ONE;
using FotNET.NETWORK.OBJECTS.MATH_OBJECTS;

namespace FotNET.SCRIPTS.GENERATIVE_ADVERSARIAL_NETWORK;

public class GaNetwork {
    public GaNetwork(Network generator, Network discriminator) {
        Generator = generator;
        Discriminator = discriminator;
    }

    private Network Generator { get; }
    private Network Discriminator { get; }

    public List<Tensor> GenerateFake(int count) {
        var fake = new List<Tensor>();
        for (var i = 0; i < count; i++) 
            fake.Add(Generator.ForwardFeed(Vector.GenerateGaussianNoise(144).AsTensor(4,4,9)));
        
        return fake;
    }

    public static List<Tensor> LoadReal(string directoryPath) {
        var files = Directory.GetFiles(directoryPath);
        return files.Select(file => Parser.ImageToTensor
            (new Bitmap((Bitmap)Image.FromFile(file), new Size(28, 28)))).ToList();
    }
    
    public void DiscriminatorFitting(List<Tensor> realDataSet, List<Tensor> fakeDataSet, double learningRate) {
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
            Discriminator.ForwardFeed(Generator.ForwardFeed(Vector.GenerateGaussianNoise(256).AsTensor(4, 4, 16)));
            Discriminator.BackPropagation(1,1, new OneByOne(), learningRate, false);
            var error = Discriminator.GetLayers()[0].GetValues();
            
            Generator.BackPropagation(error, learningRate, true);
        }
    }

    public Bitmap GenerateTensor() => Parser.TensorToImage(Generator.ForwardFeed(Vector.GenerateGaussianNoise(256).AsTensor(4, 4, 16)));
}