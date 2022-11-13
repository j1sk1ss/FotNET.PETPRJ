using System;

namespace NeuroWeb.EXMPL.SCRIPTS
{
    public class MatrixWorker
    {
        public T[] GetRow<T>(T[,] matrix, int row) {
            if (matrix.GetLength(0) <= row)
                throw new Exception();
        
            var rowRes = new T[matrix.GetLength(1)];
            for (var i = 0; i < matrix.GetLength(1); i++) {
                rowRes[i] = matrix[row, i]; 
            }
        
            return rowRes;
        }

        public T[,] SetRow<T>(T[,] matrix, int position, T[] row)
        {
            for (var i = 0; i < matrix.GetLength(1); i++) {
                matrix[position, i] = row[i];
            }

            return matrix;
        }
    }
}