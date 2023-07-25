using FotNET.NETWORK.MATH.OBJECTS;

namespace FotNET.NETWORK.LAYERS.NOISE.SCRIPTS;

public abstract class Noise {
    public abstract Vector GenerateNoise(int size);

    protected abstract Matrix GenerateNoise((int Rows, int Columns) shape);

    public Tensor GenerateNoise((int Rows, int Columns, int Depth) shape) {
        var body = new List<Matrix>();

        for (var i = 0; i < shape.Depth; i++) 
            body.Add(GenerateNoise((shape.Rows, shape.Columns)));
        
        return new Tensor(body);
    }
}