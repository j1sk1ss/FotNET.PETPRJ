using FotNET.NETWORK.LAYERS.UP_SAMPLING.UP_SAMPLING_TYPE;
using FotNET.NETWORK.MATH.OBJECTS;

namespace FotNET.NETWORK.LAYERS.UP_SAMPLING;

public class UpSamplingLayer : ILayer {
    public UpSamplingLayer(UpSampling upSampling, int scale) {
        UpSampling = upSampling;
        Scale      = scale;
    }
    
    private UpSampling UpSampling { get; }
    private int Scale { get; }

    public Tensor GetNextLayer(Tensor tensor) => UpSampling.UpSample(tensor, Scale);

    public Tensor BackPropagate(Tensor error, double learningRate, bool backPropagate) =>
        UpSampling.DownSample(error, Scale);

    public Tensor GetValues() => null!;

    public string GetData() => "";

    public string LoadData(string data) => data;
}