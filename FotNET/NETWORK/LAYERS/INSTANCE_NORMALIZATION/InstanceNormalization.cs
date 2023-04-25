using FotNET.NETWORK.MATH.OBJECTS;

namespace FotNET.NETWORK.LAYERS.INSTANCE_NORMALIZATION;

public class InstanceNormalization : ILayer {
    public Tensor GetNextLayer(Tensor tensor) => Normalize(tensor);

    public Tensor BackPropagate(Tensor error, double learningRate, bool backPropagate) => Normalize(error);

    private static Tensor Normalize(Tensor tensor) {
        var newTensor = new Tensor(new List<Matrix>());
        foreach (var channel in tensor.Channels) {
            var sequence = channel.GetAsList().ToArray();
            var average = sequence.Sum() / sequence.Length;

            var sigma = Math.Sqrt(sequence.Sum(value => value - average) / sequence.Length);
            for (var i = 0; i < sequence.Length; i++) 
                sequence[i] *= sigma * average;
            
            newTensor.Channels.Add(new Vector(sequence).AsMatrix(channel.Rows, channel.Columns));
        }
        
        return newTensor;
    }

    public Tensor GetValues() => null!;

    public string GetData() => "";

    public string LoadData(string data) => data;
}