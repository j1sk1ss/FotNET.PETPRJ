using FotNET.NETWORK.OBJECTS.MATH_OBJECTS;

namespace FotNET.NETWORK.MATH.Initialization;

public interface IWeightsInitialization {
    public Matrix Initialize(Matrix matrix);
}