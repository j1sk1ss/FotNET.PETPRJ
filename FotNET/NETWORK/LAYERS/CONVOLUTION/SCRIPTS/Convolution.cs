﻿using FotNET.NETWORK.OBJECTS;

namespace FotNET.NETWORK.LAYERS.CONVOLUTION.SCRIPTS {
    public static class Convolution {
        public static Matrix GetConvolution(Matrix matrix, Matrix filter, int stride, double bias) {
            var xFilterSize = filter.Body.GetLength(0);
            var yFilterSize = filter.Body.GetLength(1);

            var xMatrixSize = matrix.Body.GetLength(0);
            var yMatrixSize = matrix.Body.GetLength(1);
            
            var conMat = new Matrix(xMatrixSize - xFilterSize + 1, yMatrixSize - yFilterSize + 1);

            for (var i = 0; i < conMat.Body.GetLength(0); i += stride) 
                for (var j = 0; j < conMat.Body.GetLength(1); j += stride) {
                    var subMatrix = matrix.GetSubMatrix(i, j, i + xFilterSize, j + yFilterSize);
                    conMat.Body[i, j] += (filter * subMatrix).GetSum() + bias;
                }
            
            return conMat;
        }

        public static Tensor GetConvolution(Tensor tensor, Filter[] filters, int stride) {
            var newTensor = new Tensor(new List<Matrix>());
            
            var xSize = tensor.Channels[0].Body.GetLength(0) - filters[0].Channels[0].Body.GetLength(0) + 1;
            var ySize = tensor.Channels[0].Body.GetLength(1) - filters[0].Channels[0].Body.GetLength(0) + 1;

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
                filters[0].Channels[0].Body.GetLength(0) - 1), filters, stride);
    }
}