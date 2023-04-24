using FotNET.NETWORK.MATH.OBJECTS;

namespace FotNET.NETWORK.LAYERS.LAYER_NORMALIZATION;

/// <summary>
/// Layer normalization for input tensor
/// </summary>
public class LayerNormalization : ILayer {
    public Tensor GetNextLayer(Tensor tensor) {
        var sequence = tensor.Flatten();
        var average = sequence.Sum() / sequence.Count;

        var sigma = Math.Sqrt(sequence.Sum(value => value - average) / sequence.Count);
        for (var i = 0; i < sequence.Count; i++) 
            sequence[i] *= sigma * average;
        
        return new Vector(sequence.ToArray()).AsTensor(tensor.Channels[0].Rows, tensor.Channels[0].Columns,
            tensor.Channels.Count);
    }

    public Tensor BackPropagate(Tensor error, double learningRate, bool backPropagate) {
        var sequence = error.Flatten();
        var average = sequence.Sum() / sequence.Count;

        var sigma = Math.Sqrt(sequence.Sum(value => value - average) / sequence.Count);
        for (var i = 0; i < sequence.Count; i++) 
            sequence[i] *= sigma * average;
        
        return new Vector(sequence.ToArray()).AsTensor(error.Channels[0].Rows, error.Channels[0].Columns,
            error.Channels.Count);
    }

    public Tensor GetValues() => null!;

    public string GetData() => "";

    public string LoadData(string data) => data;
}