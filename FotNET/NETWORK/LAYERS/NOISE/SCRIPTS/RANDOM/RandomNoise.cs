using FotNET.NETWORK.MATH.OBJECTS;

namespace FotNET.NETWORK.LAYERS.NOISE.SCRIPTS.RANDOM;

public class RandomNoise : Noise {
    public RandomNoise(double min = 0, double max = 1) {
        Min = min;
        Max = max;
    }

    private double Min { get; }
    private double Max { get; }

    public override Vector GenerateNoise(int size) {
        var body = new double[size];
        
        for (var i = 0; i < size; i++) 
            body[i] = new Random().NextDouble() % Max - Min;
        
        return new Vector(body);
    }
    
    protected override Matrix GenerateNoise((int Rows, int Columns) shape) {
        var body = new double[shape.Rows, shape.Columns];

        for (var i = 0; i < shape.Rows; i++)
            for (var j = 0; j < shape.Columns; j++)
                body[i,j] = new Random().NextDouble() % Max - Min;
        
        return new Matrix(body);
    }
}