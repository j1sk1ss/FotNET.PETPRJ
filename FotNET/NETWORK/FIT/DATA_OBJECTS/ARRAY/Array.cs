using FotNET.NETWORK.OBJECTS;

namespace FotNET.NETWORK.FIT.DATA_OBJECTS.ARRAY;

public class Array : IData {
    public Array(double[] data, double[] rightAnswer) {
        Body  = data;
        Right = rightAnswer;
    }
    
    private double[] Body { get; }
    private double[] Right { get; }

    public Tensor GetRight() =>
        new Vector(Right).AsTensor(1, Right.Length, 1);

    public Tensor AsTensor() =>
         new Vector(Body).AsTensor(1, Body.Length, 1);
}