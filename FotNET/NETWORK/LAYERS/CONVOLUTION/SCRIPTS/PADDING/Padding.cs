using FotNET.NETWORK.OBJECTS.MATH_OBJECTS;

namespace FotNET.NETWORK.LAYERS.CONVOLUTION.SCRIPTS.PADDING {
    public abstract class Padding {
        protected abstract Matrix GetPadding(Matrix matrix, int paddingSize);
        
        public Tensor GetPadding(Tensor tensor, int paddingSize) {
            var newTensor = new Tensor(new List<Matrix>());
            for (var i = 0; i < tensor.Channels.Count; i++)
                newTensor.Channels.Add(new Matrix(0, 0));
            
            Parallel.For(0, tensor.Channels.Count, i => {
                newTensor.Channels[i] = GetPadding(tensor.Channels[i],  paddingSize);
            });

            return newTensor;
        }
    }
}
