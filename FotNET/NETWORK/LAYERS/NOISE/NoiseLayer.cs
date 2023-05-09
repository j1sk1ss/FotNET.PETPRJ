using FotNET.NETWORK.LAYERS.NOISE.SCRIPTS;
using FotNET.NETWORK.MATH.OBJECTS;

namespace FotNET.NETWORK.LAYERS.NOISE;

public class NoiseLayer : ILayer {
    /// <summary>
    /// Generates noise tensor. If get not-null data tensor, skip noise generation and return dota tensor
    /// </summary>
    /// <param name="size"> Size of noise tensor </param>
    /// <param name="noise"> Noise type </param>
    public NoiseLayer(int size, INoise noise) {
        Noise = noise;
        Size  = size;
    }

    private int Size { get; }
    private INoise Noise { get; }
    
    public Tensor GetNextLayer(Tensor? tensor) => tensor ?? Noise.GenerateNoise(Size).AsTensor(1, Size, 1);

    public Tensor BackPropagate(Tensor error, double learningRate, bool backPropagate) => error;

    public Tensor GetValues() => null!;

    public string GetData() => "";

    public string LoadData(string data) => data;
}