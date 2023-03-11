using FotNET.NETWORK.OBJECTS;

namespace FotNET.NETWORK.LAYERS.POOLING.SCRIPTS {
    public static class Pooling {
        public static Tensor BackMaxPool(Tensor picture, Tensor previousTensor, int poolSize) {
            for (var i = 0; i < picture.Channels.Count; i++) 
                picture.Channels[i] = MatrixBackMaxPool(picture.Channels[i], previousTensor.Channels[i], poolSize);
            
            return picture;
        }

        public static Tensor MaxPool(Tensor picture, int poolSize) {
            for (var i = 0; i < picture.Channels.Count; i++) 
                picture.Channels[i] = MatrixMaxPool(picture.Channels[i], poolSize);
            
            return picture;
        }

        public static Tensor AveragePool(Tensor picture, int poolSize) {
            for (var i = 0; i < picture.Channels.Count; i++) 
                picture.Channels[i] = MatrixAveragePool(picture.Channels[i], poolSize, 1);
            
            return picture;
        }

        private static Matrix MatrixBackMaxPool(Matrix matrix, Matrix referenceMatrix, int poolSize) {
            var backPooledMatrix      = new Matrix(referenceMatrix.Body.GetLength(0),
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

        private static Matrix MatrixMaxPool(Matrix matrix, int poolSize) {
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

        private static Matrix MatrixAveragePool(Matrix matrix, int poolSize, int stride) {
            var inputHeight = matrix.Body.GetLength(0);
            var inputWidth  = matrix.Body.GetLength(1);

            var outputHeight = (inputHeight - poolSize) / stride + 1;
            var outputWidth  = (inputWidth - poolSize) / stride + 1;

            var output = new double[outputHeight, outputWidth];

            for (var i = 0; i < outputHeight; i++) 
                for (var j = 0; j < outputWidth; j++) {
                    var sum = 0.0;
                    for (var k = i * stride; k < i * stride + poolSize; k++)
                        for (var l = j * stride; l < j * stride + poolSize; l++)
                            sum += matrix.Body[k, l];

                    output[i, j] = sum / (poolSize * poolSize);
                }
            
            return new Matrix(output);
        }
    }
}