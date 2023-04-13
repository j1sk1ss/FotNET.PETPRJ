using FotNET.NETWORK.OBJECTS.MATH_OBJECTS;

namespace FotNET.NETWORK.LAYERS.CONVOLUTION.SCRIPTS.PADDING.CUSTOM;

public class CustomPadding : Padding {
    public CustomPadding(int puddingSize) => PaddingSize = puddingSize;
    
    private int PaddingSize { get; }
    
    protected override Matrix GetPadding(Matrix matrix, int paddingSize) {
        var newMatrix = new Matrix(matrix.Rows + PaddingSize, matrix.Columns + PaddingSize);

        for (var i = PaddingSize; i < newMatrix.Rows - PaddingSize; i++)
        for (var j = PaddingSize; j < newMatrix.Columns - PaddingSize; j++)
            newMatrix.Body[i, j] = matrix.Body[i - PaddingSize, j - PaddingSize];

        return newMatrix;
    }
}