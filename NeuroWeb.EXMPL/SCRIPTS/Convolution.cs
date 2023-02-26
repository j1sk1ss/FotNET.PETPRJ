using System.Collections.Generic;
using System.Linq;
using NeuroWeb.EXMPL.OBJECTS;

namespace NeuroWeb.EXMPL.SCRIPTS {
    public static class Convolution {
        public static Matrix GetConvolution(Matrix matrix, Matrix filter) {
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
        
        public static List<Matrix> GetConvolution(Matrix matrix, IEnumerable<Matrix> filters) {
            return filters.Select(t => GetConvolution(matrix, t)).ToList();
        }
    }
}