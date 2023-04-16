using FotNET.NETWORK.MATH.OBJECTS;

namespace FotNET.NETWORK.LAYERS.NOISE;

public class NoiseLayer : ILayer {
    /// <summary>
    /// Generates noise tensor
    /// </summary>
    /// <param name="size"> Size of noise tensor </param>
    public NoiseLayer(int size) {
        Size = size;
    }
    
    private int Size { get; }
    
    public Tensor GetNextLayer(Tensor tensor) =>
        Vector.GenerateGaussianNoise(Size).AsTensor(1, Size, 1);

    public Tensor BackPropagate(Tensor error, double learningRate, bool backPropagate) => error;

    public Tensor GetValues() => null!;

    public string GetData() => "";

    public string LoadData(string data) => data;
}