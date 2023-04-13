using FotNET.NETWORK.OBJECTS.MATH_OBJECTS;

namespace FotNET.NETWORK.LAYERS.CONVOLUTION.SCRIPTS.PADDING.SAME;

public class SamePadding : Padding {
    protected override Matrix GetPadding(Matrix matrix, int paddingSize) {
        var newMatrix = new Matrix(matrix.Rows + paddingSize, matrix.Columns + paddingSize);

        for (var i = paddingSize; i < newMatrix.Rows - paddingSize; i++)
            for (var j = paddingSize; j < newMatrix.Columns - paddingSize; j++)
                newMatrix.Body[i, j] = matrix.Body[i - paddingSize, j - paddingSize];

        return newMatrix;
    }
}