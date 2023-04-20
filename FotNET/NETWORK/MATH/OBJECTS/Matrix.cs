﻿namespace FotNET.NETWORK.MATH.OBJECTS {
    public class Matrix {
        /// <summary>
        /// Matrix object for working with 2D arrays
        /// </summary>
        /// <param name="body"> 2D array </param>
        public Matrix(double[,] body) {
            Rows    = body.GetLength(0);
            Columns = body.GetLength(1);
            Body    = body;
        }

        /// <summary>
        /// Matrix object for working with 2D arrays
        /// </summary>
        /// <param name="body"> 1D array </param>
        public Matrix(IReadOnlyList<double> body) {
            Rows    = body.Count;
            Columns = 1;

            Body = new double[Rows, Columns];
            for (var i = 0; i < Rows; i++)
                Body[i, 0] = body[i];
        }
        
        /// <summary>
        /// Matrix object for working with 2D arrays
        /// </summary>
        /// <param name="rows"> Rows of matrix </param>
        /// <param name="columns"> Columns of matrix </param>
        public Matrix(int rows, int columns) {
            Rows    = rows;
            Columns = columns;
            Body    = new double[rows, columns];
        }

        /// <summary>
        /// Matrix object for working with 2D arrays
        /// </summary>
        /// <param name="data"> String with matrix data </param>
        public Matrix(string data) {
            var rows = data.Split("\n", StringSplitOptions.RemoveEmptyEntries);

            Rows    = rows.Length;
            Columns = rows[0].Split(" ", StringSplitOptions.RemoveEmptyEntries).Length;
            
            Body = new double[Rows, Columns];
            for (var x = 0; x < Rows; x++) {
                var elements = rows[x].Split(" ", StringSplitOptions.RemoveEmptyEntries);
                for (var y = 0; y < Columns; y++)
                    if (double.TryParse(elements[y], out var db)) Body[x, y] = db;
                    else Body[x, y] = 0;
            }
        }
        
        public int Rows { get; }
        public int Columns { get; }
        public double[,] Body { get; }

        /// <summary>
        /// Transpose matrix
        /// </summary>
        /// <returns> Transposed matrix </returns>
        public Matrix Transpose() {
            var temp = new double[Columns, Rows];
            
            for (var i = 0; i < Rows; i++) 
                for (var j = 0; j < Columns; j++) 
                    temp[j, i] = Body[i, j];
        
            return new Matrix(temp);
        }

        /// <summary>
        /// Flip matrix or rotate it by 180 degrees 
        /// </summary>
        /// <returns> Flipped matrix </returns>
        public Matrix Flip() {
            var rotatedMatrix = new Matrix(new double[Rows, Columns]);

            for (var i = 0; i < rotatedMatrix.Rows; i++) 
                for (var j = 0; j < rotatedMatrix.Columns; j++) 
                    rotatedMatrix.Body[j, i] = Body[Rows - j - 1, Columns - i - 1];
        
            return rotatedMatrix;
        }

        public double Average() => GetAsList().Average();

        public static Vector operator *(Vector vector, Matrix matrix) {
            var endVector = new Vector(matrix.Rows);

            Parallel.For(0, matrix.Rows, i => {
                double tmp = 0;
                for (var y = 0; y < matrix.Columns; ++y)
                    tmp += matrix.Body[i, y] * vector[y];

                endVector[i] = tmp;
            });

            return endVector;
        }

        public static Matrix operator +(Matrix matrix1, Matrix matrix2) {
            var endMatrix = new Matrix(new double[matrix1.Rows, matrix1.Columns]);

            for (var i = 0; i < matrix1.Rows; i++)
                for (var j = 0; j < matrix1.Columns; j++)
                    endMatrix.Body[i, j] = matrix1.Body[i, j] + matrix2.Body[i, j];

            return endMatrix;
        }

        public static Matrix operator *(Matrix matrix1, Matrix matrix2) {
            var endMatrix = new Matrix(new double[matrix1.Rows, matrix1.Columns]);

            for (var i = 0; i < matrix1.Rows; i++)
                for (var j = 0; j < matrix1.Columns; j++)
                    endMatrix.Body[i, j] = matrix1.Body[i, j] * matrix2.Body[i, j];

            return endMatrix;
        }

        public static Matrix Multiply(Matrix firstMatrix, Matrix secondMatrix) {
            var endMatrix = new Matrix(firstMatrix.Rows, secondMatrix.Columns);

            Parallel.For(0, endMatrix.Rows, i => {
                for (var j = 0; j < endMatrix.Columns; j++) 
                    for (var k = 0; k < secondMatrix.Rows; k++) 
                        endMatrix.Body[i, j] += firstMatrix.Body[i, k] * secondMatrix.Body[k, j];
            });
            
            return endMatrix;
        }
        
        public static Matrix operator -(Matrix matrix1, Matrix matrix2) {
            var endMatrix = new Matrix(new double[matrix1.Rows, matrix1.Columns]);

            for (var i = 0; i < matrix1.Rows; i++)
                for (var j = 0; j < matrix1.Columns; j++)
                    endMatrix.Body[i, j] = matrix1.Body[i, j] - matrix2.Body[i, j];

            return endMatrix;
        }

        public static Matrix operator -(Matrix matrix1, double value) {
            var endMatrix = new Matrix(new double[matrix1.Rows, matrix1.Columns]);

            for (var i = 0; i < matrix1.Rows; i++)
                for (var j = 0; j < matrix1.Columns; j++)
                        endMatrix.Body[i, j] = matrix1.Body[i, j] - value;

            return endMatrix;
        }

        public static Matrix operator *(Matrix matrix1, double value) {
            var endMatrix = new Matrix(new double[matrix1.Rows, matrix1.Columns]);

            for (var i = 0; i < matrix1.Rows; i++)
                for (var j = 0; j < matrix1.Columns; j++)
                        endMatrix.Body[i, j] = matrix1.Body[i, j] * value;

            return endMatrix;
        }

        public static Matrix operator +(Matrix matrix1, double value) {
            var endMatrix = new Matrix(new double[matrix1.Rows, matrix1.Columns]);

            for (var i = 0; i < matrix1.Rows; i++)
                for (var j = 0; j < matrix1.Columns; j++)
                    endMatrix.Body[i, j] = matrix1.Body[i, j] + value;

            return endMatrix;
        }
        
        public double Sum() {
            var sum = 0d;
            
            for (var i = 0; i < Rows; i++)
                for (var j = 0; j < Columns; j++)
                    sum += Body[i, j];
            
            return sum;
        }

        /// <summary>
        /// Create sub-matrix
        /// </summary>
        /// <param name="x1"> First x coordinate </param>
        /// <param name="y1"> First y coordinate </param>
        /// <param name="x2"> Second x coordinate </param>
        /// <param name="y2"> Second y coordinate </param>
        /// <returns> Sub-matrix </returns>
        public Matrix GetSubMatrix(int x1, int y1, int x2, int y2) {
            var subMatrix = new Matrix(x2 - x1, y2 - y1);

            for (var i = x1; i < x2; i++) 
                for (var j = y1; j < y2; j++) 
                    subMatrix.Body[i - x1, j - y1] = Body[i, j];
        
            return subMatrix;
        }

        public string GetValues() {
            var tempValues = "";

            for (var i = 0; i < Rows; i++)
                for (var j = 0; j < Columns; j++)
                    tempValues += Body[i, j] + " ";

            return tempValues;
        }

        public string Print() {
            var tempValues = "";

            for (var i = 0; i < Rows; i++) {
                for (var j = 0; j < Columns; j++) 
                    tempValues += Body[i, j] + " ";
                
                tempValues += "\n";
            }

            return tempValues;
        }

        public IEnumerable<double> GetAsList() {
            var tempValues = new List<double>();

            for (var i = 0; i < Rows; i++)
                for (var j = 0; j < Columns; j++)
                    tempValues.Add(Body[i, j]);

            return tempValues;
        }
    }
}