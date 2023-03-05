using NeuroWeb.EXMPL.OBJECTS.MATH;
using NeuroWeb.EXMPL.OBJECTS.NETWORK;

namespace NeuroWeb.EXMPL.SCRIPTS.MATH {
    public static class NeuronActivate {
        private static double LeakyReLu(double value) => value switch {
            <= 0 => value * .01d,
            _    => 1d + .01d * (value - 1d)
        };
        
        public static double[] LeakyReLu(double[] value) {
            for (var i = 0; i < value.Length; i++)
                value[i] = LeakyReLu(value[i]);
            
            return value;
        }

        private static Matrix LeakyReLu(Matrix matrix) {
            for (var i = 0; i < matrix.Body.GetLength(0); i++)
                for (var j = 0; j < matrix.Body.GetLength(1); j++)
                    matrix.Body[i, j] = LeakyReLu(matrix.Body[i, j]);
            return matrix;
        }
        
        public static Tensor LeakyReLu(Tensor tensor) {
            for (var i = 0; i < tensor.Channels.Count; i++)
                tensor.Channels[i] = LeakyReLu(tensor.Channels[i]);
            
            return tensor;
        }
        
        public static double GetDerivative(double value) => value is < 0 or > 1 ? .01d : value;

        public static double[] GetDerivative(double[] values) {
            for (var i = 0; i < values.Length; i++) 
                values[i] = GetDerivative(values[i]); 
            return values;
        }
        
        public static Tensor GetDerivative(Tensor tensor) {
            for (var channels = 0; channels < tensor.Channels.Count; channels++)
                for (var x = 0; x < tensor.Channels[channels].Body.GetLength(0); x++)
                    for (var y = 0; y < tensor.Channels[channels].Body.GetLength(1); y++)  
                        tensor.Channels[channels].Body[x, y] = GetDerivative(tensor.Channels[channels].Body[x, y]); 
            return tensor;
        }
    }
}