using FotNET.NETWORK.MATH.OBJECTS;

namespace FotNET.NETWORK.MATH.LOSS_FUNCTION.MBE;

public class Mbe : LossFunction {
    protected override double Calculate(Tensor expected, Tensor predicted, int channel, int x, int y) => 
        expected.Channels[channel].Body[x, y] - predicted.Channels[channel].Body[x, y];

    protected override double Derivation(Tensor expected, Tensor predicted, int channel, int x, int y) =>
        predicted.Channels[channel].Body[x, y] - expected.Channels[channel].Body[x, y];
}