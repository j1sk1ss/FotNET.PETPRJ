using FotNET.NETWORK.OBJECTS.MATH_OBJECTS;

namespace FotNET.NETWORK.MATH.Initialization.Xavier;

public class NormalizedXavierInitialization : IWeightsInitialization {
    public Matrix Initialize(Matrix matrix) {
        for (var i = 0; i < matrix.Rows; i++)
            for (var j = 0; j < matrix.Columns; j++)
                matrix.Body[i, j] = new Random().NextDouble() % (Math.Sqrt(6d) / Math.Sqrt(matrix.Columns + matrix.Rows)) 
                                    - Math.Sqrt(6d) / Math.Sqrt(matrix.Columns + matrix.Rows);

        return matrix;
    }
}