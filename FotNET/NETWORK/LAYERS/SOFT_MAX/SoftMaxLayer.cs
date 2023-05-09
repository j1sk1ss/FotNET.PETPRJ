using FotNET.NETWORK.LAYERS.SOFT_MAX.SCRIPTS;
using FotNET.NETWORK.MATH.OBJECTS;

namespace FotNET.NETWORK.LAYERS.SOFT_MAX;

public class SoftMaxLayer : ILayer {
    /// <summary> Layer that convert data tensor to soft-maxed-data tensor. </summary>
    public SoftMaxLayer() => InputTensor = new Tensor(new List<Matrix>());
    
    private Tensor InputTensor { get; set; }
    
    public Tensor GetNextLayer(Tensor tensor) {
        InputTensor = tensor.Copy();
        return new Vector(SoftMax.Softmax(tensor.Flatten()).ToArray())
            .AsTensor(InputTensor.Channels[0].Rows, InputTensor.Channels[0].Columns, InputTensor.Channels.Count);
    }

    public Tensor BackPropagate(Tensor error, double learningRate, bool backPropagate) => InputTensor;

    public Tensor GetValues() => InputTensor;
    
    public string GetData() => "";
    public string LoadData(string data) => data;
}