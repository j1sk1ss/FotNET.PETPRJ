using FotNET.NETWORK.MATH.OBJECTS;

namespace FotNET.NETWORK.MATH.Initialization;

public interface IWeightsInitialization {
    public Matrix Initialize(Matrix matrix);
}