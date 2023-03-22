using FotNET.NETWORK.OBJECTS.MATH_OBJECTS;

namespace FotNET.NETWORK.LAYERS.ACTIVATION.ACTIVATION_FUNCTION {
   public abstract class Function {
       protected abstract double Activate(double value);

       public Tensor Activate(Tensor tensor) {
           foreach (var channel in tensor.Channels)
               for (var x = 0; x < channel.Rows; x++)
                   for (var y = 0; y < channel.Columns; y++)
                       channel.Body[x, y] = Activate(channel.Body[x, y]);
   
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
       
       protected abstract double Derivation(double value);       
       
       public Tensor Derivation(Tensor tensor) {
           foreach (var channel in tensor.Channels)
               for (var x = 0; x < channel.Rows; x++)
                   for (var y = 0; y < channel.Columns; y++)
                       channel.Body[x, y] = Derivation(channel.Body[x, y]);
   
           return tensor;
       }
       
       public Matrix Derivation(Matrix matrix) {
           for (var x = 0; x < matrix.Rows; x++)
               for (var y = 0; y < matrix.Columns; y++)
                   matrix.Body[x, y] = Derivation(matrix.Body[x, y]);
           
           return matrix;
       }
       
       public double[] Derivation(double[] array) {
           for (var i = 0; i < array.Length; i++)
               array[i] = Derivation(array[i]);

           return array;
       }
   } 
}
