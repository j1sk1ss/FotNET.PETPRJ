using FotNET.NETWORK.MATH.LOSS_FUNCTION.REGULARIZATION;
using FotNET.NETWORK.MATH.OBJECTS;

namespace FotNET.NETWORK.MATH.LOSS_FUNCTION.CELl;

public class CeLl : LossFunction {
    public CeLl(Regularization regularization) =>
        Regularization = regularization;
        
    public CeLl() =>
        Regularization = null!;
    
    private Regularization Regularization { get; }
    
    protected override double Calculate(Tensor expected, Tensor predicted, int channel, int x, int y) => 
        Math.Abs(expected.Channels[channel].Body[x, y] *
                 Math.Log(predicted.Channels[channel].Body[x, y]) +
                 (1 - expected.Channels[channel].Body[x, y]) *
                 Math.Log(1 - predicted.Channels[channel].Body[x, y])) 
        + Regularization.GetRegularization() / expected.Flatten().Count;

    protected override double Derivation(Tensor expected, Tensor predicted, int channel, int x, int y) =>
        (predicted.Channels[channel].Body[x, y] - expected.Channels[channel].Body[x, y]) /
        (predicted.Channels[channel].Body[x, y] * (1 - predicted.Channels[channel].Body[x, y])) 
        + Regularization.GetRegularization() / expected.Flatten().Count;
}