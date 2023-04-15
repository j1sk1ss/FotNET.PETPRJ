using FotNET.NETWORK.MATH.OBJECTS;

namespace FotNET.NETWORK.LAYERS.POOLING.SCRIPTS.AVERAGE;

public class AveragePooling : Pooling {
    protected override Matrix Pool(Matrix matrix, int poolSize) {
        var outputWidth  = matrix.Rows / poolSize;
        var outputHeight = matrix.Columns / poolSize;

        var output = new double[outputHeight, outputWidth];

        Parallel.For(0, outputHeight, i => {
            for (var j = 0; j < outputWidth; j++) {
                var sum = 0.0;
                for (var k = i * poolSize; k < i * poolSize + poolSize; k++)
                for (var l = j * poolSize; l < j * poolSize + poolSize; l++)
                    sum += matrix.Body[k, l];

                output[i, j] = sum / (poolSize * poolSize);
            }
        });
        
        return new Matrix(output);
    }

    protected override Matrix BackPool(Matrix matrix, Matrix referenceMatrix, int poolSize) {
        var backPooledMatrix = new Matrix(referenceMatrix.Rows, referenceMatrix.Columns);

        Parallel.For(0, matrix.Rows, x => {
            for (var y = 0; y < matrix.Columns; y++) 
                for (var i = 0; i < referenceMatrix.Rows; i++) 
                    for (var j = 0; j < referenceMatrix.Columns; j++) 
                        backPooledMatrix.Body[i, j] += matrix.Body[x, y] / poolSize * 2;   
        });
        
        return backPooledMatrix;
    }
}