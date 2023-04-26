using FotNET.NETWORK.MATH.OBJECTS;

namespace FotNET.NETWORK.LAYERS.POOLING.SCRIPTS.BILINEAR;

/// <summary>
/// Takes bilinear value from area and puts in new matrix
/// </summary>
public class BilinearPooling : Pooling {
    protected override Matrix BackPool(Matrix matrix, Matrix referenceMatrix, int poolSize) {
        var height = matrix.Rows * poolSize;
        var width  = matrix.Columns * poolSize;
        
        var dInput = new Matrix(height, width);

        Parallel.For(0, matrix.Rows, i => {
            for (var j = 0; j < matrix.Columns; j++) 
                         for (var p = 0; p < poolSize; p++) 
                             for (var q = 0; q < poolSize; q++) 
                                 dInput.Body[i * poolSize + p, j * poolSize + q] 
                                     = matrix.Body[i, j] * (1 - Math.Abs(i * poolSize + p + 0.5 - height) / poolSize) 
                                                         * (1 - Math.Abs(j * poolSize + q + 0.5 - width) / poolSize);
        });

        return dInput;
    }

    protected override Matrix Pool(Matrix matrix, int poolSize) {
        var outHeight = matrix.Rows / poolSize;
        var outWidth  = matrix.Columns / poolSize;

        var output = new Matrix(outHeight, outWidth);

        Parallel.For(0, outHeight, i => {
            for (var j = 0; j < outWidth; j++) {
                var val = 0d;
                
                for (var p = 0; p < poolSize; p++) 
                    for (var q = 0; q < poolSize; q++) 
                        val += matrix.Body[i * poolSize + p, j * poolSize + q] 
                               * (1 - Math.Abs(i * poolSize + p + 0.5 - matrix.Rows) / poolSize) 
                               * (1 - Math.Abs(j * poolSize + q + 0.5 - matrix.Columns) / poolSize);
                
                output.Body[i, j] = val;
            }
        });

        return output;
    }
}