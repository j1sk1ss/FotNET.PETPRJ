using FotNET.NETWORK.MATH.OBJECTS;

namespace FotNET.NETWORK.LAYERS.CONVOLUTION.SCRIPTS.PADDING.EQUALS;

public class EqualsPadding : Padding {
    /// <summary>
    /// Equals padding with reference tensor that will be equals with tensor
    /// </summary>
    /// <param name="tensor"> Reference tensor </param>
    public EqualsPadding(Tensor tensor) => PaddingSize = tensor.Channels[0].Rows - 1;
    
    private int PaddingSize { get; set; }

    protected override Matrix GetPadding(Matrix matrix) {
        PaddingSize -= matrix.Columns;
        var newMatrix = new Matrix(matrix.Rows + PaddingSize * 2, matrix.Columns + PaddingSize * 2);

        for (var i = PaddingSize; i < newMatrix.Rows - PaddingSize; i++)
            for (var j = PaddingSize; j < newMatrix.Columns - PaddingSize; j++)
                newMatrix.Body[i, j] = matrix.Body[i - PaddingSize, j - PaddingSize];

        return newMatrix;
    }
}