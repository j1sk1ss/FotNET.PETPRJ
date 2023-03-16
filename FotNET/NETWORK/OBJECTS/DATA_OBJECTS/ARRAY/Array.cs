using FotNET.NETWORK.DATA.CSV;

namespace FotNET.NETWORK.OBJECTS.DATA_OBJECTS.ARRAY;

public class Array : IData {
    public Array(string[] data, DataConfig config) {
        Body  = new double[config.InputColumnEnd - config.InputColumnStart];
        Right = new double[config.OutputColumnEnd - config.OutputColumnStart];

        for (var i = config.InputColumnStart; i < config.InputColumnEnd ; i++) 
            Body[i - config.InputColumnStart] = double.Parse(data[i]);
        
        for (var i = config.OutputColumnStart; i < config.OutputColumnEnd; i++) 
            Right[i - config.OutputColumnStart] = double.Parse(data[i]);
    }
    
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