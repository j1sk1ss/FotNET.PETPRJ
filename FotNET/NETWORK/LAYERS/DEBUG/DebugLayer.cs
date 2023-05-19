using FotNET.NETWORK.MATH.OBJECTS;

namespace FotNET.NETWORK.LAYERS.DEBUG;

public class DebugLayer : ILayer {

    /// <summary>
    /// Debug layer for finding issues
    /// </summary>
    /// <param name="exception"> Type of exception what will be throw </param>
    /// <param name="expectedShape"> Tensor expected shape </param>
    public DebugLayer(Exception exception, (int Weight, int Height, int Depth) expectedShape) {
        Exception = exception;
        Shape     = expectedShape;
    }

    private Exception Exception { get; }
    private (int Weight, int Height, int Depth) Shape { get; }
    
    public Tensor GetNextLayer(Tensor tensor) {
        Console.WriteLine($"Data tensor: {tensor.GetInfo()}\n" +
                          $"\n" +
                          $"First channel: {tensor.Channels[0].Print()}");

        if (tensor.Shape != Shape) throw Exception;
        
        return tensor;
    }

    public Tensor BackPropagate(Tensor error, double learningRate, bool backPropagate) {
        Console.WriteLine($"Error tensor: {error.GetInfo()}\n" +
                          $"Learning rate: {learningRate}\n" +
                          $"Back propagation status: {backPropagate.ToString()}\n" +
                          $"\n" +
                          $"First channel: {error.Channels[0].Print()}");

        return error;
    }

    public Tensor GetValues() => null!;

    public string GetData() => "";

    public string LoadData(string data) => data;
}