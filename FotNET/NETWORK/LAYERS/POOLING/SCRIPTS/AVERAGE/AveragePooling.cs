using FotNET.NETWORK.OBJECTS;

namespace FotNET.NETWORK.LAYERS.POOLING.SCRIPTS.AVERAGE;

public class AveragePooling : Pooling {
    protected override Matrix Pool(Matrix matrix, int poolSize) {
        var outputWidth  = matrix.Body.GetLength(0) / poolSize;
        var outputHeight = matrix.Body.GetLength(1) / poolSize;

        var output = new double[outputHeight, outputWidth];

        for (var i = 0; i < outputHeight; i++) 
            for (var j = 0; j < outputWidth; j++) {
                var sum = 0.0;
                for (var k = i * poolSize; k < i * poolSize + poolSize; k++)
                    for (var l = j * poolSize; l < j * poolSize + poolSize; l++)
                        sum += matrix.Body[k, l];

                output[i, j] = sum / (poolSize * poolSize);
            }
            
        return new Matrix(output);
    }

    protected override Matrix BackPool(Matrix matrix, Matrix referenceMatrix, int poolSize) {
        var backPooledMatrix = new Matrix(referenceMatrix.Body.GetLength(0),
            referenceMatrix.Body.GetLength(1));

        for (var x = 0; x < matrix.Body.GetLength(0); x++) 
            for (var y = 0; y < matrix.Body.GetLength(1); y++) 
                for (var i = 0; i < referenceMatrix.Body.GetLength(0); i++) 
                    for (var j = 0; j < referenceMatrix.Body.GetLength(0); j++) 
                        backPooledMatrix.Body[i, j] += matrix.Body[x, y] / poolSize * 2;   
        
        return backPooledMatrix;
    }
}