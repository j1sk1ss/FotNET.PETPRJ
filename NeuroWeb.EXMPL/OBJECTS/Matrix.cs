using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace NeuroWeb.EXMPL.OBJECTS {
    public class Matrix {
        public Matrix(double[,] body) {
            Row  = body.GetLength(0);
            Col  = body.GetLength(1);
            Body = body;
        }
        
        public Matrix(int row, int col) {
            Row  = row;
            Col  = col;
            Body = new double[row, col];
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

        public Matrix GetFlip() {
            var rotatedMatrix = new Matrix(new double[Row, Col]);
            
            for (var i = 0; i < rotatedMatrix.Row; i++) {
                for (var j = 0; j < rotatedMatrix.Col; j++) {
                    rotatedMatrix.Body[j,i] = Body[Row - j - 1,Col - i - 1];
                }
            }

            return rotatedMatrix;
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

        public static Matrix operator +(Matrix matrix1, Matrix matrix2) {
            var xSize = matrix1.Body.GetLength(0);
            var ySize = matrix2.Body.GetLength(1);

            var endMatrix = new Matrix(new double[xSize, ySize]);

            for (var i = 0; i < xSize; i++)
                for (var j = 0; j < ySize; j++)
                    endMatrix.Body[i, j] = matrix1.Body[i, j] + matrix2.Body[i, j];

            return endMatrix;
        }

        public static Matrix operator *(Matrix matrix1, Matrix matrix2) {
            var xSize = matrix1.Body.GetLength(0);
            var ySize = matrix2.Body.GetLength(1);
            
            var endMatrix = new Matrix(new double[xSize, ySize]);

            for (var i = 0; i < xSize; i++) 
                for (var j = 0; j < ySize; j++) 
                    endMatrix.Body[i, j] = matrix1.Body[i, j] * matrix2.Body[i, j];

            return endMatrix;
        }

        public static Matrix operator -(Matrix matrix1, Matrix matrix2) {
            var xSize = matrix1.Body.GetLength(0);
            var ySize = matrix2.Body.GetLength(1);
            
            var endMatrix = new Matrix(new double[xSize, ySize]);

            for (var i = 0; i < xSize; i++) 
                for (var j = 0; j < ySize; j++) 
                    endMatrix.Body[i, j] = matrix1.Body[i, j] - matrix2.Body[i, j];

            return endMatrix;
        }
        
        public static Matrix operator *(Matrix matrix1, double value) {
            var xSize = matrix1.Body.GetLength(0);
            var ySize = matrix1.Body.GetLength(1);
            
            var endMatrix = new Matrix(new double[xSize, ySize]);

            for (var i = 0; i < xSize; i++) 
                for (var j = 0; j < ySize; j++) 
                    endMatrix.Body[i, j] = matrix1.Body[i, j] * value;

            return endMatrix;
        }
        
        public double GetSum() {
            var sum = 0d;
            for (var i = 0; i < Body.GetLength(0); i++)
                for (var j = 0; j < Body.GetLength(1); j++)
                    sum += Body[i, j];
            return sum;
        }

        public Matrix GetSubMatrix(int x1, int y1, int x2, int y2) {
            var subMatrix = new Matrix(x2 - x1, y2 - y1);
            
            for (var i = x1; i < x2; i++) {
                for (var j = y1; j < y2; j++) {
                    subMatrix.Body[i - x1, j - y1] = Body[i, j];
                }
            }

            return subMatrix;
        }

        public Matrix Resize(int x, int y) {
            var newMatrix = new Matrix(x, y);
            
            for (var i = 0; i < x; i++) 
                if (i < Body.GetLength(0))
                    for (var j = 0; j < y; j++) 
                        if (j < Body.GetLength(1)) newMatrix.Body[i, j] = Body[i, j];
            
            return newMatrix;
        }

        public void FillRandom() {
            for (var i = 0; i < Row; i++)
                for (var j = 0; j < Col; j++) 
                    Body[i, j] = new Random().Next() % 100 * 0.03 / (Row + 35);
        }

        [SuppressMessage("ReSharper.DPA", "DPA0000: DPA issues")]
        public string GetValues() {
            var tempValues = "";

            for (var i = 0; i < Row; i++) 
                for (var j = 0; j < Col; j++)
                    tempValues += Body[i, j] + " ";
            
            return tempValues;
        }

        public string Print() {
            var tempValues = "";

            for (var i = 0; i < Row; i++) {
                for (var j = 0; j < Col; j++) {
                    tempValues += Body[i, j] + " ";
                }

                tempValues += "\n";
            }
            
            return tempValues;
        }
        
        public List<double> GetAsList() {
            var tempValues = new List<double>();

            for (var i = 0; i < Row; i++) 
                for (var j = 0; j < Col; j++)
                    tempValues.Add(Body[i,j]);
            
            return tempValues;
        }
        
        public void SetValues(string value, int x, int y) {
            Body[x, y] = double.Parse(value, CultureInfo.InvariantCulture);
        }
    }
}