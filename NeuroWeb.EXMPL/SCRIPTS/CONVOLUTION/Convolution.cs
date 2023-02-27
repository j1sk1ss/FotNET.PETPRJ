using System.Collections.Generic;
using System.Linq;
using NeuroWeb.EXMPL.OBJECTS;

namespace NeuroWeb.EXMPL.SCRIPTS {
    public static class Convolution {
        private static Matrix GetConvolution(Matrix matrix, Matrix filter, int stride) {
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
        
        public static Tensor GetConvolution(Matrix matrix, Tensor filters, int stride) {
            return new Tensor(filters.Body.Select(t => GetConvolution(matrix, t, stride)).ToList());
        }
        
        public static Tensor GetConvolution(Tensor matrix, Tensor filters, int stride) {
            var newMatrix = new List<Matrix>();

            foreach (var t1 in matrix.Body) 
                newMatrix.AddRange(filters.Body.Select(t => GetConvolution(t1, t, stride)));
            
            return new Tensor(newMatrix);
        }
    }
}