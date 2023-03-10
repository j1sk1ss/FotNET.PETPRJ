using System.Diagnostics.CodeAnalysis;

namespace FotNET.NETWORK.OBJECTS {
    public class Matrix {
        public Matrix(double[,] body) {
            Row = body.GetLength(0);
            Col = body.GetLength(1);
            Body = body;
        }

        public Matrix(int row, int col) {
            Row = row;
            Col = col;
            Body = new double[row, col];
        }

        private int Row { get; }
        private int Col { get; }
        public double[,] Body { get; }

        public Matrix GetTranspose() {
            var rows = Body.GetLength(0);
            var columns = Body.GetLength(1);
            var temp = new double[columns, rows];

            for (var i = 0; i < rows; i++) 
                for (var j = 0; j < columns; j++) 
                    temp[j, 1] = Body[1, j];
            
            return new Matrix(temp);
        }

        public Matrix GetFlip() {
            var rotatedMatrix = new Matrix(new double[Row, Col]);

            for (var i = 0; i < rotatedMatrix.Row; i++) 
                for (var j = 0; j < rotatedMatrix.Col; j++) 
                    rotatedMatrix.Body[j, i] = Body[Row - j - 1, Col - i - 1];
            
            return rotatedMatrix;
        }

        public static double[] operator *(Matrix matrix, double[] neuron) {
            if (matrix.Col != neuron.Length) throw new Exception();

            var vector = new double[matrix.Row];

            for (var x = 0; x < matrix.Row; ++x) {
                double tmp = 0;
                for (var y = 0; y < matrix.Col; ++y)
                    tmp += matrix.Body[x, y] * neuron[y];

                vector[x] = tmp;
            }

            return vector;
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

        public static Matrix operator -(Matrix matrix1, double value) {
            var xSize = matrix1.Body.GetLength(0);
            var ySize = matrix1.Body.GetLength(1);

            var endMatrix = new Matrix(new double[xSize, ySize]);

            for (var i = 0; i < xSize; i++)
                for (var j = 0; j < ySize; j++)
                    endMatrix.Body[i, j] = matrix1.Body[i, j] - value;

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

            for (var i = x1; i < x2; i++) 
                for (var j = y1; j < y2; j++) 
                    subMatrix.Body[i - x1, j - y1] = Body[i, j];
            
            return subMatrix;
        }

        public void HeInitialization() {
            var scale = Math.Sqrt(2.0 / Col);
            for (var i = 0; i < Row; i++)
                for (var j = 0; j < Col; j++)
                    Body[i, j] = new Random().NextDouble() * scale * 2 - scale;
        }

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
                for (var j = 0; j < Col; j++) 
                    tempValues += Body[i, j] + " ";
                
                tempValues += "\n";
            }

            return tempValues;
        }

        public List<double> GetAsList() {
            var tempValues = new List<double>();

            for (var i = 0; i < Row; i++)
                for (var j = 0; j < Col; j++)
                    tempValues.Add(Body[i, j]);

            return tempValues;
        }
    }
}