using FotNET.NETWORK.MATH.OBJECTS;

namespace FotNET.NETWORK.LAYERS.CONVOLUTION.SCRIPTS.PADDING.CUSTOM;

public class CustomPadding : Padding {
    public CustomPadding(int puddingSize) => PaddingSize = puddingSize;
    
    private int PaddingSize { get; }
    
    public override Matrix GetPadding(Matrix matrix) {
        var newMatrix = new Matrix(matrix.Rows + PaddingSize, matrix.Columns + PaddingSize);

        for (var i = PaddingSize; i < newMatrix.Rows - PaddingSize; i++)
        for (var j = PaddingSize; j < newMatrix.Columns - PaddingSize; j++)
            newMatrix.Body[i, j] = matrix.Body[i - PaddingSize, j - PaddingSize];

        return newMatrix;
    }
}