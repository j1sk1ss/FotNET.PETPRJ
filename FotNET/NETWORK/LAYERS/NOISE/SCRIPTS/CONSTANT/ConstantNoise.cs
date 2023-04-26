using FotNET.NETWORK.MATH.OBJECTS;

namespace FotNET.NETWORK.LAYERS.NOISE.SCRIPTS.CONSTANT;

public class ConstantNoise : INoise {
    public ConstantNoise(int value) => Constant = value;
    
    private int Constant { get; }
    
    public Vector GenerateNoise(int size) {
        var body = new double[size];
        
        for (var i = 0; i < size; i++) 
            body[i] = Constant;
        
        return new Vector(body);
    }
}