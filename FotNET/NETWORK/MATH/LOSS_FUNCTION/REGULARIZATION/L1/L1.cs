namespace FotNET.NETWORK.MATH.LOSS_FUNCTION.REGULARIZATION.L1;

public class L1 : Regularization {
    
    /// <summary>
    /// L1 Regularization
    /// </summary>
    /// <param name="model"> Neural network model </param>
    public L1(Network model) =>
        Model = model;

    private Network Model { get; }
    
    /// <summary>
    /// Gets value for regularization 
    /// </summary>
    /// <returns> Regularization value </returns>
    public override double GetRegularization() {
        var values = Model.GetWeights().Split(" ", StringSplitOptions.RemoveEmptyEntries);
        var sum = 0d;

        foreach (var value in values)
            if (double.TryParse(value, out var cur)) sum += Math.Abs(cur);
            else sum += 0; 

        return sum;
    }
}