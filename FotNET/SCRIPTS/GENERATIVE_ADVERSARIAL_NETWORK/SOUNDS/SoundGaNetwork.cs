using FotNET.DATA.SOUND;
using FotNET.NETWORK;
using FotNET.NETWORK.MATH.LOSS_FUNCTION.MAE;
using FotNET.NETWORK.MATH.OBJECTS;
using FotNET.SCRIPTS.GENERATIVE_ADVERSARIAL_NETWORK.SOUNDS.SCRIPTS;

namespace FotNET.SCRIPTS.GENERATIVE_ADVERSARIAL_NETWORK.SOUNDS;

public class SoundGaNetwork {
    /// <summary>
    /// Generative Adversarial model 
    /// </summary>
    /// <param name="generator"> Generator model </param>
    /// <param name="discriminator"> Discriminator model </param>
    public SoundGaNetwork(Network generator, Network discriminator) {
        Generator     = generator;
        Discriminator = discriminator;
    }
    
    private Network Generator { get; }
    private Network Discriminator { get; }
    
    public Network GetDiscriminator() => Discriminator;
    
    private List<Tensor> GenerateFake(int count) {
        var fake = new List<Tensor>();
        for (var i = 0; i < count; i++) 
            fake.Add(Generator.ForwardFeed(null!));
        
        return fake;
    }
    
    /// <summary>
    /// Generate real data set
    /// </summary>
    /// <param name="directoryPath"> Path for directory with images </param>
    /// <returns> Data set of real sounds </returns>
    public static List<Tensor> LoadReal(string directoryPath) =>
         Directory.GetFiles(directoryPath).Select(file => 
             new Tensor(Parser.ConvertSoundToSpectrogram(file))).ToList();
    
    /// <summary>
    /// Discriminator fitting
    /// </summary>
    /// <param name="epochs"> Epochs count </param>
    /// <param name="realDataSet"> Real data set </param>
    /// <param name="learningRate"> Learning rate </param>
    public void DiscriminatorFitting(int epochs, List<Tensor> realDataSet, double learningRate) {
        for (var j = 0; j < epochs; j++) {
            var fakeDataSet = GenerateFake(realDataSet.Count);
            for (var i = 0; i < realDataSet.Count; i++)
                switch (new Random().Next() % 100 > 50) {
                    case true:
                        if (Math.Abs(Discriminator.ForwardFeed(realDataSet[i], AnswerType.Class) - 1) > .1)
                            Discriminator.BackPropagation(1, 1, new Mae(), learningRate, true);
                        break;
                    case false:
                        if (Discriminator.ForwardFeed(fakeDataSet[i], AnswerType.Class) > 0.01d)
                            Discriminator.BackPropagation(0, 1, new Mae(), learningRate, true);
                        break;
                }
        }
    }
    
    /// <summary>
    /// Generator fitting
    /// </summary>
    /// <param name="epochs"> Epochs count </param>
    /// <param name="learningRate"> Learning rate </param>
    public void GeneratorFitting(int epochs, double learningRate) {
        for (var i = 0; i < epochs; i++) {
            var generated = Generator.ForwardFeed(null!);
            var answer = Discriminator.ForwardFeed(generated, AnswerType.Class);

            if (Math.Abs(answer - 1) > .1)
                Generator.BackPropagation(Discriminator.BackPropagation(1, 1,
                    new Mae(), learningRate, false), learningRate, true);
            else
                Discriminator.BackPropagation(0, 1,
                    new Mae(), learningRate, false);
        }
    }
    
    /// <summary>
    /// Generator fitting with saving steps of fitting
    /// </summary>
    /// <param name="epochs"> Epochs count </param>
    /// <param name="learningRate"> Learning rate </param>
    /// <param name="saveStep"> Count of steps between that will be saved generator output </param>
    /// <param name="path"> Path to directory for save </param>
    public void GeneratorFitting(int epochs, double learningRate, int saveStep, string path) {
        for (var i = 0; i < epochs; i++) {
            var generated = Generator.ForwardFeed(null!);
            if (i % saveStep == 0)
                Parser.ConvertArrayToSound(new Vector(SoundConverter.SpectrogramToWaveform(generated.Channels[0], 1, 1)), path);
            var answer = Discriminator.ForwardFeed(generated, AnswerType.Class);
            
            if (Math.Abs(answer - 1) > .1)
                Generator.BackPropagation(Discriminator.BackPropagation(1, 1,
                    new Mae(), learningRate, false), learningRate, true);
            else
                Discriminator.BackPropagation(0, 1,
                    new Mae(), learningRate, false);
        }
    }

    /// <summary>
    /// Generate tensor by generator model
    /// </summary>
    /// <returns> Generated tensor </returns>
    public Tensor GenerateTensor() =>
         Generator.ForwardFeed(null!);
}