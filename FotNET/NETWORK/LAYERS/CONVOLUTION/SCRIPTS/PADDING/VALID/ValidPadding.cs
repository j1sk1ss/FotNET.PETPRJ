using FotNET.NETWORK.MATH.OBJECTS;

namespace FotNET.NETWORK.LAYERS.CONVOLUTION.SCRIPTS.PADDING.VALID;

public class ValidPadding : Padding {
    protected override Matrix GetPadding(Matrix matrix) => matrix;
}