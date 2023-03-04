using NeuroWeb.EXMPL.OBJECTS;
using NeuroWeb.EXMPL.OBJECTS.CONVOLUTION;
using System;
using System.ComponentModel;

namespace NeuroWeb.EXMPL.SCRIPTS {
    public static class NeuronActivate {
        public static double[] Activation(double[] value) {
            for (var i = 0; i < value.Length; i++) 
                switch (value[i]) {
                    case < 0:
                        value[i] = 0d;
                        break;
                    //case > 1:
                      //  value[i] = 1d + .01d * (value[i] - 1d);
                        //break;
                }
            
            return value;
        }
        
        public static Matrix Activation(Matrix matrix) {
            for (var i = 0; i < matrix.Body.GetLength(0); i++)
                for (var j = 0; j < matrix.Body.GetLength(1); j++)  
                    switch (matrix.Body[i,j]) {
                        case < 0:
                            matrix.Body[i,j] = 0d;
                            break;
                        //case > 1:
                          //  matrix.Body[i,j] = 1d + .01d * (matrix.Body[i,j] - 1d);
                            //break;
                    } 
            return matrix;
        }
        
        public static Tensor Activation(Tensor tensor) {
            for (var i = 0; i < tensor.Channels.Count; i++)
                tensor.Channels[i] = Activation(tensor.Channels[i]);
            
            return tensor;
        }
        
        public static double GetDerivative(double value) => value < 0 ? .001d : value; //value is < 0 or > 1 ? .001d : 1;

        public static Tensor GetDerivative(Tensor tensor) {
            for (var channels = 0; channels < tensor.Channels.Count; channels++)
                for (var x = 0; x < tensor.Channels[channels].Body.GetLength(0); x++)
                    for (var y = 0; y < tensor.Channels[channels].Body.GetLength(1); y++)  
                        tensor.Channels[channels].Body[x, y] = GetDerivative(tensor.Channels[channels].Body[x, y]); 
            return tensor;
        }
    }
}