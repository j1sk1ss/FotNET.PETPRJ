using FotNET.NETWORK.MATH.OBJECTS;

namespace FotNET.NETWORK.LAYERS.NOISE.SCRIPTS.RANDOM;

public class RandomNoise : INoise {
    public RandomNoise(double min = 0, double max = 1) {
        Min = min;
        Max = max;
    }

    private double Min { get; }
    private double Max { get; }

    public Vector GenerateNoise(int size) {
        var body = new double[size];
        
        for (var i = 0; i < size; i++) 
            body[i] = new Random().NextDouble() % Max - Min;
        
        return new Vector(body);
    }
}