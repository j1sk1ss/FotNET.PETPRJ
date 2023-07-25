using FotNET.NETWORK.MATH.OBJECTS;

namespace FotNET.NETWORK.LAYERS.NOISE.SCRIPTS.ZERO;

public class ZeroNoise : Noise {
    public override Vector GenerateNoise(int size) {
        var body = new double[size];

        for (var i = 0; i < size; i++)
            body[i] = 0;
        
        return new Vector(body);
    }

    protected override Matrix GenerateNoise((int Rows, int Columns) shape) {
        var body = new double[shape.Rows, shape.Columns];

        for (var i = 0; i < shape.Rows; i++)
            for (var j = 0; j < shape.Columns; j++)
                body[i,j] = 0;
        
        return new Matrix(body);
    }
}