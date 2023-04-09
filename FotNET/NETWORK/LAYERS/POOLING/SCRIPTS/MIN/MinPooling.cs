using FotNET.NETWORK.OBJECTS.MATH_OBJECTS;

namespace FotNET.NETWORK.LAYERS.POOLING.SCRIPTS.MIN;

public class MinPooling : Pooling {
    protected override Matrix Pool(Matrix matrix, int poolSize) {
        var outputWidth  = matrix.Rows / poolSize;
        var outputHeight = matrix.Columns / poolSize;

        var pooledMatrix = new Matrix(new double[outputWidth, outputHeight]);

        for (var x = 0; x < outputWidth; x++) 
            for (var y = 0; y < outputHeight; y++) {
                var minValue = double.MaxValue;

                for (var i = 0; i < poolSize; i++) 
                    for (var j = 0; j < poolSize; j++) {
                        var inputVal = matrix.Body[x * poolSize + i, y * poolSize + j];
                        minValue = Math.Min(minValue, inputVal);
                    }
                            
                pooledMatrix.Body[x, y] = minValue;
            }
            
        return pooledMatrix;
    }

    protected override Matrix BackPool(Matrix matrix, Matrix referenceMatrix, int poolSize) {
        var backPooledMatrix = new Matrix(referenceMatrix.Rows, referenceMatrix.Columns);

        for (var x = 0; x < matrix.Rows; x++) 
            for (var y = 0; y < matrix.Rows; y++) {
                var minValue = double.MaxValue;

                var minX = 0;
                var minY = 0;

                for (var i = 0; i < poolSize; i++) 
                    for (var j = 0; j < poolSize; j++) {
                        var inputVal = referenceMatrix.Body[x * poolSize + i, y * poolSize + j];
                        if (!(inputVal < minValue)) continue;
                        minValue = inputVal;
                        minX = x * poolSize + i;
                        minY = y * poolSize + j;
                    }
                            
                backPooledMatrix.Body[minX, minY] += matrix.Body[x, y];
            }
            
        return backPooledMatrix;
    }
}