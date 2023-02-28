using System.Linq;

namespace NeuroWeb.EXMPL.OBJECTS {
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
        private void SetElement(int index, double value) {
            Body[index] = value;
        }
        public static double[] operator +(Vector vector1, Vector vector2) {
            for (var i = 0; i < vector1.Size; i++) {
                vector1[i] += vector2[i];
            }
            return vector1.Body;
        }

        public static Vector operator -(Vector vector1, double value) {
            for (var i = 0; i < vector1.Size; i++) {
                vector1[i] -= value;
            }
            return vector1;
        }
        
        public static Vector operator *(Vector vector1, double value) {
            for (var i = 0; i < vector1.Size; i++) {
                vector1[i] *= value;
            }
            return vector1;
        }
        
        public Matrix AsMatrix(int x, int y) {
            var matrix   = new Matrix(x, y);
            var position = 0;
            
            for (var i = 0; i < x; i++) {
                for (var j = 0; j < y; j++) {
                    if (Body.Length <= position) return null;
                    matrix.Body[i, j] = Body[position++];
                }
            }

            return matrix;
        }

        public string Print() =>
            Body.Aggregate("", (current, t) => current + (" " + t));
    }
}