using FotNET.NETWORK.LAYERS;
using FotNET.NETWORK.OBJECTS.MATH_OBJECTS;

namespace FotNET.NETWORK.ROUGHEN;

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

    public Tensor BackPropagate(Tensor error, double learningRate, bool backPropagate) => new (new Matrix(error.Flatten().ToArray()));

    public Tensor GetValues() => null!;

    public string GetData() => "";

    public string LoadData(string data) => data;
}