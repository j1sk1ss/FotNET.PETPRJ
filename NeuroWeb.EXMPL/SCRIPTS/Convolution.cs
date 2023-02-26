using System.Collections.Generic;
using System.Linq;
using NeuroWeb.EXMPL.OBJECTS;

namespace NeuroWeb.EXMPL.SCRIPTS {
    public static class Convolution {
        private static Matrix GetConvolution(Matrix matrix, Matrix filter) {
            var xFilterSize = filter.Body.GetLength(0);
            var yFilterSize = filter.Body.GetLength(1);
            
            var matrixSize = matrix.Body.GetLength(0);
            var conMat = new Matrix(matrixSize - xFilterSize + 1, matrixSize - yFilterSize + 1);
            
            for (var i = 0; i < conMat.Body.GetLength(0); i++) {
                for (var j = 0; j < conMat.Body.GetLength(1); j++) {
                    var subMatrix = matrix.GetSubMatrix(i, j, i + xFilterSize, j + yFilterSize);
                    conMat.Body[i,j] = (filter * subMatrix).GetSum();
                }
            }
            
            return conMat;
        }
        
        public static Tensor GetConvolution(Matrix matrix, Tensor filters) {
            return new Tensor(filters.Body.Select(t => GetConvolution(matrix, t)).ToList());
        }
        
        public static Tensor GetConvolution(Tensor matrix, Tensor filters) {
            var newMatrix = new List<Matrix>();

            foreach (var t1 in matrix.Body) 
                newMatrix.AddRange(filters.Body.Select(t => GetConvolution(t1, t)));
            
            return new Tensor(newMatrix);
        }
    }
}