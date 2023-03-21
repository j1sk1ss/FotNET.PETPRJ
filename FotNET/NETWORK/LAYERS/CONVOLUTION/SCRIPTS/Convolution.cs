using FotNET.NETWORK.OBJECTS.MATH_OBJECTS;

namespace FotNET.NETWORK.LAYERS.CONVOLUTION.SCRIPTS {
    public static class Convolution {
        public static Matrix GetConvolution(Matrix matrix, Matrix filter, int stride, double bias) {
            var xFilterSize = filter.Rows;
            var yFilterSize = filter.Columns;

            var xMatrixSize = matrix.Rows;
            var yMatrixSize = matrix.Columns;
            
            var conMat = new Matrix(xMatrixSize - xFilterSize + 1, yMatrixSize - yFilterSize + 1);

            for (var i = 0; i < conMat.Rows; i += stride) 
                for (var j = 0; j < conMat.Columns; j += stride) {
                    var subMatrix = matrix.GetSubMatrix(i, j, i + xFilterSize, j + yFilterSize);
                    conMat.Body[i, j] += (filter * subMatrix).Sum() + bias;
                }
            
            return conMat;
        }

        public static Tensor GetConvolution(Tensor tensor, Filter[] filters, int stride) {
            var newTensor = new Tensor(new List<Matrix>());
            
            var xSize = tensor.Channels[0].Rows - filters[0].Channels[0].Columns + 1;
            var ySize = tensor.Channels[0].Columns - filters[0].Channels[0].Rows + 1;

            foreach (var filter in filters) {
                var tempMatrix = new Matrix(xSize, ySize);

                for (var j = 0; j < tensor.Channels.Count; j++)
                    tempMatrix += GetConvolution(tensor.Channels[j], filter.Channels[j], stride, filter.Bias);

                newTensor.Channels.Add(tempMatrix);
            }

            return newTensor;
        }

        public static Tensor GetExtendedConvolution(Tensor tensor, Filter[] filters, int stride) =>
             GetConvolution(Padding.GetPadding(tensor,
                filters[0].Channels[0].Rows - 1), filters, stride);
    }
}