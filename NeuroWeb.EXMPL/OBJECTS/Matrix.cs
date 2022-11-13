using System;

namespace NeuroWeb.EXMPL.OBJECTS
{
    public class Matrix
    {
        public Matrix(int row, int col) {
            Row = row;
            Col = col;
            Body = new double[row, col];
            
            for (var i = 0; i < Row; i++)
                for (var j = 0; j < Col; j++)
                {
                    Body[i, j] = 0.0;
                }
        }
        private int Row { get; }
        private int Col { get; }
        private double[,] Body { get; }
        
        public static double[] operator *(Matrix matrix, double[] neuron) {
            if (matrix.Col != neuron.Length) throw new Exception();
            
            var c = new double[matrix.Row];
            
            for (var x = 0; x < matrix.Row; ++x) {
                double tmp = 0;
                for (var y = 0; y < matrix.Col; ++y) {
                    tmp += matrix.Body[x, y] * neuron[y];
                }
                c[x] = tmp;
            }
            
            return c;
        }

        public void FillRandom() {
            for (var i = 0; i < Row; i++)
                for (var j = 0; j < Col; j++) {
                    Body[i, j] = new Random().Next() % 100 * 0.03 / (Row + 35);
                }
        }
    }
}