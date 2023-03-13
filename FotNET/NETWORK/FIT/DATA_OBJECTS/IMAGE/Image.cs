using FotNET.NETWORK.OBJECTS;

namespace FotNET.NETWORK.FIT.DATA_OBJECTS.IMAGE;

public class Image : IData {
    public Image(double[,,] image, double[] rightAnswer) {
        Body  = image;
        Right = rightAnswer;
    }
    
    private double[,,] Body { get; }
    private double[] Right { get; }

    public Tensor GetRight() =>
        new Vector(Right).AsTensor(1, Right.Length, 1);

    public Tensor AsTensor() {
        var tensor = new Tensor(new List<Matrix>());
        for (var depth = 0; depth < Body.GetLength(2); depth++)
        {
            tensor.Channels.Add(new Matrix(Body.GetLength(0), Body.GetLength(1)));
            for (var x = 0; x < Body.GetLength(0); x++)
                for (var y = 0; y < Body.GetLength(1); y++)
                    tensor.Channels[^1].Body[x, y] = Body[x, y, depth];
        }

        return tensor;
    }
}