using FotNET.NETWORK.MATH.OBJECTS;

namespace FotNET.NETWORK.MATH.Initialization.ZERO;

/// <summary>
/// Initialization type where all weights sets to zero
/// </summary>
public class ZeroInitialization : IWeightsInitialization {
    public Matrix Initialize(Matrix matrix) {
        for (var i = 0; i < matrix.Rows; i++)
            for (var j = 0; j < matrix.Columns; j++)
                matrix.Body[i, j] = 0;

        return matrix;
    }
}