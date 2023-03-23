using FotNET.NETWORK.OBJECTS.MATH_OBJECTS;

namespace FotNET.NETWORK.LAYERS.RECURRENT.RECURRENCY_TYPE;

public interface IRecurrent {
    public Tensor GetNextLayer(RecurrentLayer layer, Tensor tensor);
    public Tensor BackPropagate(RecurrentLayer layer, Tensor error, double learningRate);
}