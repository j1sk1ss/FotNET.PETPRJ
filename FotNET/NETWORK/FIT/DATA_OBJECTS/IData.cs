using FotNET.NETWORK.OBJECTS;

namespace FotNET.NETWORK.FIT.DATA_OBJECTS;

public interface IData {
    public Tensor GetRight();
    public Tensor AsTensor();
}