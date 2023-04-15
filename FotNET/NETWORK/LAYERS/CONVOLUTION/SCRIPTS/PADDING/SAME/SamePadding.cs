using FotNET.NETWORK.MATH.OBJECTS;

namespace FotNET.NETWORK.LAYERS.CONVOLUTION.SCRIPTS.PADDING.SAME;

public class SamePadding : Padding {
    public SamePadding(Tensor tensor) => PaddingSize = tensor.Channels[0].Rows - 2;
    
    private int PaddingSize { get; }
    
    public override Matrix GetPadding(Matrix matrix) {
        var newMatrix = new Matrix(matrix.Rows + PaddingSize * 2, matrix.Columns + PaddingSize * 2);

        for (var i = PaddingSize; i < newMatrix.Rows - PaddingSize; i++)
            for (var j = PaddingSize; j < newMatrix.Columns - PaddingSize; j++)
                newMatrix.Body[i, j] = matrix.Body[i - PaddingSize, j - PaddingSize];

        return newMatrix;
    }
}