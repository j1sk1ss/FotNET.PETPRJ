namespace FotNET.NETWORK.MATH.LOSS_FUNCTION.REGULARIZATION.L1;

public class L1 : Regularization {
    public L1(Network model) =>
        Model = model;

    private Network Model { get; }
    
    public override double GetRegularization() {
        var values = Model.GetWeights().Split(" ", StringSplitOptions.RemoveEmptyEntries);
        var sum = 0d;

        foreach (var value in values)
            if (double.TryParse(value, out var cur)) sum += Math.Pow(cur, 2);

        return sum;
    }
}