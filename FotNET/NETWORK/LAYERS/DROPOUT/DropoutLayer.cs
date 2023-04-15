using FotNET.NETWORK.MATH.OBJECTS;

namespace FotNET.NETWORK.LAYERS.DROPOUT;

public class DropoutLayer : ILayer {
    /// <summary> Layer that perform dropout of neurons. </summary>
    /// <param name="percent"> Percent of elements in data tensor of neurons that will be "killed". </param>
    public DropoutLayer(double percent) => Percent = percent;
    
    private double Percent { get; }
    
    public Tensor GetNextLayer(Tensor tensor) {
        var neuronsCount = (int)(tensor.Flatten().Count * (Percent / 100d));
        
        foreach (var channel in tensor.Channels) 
            for (var i = 0; i < channel.Rows; i++)
                for (var j = 0; j < channel.Columns; j++)
                    if (new Random().Next() % 100 <= Percent && --neuronsCount > 0)
                        channel.Body[i, j] = 0;
        
        return tensor;
    }

    public Tensor BackPropagate(Tensor error, double learningRate, bool backPropagate) => error;

    public Tensor GetValues() => null!;
    public string GetData() => "";
    public string LoadData(string data) => data;
}