using FotNET.NETWORK.MATH.OBJECTS;

namespace FotNET.NETWORK.MATH.LOSS_FUNCTION.MSE;

/// <summary>
/// Mean square error (MSE)
/// </summary>
public class Mse : LossFunction {
    protected override double GetValue(Tensor expected, Tensor predicted, int channel, int x, int y) =>
        Math.Pow(expected.Channels[channel].Body[x, y] - predicted.Channels[channel].Body[x, y], 2);

    protected override double GetValueForError(Tensor expected, Tensor predicted, int channel, int x, int y) => 
        2 * (predicted.Channels[channel].Body[x, y] - expected.Channels[channel].Body[x, y]);
}