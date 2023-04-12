using FotNET.NETWORK.LAYERS.NORMALIZATION.NORMALIZATION_TYPE;
using FotNET.NETWORK.OBJECTS.MATH_OBJECTS;

namespace FotNET.NETWORK.LAYERS.NORMALIZATION;

public class NormalizationLayer : ILayer {
    public NormalizationLayer(INormalization normalization) {
        Normalization = normalization;
    }
    
    private INormalization Normalization { get; }

    public Tensor GetNextLayer(Tensor tensor) => Normalization.Normalize(tensor);

    public Tensor BackPropagate(Tensor error, double learningRate, bool backPropagate) => error;

    public Tensor GetValues() => null!;

    public string GetData() => "";

    public string LoadData(string data) => data;
}