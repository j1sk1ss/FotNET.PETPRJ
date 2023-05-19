using FotNET.NETWORK.MATH.OBJECTS;

namespace FotNET.NETWORK.LAYERS.ACTIVATION.ACTIVATION_FUNCTION {
   public abstract class Function {
       protected abstract double Activate(double value);
       
       public Tensor Activate(Tensor tensor) {
           Parallel.For(0, tensor.Shape.Depth, channel => {
                tensor.Channels[channel] = Activate(tensor.Channels[channel]);
           });
   
           return tensor;
       }

       public Matrix Activate(Matrix matrix) {
           for (var x = 0; x < matrix.Rows; x++)
               for (var y = 0; y < matrix.Columns; y++)
                   matrix.Body[x, y] = Activate(matrix.Body[x, y]);
           
           return matrix;
       }

       public double[] Activate(double[] array) {
           for (var i = 0; i < array.Length; i++)
               array[i] = Activate(array[i]);

           return array;
       }
       
       protected abstract double Derivation(double value, double activatedValue);       
       
       public Tensor Derivation(Tensor tensor, Tensor referenceTensor) {
           referenceTensor = referenceTensor.GetSameChannels(tensor);
           Parallel.For(0, tensor.Shape.Depth, channel => {
               tensor.Channels[channel] = Derivation(tensor.Channels[channel], 
                   referenceTensor.Channels[channel]);
           });
           
           return tensor;
       }
       
       public Matrix Derivation(Matrix matrix, Matrix referenceMatrix) {
           for (var x = 0; x < matrix.Rows; x++)
               for (var y = 0; y < matrix.Columns; y++)
                   matrix.Body[x, y] = Derivation(matrix.Body[x, y], referenceMatrix.Body[x, y]);
           
           return matrix;
       }
   } 
}
