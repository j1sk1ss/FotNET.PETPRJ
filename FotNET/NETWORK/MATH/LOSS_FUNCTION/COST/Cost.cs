using FotNET.NETWORK.MATH.LOSS_FUNCTION.REGULARIZATION;
using FotNET.NETWORK.MATH.OBJECTS;

namespace FotNET.NETWORK.MATH.LOSS_FUNCTION.COST;

/// <summary>
/// Cost Loss function
/// </summary>
public class Cost : LossFunction {
    public Cost(Regularization regularization) =>
        Regularization = regularization;
        
    public Cost() =>
        Regularization = null!;
    
    private Regularization Regularization { get; }
    
    protected override double Calculate(Tensor expected, Tensor predicted, int channel, int x, int y) =>
        Math.Pow((expected.Channels[channel].Body[x, y] - predicted.Channels[channel].Body[x, y]) / 2, 2) 
        + Regularization.GetRegularization() / expected.Flatten().Count;

    protected override double Derivation(Tensor expected, Tensor predicted, int channel, int x, int y) => 
        2 * (predicted.Channels[channel].Body[x, y] - expected.Channels[channel].Body[x, y]) 
        + Regularization.GetRegularization() / expected.Flatten().Count;
}