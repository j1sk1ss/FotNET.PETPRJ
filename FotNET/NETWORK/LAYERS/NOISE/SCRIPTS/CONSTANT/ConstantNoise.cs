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
    
    public Matrix GenerateNoise((int Rows, int Columns) shape) {
        var body = new double[shape.Rows, shape.Columns];

        for (var i = 0; i < shape.Rows; i++)
        for (var j = 0; j < shape.Columns; j++)
            body[i,j] = Constant;
        
        return new Matrix(body);
    }

    public Tensor GenerateNoise((int Rows, int Columns, int Depth) shape) {
        var body = new List<Matrix>();

        for (var i = 0; i < shape.Depth; i++) 
            body.Add(GenerateNoise((shape.Rows, shape.Columns)));
        
        return new Tensor(body);
    }
}