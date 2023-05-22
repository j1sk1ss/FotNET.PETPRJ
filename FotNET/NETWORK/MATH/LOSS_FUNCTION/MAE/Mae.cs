using FotNET.NETWORK.MATH.LOSS_FUNCTION.REGULARIZATION;
using FotNET.NETWORK.MATH.LOSS_FUNCTION.REGULARIZATION.NO_REGULARIZATION;
using FotNET.NETWORK.MATH.OBJECTS;

namespace FotNET.NETWORK.MATH.LOSS_FUNCTION.MAE;

/// <summary>
/// Mean absolute error (MAE)
/// </summary>
public class Mae : LossFunction {
    public Mae(Regularization regularization) =>
        Regularization = regularization;
        
    public Mae() =>
        Regularization = new NoRegularization();
    
    private Regularization Regularization { get; }
    
    protected override double Calculate(Tensor expected, Tensor predicted, int channel, int x, int y) => 
        Math.Abs(expected.Channels[channel].Body[x, y] - predicted.Channels[channel].Body[x, y]) 
        + Regularization.GetRegularization() / expected.Flatten().Count;
    
    protected override double Derivation(Tensor expected, Tensor predicted, int channel, int x, int y) => 
        predicted.Channels[channel].Body[x, y] - expected.Channels[channel].Body[x, y] 
        + Regularization.GetRegularization() / expected.Flatten().Count;
}