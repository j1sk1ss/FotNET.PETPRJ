using FotNET.NETWORK.OBJECTS;

namespace FotNET.NETWORK.ACTIVATION {
   public abstract class Function {
       protected abstract double Activate(double value);
   
       private Matrix Activate(Matrix matrix) {
           for (var i = 0; i < matrix.Body.GetLength(0); i++)
               for (var j = 0; j < matrix.Body.GetLength(1); j++)
                   matrix.Body[i, j] = Activate(matrix.Body[i, j]);
           return matrix;
       }
   
       public Tensor Activate(Tensor tensor) {
           for (var i = 0; i < tensor.Channels.Count; i++)
               tensor.Channels[i] = Activate(tensor.Channels[i]);
   
           return tensor;
       }
   
       public Tensor Derivation(Tensor tensor) {
           foreach (var channel in tensor.Channels)
               for (var x = 0; x < channel.Body.GetLength(0); x++)
                   for (var y = 0; y < channel.Body.GetLength(1); y++)
                       channel.Body[x, y] = Derivation(channel.Body[x, y]);
   
           return tensor;
       }
   
       public abstract double Derivation(double value);
   } 
}
