using FotNET.NETWORK.MATH.OBJECTS;

namespace FotNET.NETWORK.MATH.Initialization.Xavier;

public class NormalizedXavierInitialization : IWeightsInitialization {
    public NormalizedXavierInitialization(double value) => Value = value;
    
    private double Value { get; }
    
    public Matrix Initialize(Matrix matrix) {
        for (var i = 0; i < matrix.Rows; i++)
            for (var j = 0; j < matrix.Columns; j++)
                matrix.Body[i, j] = new Random().NextDouble() % (Math.Sqrt(Value) / Math.Sqrt(matrix.Columns + matrix.Rows)) 
                                    - Math.Sqrt(Value) / Math.Sqrt(matrix.Columns + matrix.Rows);

        return matrix;
    }
}