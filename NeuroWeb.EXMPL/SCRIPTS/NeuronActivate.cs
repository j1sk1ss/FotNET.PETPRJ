using NeuroWeb.EXMPL.OBJECTS;
using NeuroWeb.EXMPL.OBJECTS.CONVOLUTION;

namespace NeuroWeb.EXMPL.SCRIPTS {
    public static class NeuronActivate {
        private static double Activation(double value) => value switch {
            < 0 => value * .00001d,
            _   => value
        };
        
        public static double[] Activation(double[] value) {
            for (var i = 0; i < value.Length; i++)
                value[i] = Activation(value[i]);
            
            return value;
        }

        private static Matrix Activation(Matrix matrix) {
            for (var i = 0; i < matrix.Body.GetLength(0); i++)
                for (var j = 0; j < matrix.Body.GetLength(1); j++)
                    matrix.Body[i, j] = Activation(matrix.Body[i, j]);
            return matrix;
        }
        
        public static Tensor Activation(Tensor tensor) {
            for (var i = 0; i < tensor.Channels.Count; i++)
                tensor.Channels[i] = Activation(tensor.Channels[i]);
            
            return tensor;
        }
        
        public static double GetDerivative(double value) => value < 0 ? .00001d : value;

        public static Tensor GetDerivative(Tensor tensor) {
            for (var channels = 0; channels < tensor.Channels.Count; channels++)
                for (var x = 0; x < tensor.Channels[channels].Body.GetLength(0); x++)
                    for (var y = 0; y < tensor.Channels[channels].Body.GetLength(1); y++)  
                        tensor.Channels[channels].Body[x, y] = GetDerivative(tensor.Channels[channels].Body[x, y]); 
            return tensor;
        }
    }
}