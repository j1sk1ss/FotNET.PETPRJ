using FotNET.NETWORK.OBJECTS;

namespace FotNET.NETWORK.LAYERS.POOLING.SCRIPTS.MIN;

public class MinPooling : Pooling {
    protected override Matrix Pool(Matrix matrix, int poolSize) {
        var outputWidth  = matrix.Body.GetLength(0) / poolSize;
        var outputHeight = matrix.Body.GetLength(1) / poolSize;

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
        var backPooledMatrix = new Matrix(referenceMatrix.Body.GetLength(0),
            referenceMatrix.Body.GetLength(1));

        for (var x = 0; x < matrix.Body.GetLength(0); x++) 
            for (var y = 0; y < matrix.Body.GetLength(1); y++) {
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