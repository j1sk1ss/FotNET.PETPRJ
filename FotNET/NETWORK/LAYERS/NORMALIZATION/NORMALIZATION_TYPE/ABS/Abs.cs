using FotNET.NETWORK.OBJECTS.MATH_OBJECTS;

namespace FotNET.NETWORK.LAYERS.NORMALIZATION.NORMALIZATION_TYPE.ABS;

public class Abs : INormalization {
    public Tensor Normalize(Tensor tensor) {
        var newTensor = new Tensor(new List<Matrix>());
        
        foreach (var matrix in tensor.Channels) {
            var newMatrix = new Matrix(matrix.Rows, matrix.Columns);
            
            for (var i = 0; i < matrix.Rows; i++) 
                for (var j = 0; j < matrix.Columns; j++) 
                    newMatrix.Body[i, j] = Math.Abs(matrix.Body[i, j]);
            
            newTensor.Channels.Add(newMatrix);
        }
        
        return newTensor;
    }
}