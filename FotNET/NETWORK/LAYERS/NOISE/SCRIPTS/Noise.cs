using FotNET.NETWORK.MATH.OBJECTS;

namespace FotNET.NETWORK.LAYERS.NOISE.SCRIPTS;

public interface INoise {
    public Vector GenerateNoise(int size);
}