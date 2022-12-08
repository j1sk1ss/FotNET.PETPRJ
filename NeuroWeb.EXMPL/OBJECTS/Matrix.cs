using System;

namespace NeuroWeb.EXMPL.OBJECTS {
    public class Matrix {
        private Matrix(double[,] body) {
            Row  = body.GetLength(0);
            Col  = body.GetLength(1);
            Body = body;
        }
        
        public Matrix(int row, int col) {
            Row  = row;
            Col  = col;
            Body = new double[row, col];
            
            for (var i = 0; i < Row; i++)
                for (var j = 0; j < Col; j++) {
                    Body[i, j] = 0.0;
                }
        }
        
        private int Row { get; }
        
        private int Col { get; }
        
        public double[,] Body { get; }
        
        public Matrix GetTranspose() {
            var rows    = Body.GetLength(0);
            var columns = Body.GetLength(1);
            var temp = new double[columns, rows];

            for (var i = 0; i < temp.GetLength(0); i++) {
                for (var j = 0; j < temp.GetLength(1); j++) {
                    temp[i, j] = Body[j, i];
                }
            }

            return new Matrix(temp);
        }
        
        public static double[] operator *(Matrix matrix, double[] neuron) {
            if (matrix.Col != neuron.Length) throw new Exception();
            
            var c = new double[matrix.Row];
            
            for (var x = 0; x < matrix.Row; ++x) {
                double tmp = 0;
                for (var y = 0; y < matrix.Col; ++y) 
                    tmp += matrix.Body[x, y] * neuron[y];
                
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

        public string GetValues() {
            var tempValues = "";

            for (var i = 0; i < Row; i++) 
                for (var j = 0; j < Col; j++)
                    tempValues += Body[i, j] + " ";
            
            return tempValues;
        }

        public void SetValues(string value, int x, int y) {
            if (double.TryParse(value, out var db)) Body[x, y] = db;
        }
    }
}