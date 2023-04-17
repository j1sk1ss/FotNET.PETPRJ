using FotNET.NETWORK.MATH.OBJECTS;

namespace FotNET.NETWORK.LAYERS.ROUGHEN;

public class RoughenLayer : ILayer {
    /// <summary> Layer that convert 1D vector-data tensor to multi-dimension data tensor. </summary>
    public RoughenLayer(int xSize, int ySize, int depth) {
        XSize = xSize;
        YSize = ySize;
        Depth = depth;
    }
    
    private int XSize { get; }
    private int YSize { get; }
    private int Depth { get; }
    
    public Tensor GetNextLayer(Tensor tensor) =>
         new Vector(tensor.Flatten().ToArray()).AsTensor(XSize, YSize, Depth);

    public Tensor BackPropagate(Tensor error, double learningRate, bool backPropagate) =>
        new Vector(error.Flatten().ToArray()).AsTensor(1, error.Flatten().Count, 1);

    public Tensor GetValues() => null!;

    public string GetData() => "";

    public string LoadData(string data) => data;
}