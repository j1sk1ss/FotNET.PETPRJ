using FotNET.NETWORK.MATH.OBJECTS;

namespace FotNET.NETWORK.LAYERS.CONVOLUTION.SCRIPTS.PADDING {
    public abstract class Padding {
        public abstract Matrix GetPadding(Matrix matrix);
        
        public Tensor GetPadding(Tensor tensor) {
            var newTensor = new Tensor(new List<Matrix>());
            for (var i = 0; i < tensor.Channels.Count; i++)
                newTensor.Channels.Add(new Matrix(0, 0));
            
            Parallel.For(0, tensor.Channels.Count, i => {
                newTensor.Channels[i] = GetPadding(tensor.Channels[i]);
            });

            return newTensor;
        }
    }
}
