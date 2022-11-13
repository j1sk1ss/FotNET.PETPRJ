namespace NeuroWeb.EXMPL.OBJECTS
{
    public class Vector
    {
        public Vector(double[] array) {
            Body = array;
            Size = array.Length;
        }
        public double[] Body { get; set; }
        public int Size { get; set; }
        public double this[int key] {
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
    }
}