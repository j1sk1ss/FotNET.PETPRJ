using FotNET.NETWORK.OBJECTS;

namespace FotNET.NETWORK.LAYERS.DROPOUT;

public class DropoutLayer : ILayer {

    public DropoutLayer(double percent) {
        Percent = percent;
    }
    
    private double Percent { get; }
    
    public Tensor GetNextLayer(Tensor tensor) {
        foreach (var channel in tensor.Channels) 
            for (var i = 0; i < channel.Body.GetLength(0); i++)
                for (var j = 0; j < channel.Body.GetLength(1); j++)
                    if (new Random().Next() % 100 >= Percent)
                        channel.Body[i, j] = 0;
        
        return tensor;
    }

    public Tensor BackPropagate(Tensor error) => error;

    public Tensor GetValues() => null!;
    public string GetData() => "";
    public string LoadData(string data) => data;
}