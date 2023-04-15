using FotNET.NETWORK.MATH.OBJECTS;

namespace FotNET.DATA.DATA_OBJECTS;

public interface IData {
    public Tensor GetRight();
    public Tensor AsTensor();
    
    public enum Type {
        Array,
        Image
    }
}