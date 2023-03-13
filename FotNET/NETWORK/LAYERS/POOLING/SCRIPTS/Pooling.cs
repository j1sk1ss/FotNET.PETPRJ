using FotNET.NETWORK.OBJECTS;

namespace FotNET.NETWORK.LAYERS.POOLING.SCRIPTS {
    public abstract class Pooling {
        public Tensor BackPool(Tensor picture, Tensor previousTensor, int poolSize) {
            for (var i = 0; i < picture.Channels.Count; i++) 
                picture.Channels[i] = BackPool(picture.Channels[i], previousTensor.Channels[i], poolSize);
            
            return picture;
        }

        public Tensor Pool(Tensor picture, int poolSize) {
            for (var i = 0; i < picture.Channels.Count; i++) 
                picture.Channels[i] = Pool(picture.Channels[i], poolSize);
            
            return picture;
        }

        protected abstract Matrix BackPool(Matrix matrix, Matrix referenceMatrix, int poolSize);
        
        protected abstract Matrix Pool(Matrix matrix, int poolSize);
    }
}