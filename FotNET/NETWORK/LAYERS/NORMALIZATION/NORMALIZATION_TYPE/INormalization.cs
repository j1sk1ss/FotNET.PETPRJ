using FotNET.NETWORK.OBJECTS.MATH_OBJECTS;

namespace FotNET.NETWORK.LAYERS.NORMALIZATION.NORMALIZATION_TYPE;

public interface INormalization {
    public Tensor Normalize(Tensor tensor);
}