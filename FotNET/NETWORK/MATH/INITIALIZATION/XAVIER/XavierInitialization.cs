using FotNET.NETWORK.MATH.OBJECTS;

namespace FotNET.NETWORK.MATH.Initialization.Xavier;

public class XavierInitialization : IWeightsInitialization {
    public Matrix Initialize(Matrix matrix) {
        for (var i = 0; i < matrix.Rows; i++)
            for (var j = 0; j < matrix.Columns; j++)
                matrix.Body[i, j] = new Random().NextDouble() % (1.0d / Math.Sqrt(matrix.Columns)) 
                                    - 1.0d / Math.Sqrt(matrix.Columns);

        return matrix;
    }
}
