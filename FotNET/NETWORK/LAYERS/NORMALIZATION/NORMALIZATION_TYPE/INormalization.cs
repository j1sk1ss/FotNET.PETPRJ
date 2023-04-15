using FotNET.NETWORK.MATH.OBJECTS;

namespace FotNET.NETWORK.LAYERS.NORMALIZATION.NORMALIZATION_TYPE;

public interface INormalization {
    public Tensor Normalize(Tensor tensor);
}