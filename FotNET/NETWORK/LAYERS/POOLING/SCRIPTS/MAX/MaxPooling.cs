using FotNET.NETWORK.OBJECTS;

namespace FotNET.NETWORK.LAYERS.POOLING.SCRIPTS.MAX;

public class MaxPooling : Pooling {
    protected override Matrix Pool(Matrix matrix, int poolSize) {
        var outputWidth  = matrix.Body.GetLength(0) / poolSize;
        var outputHeight = matrix.Body.GetLength(1) / poolSize;

        var pooledMatrix = new Matrix(new double[outputWidth, outputHeight]);

        for (var x = 0; x < outputWidth; x++) 
            for (var y = 0; y < outputHeight; y++) {
                var maxVal = double.MinValue;

                for (var i = 0; i < poolSize; i++) 
                    for (var j = 0; j < poolSize; j++) {
                        var inputVal = matrix.Body[x * poolSize + i, y * poolSize + j];
                        maxVal = Math.Max(maxVal, inputVal);
                    }
                        
                pooledMatrix.Body[x, y] = maxVal;
            }
            
        return pooledMatrix;
    }

    protected override Matrix BackPool(Matrix matrix, Matrix referenceMatrix, int poolSize) {
        var backPooledMatrix = new Matrix(referenceMatrix.Body.GetLength(0),
            referenceMatrix.Body.GetLength(1));

        for (var x = 0; x < matrix.Body.GetLength(0); x++) 
            for (var y = 0; y < matrix.Body.GetLength(1); y++) {
                var maxVal = double.MinValue;

                var maxX = 0;
                var maxY = 0;

                for (var i = 0; i < poolSize; i++) 
                    for (var j = 0; j < poolSize; j++) {
                        var inputVal = referenceMatrix.Body[x * poolSize + i, y * poolSize + j];
                        if (!(inputVal > maxVal)) continue;
                        maxVal = inputVal;
                        maxX = x * poolSize + i;
                        maxY = y * poolSize + j;
                    }
                        
                backPooledMatrix.Body[maxX, maxY] += matrix.Body[x, y];
            }
            
        return backPooledMatrix;
    }
}