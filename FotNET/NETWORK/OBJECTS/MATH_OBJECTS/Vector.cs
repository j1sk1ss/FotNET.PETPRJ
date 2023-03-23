namespace FotNET.NETWORK.OBJECTS.MATH_OBJECTS {
    public class Vector {
        public Vector(double[] array) {
            Body = array;
            Size = array.Length;
        }

        public double[] Body { get; }
        private int Size { get; }

        private double this[int key] {
            get => Body[key];
            set => SetElement(key, value);
        }

        private void SetElement(int index, double value) => Body[index] = value;

        public static double[] operator +(Vector vector1, Vector vector2) {
            try {
                for (var i = 0; i < vector1.Size; i++) 
                    vector1[i] += vector2[i];
            
                return vector1.Body;
            }
            catch (Exception ex) {
                Console.WriteLine("Код ошибки: 1v\n" + ex);
                return null!;
            }
        }

        public static Vector operator -(Vector vector1, double value) {
            for (var i = 0; i < vector1.Size; i++) 
                vector1[i] -= value;
        
            return vector1;
        }
        
        public static Vector operator +(Vector vector1, double value) {
            for (var i = 0; i < vector1.Size; i++) 
                vector1[i] += value;
        
            return vector1;
        }

        public static Vector operator *(Vector vector1, Vector vector2) {
            try {
                for (var i = 0; i < vector1.Size; i++) 
                    vector1[i] *= vector2[i];
            
                return vector1;
            }
            catch (Exception) {
                Console.WriteLine("Код ошибки: 2v");
                return null!;
            }
        }

        public static Vector operator *(Vector vector1, double value) {
            for (var i = 0; i < vector1.Size; i++) 
                vector1[i] *= value;
            
            return vector1;
        }

        public Matrix AsMatrix(int x, int y, int pos) {
            var matrix = new Matrix(x, y);
            var position = pos;
            for (var i = 0; i < x; i++)
                for (var j = 0; j < y; j++) {
                    if (Size <= position) return null!;
                    matrix.Body[i, j] = Body[position++];
            }

            return matrix;
        }
        
        public Tensor AsTensor(int x, int y, int channels) {
            try {
                var tensor = new Tensor(new List<Matrix>());
                var position = 0;

                for (var k = 0; k < channels; k++) 
                    tensor.Channels.Add(AsMatrix(x,y, position));
                
                return tensor;
            }
            catch (Exception) {
                Console.WriteLine("Код ошибки: 3v");
                return null!;
            }
        }

        public string Print() =>
            Body.Aggregate("", (current, t) => current + " " + t);
    }
}