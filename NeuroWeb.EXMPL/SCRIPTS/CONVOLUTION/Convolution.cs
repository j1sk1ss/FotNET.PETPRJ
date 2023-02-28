using System.Collections.Generic;
using System.Linq;
using System.Windows;
using NeuroWeb.EXMPL.OBJECTS;

namespace NeuroWeb.EXMPL.SCRIPTS.CONVOLUTION {
    public static class Convolution {
        private static Matrix GetConvolution(Matrix matrix, Matrix filter, int stride, List<double> bias) {
            var xFilterSize = filter.Body.GetLength(0);
            var yFilterSize = filter.Body.GetLength(1);
            
            var matrixSize = matrix.Body.GetLength(0);
            var conMat = new Matrix(matrixSize - xFilterSize + 1, matrixSize - yFilterSize + 1);
            
            for (var f = 0; f < bias.Count; f++)
                for (var i = 0; i < conMat.Body.GetLength(0); i += stride) {
                    for (var j = 0; j < conMat.Body.GetLength(1); j += stride) {
                        var subMatrix = matrix.GetSubMatrix(i, j, i + xFilterSize, j + yFilterSize);
                        conMat.Body[i,j] += (filter * subMatrix).GetSum() + bias[f];
                    }
                }
            
            return conMat;
        }
        
        public static Tensor GetConvolution(Matrix matrix, Filter filters, int stride) {
            var newTensor = filters.Channels.Select(filter => GetConvolution(matrix, filter, stride, filters.Bias)).ToList();
            return new Tensor(newTensor);
        }
        
        public static Tensor GetConvolution(Tensor matrix, Filter filters, int stride) {
            var newMatrix = new List<Matrix>();

            foreach (var t1 in matrix.Channels) 
                newMatrix.AddRange(GetConvolution(t1, filters, stride).Channels);

            return new Tensor(newMatrix);
        }
    }
}