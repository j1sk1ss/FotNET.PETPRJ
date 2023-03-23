namespace FotNET.NETWORK.OBJECTS.MATH_OBJECTS {
    public class Matrix {
        public Matrix(double[,] body) {
            Rows    = body.GetLength(0);
            Columns = body.GetLength(1);
            Body    = body;
        }

        public Matrix(double[] body) {
            Rows    = body.Length;
            Columns = 1;

            Body = new double[Rows, Columns];
            for (var i = 0; i < Rows; i++)
                Body[i, 0] = body[i];
        }
        
        public Matrix(int rows, int columns) {
            Rows    = rows;
            Columns = columns;
            Body    = new double[rows, columns];
        }

        public int Rows { get; }
        public int Columns { get; }
        public double[,] Body { get; }

        public Matrix Transpose() {
            try {
                var temp = new double[Columns, Rows];
                for (var i = 0; i < Rows; i++) 
                    for (var j = 0; j < Columns; j++) 
                        temp[j, i] = Body[i, j];
            
                return new Matrix(temp);
            }
            catch (Exception) {
                Console.WriteLine("Код ошибки: 1m");
                return null!;
            }
        }

        public Matrix Flip() {
            try {
                var rotatedMatrix = new Matrix(new double[Rows, Columns]);

                for (var i = 0; i < rotatedMatrix.Rows; i++) 
                    for (var j = 0; j < rotatedMatrix.Columns; j++) 
                        rotatedMatrix.Body[j, i] = Body[Rows - j - 1, Columns - i - 1];
            
                return rotatedMatrix;
            }
            catch (Exception) {
                Console.WriteLine("Код ошибки: 2m");
                return null!;
            }
        }

        public double Average() => GetAsList().Average();

        public static double[] operator *(double[] vector, Matrix matrix) {
            try {
                var endVector = new double[matrix.Rows];

                for (var x = 0; x < matrix.Rows; ++x) {
                    double tmp = 0;
                    for (var y = 0; y < matrix.Columns; ++y)
                        tmp += matrix.Body[x, y] * vector[y];

                    endVector[x] = tmp;
                }

                return endVector;
            }
            catch (Exception) {
                Console.WriteLine("Код ошибки: 3m");
                return null!;
            }
        }

        public static Matrix operator +(Matrix matrix1, Matrix matrix2) {
            try {
                var endMatrix = new Matrix(new double[matrix1.Rows, matrix1.Columns]);

                for (var i = 0; i < matrix1.Rows; i++)
                    for (var j = 0; j < matrix1.Columns; j++)
                        endMatrix.Body[i, j] = matrix1.Body[i, j] + matrix2.Body[i, j];

                return endMatrix;
            }
            catch (Exception ex) {
                Console.WriteLine("Код ошибки: 4m\n" + ex);
                return null!;
            }
        }

        public static Matrix operator *(Matrix matrix1, Matrix matrix2) {
            try {
                var endMatrix = new Matrix(new double[matrix1.Rows, matrix1.Columns]);

                for (var i = 0; i < matrix1.Rows; i++)
                    for (var j = 0; j < matrix1.Columns; j++)
                        endMatrix.Body[i, j] = matrix1.Body[i, j] * matrix2.Body[i, j];

                return endMatrix;
            }
            catch (Exception ex) {
                Console.WriteLine("Код ошибки: 5m\n" + ex);
                return null!;
            }
        }

        public static Matrix Multiply(Matrix firstMatrix, Matrix secondMatrix) {
            var endMatrix = new Matrix(firstMatrix.Rows, secondMatrix.Columns);

            for (var i = 0; i < endMatrix.Rows; i++) 
                for (var j = 0; j < endMatrix.Columns; j++) 
                    for (var k = 0; k < secondMatrix.Rows; k++) 
                        endMatrix.Body[i, j] += firstMatrix.Body[i, k] * secondMatrix.Body[k, j];
            
            return endMatrix;
        }
        
        public static Matrix operator -(Matrix matrix1, Matrix matrix2) {
            try {
                var endMatrix = new Matrix(new double[matrix1.Rows, matrix1.Columns]);

                for (var i = 0; i < matrix1.Rows; i++)
                    for (var j = 0; j < matrix1.Columns; j++)
                        endMatrix.Body[i, j] = matrix1.Body[i, j] - matrix2.Body[i, j];

                return endMatrix;
            }
            catch (Exception ex) {
                Console.WriteLine("Код ошибки: 6m\n" + ex);
                return null!;
            }
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

        public double Sum() {
            var sum = 0d;
            
            for (var i = 0; i < Rows; i++)
                for (var j = 0; j < Columns; j++)
                    sum += Body[i, j];
            
            return sum;
        }

        public Matrix GetSubMatrix(int x1, int y1, int x2, int y2) {
            try {
                var subMatrix = new Matrix(x2 - x1, y2 - y1);

                for (var i = x1; i < x2; i++) 
                    for (var j = y1; j < y2; j++) 
                        subMatrix.Body[i - x1, j - y1] = Body[i, j];
            
                return subMatrix;
            }
            catch (Exception) {
                Console.WriteLine("Код ошибки: 7m");
                return null!;
            }
        }

        public void HeInitialization() {
            var scale = Math.Sqrt(2.0 / Columns);
            for (var i = 0; i < Rows; i++)
                for (var j = 0; j < Columns; j++)
                    Body[i, j] = new Random().NextDouble() * scale * 2 - scale;
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

        public List<double> GetAsList() {
            var tempValues = new List<double>();

            for (var i = 0; i < Rows; i++)
                for (var j = 0; j < Columns; j++)
                    tempValues.Add(Body[i, j]);

            return tempValues;
        }
    }
}