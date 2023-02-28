using NeuroWeb.EXMPL.OBJECTS;
using NeuroWeb.EXMPL.OBJECTS.CONVOLUTION;

namespace NeuroWeb.EXMPL.SCRIPTS {
    public static class NeuronActivate {
        public static double[] Activation(double[] value) {
            for (var i = 0; i < value.Length; i++) 
                switch (value[i]) {
                    case < 0:
                        value[i] *= 0.01d;
                        break;
                    case > 1:
                        value[i] = 1d + .01d * (value[i] - 1d);
                        break;
                }
            
            return value;
        }
        
        public static Matrix Activation(Matrix matrix) {
            for (var i = 0; i < matrix.Body.GetLength(0); i++) 
                for (var j = 0; j < matrix.Body.GetLength(1); j++) 
                    switch (matrix.Body[i,j]) {
                        case < 0:
                            matrix.Body[i,j] *= 0.01d;
                            break;
                        case > 1:
                            matrix.Body[i,j] = 1d + .01d * (matrix.Body[i,j] - 1d);
                            break;
                    }
            
            return matrix;
        }
        
        public static Tensor Activation(Tensor tensor) {
            foreach (var matrix in tensor.Channels) {
               for (var i = 0; i < matrix.Body.GetLength(0); i++) 
                   for (var j = 0; j < matrix.Body.GetLength(1); j++) 
                       switch (matrix.Body[i,j]) {
                           case < 0:
                               matrix.Body[i,j] *= 0.01d;
                               break;
                           case > 1:
                               matrix.Body[i,j] = 1d + .01d * (matrix.Body[i,j] - 1d);
                               break;
                       } 
            }
            
            
            return tensor;
        }
        
        public static double GetDerivative(double value) => value is < 0 or > 1 ? 0.01d : value;
    }
}