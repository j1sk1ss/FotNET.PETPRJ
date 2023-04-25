namespace FotNET.NETWORK.MATH.OBJECTS {
    public class Vector {
        /// <summary>
        /// Vector object for working with arrays
        /// </summary>
        /// <param name="array"> 1D array </param>
        public Vector(double[] array) {
            Body = array;
            Size = array.Length;
        }

        /// <summary>
        /// Vector object for working with arrays
        /// </summary>
        /// <param name="size"> Size of array </param>
        public Vector(int size) {
            Body = new double[size];
            Size = size;
        }
        
        private double[] Body { get; }
        public int Size { get; }

        public double this[int key] {
            get => Body[key];
            set => SetElement(key, value);
        }

        private void SetElement(int index, double value) => Body[index] = value;

        public static Vector operator +(Vector firstVector, Vector secondVector) {
            for (var i = 0; i < firstVector.Size; i++) 
                firstVector[i] += secondVector[i];
        
            return firstVector;
        }

        public static Vector operator -(Vector firstVector, double value) {
            for (var i = 0; i < firstVector.Size; i++) 
                firstVector[i] -= value;
        
            return firstVector;
        }
        
        public static Vector operator +(Vector firstVector, double value) {
            for (var i = 0; i < firstVector.Size; i++) 
                firstVector[i] += value;
        
            return firstVector;
        }

        public static Vector operator *(Vector firstVector, Vector secondVector) {
            for (var i = 0; i < firstVector.Size; i++) 
                firstVector[i] *= secondVector[i];
        
            return firstVector;
        }

        public static Vector operator *(Vector firstVector, double value) {
            for (var i = 0; i < firstVector.Size; i++) 
                firstVector[i] *= value;
            
            return firstVector;
        }

        public static Vector GenerateGaussianNoise(int size, double mean = 0, double stdDev = 1) {
            var noise = new double[size];

            for (var i = 0; i < size; i += 2) {
                var u1 = new Random().NextDouble();
                var u2 = new Random().NextDouble();

                var z1 = Math.Sqrt(-2 * Math.Log(u1)) * Math.Cos(2 * Math.PI * u2);
                var z2 = Math.Sqrt(-2 * Math.Log(u1)) * Math.Sin(2 * Math.PI * u2);

                noise[i] = mean + stdDev * z1;
                
                if (i + 1 < size) 
                    noise[i + 1] = mean + stdDev * z2;
            }

            return new Vector(noise);
        }

        /// <summary>
        /// Transform vector into matrix
        /// </summary>
        /// <param name="x"> X size of matrix </param>
        /// <param name="y"> Y size of matrix </param>
        /// <returns> Matrix </returns>
        public Matrix AsMatrix(int x, int y) {
            var matrix = new Matrix(x, y);
            var position = 0;
            
            for (var i = 0; i < x; i++)
            for (var j = 0; j < y; j++) {
                if (Size <= position) return null!;
                matrix.Body[i, j] = Body[position++];
            }

            return matrix;
        }
        
        private Matrix AsMatrix(int x, int y, int pos) {
            var matrix = new Matrix(x, y);
            var position = pos;
            
            for (var i = 0; i < x; i++)
                for (var j = 0; j < y; j++) {
                    if (Size <= position) return null!;
                    matrix.Body[i, j] = Body[position++];
                }

            return matrix;
        }
        
        /// <summary>
        /// Method for converting vector to tensor
        /// </summary>
        /// <param name="x"> X size of tensor </param>
        /// <param name="y"> Y size of tensor </param>
        /// <param name="channels"> Depth of tensor </param>
        /// <returns> Tensor from vector </returns>
        public Tensor AsTensor(int x, int y, int channels) {
            var tensor = new Tensor(new List<Matrix>());

            for (var k = 0; k < channels; k++) 
                tensor.Channels.Add(AsMatrix(x, y, x * y * k));

            return tensor;
        }

        public string Print() =>
            Body.Aggregate("", (current, t) => current + " " + t);
    }
}