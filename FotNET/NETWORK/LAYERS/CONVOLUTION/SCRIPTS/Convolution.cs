using FotNET.NETWORK.LAYERS.CONVOLUTION.SCRIPTS.PADDING.SAME;
using FotNET.NETWORK.OBJECTS.MATH_OBJECTS;

namespace FotNET.NETWORK.LAYERS.CONVOLUTION.SCRIPTS {
    public static class Convolution {
        public static Matrix GetConvolution(Matrix matrix, Matrix filter, int stride, double bias) {
            var xFilterSize = filter.Rows;
            var yFilterSize = filter.Columns;

            var xMatrixSize = matrix.Rows;
            var yMatrixSize = matrix.Columns;
            
            var conMat = new Matrix(xMatrixSize - xFilterSize + 1, yMatrixSize - yFilterSize + 1);

            Parallel.For(0, conMat.Rows, i => {
                    for (var j = 0; j < conMat.Columns; j += stride) {
                        var subMatrix = matrix.GetSubMatrix(i, j, i + xFilterSize, j + yFilterSize);
                        conMat.Body[i, j] += (filter * subMatrix).Sum() + bias;
                    }
            });

            return conMat;
        }

        public static Tensor GetConvolution(Tensor tensor, Filter[] filters, int stride) {
            var xSize = tensor.Channels[0].Rows - filters[0].Channels[0].Columns + 1;
            var ySize = tensor.Channels[0].Columns - filters[0].Channels[0].Rows + 1;

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
        
        public static Tensor BackConvolution(Tensor tensor, Filter[] filters, int stride) =>
            GetConvolution(new SamePadding().GetPadding(tensor, filters[0].Channels[0].Rows - 1), filters, stride);
    }
}