using System.Collections.Generic;
using System.Linq;
using NeuroWeb.EXMPL.OBJECTS;

namespace NeuroWeb.EXMPL.SCRIPTS {
    public static class Convolution {
        private static Matrix GetConvolution(Matrix matrix, Matrix filter, int stride, double bias) {
            var xFilterSize = filter.Body.GetLength(0);
            var yFilterSize = filter.Body.GetLength(1);
            
            var matrixSize = matrix.Body.GetLength(0);
            var conMat = new Matrix(matrixSize - xFilterSize + 1, matrixSize - yFilterSize + 1);
            
            for (var i = 0; i < conMat.Body.GetLength(0); i += stride) {
                for (var j = 0; j < conMat.Body.GetLength(1); j += stride) {
                    var subMatrix = matrix.GetSubMatrix(i, j, i + xFilterSize, j + yFilterSize);
                    conMat.Body[i,j] = (filter * subMatrix).GetSum();
                }
            }
            
            return conMat;
        }
        
        public static Tensor GetConvolution(Matrix matrix, Filter filters, int stride) {
            return new Tensor(filters.Channels.Select(t => GetConvolution(matrix, t, stride, filters.Bias)).ToList());
        }
        
        public static Tensor GetConvolution(Tensor matrix, Filter filters, int stride) {
            var newMatrix = new List<Matrix>();

            foreach (var t1 in matrix.Channels) 
                newMatrix.AddRange(filters.Channels.Select(t => GetConvolution(t1, t, stride, filters.Bias)));
            
            return new Tensor(newMatrix);
        }
    }
}