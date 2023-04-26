using FotNET.NETWORK.MATH.OBJECTS;

namespace FotNET.NETWORK.LAYERS.CONVOLUTION.SCRIPTS {
    public static class Convolution {
        public static Matrix GetConvolution(Matrix matrix, Matrix filter, int stride, double bias) {
            var conMat = new Matrix((matrix.Rows - filter.Rows) / stride + 1, 
                (matrix.Columns - filter.Columns) / stride + 1);

            Parallel.For(0, conMat.Rows, i => {
                    for (var j = 0; j < conMat.Columns; j += stride) {
                        var subMatrix = matrix.GetSubMatrix(i, j, i + filter.Rows, j + filter.Columns);
                        conMat.Body[i, j] += (filter * subMatrix).Sum() + bias;
                    }
            });

            return conMat;
        }

        public static Tensor GetConvolution(Tensor tensor, Filter[] filters, int stride) {
            var xSize = (tensor.Channels[0].Rows - filters[0].Channels[0].Rows) / stride + 1;
            var ySize = (tensor.Channels[0].Columns - filters[0].Channels[0].Columns) / stride + 1;

            var newTensor = new Tensor(new List<Matrix>());
            for (var i = 0; i < filters.Length; i++)
                newTensor.Channels.Add(new Matrix(0,0));
            
            Parallel.For(0, filters.Length, filter => {
                    var tempMatrix = new Matrix(xSize, ySize);

                    for (var j = 0; j < tensor.Channels.Count; j++) {
                        tempMatrix += GetConvolution(tensor.Channels[j], filters[filter].Channels[j], stride,
                            filters[filter].Bias);
                    }

                    newTensor.Channels[filter] = tempMatrix;
            });
            
            return newTensor;
        }
    }
}