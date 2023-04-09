using FotNET.NETWORK.OBJECTS.MATH_OBJECTS;

namespace FotNET.NETWORK.LAYERS.DROPOUT;

public class DropoutLayer : ILayer {
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

    public Tensor BackPropagate(Tensor error, double learningRate) => error;

    public Tensor GetValues() => null!;
    public string GetData() => "";
    public string LoadData(string data) => data;
}