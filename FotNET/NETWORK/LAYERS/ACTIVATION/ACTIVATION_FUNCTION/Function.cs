using FotNET.NETWORK.MATH.OBJECTS;

namespace FotNET.NETWORK.LAYERS.ACTIVATION.ACTIVATION_FUNCTION {
   public abstract class Function {
       protected abstract double Activate(double value);
       
       public Tensor Activate(Tensor tensor) {
           Parallel.For(0, tensor.Channels.Count, channel => {
               for (var x = 0; x < tensor.Channels[channel].Rows; x++)
                   for (var y = 0; y < tensor.Channels[channel].Columns; y++)
                       tensor.Channels[channel].Body[x, y] = Activate(tensor.Channels[channel].Body[x, y]);
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
       
       protected abstract double Derivation(double value, double referenceValue);       
       
       public Tensor Derivation(Tensor tensor, Tensor referenceTensor) {
           Parallel.For(0, tensor.Channels.Count, channel => {
               for (var x = 0; x < tensor.Channels[channel].Rows; x++)
                   for (var y = 0; y < tensor.Channels[channel].Columns; y++)
                       tensor.Channels[channel].Body[x, y] = Derivation(tensor.Channels[channel].Body[x, y], 
                           referenceTensor.Channels[channel].Body[x,y]);
           });

           return tensor;
       }
       
       public Matrix Derivation(Matrix matrix, Matrix referenceMatrix) {
           for (var x = 0; x < matrix.Rows; x++)
               for (var y = 0; y < matrix.Columns; y++)
                   matrix.Body[x, y] = Derivation(matrix.Body[x, y], referenceMatrix.Body[x, y]);
           
           return matrix;
       }
       
       public double[] Derivation(double[] array, double[] referenceArray) {
           for (var i = 0; i < array.Length; i++)
               array[i] = Derivation(array[i], referenceArray[i]);

           return array;
       }
   } 
}
