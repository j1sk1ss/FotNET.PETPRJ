using FotNET.NETWORK.OBJECTS;
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

       protected abstract double Derivation(double value);       
       
       public Tensor Derivation(Tensor tensor) {
           foreach (var channel in tensor.Channels)
               for (var x = 0; x < channel.Rows; x++)
                   for (var y = 0; y < channel.Columns; y++)
                       channel.Body[x, y] = Derivation(channel.Body[x, y]);
   
           return tensor;
       }
   } 
}
